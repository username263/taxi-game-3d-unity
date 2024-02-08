using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class TriggerInvoker : MonoBehaviour
    {
        public event EventHandler<Collider> TriggerEnteredEvent;
        public event EventHandler<Collider> TriggerStayingEvent;
        public event EventHandler<Collider> TriggerExitedEvent;

        public void OnTriggerEnter(Collider other) =>
            TriggerEnteredEvent?.Invoke(this, other);

        public void OnTriggerStay(Collider other) =>
            TriggerStayingEvent?.Invoke(this, other);

        public void OnTriggerExit(Collider other) =>
            TriggerExitedEvent?.Invoke(this, other);
    }
}