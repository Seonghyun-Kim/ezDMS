using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ezDMS.Models.Dist
{
    public class BookmarkGroup
    {
        public int? grp_idx { get; set; }
        public string grp_nm { get; set; }
        public int? create_us { get; set; }
        public string create_us_nm { get; set; }
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
    }

    public class BookmarkUser : BookmarkGroup
    {
        public int? grp_us_idx { get; set; }
        public string grp_us_nm { get; set; }

        public string grp_us_group_nm { get; set; }

        // 검색용

        public int? us_role { get; set; }
    }


}