using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        [Range(-1, 1)]
        float x;
        [SerializeField]
        [Range(-1, 1)]
        float y;
        [SerializeField]
        [Range(-1, 1)]
        float z;
        [Space]
        [SerializeField]
        float speed = 5f;

        void Update()
        {
            transform.Rotate(new Vector3(x, y, z) * speed * Time.deltaTime);
        }
    }
}