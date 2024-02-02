using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.PackageManager;

namespace TaxiGame3D
{
    public class DailyRewardEntryViewUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        TMP_Text dayText;
        [SerializeField]
        Image iconImage;
        [SerializeField]
        TMP_Text amountText;
        [SerializeField]
        GameObject taken;
        [SerializeField]
        GameObject highlight;

        DailyRewardTemplate template;

        public DailyRewardTemplate Template
        {
            get => template;
            set
            {
                template = value;
                if (enabled)
                    Refresh();
            }
        }

        void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            if (template == null)
            {
                dayText.text = "0";
                iconImage.sprite = null;
                iconImage.gameObject.SetActive(false);
                amountText.text = null;
                taken.SetActive(false);
                highlight.SetActive(false);
            }

            var user = ClientManager.Instance.UserService.User; 
            var day = template.Index + 1;
            dayText.text = day.ToString();
            iconImage.sprite = template.Type == DailyRewardType.Coin ?
                template.Icon :
                user.DailyCarRewards[day - 1].Icon;
            iconImage.gameObject.SetActive(iconImage.sprite != null);
            amountText.text = template.Type == DailyRewardType.Coin ?
                template.Amount.ToString() :
                null;
            taken.SetActive(day <= user.NumberOfAttendance);
            highlight.SetActive(day == user.NumberOfAttendance + 1);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            var gameUi = GameUI.Instance;
            var userService = ClientManager.Instance.UserService;

            if (!userService.CheckEnableAttendance())
                return;

            var rewardCar = userService.User.DailyCarRewards[Template.Index];
            var newCar = !userService.User.Cars.Contains(rewardCar);

            userService.Attendance();
            if (Template.Type == DailyRewardType.Coin)
                gameUi.ShowRewardPopup(Template.Amount);
            else if (Template.Type == DailyRewardType.Car)
                gameUi.ShowRewardPopup(rewardCar.Id, newCar);
                
            gameUi.Refresh();
        }
    }
}