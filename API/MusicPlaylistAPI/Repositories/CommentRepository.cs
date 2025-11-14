using MongoDB.Driver;
using MusicPlaylistAPI.Models.Entity;
using MusicPlaylistAPI.Repositories.Interface;

namespace MusicPlaylistAPI.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly IMongoCollection<Comment> _collection;

    public CommentRepository() =>
        _collection = MongoDBClient.Instance.GetCollection<Comment>("Comments");

    public async Task CreateAsync(Comment comment) =>
        await _collection.InsertOneAsync(comment);

    public async Task<List<Comment>> GetAllAsync() =>
        await (await _collection.FindAsync(_ => true)).ToListAsync();

    public async Task<Comment?> GetByIdAsync(string id) =>
        await (await _collection.FindAsync(c => c.Id == id)).FirstOrDefaultAsync();

    public async Task<List<Comment>> GetByPlaylistIdAsync(string id) =>
        await (await _collection.FindAsync(c => c.PlaylistId == id)).ToListAsync();

    public async Task UpdateAsync(string id, Comment comment) =>
        await _collection.ReplaceOneAsync(c => c.Id == id, comment);

    public async Task DeleteAsync(string id) =>
        await _collection.DeleteOneAsync(c => c.Id == id);

   
}
