namespace MusicPlaylistAPI.Models.Dto.Get;

public class CommentGetDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserGetDto Author { get; set; }
}
