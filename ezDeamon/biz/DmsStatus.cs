using ezDMS.Models.Dist;
using ezDMS.Models.Log;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;
using static ezDMS.Define.LogDefine;

namespace ezDeamon.biz
{
    public class DmsStatus
    {
        public void StatusManage()
        {
            DmsBiz biz = new DmsBiz();

            NpgsqlTransaction tran = biz.dbCon.DBBeginTrans();
            try
            {
                var distList = biz.GetDistList();

                if (distList == null || distList.Count == 0) { return; }

                foreach (DistMasterModel dist in distList)
                {
                    DateTime chkDt;
                    string sChkDate = string.Empty;
                    string sStatus = dist.dist_st;

                    string sNextStatus = string.Empty;

                    sChkDate = dist.finish_date;

                    chkDt = DateTime.Parse(sChkDate);
                    TimeSpan TS = chkDt - DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

                    int diffDay = TS.Days;

                    if (diffDay >= 0)
                    {
                        continue;
                    }

                    biz.UpdateDistStatus(tran, new DistMasterModel() { dist_idx = dist.dist_idx, dist_st = "DF" });

                    ActionHistoryModel log = new ActionHistoryModel();

                    log.action_module = dist.action_module;
                    log.action_module_idx = dist.action_module_idx;
                    log.action_type = eActionType.Expire;
                    log.action_target_idx = dist.target_idx;

                    biz.UpdateLog(log);

                    
                }

                biz.dbCon.DBCommit(tran);
            }
            catch(Exception ex)
            {
                biz.dbCon.DBRollBack(tran);
                throw ex;
            }    
            finally
            {
                biz.dbCon.DBDisconnect();
            }
           

            
        }
    }
}
