using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    [CreateAssetMenu(menuName = "TaxiGame/ClientSettings")]
    public class ClientSettings : ScriptableObject
    {
        [field: SerializeField]
        public string Enviroment
        {
            get;
            private set;
        }

        [field: SerializeField]
        public string ServerUri
        {
            get;
            private set;
        }
    }
}