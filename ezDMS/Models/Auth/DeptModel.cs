using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Auth
{
    public class DeptModel : IAction
    {
        public int? dept_idx { get; set; }

        public int? high_dept_idx { get; set; }

        public string dept_nm { get; set; }

        public DateTime create_dt { get; set; }

        public int create_us {get; set;}

        public string create_us_nm { get; set; }

        public string use_fl { get; set; }

        public string dept_icon
        {
            get
            {
                if (high_dept_idx == null) { return "/Content/Images/Icon/iconmonstr-building-20-240.png"; }
                else { return "/Content/Images/Icon/iconmonstr-menu-1-240.png"; }
            }
        }

        public int? target_idx { get { return dept_idx; } }

        public eModule action_module { get { return eModule.Admin_Dept; } }

        public int? action_module_idx { get { return null; } }
    }
}