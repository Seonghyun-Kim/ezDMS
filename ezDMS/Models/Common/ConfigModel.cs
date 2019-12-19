using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static SmartDSP.Define.LogDefine;

namespace SmartDSP.Models.Common
{
    public class ConfigModel
    {
        public string comm_section { get; set; }
        public string comm_code { get; set; } 
        public string comm_value { get; set; }
    }
}