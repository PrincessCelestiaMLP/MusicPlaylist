using MusicPlaylistAPI.Models.Dto;
using MusicPlaylistAPI.Models.Dto.Create;

namespace MusicPlaylistAPI.Services.Interface;

public interface IPlaylistService
{
    Task<PlaylistGetDto> CreateAsync(PlaylistCreateDto playlist);
    Task<List<PlaylistGetDto>> GetAsync();
    Task<PlaylistGetDto> GetAsync(string id);
    Task<List<PlaylistGetDto>> GetByUserAsync(string id);
    Task<PlaylistGetDto> UpdateAsync(string id, PlaylistCreateDto playlist);
    Task DeleteAsync(string id);
}
