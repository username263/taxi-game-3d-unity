using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class CarListViewUI : MonoBehaviour
    {
        [SerializeField]
        TMP_Text coinText;
        [SerializeField]
        CarEntryViewUI[] carEntries;
        [SerializeField]
        TMP_Text pageText;
        [SerializeField]
        Button leftPageButton;
        [SerializeField]
        Button rightPageButton;
        [SerializeField]
        TMP_Text costText;
        [SerializeField]
        Button buyButton;
        [SerializeField]
        Button selectButton;
        [SerializeField]
        Button cancelButton;

        bool wasStarted;
        CarTemplate selectedCar;
        int pageIndex;
        int pageCount;

        void Start()
        {
            var cliMgr = ClientManager.Instance;

            var templateCount = cliMgr.TemplateService.CarTemplates.Count;
            pageCount = templateCount / carEntries.Length;
            if (templateCount % carEntries.Length != 0)
                ++pageCount;

            foreach (var entry in carEntries)
            {
                entry.ClickEvent += (sender, e) =>
                {
                    var entry = sender as CarEntryViewUI;
                    entry.IsSelect = true;
                    selectedCar = entry.Template;
                    var other = carEntries
                        .Where(x => x != entry && x.IsSelect)
                        .FirstOrDefault();
                    if (other != null)
                        other.IsSelect = false;
                    RefreshBuyOrSelect();
                };
            }

            leftPageButton.onClick.AddListener(() =>
            {
                if (pageIndex <= 0)
                    return;
                --pageIndex;
                RefreshEntries();
            });
            rightPageButton.onClick.AddListener(() =>
            {
                if (pageIndex >= carEntries.Length - 1)
                    return;
                ++pageIndex;
                RefreshEntries();
            });

            buyButton.onClick.AddListener(() =>
            {
                _ = ClientManager.Instance.UserService.BuyCar(selectedCar.Id);
                Refresh();
            });
            selectButton.onClick.AddListener(() =>
            {
                if (selectedCar != cliMgr.UserService.User.CurrentCar)
                    cliMgr.UserService.SelectCar(selectedCar.Id);
                GameLogic.Instance.RespawnPlayerCar();
                gameObject.SetActive(false);
            });
            cancelButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });

            wasStarted = true;
            Refresh();
        }

        void OnEnable()
        {
            pageIndex = 0;
            selectedCar = ClientManager.Instance.UserService.User.CurrentCar;
            if (wasStarted)
                Refresh();
        }

        void Refresh()
        {
            coinText.text = ClientManager.Instance.UserService.User.Coin.ToString();
            RefreshEntries();
            RefreshBuyOrSelect();
        }

        void RefreshEntries()
        {
            var cliMgr = ClientManager.Instance;
            pageText.text = $"{pageIndex + 1}/{pageCount}";

            leftPageButton.interactable = pageIndex > 0;
            rightPageButton.interactable = pageIndex < pageCount - 1;

            int tempIndex = pageIndex * carEntries.Length;
            for (int i = 0; i < carEntries.Length; i++)
            {
                if (tempIndex >= cliMgr.TemplateService.CarTemplates.Count)
                {
                    carEntries[i].IsSelect = false;
                    carEntries[i].gameObject.SetActive(false);
                    continue;
                }
                var template = cliMgr.TemplateService.CarTemplates[tempIndex];
                carEntries[i].Template = template;
                carEntries[i].IsSelect = selectedCar == template;
                carEntries[i].gameObject.SetActive(true);
                ++tempIndex;
            }
        }

        void RefreshBuyOrSelect()
        {
            var user = ClientManager.Instance.UserService.User;
            var hasCar = user.Cars.Contains(selectedCar);
            if (hasCar)
            {
                selectButton.gameObject.SetActive(true);
                selectButton.interactable = selectedCar != user.CurrentCar;
                buyButton.gameObject.SetActive(false);
            }
            else
            {
                var enoughCoin = user.Coin >= selectedCar.Cost;
                selectButton.gameObject.SetActive(false);
                buyButton.gameObject.SetActive(true);
                buyButton.interactable = enoughCoin;
                costText.text = selectedCar.Cost.ToString();
                costText.color = enoughCoin ? Color.white : Color.red;
            }   
        }
    }
}