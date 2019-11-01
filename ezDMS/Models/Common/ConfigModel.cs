using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static IS_PODS.Define.LogDefine;

namespace IS_PODS.Models.Common
{
    public class ConfigModel
    {
        public string comm_section { get; set; }
        public string comm_code { get; set; } 
        public string comm_value { get; set; }
    }
}