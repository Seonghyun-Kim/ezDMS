using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ezDMS.Class;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Dist
{
    public class DistMasterModel : IAction
    {
        public eModule action_module { get { return eModule.Dist; } } 
        public int? dist_idx { get; set; }
        public int? target_idx { get { return dist_idx; } }
        public int? action_module_idx { get { return dist_idx; } }
        public string dist_title { get; set; }
        public string dist_msg { get; set; }
        public DateTime create_dt { get; set; }
        public string create_date
        {
            get
            {
                if (create_dt.Year.ToString() == "1") { return null; }
                return create_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                if (value == null) return;
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string create_datetime
        {
            get
            {
                if (create_dt.Year.ToString() == "1") { return null; }
                return create_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                if (value == null) return;
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public DateTime edit_dt { get; set; }
        public string edit_date
        {
            get
            {
                if (edit_dt.Year.ToString() == "1") { return null; }
                return edit_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                if (value == null) return;
                edit_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string edit_datetime
        {
            get
            {
                if (edit_dt.Year.ToString() == "1") { return null; }
                return edit_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                if (value == null) return;
                edit_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        public DateTime dist_dt { get; set; }
        public string dist_date
        {
            get
            {
                if (dist_dt.Year.ToString() == "1") { return null; }
                return dist_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                if (value == null) return;
                dist_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string dist_datetime
        {
            get
            {
                if (dist_dt.Year.ToString() == "1") { return null; }
                return dist_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                if (value == null) return;
                dist_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }
        public int? create_us { get; set; }
        public string create_us_nm { get; set; }
        public DateTime finish_dt { get; set; }
        public string finish_date
        {
            get
            {
                if (finish_dt.Year.ToString() == "1") { return null; }
                return finish_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                if (value == null) return;
                finish_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
      
        public string dist_st { get; set; }
        public string dist_st_nm 
        {
            get 
            {
                return CommonUtil.GetLngNM(dist_st_kor_nm, dist_st_eng_nm, dist_st_chn_nm, dist_st_etc1_nm, dist_st_etc2_nm);
            }
        }

        public string dist_st_kor_nm { get; set; }
        public string dist_st_eng_nm { get; set; }
        public string dist_st_chn_nm { get; set; }
        public string dist_st_etc1_nm { get; set; }
        public string dist_st_etc2_nm { get; set; }
        
        public string eo_fl { get; set; }


        // LIST 조회 용
        public int? eo_idx { get; set; }
        public string eo_nm { get; set; }
        public string eo_no { get; set; }
        public string vender_list { get; set; }
        public int file_cnt { get; set; }
        public int down_cnt { get; set; }

        public string file_down_status
        {
            get
            {
                return down_cnt.ToString() + " / " + file_cnt.ToString();
            }
        }

        public int recv_cnt { get; set; } // 수신자 총 수
        public int  recv_full_down_cnt { get; set; } // 수신자 중 모든 파일을 다운로드 받은 수신자 수



        // 검색 용
        public string create_sdt { get; set; }
        public string create_edt { get; set; }
        public string dist_sdt { get; set; }
        public string dist_edt { get; set; }
        public string finish_sdt { get; set; }
        public string finish_edt { get; set; }

        public string isDist { get; set; }

        public string recv_us_nm { get; set; }

        public int? recv_us { get; set; }
        public int? vender_idx { get; set; }

        public string part_no { get; set; }
        public string part_rev_no { get; set; }


    }
}