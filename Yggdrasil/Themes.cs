using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

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
                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    Environment.Exit(0);
                }
            } catch { }
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
