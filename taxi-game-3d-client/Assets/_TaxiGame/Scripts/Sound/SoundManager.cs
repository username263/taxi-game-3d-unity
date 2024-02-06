using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace TaxiGame3D
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        AudioMixer audioMixer;
        [SerializeField]
        AudioSource bgmSource;
        [SerializeField]
        AudioSource sfxSource;

        public const float maxVolume = 0f;
        public const float minVolume = -80f;

        public static SoundManager Instance
        {
            get;
            private set;
        }

        public float BgmVolume
        {
            get
            {
                audioMixer.GetFloat("BgmVolume", out var value);
                return value;
            }
            set
            {
                var volume = Mathf.Clamp(value, minVolume, maxVolume);
                audioMixer.SetFloat("BgmVolume", volume);
                PlayerPrefs.SetFloat("BgmVolume", volume);
            }
        }

        public float SfxVolume
        {
            get
            {
                audioMixer.GetFloat("SfxVolume", out var value);
                return value;
            }
            set
            {
                var volume = Mathf.Clamp(value, minVolume, maxVolume);
                audioMixer.SetFloat("SfxVolume", volume);
                PlayerPrefs.SetFloat("SfxVolume", volume);
            }
        }

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioMixer.SetFloat("BgmVolume", PlayerPrefs.GetFloat("BgmVolume", maxVolume));
            audioMixer.SetFloat("SfxVolume", PlayerPrefs.GetFloat("SfxVolume", maxVolume));
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }

        public void PlayBgm(AudioClip clip)
        {
            if (clip == null)
                return;
            if (bgmSource.isPlaying && bgmSource.clip == clip)
                return;
            if (bgmSource.clip != clip)
                bgmSource.clip = clip;
            bgmSource.Play();
        }

        public void StopBgm()
        {
            bgmSource.Stop();
        }

        public void PauseBgm()
        {
            bgmSource.Pause();
        }

        public void ResumeBgm()
        {
            bgmSource.UnPause();
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip != null)
                sfxSource.PlayOneShot(clip);
        }

        public static void CreateInstance()
        {
            if (Instance != null)
                return;
            Instantiate(Resources.Load(nameof(SoundManager)));
        }
    }
}