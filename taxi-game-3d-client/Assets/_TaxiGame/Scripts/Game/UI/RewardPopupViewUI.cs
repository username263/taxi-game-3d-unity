using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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
            contentText.text = $"Gain {coin}";
            gameObject.SetActive(true);
        }

        public void Show(string carId, bool newCar)
        {
            coinImage.gameObject.SetActive(false);
            carImage.gameObject.SetActive(true);
            carManager.Select(carId);
            if (newCar)
            {
                contentText.text = $"Gain {carId}";
            }
            else
            {
                var car = ClientManager.Instance.UserService.User.Cars.Find(e => e.Id == carId);
                contentText.text = $"{carId} is already exit. Gain {car.Cost}";
            }
            gameObject.SetActive(true);
        }
    }
}
