using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IS_PODS.Models.Stats
{  
    public class DistRecvStats
    {
        public int? vend_idx { get; set; }
        public string vend_nm { get; set; }

        public int dist_file_cnt { get; set; }
        public int down_cnt { get; set; }
         
        // Search 
        public DateTime search_dt { get; set; }
    }

    public class DistCntStats
    {
        public string dist_date { get; set; }
        public int dist_cnt { get; set; }
        // Search 
        public DateTime search_dt { get; set; }
    }

    public class LoginDateStats
    {
        public string login_date { get; set; }
        public int login_cnt { get; set; }
        // Search 
        public string searchSdate { get; set; }
        public string searchEdate { get; set; }
    }

    public class LoginTopUserStats
    {
        public int? login_us { get; set; }
        public string login_us_nm { get; set; }
        public int login_cnt { get; set; }
        // Search 
        public string searchSdate { get; set; }
        public string searchEdate { get; set; }
    }
}