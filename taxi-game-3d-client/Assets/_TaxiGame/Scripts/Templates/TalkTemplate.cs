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
        public TalkType Type { get; set; }
        public LocalizationTemplate Content { get; set; }
    }
}