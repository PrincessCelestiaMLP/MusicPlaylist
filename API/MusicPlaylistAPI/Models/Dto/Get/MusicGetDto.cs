namespace MusicPlaylistAPI.Models.Dto.Get
{
    public class MusicGetDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public byte[] Cover { get; set; }
        public string Link { get; set; }
    }
}
