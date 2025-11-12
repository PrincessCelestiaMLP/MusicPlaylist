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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        
    }
}
