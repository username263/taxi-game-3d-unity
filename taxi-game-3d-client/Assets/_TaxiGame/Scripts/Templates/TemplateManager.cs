using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace TaxiGame3D
{
    public class TemplateManager : MonoBehaviour
    {
        public static TemplateManager Instance
        {
            get;
            private set;
        }

        public List<CarTemplate> Cars
        {
            get;
            private set;
        }

        public List<StageTamplate> Stages
        {
            get;
            private set;
        }
        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Cars = Load<CarTemplate>("Templates/Car");
            Stages = Load<StageTamplate>("Templates/Stage");
        }
        List<T> Load<T>(string path)
        {
            var asset = Resources.Load<TextAsset>(path);
            return JsonConvert.DeserializeObject<List<T>>(asset.text);
        }
    }
}