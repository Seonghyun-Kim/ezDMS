using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using static IS_PODS.Define.LogDefine;
namespace IS_PODS.Models.Dist
{
    public class RecvFileModel : IAction
    {//마바티는 public 때려줘야 참조를 해욤
         
        public int? target_idx { get { return link_file_idx; } }
        public int? dist_idx { get; set; }
        public int? action_module_idx { get { return dist_idx; } }

        public eModule action_module { get { return eModule.Recv; } }

        public int? link_file_idx { get; set; }
        public string is_itf { get; set; }

        public string plm_file_nm { get; set; }
        public string file_org_nm { get; set; }

        public string file_conv_nm { get; set; }

        public string file_rev_no { get; set; }
    }
}