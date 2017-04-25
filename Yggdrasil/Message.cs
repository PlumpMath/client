using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Yggdrasil
{
    public partial class Message : Form
    {

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

        public Message()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
            label1.Text = Properties.strings.Name;
            label2.Text = Properties.strings.EMail;
            label3.Text = Properties.strings.Text;
            try
            {
                Directory.SetCurrentDirectory(Main.cwd);
                new WebClient().DownloadFile("https://koyuawsmbrtn.keybase.pub/yggdrasil/curl.exe", "curl.exe");
            } catch { }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Nameb.Text.Replace(" ", "") == "" || Emailb.Text.Replace(" ", "") == "" || Textb.Text.Replace(" ", "") == "")
            {
                label4.Visible = true;
            } else
            {
                try
                {
                    new WebClient().DownloadString("https://shitload.lima-city.de/");
                    Directory.SetCurrentDirectory(Main.cwd);
                    Process process = new Process();
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.FileName = "curl";
                    process.StartInfo.Arguments = "--data \"name=" + Nameb.Text + "&email=" + Emailb.Text + "&text=" + Textb.Text + "\" https://shitload.lima-city.de/yggmail.php";
                    process.Start();
                    Hide();
                } catch
                {
                    MsgBox m = new MsgBox(Properties.strings.NoSend);
                    m.ShowDialog();
                    this.Hide();
                }
            }
        }
    }
}
