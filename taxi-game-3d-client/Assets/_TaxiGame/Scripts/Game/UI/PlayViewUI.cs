using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class PlayViewUI : MonoBehaviour
    {
        [SerializeField]
        StageProgressViewUI stageProgressView;
        [SerializeField]
        ResultViewUI resultView;


        public void OnGameLoaded()
        {
            stageProgressView.Init();
            resultView.gameObject.SetActive(false);

            GameLogic.Instance.GameEndedEvent += (sender, isGoal) =>
            {
                resultView.gameObject.SetActive(true);
            };
        }
    }
}