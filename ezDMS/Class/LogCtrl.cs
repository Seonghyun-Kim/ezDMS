using SmartDSP.Models.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SmartDSP.Define.LogDefine;

namespace SmartDSP.Class
{
    public class LogCtrl
    {
        public static void SetLog(IAction action, eActionType actionType, HttpContextBase context, string Description = "")
        {    
            ActionHistoryModel history = new ActionHistoryModel();

            history.action_module = action.action_module;
            history.action_module_idx = action.action_module_idx;
            history.action_type = actionType;
            history.action_target_idx = action.target_idx;
            history.action_ip = CommonUtil.GetRemoteIP(context.Request);
            history.action_us = Convert.ToInt32(context.Session["USER_IDX"]);
            history.session_id = context.Session.SessionID;
            history.description = Description;
            history.action_url = context.Request.FilePath; 
            history.Save();
        }

        public static void SetLog(IAction action, eActionType actionType, string Description = "")
        {
            ActionHistoryModel history = new ActionHistoryModel();

            history.action_module = action.action_module;
            history.action_module_idx = action.action_module_idx;
            history.action_type = actionType;
            history.action_target_idx = action.target_idx;
            history.action_ip = "192.0.0.1";
            history.action_us = -1;
            history.session_id = null;
            history.description = Description;
            history.action_url = "ezDeamon";
            history.Save();
        }
    }
}