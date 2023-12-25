using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TaxiGame3D
{
    public class TemplateService : MonoBehaviour
    {
        string enviroment;
        HttpContext http;

        public List<CarTemplate> CarTemplates
        {
            get;
            private set;
        }

        public List<StageTemplate> StageTemplates
        {
            get;
            private set;
        }

        void Start()
        {
            enviroment = ClientManager.Instance?.Enviroment;
            http = ClientManager.Instance?.Http;
        }

        public async UniTask<bool> LoadAll()
        {
            var res = await http.Get<Dictionary<string, ulong>>("Template/Versions");
            if (!res.Item1.IsSuccess())
            {
                Debug.LogError($"Load template versions failed. - {res.Item1}");
                return false;
            }

            if (!res.Item2.TryGetValue("Car", out var version))
                return false;
            CarTemplates = await Load<CarTemplate>("Car", version);
            if (CarTemplates == null)
            {
                Debug.LogWarning("Load car templates failed.");
                return false;
            }
            for (int i = 0; i < CarTemplates.Count; i++)
                CarTemplates[i].Index = i;

            if (!res.Item2.TryGetValue("Stage", out version))
                return false;
            StageTemplates = await Load<StageTemplate>("Stage", version);
            if (StageTemplates == null)
            {
                Debug.LogWarning("Load stage templates failed.");
                return false;
            }
            for (int i = 0; i < StageTemplates.Count; i++)
                StageTemplates[i].Index = i;

            return true;
        }

        async UniTask<List<T>> Load<T>(string name, ulong remoteVersion)
        {
            var path = $"{enviroment}/TemplateVersions/{name}";
            var versionText = PlayerPrefs.GetString(path, "0");
            ulong.TryParse(versionText, out var localVersion);
            
            if (localVersion >= remoteVersion)
                return LoadFromLocal<T>(name);
            
            var res = await http.Get<List<T>>($"Template/{name}");
            if (!res.Item1.IsSuccess())
            {
                Debug.LogWarning($"Load {name} template failed. - {res.Item1}");
                return res.Item2;
            }
            SaveToLocal(name, res.Item2);
            PlayerPrefs.SetString(path, remoteVersion.ToString());

            return res.Item2;
        }

        List<T> LoadFromLocal<T>(string name)
        {
            try
            {
                var path = $"{Application.persistentDataPath}/{enviroment}/Templates/{name}";
                var content = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<List<T>>(content);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return default;
            }
        }

        void SaveToLocal(string name, object content)
        {
            try
            {
                var directory = $"{Application.persistentDataPath}/{enviroment}/Templates";
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                
                var path = $"{directory}/{name}";
                if (!File.Exists(path))
                    File.Create(path).Close();

                File.WriteAllText(path, JsonConvert.SerializeObject(content));
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public static void ResetTemplateVersions(string enviroment)
        {
            PlayerPrefs.DeleteKey($"{enviroment}/TemplateVersions/Car");
            PlayerPrefs.DeleteKey($"{enviroment}/TemplateVersions/Stage");
        }
    }
}