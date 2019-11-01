using IS_PODS.Models.Dist;
using IS_PODS.Models.Log;
using System;
using System.Collections.Generic;
using System.Text;
using static IS_PODS.Define.LogDefine;

namespace ezDeamon.biz
{
    public class DmsStatus
    {
        public void StatusManage()
        {
            DmsBiz biz = new DmsBiz();

            var distList = biz.GetDistList();

            if(distList == null || distList.Count == 0) { return; }

            foreach(DistMasterModel dist in distList)
            {
                DateTime chkDt;
                string sChkDate = string.Empty;
                string sStatus = dist.dist_st;

                string sNextStatus = string.Empty;

                sChkDate = dist.finish_date;

                chkDt = DateTime.Parse(sChkDate);
                TimeSpan TS = chkDt - DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

                int diffDay = TS.Days;

                if(diffDay >= 0)
                {
                    continue;
                }

                biz.UpdateDistStatus(new DistMasterModel() { dist_idx = dist.dist_idx, dist_st = "DF" });

                ActionHistoryModel log = new ActionHistoryModel();

                log.action_module = dist.action_module;
                log.action_module_idx = dist.action_module_idx;
                log.action_type = eActionType.Expire;
                log.action_target_idx = dist.target_idx;

                biz.UpdateLog(log);
            }
        }
    }
}
