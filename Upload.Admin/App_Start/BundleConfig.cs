using System.Web;
using System.Web.Optimization;

namespace Upload.Admin
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //login
            bundles.Add(new ScriptBundle("~/bundles/loginjs").Include(
                     "~/Assets/js/jquery.min.js",
                     "~/Assets/js/matrix.login.js"));

            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                      "~/Assets/css/bootstrap.min.css",
                      "~/Assets/css/bootstrap-responsive.min.css",
                      "~/Assets/css/matrix-login.css",
                      "~/Assets/css/font-awesome.css"));

            //error
            bundles.Add(new ScriptBundle("~/bundles/error").Include(
                     "~/Assets/js/jquery.min.js",
                     "~/Assets/js/jquery.ui.custom.js",
                     "~/Assets/js/bootstrap.min.js",
                     "~/Assets/js/maruti.js"));

            bundles.Add(new StyleBundle("~/Content/error").Include(
                      "~/Assets/css/bootstrap.min.css",
                      "~/Assets/css/bootstrap-responsive.min.css",
                      "~/Assets/css/matrix-style.css",
                      "~/Assets/css/matrix-media.css",
                      "~/Assets/css/font-awesome.css"));

            //matrix
            bundles.Add(new ScriptBundle("~/bundles/matrix").Include(
                      "~/Assets/js/jquery.min.js"
                     , "~/Assets/js/jquery.ui.custom.js"
                     , "~/Assets/js/jquery.validate.js"
                     , "~/Assets/js/bootstrap.min.js"
                     , "~/Assets/js/bootstrap-datepicker.js"
                     , "~/Assets/js/bootstrap-colorpicker.js"
                     , "~/Assets/js/masked.js"
                     , "~/Assets/js/jquery.uniform.js"
                     , "~/Assets/js/select2.min.js"
                     , "~/Assets/js/jquery.peity.min.js"
                     , "~/Assets/js/jquery.dataTables.min.js"
                     , "~/Assets/js/jquery.gritter.min.js"
                     , "~/Assets/js/jquery.flot.min.js"
                     , "~/Assets/js/jquery.flot.resize.min.js"
                      , "~/Assets/js/matrix.tables.js"
                     , "~/Assets/js/matrix.js"
                     , "~/Assets/js/matrix.form_common.js"
                     , "~/Assets/js/wysihtml5-0.3.0.js"
                     , "~/Assets/js/excanvas.min.js"
                     , "~/Assets/js/fullcalendar.min.js"
                     , "~/Assets/js/matrix.js"
                     , "~/Assets/js/matrix.dashboard.js"              
                     , "~/Assets/js/matrix.interface.js"                   
                     , "~/Assets/js/matrix.popover.js"
                     , "~/Assets/js/sweetalert.min.js"
                     , "~/Assets/js/bootstrap-wysihtml5.js"));

            bundles.Add(new StyleBundle("~/Content/matrix").Include(
                      "~/Assets/css/bootstrap.min.css"
                      , "~/Assets/css/bootstrap-responsive.min.css"
                      , "~/Assets/css/fullcalendar.css"
                      , "~/Assets/css/colorpicker.css"
                      , "~/Assets/css/datepicker.css"
                      , "~/Assets/css/matrix-style.css"
                      , "~/Assets/css/matrix-media.css"
                      , "~/Assets/css/jquery.gritter.css"
                      , "~/Assets/css/font-awesome.css"
                      , "~/Assets/css/select2.css"
                      , "~/Assets/css/uniform.css"
                      , "~/Assets/css/bootstrap-wysihtml5.css"));
        }
    }
}
