using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IS_PODS.Models.Common
{
    public class ResultJsonModel
    {
        public bool isError { get; set; }
        public string resultMessage { get; set; } 
        public string resultDescription { get; set; }

        public string resultStackTrace { get; set; }
    }
}