using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static ezDMS.Define.LogDefine;

namespace ezDMS.Models.Common
{
    public class ConfigModel
    {
        public string comm_section { get; set; }
        public string comm_code { get; set; } 
        public string comm_value { get; set; }
    }
}