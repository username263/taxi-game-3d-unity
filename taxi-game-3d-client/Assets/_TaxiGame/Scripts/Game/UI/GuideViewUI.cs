using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class GuideViewUI : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        List<TMP_Text> texts;
        [SerializeField]
        Material normalMaterial;
        [SerializeField]
        Material highlightedMaterial;


        void OnEnable()
        {
            if (texts.Count == 0)
            {
                Debug.LogError("Not exist any texts.");
                return;
            }
            StartCoroutine(HighlightText(0));
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            gameObject.SetActive(false);
        }

        IEnumerator HighlightText(int index)
        {
            for (int i = 0; i < texts.Count; i++)
            {
                if (i == index)
                {
                    texts[i].textStyle = texts[i].styleSheet.GetStyle("Guide Highlighted");
                    texts[i].fontMaterial = highlightedMaterial;
                }
                else
                {
                    texts[i].textStyle = texts[i].styleSheet.GetStyle("Normal");
                    texts[i].fontMaterial = normalMaterial;
                }
            }
            yield return new WaitForSecondsRealtime(2f);
            if (gameObject.activeInHierarchy)
                StartCoroutine(HighlightText((index + 1) % texts.Count));
        }
    }
}