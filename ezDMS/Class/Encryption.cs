using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IS_PODS.Class
{
    public static class Encryption
    {
        public static string EncryptData(string sData)
        {
            return Base64_Encode(Utf8ToAscii(UrlEncode(sData)));
        }

        private static string UrlEncode(string sData)
        {
            byte[] barData = Encoding.UTF8.GetBytes(sData);

            string sUrlData = "";

            foreach (byte bWord in barData)
            {
                // PHP UrlEncode 방법
                // .Net은 한글을 소문자로 변환하고 특수문자중 !, ', (, ), *, =, ., _ 를 제외하기에 수정함 
                if (Convert.ToInt32(bWord) > 127 || Convert.ToChar(bWord) == '-' || Convert.ToChar(bWord) == '.' || Convert.ToChar(bWord) == '_')
                {
                    sUrlData += "%" + string.Format("{0:X}", bWord);
                }
                else
                {
                    sUrlData += Convert.ToChar(bWord);
                }
            }

            return sUrlData;
        }

        private static string Utf8ToAscii(string sData)
        {
            string sAsciiData = "";

            for (int iCnt = 0; iCnt < sData.Length; iCnt++)
            {
                char sOneChar = Convert.ToChar(sData.Substring(iCnt, 1));
                string sOneAscii = Convert.ToInt32(sOneChar).ToString();

                if (iCnt == 0)
                {
                    sAsciiData = sOneAscii;
                }
                else
                {
                    sAsciiData = sAsciiData + "/" + sOneAscii;
                }
            }
            return sAsciiData;
        }

        private static string Base64_Encode(string sData)
        {
            return Convert.ToBase64String(System.Text.Encoding.GetEncoding("euc-kr").GetBytes(sData));
        }

    }
}