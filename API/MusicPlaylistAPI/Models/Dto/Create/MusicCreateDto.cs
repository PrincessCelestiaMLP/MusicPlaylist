namespace MusicPlaylistAPI.Models.Dto.Create;

public class MusicCreateDto
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public byte[] Cover { get; set; }
    public string Link { get; set; }
    public string PlaylistId { get; set; }
}
