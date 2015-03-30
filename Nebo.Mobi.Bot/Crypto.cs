using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebo.Mobi.Bot
{
    public class Crypto
    {
        public static string EncryptStr(string p)
        {
            string tmp = "";
            char[] m = p.ToCharArray();
            for (int i = 0; i < m.Length; i++)
            {
                tmp += string.Format("{0:X3}", Convert.ToInt32(m[i]));
            }
            return tmp;
        }

        public static string DecryptStr(string p)
        {
            UTF8Encoding utf = new UTF8Encoding();
            string tmp = "";
            char[] m = p.ToCharArray();
            for (int i = 0; i < m.Length; i += 3)
            {
                int q = 0;
                if (Char.IsDigit(m[i])) q += m[i] - '0';
                else q += m[i] - '0' - 7;
                q *= 16;
                if (Char.IsDigit(m[i + 1])) q += m[i + 1] - '0';
                else q += m[i + 1] - '0' - 7;
                q *= 16;
                if (Char.IsDigit(m[i + 2])) q += m[i + 2] - '0';
                else q += m[i + 2] - '0' - 7;
                tmp += Convert.ToChar(q);
            }
            return tmp;
        }
    }
}
