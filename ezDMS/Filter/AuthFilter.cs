using ezDMS.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ezDMS.Filter
{
    public class AuthFilter : AuthorizeAttribute
    {
        public new eRole limitRole;
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["USER_IDX"] == null)
            {
                filterContext.Result = new RedirectResult("/Login");
                return;
            }

            eRole Role = (eRole)filterContext.HttpContext.Session["USER_ROLE"];

            if (limitRole > Role)
            {
                filterContext.Result = new RedirectResult("/Auth/NoAuth");
                return;
            }


        }
    }
}