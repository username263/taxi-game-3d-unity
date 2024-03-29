﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TaxiGame3D.Server.Models;

public class UserModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id
    {
        get;
        set;
    }
    public string Nickname
    {
        get;
        set;
    }
    public required string Email
    {
        get;
        set;
    }
    public required string Password
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
    [BsonElement("CurrentCar")]
    [JsonPropertyName("CurrentCar")]
    public string CurrentCarId
    {
        get;
        set;
    }
    [BsonElement("CurrentStage")]
    [JsonPropertyName("CurrentStage")]
    public int CurrentStageIndex
    {
        get;
        set;
    }
    public Dictionary<string, string> DailyCarRewards
    {
        get;
        set;
    }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DailyRewardedAtUtc
    {
        get;
        set;
    }
    public short NumberOfAttendance
    {
        get;
        set;
    }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CoinCollectedAtUtc
    {
        get;
        set;
    }
    public List<string> RouletteCarRewards
    {
        get;
        set;
    }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime RouletteSpunAtUtc
    {
        get;
        set;
    }
}
