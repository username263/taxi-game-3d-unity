namespace TaxiGame3D.Server.Templates;

public enum DailyRewardType
{
    Undefined = 0,
    Coin = 1,
    Car = 2
}

public class DailyRewardTemplate
{
    public DailyRewardType Type
    {
        get;
        set;
    }

    public int Amount
    {
        get;
        set;
    }
}
