using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;

namespace MusicPlaylistAPI.Services.Interface;

public interface IMusicService
{
    Task<MusicGetDto> CreateAsync(MusicCreateDto music);
    Task<List<MusicGetDto>> GetAsync();
    Task<MusicGetDto> GetAsync(string id);
    Task<List<MusicGetDto>> GetByPlaylistAsync(string id);
    Task<MusicGetDto> UpdateAsync(string id, MusicCreateDto user);
    Task DeleteAsync(string id);
}
