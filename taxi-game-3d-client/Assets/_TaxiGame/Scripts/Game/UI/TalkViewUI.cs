using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class TalkViewUI : MonoBehaviour
    {
        [SerializeField]
        Image customerIconImage;
        [SerializeField]
        TMP_Text talkContentText;


        public void Show(int customerIndex, int talkIndex)
        {
            var templateService = ClientManager.Instance.TemplateService;
            customerIconImage.sprite = templateService.Customers[customerIndex].Icon;
            talkContentText.text = templateService.Talks[talkIndex].Content.Key;
        }
    }
}