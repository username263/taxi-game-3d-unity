using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField]
        ReadyViewUI readyView;
        [SerializeField]
        PlayViewUI playView;
        [SerializeField]
        CarListViewUI carListView;
        [SerializeField]
        DailyRewardListViewUI dailyRewardListView;
        [SerializeField]
        RouletteViewUI rouletteView;
        [SerializeField]
        RewardPopupViewUI rewardPopupView;
        [SerializeField]
        GameObject eventSystem;


        public static GameUI Instance
        {
            get;
            private set;
        }

        void Awake()
        {
            Instance = this;
            ShowReadyView();
        }

        public void OnGameLoaded()
        {
            playView.OnGameLoaded();
            ShowReadyView();
        }

        public void Refresh()
        {
            if (readyView.gameObject.activeInHierarchy)
                readyView.Refresh();
            if (carListView.gameObject.activeInHierarchy)
                carListView.Refresh();
            if (dailyRewardListView.gameObject.activeInHierarchy)
                dailyRewardListView.Refresh();
        }

        public void ShowReadyView()
        {
            readyView.gameObject.SetActive(true);
            playView.gameObject.SetActive(false);
            carListView.gameObject.SetActive(false);
            dailyRewardListView.gameObject.SetActive(false);
            rouletteView.gameObject.SetActive(false);
            rewardPopupView.gameObject.SetActive(false);
            eventSystem.SetActive(true);
        }

        public void ShowPlayView()
        {
            readyView.gameObject.SetActive(false);
            playView.gameObject.SetActive(true);
            carListView.gameObject.SetActive(false);
            dailyRewardListView.gameObject.SetActive(false);
            rouletteView.gameObject.SetActive(false);
            rewardPopupView.gameObject.SetActive(false);
        }

        public void ShowCarList()
        {
            carListView.gameObject.SetActive(true);
        }

        public void ShowDailyRewardList()
        {
            dailyRewardListView.gameObject.SetActive(true);
        }

        public void ShowRoulette()
        {
            rouletteView.gameObject.SetActive(true);
        }

        public void ShowRewardPopup(int coin)
        {
            rewardPopupView.Show(coin);
        }

        public void ShowRewardPopup(string carId, bool newCar)
        {
            rewardPopupView.Show(carId, newCar);
        }

        public void HideAll()
        {
            readyView.gameObject.SetActive(false);
            playView.gameObject.SetActive(false);
            carListView.gameObject.SetActive(false);
            dailyRewardListView.gameObject.SetActive(false);
            rouletteView.gameObject.SetActive(false);
            rewardPopupView.gameObject.SetActive(false);
            eventSystem.SetActive(false);
        }

        public static void CreateInstance()
        {
            if (Instance != null)
                return;
            Instantiate(Resources.Load(nameof(GameUI)));
        }
    }
}