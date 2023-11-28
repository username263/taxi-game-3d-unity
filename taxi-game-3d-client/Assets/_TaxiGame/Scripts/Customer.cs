using System;
using UnityEngine;

namespace TaxiGame3D
{
    public class Customer : MonoBehaviour
    {
        [SerializeField]
        float moveSpeed = 10f;

        Rigidbody rb;
        Animator animator;
        bool isMoving;
        Vector3 destination;

        public bool IsMoving
        {
            get => isMoving;
            private set
            {
                isMoving = value;
                animator.SetBool("IsMoving", isMoving);
            }
        }

        public event EventHandler OnTakeIn;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            StopMove();
        }

        void Update()
        {
            if (!IsMoving)
                return;

            if (Vector3.Distance(rb.position, destination) <= 0.01f)
            {
                IsMoving = false;
                return;
            }

            var pos = Vector3.MoveTowards(rb.position, destination, Time.deltaTime * moveSpeed);
            var rot = Quaternion.LookRotation(destination - rb.position);
            rb.Move(pos, rot);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                OnTakeIn?.Invoke(this, EventArgs.Empty);
        }

        public void MoveTo(Vector3 destination)
        {
            this.destination = destination;
            IsMoving = true;
        }

        public void StopMove()
        {
            IsMoving = false;
            destination = rb.position;
        }
    }
}