using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Musi;

namespace Music
{
    public partial class View : Form
    {
        private Playlist currentPlaylist;
        private string currentID;
        private List<Playlist> userPlaylists;
        public View(Playlist playlist,bool My, string currentID, List<Playlist> userPlaylist)
        {
            InitializeComponent(); 
            this.currentID = currentID;
            currentPlaylist = playlist;
            userPlaylists = userPlaylist;
            if (My)
            {
                button6.Text = "Do";
            }
            else
            {
                button6.Text = "Follow";
            }
            DisplayPlaylistInfo();
            SetupButtonCover();
            SetupDataGridView();
        }
        private void DisplayPlaylistInfo()
        {
            int followersCount = currentPlaylist.Follows?.Count ?? 0;
            int commentsCount = currentPlaylist.Comments?.Count ?? 0;

            label1.Text = $"{followersCount} ❤️";
            label2.Text = $"{commentsCount} comments";
        }

        private void SetupButtonCover()
        {
            if (currentPlaylist.Cover != null && currentPlaylist.Cover.Length > 0)
            {
                using (var ms = new System.IO.MemoryStream(currentPlaylist.Cover))
                {
                    var image = Image.FromStream(ms);
                    button5.BackgroundImage = image;
                    button5.BackgroundImageLayout = ImageLayout.Stretch;
                }
            }
            else
            {
                button5.BackgroundImage = null;
            }
        }

        private void SetupDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("Title", "Назва");
            dataGridView1.Columns.Add("Artist", "Виконавець");
            dataGridView1.Columns.Add("Link", "Посилання");

            if (currentPlaylist.Musics != null)
            {
                foreach (var music in currentPlaylist.Musics)
                {
                    dataGridView1.Rows.Add(music.Title, music.Artist, music.Link);
                }
            }

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private void View_Load(object sender, EventArgs e)
        {

        }

        private async void button6_Click(object sender, EventArgs e)
        {
            if (button6.Text == "Do")
            { 
                DoList form4 = new DoList(
                    currentPlaylist.Id ?? "",
                    currentPlaylist.Cover ?? new byte[0],
                    currentPlaylist.Title ?? "",
                    currentPlaylist.Follows ?? new List<Follow>(),
                    currentPlaylist.Comments ?? new List<Comment>(),
                    true,
                    currentID,
                    userPlaylists
                );

                form4.Show();
                this.Hide();
            }
            else if (button6.Text == "Follow")
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://musicplaylist-a1qc.onrender.com/");

                    if (currentPlaylist.Follows == null)
                        currentPlaylist.Follows = new List<Follow>();

                    var newFollower = new Follow
                    {
                        Id = Guid.NewGuid().ToString(),
                    };

                    currentPlaylist.Follows.Add(newFollower);
                    string updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(currentPlaylist);
                    var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");

                    HttpResponseMessage putResponse = await client.PutAsync($"api/Playlists/{currentPlaylist.Id}", content);

                    if (putResponse.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Ви підписалися на цей плейлист!");

                        label1.Text = $"{currentPlaylist.Follows.Count} ❤️";

                        button6.Enabled = false;
                        button6.Text = "Following";
                    }
                    else
                    {
                        MessageBox.Show("Помилка при підписці на плейлист.");
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Menu menu = new Menu(userPlaylists, currentID);
            menu.Show();
            this.Hide();
        }
    }
}
