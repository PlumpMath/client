using System;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading;
using System.Drawing;
using System.Media;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

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
            Thread t = new Thread(new ThreadStart(splash));
            t.Start();
            Hide();
            ygginit();
            localize();
            t.Abort();
            this.Show();
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
            listDirectoryToolStripMenuItem1.Enabled = false;
            uploadToolStripMenuItem1.Enabled = false;
            downloadToolStripMenuItem1.Enabled = false;
            deleteToolStripMenuItem1.Enabled = false;
            importFilelistToolStripMenuItem.Enabled = false;
            Stream str = Properties.Resources.startup;
            SoundPlayer snd = new SoundPlayer(str);
            snd.Play();
        }

        public void splash()
        {
            Application.Run(new Splash());
        }

        public void ygginit()
        {
            if (!File.Exists("peazip.exe"))
            {
                compressionToolToolStripMenuItem.Enabled = false;
            }
            try
            {
                string ads = new WebClient().DownloadString("https://koyuawsmbrtn.keybase.pub/yggdrasil/msg_6.txt");
                ads = ads.Replace("\n", Environment.NewLine);
                richTextBox1.Text += ads + Environment.NewLine;
            }
            catch { }
            bool waserror = false;
            if (!File.Exists("patch061_complete"))
            {
                try
                {
                    textBox4.Text = "Downloading Yggdrasil Player patch.";
                    new WebClient().DownloadFile("https://koyuawsmbrtn.keybase.pub/yggdrasil/yggplayer.exe", "yggplayer.exe");
                    File.WriteAllText("patch061_complete", "");
                    textBox4.Text = "Patched Yggdrasil to version 0.6.1.";
                }
                catch
                {
                    textBox4.Text = "Error getting patch from Yggdrasil 0.6.1.";
                    waserror = true;
                }
            }
            try
            {
                string[] playerlist = new WebClient().DownloadString("https://koyuawsmbrtn.keybase.pub/yggdrasil/playerlist.txt").Split('\n');
                foreach (string file in playerlist)
                {
                    if (File.Exists(file))
                    {
                        textBox4.Text = "File \"" + file + "\" OK";
                    }
                    else
                    {
                        textBox4.Text = "File \"" + file + "\" ERROR";
                        waserror = true;
                    }
                }
                if (waserror)
                {
                    radio = !waserror;
                    radioStateToolStripMenuItem1.Text = Properties.strings.RadioOn;
                    radioStateToolStripMenuItem2.Text = Properties.strings.RadioOn;
                    radioStateToolStripMenuItem1.Enabled = false;
                    radioStateToolStripMenuItem2.Enabled = false;
                }
            }
            catch
            {
                textBox4.Text = "Error getting file list from server.";
            }
            textBox4.Text = "Done. Have a nice day :)";
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
            button8.Text = Properties.strings.About;
            label3.Text = Properties.strings.File;
            label4.Text = Properties.strings.Disconnected;
            checkBox1.Text = Properties.strings.PasswordProtected;
            connectToolStripMenuItem1.Text = Properties.strings.Connect;
            serverToolStripMenuItem1.Text = Properties.strings.Server;
            listDirectoryToolStripMenuItem1.Text = Properties.strings.ListDir;
            uploadToolStripMenuItem1.Text = Properties.strings.Upload;
            downloadToolStripMenuItem1.Text = Properties.strings.Download;
            deleteToolStripMenuItem1.Text = Properties.strings.Delete;
            consoleToolStripMenuItem1.Text = Properties.strings.Console;
            clearToolStripMenuItem1.Text = Properties.strings.Clear;
            extrasToolStripMenuItem1.Text = Properties.strings.Extras;
            themesToolStripMenuItem1.Text = Properties.strings.Themes;
            quitToolStripMenuItem2.Text = Properties.strings.Quit;
            radioStateToolStripMenuItem2.Text = Properties.strings.RadioOff;
            radioStateToolStripMenuItem1.Text = Properties.strings.RadioOff;
            aboutToolStripMenuItem.Text = Properties.strings.About;
            quitToolStripMenuItem1.Text = Properties.strings.Quit;
            compressionToolToolStripMenuItem.Text = Properties.strings.CompressionTool;
            importFilelistToolStripMenuItem.Text = Properties.strings.Import;
            exportFileListToolStripMenuItem.Text = Properties.strings.Export;
            filesToolStripMenuItem.Text = Properties.strings.Files;
        }

        #region player

        public static void PlayMusicFromURL(string url)
        {
            Process.Start("yggplayer.exe", "--qt-start-minimized " + url + " -I null");
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

        #endregion

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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains("\n"))
            {
                textBox1.Text = textBox1.Text.Replace('\n', ' ');
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (connected && !new WebClient().DownloadString("http://" + textBox1.Text + "/deactivated").Contains("ls"))
            {
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

        private void button8_Click_1(object sender, EventArgs e)
        {
            About a = new About();
            if (!a.Visible)
            {
                a.Show();
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
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
                        label4.Text = Properties.strings.Connected;
                        label4.ForeColor = System.Drawing.Color.Green;
                        connectToolStripMenuItem1.Text = Properties.strings.Disconnect;
                        connected = true;
                        string motd = new WebClient().DownloadString("http://" + textBox1.Text + "/motd");
                        motd = motd.Replace("\n", Environment.NewLine);
                        byte[] bytes = Encoding.Default.GetBytes(motd);
                        motd = Encoding.UTF8.GetString(bytes);
                        richTextBox1.Text += motd + Environment.NewLine;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                        string deactivated = new WebClient().DownloadString("http://" + textBox1.Text + "/deactivated");
                        if (!deactivated.Contains("ls"))
                        {
                            listDirectoryToolStripMenuItem1.Enabled = true;
                        }
                        if (!deactivated.Contains("upload"))
                        {
                            uploadToolStripMenuItem1.Enabled = true;
                        }
                        if (!deactivated.Contains("download"))
                        {
                            downloadToolStripMenuItem1.Enabled = true;
                            importFilelistToolStripMenuItem.Enabled = true;
                        }
                        if (!deactivated.Contains("del"))
                        {
                            deleteToolStripMenuItem1.Enabled = true;
                        }
                        try
                        {
                            textBox4.Text = new WebClient().DownloadString("http://" + textBox1.Text + "/news");
                        }
                        catch { }
                        try
                        {
                            stream = new WebClient().DownloadString("http://" + textBox1.Text + "/stream").Split('\n')[0];
                            if (!stream.Contains("ERR_") && radio)
                            {
                                PlayMusicFromURL(stream);
                            }
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
                if (connected)
                {
                    richTextBox1.Text = "";
                }
                textBox1.Enabled = true;
                //button1.Text = Properties.strings.Connect;
                connectToolStripMenuItem1.Text = Properties.strings.Connect;
                label4.Text = Properties.strings.Disconnected;
                label4.ForeColor = System.Drawing.Color.Red;
                textBox4.Text = "";
                if (textBox1.Text.Contains(":82"))
                {
                    textBox1.Text = textBox1.Text.Split(':')[0];
                }
                listDirectoryToolStripMenuItem1.Enabled = false;
                uploadToolStripMenuItem1.Enabled = false;
                downloadToolStripMenuItem1.Enabled = false;
                deleteToolStripMenuItem1.Enabled = false;
                importFilelistToolStripMenuItem.Enabled = false;
                connected = false;
                try
                {
                    PlayerStop();
                }
                catch { }
            }
        }

        private void listDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
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
                        richTextBox1.Text += dir;
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

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerStop();
            Environment.Exit(0);
        }

        private void themesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Themes t = new Themes();
            if (!t.Visible)
            {
                t.Show();
            }
        }

        private void radioStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (radio)
                {
                    PlayerStop();
                    radio = false;
                    radioStateToolStripMenuItem2.Text = Properties.strings.RadioOn;
                    radioStateToolStripMenuItem1.Text = Properties.strings.RadioOn;
                }
                else
                {
                    radio = true;
                    radioStateToolStripMenuItem2.Text = Properties.strings.RadioOff;
                    radioStateToolStripMenuItem1.Text = Properties.strings.RadioOff;
                    if (connected)
                    {
                        PlayMusicFromURL(stream);
                    }
                }
            }
            catch { }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                connectToolStripMenuItem_Click("Yggdrasil", EventArgs.Empty);
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (!Visible && e.Button == MouseButtons.Left)
            {
                Show();
            }
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = true;
                Hide();
            }
            catch { }
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlayerStop();
            notifyIcon1.Visible = false;
            Environment.Exit(0);
        }

        private void radioStateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (radio)
                {
                    PlayerStop();
                    radio = false;
                    radioStateToolStripMenuItem.Text = Properties.strings.RadioOn;
                    radioStateToolStripMenuItem1.Text = Properties.strings.RadioOn;
                }
                else
                {
                    radio = true;
                    radioStateToolStripMenuItem.Text = Properties.strings.RadioOff;
                    radioStateToolStripMenuItem1.Text = Properties.strings.RadioOff;
                    if (connected)
                    {
                        PlayMusicFromURL(stream);
                    }
                }
            }
            catch { }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About a = new About();
            if (!a.Visible)
            {
                a.Show();
            }
        }

        private void compressionToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("peazip.exe", "-h");
        }

        private void importFilelistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (connected)
                {
                    Uri uri = new Uri("http://" + textBox1.Text + "/download");
                    openFileDialog1.Filter = "YggFile|*.ygg";
                    DialogResult result = openFileDialog1.ShowDialog();
                    passPhrase = textBox3.Text;
                    if (result == DialogResult.OK)
                    {
                        if (File.Exists(openFileDialog1.FileName))
                        {
                            string[] flist = File.ReadAllLines(openFileDialog1.FileName);
                            var temp = new List<string>();
                            foreach (var s in flist)
                            {
                                if (!string.IsNullOrEmpty(s))
                                    temp.Add(s);
                            }
                            flist = temp.ToArray();
                            DialogResult r = folderBrowserDialog1.ShowDialog();
                            if (r == DialogResult.OK)
                            {
                                foreach (string filename in flist)
                                {
                                    string downloadedfile = new WebClient().DownloadString("http://" + textBox1.Text + "/download?filename=" + filename + "&password=" + CalculateMD5Hash(passPhrase));
                                    downloadedfile = downloadedfile.Replace(' ', '+');
                                    string decrypted = Setup.Decrypt(downloadedfile);
                                    File.WriteAllText(folderBrowserDialog1.SelectedPath + "\\" + filename, decrypted, Encoding.Default);
                                }
                            }
                        }
                    }
                }
                openFileDialog1.Filter = "";
            } catch
            {
                MessageBox.Show(Properties.strings.CannotRead, Properties.strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportFileListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                string filelist = String.Join("\n", files).Replace(folderBrowserDialog1.SelectedPath, "").Replace("\\", "");
                saveFileDialog1.Filter = "YggFile|*.ygg";
                DialogResult r = saveFileDialog1.ShowDialog();
                if (r == DialogResult.OK)
                {
                    File.WriteAllText(saveFileDialog1.FileName, filelist);
                }
            }
            saveFileDialog1.Filter = "";
        }
    }
}
