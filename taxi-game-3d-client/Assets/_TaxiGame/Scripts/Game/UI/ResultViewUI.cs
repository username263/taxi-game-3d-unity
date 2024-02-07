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
        [SerializeField]
        AudioClip goalSfx;

        void Start()
        {
            claimButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
                GameLogic.LoadStage(ClientManager.Instance.UserService.User.CurrentStage.Index);
            });
        }

        void OnDisable()
        {
            coinText.text = "0";
            SoundManager.Instance.ResumeBgm();
        }

        public void Show(bool isGoal)
        {
            coinText.text = GameLogic.Instance.RewardedCoin.ToString();
            gameObject.SetActive(true);
            if (isGoal)
                StartCoroutine(PlayGoalSfx());
        }

        IEnumerator PlayGoalSfx()
        {
            SoundManager.Instance.PauseBgm();
            SoundManager.Instance.PlaySfx(goalSfx);
            yield return new WaitForSeconds(goalSfx.length);
            SoundManager.Instance.ResumeBgm();
        }
    }
}
