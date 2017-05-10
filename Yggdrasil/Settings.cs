using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Media;
using System.Threading;
using System.Runtime.InteropServices;

namespace Yggdrasil
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            button1.Text = Properties.strings.Browse;
            label1.Text = Properties.strings.ChooseImage;
            button3.Text = Properties.strings.RemoveTheme;
            label3.Text = Properties.strings.MonoTitle;
            usemono.Text = Properties.strings.MonoIcons;
            nlist.Text = Properties.strings.UseNamelist;
            label4.Text = Properties.strings.NameListTitle;
            if (File.Exists("monoicon"))
            {
                usemono.Checked = true;
            }
            else
            {
                usemono.Checked = false;
            }
            if (Main.namelist)
            {
                nlist.Checked = true;
            } else
            {
                nlist.Checked = false;
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
        );


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    File.WriteAllText("ygg_bgimage.conf", openFileDialog1.FileName);
                    PlayerStop();
                    File.Create("hidenotify");
                    try
                    {
                        Stream str = Properties.sounds._out;
                        SoundPlayer snd = new SoundPlayer(str);
                        snd.Play();
                    }
                    catch { }
                    Thread.Sleep(200);
                    File.Delete("lock");
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Environment.Exit(0);
                }
            }
            catch { }
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
                File.Create("hidenotify");
                try
                {
                    Stream str = Properties.sounds._out;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();
                }
                catch { }
                Thread.Sleep(200);
                File.Delete("lock");
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Environment.Exit(0);
            }
            catch { }
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

        private void usemono_CheckedChanged(object sender, EventArgs e)
        {
            if (!usemono.Checked)
            {
                File.Delete("monoicon");
            }
            else
            {
                var f = File.Create("monoicon");
                f.Close();
            }
        }

        private void nlist_CheckedChanged(object sender, EventArgs e)
        {
            if (nlist.Checked)
            {
                var f = File.Create("use_namelist");
                f.Close();
            }
            else
            {
                File.Delete("use_namelist");
            }
            Main.namelist = nlist.Checked;
        }
    }
}
