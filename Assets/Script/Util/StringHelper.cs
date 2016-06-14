using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;

    public class StringHelper
    {
        static StringBuilder sb = new StringBuilder(1024);
        public static string Concat(string strA, string strB)
        {
            sb.Length = 0;
            sb.Append(strA);
            sb.Append(strB);

            return sb.ToString();
        }

        //去除一个字符串里面的所有/n/t/r空格
        //返回一定清除了几个
        //参数2 是否删除 /r
        //参数3 是否删除 /t
        //参数4 是否删除 空格
        //参数5 是否删除 /n
        public static void Strip(ref string strOut, bool bR, bool bT, bool bSpace, bool bN, bool bLK, bool bRK, bool bC)
        {
            if (bR)
            {
                strOut.Replace("\r", "");
            }
            if (bT)
            {
                strOut.Replace("\t", "");
            }
            if (bSpace)
            {
                strOut.Replace(" ", "");
            }
            if (bN)
            {
                strOut.Replace("\n", "");
            }
            if (bLK)
            {
                strOut.Replace("<", "");
            }
            if (bRK)
            {
                strOut.Replace(">", "");
            }
            if (bC)
            {
                strOut.Replace("\"", "");
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void StandardPath(ref string strPath)
        {
            if (strPath == null && strPath.Length == 0)
            {
                return;
            }
            strPath.Replace("//", "\\");
            strPath.Replace("/", "\\");
            //如果最后没有\\同时又没有.那么这个路径还不完整在拼接一个
            if (strPath[(strPath.Length - 1)] != '\\')
            {
                int iLastDot = strPath.LastIndexOf('.');
                //找不到点或者点是第一个。
                if (iLastDot <= 0)
                {
                    //找不到点肯定不是全路径了
                    strPath += "\\";
                }
            }
            strPath.ToLower();
        }

        public static string getUnicodeStringByBytes(ref byte[] byArray)
        {
            return Encoding.Unicode.GetString(byArray, 0, Strlen(ref byArray));
        }

        //------------------------------------------------------------------------------------------------------
        //  [9/24/2013 Fisho]
        public static string getUTF8StringByBytes(ref byte[] byArray)
        {
            return Encoding.UTF8.GetString(byArray, 0, Strlen(ref byArray));
        }

        public static string GBKToUTF8String(byte[] byGBK)
        {
            byGBK = Encoding.Convert(Encoding.Default, Encoding.UTF8, byGBK);
            return Encoding.UTF8.GetString(byGBK);
        }

        public static byte[] GBKToUTF8(byte[] byGBK)
        {
            return Encoding.Convert(Encoding.Default, Encoding.UTF8, byGBK);
        }

        public static byte[] UTF8ToGBK(byte[] byUTF8)
        {
            return Encoding.Convert(Encoding.UTF8, Encoding.Default, byUTF8);
        }
        public static byte[] UTF8ToGBK(string strUTF8)
        {
            return Encoding.Convert(Encoding.UTF8, Encoding.Default, Encoding.UTF8.GetBytes(strUTF8));
        }

        public static int Strlen(ref byte[] byArray)
        {
            int nLen = 0;
            foreach (byte by in byArray)
            {
                if (by == 0)
                    break;
                nLen++;
            }
            return nLen;
        }
        //------------------------------------------------------------------------------------------------------
    }
