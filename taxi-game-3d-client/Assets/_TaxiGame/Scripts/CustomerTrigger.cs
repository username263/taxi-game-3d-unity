using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TaxiGame3D
{
    public class CustomerTrigger : MonoBehaviour
    {
        /// <summary>
        /// ������ �մ��� �����ϴ� ��
        /// ������ �մ��� �̵��ϴ� ��
        /// </summary>
        [field: SerializeField]
        public Transform CustomerPoint
        {
            get;
            private set;
        }

        public event EventHandler OnPlayerEntered;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                OnPlayerEntered?.Invoke(this, EventArgs.Empty);
        }
    }
}
