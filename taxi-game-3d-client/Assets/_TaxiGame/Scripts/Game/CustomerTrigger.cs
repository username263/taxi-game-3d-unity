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
        /// 승차할 손님이 등장하는 곳
        /// 하차한 손님이 이동하는 곳
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
