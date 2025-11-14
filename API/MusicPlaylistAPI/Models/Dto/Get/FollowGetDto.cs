namespace MusicPlaylistAPI.Models.Dto.Get;

public class FollowGetDto
{
    public string Id { get; set; }
    public UserView Follower { get; set; }
    public DateTime FollowedAt { get; set; }
}
