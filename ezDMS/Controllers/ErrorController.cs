using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IS_PODS.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult ErrorView(string ExceptionMessage)
        {
            return View(ExceptionMessage);
        }
    }
}