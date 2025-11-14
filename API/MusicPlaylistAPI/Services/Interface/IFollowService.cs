using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;

namespace MusicPlaylistAPI.Services.Interface;

public interface IFollowService
{
    Task<FollowGetDto> CreateAsync(FollowCreteDto follow);
    Task<List<FollowGetDto>> GetAsync();
    Task<FollowGetDto> GetAsync(string id);
    Task<List<FollowGetDto>> GetByPlaylistAsync(string id);
    Task<FollowGetDto> UpdateAsync(string id, FollowCreteDto follow);
    Task DeleteAsync(string id);
}
