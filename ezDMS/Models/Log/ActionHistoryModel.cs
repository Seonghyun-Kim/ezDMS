using IBatisNet.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ezDMS.Class;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Log
{
    public class ActionHistoryModel
    {
        public int? action_idx { get; set; }
        public eModule action_module { get; set; }
        public string action_module_kor_nm { get; set; }
        public string action_module_eng_nm { get; set; }
        public string action_module_chn_nm { get; set; }
        public string action_module_etc1_nm { get; set; }
        public string action_module_etc2_nm { get; set; }

        public string action_module_nm 
        { 
            get
            {
                return CommonUtil.GetLngNM(action_module_kor_nm, action_module_eng_nm, action_module_chn_nm, action_module_etc1_nm, action_module_etc2_nm);
            }
        } 
        public int? action_module_idx { get; set; }
        public eActionType action_type { get; set; }
        public string action_type_kor_nm { get; set; }
        public string action_type_eng_nm { get; set; }
        public string action_type_chn_nm { get; set; }
        public string action_type_etc1_nm { get; set; }
        public string action_type_etc2_nm { get; set; }

        public string action_type_nm 
        {
            get 
            {
                return CommonUtil.GetLngNM(action_type_kor_nm, action_type_eng_nm, action_type_chn_nm, action_type_etc1_nm, action_type_etc2_nm);
            }
        }

        public int? action_target_idx { get; set; }
        public string action_ip { get; set; }
        public DateTime action_dt { get; set; }

        public string action_date
        {
            get
            {
                if (action_dt.Year.ToString() == "1") { return null; }
                return action_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                if (value == null) return;
                action_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public string action_datetime
        {
            get
            {
                if (action_dt.Year.ToString() == "1") { return null; }
                return action_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                if (value == null) return;
                action_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public int? action_us { get; set; }
        public string action_us_nm 
        { get;
            //{
            //    //return CommonUtil.GetLngNM(kor_nm, eng_nm, chn_nm, etc1_nm, etc2_nm);
            //}
            set;
        }
        public string action_url { get; set; }
        public string session_id { get; set; }
        public string description { get; set; }

        // 조회용
        public int? us_role { get; set; }
        public string us_role_kor_nm { get; set; }
        public string us_role_eng_nm { get; set; }
        public string us_role_chn_nm { get; set; }
        public string us_role_etc1_nm { get; set; }
        public string us_role_etc2_nm { get; set; }


        public string us_role_nm 
        {
            get 
            {
                return CommonUtil.GetLngNM(us_role_kor_nm, us_role_eng_nm, us_role_chn_nm, us_role_etc1_nm, us_role_etc2_nm);
            }
        
        }
        public int? us_group { get; set; }
        public string us_group_nm { get; set; }

        // SEARCH 용
        public string actionSDate { get; set; }
        public string actionEDate { get; set; }

        public int? search_action_type { get; set; }

        public int? search_action_module { get; set; }

        public void Save()
        {
            if(action_target_idx != null && action_module > 0 && action_type > 0)
            {
                Mapper.Instance().Insert("Log.InsActionHis", this);
            }
        }
        public string isFileHistory { get; set; }
    }
}