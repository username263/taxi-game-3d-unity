using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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