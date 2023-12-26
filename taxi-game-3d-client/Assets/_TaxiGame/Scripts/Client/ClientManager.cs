using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class ClientManager : MonoBehaviour
    {
        [SerializeField]
        string enviroment = "Development";
        [SerializeField]
        string serverUri;

        public static ClientManager Instance
        {
            get;
            private set;
        }

        public string Enviroment => enviroment;

        public HttpContext Http
        {
            get;
            private set;
        }

        public AuthService AuthService
        {
            get;
            private set;
        }

        public UserService UserService
        {
            get;
            private set;
        }

        public TemplateService TemplateService
        {
            get;
            private set;
        }


        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Http = new(serverUri);
            
            AuthService = gameObject.AddComponent<AuthService>();
            UserService = gameObject.AddComponent<UserService>();
            TemplateService = gameObject.AddComponent<TemplateService>();
        }

        public static void CreateInstance()
        {
            if (Instance != null)
                return;
            Instantiate(Resources.Load(nameof(ClientManager)));
        }

        [ContextMenu("Reset Template Versions")]
        void ResetTemplateVersions() => TemplateService.ResetTemplateVersions(enviroment);

        [ContextMenu("Reset Saved Auth")]
        void ResetSavedAuth() => AuthService.ResetSavedAuth();
    }
}