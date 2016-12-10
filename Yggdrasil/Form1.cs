using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yggdrasil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string alive = new WebClient().DownloadString("http://" + textBox1.Text + "/alive");
                if (alive == "OK")
                {
                    MessageBox.Show("Successfully connected to server!");
                    Main m = new Main();
                    m.Show();
                    this.Hide();
                }
            }
            catch
            {
                MessageBox.Show("Connection to server failed.");
            }
        }

        public string serverip
        {
            get { return textBox1.Text; }
            set { textBox1.Text = value; }
        }
    }
}
