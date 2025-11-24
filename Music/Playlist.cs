using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Musi;

namespace Music
{
    public class Playlist
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public byte[] Cover { get; set; }
        public User creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Music> Musics { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Follow> Follows { get; set; }
    }
}
