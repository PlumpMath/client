using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

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
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Environment.Exit(0);
            } catch { }
        }
    }
}
