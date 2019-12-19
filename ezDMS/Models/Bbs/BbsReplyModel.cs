using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SmartDSP.Define.LogDefine;
namespace SmartDSP.Models.Bbs
{
    public class BbsReplyModel: IAction 
    {
        public int? target_idx { get { return bbs_reply_idx; } } //댓글은 action_module_idx= bbs_idx

        public eModule action_module { get { return eModule.Board; } }//Board = 30,

        public int? action_module_idx { get { return bbs_idx; } }
       // public int? parent_reply_idx { get; set; } 
        public int? bbs_reply_idx { get; set; }
        public int? parent_reply_idx { get; set; }
        public string bbs_reply_content { get; set; }

        public int? create_us { get; set; }
        public string create_us_nm { get; set; }


        public DateTime create_dt { get; set; }
        public string create_date
        {
            get
            {
                return create_dt.ToString("yyyy-MM-dd");
            }
            set
            {
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd", null);
            }
        }
        public string create_datetime
        {
            get
            {
                if (create_dt.Year.ToString() == "1") { return ""; }
                return create_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                if (value == null) return;
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }
        //게시물
        public int? bbs_idx { get; set; }
        public string use_fl { get; set; }

    }
}