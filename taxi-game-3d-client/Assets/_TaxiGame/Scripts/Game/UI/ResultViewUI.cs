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
        AudioSource goalSfxSource;

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
            if (isGoal && goalSfxSource != null)
                StartCoroutine(PlayGoalSfx());
        }

        IEnumerator PlayGoalSfx()
        {
            SoundManager.Instance.PauseBgm();
            goalSfxSource.Play();
            yield return new WaitForSeconds(goalSfxSource.clip.length);
            SoundManager.Instance.ResumeBgm();
        }
    }
}
