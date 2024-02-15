using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization;

namespace TaxiGame3D
{
    public class LocalizationTemplate
    {
        [JsonIgnore]
        LocalizedString localizedString;

        public string Table { get; set; }
        public string Key { get; set; }

        public string GetLocalizedString()
        {
            if (localizedString == null)
                localizedString = new LocalizedString(Table, Key);
            return localizedString.GetLocalizedString();
        }

        public string GetLocalizedString(params object[] arguments)
        {
            if (localizedString == null)
                localizedString = new LocalizedString(Table, Key);
            return localizedString.GetLocalizedString(arguments);
        }
    }
}