﻿using SmartDSP.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SmartDSP.Define.LogDefine;

namespace SmartDSP.Models.Interface
{
    public class ItfFileInfo : ItfPartMaster, IAction
    {
        public int? temp_idx { get; set; }
        
        public int? file_idx { get; set; }
        public new int? action_module_idx { get { return part_idx; } }
        public new int? target_idx { get { return file_idx; } }
        public string file_rev_no { get; set; } 
        public string plm_file_nm { get; set; }
        public string file_oid { get; set; }
        public int? file_ord { get; set; }

        // PDM, PLM 용 -도면, 문서 등등 
        public string file_type { get; set; }
        // PDM, PLM 용 - 도면 내의 어떤 종류인지 (승인도 등등) 
        public string file_categorize { get; set; }

        public long? file_size { get; set; }

        public string file_format { get; set; }

        public string file_org_nm { get; set; }
        public string file_conv_nm { get; set; }

        public string dms_file_type
        {
            get
            {
                return "PDM";
            }
        }

        public string file_extention
        {
            get
            {
                if (file_org_nm == null)
                {
                    return "";
                }
                int intPos = file_org_nm.LastIndexOf('.');

                return file_org_nm.Substring(intPos + 1);
            }
        }
    }
}