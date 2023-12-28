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
        GameObject eventSystem;


        public static GameUI Instance
        {
            get;
            private set;
        }

        void Awake()
        {
            Instance = this;
        }

        public void StartForGame()
        {
            readyView.gameObject.SetActive(true);
            playView.gameObject.SetActive(false);
            eventSystem.SetActive(true);
        }

        public void HideAll()
        {
            readyView.gameObject.SetActive(false);
            playView.gameObject.SetActive(false);
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