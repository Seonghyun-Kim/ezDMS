using ezDMS.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ezDMS.Models.Common
{
    public class MenuModel
    {
        public int menu_idx { get; set; }
        public int? group_idx { get; set; }

        public string menu_kor_nm { get; set; } 
        public string menu_eng_nm { get; set; }
        public string menu_chn_nm { get; set; }
        public string menu_etc1_nm { get; set; }
        public string menu_etc2_nm { get; set; }

        public string menu_nm
        {
            get
            {
                return CommonUtil.GetLngNM(menu_kor_nm, menu_eng_nm, menu_chn_nm, menu_etc1_nm, menu_etc2_nm);
            }
        }

        public string menu_url { get; set; }
        public string menu_des { get; set; }

        public string menu_icon { get; set; }

        public string use_fl { get; set; }



    }
}