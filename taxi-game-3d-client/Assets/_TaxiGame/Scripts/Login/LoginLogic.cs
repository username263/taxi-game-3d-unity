using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaxiGame3D
{
    public class LoginLogic : MonoBehaviour
    {
        IEnumerator Start()
        {
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

            var email = "test@test.com";
            var password = "12345678";

            var statusCode = await cliMgr.AuthService.Relogin();
            if (!statusCode.IsSuccess())
            {
                statusCode = await cliMgr.AuthService.Login(new()
                {
                    Email = email,
                    Password = password
                });
            }
            if (!statusCode.IsSuccess())
            {
                statusCode = await cliMgr.AuthService.CreateUser(new()
                {
                    Email = email,
                    Password = password
                });
            }
            if (!statusCode.IsSuccess())
            {
                Debug.LogError("Login and CreateUser failed.");
                return;
            }

            statusCode = await cliMgr.UserService.Load();
            if (!statusCode.IsSuccess())
            {
                Debug.LogError("Load user failed.");
                return;
            }

            SceneManager.LoadScene(cliMgr.UserService.User.CurrentStage.SceneName);
        }
    }
}