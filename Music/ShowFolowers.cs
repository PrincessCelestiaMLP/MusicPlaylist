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
    public partial class ShowFolowers : Form
    {
        private List<Follow> followers;

        public ShowFolowers(List<Follow> followers)
        {
            InitializeComponent();
            this.followers = followers ?? new List<Follow>();

            DisplayFollowers();
        }
        private void DisplayFollowers()
        {
            panelFollowers.Controls.Clear();

            int xLeft = 10;
            int xRight = 150;
            int y = 10;
            int labelHeight = 25;
            int gapY = 5;

            for (int i = 0; i < followers.Count; i++)
            {
                Label lbl = new Label();
                lbl.AutoSize = true;

                string followerName = "Unknown";

                if (followers[i].Follower != null)
                {
                    if (!string.IsNullOrEmpty(followers[i].Follower.Username))
                        followerName = followers[i].Follower.Username;
                    else if (!string.IsNullOrEmpty(followers[i].Follower.Email))
                        followerName = followers[i].Follower.Email;
                }

                lbl.Text = followerName;
                lbl.Font = new Font("Segoe UI", 10, FontStyle.Regular);

                if (i != 0 && i % 4 == 0)
                {
                    y += labelHeight + gapY;
                }

                if (i % 2 == 0)
                {
                    lbl.Location = new Point(xLeft, y);
                }
                else
                {
                    lbl.Location = new Point(xRight, y);
                }

                panelFollowers.Controls.Add(lbl);
            }
        }
        private void ShowFolowers_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            var previousForm = Application.OpenForms.OfType<View>().FirstOrDefault();
            if (previousForm != null)
            {
                previousForm.Show();
            }
        }
    }
}
