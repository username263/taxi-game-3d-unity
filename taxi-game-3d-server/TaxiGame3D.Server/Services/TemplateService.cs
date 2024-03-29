﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text.Json;
using System.Text.Json.Nodes;
using TaxiGame3D.Server.Models;
using TaxiGame3D.Server.Templates;

namespace TaxiGame3D.Server.Services;

public class TemplateService
{
    readonly IMongoCollection<TemplateVersionModel> versions;
    readonly IMongoCollection<TemplateModel> templates;

    List<CarTemplate>? cars;
    List<DailyRewardTemplate>? dailyRewards;
    List<StageTemplate>? stages;

    public TemplateService(DatabaseContext context)
    {
        versions = context.TemplateVersions;
        templates = context.Templates;
    }

    public async Task<ulong> Update(string name, JsonArray datas)
    {
        switch (name)
        {
            case "Car":
                cars = datas.Deserialize<List<CarTemplate>>();
                break;
            case "DailyReward":
                dailyRewards = datas.Deserialize<List<DailyRewardTemplate>>();
                break;
            case "Stage":
                stages = datas.Deserialize<List<StageTemplate>>();
                break;
            // 클라이언트 전용
            case "Customer":
            case "Talk":
                break;
            default:
                return 0;
        }

        var verModel = await versions.Find(e => e.Name == name).FirstOrDefaultAsync();
        if (verModel != null)
        {
            ++verModel.Version;
            await versions.ReplaceOneAsync(e => e.Name == name, verModel);
            await templates.ReplaceOneAsync(e => e.Name == name, new()
            {
                Name = name,
                Datas = BsonSerializer.Deserialize<BsonArray>(datas.ToJsonString())
            });
            return verModel.Version;
        }
        else
        {
            await versions.InsertOneAsync(new()
            {
                Name = name,
                Version = 1
            });
            await templates.InsertOneAsync(new()
            {
                Name = name,
                Datas = BsonSerializer.Deserialize<BsonArray>(datas.ToJsonString())
            });
            return 1;
        }
    }

    public async Task Delete(string name)
    {
        await versions.DeleteOneAsync(e => e.Name == name);
        await templates.DeleteOneAsync(e => e.Name == name);
    }

    public async Task<Dictionary<string, ulong>> GetVersions()
    {
        var models = await versions.Find(e => true).ToListAsync();
        var result = new Dictionary<string, ulong>();
        foreach (var e in models)
        {
            if (!string.IsNullOrEmpty(e.Name))
                result.Add(e.Name, e.Version);
        }
        return result;
    }

    public async Task<JsonArray?> Get(string name)
    {
        var model = await templates.Find(e => e.Name == name).FirstOrDefaultAsync();
        if (model == null)
            return null;
        return JsonSerializer.Deserialize<JsonArray>(model.Datas.ToJson());
    }

    public async Task<List<CarTemplate>?> GetCars()
    {
        if (cars == null)
        {
            var model = await templates.Find(e => e.Name == "Car").FirstOrDefaultAsync();
            cars = JsonSerializer.Deserialize<List<CarTemplate>>(model.Datas.ToJson());
        }
        return cars;
    }

    public async Task<List<DailyRewardTemplate>?> GetDailyRewards()
    {
        if (dailyRewards == null)
        {
            var model = await templates.Find(e => e.Name == "DailyReward").FirstOrDefaultAsync();
            dailyRewards = JsonSerializer.Deserialize<List<DailyRewardTemplate>>(model.Datas.ToJson());
        }
        return dailyRewards;
    }

    public async Task<List<StageTemplate>?> GetStages()
    {
        if (stages == null)
        {
            var model = await templates.Find(e => e.Name == "Stage").FirstOrDefaultAsync();
            stages = JsonSerializer.Deserialize<List<StageTemplate>>(model.Datas.ToJson());
        }
        return stages;
    }
}
