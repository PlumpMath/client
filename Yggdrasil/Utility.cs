using System;
using System.Text;
using System.Security.Cryptography;

namespace Yggdrasil
{
    static class Utility
    {
        internal static string GetHexString(byte[] byIn)
        {
            string strOut = "";
            foreach (byte someByte in byIn)
            {
                strOut += someByte.ToString("X2");
            }
            return strOut;
        }

        internal static byte[] GetBytesFromHex(string strIn)
        {
            byte[] byOut = new byte[strIn.Length / 2];
            for (int i = 0; i < strIn.Length; i += 2)
            {
                byOut[i / 2] = Convert.ToByte(strIn.Substring(i, 2), 16);
            }
            return byOut;
        }

        internal static string GetRandomString(int length)
        {
            RNGCryptoServiceProvider RNGsus = new RNGCryptoServiceProvider();
            Random random = new Random();
            string strOut = "";
            string strTestSanity = "";
            bool IsSane;
            do
            {
                for (int i = 0; i < length; i++)
                {
                    byte[] byChar = new byte[2];
                    RNGsus.GetBytes(byChar);
                    char myChar = BitConverter.ToChar(byChar, 0);
                    if (myChar >= 55269 && myChar <= 57343)
                    {
                        if (myChar <= 56319)
                        {
                            char otherChar = (char)random.Next(56320, 57344);
                            strOut += myChar + otherChar;
                        }
                        else
                        {
                            char otherChar = (char)random.Next(55269, 56320);
                            strOut += otherChar + myChar;
                        }
                    }
                    else if (myChar == '\t' || myChar == '\n' || myChar == '\r')
                    {
                        i--;
                    }
                    else if (myChar < 20)
                    {
                        i--;
                    }
                    else
                    {
                        strOut += myChar;
                    }
                }
                byte[] byOut = Encoding.Unicode.GetBytes(strOut);
                strTestSanity = Encoding.Unicode.GetString(byOut);
                if (strTestSanity.Split('\t').Length != 1)
                {
                    IsSane = false;
                }
                else if (strOut != strTestSanity)
                {
                    IsSane = false;
                }
                else
                {
                    Byte[] bySanaty = Encoding.Unicode.GetBytes(strTestSanity);
                    if (byOut.Length == bySanaty.Length)
                    {
                        IsSane = true;
                        for (int i = 0; i < byOut.Length; i++)
                        {
                            if (byOut[i] != bySanaty[i])
                            {
                                IsSane = false;
                                break;
                            }
                        }
                        if (strOut.Length != strTestSanity.Length)
                        {
                            IsSane = false;
                        }
                        else
                        {
                            for (int i = 0; i < strTestSanity.Length; i++)
                            {
                                if (strOut[i] != strTestSanity[i])
                                {
                                    IsSane = false;
                                    break;
                                }
                            }
                        }
                    }
                    else
                        IsSane = false;
                }
            } while (!IsSane);
            return strTestSanity;
        }
    }
}
