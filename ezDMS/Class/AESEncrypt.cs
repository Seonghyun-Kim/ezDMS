using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace IS_PODS.Class
{
    public class AESEncrypt
    {//비번 암호화
        public static string AESEncrypt256(string _text, string _password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] PlainText = Encoding.Unicode.GetBytes(_text);
            byte[] Salt = Encoding.ASCII.GetBytes(_password.Length.ToString());
             
            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_password, Salt);//암호화 된 키
            
            ICryptoTransform Encrytor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16)); //암호화되면 길이, 암호화 시킬 것?
            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, Encrytor, CryptoStreamMode.Write);

            cryptoStream.Write(PlainText, 0, PlainText.Length);
            cryptoStream.FlushFinalBlock();

            byte[] CipherBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            string base64String = Convert.ToBase64String(CipherBytes);

            return base64String.Replace("/", "_");
        }

        public static string AESDecrypte256(string _text, string _password)
        {
            RijndaelManaged RijndaelCipher = new RijndaelManaged();

            byte[] EncryptedData = Convert.FromBase64String(_text.Replace("_", "/"));
            byte[] Salt = Encoding.ASCII.GetBytes(_password.Length.ToString());

            PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(_password, Salt);

            ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
            MemoryStream memoryStream = new MemoryStream(EncryptedData);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

            byte[] PlainText = new byte[EncryptedData.Length];
            int DecrypedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

            memoryStream.Close();
            cryptoStream.Close();

            return Encoding.Unicode.GetString(PlainText, 0, DecrypedCount);
        }
    }
}