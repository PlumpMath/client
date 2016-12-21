using System;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Drawing;

namespace Yggdrasil
{
    public partial class Main : Form
    {
        bool connected = false;
        string passPhrase = "";

        public Main()
        {
            InitializeComponent();
            this.Hide();
            Thread t = new Thread(new ThreadStart(splash));
            t.Start();
            Thread.Sleep(5000);
            localize();
            t.Abort();
            this.Show();
            try
            {
                label1.Text = new WebClient().DownloadString("http://yggdrasilfs.neocities.org/msg.txt");
            }
            catch { }
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
            this.textBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.textBox2.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }

        public void splash()
        {
            Application.Run(new Splash());
        }

        private void localize()
        {
            /*button5.Text = Properties.strings.Download;
            button6.Text = Properties.strings.Remove;
            button3.Text = Properties.strings.Clear;
            button4.Text = Properties.strings.Upload;
            button2.Text = Properties.strings.ListDir;*/
            //button1.Text = Properties.strings.Connect;
            //button7.Text = Properties.strings.Quit;
            button8.Text = Properties.strings.Blog;
            label3.Text = Properties.strings.File;
            label4.Text = Properties.strings.Disconnected;
            checkBox1.Text = Properties.strings.PasswordProtected;
            //button9.Text = Properties.strings.Themes;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (connected == false)
            {
                try
                {
                    if (!textBox1.Text.Contains(":"))
                    {
                        if (!Text.Contains(":82"))
                        {
                            textBox1.Text = textBox1.Text + ":82";
                        }
                    }
                    string alive = new WebClient().DownloadString("http://" + textBox1.Text + "/alive");
                    if (alive == "OK")
                    {
                        textBox1.Enabled = false;
                        //button1.Text = Properties.strings.Disconnect;
                        richTextBox1.Text = "";
                        button1.Image = Properties.Resources.disconnect;
                        label4.Text = Properties.strings.Connected;
                        label4.ForeColor = System.Drawing.Color.Green;
                        connected = true;
                        string motd = new WebClient().DownloadString("http://" + textBox1.Text + "/motd");
                        motd = motd.Replace("\n", Environment.NewLine);
                        byte[] bytes = Encoding.Default.GetBytes(motd);
                        motd = Encoding.UTF8.GetString(bytes);
                        richTextBox1.Text += motd + Environment.NewLine;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                        try
                        {
                            textBox4.Text = new WebClient().DownloadString("http://" + textBox1.Text + "/news");
                        }
                        catch { }
                    }
                    else
                    {
                        richTextBox1.Text += "Cannot connect to server: Not alive!\n" + Environment.NewLine;
                    }
                }
                catch (Exception ee)
                {
                    richTextBox1.Text += "Cannot connect to server:\n\n" + ee + "\n" + Environment.NewLine;
                }
            }
            else
            {
                richTextBox1.Text = "";
                textBox1.Enabled = true;
                //button1.Text = Properties.strings.Connect;
                button1.Image = Properties.Resources.connect;
                label4.Text = Properties.strings.Disconnected;
                label4.ForeColor = System.Drawing.Color.Red;
                textBox4.Text = "";
                if (textBox1.Text.Contains(":82"))
                {
                    textBox1.Text = textBox1.Text.Split(':')[0];
                }
                connected = false;
            }
        }

        public void button2_Click(object sender, EventArgs e)
        {
            if (connected == true)
            {
                try
                {
                    string dir = new WebClient().DownloadString("http://" + textBox1.Text + "/ls");
                    dir = dir.Replace("\n", Environment.NewLine);
                    byte[] bytes = Encoding.Default.GetBytes(dir);
                    dir = Encoding.UTF8.GetString(bytes);
                    if (dir == "")
                    {
                        richTextBox1.Text += Properties.strings.NoFiles + Environment.NewLine;
                    }
                    else
                    {
                        richTextBox1.Text += dir + Environment.NewLine;
                    }
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }
                catch
                {
                    richTextBox1.Text += "Cannot list directory!";
                }
            }
        }

        public void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        public static string Base64Encode(string plainText, string useless)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData, string useless)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public void button4_Click(object sender, EventArgs e)
        {
            passPhrase = textBox3.Text;
            try
            {
                if (connected)
                {
                    var encryptedfile = "";
                    DialogResult result = openFileDialog1.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (System.IO.File.Exists(openFileDialog1.FileName))
                        {
							encryptedfile = Setup.Encrypt(System.IO.File.ReadAllText(openFileDialog1.FileName, Encoding.Default));
                            Uri uri = new Uri("http://" + textBox1.Text + "/upload");
                            string filename = openFileDialog1.SafeFileName;
                            string downloadedfile = new WebClient().UploadString(uri, "content=" + encryptedfile + "&filename=" + filename + "&password=" + CalculateMD5Hash(passPhrase));
                            if (downloadedfile != "ERR_WRONG_PW")
                            {
                                richTextBox1.Text += "File \"" + openFileDialog1.SafeFileName + "\" uploaded.\n" + Environment.NewLine;
                            }
                            else
                            {
                                richTextBox1.Text += Properties.strings.WrongPassword + Environment.NewLine;
                            }
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            richTextBox1.ScrollToCaret();
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                richTextBox1.Text += "Error encrypting or uploading file:\n\n" + ee + "\n" + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }

        public void button5_Click(object sender, EventArgs e)
        {
            passPhrase = textBox3.Text;
            try
            {
                if (connected)
                {
                    string filename = textBox2.Text;
                    string downloadedfile = new WebClient().DownloadString("http://" + textBox1.Text + "/download?filename=" + filename + "&password=" + CalculateMD5Hash(passPhrase));
                    DialogResult result = saveFileDialog1.ShowDialog();
                    if (result == DialogResult.OK && saveFileDialog1.CheckPathExists)
                    {
                        //OK
                    }
                    downloadedfile = downloadedfile.Replace(' ', '+');
                    string decrypted = Setup.Decrypt(downloadedfile);
                    System.IO.File.WriteAllText(saveFileDialog1.FileName, decrypted, Encoding.Default);
                    richTextBox1.Text += "File \"" + textBox2.Text + "\" downloaded.\n" + Environment.NewLine;
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }
            }
            catch (Exception ee)
            {
                richTextBox1.Text += "An error occured:\n\n" + ee + "\n" + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }

        public void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (connected)
                {
                    string filename = textBox2.Text;
                    string callback = new WebClient().DownloadString("http://" + textBox1.Text + "/del?filename=" + textBox2.Text + "&password=" + CalculateMD5Hash(passPhrase));
                    if (callback == "OK")
                    {
                        richTextBox1.Text += "File \"" + filename + "\" deleted\n\n" + "\n" + Environment.NewLine;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                    }
                }
            }
            catch (Exception ee)
            {
                richTextBox1.Text += "An error occured:\n\n" + ee + "\n" + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }

        public void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox3.Enabled = true;
            }
            else
            {
                textBox3.Enabled = false;
                textBox3.Text = "";
                passPhrase = "";
            }
        }

        public string CalculateMD5Hash(string input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                new WebClient().DownloadString("http://yggdrasilfs.neocities.org");
                Blog b = new Blog();
                if (!b.Visible)
                {
                    b.Show();
                }
            }
            catch
            {
                MessageBox.Show(Properties.strings.NoBlog, Properties.strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("\n"))
            {
                textBox1.Text = textBox1.Text.Replace('\n', ' ');
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            Themes t = new Themes();
            if (!t.Visible)
            {
                t.Show();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox t = sender as TextBox;
            if (t != null && connected)
            {
                string[] arr = new WebClient().DownloadString("http://" + textBox1.Text + "/ls").Split('\n');
                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                collection.AddRange(arr);
                this.textBox2.AutoCompleteCustomSource = collection;
            }
        }

    }
}
