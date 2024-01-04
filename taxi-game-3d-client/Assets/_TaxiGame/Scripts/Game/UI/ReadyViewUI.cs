using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class ReadyViewUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text coinText;
        [SerializeField]
        Button playButton;
        [SerializeField]
        Button carListButton;

        void Awake()
        {
            UpdateCointext();
        }

        void Start()
        {
            playButton.onClick.AddListener(() =>
            {
                GameUI.Instance.ShowPlayView();
                GameLogic.Instance.PlayGame();
                gameObject.SetActive(false);
            });
            carListButton.onClick.AddListener(() =>
            {
                GameUI.Instance.ShowCarList();
            });
        }

        public void UpdateCointext()
        {
            coinText.text = ClientManager.Instance.UserService.User.Coin.ToString();
        }
    }
}
