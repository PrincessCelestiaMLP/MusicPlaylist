using MongoDB.Driver;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories;

namespace MusicPlaylistAPI;

public class MusicRepository : IMusicRepository
{
    private readonly IMongoCollection<Music> _collection;

    public MusicRepository() =>
        _collection = MongoDBClient.Instance.GetCollection<Music>("Musics");

    public async Task CreateAsync(Music music) =>
        await _collection.InsertOneAsync(music);

    public async Task<List<Music>> GetAllAsync() =>
        await (await _collection.FindAsync(_ => true)).ToListAsync();

    public async Task<Music?> GetByIdAsync(string id) =>
        await (await _collection.FindAsync(m => m.Id == id)).FirstOrDefaultAsync();

    public async Task UpdateAsync(string id, Music music) =>
        await _collection.ReplaceOneAsync(m => m.Id == id, music);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(id);
}
