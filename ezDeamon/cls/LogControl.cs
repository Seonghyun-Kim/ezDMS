using System;
using System.Collections.Generic;
using System.Text;

namespace ezDeamon.cls
{
    class LogControl
    {
        FileControl fileCtrl = null;
        private static string LOG_PATH = string.Empty;

        #region -- 생성자, 소멸자
        public LogControl()
        {
            //LOG_PATH = System.Environment.CurrentDirectory + "\\LOG";

            LOG_PATH = AppDomain.CurrentDomain.BaseDirectory + "\\LOG";
            fileCtrl = new FileControl();
        }

        public LogControl(string pLOG_PATH)
        {
            LOG_PATH = pLOG_PATH;
            fileCtrl = new FileControl();
        }

        ~LogControl()
        {

        }
        #endregion

      
        public void WriteLog(string LogMessage)
        {
            WriteLog(LogStatus.INFO, LogMessage);
        }

        public void WriteLog(LogStatus status, string LogMessage)
        {
            string logMessage = string.Empty;
            try
            {
                string logPath = status == LogStatus.ERROR ? LOG_PATH + "\\ERROR" : LOG_PATH; 
                logMessage = string.Format("[{0}] {1} : {2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), status, LogMessage);

                fileCtrl.WriteFile(logPath, DateTime.Now.ToString("yyyyMMdd") + "_Log.txt", logMessage, true);

                #if DEBUG
                Console.WriteLine(logMessage);
                #endif
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DeleteLogFile()
        {

        }
    }
}
