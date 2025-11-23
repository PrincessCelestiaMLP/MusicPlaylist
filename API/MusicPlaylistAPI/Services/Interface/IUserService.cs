using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;

namespace MusicPlaylistAPI.Services.Interface;

public interface IUserService
{
    Task<UserGetDto> CreateAsync(UserCreateDto user);
    Task<List<UserGetDto>> GetAsync();
    Task<UserGetDto> GetAsync(string id);
    Task<UserGetDto> GetByEmailAsync(string email);
    Task<UserGetDto> UpdateAsync(string id, UserCreateDto user);
    Task DeleteAsync(string id);
}
