using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ezDeamon.cls
{
    public class FileControl
    {
        #region -- 폴더 관련
        /// <summary>
        /// 폴더의 유무를 파악함.
        /// </summary>
        /// <param name="FolderPath">폴더 경로</param>
        /// <returns>폴더 존재 유무</returns>
        public bool IsExistFolder(string FolderPath)
        {
            bool IsExist = false;

            try
            {
                IsExist = Directory.Exists(FolderPath);

                return IsExist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 폴더의 유무를 파악하여 폴더가 존재하지 않을 경우 폴더를 생성함.
        /// </summary>
        /// <param name="FolderPath">폴더 경로</param>
        /// <returns>생성유무</returns>
        public bool CreateFolder(string FolderPath)
        {
            try
            {
                if (!IsExistFolder(FolderPath)) Directory.CreateDirectory(FolderPath);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 폴더의 유무를 파악하여 폴더가 존재할 경우 폴더를 삭제함.
        /// </summary>
        /// <param name="FolderPath">폴더 경로</param>
        /// <returns>삭제유무</returns>
        public bool DeleteFolder(string FolderPath)
        {
            try
            {
                if (IsExistFolder(FolderPath)) Directory.Delete(FolderPath);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 기준 폴더안에 존재하는 폴더들의 생성일과 현재 날짜의 차이가 gapDay만큰 차이가 나면 해당 폴더 삭제
        /// gapDay가 0 일 경우 기준 폴더 삭제
        /// </summary>
        /// <param name="TargetFolderPath">기준 폴더</param>
        /// <param name="gapDay">삭제 기준 일</param>
        /// <returns></returns>
        public bool DeleteDayFolder(string TargetFolderPath, int gapDay)
        {
            try
            {
                if (gapDay == 0) { if (IsExistFolder(TargetFolderPath)) { Directory.Delete(TargetFolderPath); } }
                else
                {
                    if (IsExistFolder(TargetFolderPath))
                    {
                        System.IO.DirectoryInfo di = new DirectoryInfo(TargetFolderPath);

                        foreach (DirectoryInfo Folder in di.GetDirectories())
                        {
                            TimeSpan ts = DateTime.Now - Folder.CreationTime;
                            if (ts.Days > gapDay)
                            {
                                Directory.Delete(Folder.FullName, true);
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region -- 파일 관련

        private bool IsExistFile(string FilePath)
        {
            bool IsExist = false;

            try
            {
                IsExist = File.Exists(FilePath);

                return IsExist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool WriteFile(string FilePath, string FileName, string FileText)
        {
            try
            {
                return WriteFile(FilePath, FileName, FileText, Encoding.UTF8, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool WriteFile(string FilePath, string FileName, string FileText, bool IsWriteAppend)
        {
            try
            {
                return WriteFile(FilePath, FileName, FileText, Encoding.UTF8, IsWriteAppend);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool WriteFile(string FilePath, string FileName, string FileText, Encoding Encode, bool IsWriteAppend)
        {
            try
            {

                string FileFullPath = FilePath + "\\" + FileName;

                if (!IsExistFile(FilePath)) Directory.CreateDirectory(FilePath);

                lock (FileFullPath)
                {
                    //File.WriteAllText(FileFullPath, FileText, Encode);

                    using (StreamWriter pFileWriter = new StreamWriter(FileFullPath, IsWriteAppend, Encode))
                    {
                        pFileWriter.WriteLine(FileText);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteFile(string FilePath, string FileName)
        {
            try
            {

                string FileFullPath = Path.Combine(FilePath , FileName);

                if (!IsExistFile(FilePath))
                {
                    throw new Exception("파일이 없습니다.");
                }

                FileInfo file = new FileInfo(FileFullPath);

                file.Delete();

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    

        bool m_bFileReading = false;

        #region // 해당 파일의 정보를 읽어서 ARRAY LIST로 저장한다 : FileIORead
        public int FileIORead(string p_strFileName, ref ArrayList p_alFile)
        {
            int nCount = 0;

            if (!File.Exists(p_strFileName)) return nCount;

            m_bFileReading = true;

            FileStream fileStream = File.Open(p_strFileName, FileMode.Open, FileAccess.Read);
            StreamReader fileReader = new StreamReader(fileStream, System.Text.Encoding.UTF8);

            string strReadFileData = null;
            strReadFileData = fileReader.ReadLine();

            while (strReadFileData != null)
            {
                nCount++;
                p_alFile.Add(strReadFileData);

                strReadFileData = null;
                strReadFileData = fileReader.ReadLine();
            }

            fileStream.Close();

            m_bFileReading = false;
            return nCount;
        }
        #endregion
        //-------------------------------------------------------

        // 해당 파일로 정보를 저장한다
        public bool FileIOWrite(string p_strFileName, ArrayList p_alFile)
        {
            bool bWriteOK = true;

            // 이전 파일 백업.....처리
            if (File.Exists(p_strFileName))
            {
                string strBackupFile = p_strFileName + ".bak";
            }

            m_bFileReading = true;

            FileStream fileStream = File.Open(p_strFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
            StreamWriter fileWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);

            try
            {
                for (int n = 0; n < p_alFile.Count; n++)
                {
                    string strFileOrder = "";

                    strFileOrder = string.Format("{0}\r\n", p_alFile[n].ToString());

                    fileWriter.Write(strFileOrder);
                    fileWriter.Flush();

                }
            }
            catch (Exception e)
            {
                // 오류 처리
                bWriteOK = false;
                ///				MessageBox.Show(e.Message);
            }

            fileStream.Close();

            return bWriteOK;
        }


        //-------------------------------------------------------
        #region // 정보를 해당 파일로 저장한다 : AppendFileIOWrite
        public bool AppendFileIOWrite(string strFileNm, string strPath, string p_strData)
        {
            bool bWriteOK = true;

            try
            {
                lock (strFileNm)
                {
                    if (!File.Exists(strFileNm))
                    {
                        if (!Directory.Exists(strPath))
                        {
                            Directory.CreateDirectory(strPath);
                        }
                    }

                    StreamWriter fileWriter = File.AppendText(strFileNm);

                    string strFileOrder = "";

                    strFileOrder = string.Format("{0}\r\n", p_strData);

                    fileWriter.Write(strFileOrder);
                    fileWriter.Flush();

                    fileWriter.Close();
                }

            }
            catch (Exception e)
            {
                // 오류 처리
                bWriteOK = false;
            }



            return bWriteOK;
        }
        #endregion
        //-------------------------------------------------------


        // 현재 파일을 읽는 중인지 확인한다
        public bool GetFileReadStatus()
        {
            return m_bFileReading;
        }

    #endregion
    }
}
