using SmartDSP.Models.Auth;
using SmartDSP.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace SmartDSP.Class
{
    public class CommonUtil
    {
        #region -- IP 조회 영역
        public static string GetRemoteIP(HttpRequestBase Request)
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
             
            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return Request.ServerVariables["REMOTE_ADDR"];
        }

        public static string GetRemoteIP(HttpRequest Request)
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }

            return Request.ServerVariables["REMOTE_ADDR"];
        }

        #endregion

        #region -- 메뉴 권한
        public static List<MenuModel> CheckMenusAuth(Hashtable authList, IList<MenuModel> menuList)
        {
            List<MenuModel> retAuthList = new List<MenuModel>();

            var userAuth = (IList<AuthModel>)authList["USER"];
            var deptAuth = (IList<AuthModel>)authList["DEPT"];
            var roleAuth = (IList<AuthModel>)authList["ROLE"];

            foreach(var menu in menuList)
            {
                var Auth = (from user in userAuth
                            where user.menu_idx == menu.menu_idx
                            select user).Union
                            (from dept in deptAuth
                            where dept.menu_idx == menu.menu_idx
                             select dept).Union
                            (from role in roleAuth
                            where role.menu_idx == menu.menu_idx
                             select role);

                var isCreate = false;
                var isDelete = false;
                var isView = false;

                foreach (AuthModel o in Auth)
                {
                    if (o.auth_create == "Y")
                    {
                        isCreate = true;
                    }

                    if (o.auth_delete == "Y")
                    {
                        isDelete = true;
                    }

                    if (o.auth_view == "Y")
                    {
                        isView = true;
                    }

                    if (isCreate && isDelete && isView) { break; }
                }

                if(isView)
                {
                    retAuthList.Add(menu);
                }
            }

            return retAuthList;
        }

        public static AuthModel CheckMenuAuth(Hashtable authList, int? target_menu_idx)
        {
            AuthModel retAuth = new AuthModel();

            var userAuth = (IList<AuthModel>)authList["USER"];
            var deptAuth = (IList<AuthModel>)authList["DEPT"];
            var roleAuth = (IList<AuthModel>)authList["ROLE"];

            var Auth = (from user in userAuth
                        where user.menu_idx == target_menu_idx
                        select user).Union
                       (from dept in deptAuth
                        where dept.menu_idx == target_menu_idx
                        select dept).Union
                       (from role in roleAuth
                        where role.menu_idx == target_menu_idx
                        select role);

            var isCreate = false;
            var isDelete = false;
            var isView = false;

            foreach(AuthModel o in Auth)
            {
                if(o.auth_create == "Y")
                {
                    isCreate = true;
                }

                if (o.auth_delete == "Y")
                {
                    isDelete = true;
                }

                if (o.auth_view == "Y")
                {
                    isView = true;
                }

                if(isCreate && isDelete && isView) { break; }
            }

            retAuth.menu_idx = target_menu_idx;
            retAuth.auth_create = isCreate ? "Y" : "N";
            retAuth.auth_delete = isDelete ? "Y" : "N";
            retAuth.auth_view = isView ? "Y" : "N";

            return retAuth;
        }

        #endregion

        #region -- FILE 처리 
        public static string FileSave(string dirPath, HttpPostedFileBase file)
        {
            return FileSave(dirPath, file, "");
        }

        public static string FileSave(string dirPath, HttpPostedFileBase file, string fileConvName)
        {         
            string sFileName = fileConvName == "" ? file.FileName : fileConvName;

            try
            {
                string fileFullDirectory = FolderCreate(dirPath);

                /*중복되지 않은 파일명 생성*/
                FileInfo fi = new FileInfo(Path.Combine(fileFullDirectory, sFileName));

                if (fi.Exists)
                {
                    string sFileNewName = string.Empty;
                    string sFileExt = Path.GetExtension(sFileName);
                    /*중복되지 않은 파일명 생성*/
                    int cnt = 1;

                    string pureName = Path.GetFileNameWithoutExtension(fi.FullName);

                    while (fi.Exists)
                    {
                        sFileNewName = pureName + " (" + (cnt++) + ")" + sFileExt;
                        fi = new FileInfo(Path.Combine(fileFullDirectory, sFileNewName));
                    }

                    /*파일 쓰기*/
                    FileStream fs = File.Create(fi.FullName);
                    file.InputStream.CopyTo(fs);

                    fs.Close();
                    file.InputStream.Close();

                    return sFileNewName;
                }
                else
                {
                    /*파일 쓰기*/
                    FileStream fs = File.Create(fi.FullName);
                    file.InputStream.CopyTo(fs);

                    fs.Close();
                    file.InputStream.Close();

                    return sFileName;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string FolderCreate(string createPath)
        {
            if (!Directory.Exists(createPath))
                Directory.CreateDirectory(createPath);

            return createPath;
        }

        public static string FolderDelete(string delPath)
        {
            if (Directory.Exists(delPath))
                Directory.Delete(delPath, true);

            return delPath;
        }

        public static bool IsFile(string dirPath, string sFileName)
        {
            return File.Exists(Path.Combine(dirPath, sFileName));
        }

        public static Stream FileStream(string fileFullPath)
        {
            var fs = new FileStream(fileFullPath, FileMode.Open);
            return fs;
        }

        public static Stream FileStream(string dirPath, string fileName)
        {
            string fileFullDirectory = Path.Combine(dirPath, fileName);
            var fs = new FileStream(fileFullDirectory, FileMode.Open);
            return fs;
        }

        public static string FileCopyToTemp(string filePath, string fileName)
        {
            string fileTempDirectory = System.Configuration.ConfigurationManager.AppSettings["ViewTempFilePath"].ToString();

            /*템프 폴더에 폴더생성*/
            if (!Directory.Exists(fileTempDirectory))
                Directory.CreateDirectory(fileTempDirectory);

            string fileFullDirectory = Path.Combine(filePath, fileName);
            var fs = new FileStream(fileFullDirectory, FileMode.Open);

            string sFileExt = Path.GetExtension(fileName);

            string sTempFileName = DateTime.Now.Ticks.ToString() + sFileExt;

            var targetFs = new FileStream(Path.Combine(fileTempDirectory, sTempFileName), FileMode.Create);
            fs.CopyTo(targetFs);
            fs.Close();
            targetFs.Close();

            return Path.Combine(fileTempDirectory, sTempFileName);
        }

        #endregion

        #region -- 기타
        public static string DefaultPassword()
        {
            string defaultPassword = System.Configuration.ConfigurationManager.AppSettings["DefaultPassword"];
            string isKey = System.Configuration.ConfigurationManager.AppSettings["LoginKey"];

            return AESEncrypt.AESDecrypte256(defaultPassword, isKey);
        }

        public static Color CodeToColor(string sCode)
        {
            sCode = sCode.TrimStart('#');

            Color col; // from System.Drawing or System.Windows.Media
            if (sCode.Length == 6)
                col = Color.FromArgb(255, // hardcoded opaque
                            int.Parse(sCode.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(sCode.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(sCode.Substring(4, 2), NumberStyles.HexNumber));
            else // assuming length of 8
                col = Color.FromArgb(
                            int.Parse(sCode.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(sCode.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(sCode.Substring(4, 2), NumberStyles.HexNumber),
                            int.Parse(sCode.Substring(6, 2), NumberStyles.HexNumber));

            return col;
        }

        public static string GetLngNM(string kor_nm, string eng_nm, string chn_nm, string etc_1nm, string etc_2nm)
        {
            switch (System.Threading.Thread.CurrentThread.CurrentCulture.Name.ToLower())
            {
                case "ko-kr":
                    return kor_nm;
                case "en-us":
                    return eng_nm;
                case "zh-cn":
                    return chn_nm;
                case "etc1":
                    return etc_1nm;
                case "etc2":
                    return etc_2nm;
                default:
                    return "";
            }
        }

        #endregion

    }
}