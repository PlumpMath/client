using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Yggdrasil
{
    public partial class Setup
    {
        private static byte[] Key = Utility.GetBytesFromHex("CE5684A9C793E11C185928855A0BBCA31B68C1711BF67BB8CDDEFABBD0FE7B72");
        private static byte[] IV = Utility.GetBytesFromHex("2E1906762915677DFF5B153DDBABCC8F");

        public static string Encrypt(string plain)
        {
            byte[] byIn = Encoding.Unicode.GetBytes(plain);
            short shLength = (short)byIn.Length;
            byte[] byLength = BitConverter.GetBytes(shLength);
            AesCryptoServiceProvider myAES = new AesCryptoServiceProvider();
            myAES.Key = Key;
            myAES.IV = IV;
            byte[] byOut;
            ICryptoTransform myEncrypt = myAES.CreateEncryptor(myAES.Key, myAES.IV);
            using (MemoryStream msCrypt = new MemoryStream())
            {
                using (CryptoStream csCrypt = new CryptoStream(msCrypt, myEncrypt, CryptoStreamMode.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(csCrypt))
                    {
                        bw.Write(byIn);
                    }
                    byOut = msCrypt.ToArray();
                }
            }
            myEncrypt.Dispose();
            myAES.Dispose();
            string strOut = Convert.ToBase64String(byOut);
            return strOut;
        }

        public static string Decrypt(string encrypted)
        {
            byte[] byIn = Convert.FromBase64String(encrypted);
            AesCryptoServiceProvider myAES = new AesCryptoServiceProvider();
            myAES.Key = Key;
            myAES.IV = IV;
            byte[] byOut;
            ICryptoTransform myDecrypt = myAES.CreateDecryptor(myAES.Key, myAES.IV);
            using (MemoryStream msCrypt = new MemoryStream())
            {
                using (CryptoStream csCrypt = new CryptoStream(msCrypt, myDecrypt, CryptoStreamMode.Write))
                {
                    using (BinaryWriter bw = new BinaryWriter(csCrypt))
                    {
                        bw.Write(byIn);
                    }
                    byOut = msCrypt.ToArray();
                }
            }
            myDecrypt.Dispose();
            myAES.Dispose();
            string strOut = Encoding.Unicode.GetString(byOut);
            return strOut;
        }
    }
}
