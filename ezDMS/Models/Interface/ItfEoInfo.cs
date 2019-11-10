using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Interface
{
    public class ItfEoInfo : IAction
    {
        public eModule action_module { get { return eModule.Interface; } }
        public int? eo_idx { get; set; } // idx

        public int? action_module_idx { get { return eo_idx; } } 
        public int? target_idx { get { return eo_idx; } }
        public string eo_type { get; set; } // 타입 (ECO, ECA)
        public string eo_no { get; set; } // EO 번호
        public string eo_nm { get; set; } // EO 제목
        public string eo_des { get; set; }

        public DateTime eo_occr_dt { get; set; }
        public string eo_occr_date
        {
            get
            {
                if (eo_occr_dt.Year.ToString() == "1") { return ""; }
                return eo_occr_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                eo_occr_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string eo_occr_datetime
        {
            get
            {
                if (eo_occr_dt.Year.ToString() == "1") { return ""; }
                return eo_occr_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                eo_occr_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public DateTime eo_apply_dt { get; set; }
        public string eo_apply_date
        {
            get
            {
                if (eo_apply_dt.Year.ToString() == "1") { return ""; }
                return eo_apply_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                eo_apply_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string eo_apply_datetime
        {
            get
            {
                if (eo_apply_dt.Year.ToString() == "1") { return ""; }
                return eo_apply_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                eo_apply_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public DateTime itf_dt { get; set; }
        public string itf_date
        {
            get
            {
                if (itf_dt.Year.ToString() == "1") { return ""; }
                return itf_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                itf_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string itf_datetime
        {
            get
            {
                if (itf_dt.Year.ToString() == "1") { return ""; }
                return itf_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                itf_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public string itf_us_id { get; set; }
        public string itf_us_nm { get; set; }

        public string eo_cause { get; set; }
        public string eo_attr1 { get; set; }
        public string eo_attr2 { get; set; }
        public string eo_attr3 { get; set; }
        public string eo_attr4 { get; set; }
        public string eo_attr5 { get; set; }

        public string eo_oid { get; set; }

        public string notuse_fl { get; set; }

        public int? json_idx { get; set; }

        #region -- 검색용
        public string eo_occr_sdt { get; set; }
        public string eo_occr_edt { get; set; }
        public string eo_itf_sdt { get; set; }
        public string eo_itf_edt { get; set; }
        #endregion
    }
}