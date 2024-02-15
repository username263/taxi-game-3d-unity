using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class RewardPopupViewUI : MonoBehaviour
    {
        [SerializeField]
        Image coinImage;
        [SerializeField]
        RawImage carImage;
        [SerializeField]
        UICarManager carManager;
        [SerializeField]
        TMP_Text contentText;
        [SerializeField]
        LocalizedString contentForCoin;
        [SerializeField]
        LocalizedString contentForCar;
        [SerializeField]
        LocalizedString contentForCarCost;
        [SerializeField]
        Button closeButton;

        void Start()
        {
            closeButton.onClick.AddListener(() =>
            {
                carManager.Deselect();
                gameObject.SetActive(false);
            });
        }

        public void Show(int coin)
        {
            coinImage.gameObject.SetActive(true);
            carImage.gameObject.SetActive(false);
            contentText.text = contentForCoin.GetLocalizedString(coin);
            gameObject.SetActive(true);
        }

        public void Show(string carId, bool newCar)
        {
            coinImage.gameObject.SetActive(false);
            carImage.gameObject.SetActive(true);
            carManager.Select(carId);
            var car = ClientManager.Instance.UserService.User.Cars.Find(e => e.Id == carId);
            var carName = car.Name.GetLocalizedString();
            if (newCar)
                contentText.text = contentForCar.GetLocalizedString(carName);
            else
                contentText.text = contentForCarCost.GetLocalizedString(carName, car.Cost);
            gameObject.SetActive(true);
        }
    }
}
