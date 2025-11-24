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
using Newtonsoft.Json;

namespace Music
{

    public partial class Form1 : Form
    {
        private UserView currentUser;
        public Form1()
        {
            InitializeComponent();
            currentUser = new UserView();
            label2.Location = new Point(69, 256);
            textBox1.Location = new Point(74, 285);
            label3.Location = new Point(69, 335);
            textBox2.Location = new Point(74, 364);
            label6.Visible = false;
            textBox3.Visible = false;
        }
        private void label4_Click(object sender, EventArgs e)
        {
            label1.Text = "LOGIN";
            label1.Location = new Point(149, 198);
            panel3.BackColor = Color.White;
            panel4.BackColor = Color.FromArgb(255, 192, 255);
            label4.ForeColor = Color.FromArgb(255, 128, 255);
            label4.BackColor = Color.White;
            label5.ForeColor = Color.DimGray;
            label5.BackColor = Color.FromArgb(255, 192, 255);

            label2.Location = new Point(69, 256);
            textBox1.Location = new Point(74, 285);
            label3.Location = new Point(69, 335);
            textBox2.Location = new Point(74, 364);
            label6.Visible = false;
            textBox3.Visible = false;

            button3.Text = "Login";
        }
        private void label5_Click(object sender, EventArgs e)
        {
            label1.Text = "REGISTRATION";
            label1.Location = new Point(100, 198);
            panel3.BackColor = Color.FromArgb(255, 192, 255);
            panel4.BackColor = Color.White;
            label4.ForeColor = Color.DimGray;
            label4.BackColor = Color.FromArgb(255, 192, 255);
            label5.ForeColor = Color.FromArgb(255, 128, 255);
            label5.BackColor = Color.White;

            label2.Location = new Point(69, 230);
            textBox1.Location = new Point(74, 259);
            label3.Location = new Point(69, 360);
            textBox2.Location = new Point(74, 389);
            label6.Visible = true;
            textBox3.Visible = true;

            button3.Text = "Registr";
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
        public async Task<string> RegisterAsync(string username, string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://musicplaylist-a1qc.onrender.com");

                var data = new
                {
                    username = username,
                    email = email,
                    password = password
                };
                string json = JsonConvert.SerializeObject(data);

                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("/api/Users", content);
                
                return await response.Content.ReadAsStringAsync();
            }
        }
        
        public async Task<bool> LoginAsync(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://musicplaylist-a1qc.onrender.com/");
               
                string url = $"api/Users/email?email={email}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Помилка сервера: {response.StatusCode}");
                    return false;
                }

                string jsonResponse = await response.Content.ReadAsStringAsync();
                
                var user = JsonConvert.DeserializeObject<User>(jsonResponse);

                if (user.Password == password)
                {
                    currentUser.Id = user.Id;
                    currentUser.Email = user.Email;
                    currentUser.Username = user.Username;
                    return true; 
                }
                else
                {
                    MessageBox.Show("Невірний пароль");
                    return false;
                }
            }
        }
        public async Task<List<Playlist>> GetUserPlaylistsAsync(string userId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://musicplaylist-a1qc.onrender.com/");

                string url = $"api/Playlists/Users/{userId}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Помилка сервера: {response.StatusCode}");
                    return new List<Playlist>();
                }

                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Playlist>>(json);
            }
        }
        private List<Playlist> userPlaylists = new List<Playlist>();
        private async void button3_Click(object sender, EventArgs e)
        {
            string username, email, password;
            if (button3.Text == "Login")
            {
                email = textBox1.Text;
                password = textBox2.Text;

                bool result = await LoginAsync(email, password);

                if (result)
                {
                    userPlaylists = await GetUserPlaylistsAsync(currentUser.Id);
                    Menu menu = new Menu(userPlaylists, currentUser.Id);
                    menu.Show();
                    this.Hide();
                }
                textBox1.Text = "";
                textBox2.Text = "";
            }
            else
            {
                username = textBox3.Text;
                password = textBox2.Text;
                email = textBox1.Text;
               
                await RegisterAsync(username, email, password);
                
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";

            }
        }
    }
}
