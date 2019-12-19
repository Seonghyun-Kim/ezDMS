using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SmartDSP.Define.LogDefine;

namespace SmartDSP.Models.Interface
{
    public class ItfPartMaster : ItfEoInfo, IAction
    {
        public new eModule action_module { get { return eModule.part; } }
        public int? part_idx { get; set; }
        public new int? action_module_idx { get { return eo_idx; } }
        public new int? target_idx { get { return part_idx; } }
        public string part_no { get; set; }
        public string part_nm { get; set; } 
        public string part_unit { get; set; }
        public string part_weight { get; set; }
        public string part_oid { get; set; }

        public string part_rev_no { get; set; }
        public string part_des { get; set; }
        public string part_attr1 { get; set; }
        public string part_attr2 { get; set; }
        public string part_attr3 { get; set; }
        public string part_attr4 { get; set; }
        public string part_attr5 { get; set; }

        public int part_file_cnt { get; set; }

        public string is_latest { get; set; }
    }
}