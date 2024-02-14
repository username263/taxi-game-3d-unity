using PathCreation;
using System;
using UnityEngine;

namespace TaxiGame3D
{
    public class PlayerCar : MonoBehaviour
    {
        /// <summary>
        /// 가속도
        /// </summary>
        [Header("Movement")]
        [SerializeField]
        float acceleration = 1f;
        /// <summary>
        /// 최고 속도
        /// </summary>
        [SerializeField]
        float maxSpeed = 5f;
        /// <summary>
        /// 제동력
        /// </summary>
        [SerializeField]
        float brakeForce = 1f;

        VertexPath path;

        const float minSpeed = 0.5f;
        float speed = minSpeed;

        Rigidbody rb;
        CarVFXController vfxController;
        CarSFXController sfxController;

        public bool IsEnableMoving
        {
            get;
            set;
        }

        public float Movement
        {
            get;
            private set;
        }

        public bool IsArrive => Movement >= path.length;

        [field: Header("Points")]
        [field: SerializeField]
        public Transform LeftPoint
        {
            get;
            private set;
        }

        [field: SerializeField]
        public Transform RightPoint
        {
            get;
            private set;
        }

        public event EventHandler OnCrashedEvent;
        public event EventHandler OnArriveEvent;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            vfxController = GetComponent<CarVFXController>();
            sfxController = GetComponent<CarSFXController>();
        }

        void Start()
        {
            GameLogic.Instance.CustomerTakeInEvent += (sender, numOfCustomers) =>
            {
                sfxController.PlayDoorSfx();
            };
            GameLogic.Instance.CustomerTakeOutEvent += (sender, numOfCustomers) =>
            {
                sfxController.PlayDoorSfx();
                sfxController.PlayIncomeSfx();
            };
            GetComponentInChildren<TriggerInvoker>().TriggerEnteredEvent += (sender, other) =>
            {
                if (other.gameObject.CompareTag("NpcCar"))
                    sfxController.PlayHornSfx();
            };
        }

        void Update()
        {
            if (!IsEnableMoving)
                return;

            Movement += Time.deltaTime * speed;
            rb.MovePosition(path.GetPointAtDistance(Movement, EndOfPathInstruction.Stop));
            rb.MoveRotation(path.GetRotationAtDistance(Movement, EndOfPathInstruction.Stop));

            if (IsArrive)
                OnArriveEvent?.Invoke(this, EventArgs.Empty);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("NpcCar"))
            {
                sfxController.PlayCrashSfx();
                var dir = (collision.rigidbody.position - rb.position).normalized;
                dir.y = 0.1f;
                collision.gameObject.GetComponent<Rigidbody>().AddForce(dir * 500);
                collision.gameObject.GetComponent<Rigidbody>().AddTorque(Vector3.up * 100);
                OnCrashedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetPath(VertexPath path)
        {
            this.path = path;
            Movement = 0f;
            rb.position = path.GetPoint(0);
            rb.rotation = path.GetRotation(0f, EndOfPathInstruction.Stop);
        }

        public void PlayMoving()
        {
            IsEnableMoving = true;
            speed = minSpeed;
        }

        public void StopMoving()
        {
            IsEnableMoving = false;
            speed = 0f;
            vfxController.DisableBrakeLightsEmitting();
            vfxController.DisableTireMarksEmitting();
            vfxController.StopTireSmokes();
            sfxController.StopBrakeSfx();
        }

        public void PressAccel()
        {
            if (IsEnableMoving)
            {
                speed = Mathf.Min(speed + Time.deltaTime * acceleration, maxSpeed);
                vfxController.DisableBrakeLightsEmitting();
                vfxController.DisableTireMarksEmitting();
                vfxController.StopTireSmokes();
                sfxController.StopBrakeSfx();
            }
        }

        public void PressBrake()
        {
            if (IsEnableMoving)
            {
                speed = Mathf.Max(speed - Time.deltaTime * brakeForce, minSpeed);
                if (speed > minSpeed)
                {
                    vfxController.EnableBrakeLightsEmitting();
                    vfxController.EnableTireMarksEmitting();
                    vfxController.PlayTireSmokes();
                    sfxController.PlayBrakeSfx();
                }
                else
                {
                    vfxController.DisableBrakeLightsEmitting();
                    vfxController.DisableTireMarksEmitting();
                    vfxController.StopTireSmokes();
                    sfxController.StopBrakeSfx();
                }
            }
        }

        public Transform SelectNearestPoint(Vector3 poisition)
        {
            var left = (LeftPoint.position - poisition).sqrMagnitude;
            var right = (RightPoint.position - poisition).sqrMagnitude;
            return left < right ? LeftPoint : RightPoint;
        }
    }
}
