using MusicPlaylistAPI.Models.Entity;

namespace MusicPlaylistAPI.Repositories;

public interface IUserRepository
{
    Task Create(User user);
    Task<List<User>> GetAll();
    Task<User?> GetById(string id);
    Task Update(string id, User user);
    Task Delete(string id);
}
