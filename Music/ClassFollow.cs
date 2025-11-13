using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music
{
    internal class ClassFollow
    {
        public User Folover {  get; set; }
        public DateTime FolowedAt { get; set; }
        public Playlist Playlist { get; set; }
    }
}
