using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Music;

namespace Musi { 
       public class Comment
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
            public DateTime CreatedAt { get; set; }
            public UserView Author { get; set; }
        }
    
}
