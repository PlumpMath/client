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
        string stream;
        bool radio = true;

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
                label1.Text = new WebClient().DownloadString("https://yggdrasilfs.neocities.org/msg.txt");
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
            button2.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
        }

        public void splash()
        {
            Application.Run(new Splash());
        }

        private void localize()
        {
            /*
            button5.Text = Properties.strings.Download;
            button6.Text = Properties.strings.Remove;
            button3.Text = Properties.strings.Clear;
            button4.Text = Properties.strings.Upload;
            button2.Text = Properties.strings.ListDir;
            button1.Text = Properties.strings.Connect;
            button7.Text = Properties.strings.Quit;
            button9.Text = Properties.strings.Themes;
             */
            button8.Text = Properties.strings.Blog;
            label3.Text = Properties.strings.File;
            label4.Text = Properties.strings.Disconnected;
            checkBox1.Text = Properties.strings.PasswordProtected;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Environment.Exit(0);
            } catch { }
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
                            stream = new WebClient().DownloadString("http://" + textBox1.Text + "/stream");
                            if (!stream.Contains("ERR_") && radio)
                            {
                                PlayMusicFromURL(stream);
                                SetPlayerVolume(100);
                            }
                        }
                        catch { }
                        string deactivated = new WebClient().DownloadString("http://" + textBox1.Text + "/deactivated");
                        if (!deactivated.Contains("ls"))
                        {
                            button2.Enabled = true;
                        }
                        if (!deactivated.Contains("upload"))
                        {
                            button4.Enabled = true;
                        }
                        if (!deactivated.Contains("download"))
                        {
                            button5.Enabled = true;
                        }
                        if (!deactivated.Contains("del"))
                        {
                            button6.Enabled = true;
                        }
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
                button2.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                connected = false;
                try
                {
                    PlayerStop(stream);
                } catch { }
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

        #region player

        public static WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();

        public static void PlayMusicFromURL(string url)
        {
            player.URL = url;
            player.settings.volume = 100;
            player.controls.play();
        }

        public static void PlayerStop(string url)
        {
            player.controls.stop();
        }

        public static void SetPlayerVolume(int volume)
        {
            player.settings.volume = volume;
        }

        #endregion

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
                new WebClient().DownloadString("https://yggdrasilfs.neocities.org");
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
            if (connected && new WebClient().DownloadString("http://" + textBox1.Text + "/ls").Split('\n')[0] != "ERR_DEACTIVATED") {
                TextBox t = sender as TextBox;
                if (t != null && connected && textBox2.TextLength >= 3)
                {
                    string[] arr = new WebClient().DownloadString("http://" + textBox1.Text + "/ls").Split('\n');
                    AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                    collection.AddRange(arr);
                    this.textBox2.AutoCompleteCustomSource = collection;
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            try
            {
                SetPlayerVolume(trackBar1.Value);
            } catch { }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                if (radio)
                {
                    PlayerStop(stream);
                    button10.Image = Properties.Resources.radio_off;
                    radio = false;
                } else
                {
                    PlayMusicFromURL(stream);
                    button10.Image = Properties.Resources.radio_on;
                    radio = true;
                }
            } catch { }
        }

        #region tooltip

        private void button2_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(Properties.strings.ListDir, button2);
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button2);
        }

        private void button4_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(Properties.strings.Upload, button4);
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button4);
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show(Properties.strings.Download, button5);
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button5);
        }

        private void button6_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show(Properties.strings.Delete, button6);
        }

        private void button6_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button6);
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            toolTip1.Show(Properties.strings.Clear, button3);
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button3);
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            if (connected)
            {
                toolTip1.Show(Properties.strings.Disconnect, button1);
            } else
            {
                toolTip1.Show(Properties.strings.Connect, button1);
            }
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button1);
        }

        private void button9_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(Properties.strings.Themes, button9);
        }

        private void button9_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button9);
        }

        private void button10_MouseHover(object sender, EventArgs e)
        {
            if (radio)
            {
                toolTip1.Show(Properties.strings.RadioOff, button10);
            } else
            {
                toolTip1.Show(Properties.strings.RadioOn, button10);
            }
        }

        private void button10_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button10);
        }

        private void button7_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show(Properties.strings.Quit, button7);
        }

        private void button7_MouseLeave(object sender, EventArgs e)
        {
            toolTip1.Hide(button7);
        }

        #endregion
    }
}
