namespace MusicPlaylistAPI.Models.Dto.Create;

public class CommentCreateDto
{
    public string Title { get; set; }
    public string Text { get; set; }
    public string PlaylistId { get; set; }
    public string UserId { get; set; }
}
