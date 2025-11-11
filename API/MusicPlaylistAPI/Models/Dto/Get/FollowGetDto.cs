namespace MusicPlaylistAPI.Models.Dto.Get;

public class FollowGetDto
{
    public string Id { get; set; }
    public UserGetDto Follower { get; set; }
    public DateTime FollowedAt { get; set; }
    public PlaylistGetDto Playlist { get; set; }
}
