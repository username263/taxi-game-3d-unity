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
        [SerializeField]
        AudioClip callSfx;


        public void Show(int customerIndex, int talkIndex)
        {
            var templateService = ClientManager.Instance.TemplateService;
            var talk = templateService.Talks[talkIndex];
            if (talk.Type == TalkType.Call)
                SoundManager.Instance.PlaySfx(callSfx);
            customerIconImage.sprite = templateService.Customers[customerIndex].Icon;
            talkContentText.text = talk.Content.Key;
            gameObject.SetActive(true);
        }
    }
}