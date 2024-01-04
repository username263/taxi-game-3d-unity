using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class CarEntryViewUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        Image iconImage;
        [SerializeField]
        GameObject selected;

        CarTemplate template;
        bool isSelected;

        public CarTemplate Template
        {
            get => template;
            set
            {
                template = value;
                if (enabled)
                    Refresh();
            }
        }

        public bool IsSelect
        {
            get => isSelected;
            set
            {
                isSelected = value;
                selected.SetActive(isSelected);
            }
        }

        public bool HasCar
        {
            get
            {
                if (template == null)
                    return false;
                return ClientManager.Instance
                    .UserService
                    .User
                    .Cars
                    .Contains(template);
            }
        }


        public event EventHandler ClickEvent;

        void OnEnable()
        {
            Refresh();
        }

        void Refresh()
        {
            if (template == null)
            {
                iconImage.gameObject.SetActive(false);
                return;
            }
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = template.Icon;
            iconImage.color = HasCar ? Color.white : new Color(0.4f, 0.4f, 0.4f, 0.8f);
            selected.SetActive(isSelected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            ClickEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}