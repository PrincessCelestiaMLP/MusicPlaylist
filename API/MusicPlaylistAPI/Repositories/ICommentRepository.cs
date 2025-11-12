using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories;

public interface ICommentRepository
{
    Task CreateAsync(Comment comment);
    Task<List<Comment>> GetAllAsync();
    Task<Comment> GetByIdAsync(string id);
    Task DeleteAsync(string id);
}
