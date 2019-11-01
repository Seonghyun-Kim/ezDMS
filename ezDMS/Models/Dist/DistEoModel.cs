using IS_PODS.Models.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IS_PODS.Define.LogDefine;

namespace IS_PODS.Models.Dist
{
    public class DistEoModel : ItfEoInfo, IAction
    {
        public new eModule action_module { get { return eModule.Dist; } }

        public new int? target_idx { get { return eo_idx; } } 
        public new int? action_module_idx { get { return dist_idx ; } }
        public  int? dist_idx { get; set; }
        public int? itf_eo_idx { get { return eo_idx; } set { eo_idx = value; } }

    }
}