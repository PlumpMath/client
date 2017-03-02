using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
        public Ads()
        {
            InitializeComponent();
            if (File.Exists("ygg_bgimage.conf"))
            {
                try
                {
                    string bgimage_f = File.ReadAllText("ygg_bgimage.conf");
                    pictureBox1.BackgroundImage = Image.FromFile(bgimage_f);
                    pictureBox1.Update();
                }
                catch
                {
                    File.Delete("ygg_bgimage.conf");
                }
            }
            try
            {
                string ads = new WebClient().DownloadString("https://koyuawsmbrtn.keybase.pub/yggdrasil/msg_1000.txt");
            }
            catch {
                webBrowser1.Visible = false;
            }
            this.TransparencyKey = this.BackColor;
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
    }
}
