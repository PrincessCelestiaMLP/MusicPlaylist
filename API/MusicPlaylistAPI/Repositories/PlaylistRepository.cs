using MongoDB.Driver;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;

namespace MusicPlaylistAPI.Repositories;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly IMongoCollection<Playlist> _collection;

    public PlaylistRepository() =>
        _collection = MongoDBClient.Instance.GetCollection<Playlist>("Playlists");

    public async Task CreateAsync(Playlist playlist) =>
        await _collection.InsertOneAsync(playlist);

    public async Task<List<Playlist>> GetAllAsync() =>
        await (await _collection.FindAsync(_ => true)).ToListAsync();

    public async Task<Playlist?> GetByIdAsync(string id) =>
        await (await _collection.FindAsync(p => p.Id == id)).FirstOrDefaultAsync();

    public async Task<List<Playlist>> GetByUserIdAsync(string id) =>
        await (await _collection.FindAsync(p => p.UserId == id)).ToListAsync();

    public async Task UpdateAsync(string id, Playlist playlist) =>
        await _collection.ReplaceOneAsync(p => p.Id == id, playlist);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(p => p.Id == id);
}
