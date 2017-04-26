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
        About a = new About();
        Thread refresh;
        string version = "1.1.4";
        public static string cwd = Directory.GetCurrentDirectory();
        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "FreeBrowse\\freebrowse.exe";
        string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "FreeBrowse\\";

        public Main()
        {
            InitializeComponent();
            Hide();
            if (File.Exists("lock"))
            {
                MessageBox.Show("Yggdrasil is already running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            Thread t = new Thread(new ThreadStart(splash));
            t.Start();
            ygginit();
            localize();
            Thread.Sleep(420);
            refresh = new Thread(check);
            t.Abort();
            this.TransparencyKey = this.BackColor;
            this.Show();
            try
            {
                Stream str = Properties.sounds._in;
                SoundPlayer snd = new SoundPlayer(str);
                snd.Play();
            }
            catch { }
            listDirectoryToolStripMenuItem1.Enabled = false;
            uploadToolStripMenuItem1.Enabled = false;
            downloadToolStripMenuItem1.Enabled = false;
            deleteToolStripMenuItem1.Enabled = false;
            importFilelistToolStripMenuItem.Enabled = false;
            massUploadToolStripMenuItem.Enabled = false;
            listBox1.Enabled = false;
            textBox1.Focus();
            File.Create("lock");
        }

        private bool _dragging = false;
        private Point _start_point = new Point(0, 0);

        public int WebClientUploadProgressChanged { get; private set; }

        private void check()
        {
            while (true)
            {
                Thread.Sleep(20);
                if (connected)
                {
                    try
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
                                    MsgBox msg = new MsgBox(Properties.strings.ErrorDisconnect);
                                    msg.ShowDialog();
                                }
                            });
                        }
                    }
                    catch { }
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
                    Stream str = Properties.sounds.off;
                    SoundPlayer snd = new SoundPlayer(str);
                    snd.Play();
                }
                catch { }
                Thread.Sleep(1500);
            }
            catch { }
        }

        private void resetBox()
        {
            try
            {
                string ads = new WebClient().DownloadString("https://koyuawsmbrtn.keybase.pub/yggdrasil/msg_1000.txt");
                ads = ads.Replace("\n", Environment.NewLine);
                richTextBox1.Text = ads + Environment.NewLine;
            }
            catch { }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _dragging = true;
            _start_point = new Point(e.X, e.Y);
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
        }

        public void splash()
        {
            Application.Run(new Splash());
        }

        public void ygginit()
        {
            if (File.Exists("monoicons"))
            {
                useMonochromeIconsToolStripMenuItem.Checked = true;
                notifyIcon1.Icon = Properties.icons.logo_new_big_mono;
            }
            else
            {
                useMonochromeIconsToolStripMenuItem.Checked = false;
                notifyIcon1.Icon = Properties.icons.logo_new;
                this.Icon = Properties.icons.logo_new;
            }
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
                new WebClient().DownloadString("https://koyuawsmbrtn.keybase.pub/yggdrasil/latest.txt");
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
            button8.Text = Properties.strings.About;
            label3.Text = Properties.strings.File;
            label4.Text = Properties.strings.Disconnected;
            label1.Text = Properties.strings.Files + ":";
            checkBox1.Text = Properties.strings.PasswordProtected;
            connectToolStripMenuItem1.Text = Properties.strings.Connect;
            serverToolStripMenuItem1.Text = Properties.strings.Server;
            listDirectoryToolStripMenuItem1.Text = Properties.strings.RefreshFileList;
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
            useMonochromeIconsToolStripMenuItem.Text = Properties.strings.MonoIcons;
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

        private void button8_Click_1(object sender, EventArgs e)
        {
            a.ShowDialog();
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Directory.SetCurrentDirectory(cwd);
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
                            PlayMusicFromURL(stream);
                        }
                        catch { }
                        try
                        {
                            Stream str = Properties.sounds.on;
                            SoundPlayer snd = new SoundPlayer(str);
                            snd.Play();
                            refresh.Start();
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
                }
                catch
                {
                    richTextBox1.Text += "Cannot list directory!";
                }
            }
            textBox2.Text = "";
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var encryptedfile = "";
            DialogResult res = openFileDialog1.ShowDialog();
            WebClient wc = new WebClient();
            if (res == DialogResult.OK)
            {
                if (System.IO.File.Exists(openFileDialog1.FileName))
                {
                    encryptedfile = Setup.Encrypt(System.IO.File.ReadAllText(openFileDialog1.FileName, Encoding.Default));
                    passPhrase = textBox3.Text;
                    string filename = openFileDialog1.SafeFileName;
                    wc.UploadStringAsync(new Uri("http://" + textBox1.Text + "/upload"), "content=" + encryptedfile + "&filename=" + filename + "&password=" + CalculateMD5Hash(passPhrase));
                    wc.UploadProgressChanged += (s, ee) =>
                    {
                        progressBar1.Visible = true;
                        progressBar1.Value = ee.ProgressPercentage;
                    };
                    wc.UploadStringCompleted += (s, ee) =>
                    {
                        progressBar1.Visible = false;
                    };
                }
            }
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted1);
        }

        private void wc_UploadStringCompleted1(object sender, UploadStringCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            try
            {
                string downloadedfile = e.Result;
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
            catch
            {
                richTextBox1.Text += Properties.strings.ErrorUpload + "\n" + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }

        string ff;

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = textBox2.Text;
            ff = filename;
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wb_DownloadStringCompleted);
            webClient.DownloadProgressChanged += (s, ee) =>
            {
                progressBar1.Visible = true;
                progressBar1.Value = ee.ProgressPercentage;
            };
            webClient.DownloadFileCompleted += (s, ee) =>
            {
                progressBar1.Visible = false;
            };
            passPhrase = textBox3.Text;
            webClient.DownloadStringAsync(new Uri("http://" + textBox1.Text + "/download?filename=" + filename + "&password=" + CalculateMD5Hash(passPhrase)));
        }

        private void wb_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            try
            {
                if (connected)
                {
                    DialogResult result = folderBrowserDialog1.ShowDialog();
                    if (result == DialogResult.OK && Directory.Exists(folderBrowserDialog1.SelectedPath))
                    {
                        string downloadedfile = e.Result;
                        downloadedfile = downloadedfile.Replace(' ', '+');
                        string decrypted = Setup.Decrypt(downloadedfile);
                        System.IO.File.WriteAllText(folderBrowserDialog1.SelectedPath + "\\" + ff, decrypted, Encoding.Default);
                        richTextBox1.Text += "File \"" + textBox2.Text + "\" downloaded.\n" + Environment.NewLine;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
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

        private void wb_DownloadFreeBrowseCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            try
            {
                string downloadedfile = e.Result;
                downloadedfile = downloadedfile.Replace(' ', '+');
                string decrypted = Setup.Decrypt(downloadedfile);
                System.IO.File.WriteAllText("setup_freebrowse.exe", decrypted, Encoding.Default);
                richTextBox1.Text += "File \"setup_freebrowse.exe\" downloaded.\n" + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
                Process.Start("setup_freebrowse.exe");
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
            t.ShowDialog();
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
                e.SuppressKeyPress = true;
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
            File.Delete("lock");
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
            File.Delete("lock");
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
            a.ShowDialog();
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
                MsgBox ms = new MsgBox(Properties.strings.CannotRead);
                ms.ShowDialog();
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
                MsgBox ms = new MsgBox(Properties.strings.CannotRead);
                ms.ShowDialog();
            }
        }

        private void listBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            string deactivated = "del";
            try
            {
                if (textBox1.Text.Contains(":82"))
                {
                    deactivated = new WebClient().DownloadString("http://" + textBox1.Text + "/deactivated");
                }
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
                    textBox2.Text = listBox1.GetItemText(listBox1.SelectedItem);
                }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void freeBrowseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(path))
                {
                    Directory.SetCurrentDirectory(path2);
                    Process.Start(path);
                }
                else
                {
                    Directory.SetCurrentDirectory(cwd);
                    DialogResult dr = MessageBox.Show(Properties.strings.NoFreeBrowse, Properties.strings.Error, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (dr == DialogResult.Yes)
                    {
                        MessageBox.Show(Properties.strings.FreeBrowseWait);
                        string filename = "setup_freebrowse.exe";
                        ff = filename;
                        WebClient webClient = new WebClient();
                        webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wb_DownloadFreeBrowseCompleted);
                        webClient.DownloadProgressChanged += (s, ee) =>
                        {
                            progressBar1.Visible = true;
                            progressBar1.Value = ee.ProgressPercentage;
                        };
                        webClient.DownloadFileCompleted += (s, ee) =>
                        {
                            progressBar1.Visible = false;
                        };
                        passPhrase = textBox3.Text;
                        webClient.DownloadStringAsync(new Uri("https://koyuawsmbrtn.keybase.pub/yggdrasil/setup_freebrowse.exe"));
                    }
                }
            }
            catch
            {
                MessageBox.Show(Properties.strings.FreeBrowseError, Properties.strings.Error);
            }
        }

        private void useMonochromeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("monoicons"))
            {
                Directory.SetCurrentDirectory(cwd);
                notifyIcon1.Icon = Properties.icons.logo_new_big_mono;
                notifyIcon1.Visible = false;
                notifyIcon1.Visible = true;
                File.WriteAllText("monoicons", "");
                useMonochromeIconsToolStripMenuItem.Checked = true;
            }
            else
            {
                notifyIcon1.Icon = Properties.icons.logo_new;
                notifyIcon1.Visible = false;
                notifyIcon1.Visible = true;
                File.Delete("monoicons");
                useMonochromeIconsToolStripMenuItem.Checked = false;
            }
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            WebClient wc = new WebClient();
            string[] list = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string filename = list[0];
            ff = Path.GetFileName(filename);
            if (File.Exists(filename) && connected)
            {
                string encryptedfile = Setup.Encrypt(System.IO.File.ReadAllText(filename, Encoding.Default));
                passPhrase = textBox3.Text;
                wc.UploadStringAsync(new Uri("http://" + textBox1.Text + "/upload"), "content=" + encryptedfile + "&filename=" + ff + "&password=" + CalculateMD5Hash(passPhrase));
                wc.UploadProgressChanged += (s, ee) =>
                {
                    progressBar1.Visible = true;
                    progressBar1.Value = ee.ProgressPercentage;
                };
                wc.UploadStringCompleted += (s, ee) =>
                {
                    progressBar1.Visible = false;
                };
            }
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler(wc_UploadStringCompleted2);
        }

        private void wc_UploadStringCompleted2(object sender, UploadStringCompletedEventArgs e)
        {
            progressBar1.Visible = false;
            try
            {
                string downloadedfile = e.Result;
                if (downloadedfile != "ERR_WRONG_PW")
                {
                    richTextBox1.Text += "File \"" + ff + "\" uploaded.\n" + Environment.NewLine;
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
            catch
            {
                richTextBox1.Text += Properties.strings.ErrorUpload + "\n" + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            }
        }

        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }
}
