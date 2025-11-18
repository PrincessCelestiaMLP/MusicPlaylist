using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories.Interface;

public interface IFollowRepository
{
    Task CreateAsync(Follow follow);
    Task<List<Follow>> GetAllAsync();
    Task<Follow?> GetByIdAsync(string id);
    Task<List<Follow>> GetByPlaylistIdAsync(string id);
    Task DeleteAsync(string id);
}
