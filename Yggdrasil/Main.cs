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

namespace Yggdrasil
{
    public partial class Main : Form
    {
        bool connected = false;
        string passPhrase = "";
        string stream;
        bool radio = true;
        Ads af = new Ads();
        About a = new About();
        Thread refresh;
        string version = "1.1.0";

        public Main()
        {
            InitializeComponent();
            Thread t = new Thread(new ThreadStart(splash));
            t.Start();
            Hide();
            ygginit();
            localize();
            Thread.Sleep(420); //If you know, what I mean ;)
            refresh = new Thread(check);
            t.Abort();
            refresh.Start();
            this.TransparencyKey = this.BackColor;
            this.Show();
            /*
            af.Show();
            this.CenterToScreen();
            af.Left = this.Left + 20;
            af.Top = this.Top;
            af.SetBounds(af.Bounds.X, this.Bounds.Y, af.Width, af.Height);
            */
            try
            {
                Stream str = Properties.sounds._in;
                SoundPlayer snd = new SoundPlayer(str);
                snd.Play();
            }
            catch { }
            this.textBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.textBox2.AutoCompleteSource = AutoCompleteSource.CustomSource;
            listDirectoryToolStripMenuItem1.Enabled = false;
            uploadToolStripMenuItem1.Enabled = false;
            downloadToolStripMenuItem1.Enabled = false;
            deleteToolStripMenuItem1.Enabled = false;
            importFilelistToolStripMenuItem.Enabled = false;
            massUploadToolStripMenuItem.Enabled = false;
            listBox1.Enabled = false;
        }

        //Global variables;
        private bool _dragging = false;
        //private Point _offset;
        private Point _start_point = new Point(0, 0);

        private void check()
        {
            while (true)
            {
                Thread.Sleep(20);
                if (connected)
                {
                    if (textBox1.Text.Contains(":82"))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            try
                            {

                                new WebClient().DownloadString("http://" + textBox1.Text + "/alive");
                            }
                            catch
                            {
                                disconnect();
                                MessageBox.Show(Properties.strings.ErrorDisconnect, Properties.strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        });
                    }
                }
                Thread.Sleep(1000);
            }
        }
        public void disconnect()
        {
            try
            {
                richTextBox1.Text = "";
                connected = false;
                textBox1.Enabled = true;
                textBox2.Text = "";
                listBox1.Items.Clear();
                listBox1.Enabled = false;
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
                massUploadToolStripMenuItem.Enabled = false;
                try
                {
                    PlayerStop();
                }
                catch { }
                try
                {
                    string ads = new WebClient().DownloadString("https://koyuawsmbrtn.keybase.pub/yggdrasil/msg_1000.txt");
                    ads = ads.Replace("\n", Environment.NewLine);
                    richTextBox1.Text += ads + Environment.NewLine;
                }
                catch { }
                try
                {
                    Stream str = Properties.sounds.off;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();
                }
                catch { }
                Thread.Sleep(1500);
            }
            catch { }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;  // _dragging is your variable flag
            _start_point = new Point(e.X, e.Y);
            af.SetBounds(af.Bounds.X, af.Bounds.Y, af.Width, af.Height);
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
            af.SetBounds(af.Bounds.X, af.Bounds.Y, af.Width, af.Height);
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
                string ads = new WebClient().DownloadString("https://koyuawsmbrtn.keybase.pub/yggdrasil/msg_1000.txt");
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
                new WebClient().DownloadFile("https://koyuawsmbrtn.keybase.pub/yggdrasil/Launcher.exe", "Launcher.exe");
            }
            catch
            {
                textBox4.Text = "Error getting patch from Launcher.";
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
            File.WriteAllText("verinfo", version);
            if (File.Exists("ygg_bgimage.conf"))
            {
                try
                {
                    this.BackgroundImage = Image.FromFile(File.ReadAllLines("ygg_bgimage.conf")[0]);
                }
                catch
                {
                    File.Delete("ygg_bgimage.conf");
                }
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
            label1.Text = Properties.strings.Files + ":";
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
            quitToolStripMenuItem2.Text = Properties.strings.Quit;
            radioStateToolStripMenuItem2.Text = Properties.strings.RadioOff;
            radioStateToolStripMenuItem1.Text = Properties.strings.RadioOff;
            aboutToolStripMenuItem.Text = Properties.strings.About;
            quitToolStripMenuItem1.Text = Properties.strings.Quit;
            compressionToolToolStripMenuItem.Text = Properties.strings.CompressionTool;
            importFilelistToolStripMenuItem.Text = Properties.strings.Import;
            exportFileListToolStripMenuItem.Text = Properties.strings.Export;
            filesToolStripMenuItem.Text = Properties.strings.Files;
            massUploadToolStripMenuItem.Text = Properties.strings.MassUpload;
            themesToolStripMenuItem.Text = Properties.strings.Themes;
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
                    PlayerStop();
                }
                catch { }
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
                        listBox1.Enabled = true;
                        listBox1.Items.Clear();
                        string[] dir = new WebClient().DownloadString("http://" + textBox1.Text + "/ls").Split('\n');
                        var temp = new List<string>();
                        foreach (var s in dir)
                        {
                            if (!string.IsNullOrEmpty(s))
                                temp.Add(s);
                        }
                        dir = temp.ToArray();
                        foreach (string item in dir)
                        {
                            listBox1.Items.Add(item);
                        }
                        if (!deactivated.Contains("ls"))
                        {
                            listDirectoryToolStripMenuItem1.Enabled = true;
                        }
                        if (!deactivated.Contains("upload"))
                        {
                            uploadToolStripMenuItem1.Enabled = true;
                            massUploadToolStripMenuItem.Enabled = true;
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
                        try
                        {
                            Stream str = Properties.sounds.on;
                            SoundPlayer snd = new SoundPlayer(str);
                            snd.Play();
                        }
                        catch { }
                    }
                    else
                    {
                        richTextBox1.Text += Properties.strings.NotAlive + "\n" + Environment.NewLine;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                    }
                }
                catch
                {
                    richTextBox1.Text += Properties.strings.NotAlive + "\n" + Environment.NewLine;
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                }
            }
            else
            {
                disconnect();
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
                            listBox1.Items.Clear();
                            string[] dir = new WebClient().DownloadString("http://" + textBox1.Text + "/ls").Split('\n');
                            var temp = new List<string>();
                            foreach (var s in dir)
                            {
                                if (!string.IsNullOrEmpty(s))
                                    temp.Add(s);
                            }
                            dir = temp.ToArray();
                            foreach (string item in dir)
                            {
                                listBox1.Items.Add(item);
                            }
                            richTextBox1.SelectionStart = richTextBox1.Text.Length;
                            richTextBox1.ScrollToCaret();
                        }
                    }
                }
            }
            catch
            {
                richTextBox1.Text += "Error encrypting or uploading file!\n" + Environment.NewLine;
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
            catch
            {
                richTextBox1.Text += Properties.strings.ErrorDownload + "\n" + Environment.NewLine;
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
                    passPhrase = textBox3.Text;
                    string callback = new WebClient().DownloadString("http://" + textBox1.Text + "/del?filename=" + textBox2.Text + "&password=" + CalculateMD5Hash(passPhrase));
                    if (callback == "OK")
                    {
                        richTextBox1.Text += "File \"" + filename + "\" deleted\n\n" + "\n" + Environment.NewLine;
                        int nextitem = listBox1.SelectedIndex;
                        listBox1.Items.Clear();
                        string[] dir = new WebClient().DownloadString("http://" + textBox1.Text + "/ls").Split('\n');
                        var temp = new List<string>();
                        foreach (var s in dir)
                        {
                            if (!string.IsNullOrEmpty(s))
                                temp.Add(s);
                        }
                        dir = temp.ToArray();
                        foreach (string item in dir)
                        {
                            listBox1.Items.Add(item);
                        }
                        try
                        {
                            listBox1.SetSelected(nextitem, true);
                            textBox2.Text = listBox1.Items[nextitem].ToString();
                        }
                        catch { }
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                    }
                    else if (callback == "ERR_WRONG_PW")
                    {
                        richTextBox1.Text += Properties.strings.WrongPassword + Environment.NewLine;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                    }
                }
            }
            catch
            {
                richTextBox1.Text += Properties.strings.ErrorDelete + "\n" + Environment.NewLine;
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
            notifyIcon1.Visible = false;
            Hide();
            try
            {
                Stream str = Properties.sounds._out;
                SoundPlayer snd = new SoundPlayer(str);
                snd.Play();
            }
            catch { }
            Thread.Sleep(1500);
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
                try
                {
                    Stream str = Properties.sounds._out;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();
                }
                catch { }
            }
            catch { }
        }

        private void quitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlayerStop();
            notifyIcon1.Visible = false;
            Hide();
            try
            {
                Stream str = Properties.sounds._out;
                SoundPlayer snd = new SoundPlayer(str);
                snd.Play();
            }
            catch { }
            Thread.Sleep(1500);
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
                            try
                            {
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
                            catch
                            {
                                richTextBox1.Text += Properties.strings.ErrorDownload + "\n" + Environment.NewLine;
                                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                                richTextBox1.ScrollToCaret();
                            }
                        }
                    }
                }
                openFileDialog1.Filter = "";
            }
            catch
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

        private void massUploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (connected)
                {
                    DialogResult result = folderBrowserDialog1.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string[] flist = Directory.GetFiles(folderBrowserDialog1.SelectedPath);
                        var temp = new List<string>();
                        foreach (var s in flist)
                        {
                            if (!string.IsNullOrEmpty(s))
                                temp.Add(s);
                        }
                        flist = temp.ToArray();
                        foreach (string f in flist)
                        {
                            string passPhrase = textBox3.Text;
                            string encryptedfile = Setup.Encrypt(System.IO.File.ReadAllText(f, Encoding.Default));
                            Uri uri = new Uri("http://" + textBox1.Text + "/upload");
                            string filename = f.Replace(folderBrowserDialog1.SelectedPath, "").Replace("\\", "");
                            string downloadedfile = new WebClient().UploadString(uri, "content=" + encryptedfile + "&filename=" + filename + "&password=" + CalculateMD5Hash(passPhrase));
                        }
                        listBox1.Items.Clear();
                        string[] dir = new WebClient().DownloadString("http://" + textBox1.Text + "/ls").Split('\n');
                        var tmp = new List<string>();
                        foreach (var s in dir)
                        {
                            if (!string.IsNullOrEmpty(s))
                                tmp.Add(s);
                        }
                        dir = tmp.ToArray();
                        foreach (string item in dir)
                        {
                            listBox1.Items.Add(item);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(Properties.strings.CannotRead, Properties.strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string deactivated = "del";
            try
            {
                deactivated = new WebClient().DownloadString("http://" + textBox1.Text + "/deactivated");
            }
            catch { }
            if (connected && !deactivated.Contains("del"))
            {
                textBox2.Text = listBox1.GetItemText(listBox1.SelectedItem);
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            string deactivated = "del";
            try
            {
                deactivated = new WebClient().DownloadString("http://" + textBox1.Text + "/deactivated");
            }
            catch { }
            if (e.KeyCode == Keys.Delete && connected && !deactivated.Contains("del"))
            {
                try
                {
                    deleteToolStripMenuItem_Click("Yggdrasil", EventArgs.Empty);
                    textBox2.Text = "";
                }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            af.Hide();
            Hide();
        }
    }
}
