using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaxiGame3D.Server.Models;

namespace TaxiGame3D.Server.Database;

public class DatabaseContext
{
    public IMongoCollection<UserModel> Users
    {
        get;
        private set;
    }

    public IMongoCollection<TemplateVersionModel> TemplateVersions
    {
        get;
        private set;
    }

    public IMongoCollection<TemplateModel> Templates
    {
        get;
        private set;
    }

    public DatabaseContext(IOptions<DatabaseSettings> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        var database = client.GetDatabase(options.Value.DatabaseName);

        Users = database.GetCollection<UserModel>("Users");
        TemplateVersions = database.GetCollection<TemplateVersionModel>("TemplateVersions");
        Templates = database.GetCollection<TemplateModel>("Templates");
    }
}
