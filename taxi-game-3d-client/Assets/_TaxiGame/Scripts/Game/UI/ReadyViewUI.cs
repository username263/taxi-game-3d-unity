using System;
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
        [SerializeField]
        Button attendanceButton;
        [SerializeField]
        Image collectProgress;
        [SerializeField]
        TMP_Text collectAmountText;
        [SerializeField]
        Button collectButton;

        void Awake()
        {
            coinText.text = "0";
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
            attendanceButton.onClick.AddListener(() =>
            {
                GameUI.Instance.ShowDailyRewardList();
            });
            collectButton.onClick.AddListener(() =>
            {
                var userService = ClientManager.Instance.UserService;
                var collectMinutes = (DateTime.UtcNow - userService.User.CoinCollectedAtUtc).TotalMinutes;
                if (collectMinutes < 1.0)
                    return;
                userService.CollectCoin();
                Refresh();
            });
        }

        void OnEnable()
        {
            Refresh();
        }

        void Update()
        {
            var user = ClientManager.Instance.UserService.User;
            var collectMinutes = (DateTime.UtcNow - user.CoinCollectedAtUtc).TotalMinutes;
            collectButton.interactable = collectMinutes < 1.0;
            if (collectMinutes >= user.CurrentStage.MaxCollect)
            {
                collectAmountText.text = user.CurrentStage.MaxCollect.ToString();
                collectProgress.fillAmount = 1f;
                return;
            }
            var collectAmount = Math.Ceiling(collectMinutes);
            collectAmountText.text = collectAmount.ToString();
            collectProgress.fillAmount = (float)(collectMinutes - collectAmount);
        }

        public void Refresh()
        {
            coinText.text = ClientManager.Instance.UserService.User.Coin.ToString();
        }
    }
}
