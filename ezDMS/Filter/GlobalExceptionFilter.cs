using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartDSP.Models.Common;
using System.Web.Routing;

namespace SmartDSP.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILog _logger;

        public GlobalExceptionFilter(ILog logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Try Catch 가 안걸린 함수들에 대한 Exception 처리
        /// </summary>
        /// <param name="filterContext"></param>
        public virtual void OnException(ExceptionContext filterContext)
        {
            if (filterContext.HttpContext.Session["USER_IDX"] == null)
            {
                //filterContext.Result = new ViewResult
                //{
                //    ViewName = "~/Views/Login/Index.cshtml"
                //};
                filterContext.Result = new RedirectResult("/Login");
                filterContext.ExceptionHandled = true;
                return;
            }

            _logger.Error(filterContext.Exception);
    

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
            {
                controller = "Error",
                action = "ErrorView",
                param = filterContext.Exception.Message
            }));
            return;
        }

        public interface IExceptionFilter
        {
            void OnException(ExceptionContext filterContext);
        }
    }
}