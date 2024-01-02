using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class StageProgressViewUI : MonoBehaviour
    {
        [Serializable]
        public class Part
        {
            [SerializeField]
            GameObject container;
            [SerializeField]
            Image stateImage;
            [SerializeField]
            Sprite nonSprite;
            [SerializeField]
            Sprite takeInSprite;
            [SerializeField]
            Sprite passSprite;

            public void SetVisible(bool visible) =>
                container.SetActive(visible);

            public void SetState(int state)
            {
                if (state == 1)
                    stateImage.sprite = takeInSprite;
                else if (state == 2)
                    stateImage.sprite = passSprite;
                else
                    stateImage.sprite = nonSprite;
            }
        }

        [SerializeField]
        LayoutGroup layoutGroup;
        [SerializeField]
        TMP_Text currentStageText;
        [SerializeField]
        Part[] parts;
        [SerializeField]
        Image nextStageImage;
        [SerializeField]
        Sprite nextStageNonSprite;
        [SerializeField]
        Sprite nextStagePassSprite;
        [SerializeField]
        TMP_Text nextStageText;


        void Awake()
        {
            currentStageText.text = "0";
            foreach (var p in parts)
                p.SetState(0);
            nextStageImage.sprite = nextStageNonSprite;
            nextStageText.text = "0";
        }

        void Start()
        {
            currentStageText.text = GameLogic.StageIndex.ToString();
            var customerCount = GameLogic.Instance.CustomerCount;
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i].SetVisible(i < customerCount);
                parts[i].SetState(0);
            }
            nextStageText.text = ClientManager
                .Instance
                .TemplateService
                .StageTemplates
                .Count
                .ToString();
            StartCoroutine(UpdateLayoutGroup());

            GameLogic.Instance.CustomerTakeInEvent += (sender, customerNumber) =>
            {
                parts[customerNumber - 1].SetState(1);
            };
            GameLogic.Instance.CustomerTakeOutEvent += (sender, customerNumber) =>
            {
                parts[customerNumber - 1].SetState(2);
            };
            GameLogic.Instance.GameEndedEvent += (sender, isGoal) =>
            {
                if (isGoal)
                    nextStageImage.sprite = nextStagePassSprite;
            };
        }

        IEnumerator UpdateLayoutGroup()
        {
            layoutGroup.enabled = true;
            yield return new WaitForEndOfFrame();
            layoutGroup.enabled = false;
        }
    }
}