using TaxiGame3D.Server.Models;

namespace TaxiGame3D.Server.DTOs;

public class UserResponse
{
    public string Nickname
    {
        get;
        set;
    }
    public long Coin
    {
        get;
        set;
    }
    public List<string> Cars
    {
        get;
        set;
    }
    public string CurrentCar
    {
        get;
        set;
    }
    public int CurrentStage
    {
        get;
        set;
    }
    public Dictionary<string, string> DailyCarRewards
    {
        get;
        set;
    }
    public DateTime DailyRewardedAt
    {
        get;
        set;
    }
    public short NumberOfAttendance
    {
        get;
        set;
    }
    public DateTime CoinCollectedAt
    {
        get;
        set;
    }
    public List<string> RouletteCarRewards
    {
        get;
        set;
    }
    public DateTime RouletteSpunAt
    {
        get;
        set;
    }

    public UserResponse()
    {
    }

    public UserResponse(UserModel model)
    {
        Nickname = model.Nickname;
        Coin = model.Coin;
        Cars = model.Cars;
        CurrentCar = model.CurrentCarId;
        CurrentStage = model.CurrentStageIndex;
        DailyCarRewards = model.DailyCarRewards;
        DailyRewardedAt = model.DailyRewardedAtUtc;
        NumberOfAttendance = model.NumberOfAttendance;
        CoinCollectedAt = model.CoinCollectedAtUtc;
        RouletteCarRewards = model.RouletteCarRewards;
        RouletteSpunAt = model.RouletteSpunAtUtc;
    }
}
