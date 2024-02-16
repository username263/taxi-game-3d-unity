using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TaxiGame3D
{
    public class SettingsViewUI : MonoBehaviour
    {
        [SerializeField]
        SimpleToggle bgmToggle;
        [SerializeField]
        SimpleToggle sfxToggle;
        [SerializeField]
        TMP_Dropdown languageDropdown;
        [SerializeField]
        Button logoutButton;
        [SerializeField]
        Button closeButton;

        void Start()
        {
            bgmToggle.ValueChangedEvent += (sender, value) =>
            {
                SoundManager.Instance.BgmVolume = value ? SoundManager.maxVolume : SoundManager.minVolume;
            };
            sfxToggle.ValueChangedEvent += (sender, value) =>
            {
                SoundManager.Instance.SfxVolume = value ? SoundManager.maxVolume : SoundManager.minVolume;
            };
            languageDropdown.ClearOptions();
            languageDropdown.AddOptions(
                LocalizationSettings.AvailableLocales.Locales.Select(
                    l => l.Identifier.CultureInfo.NativeName
                ).ToList()
            );
            languageDropdown.value = LocalizationSettings
                .AvailableLocales
                .Locales
                .IndexOf(LocalizationSettings.SelectedLocale);
            languageDropdown.onValueChanged.AddListener(value =>
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[value];
                if (LocalizationSettings.SelectedLocale == locale)
                    return;
                LocalizationSettings.SelectedLocale = locale;
                PlayerPrefs.SetInt("SelctedLocale", value);
            });
            logoutButton.onClick.AddListener(() =>
            {
                ClientManager.Instance.AuthService.Logout();
                GameUI.Instance.HideAll();
                SceneManager.LoadScene(0);
            });
            closeButton.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }

        void OnEnable()
        {
            bgmToggle.SetValue(SoundManager.Instance.BgmVolume > SoundManager.minVolume, false);
            sfxToggle.SetValue(SoundManager.Instance.SfxVolume > SoundManager.minVolume, false);
        }
    }
}