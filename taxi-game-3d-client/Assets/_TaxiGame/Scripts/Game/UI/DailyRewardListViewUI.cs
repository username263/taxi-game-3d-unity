using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class DailyRewardListViewUI : MonoBehaviour
    {
        [SerializeField]
        DailyRewardEntryViewUI[] entries;
        [SerializeField]
        Button closeButton;


        void Start()
        {
            closeButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }

        void OnEnable()
        {
            var templates = ClientManager.Instance.TemplateService.DailyRewards;
            for (int i = 0; i < templates.Count; i++)
            {
                entries[i].gameObject.SetActive(true);
                entries[i].Template = templates[i];
            }
            for (int i = templates.Count; i < entries.Length; i++)
                entries[i].gameObject.SetActive(false);
        }

        public void Refresh()
        {
            foreach (var e in entries)
            {
                if (e.gameObject.activeInHierarchy)
                    e.Refresh();
            }    
        }
    }
}