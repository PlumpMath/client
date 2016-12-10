using System;
using System.Net;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Threading;

namespace Yggdrasil
{
    public partial class Main : Form
    {
        bool connected = false;
        string passPhrase = "";
        string saltValue = "YGDRSL13";
        string hashAlgorithm = "MD5";
        int passwordIterations = 7;
        string initVector = "~1B2c3D4e5F6g7H8";
        int keySize = 192;

        public Main()
        {
            InitializeComponent();
            this.Hide();
            Thread t = new Thread(new ThreadStart(splash));
            t.Start();
            Thread.Sleep(5000);
            this.textBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            this.textBox1.AutoCompleteSource = AutoCompleteSource.CustomSource;
            localize();
            t.Abort();
            this.Show();
            try
            {
                label1.Text = new WebClient().DownloadString("http://yggdrasilfs.neocities.org/msg.txt");
            } catch { }
        }

        public void splash()
        {
            Application.Run(new Splash());
        }

        private void localize()
        {
            button5.Text = Properties.strings.Download;
            button6.Text = Properties.strings.Remove;
            button3.Text = Properties.strings.Clear;
            button4.Text = Properties.strings.Upload;
            button2.Text = Properties.strings.ListDir;
            button1.Text = Properties.strings.Connect;
            checkBox1.Text = Properties.strings.PasswordProtected;
            button7.Text = Properties.strings.Quit;
            button8.Text = Properties.strings.Blog;
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (connected == false)
            {
                try
                {
                    string alive = new WebClient().DownloadString("http://" + textBox1.Text + "/alive");
                    if (alive == "OK")
                    {
                        textBox1.Enabled = false;
                        button1.Text = Properties.strings.Disconnect;
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
                button1.Text = Properties.strings.Connect;
                connected = false;
                textBox4.Text = "";
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
                    } else
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
                if (connected) {
                    var encryptedfile = "";
                    DialogResult result = openFileDialog1.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (System.IO.File.Exists(openFileDialog1.FileName))
                        {
                            encryptedfile = Encrypt(System.IO.File.ReadAllText(openFileDialog1.FileName, Encoding.Default));
                            Uri uri = new Uri("http://" + textBox1.Text + "/upload");
                            string filename = openFileDialog1.SafeFileName;
                            string downloadedfile = new WebClient().UploadString(uri, "content=" + encryptedfile + "&filename=" + filename + "&password=" + CalculateMD5Hash(passPhrase));
                            if (downloadedfile != "ERR_WRONG_PW")
                            {
                                richTextBox1.Text += "File \"" + openFileDialog1.SafeFileName + "\" uploaded.\n" + Environment.NewLine;
                            } else
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
                    string decrypted = Decrypt(downloadedfile);
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
            passPhrase = textBox3.Text;
            try
            {
                byte[] bytes = Encoding.Convert(Encoding.ASCII, Encoding.Default, Encoding.ASCII.GetBytes(textBox2.Text));
                string filename = Encoding.Default.GetString(bytes);
                string callback = new WebClient().DownloadString("http://" + textBox1.Text + "/del?filename=" + textBox2.Text + "&password=" + CalculateMD5Hash(passPhrase));
                if (callback != "ERR_WRONG_PW")
                {
                    richTextBox1.Text += "File \"" + textBox2.Text + "\" deleted.\n" + Environment.NewLine;
                    richTextBox1.SelectionStart = richTextBox1.Text.Length;
                    richTextBox1.ScrollToCaret();
                } else
                {
                    richTextBox1.Text += Properties.strings.WrongPassword + Environment.NewLine;
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

        #region Encryption

        public string Encrypt(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(this.initVector);
            byte[] rgbSalt = Encoding.UTF8.GetBytes(this.saltValue);
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            byte[] rgbKey = new PasswordDeriveBytes(this.passPhrase, rgbSalt, this.hashAlgorithm, this.passwordIterations).GetBytes(this.keySize / 8);
            RijndaelManaged managed = new RijndaelManaged();
            managed.Mode = CipherMode.CBC;
            ICryptoTransform transform = managed.CreateEncryptor(rgbKey, bytes);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            byte[] inArray = stream.ToArray();
            stream.Close();
            stream2.Close();
            return Convert.ToBase64String(inArray);
        }

        public string Decrypt(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(this.initVector);
            byte[] rgbSalt = Encoding.UTF8.GetBytes(this.saltValue);
            byte[] buffer = Convert.FromBase64String(data);
            byte[] rgbKey = new PasswordDeriveBytes(this.passPhrase, rgbSalt, this.hashAlgorithm, this.passwordIterations).GetBytes(this.keySize / 8);
            RijndaelManaged managed = new RijndaelManaged();
            managed.Mode = CipherMode.CBC;
            ICryptoTransform transform = managed.CreateDecryptor(rgbKey, bytes);
            MemoryStream stream = new MemoryStream(buffer);
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
            byte[] buffer5 = new byte[buffer.Length];
            int count = stream2.Read(buffer5, 0, buffer5.Length);
            stream.Close();
            stream2.Close();
            return Encoding.UTF8.GetString(buffer5, 0, count);
        }
        #endregion

        public void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox3.Enabled = true;
            } else
            {
                textBox3.Enabled = false;
                textBox3.Text = "";
                passPhrase = "";
            }
        }

        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Blog b = new Blog();
            if (!b.Visible)
            {
                b.Show();
            }
        }
    }
}
