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

        public event EventHandler OnCrashed;
        public event EventHandler OnArrive;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (!IsEnableMoving)
                return;

            Movement += Time.deltaTime * speed;
            rb.MovePosition(path.GetPointAtDistance(Movement, EndOfPathInstruction.Stop));
            rb.MoveRotation(path.GetRotationAtDistance(Movement, EndOfPathInstruction.Stop));

            if (IsArrive)
                OnArrive?.Invoke(this, EventArgs.Empty);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("NpcCar"))
                OnCrashed?.Invoke(this, EventArgs.Empty);
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
        }

        public void PressAccel()
        {
            if (IsEnableMoving)
                speed = Mathf.Min(speed + Time.deltaTime * acceleration, maxSpeed);
        }

        public void PressBrake()
        {
            if (IsEnableMoving)
                speed = Mathf.Max(speed - Time.deltaTime * brakeForce, minSpeed);
        }

        public Transform SelectNearestPoint(Vector3 poisition)
        {
            var left = (LeftPoint.position - poisition).sqrMagnitude;
            var right = (RightPoint.position - poisition).sqrMagnitude;
            return left < right ? LeftPoint : RightPoint;
        }
    }
}
