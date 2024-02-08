using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class CarSFXController : MonoBehaviour
    {
        [SerializeField]
        AudioClip[] brakeSfx;
        [SerializeField]
        AudioClip[] doorSfx;
        [SerializeField]
        AudioClip[] incomeSfx;
        [SerializeField]
        AudioClip[] crashSfx;
        [SerializeField]
        AudioClip[] hornSfx;

        AudioSource brakeSfxSource;
        AudioSource hornSfxSource;

        public void PlayBrakeSfx()
        {
            if (brakeSfx.Length == 0)
                return;
            if (brakeSfxSource == null)
            {
                brakeSfxSource = SoundManager.Instance.CreateSfxSource("BrakeSFX", transform);
                brakeSfxSource.loop = true;
                brakeSfxSource.transform.localPosition = Vector3.zero;
            }
            if (brakeSfxSource.isPlaying)
                return;
            brakeSfxSource.clip = brakeSfx[Random.Range(0, brakeSfx.Length)];
            brakeSfxSource.Play();
        }

        public void StopBrakeSfx()
        {
            if (brakeSfxSource == null || !brakeSfxSource.isPlaying)            
                return;
            brakeSfxSource.Stop();
        }

        public void PlayDoorSfx()
        {
            if (doorSfx.Length == 0)
                return;
            SoundManager.Instance.PlaySfx(doorSfx[Random.Range(0, doorSfx.Length)]);
        }

        public void PlayIncomeSfx()
        {
            if (incomeSfx.Length == 0)
                return;
            SoundManager.Instance.PlaySfx(incomeSfx[Random.Range(0, incomeSfx.Length)]);
        }

        public void PlayCrashSfx()
        {
            if (crashSfx.Length == 0)
                return;
            SoundManager.Instance.PlaySfx(crashSfx[Random.Range(0, crashSfx.Length)]);
        }

        public void PlayHornSfx()
        {
            if (hornSfx.Length == 0)
                return;
            if (hornSfxSource == null)
            {
                hornSfxSource = SoundManager.Instance.CreateSfxSource("HornSFX", transform);
                hornSfxSource.transform.localPosition = Vector3.zero;
            }
            if (hornSfxSource.isPlaying)
                return;
            hornSfxSource.clip = hornSfx[Random.Range(0, hornSfx.Length)];
            hornSfxSource.Play();
        }

        public void StopHornSfx()
        {
            if (hornSfxSource == null)
                return;
            hornSfxSource.Stop();
        }
    }
}