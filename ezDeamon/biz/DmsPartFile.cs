using ezDeamon.cls;
using ezDMS.Models.Interface;
using ezDMS.Models.Log;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static ezDMS.Define.LogDefine;

namespace ezDeamon.biz
{
    class DmsPartFile
    {
        DmsBiz biz = new DmsBiz();
        public void FileRemove()
        {
            NpgsqlTransaction tran = biz.dbCon.DBBeginTrans();
            try
            { 
                FileControl fileCtrl = new FileControl();

                string isFileDelete = biz.SysConfigValue("FILE", "IS_DEL");

                if(isFileDelete != "Y") { return; }

                string iniPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\ini\app.ini";
                string filePath = CfgControl.IniReadValue("PROCESS", "ITF_FILE_FOLDER", iniPath);

                int fileDelTermDay = Convert.ToInt32(biz.SysConfigValue("FILE", "DEL_TERM"));

                List<ItfFileInfo> fileList = biz.TermFileList(fileDelTermDay);

                if(fileList.Count == 0) { return; }
                
                foreach (ItfFileInfo file in fileList)
                {
                    string fileFullPath = Path.Combine(filePath, file.part_no);

                    fileCtrl.DeleteFile(fileFullPath, file.file_org_nm);

                    biz.UpdateFileStatus(tran, file.file_idx);

                    ActionHistoryModel log = new ActionHistoryModel();

                    log.action_module = file.action_module;
                    log.action_module_idx = null;
                    log.action_type = eActionType.FileDelete;
                    log.action_target_idx = file.target_idx;

                    biz.UpdateLog(log);
                }

                biz.dbCon.DBCommit(tran);
            }
            catch(Exception ex)
            {
                biz.dbCon.DBRollBack(tran);
                throw ex;
            }
        }

        public void EoDisable()
        {
            NpgsqlTransaction tran = biz.dbCon.DBBeginTrans();
            try
            {
                int eoTermDay = Convert.ToInt32(biz.SysConfigValue("DIST", "PART_DIST_TERM"));

                List<ItfEoInfo> eoList = biz.TermEOList(eoTermDay);

                if(eoList.Count == 0) { return; }

                foreach (ItfEoInfo eo in eoList)
                {
                    biz.UpdateEOStatus(tran, eo.eo_idx);
                }

                biz.dbCon.DBCommit(tran);
            }
            catch(Exception ex)
            {
                biz.dbCon.DBRollBack(tran);
                throw ex;
            }
        }
    }
}
