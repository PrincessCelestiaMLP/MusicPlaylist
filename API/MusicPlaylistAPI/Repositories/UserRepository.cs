using MongoDB.Driver;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;

namespace MusicPlaylistAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _collection;
    private readonly IMongoCollection<Comment> _comments;

    public UserRepository() =>
        _collection = MongoDBClient.Instance.GetCollection<User>("Users");

    public async Task CreateAsync(User user) =>
        await _collection.InsertOneAsync(user);

    public async Task<List<User>> GetAllAsync() =>
        await (await _collection.FindAsync(_ => true)).ToListAsync();

    public async Task<User?> GetByIdAsync(string id) =>
        await (await _collection.FindAsync(u => u.Id == id)).FirstOrDefaultAsync();

    public async Task<User?> GetByEmailAsync(string email) =>
        await (await _collection.FindAsync(u => u.Email == email)).FirstOrDefaultAsync();

    public async Task UpdateAsync(string id, User user) =>
        await _collection.ReplaceOneAsync(u => u.Id == id, user);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(u => u.Id == id);

}
