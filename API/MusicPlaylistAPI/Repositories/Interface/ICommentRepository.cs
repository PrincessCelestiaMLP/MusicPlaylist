using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories.Interface;

public interface ICommentRepository
{
    Task CreateAsync(Comment comment);
    Task<List<Comment>> GetAllAsync();
    Task<Comment> GetByIdAsync(string id);
    Task<List<Comment>> GetByPlaylistIdAsync(string id);
    Task DeleteAsync(string id);
}
