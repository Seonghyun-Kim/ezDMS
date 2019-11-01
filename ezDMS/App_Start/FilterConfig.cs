using IS_PODS.Filter;
using log4net;
using System.Web;
using System.Web.Mvc;

namespace IS_PODS
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters, ILog logger)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalExceptionFilter(logger));
        }
    
    }
}
