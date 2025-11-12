using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories.Interface;

public interface IMusicRepository
{
    Task CreateAsync(Music music);
    Task<List<Music>> GetAllAsync();
    Task<Music?> GetByIdAsync(string id);
    Task UpdateAsync(string id, Music music);
    Task DeleteAsync(string id);
}
