using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Volte.Utils
{
 public class CRC32
    {
        protected ulong[] Crc32Table;
        //生成CRC32码表
        private void Initialize()
        {
            ulong Crc;
            Crc32Table = new ulong[256];
            int i, j;
            for (i = 0; i < 256; i++)
            {
                Crc = (ulong)i;
                for (j = 8; j > 0; j--)
                {
                    if ((Crc & 1) == 1)
                        Crc = (Crc >> 1) ^ 0xEDB88320;
                    else
                        Crc >>= 1;
                }
                Crc32Table[i] = Crc;
            }
        }

        //获取字符串的CRC32校验值
        public ulong CRC32Hash(string sInputString)
        {
            //生成码表
            Initialize();
            byte[] buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(sInputString);
            ulong value = 0xffffffff;
            int len = buffer.Length;
            for (int i = 0; i < len; i++)
            {
                value = (value >> 8) ^ Crc32Table[(value & 0xFF) ^ buffer[i]];
            }
            return value ^ 0xffffffff;
        }
        public string ToBase34(string sInputString)
        {
            return ToBase34(CRC32Hash(sInputString));
        }
        public string ToBase34(ulong num)
        {
            if (num < 0)
            {
                return "";
            }
            //10进制数所对应的34进制数
            char[] rule = new char[] {'0','1','2','3','4','5','6','7','8','9',
                              'A','B','C','D','E','F','G','H','J','K',
                              'L','M','N','P','Q','R','S','T','U','V',
                              'W','X','Y','Z',};
            ulong nBase = 34;
            //保存除34后的余数
            List<ulong> list = new List<ulong>();
            while (num >= 34)
            {
                ulong a = num % nBase;
                num /= nBase;
                list.Add(a);
            }
            list.Add(num);

            StringBuilder sb = new StringBuilder();
            //结果要从后往前排
            for (int i = list.Count - 1; i >= 0; i--)
            {
                sb.Append(rule[list[i]]);
            }
            return sb.ToString();
        }
    }
}