using ezDMS.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Dist
{
    public class DistTempFileModel : IAction
    {
        public int? temp_file_idx { get; set; }
        public int? dist_idx { get; set; } 
        public eModule action_module { get { return eModule.Dist; } }
        public int? target_idx { get { return temp_file_idx; } }
        public int? action_module_idx { get { return dist_idx; } }

        public DateTime file_upload_dt { get; set; }
        public string file_upload_date
        {
            get
            {
                return file_upload_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                file_upload_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }

        public string file_upload_datetime
        {
            get
            {
                return file_upload_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                file_upload_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public string file_org_nm { get; set; }
        public string file_conv_nm { get; set; }

        public string dms_file_type
        {
            get
            {
                return "LOCAL";
            }
        }
    }
}