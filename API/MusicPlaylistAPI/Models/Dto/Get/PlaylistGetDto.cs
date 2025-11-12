using MusicPlaylistAPI.Models.Dto.Get;

namespace MusicPlaylistAPI.Models.Dto;

public class PlaylistGetDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<MusicGetDto> Musics { get; set; }
    public List<CommentGetDto> Comments { get; set; }
    public List<FollowGetDto> Follows { get; set; }
}
