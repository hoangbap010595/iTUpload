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
                //jQuery 3
                "~/Assets/js/jquery.min.js"
                //jQuery UI 1.11.4
                , "~/Assets/js/jquery-ui.min.js"
                //Bootstrap 3.3.7
                , "~/Assets/js/bootstrap.min.js"
                //Morris.js charts
                //, "~/Assets/js/raphael.min.js"
                //, "~/Assets/js/morris.min.js"
                //Sparkline
                , "~/Assets/js/jquery.sparkline.min.js"
                //jvectormap
                , "~/Assets/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js"
                , "~/Assets/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"
                //jQuery Chart JS
                , "~/Assets/plugins/chart.js/Chart.js"
                //daterangepicker
                , "~/Assets/js/moment.min.js"
                , "~/Assets/js/daterangepicker.js"
                //datepicker
                , "~/Assets/js/bootstrap-datepicker.min.js"
                //CK Editor
                , "~/Assets/plugins/ckeditor/ckeditor.js"
                , "~/Assets/plugins/ckfinder/ckfinder.js"
                //Bootstrap WYSIHTML5
                , "~/Assets/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js"
                //Slimscroll
                , "~/Assets/js/jquery.slimscroll.min.js"
                //FastClick
                , "~/Assets/js/fastclick.js"
                //DataTables
                , "~/Assets/plugins/datatables.net/js/jquery.dataTables.min.js"
                , "~/Assets/plugins/datatables.net-bs/js/dataTables.bootstrap.min.js"
                , "~/Assets/plugins/datatables.net/js/dataTables.fixedColumns.min.js"
                , "~/Assets/plugins/datatables.net/js/dataTables.select.min.js"
                , "~/Assets/plugins/datatables.net/js/dataTables.keyTable.min.js"
                //Sweat alert
                , "~/Assets/plugins/sweetalert/sweetalert.min.js"
                 //Select
                 , "~/Assets/plugins/select2/dist/js/select2.full.min.js"
                //AdminLTE App
                , "~/Assets/dist/js/adminlte.min.js"
                //CheckBox
                , "~/Assets/plugins/iCheck/icheck.min.js"
                //Munti selected dropdownlist
                , "~/Assets/dist/js/bootstrap-multiselect.js"
                //, "~/Assets/dist/js/pages/dashboard.js"
                //, "~/Assets/dist/js/demo.js"
                //UserControl
                , "~/Scripts/UserControl1.0.js"));

            bundles.Add(new StyleBundle("~/Content/matrix").Include(
                //Bootstrap 3.3.7
                "~/Assets/css/bootstrap.min.css"
                //Font Awesome
                , "~/Assets/css/font-awesome.min.css"
                //Ionicons
                , "~/Assets/css/ionicons.min.css"
                //Theme style
                , "~/Assets/dist/css/AdminLTE.min.css"
                //AdminLTE Skins. Choose a skin from the css/skins
                //folder instead of downloading all of them to reduce the load.
                , "~/Assets/dist/css/skins/_all-skins.min.css"
                //Morris chart
                //, "~/Assets/css/morris.js/morris.css"
                //jvectormap
                , "~/Assets/css/jquery-jvectormap.css"
                //Date Picker
                , "~/Assets/css/bootstrap-datepicker.min.css"
                //Daterange picker
                , "~/Assets/css/daterangepicker.css"
                //bootstrap wysihtml5 - text editor
                , "~/Assets/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css"
                //DataTables 
                , "~/Assets/plugins/datatables.net-bs/css/dataTables.bootstrap.min.css"
                //Sweat alert
                , "~/Assets/plugins/sweetalert/sweetalert.css"
                //Selected
                , "~/Assets/plugins/select2/dist/css/select2.min.css"
                //iCheck
                , "~/Assets/plugins/iCheck/square/_all.css"
                //Munti selected dropdownlist
                , "~/Assets/dist/css/bootstrap-multiselect.css"
                //Upload CSS
                , "~/Content/upload/css/component.css"
                , "~/Content/upload/css/normalize.css"
                //, "~/Content/upload/css/demo.css"
                //Define CSS
                , "~/Assets/css/style.css"));
        }
    }
}
