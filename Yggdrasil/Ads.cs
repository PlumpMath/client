using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yggdrasil
{
    public partial class Ads : Form
    {
        string link = "";
        public Ads()
        {
            InitializeComponent();
            this.TransparencyKey = this.BackColor;
            try
            {
                link = new WebClient().DownloadString("https://koyuawsmbrtn.keybase.pub/yggdrasil/ad.txt");
                new WebClient().DownloadFile("https://koyuawsmbrtn.keybase.pub/yggdrasil/ad.png", "ad.png");
                webBrowser1.Image = Image.FromFile("ad.png");
                File.Delete("ad.png");
            } catch
            {
                webBrowser1.Visible = false;
            }
        }

        //Global variables;
        private bool _dragging = false;
        //private Point _offset;
        private Point _start_point = new Point(0, 0);


        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;  // _dragging is your variable flag
            _start_point = new Point(e.X, e.Y);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                Point p = PointToScreen(e.Location);
                Location = new Point(p.X - this._start_point.X, p.Y - this._start_point.Y);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void webBrowser1_Click(object sender, EventArgs e)
        {
            Process.Start(link);
        }
    }
}
