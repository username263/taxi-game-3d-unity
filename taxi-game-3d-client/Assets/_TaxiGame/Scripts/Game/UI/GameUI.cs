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

        public void ShowReadyView()
        {
            readyView.gameObject.SetActive(true);
            playView.gameObject.SetActive(false);
            carListView.gameObject.SetActive(false);
            eventSystem.SetActive(true);
        }

        public void ShowPlayView()
        {
            readyView.gameObject.SetActive(false);
            playView.gameObject.SetActive(true);
            carListView.gameObject.SetActive(false);
        }

        public void ShowCarList()
        {
            carListView.gameObject.SetActive(true);
        }

        public void HideAll()
        {
            readyView.gameObject.SetActive(false);
            playView.gameObject.SetActive(false);
            carListView.gameObject.SetActive(false);
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