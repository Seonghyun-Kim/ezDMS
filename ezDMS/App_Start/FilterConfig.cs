using SmartDSP.Filter;
using log4net;
using System.Web;
using System.Web.Mvc;

namespace SmartDSP
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, ILog logger)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ActionFilter());
            filters.Add(new GlobalExceptionFilter(logger));
        }
    
    }
}
