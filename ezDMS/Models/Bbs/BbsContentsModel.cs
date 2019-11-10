using ezDMS.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Bbs
{
    public class BbsContentsModel : IAction

    {
        public int? target_idx { get { return bbs_idx; } } //댓글은 action_module_idx= bbs_idx
        public eModule action_module { get { return eModule.Board; } }//Board = 30,
        public int? action_module_idx { get { return bbs_idx; } }

        //게시판
        public int? bbs_idx { get; set; }
        public int? bbs_category { get; set; }
        public string bbs_title { get; set; }
        public string bbs_content { get; set; }
        //댓글, 파일의 조회를 따질때  target_idx 대상이 각각 reply, file's idx로  
        public int? bbs_read_cnt { get; set; }//조회수 
       
        //작성자 
        public int? create_us { get; set; }
        public string create_us_nm { get; set; }
        //작성일 + 최종수정날짜시간(가장최신)
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
                return create_dt.ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                create_dt = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }
       
        //댓글
        public int? bbs_reply_idx { get; set; }
        public int? reply_cnt { get; set; } //해당 게시글에 달린 리플의 총 갯수 
   
        //파일
        public int? bbs_file_idx { get; set; }
       
    }
}