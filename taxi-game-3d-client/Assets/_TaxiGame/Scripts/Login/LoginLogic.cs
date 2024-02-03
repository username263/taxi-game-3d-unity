using Cysharp.Threading.Tasks;
using System.Collections;
using System.Net;
using UnityEngine;

namespace TaxiGame3D
{
    public class LoginLogic : MonoBehaviour
    {
        public static LoginLogic Instance
        {
            get;
            private set;
        }

        void Awake()
        {
            Instance = this;
        }

        IEnumerator Start()
        {
            if (GameUI.Instance != null)
                GameUI.Instance.HideAll();
            
            SoundManager.CreateInstance();
            SoundManager.Instance.StopBgm();

            ClientManager.CreateInstance();
            yield return new WaitForEndOfFrame();
            Loading();
        }

        async void Loading()
        {
            var cliMgr = ClientManager.Instance;

            if (!await cliMgr.TemplateService.LoadAll())
            {
                Debug.LogError("Load templates failed.");
                return;
            }

            var statusCode = await cliMgr.AuthService.Relogin();
            if (!statusCode.IsSuccess())
            {
                Debug.Log("Relogin failed.");
                LoginUI.Instance.SetLoginContainerVisible(true);
                return;
            }

            statusCode = await cliMgr.UserService.Load();
            if (!statusCode.IsSuccess())
            {
                Debug.LogError("Load user failed.");
                LoginUI.Instance.SetLoginContainerVisible(true);
                return;
            }

            GotoGame();
        }

        public async UniTask<HttpStatusCode> Login(string email, string password)
        {
            var statusCode = await ClientManager.Instance.AuthService.Login(new()
            {
                Email = email,
                Password = password
            });
            if (!statusCode.IsSuccess())
                return statusCode;
            return await ClientManager.Instance.UserService.Load();
        }

        public async UniTask<HttpStatusCode> Register(string email, string password)
        {
            var statusCode = await ClientManager.Instance.AuthService.CreateUser(new()
            {
                Email = email,
                Password = password
            });
            if (!statusCode.IsSuccess())
                return statusCode;
            return await ClientManager.Instance.UserService.Load();
        }

        public void GotoGame()
        {
            GameLogic.LoadStage(ClientManager.Instance.UserService.User.CurrentStage.Index);
        }
    }
}