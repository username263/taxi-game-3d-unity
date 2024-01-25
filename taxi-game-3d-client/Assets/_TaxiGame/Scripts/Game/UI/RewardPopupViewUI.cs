using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class RewardPopupViewUI : MonoBehaviour
    {
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TMP_Text contentText;
        [SerializeField]
        Button closeButton;

        void Start()
        {
            closeButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }

        void OnEnable()
        {

        }
    }
}
