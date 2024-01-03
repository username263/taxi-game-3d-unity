using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class PlayViewUI : MonoBehaviour
    {
        [SerializeField]
        StageProgressViewUI stageProgressView;

        public void OnGameLoaded()
        {
            stageProgressView.Init();
        }
    }
}