using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories;

public interface IFollowRepository
{
    Task CreateAsync(Follow follow);
    Task<List<Follow>> GetAllAsync();
    Task<Follow> GetByIdAsync(string id);
    Task DeleteAsync(string id);
}
