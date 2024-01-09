using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaxiGame3D
{
    public class PlayViewUI : MonoBehaviour
    {
        [SerializeField]
        StageProgressViewUI stageProgressView;
        [SerializeField]
        TalkViewUI talkView;
        [SerializeField]
        GuideViewUI guideView;
        [SerializeField]
        ResultViewUI resultView;


        void OnEnable()
        {
            talkView.gameObject.SetActive(false);
            guideView.gameObject.SetActive(true);
        }

        public void OnGameLoaded()
        {
            stageProgressView.Init();
            resultView.gameObject.SetActive(false);

            GameLogic.Instance.CustomerTakeInEvent += (sender, customer) =>
            {
                StopCoroutine("ShowTalkView");
                talkView.gameObject.SetActive(false);
                
                var ci = GameLogic.Instance.CustomerManager.CurrentCustomerIndex;
                var ti = ClientManager.Instance.TemplateService.Talks
                    .Where(e => e.Type == TalkType.Request)
                    .OrderBy(e => Random.value)
                    .FirstOrDefault()?.Index ?? 0;
                StartCoroutine(ShowTalkView(0f, ci, ti));
            };
            GameLogic.Instance.CustomerTakeOutEvent += (sender, customer) =>
            {
                StopCoroutine("ShowTalkView");
                talkView.gameObject.SetActive(false);

                var ci = GameLogic.Instance.CustomerManager.NextCustomerIndex;
                if (ci < 0)
                    return;

                var ti = ClientManager.Instance.TemplateService.Talks
                    .Where(e => e.Type == TalkType.Call)
                    .OrderBy(e => Random.value)
                    .FirstOrDefault()?.Index ?? 0;
                StartCoroutine(ShowTalkView(2f, ci, ti));
            };
            GameLogic.Instance.GameEndedEvent += (sender, isGoal) =>
            {
                resultView.gameObject.SetActive(true);
            };
        }

        IEnumerator ShowTalkView(float delay, int customerIndex, int talkIndex)
        {
            talkView.Show(customerIndex, talkIndex);
            if (delay > 0)
                yield return new WaitForSecondsRealtime(delay);
            else
                yield return new WaitForEndOfFrame();
            talkView.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(2f);
            talkView.gameObject.SetActive(false);
        }
    }
}