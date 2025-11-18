namespace MusicPlaylistAPI.Models.Dto.Get;

public class FolllowGetDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserView Author { get; set; }
}
