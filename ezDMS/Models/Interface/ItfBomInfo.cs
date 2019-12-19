using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartDSP.Models.Interface
{
    public class ItfBomInfo : ItfPartMaster
    {
        public int? parent_part_idx { get; set; }
        public int? part_ord { get; set; }
        public float? qty { get; set; }
        public string find_no { get; set; }
         
        public string p_path { get; set; }

        public int? depth { get; set; }
        public string path { get; set; }


        // 수신자 검색용
        public int? dist_idx { get; set; }
        public int? recv_us { get; set; }
    }
}