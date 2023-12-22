using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Net;
using UnityEngine;

namespace TaxiGame3D
{
    public class AuthService : MonoBehaviour
    {
        const string AuthHeaderKey = "Authorization";
        const string AuthTypeKey = "AuthType";
        const string AuthIdKey = "AuthId";
        const string AuthPasswordKey = "AuthPassword";

        HttpContext http;
        UserService userService;
        DateTime tokenExpireUtc;

        public bool WasLoggedIn => http.Headers.ContainsKey(AuthHeaderKey);

        public event EventHandler TokenExpired;


        void Start()
        {
            http = ClientManager.Instance?.Http;
            userService = ClientManager.Instance?.UserService;
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause)
                return;

            if (!WasLoggedIn)
                return;

            if (tokenExpireUtc >= DateTime.UtcNow)
            {
                http.Headers.Remove(AuthHeaderKey);
                TokenExpired?.Invoke(this, EventArgs.Empty);
                return;
            }

            StartCoroutine(AutoRefreshToken());
        }

        public async UniTask<HttpStatusCode> Relogin()
        {
            var type = (AuthType)PlayerPrefs.GetInt(AuthTypeKey);
            if (type == AuthType.Email)
            {
                return await Login(new LoginWithEmailRequest
                {
                    Email = PlayerPrefs.GetString(AuthIdKey),
                    Password = PlayerPrefs.GetString(AuthPasswordKey)
                });
            }
            return HttpStatusCode.Unauthorized;
        }

        public async UniTask<HttpStatusCode> Login(LoginWithEmailRequest request)
        {
            var res = await http.Post<LoginResponse>("Auth/LoginEmail", request);
            if (!res.Item1.IsSuccess())
            {
                Debug.LogWarning($"Login with email failed. - {res.Item1}");
                http.Headers.Remove(AuthHeaderKey);
                return res.Item1;
            }
            http.Headers[AuthHeaderKey] = res.Item2.BearerToken;
            SaveAuth(AuthType.Email, request.Email, request.Password);
            StartCoroutine(AutoRefreshToken());
            return res.Item1;
        }

        public async UniTask<HttpStatusCode> CreateUser(LoginWithEmailRequest request)
        {
            var res = await http.Post<LoginResponse>("Auth/CreateEmail", request);
            if (!res.Item1.IsSuccess())
            {
                Debug.LogWarning($"Ceate with email failed. - {res.Item1}");
                http.Headers.Remove(AuthHeaderKey);
                return res.Item1;
            }
            http.Headers[AuthHeaderKey] = res.Item2.BearerToken;
            SaveAuth(AuthType.Email, request.Email, request.Password);
            StartCoroutine(AutoRefreshToken());
            return res.Item1;
        }

        public async void RefreshToken()
        {
            var res = await http.Get<LoginResponse>("Auth/RefreshToken");
            if (!res.Item1.IsSuccess())
            {
                Debug.LogWarning($"Refresh token failed. - {res.Item1}");
                http.Headers.Remove(AuthHeaderKey);
                TokenExpired?.Invoke(this, EventArgs.Empty);
                return;
            }

            http.Headers[AuthHeaderKey] = res.Item2.BearerToken;
            tokenExpireUtc = res.Item2.ExpireUtc;

            StartCoroutine(AutoRefreshToken());
        }

        public void Logout()
        {
            http.Headers.Remove(AuthHeaderKey);
            userService.Clear();
            PlayerPrefs.DeleteKey(AuthTypeKey);
            PlayerPrefs.DeleteKey(AuthIdKey);
            PlayerPrefs.DeleteKey(AuthPasswordKey);
        }

        void SaveAuth(AuthType type, string id, string pw)
        {
            PlayerPrefs.SetInt(AuthTypeKey, (int)type);
            PlayerPrefs.SetString(AuthIdKey, id);
            if (type == AuthType.Email)
                PlayerPrefs.SetString(AuthPasswordKey, pw);
        }

        IEnumerator AutoRefreshToken()
        {
            // 토큰이 만료되기 1분전에 토큰을 갱신하도록 처리
            var ts = tokenExpireUtc - DateTime.UtcNow.AddMinutes(1);
            yield return new WaitForSecondsRealtime((float)ts.TotalSeconds);
            RefreshToken();   
        }
    }
}