using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;

namespace MusicPlaylistAPI.Services.Interface;

public interface ICommentService
{
    Task<FolllowGetDto> CreateAsync(CommentCreateDto comment);
    Task<List<FolllowGetDto>> GetAsync();
    Task<FolllowGetDto> GetAsync(string id);
    Task<List<FolllowGetDto>> GetByPlaylistAsync(string id);
    Task DeleteAsync(string id);
}
