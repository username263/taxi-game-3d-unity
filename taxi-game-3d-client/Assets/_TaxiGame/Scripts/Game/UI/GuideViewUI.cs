using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class GuideViewUI : MonoBehaviour, IPointerDownHandler
    {
        [Serializable]
        public class Guide
        {
            [SerializeField]
            string textString;
            [SerializeField]
            TMP_Text text;
        }

        [SerializeField]
        Guide[] guides;

        public void OnPointerDown(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }
    }
}