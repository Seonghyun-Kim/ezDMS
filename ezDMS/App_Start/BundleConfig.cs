using System.Web;
using System.Web.Optimization;

namespace SmartDSP
{
    public class BundleConfig
    {
        // 묶음에 대한 자세한 내용은 https://go.microsoft.com/fwlink/?LinkId=301862를 참조하세요.
        public static void RegisterBundles(BundleCollection bundles)
        {
            //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //            "~/Scripts/jquery-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //@Styles.Render();

            bundles.Add(new StyleBundle("~/bundles/Css").Include(
                 "~/Content/Styles/*.css"                
                ));
            bundles.Add(new StyleBundle("~/bundles/jqwidgetsCss").Include(
                 "~/Content/jqwidgets/styles/jqx.base.css"));
          
            bundles.Add(new StyleBundle("~/bundles/FontAwesome").Include(
             "~/Content/FontAwesome/css/*.css"));


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Content/JQuery/jquery-3.3.1.min.js"));


            //bundles.Add(new ScriptBundle("~/bundles/Script").Include(
            //   "~/Content/Scripts/*.js"));

            bundles.Add(new ScriptBundle("~/bundles/pjax").Include(
               "~/Content/Pjax/jquery.pjax.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqwidgets").Include(
                "~/Content/jqwidgets/*.js"));

         

        }
    }
}
