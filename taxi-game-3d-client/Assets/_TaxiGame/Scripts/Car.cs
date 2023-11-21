using PathCreation;
using System;
using UnityEngine;

namespace TaxiGame3D
{
    public class Car : MonoBehaviour
    {
        /// <summary>
        /// 가속도
        /// </summary>
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

        float speed = 1f;
        float movement = 0f;

        Rigidbody rb;

        public bool IsEnableMoving
        {
            get;
            set;
        }

        public event EventHandler OnArrive;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (!IsEnableMoving)
                return;

            movement += Time.deltaTime * speed;
            rb.MovePosition(path.GetPointAtDistance(movement, EndOfPathInstruction.Stop));
            rb.MoveRotation(path.GetRotationAtDistance(movement, EndOfPathInstruction.Stop));

            if (movement >= path.length)
                OnArrive?.Invoke(this, EventArgs.Empty);
        }

        public void SetPath(VertexPath path)
        {
            this.path = path;
            rb.position = path.GetPointAtDistance(movement, EndOfPathInstruction.Stop);
            rb.rotation = path.GetRotationAtDistance(movement, EndOfPathInstruction.Stop);
        }

        public void PlayMoving()
        {
            IsEnableMoving = true;
            speed = 1f;
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
                speed = Mathf.Max(speed - Time.deltaTime * brakeForce, 1f);
        }
    }
}
