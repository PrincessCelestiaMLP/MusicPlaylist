using MongoDB.Driver;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;

namespace MusicPlaylistAPI.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly IMongoCollection<Follow> _collection;

    public FollowRepository() =>
        _collection = MongoDBClient.Instance.GetCollection<Follow>("Follows");

    public async Task CreateAsync(Follow follow) =>
        await _collection.InsertOneAsync(follow);

    public async Task<List<Follow>> GetAllAsync() =>
        await (await _collection.FindAsync(_ => true)).ToListAsync();

    public async Task<Follow> GetByIdAsync(string id) =>
        await (await _collection.FindAsync(f => f.Id == id)).FirstOrDefaultAsync();

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(f => f.Id == id);
}
