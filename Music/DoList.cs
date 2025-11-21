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
    public partial class DoList : Form
    {
        private byte[] cover;
        private string title;
        private List<Follow> follows;
        private List<Comment> comments;
        private string playlistId;
        public DoList(string id,byte[] cover, string title, List<Follow> follows, List<Comment> comments)
        {
            InitializeComponent();

            this.playlistId = id;
            this.cover = cover ?? new byte[0];
            this.title = title ?? "";
            this.follows = follows ?? new List<Follow>();
            this.comments = comments ?? new List<Comment>();

            textBox1.Text = this.title;
            DisplayData();
        }

        private void DisplayData()
        {
            if (cover.Length > 0)
            {
                using (var ms = new System.IO.MemoryStream(cover))
                {
                    var image = Image.FromStream(ms);
                    button5.Image = image;
                    button5.ImageAlign = ContentAlignment.MiddleCenter;
                    button5.Text = ""; 
                }
            }
            else
            {
                button5.Image = null;
            }
        }


        private void DoList_Load(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            AddMusic form6 = new AddMusic(playlistId);
            form6.Show();
            this.Hide();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string updatedTitle = textBox1.Text;

            byte[] updatedCover = cover;

            var updatedPlaylist = new
            {
                Id = playlistId,
                Title = updatedTitle,
                Cover = updatedCover,
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(updatedPlaylist);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://musicplaylist-a1qc.onrender.com/");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"api/Playlists/{playlistId}", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Плейлист успішно оновлено");
                }
                else
                {
                    MessageBox.Show($"Помилка оновлення: {response.StatusCode}");
                }
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Ви впевнені, що хочете видалити цей плейлист?", "Підтвердження видалення", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://musicplaylist-a1qc.onrender.com/");
                    HttpResponseMessage response = await client.DeleteAsync($"api/Playlists/{playlistId}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Плейлист успішно видалено");
                        this.Close(); 
                    }
                    else
                    {
                        MessageBox.Show($"Помилка видалення: {response.StatusCode}");
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowFolowers followersForm = new ShowFolowers(follows);
            followersForm.Show();
        }
    }
}
