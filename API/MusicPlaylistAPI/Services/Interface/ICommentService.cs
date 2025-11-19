using MusicPlaylistAPI.Models.Dto.Create;
using MusicPlaylistAPI.Models.Dto.Get;

namespace MusicPlaylistAPI.Services.Interface;

public interface ICommentService
{
    Task<CommentGetDto> CreateAsync(CommentCreateDto comment);
    Task<List<CommentGetDto>> GetAsync();
    Task<CommentGetDto> GetAsync(string id);
    Task<List<CommentGetDto>> GetByPlaylistAsync(string id);
    Task DeleteAsync(string id);
}
