using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Optimization;

namespace JK.FMS.MVC
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
                "~/Scripts/modernizr-*"
            ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/respond.js",
                "~/Scripts/layout.js"
            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/site.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jscommon").Include(
                "~/Content/admin/assets/global/plugins/jquery.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap/js/bootstrap.min.js"
                , "~/Content/admin/assets/global/plugins/js.cookie.min.js"
                , "~/Content/admin/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js"
                , "~/Content/admin/assets/global/plugins/jquery.blockui.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-tabdrop/js/bootstrap-tabdrop.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jscommon2").Include(
                "~/Content/admin/assets/global/plugins/jquery-ui/jquery-ui.min.js"
                , "~/Content/admin/assets/global/plugins/jquery-validation/js/jquery.validate.min.js"
                , "~/Content/admin/assets/global/plugins/jquery-inputmask/jquery.inputmask.bundle.min.js"
                , "~/Content/admin/assets/global/scripts/datatable.js"
                , "~/Content/admin/assets/global/plugins/datatables/datatables.min.js"
                , "~/Content/admin/assets/global/plugins/datatables/plugins/bootstrap/datatables.bootstrap.js"
                , "~/Content/admin/assets/pages/scripts/table-datatables-managed.min.js"
                , "~/Content/admin/assets/global/plugins/moment.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-typeahead/bootstrap3-typeahead.min.js"
                , "~/Content/admin/assets/global/scripts/app.min.js"
                , "~/Content/admin/assets/global/plugins/select2/js/select2.full.min.js"
                , "~/Content/admin/assets/global/plugins/jquery-validation/js/additional-methods.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-wizard/jquery.bootstrap.wizard.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.min.js"
                , "~/Content/admin/assets/pages/scripts/form-wizard.min.js"
                , "~/Content/admin/assets/pages/scripts/form-validation.min.js"
                , "~/Content/admin/assets/pages/scripts/components-date-time-pickers.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-datepicker/js/bootstrap-datepicker.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-timepicker/js/bootstrap-timepicker.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js"
                , "~/Content/admin/assets/global/plugins/clockface/js/clockface.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/wysihtml5-0.3.0.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.js"
                , "~/Content/admin/assets/global/plugins/ckeditor/ckeditor.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-markdown/lib/markdown.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-markdown/js/bootstrap-markdown.js"
                , "~/Content/admin/assets/layouts/layout2/scripts/layout.min.js"
                , "~/Content/admin/assets/layouts/layout2/scripts/demo.min.js"
                , "~/Content/admin/assets/layouts/global/scripts/quick-sidebar.min.js"
                , "~/Content/admin/assets/layouts/global/scripts/quick-nav.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-sweetalert/sweetalert.min.js"

            ));

            bundles.Add(new ScriptBundle("~/bundles/jscommon12").Include(
                "~/Content/admin/assets/global/plugins/select2/js/select2.full.min.js"
                , "~/Content/admin/assets/global/plugins/jquery-validation/js/jquery.validate.min.js"
                , "~/Content/admin/assets/global/plugins/jquery-validation/js/additional-methods.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-wizard/jquery.bootstrap.wizard.min.js"
                , "~/Content/admin/assets/global/plugins/jquery-inputmask/jquery.inputmask.bundle.min.js"
                , "~/Content/admin/assets/global/plugins/bootstrap-typeahead/bootstrap3-typeahead.min.js"
                , "~/Content/admin/assets/pages/scripts/components-date-time-pickers.min.js"
            ));

            //Added By sohel----- Start
            bundles.Add(new ScriptBundle("~/bundles/corePlugins").Include(
                "~/Content/admin/assets/global/plugins/jquery.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap/js/bootstrap.min.js",
                "~/Content/admin/assets/global/plugins/js.cookie.min.js",
                "~/Content/admin/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                "~/Content/admin/assets/global/plugins/jquery.blockui.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-tabdrop/js/bootstrap-tabdrop.js",
                "~/Content/admin/assets/global/scripts/app.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/mainLayout").Include(
                "~/Content/admin/assets/global/scripts/datatable.js",
                "~/Content/admin/assets/global/plugins/datatables/datatables.min.js",
                "~/Content/admin/assets/global/plugins/datatables/plugins/bootstrap/datatables.bootstrap.js",
                "~/Content/admin/assets/global/plugins/moment.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-datepicker/js/bootstrap-datepicker.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-timepicker/js/bootstrap-timepicker.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js",
                "~/Content/admin/assets/global/plugins/clockface/js/clockface.js",
                "~/Content/admin/assets/pages/scripts/components-date-time-pickers.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/mainLayout2").Include(
                "~/Content/admin/assets/global/plugins/select2/js/select2.full.min.js",
                "~/Content/admin/assets/global/plugins/jquery-validation/js/jquery.validate.min.js",
                "~/Content/admin/assets/global/plugins/jquery-validation/js/additional-methods.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-wizard/jquery.bootstrap.wizard.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/wysihtml5-0.3.0.js",
                "~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.js",
                "~/Content/admin/assets/global/plugins/ckeditor/ckeditor.js",
                "~/Content/admin/assets/global/plugins/bootstrap-markdown/lib/markdown.js",
                "~/Content/admin/assets/global/plugins/bootstrap-markdown/js/bootstrap-markdown.js",
                "~/Content/admin/assets/global/plugins/jquery-inputmask/jquery.inputmask.bundle.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/mainLayout3").Include(
                "~/Content/admin/assets/layouts/layout2/scripts/layout.min.js",
                "~/Content/admin/assets/layouts/layout2/scripts/demo.min.js",
                "~/Content/admin/assets/layouts/global/scripts/quick-sidebar.min.js",
                "~/Content/admin/assets/layouts/global/scripts/quick-nav.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-sweetalert/sweetalert.min.js",
                "~/Content/admin/assets/global/plugins/fullcalendar/fullcalendar.min.js",
                "~/Content/admin/assets/global/plugins/jquery-ui/jquery-ui.min.js",
                "~/Content/admin/assets/apps/scripts/calendar.js",

                "~/Content/admin/assets/global/plugins/ladda/spin.min.js",
                "~/Content/admin/assets/global/plugins/ladda/ladda.min.js",
                "~/Content/admin/assets/pages/scripts/ui-buttons-spinners.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/mainLayout4").Include(
                "~/Content/admin/assets/global/plugins/bootstrap-fileinput/bootstrap-fileinput.js",
                "~/Content/admin/assets/global/plugins/jquery-file-upload/js/vendor/jquery.ui.widget.js",
                "~/Content/admin/assets/global/plugins/jquery-file-upload/js/vendor/tmpl.min.js",
                //"~/Content/admin/assets/pages/scripts/form-fileupload.min.js",
                "~/Content/admin/assets/global/plugins/jquery-file-upload/js/jquery.fileupload.js",
                "~/Content/admin/assets/global/plugins/jquery-file-upload/js/jquery.iframe-transport.js",
                "~/Content/admin/assets/global/plugins/Printjs/print.min.js",
                "~/Content/admin/assets/global/plugins/counterup/jquery.waypoints.min.js",
                "~/Content/admin/assets/global/plugins/counterup/jquery.counterup.min.js",
                "~/Content/admin/assets/global/plugins/bootstrap-multiselect/js/bootstrap-multiselect.js"
            ));


            bundles.Add(new ScriptBundle("~/bundles/mainLayout5").Include(
                "~/Areas/CRM/Scripts/crmapp.js",
                "~/Areas/CRM/Scripts/crmformvalidation.js",
                "~/Areas/CRM/Scripts/crmforminputmask.js",
                "~/Areas/CRM/Scripts/crm_potentialcustomer.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/ScriptsIE").Include(
                "~/Content/admin/assets/global/plugins/respond.min.js",
                "~/Content/admin/assets/global/plugins/excanvas.min.js",
                "~/Content/admin/assets/global/plugins/ie8.fix.min.js"
            ));

            //Added By sohel----- End




            #region Portal Page Bundle

            var bundleMainHeadCssLayout = new StyleBundle("~/Content/mainHeadCssLayout")
               .Include("~/Content/admin/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-datepicker/css/bootstrap-datepicker3.min.css")
                .Include("~/Content/admin/assets/global/css/components-rounded.min.css")
                .Include("~/Content/admin/assets/global/css/plugins.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.css")
                .Include("~/Content/admin/assets/layouts/layout2/css/layout.css")
                .Include("~/Content/admin/assets/layouts/layout2/css/themes/blue.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-fileinput/bootstrap-fileinput.css")
                .Include("~/Content/admin/assets/global/plugins/jquery-nestable/jquery.nestable.css")
                .Include("~/Content/admin/assets/layouts/layout2/css/custom.css")
                .Include("~/fonts/FaceFontcss.css")
                .Include("~/Content/crmstyle/crmstyles.css");
            bundleMainHeadCssLayout.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(bundleMainHeadCssLayout);

            //Style
            var bundlemainCssLayout = new StyleBundle("~/Content/mainPortalCssLayout")
                .Include("~/fonts/FaceFontcss.css")
                .Include("~/Content/admin/assets/global/plugins/font-awesome/css/font-awesome.min.css")
                .Include("~/Content/admin/assets/global/plugins/simple-line-icons/simple-line-icons.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap/css/bootstrap.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css")
                .Include("~/Content/admin/assets/global/plugins/datatables/datatables.min.css")
                .Include("~/Content/admin/assets/global/plugins/datatables/plugins/bootstrap/datatables.bootstrap.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-datepicker/css/bootstrap-datepicker3.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-timepicker/css/bootstrap-timepicker.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-datetimepicker/css/bootstrap-datetimepicker.min.css")
                .Include("~/Content/admin/assets/global/plugins/fullcalendar/fullcalendar.css")
                .Include("~/Content/admin/assets/global/css/components-rounded.min.css")
                .Include("~/Content/admin/assets/global/css/plugins.min.css")
                .Include("~/Content/admin/assets/global/plugins/select2/css/select2.min.css")
                .Include("~/Content/admin/assets/global/plugins/select2/css/select2-bootstrap.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-markdown/css/bootstrap-markdown.min.css")
                .Include("~/Content/admin/assets/layouts/layout2/css/layout.css")
                .Include("~/Content/admin/assets/layouts/layout2/css/themes/blue.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-datepicker/css/bootstrap-datepicker.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-sweetalert/sweetalert.css")
                .Include("~/Content/admin/assets/global/plugins/jquery-ui/jquery-ui.min.css")
                .Include("~/Content/admin/assets/global/plugins/ladda/ladda-themeless.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-fileinput/bootstrap-fileinput.css")
                .Include("~/Content/admin/assets/global/plugins/jquery-file-upload/css/jquery.fileupload-ui.css")
                .Include("~/Content/admin/assets/global/plugins/jquery-file-upload/css/jquery.fileupload.css")
                .Include("~/Content/admin/assets/global/plugins/jquery-nestable/jquery.nestable.css");

            //.Include("~/Content/crmstyle/crmstyles.css")
            //.Include("~/Content/admin/assets/layouts/layout2/css/custom.css");
            bundlemainCssLayout.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(bundlemainCssLayout);



            //Script
            var bundlemainLayout = new ScriptBundle("~/bundles/mainPortalLayout")
                .Include("~/Content/admin/assets/global/plugins/jquery.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap/js/bootstrap.min.js")
                .Include("~/Content/admin/assets/global/plugins/js.cookie.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery.blockui.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-tabdrop/js/bootstrap-tabdrop.js")
                .Include("~/Content/admin/assets/global/scripts/app.min.js")
                .Include("~/Content/admin/assets/global/scripts/datatable.js")
                .Include("~/Content/admin/assets/global/plugins/datatables/datatables.min.js")
                .Include("~/Content/admin/assets/global/plugins/datatables/plugins/bootstrap/datatables.bootstrap.js")
                .Include("~/Content/admin/assets/global/plugins/moment.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-daterangepicker/daterangepicker.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-datepicker/js/bootstrap-datepicker.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-timepicker/js/bootstrap-timepicker.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js")
                .Include("~/Content/admin/assets/global/plugins/clockface/js/clockface.js")
                .Include("~/Content/admin/assets/pages/scripts/components-date-time-pickers.min.js")
                .Include("~/Content/admin/assets/global/plugins/select2/js/select2.full.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-validation/js/jquery.validate.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-validation/js/additional-methods.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-wizard/jquery.bootstrap.wizard.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/wysihtml5-0.3.0.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.js")
                .Include("~/Content/admin/assets/global/plugins/ckeditor/ckeditor.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-markdown/lib/markdown.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-markdown/js/bootstrap-markdown.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-inputmask/jquery.inputmask.bundle.min.js")
                .Include("~/Content/admin/assets/layouts/layout2/scripts/layout.min.js")
                .Include("~/Content/admin/assets/layouts/layout2/scripts/demo.min.js")
                .Include("~/Content/admin/assets/layouts/global/scripts/quick-sidebar.min.js")
                .Include("~/Content/admin/assets/layouts/global/scripts/quick-nav.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-sweetalert/sweetalert.min.js")
                .Include("~/Content/admin/assets/global/plugins/fullcalendar/fullcalendar.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-ui/jquery-ui.min.js")
                .Include("~/Content/admin/assets/apps/scripts/calendar.js")
                .Include("~/Content/admin/assets/global/plugins/ladda/spin.min.js")
                .Include("~/Content/admin/assets/global/plugins/ladda/ladda.min.js")
                .Include("~/Content/admin/assets/pages/scripts/ui-buttons-spinners.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-fileinput/bootstrap-fileinput.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-file-upload/js/vendor/jquery.ui.widget.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-file-upload/js/vendor/tmpl.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-file-upload/js/jquery.fileupload.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-file-upload/js/jquery.iframe-transport.js")
                .Include("~/Content/admin/assets/global/plugins/Printjs/print.min.js")
                .Include("~/Content/admin/assets/global/plugins/counterup/jquery.waypoints.min.js")
                .Include("~/Content/admin/assets/global/plugins/counterup/jquery.counterup.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-multiselect/js/bootstrap-multiselect.js")
                .Include("~/Areas/CRM/Scripts/crmapp.js")
                .Include("~/Areas/CRM/Scripts/crmformvalidation.js")
                .Include("~/Areas/CRM/Scripts/crmforminputmask.js")
                .Include("~/Areas/CRM/Scripts/crm_potentialcustomer.js");
            bundlemainLayout.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(bundlemainLayout);
            #endregion





            #region Login/Region Page Bundle

            //Style
            var bundleLoginCss = new StyleBundle("~/Content/csslogin")
                .Include("~/Content/admin/assets/global/plugins/jquery.min.js")
                .Include("~/Content/admin/assets/global/plugins/font-awesome/css/font-awesome.min.css")
                .Include("~/Content/admin/assets/global/plugins/simple-line-icons/simple-line-icons.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap/css/bootstrap.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css")
                .Include("~/Content/admin/assets/global/plugins/select2/css/select2.min.css")
                .Include("~/Content/admin/assets/global/plugins/select2/css/select2-bootstrap.min.css")
                .Include("~/Content/admin/assets/global/css/components.min.css")
                .Include("~/Content/admin/assets/global/css/plugins.min.css")
                //.Include("~/Content/admin/assets/pages/css/login.css")
                .Include("~/Content/admin/assets/global/plugins/select2/css/select2.min.css")
                .Include("~/Content/admin/assets/global/plugins/select2/css/select2-bootstrap.min.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-wysihtml5/bootstrap-wysihtml5.css")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-markdown/css/bootstrap-markdown.min.css");
            //.Include("~/css/bootstrap-slider.min.css");
            bundleLoginCss.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(bundleLoginCss);


            //Scripts
            var bundleLogin = new ScriptBundle("~/bundles/jslogin")
                .Include("~/Content/admin/assets/global/plugins/jquery.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap/js/bootstrap.min.js")
                .Include("~/Content/admin/assets/global/plugins/js.cookie.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery.blockui.min.js")
                .Include("~/Content/admin/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js")
                .Include("~/Scripts/bootstrap-slider.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-validation/js/jquery.validate.min.js")
                .Include("~/Content/admin/assets/global/plugins/jquery-validation/js/additional-methods.min.js")
                .Include("~/Content/admin/assets/global/plugins/select2/js/select2.full.min.js")
                .Include("~/Content/admin/assets/global/scripts/app.min.js")
                .Include("~/Content/admin/assets/pages/scripts/login.min.js");
            bundleLogin.Orderer = new NonOrderingBundleOrderer();
            bundles.Add(bundleLogin);

            #endregion




            BundleTable.EnableOptimizations = true;
        }
    }

    class NonOrderingBundleOrderer : IBundleOrderer
    {
        public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
        {
            return files;
        }
    }
}
