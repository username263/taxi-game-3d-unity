using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class UserService : MonoBehaviour
    {
        HttpContext http;

        void Start()
        {
            http = GetComponent<ClientManager>()?.Http;
        }
    }
}
