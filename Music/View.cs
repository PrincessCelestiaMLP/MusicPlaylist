using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Music
{
    public partial class View : Form
    {
        private Playlist currentPlaylist;
        public View(Playlist playlist)
        {
            InitializeComponent(); 
            currentPlaylist = playlist;
        }

        private void View_Load(object sender, EventArgs e)
        {

        }
    }
}
