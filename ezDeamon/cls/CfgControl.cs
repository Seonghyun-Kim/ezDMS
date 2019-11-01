using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ezDeamon.cls
{
    public class CfgControl
    {
        [DllImport("kernel32.dll")]

        private static extern int GetPrivateProfileString(    // cfg Read 함수

            string section,

            string key,

            string def,

            StringBuilder retVal,

            int size,

            string filePath);

        [DllImport("kernel32.dll")]

        private static extern long WritePrivateProfileString(  // cfg Write 함수

            string section,

            string key,

            string val,

            string filePath);

        // cfg파일에서 읽어 오기

        public static String IniReadValue(string Section, string Key, string iniPath)
        {

            StringBuilder temp = new StringBuilder(1000);

            int i = GetPrivateProfileString(Section, Key, "", temp, 1000, iniPath);

            return temp.ToString();

        }

        // cfg파일에 쓰기

        public static void IniWriteValue(string Section, string Key, string Value, string iniPath)
        {

            WritePrivateProfileString(Section, Key, Value, iniPath);

        }
    }
}
