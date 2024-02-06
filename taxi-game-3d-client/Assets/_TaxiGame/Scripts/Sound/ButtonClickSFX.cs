using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TaxiGame3D
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSFX : MonoBehaviour
    {
        [SerializeField]
        AudioClip clip;

        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySfx(clip);
            });
        }
    }
}