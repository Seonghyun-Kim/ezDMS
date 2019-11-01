using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IS_PODS.Models.Auth
{
    public class AuthModel
    {
        public string target_type { get; set; }
        public string target_type_nm { get; set; }
        public int? target_idx { get; set; }
        public int? menu_idx { get; set; }
        public string auth_create { get; set; }
        public string auth_delete { get; set; }
        public string auth_view { get; set; }

    }
}