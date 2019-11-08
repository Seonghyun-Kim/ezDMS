using IBatisNet.DataMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ezDMS.Models.Log
{
    public class LoginHistoryModel
    {
        public int? login_idx { get; set; }
         
        public string try_id { get; set; }
        public string try_pw { get; set; }

        public string try_ip { get; set; }

        public DateTime try_dt { get; set; }

        public string try_date
        {
            get
            {
                if (try_dt.Year.ToString() == "1") { return null; }
                return try_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                if (value == null) return;
                try_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string try_datetime
        {
            get
            {
                if (try_dt.Year.ToString() == "1") { return null; }
                return try_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                if (value == null) return;
                try_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }


        public string login_st { get; set; }

        public string login_st_nm { get; set; }

        public int? login_us { get; set; }
        public string login_us_nm { get; set; }
        public string session_id { get; set; }

        // 조회용
        public int? us_role { get; set; }
        public string us_role_nm { get; set; }
        public int? us_group { get; set; }
        public string us_group_nm { get; set; }

        // SEARCH 용
        public string tryLoginSDate { get; set; }
        public string tryLoginEDate { get; set; }




        public void SetLoginHistory()
        {
            Mapper.Instance().Insert("Log.InsLoginHis", this);
        }
    }
}