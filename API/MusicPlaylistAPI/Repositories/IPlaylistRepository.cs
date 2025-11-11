using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories;

public interface IPlaylistRepository
{
    Task CreateAsync(Playlist playlist);
    Task<List<Playlist>> GetAllAsync();
    Task<Playlist?> GetByIdAsync(string id);
    Task UpdateAsync(string id, Playlist playlist);
    Task DeleteAsync(string id);
}
