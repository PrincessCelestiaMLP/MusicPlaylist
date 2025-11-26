using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Musi;

namespace Music
{
    public partial class Menu : Form
    {
        private List<Playlist> playlists;
        private string currentID;
        public Menu(List<Playlist> playlists, string currentID)
        {
            InitializeComponent();
            this.playlists = playlists;
            this.currentID = currentID;
            SetupButtonsBackground();

        }
        private void SetupButtonsBackground()
        {
            if (playlists == null || playlists.Count == 0)
                return;

            Image ByteArrayToImage(byte[] bytes)
            {
                using (var ms = new MemoryStream(bytes))
                {
                    return Image.FromStream(ms);
                }
            }
            if (playlists.Count >= 1 && playlists[0].Cover != null)
            {
                button2.BackgroundImage = ByteArrayToImage(playlists[0].Cover);
                button2.BackgroundImageLayout = ImageLayout.Stretch;
            }

            if (playlists.Count >= 2 && playlists[1].Cover != null)
            {
                button3.BackgroundImage = ByteArrayToImage(playlists[1].Cover);
                button3.BackgroundImageLayout = ImageLayout.Stretch;
            }
            if (playlists.Count >= 3 && playlists[2].Cover != null)
            {
                button4.BackgroundImage = ByteArrayToImage(playlists[2].Cover);
                button4.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }


        Point lastPoint;
        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;
            }
        }
        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void HandleButtonClick(int playlistIndex, bool My)
        {
            Button btn = null;

            switch (playlistIndex)
            {
                case 0:
                    btn = button2;
                    break;
                case 1:
                    btn = button3;
                    break;
                case 2:
                    btn = button4;
                    break;
            }

            if (btn == null)
                return;

            if (btn.BackgroundImage != null
                && playlists != null
                && playlists.Count > playlistIndex)
            {
                var selectedPlaylist = playlists[playlistIndex];
                View form3 = new View(selectedPlaylist, My, currentID, playlists);
                form3.Show();
                this.Hide();
            }
            else
            {
                bool state;
                if (playlists == null || playlists.Count == 0)
                {
                    state = false;
                    DoList form4 = new DoList("", new byte[0], "", new List<Follow>(), new List<Comment>(), state, currentID, playlists);
                    form4.Show();
                    this.Hide();
                }
                else
                {
                    var p = playlists[0];
                    state = true;
                    DoList form4 = new DoList(
                        p.Id ?? "",
                        p.Cover ?? new byte[0],
                        p.Title ?? "",
                        p.Follows ?? new List<Follow>(),
                        p.Comments ?? new List<Comment>(),
                        state, currentID, playlists);

                    form4.Show();
                    this.Hide();
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            HandleButtonClick(0, true);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HandleButtonClick(1, true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HandleButtonClick(2, true);
        }
    }
}
