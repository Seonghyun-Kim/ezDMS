using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IS_PODS.Define.LogDefine;

namespace IS_PODS.Models.Auth
{
    public class VendModel : IAction
    {
        public int? vend_idx { get; set; }

        public string vend_nm { get; set; }
        public string vend_des { get; set; }

        public DateTime create_dt { get; set; }
        public string create_date
        {
            get
            {
                return create_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }

        public string create_datetime
        {
            get
            {
                return create_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public int? create_us { get; set; }
        public string create_us_nm { get; set; }

        public string use_fl { get; set; }

        public int? target_idx { get { return vend_idx; } }

        public eModule action_module { get { return eModule.Admin_Vend; } }

        public int? action_module_idx { get { return null; } }
    }
}