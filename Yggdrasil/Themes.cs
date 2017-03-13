using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Media;
using System.Threading;

namespace Yggdrasil
{
    public partial class Themes : Form
    {
        public Themes()
        {
            InitializeComponent();
            button1.Text = Properties.strings.Browse;
            label1.Text = Properties.strings.ChooseImage;
            button2.Text = Properties.strings.OK;
            button3.Text = Properties.strings.RemoveTheme;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    File.WriteAllText("ygg_bgimage.conf", openFileDialog1.FileName);
                    PlayerStop();
                    try
                    {
                        Stream str = Properties.sounds._out;
                        SoundPlayer snd = new SoundPlayer(str);
                        snd.Play();
                    }
                    catch { }
                    Thread.Sleep(1500);
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Environment.Exit(0);
                }
            } catch { }
        }

        //Global variables;
        private bool _dragging = false;
        //private Point _offset;
        private Point _start_point = new Point(0, 0);

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;  // _dragging is your variable flag
            _start_point = new Point(e.X, e.Y);
            SetBounds(Bounds.X, Bounds.Y, Width, Height);
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
            SetBounds(Bounds.X, Bounds.Y, Width, Height);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                File.Delete("ygg_bgimage.conf");
                PlayerStop();
                try
                {
                    Stream str = Properties.sounds._out;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();
                }
                catch { }
                Thread.Sleep(1500);
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Environment.Exit(0);
            } catch { }
        }

        public static void PlayerStop()
        {
            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "taskkill";
            process.StartInfo.Arguments = "/IM yggplayer.exe /F";
            process.Start();
        }
    }
}
