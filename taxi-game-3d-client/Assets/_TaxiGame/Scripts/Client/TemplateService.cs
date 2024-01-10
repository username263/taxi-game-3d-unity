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

        public List<CarTemplate> Cars
        {
            get;
            private set;
        }

        public List<CustomerTemplate> Customers
        {
            get;
            private set;
        }

        public List<DailyRewardTemplate> DailyRewards
        {
            get;
            private set;
        }

        public List<StageTemplate> Stages
        {
            get;
            private set;
        }

        public List<TalkTemplate> Talks
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
            Cars = await Load<CarTemplate>("Car", version);
            if (Cars == null)
            {
                Debug.LogWarning("Load car templates failed.");
                return false;
            }
            for (int i = 0; i < Cars.Count; i++)
                Cars[i].Index = i;

            if (!res.Item2.TryGetValue("Customer", out version))
                return false;
            Customers = await Load<CustomerTemplate>("Customer", version);
            if (Customers == null)
            {
                Debug.LogWarning("Load customer templates failed.");
                return false;
            }

            if (!res.Item2.TryGetValue("DailyReward", out version))
                return false;
            DailyRewards = await Load<DailyRewardTemplate>("DailyReward", version);
            if (DailyRewards == null)
            {
                Debug.LogWarning("Load daily reward templates failed.");
                return false;
            }
            for (int i = 0; i < DailyRewards.Count; i++)
                DailyRewards[i].Index = i;

            if (!res.Item2.TryGetValue("Stage", out version))
                return false;
            Stages = await Load<StageTemplate>("Stage", version);
            if (Stages == null)
            {
                Debug.LogWarning("Load stage templates failed.");
                return false;
            }
            for (int i = 0; i < Stages.Count; i++)
                Stages[i].Index = i;

            if (!res.Item2.TryGetValue("Talk", out version))
                return false;
            Talks = await Load<TalkTemplate>("Talk", version);
            if (Talks == null)
            {
                Debug.LogWarning("Load talk templates failed.");
                return false;
            }
            for (int i = 0; i < Talks.Count; i++)
                Talks[i].Index = i;

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
            PlayerPrefs.DeleteKey($"{enviroment}/TemplateVersions/Customer");
            PlayerPrefs.DeleteKey($"{enviroment}/TemplateVersions/DailyReward");
            PlayerPrefs.DeleteKey($"{enviroment}/TemplateVersions/Stage");
            PlayerPrefs.DeleteKey($"{enviroment}/TemplateVersions/Talk");
        }
    }
}