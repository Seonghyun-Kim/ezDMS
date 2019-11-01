using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IS_PODS.Class
{
    public class Decryption
    {
        public static string DecryptData(string sData)
        {
            return UrlDecode(AsciiToUtf8(Base64_Decode(sData)));
        }

        private static string Base64_Decode(string sData)
        {
            byte[] decbuff = Convert.FromBase64String(sData);
            return System.Text.Encoding.UTF8.GetString(decbuff);
        } 

        private static string AsciiToUtf8(string sData)
        {
            string sUtf8 = "";

            string[] sarAsciiList = sData.Split('/');

            for (int iCnt = 0; iCnt < sarAsciiList.Length; iCnt++)
            {
                int iOneAscii = Convert.ToInt32(sarAsciiList[iCnt]);

                sUtf8 = sUtf8 + Convert.ToChar(iOneAscii).ToString();
            }

            return sUtf8;
        }

        private static string UrlDecode(string sData)
        {
            string sOriginalData = System.Web.HttpUtility.UrlDecode(sData);

            return sOriginalData;
        }
    }
}