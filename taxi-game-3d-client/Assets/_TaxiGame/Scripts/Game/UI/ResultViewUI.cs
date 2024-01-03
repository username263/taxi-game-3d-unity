using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class ResultViewUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text coinText;
        [SerializeField]
        Button claimButton;

        void Start()
        {
            claimButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                GameLogic.LoadStage(ClientManager.Instance.UserService.User.CurrentStage.Index);
            });
        }

        void OnEnable()
        {
            coinText.text = GameLogic.Instance.RewardedCoin.ToString();
        }

        void OnDisable()
        {
            coinText.text = "0";
        }
    }
}
