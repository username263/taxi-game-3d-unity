using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class CarVFXController : MonoBehaviour
    {
        [SerializeField]
        TrailRenderer[] brakeLights;
        [SerializeField]
        TrailRenderer[] tireMarks;
        [SerializeField]
        ParticleSystem[] tireSmoke;

        public void EnableBrakeLightsEmitting()
        {
            foreach (var t in brakeLights)
                t.emitting = true;
        }

        public void DisableBrakeLightsEmitting()
        {
            foreach (var t in brakeLights)
                t.emitting = false;
        }

        public void EnableTireMarksEmitting()
        {
            foreach (var t in tireMarks)
                t.emitting = true;
        }

        public void DisableTireMarksEmitting()
        {
            foreach (var t in tireMarks)
                t.emitting = false;
        }

        public void PlayTireSmokes()
        {
            foreach (var p in tireSmoke)
            {
                if (!p.isPlaying)
                    p.Play();
            }
        }

        public void StopTireSmokes()
        {
            foreach (var p in tireSmoke)
            {
                if (p.isPlaying)
                    p.Stop();
            }
        }
    }
}