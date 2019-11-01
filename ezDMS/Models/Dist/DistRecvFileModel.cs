using IS_PODS.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IS_PODS.Define.LogDefine;

namespace IS_PODS.Models.Dist
{
    public class DistRecvFileModel : IAction
    {
        public int? dist_file_idx { get; set; }

        public int? target_idx { get { return dist_file_idx; } }
        public int? dist_idx { get; set; }
        public int? action_module_idx { get { return dist_idx; } }
         
        public eModule action_module { get { return eModule.Dist;  } }
        public int? recv_idx { get; set; }

        public int? recv_us { get; set; } // 검색용
        public string is_itf { get; set; }
        public string is_itf_nm
        {
            get
            {
                if (is_itf == null) { return ""; }
                return is_itf == "Y" ? "PDM" : "LOCAL";
            }
        }
        public int? link_file_idx { get; set; }


        public int? eo_idx { get; set; }
        public string part_no { get; set; }
        public string part_rev_no { get; set; }
        public string plm_file_nm { get; set; }
        public int? rev_no { get; set;  }
        public string file_org_nm { get; set; }
        public string file_conv_nm { get; set; }

        public int? file_rev_no { get; set; }

        public string use_fl { get; set; }

        public string file_extention
        {
            get
            {
                if(file_org_nm == null)
                {
                    return "";
                }
                int intPos = file_org_nm.LastIndexOf('.');

                return file_org_nm.Substring(intPos + 1);
            }
        }

        public DateTime first_down_dt { get; set; }
        public string first_down_date
        {
            get
            {
                if (first_down_dt.Year.ToString() == "1") { return ""; }
                return first_down_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                if (value == null) return;
                first_down_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string first_down_datetime
        {
            get
            {
                if (first_down_dt.Year.ToString() == "1") { return ""; }
                return first_down_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                if (value == null) return;
                first_down_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }
        // PDM, PLM 용 -도면, 문서 등등 
        public string file_type { get; set; }
        // PDM, PLM 용 - 도면 내의 어떤 종류인지 (승인도 등등) 
        public string file_categorize { get; set; }

        public long? file_size { get; set; }
    }
}