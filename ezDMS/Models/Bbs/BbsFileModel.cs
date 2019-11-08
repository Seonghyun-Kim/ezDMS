using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Bbs
{
    public class BbsFileModel : IAction
    {//view에서는 txt만 .text.click 이벤트
        public int? target_idx { get { return bbs_file_idx; } } //댓글은 action_module_idx= bbs_idx

        public eModule action_module { get { return eModule.Board; } }//Board = 30,

        public int? action_module_idx { get { return bbs_idx; } }

        public int? bbs_file_idx { get; set; }

        public int? bbs_idx { get; set; }

        public string file_org_nm { get; set; }

        public string file_conv_nm { get; set; }

    }
}