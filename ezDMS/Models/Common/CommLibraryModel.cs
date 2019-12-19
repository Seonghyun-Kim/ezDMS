using SmartDSP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SmartDSP.Define.LogDefine;

namespace SmartDSP.Models.Common
{
    public class CommLibraryModel : IAction
    {
        public int? idx { get; set; }
        public int? parent_idx { get; set; } 
        public string parent_nm { get; set; }

        public string parent_code { get; set; }

        public string comm_code { get; set; }
        public string kor_nm { get; set; }
        public string eng_nm { get; set; }
        public string chn_nm { get; set; }
        public string etc1_nm { get; set; }
        public string etc2_nm { get; set; }
        public string code_value
        {
            get
            {
                //return kor_nm;
                return CommonUtil.GetLngNM(kor_nm, eng_nm, chn_nm, etc1_nm, etc2_nm);
            }
        }

        public string code_des { get; set; }

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

        public int? ord { get; set; }

        public string value { get { return idx.ToString(); }  }
        public string text { get { return code_value; } }

        public int? target_idx { get { return idx; } }

        public eModule action_module { get { return eModule.Admin_Common; } }

        public int? action_module_idx { get { return parent_idx; } }
    }
}