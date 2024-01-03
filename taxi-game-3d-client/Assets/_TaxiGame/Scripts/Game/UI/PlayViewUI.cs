using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;

namespace TaxiGame3D
{
    public class PlayViewUI : MonoBehaviour
    {
        [SerializeField]
        StageProgressViewUI stageProgressView;
        [SerializeField]
        GuideViewUI guideView;
        [SerializeField]
        ResultViewUI resultView;


        void OnEnable()
        {
            guideView.gameObject.SetActive(true);
        }

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