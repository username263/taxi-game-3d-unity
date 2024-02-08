using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class SimpleToggle : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        Image target;
        [SerializeField]
        Sprite on;
        [SerializeField]
        Sprite off;
        [SerializeField]
        AudioClip clickSfx;

        bool value;

        public bool Value
        {
            get => value;
            set => SetValue(value, true);
        }

        public event EventHandler<bool> ValueChangedEvent;

        void OnEnable()
        {
            target.sprite = value ? on : off;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySfx(clickSfx);
            Value = !Value;
        }

        public void SetValue(bool value, bool callEvent)
        {
            this.value = value;
            target.sprite = value ? on : off;
            if (Application.isPlaying)
            {
                if (callEvent)
                    ValueChangedEvent.Invoke(this, value);
            }
        }
    }
}