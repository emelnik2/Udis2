using System.Web;
using System.Web.Optimization;

namespace TenantMNG
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.10.2.js",
                        "~/Scripts/jquery.min.js",
                         "~/Scripts/jquery.ui.custom.js",
                          "~/Scripts/jquery.flot.min.js",
                           "~/Scripts/jquery.flot.resize.min.js",
                           "~/Scripts/jquery.maskedinput.min.js"
                         
                        ));

            bundles.Add(new ScriptBundle("~/bundles/matrix").Include(
                      "~/Scripts/matrix.js",
                       "~/Scripts/matrix.dashboard.js",
                        "~/Scripts/matrix.interface.js",
                         "~/Scripts/matrix.chat.js",
                          "~/Scripts/matrix.form_validation.js",
                          "~/Scripts/matrix.popover.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryvalajax").Include(
                     "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/morris").Include(
                       "~/Scripts/morris/raphael-2.1.0.min.js",
                       "~/Scripts/morris/morris.js"));

            bundles.Add(new ScriptBundle("~/bundles/custome").Include(
                      "~/Scripts/custom-scripts.js"
                      ));

        
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                    "~/Scripts/matrix.login.js"
                    ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                                        "~/Content/matrix-style.css",
                                        "~/Content/matrix-media.css",          
                                        "~/font-awesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                                        "~/Content/bootstrap.min.css",
                                        "~/Content/bootstrap-responsive.min.css",
                                        "~/font-awesome/css/font-awesome.css"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                                   "~/Content/matrix-login.css")
                                  );


            bundles.Add(new StyleBundle("~/Content/morriscss").Include(
                                       "~/Script/js/morris/morris-0.4.3.min.css"));
        }
    }
}
