using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BNS_Tools_ALL
{
    public partial class LoadingForm : Form
    {
        Image img;
        public LoadingForm()
        {
            InitializeComponent();
            img = pictureBox1.Image;
            pictureBox1.LoadCompleted += PictureBox1_LoadCompleted;
            this.pictureBox1.WaitOnLoad = false;
            //this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBox1.LoadAsync("http://101.37.76.151:8121/img/LogoImg.jpg");
            
            
            
        }

        private void PictureBox1_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (pictureBox1.Image.Size.Width < 600 && pictureBox1.Image.Size.Height < 400)
            {
                pictureBox1.Image = img;
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                this.Size = new Size(500, 500);
                dg();
            }
            else
            {
                pictureBox1.Dock = DockStyle.None;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                Size s = pictureBox1.Size;
                if (s.Width > 600)
                {
                    s.Width -= Convert.ToInt32(s.Width * 0.5);
                }
                if (s.Height > 600)
                {
                    s.Height -= Convert.ToInt32(s.Height * 0.5);
                }
                this.Size = s;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Size = s;
                pictureBox1.Location = new Point(0, 0);
                dg();
            }
            
        }

        private void LoadingForm_Shown(object sender, EventArgs e)
        {
            
        }
        private void dg() 
        {
            //this.Location = new Point(this.Location.X - 50, this.Location.Y + 50);
            this.TopMost = true;
            Task.Run(() =>
            {
                //for (int i = 0; i < 50; i++)
                //{
                //    this.Invoke(new Action(delegate
                //    {
                //        this.Location = new Point(this.Location.X + 1, this.Location.Y - 1);
                //    }));
                //    Thread.Sleep(4);
                //}
                Thread.Sleep(3000);
                this.Invoke(new Action(delegate
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }));
            });
        }
    }
}
