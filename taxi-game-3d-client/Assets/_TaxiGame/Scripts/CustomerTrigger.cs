using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                GameLogic.Instance.OnCarEnterTrigger(this);
        }
    }
}
