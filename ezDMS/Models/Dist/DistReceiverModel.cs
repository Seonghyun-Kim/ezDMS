using ezDMS.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Dist
{
    public class DistReceiverModel : UserModel, IAction
    {
        public new eModule action_module { get { return eModule.Dist; } } 
        public new int? action_module_idx { get { return dist_idx; } }
        public new int? target_idx { get { return recv_idx; } }
        public int? recv_idx { get; set; }
        public int? dist_idx { get; set; }
        public int? recv_us
        {
            get {
                return us_idx;
            } set {
                us_idx = value;
            }
        }
        public string recv_msg { get; set; }

        public int file_cnt { get; set; }
        public int down_cnt { get; set; }

        public string recv_dist_st { get; set; }  
        public string recv_dist_st_nm { get; set; }

        public string is_full_recv { get; set; }

        public DateTime recv_finish_dt { get; set; }  
        public string recv_finish_date
        {
            get
            {
                if (recv_finish_dt.Year.ToString() == "1") { return ""; }
                return recv_finish_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                if (value == null) return;
                recv_finish_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
       
    }
}