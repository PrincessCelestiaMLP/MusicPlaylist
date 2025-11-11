using MongoDB.Driver;
using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;

    public UserRepository() =>
        _collection = MongoDBClient.Instance.GetCollection<User>("Users");

    public async Task Create(User user) =>
        await _collection.InsertOneAsync(user);

    public async Task<List<User>> GetAll() =>
        await (await _collection.FindAsync(_ => true)).ToListAsync();

    public async Task<User?> GetById(string id) =>
        await (await _collection.FindAsync(u => u.Id == id)).FirstOrDefaultAsync();

    public async Task Update(string id, User user) =>
        await _collection.ReplaceOneAsync(u => u.Id == id, user);

    public async Task Delete(string id) =>
        await _collection.DeleteOneAsync(u => u.Id == id);

}
