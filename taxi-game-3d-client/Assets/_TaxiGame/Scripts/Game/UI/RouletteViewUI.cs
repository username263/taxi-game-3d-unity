using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class RouletteViewUI : MonoBehaviour
    {
        [SerializeField]
        Button spinButton;
        [SerializeField]
        Button closeButton;
        [SerializeField]
        Transform spinner;
        [SerializeField]
        Image[] rewardIcons;
        Tween tween;

        void Start()
        {
            spinButton.onClick.AddListener(async () =>
            {
                spinButton.interactable = false;
                closeButton.interactable = false;
                var userService = ClientManager.Instance.UserService;
                var res = await userService.SpinRoulette();
                closeButton.interactable = true;
                var curAngle = 0f;
                var targetAngle = CalcSpinnerAngle(res.index);
                tween = DOTween
                    .To(() => curAngle, x => curAngle = x, targetAngle, 3f)
                    .SetEase(Ease.OutQuart)
                    .OnUpdate(() =>
                    {
                        spinner.rotation = Quaternion.Euler(0, 0, curAngle);
                    })
                    .OnComplete(() =>
                    {
                        GameUI.Instance.ShowRewardPopup(
                            userService.User.RouletteCarRewards[res.index].Id,
                            res.newCar
                        );
                        tween = null;
                        GameUI.Instance.Refresh();
                    });
            });
            closeButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }

        void OnEnable()
        {
            var user = ClientManager.Instance.UserService.User;
            spinButton.interactable = DateTime.UtcNow > user.RouletteSpunAtUtc.AddDays(1);
            if (spinButton.interactable)
                spinner.rotation = Quaternion.identity;
            for (int i = 0; i < rewardIcons.Length; i++)
            {
                if (i >= user.RouletteCarRewards.Count)
                {
                    rewardIcons[i].gameObject.SetActive(false);
                    continue;
                }
                rewardIcons[i].sprite = user.RouletteCarRewards[i].Icon;
                rewardIcons[i].gameObject.SetActive(rewardIcons[i].sprite != null);
            }
        }

        void OnDisable()
        {
            if (tween != null && tween.IsPlaying())
                tween.Complete(false);
        }

        float CalcSpinnerAngle(int index)
        {
            return 360f * 5f + 30f + (index * 60f);
        }
    }
}