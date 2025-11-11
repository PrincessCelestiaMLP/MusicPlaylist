namespace MusicPlaylistAPI.Models.Dto.Get
{
    public class MusicGetDto
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public byte[] Cover { get; set; }
        public string Link { get; set; }
        public PlaylistGetDto Playlist { get; set; }
    }
}
