using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;
using MusicPlaylistAPI.Services.Interface;

namespace MusicPlaylistAPI.Services;

public class FollowService : IFollowService
{
    public Task<FollowGetDto> CreateAsync(FollowCreteDto follow)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<FollowGetDto>> GetAsync()
    {
        throw new NotImplementedException();
    }

    public Task<FollowGetDto> GetAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<FollowGetDto>> GetByPlaylistAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<FollowGetDto> UpdateAsync(string id, FollowCreteDto follow)
    {
        throw new NotImplementedException();
    }
}
