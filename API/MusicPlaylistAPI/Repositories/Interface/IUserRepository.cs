using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories.Interface;

public interface IUserRepository
{
    Task CreateAsync(User user);
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task UpdateAsync(string id, User user);
    Task DeleteAsync(string id);
}
