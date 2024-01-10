using Newtonsoft.Json;

namespace TaxiGame3D
{
    public enum TalkType
    {
        Undefined = 0,
        Request = 1,
        Call = 2
    }

    public class TalkTemplate
    {
        [JsonIgnore]
        public int Index
        {
            get;
            set;
        }
        public TalkType Type
        {
            get;
            set;
        }
        public LocalizationTemplate Content
        {
            get;
            set;
        }
    }
}