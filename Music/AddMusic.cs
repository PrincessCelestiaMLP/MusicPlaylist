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

namespace Music
{
    public partial class AddMusic : Form
    {
        private string playlistId;
        public AddMusic(string playlistId)
        {
            InitializeComponent();
            this.playlistId = playlistId;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text.Trim();
            string artist = textBox2.Text.Trim();
            string link = textBox3.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(artist) || string.IsNullOrEmpty(link))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля.");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://musicplaylist-a1qc.onrender.com/");

                HttpResponseMessage getResponse = await client.GetAsync($"api/Playlists/{playlistId}");
                if (!getResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Не вдалося отримати плейлист з сервера.");
                    return;
                }

                string playlistJson = await getResponse.Content.ReadAsStringAsync();
                Playlist playlist = Newtonsoft.Json.JsonConvert.DeserializeObject<Playlist>(playlistJson);

                if (playlist == null)
                {
                    MessageBox.Show("Плейлист не знайдено.");
                    return;
                }

                Music newMusic = new Music
                {
                    Id = Guid.NewGuid().ToString(), 
                    Title = title,
                    Artist = artist,
                    Link = link
                };

                if (playlist.Musics == null)
                    playlist.Musics = new List<Music>();

                playlist.Musics.Add(newMusic);

                string updatedJson = Newtonsoft.Json.JsonConvert.SerializeObject(playlist);
                var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");

                HttpResponseMessage putResponse = await client.PutAsync($"api/Playlists/{playlistId}", content);

                if (putResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Музику додано до плейлиста.");
                    this.Close(); 

                }
                else
                {
                    MessageBox.Show("Помилка при додаванні музики.");
                }
            }
        }
    }
}
