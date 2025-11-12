namespace MusicPlaylistAPI.Models.Dto.Get;

public class UserGetDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public List<PlaylistGetDto> Playlists { get; set; }
}
