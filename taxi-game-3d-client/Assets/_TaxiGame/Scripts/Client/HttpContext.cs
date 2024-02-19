using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace TaxiGame3D
{
    public class HttpContext
    {
        public string BaseUri
        {
            get;
            private set;
        }

        public Dictionary<string, string> Headers
        {
            get;
            private set;
        } = new();

        public HttpContext(string baseUri)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
                throw new InvalidOperationException("Invalid base address.");

            if (baseUri.Last() == '/')
                BaseUri = baseUri;
            else
                BaseUri = $"{baseUri}/";
        }

        public async UniTask<HttpStatusCode> Get(string subUri)
        {
            using (var req = UnityWebRequest.Get($"{BaseUri}{subUri}"))
            {
                try
                {
                    foreach (var h in Headers)
                        req.SetRequestHeader(h.Key, h.Value);
                    await req.SendWebRequest();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return (HttpStatusCode)req.responseCode;
            }
        }

        public async UniTask<(HttpStatusCode, T)> Get<T>(string subUri)
        {
            using (var req = UnityWebRequest.Get($"{BaseUri}{subUri}"))
            {
                try
                {
                    foreach (var h in Headers)
                        req.SetRequestHeader(h.Key, h.Value);
                    await req.SendWebRequest();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return (
                    (HttpStatusCode)req.responseCode,
                    FromJsonSafety<T>(req.downloadHandler.text)
                );
            }
        }

        public async UniTask<HttpStatusCode> Post(string subUri, object data)
        {
            using (var req = UnityWebRequest.Post($"{BaseUri}{subUri}", ToJsonSafety(data), "application/json"))
            {
                try
                {
                    foreach (var h in Headers)
                        req.SetRequestHeader(h.Key, h.Value);
                    await req.SendWebRequest();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return (HttpStatusCode)req.responseCode;
            }
        }

        public async UniTask<(HttpStatusCode, T)> Post<T>(string subUri, object data)
        {
            using (var req = UnityWebRequest.Post($"{BaseUri}{subUri}", ToJsonSafety(data), "application/json"))
            {
                try
                {
                    foreach (var h in Headers)
                        req.SetRequestHeader(h.Key, h.Value);
                    await req.SendWebRequest();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return (
                    (HttpStatusCode)req.responseCode,
                    FromJsonSafety<T>(req.downloadHandler.text)
                );
            }
        }

        public async UniTask<HttpStatusCode> Put(string subUri, object data)
        {
            using (var req = UnityWebRequest.Put($"{BaseUri}{subUri}", ToJsonSafety(data)))
            {
                try
                {
                    foreach (var h in Headers)
                        req.SetRequestHeader(h.Key, h.Value);
                    req.SetRequestHeader("Content-Type", "application/json");
                    await req.SendWebRequest();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return (HttpStatusCode)req.responseCode;
            }
        }

        public async UniTask<(HttpStatusCode, T)> Put<T>(string subUri, object data)
        {
            using (var req = UnityWebRequest.Put($"{BaseUri}{subUri}", ToJsonSafety(data)))
            {
                try
                {
                    foreach (var h in Headers)
                        req.SetRequestHeader(h.Key, h.Value);
                    req.SetRequestHeader("Content-Type", "application/json");
                    await req.SendWebRequest();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return (
                    (HttpStatusCode)req.responseCode,
                    FromJsonSafety<T>(req.downloadHandler.text)
                );
            }
        }

        public async UniTask<HttpStatusCode> Delete(string subUri)
        {
            using (var req = UnityWebRequest.Delete($"{BaseUri}{subUri}"))
            {
                try
                {
                    foreach (var h in Headers)
                        req.SetRequestHeader(h.Key, h.Value);
                    await req.SendWebRequest();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return (HttpStatusCode)req.responseCode;
            }
        }

        public async UniTask<(HttpStatusCode, T)> Delete<T>(string subUri)
        {
            using (var req = UnityWebRequest.Delete($"{BaseUri}{subUri}"))
            {
                try
                {
                    foreach (var h in Headers)
                        req.SetRequestHeader(h.Key, h.Value);
                    await req.SendWebRequest();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                return (
                    (HttpStatusCode)req.responseCode,
                    FromJsonSafety<T>(req.downloadHandler.text)
                );
            }
        }

        string ToJsonSafety(object data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch
            {
                return null;
            }
        }

        T FromJsonSafety<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(
                    json,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    }
                );
            }
            catch
            {
                return default;
            }
        }
    }
}