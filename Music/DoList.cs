    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
using System.IO;
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
            private bool state;
        private List<Playlist> userPlaylists;
            private string userId;
            public DoList(string id,byte[] cover, string title, List<Follow> follows, List<Comment> comments,bool state, string userId,List<Playlist> userPlaylist)
            {
                InitializeComponent();
                userPlaylists = userPlaylist;

                this.playlistId = id;
                this.userId = userId;
                this.cover = cover ?? new byte[0];
                this.title = title ?? "";
                this.follows = follows ?? new List<Follow>();
                this.comments = comments ?? new List<Comment>();
                this.state = state;

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
            string title = textBox1.Text;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введіть назву плейлиста!");
                return;
            }
            MessageBox.Show("" + cover);
            // JSON який приймає сервер
            var playlistData = new Dictionary<string, object>
        
    {
        { "title", title },
        { "userId", userId },
                {"cover", cover }
    };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(playlistData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://musicplaylist-a1qc.onrender.com/");

                HttpResponseMessage response;

                // Створення нового плейлиста (state == false)
                response = await client.PostAsync("api/Playlists", content);

                string serverAnswer = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Плейлист успішно створено!");
                }
                else
                {
                    MessageBox.Show(
                        $"❌ Помилка створення плейлиста ({(int)response.StatusCode} - {response.StatusCode}):\n\n" +
                        $"{serverAnswer}",
                        "Помилка API",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
                Menu menu = new Menu(userPlaylists, userId);
                menu.Show();
                this.Hide();
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
                            Menu menu = new Menu(userPlaylists, userId);
                            menu.Show();
                            this.Hide();

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

            private void button5_Click(object sender, EventArgs e)
            {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Виберіть зображення";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    Image img = Image.FromFile(ofd.FileName);

                    button5.BackgroundImage = img;
                    button5.BackgroundImageLayout = ImageLayout.Stretch; 

                    using (var ms = new MemoryStream())
                    {
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        cover = ms.ToArray();
                    }
                }
            }
        }
        }
    }
