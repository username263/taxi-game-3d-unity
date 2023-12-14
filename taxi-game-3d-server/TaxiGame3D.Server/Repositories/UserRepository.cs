using MongoDB.Driver;
using TaxiGame3D.Server.Database;
using TaxiGame3D.Server.Models;

namespace TaxiGame3D.Server.Repositories;

public class UserRepository
{
    readonly IMongoCollection<UserModel> users;

    public UserRepository(DatabaseContext context)
    {
        users = context.Users;
    }

    public async Task Create(UserModel model) =>
        await users.InsertOneAsync(model);

    public async Task<UserModel> Get(string id) =>
        await users.Find(e => e.Id == id).FirstOrDefaultAsync();
    
    public async Task<UserModel> FindByDevice(string deviceId) =>
        await users.Find(e => e.DeviceId == deviceId).FirstOrDefaultAsync();

    public async Task Update(string id, UserModel model) =>
        await users.ReplaceOneAsync(e => e.Id == id, model);

    public async Task Delete(string id) =>
        await users.DeleteOneAsync(e => e.Id == id);
}
