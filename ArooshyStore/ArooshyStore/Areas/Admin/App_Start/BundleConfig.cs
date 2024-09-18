using System.Web.Optimization;

namespace ArooshyStore.Areas.Admin
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Generic
            bundles.Add(new StyleBundle("~/areas/admin/content/smartadmin").Include(
                "~/Areas/Admin/Content/vendors.bundle.css",
                "~/Areas/Admin/Content/app.bundle.css",
                "~/Areas/Admin/Content/miscellaneous/reactions/reactions.css",
                "~/Areas/Admin/Content/miscellaneous/fullcalendar/fullcalendar.bundle.css",
                "~/Areas/Admin/Content/miscellaneous/jqvmap/jqvmap.bundle.css",
                "~/Areas/Admin/Content/datagrid/datatables/datatables.bundle.css",
                "~/Areas/Admin/Content/notifications/toastr/toastr.css",
                "~/Areas/Admin/Content/theme-demo.css",
                "~/Areas/Admin/Content/formplugins/select2/select2.bundle.css",
                "~/Areas/Admin/Content/fa-solid.css",
                "~/Areas/Admin/Content/formplugins/summernote/summernote.css",
                "~/Areas/Admin/Content/RichTextEditor/richtext.min.css"
               ));
            bundles.Add(new ScriptBundle("~/areas/admin/script/smartadmin").Include(
                "~/Areas/Admin/Scripts/vendors.bundle.js",
                "~/Areas/Admin/Scripts/app.bundle.js",
                "~/Areas/Admin/Scripts/dependency/moment/moment.js",
                "~/Areas/Admin/Scripts/miscellaneous/fullcalendar/fullcalendar.bundle.js",
                "~/Areas/Admin/Scripts/statistics/sparkline/sparkline.bundle.js",
                "~/Areas/Admin/Scripts/statistics/easypiechart/easypiechart.bundle.js",
                "~/Areas/Admin/Scripts/statistics/flot/flot.bundle.js",
                "~/Areas/Admin/Scripts/miscellaneous/jqvmap/jqvmap.bundle.js",
                "~/Areas/Admin/Scripts/datagrid/datatables/datatables.bundle.js",
                "~/Areas/Admin/Scripts/notifications/toastr/toastr.js",
                "~/Areas/Admin/Scripts/formplugins/select2/select2.bundle.js",
                "~/Areas/Admin/Scripts/formplugins/summernote/summernote.js",
                "~/Areas/Admin/Scripts/RichTextEditor/jquery.richtext.js",
                "~/Areas/Admin/Scripts/formplugins/inputmask/inputmask.bundle.js"
               ));
            bundles.Add(new StyleBundle("~/areas/admin/content/login").Include(
                "~/Areas/Admin/Content/vendors.bundle.css",
                "~/Areas/Admin/Content/app.bundle.css",
                "~/Areas/Admin/Content/notifications/toastr/toastr.css",
                "~/Areas/Admin/Content/formplugins/select2/select2.bundle.css",
                "~/Areas/Admin/Content/fa-solid.css",
                "~/Areas/Admin/Content/page-login.css"
               ));
            bundles.Add(new ScriptBundle("~/areas/admin/script/login").Include(
                "~/Areas/Admin/Scripts/vendors.bundle.js",
                "~/Areas/Admin/Scripts/app.bundle.js",
                "~/Areas/Admin/Scripts/notifications/toastr/toastr.js",
                "~/Areas/Admin/Scripts/formplugins/select2/select2.bundle.js"
               ));
            #endregion
            #region jQuery Validator
            bundles.Add(new ScriptBundle("~/areas/admin/script/jqueryvalidator").Include(
               "~/Areas/Admin/Scripts/jquery.validate.min.js",
               "~/Areas/Admin/Scripts/jquery.validate.unobtrusive.js"
              ));
            #endregion
            #region Jquery Confirm
            bundles.Add(new StyleBundle("~/areas/admin/content/confirm").Include(
             "~/Areas/Admin/Content/jquery-confirm.min.css"
            ));
            bundles.Add(new ScriptBundle("~/areas/admin/script/confirm").Include(
              "~/Areas/Admin/Scripts/jquery-confirm.min.js"
              ));
            #endregion
            #region jQuery UI
            bundles.Add(new ScriptBundle("~/areas/admin/bundles/jqueryui").Include(
                  "~/Areas/Admin/Scripts/JqueryUI/jquery-ui-{version}.min.js"));
            //css 
            bundles.Add(new StyleBundle("~/Areas/Admin/Content/JqueryUI/cssjQueryUI").Include(
                  "~/Areas/Admin/Content/JqueryUI/jquery-ui.min.css",
                  "~/Areas/Admin/Content/JqueryUI/core.css",
                  //"~/Areas/Admin/Content/JqueryUI/all.css",
                  //"~/Areas/Admin/Content/JqueryUI/base.css",
                  "~/Areas/Admin/Content/JqueryUI/Base_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Black-Tie_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Blitzer_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Cupertino_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Dark-Hive_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Dot-Luv_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/EggPlant_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Excite-Bite_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Hot-Sneaks_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Humanity_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Le-Frog_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Mint-Choc_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Overcast_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Pepper-Grinder_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Red-Mond_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Smoothness_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/South-Street_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Start_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Sunny_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Swanky-Purse_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Trontastic_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/UI-Darkness_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/UI-Lightness_Theme.css",
                  //"~/Areas/Admin/Content/JqueryUI/Vader_Theme.css",
                  "~/Areas/Admin/Content/JqueryUI/accordion.css",
                  "~/Areas/Admin/Content/JqueryUI/autocomplete.css",
                  "~/Areas/Admin/Content/JqueryUI/button.css",
                  "~/Areas/Admin/Content/JqueryUI/datepicker.css",
                  "~/Areas/Admin/Content/JqueryUI/dialog.css",
                  "~/Areas/Admin/Content/JqueryUI/draggable.css",
                  "~/Areas/Admin/Content/JqueryUI/menu.css",
                  "~/Areas/Admin/Content/JqueryUI/progressbar.css",
                  "~/Areas/Admin/Content/JqueryUI/resizable.css",
                  "~/Areas/Admin/Content/JqueryUI/selectable.css",
                  "~/Areas/Admin/Content/JqueryUI/selectmenu.css",
                  "~/Areas/Admin/Content/JqueryUI/slider.css",
                  "~/Areas/Admin/Content/JqueryUI/sortable.css",
                  "~/Areas/Admin/Content/JqueryUI/spinner.css",
                  "~/Areas/Admin/Content/JqueryUI/tabs.css",
                  "~/Areas/Admin/Content/JqueryUI/tooltip.css"
                  ));
            //bundles.Add(new StyleBundle("~/Content/themes/base/cssjQueryUI").IncludeDirectory(
            //     "~/Content/themes/base","*.css",true));
            #endregion
            #region Layout
            bundles.Add(new StyleBundle("~/areas/admin/content/layout").Include(
              "~/Areas/Admin/Content/ArooshyStoreCss/Layout/Header.css",
              "~/Areas/Admin/Content/ArooshyStoreCss/Layout/Layout.css",
              "~/Areas/Admin/Content/SearchBox.css"
             ));
            bundles.Add(new ScriptBundle("~/areas/admin/script/layout").Include(
            "~/Areas/Admin/Scripts/ArooshyStoreScripts/Layout/Header.js",
            "~/Areas/Admin/Scripts/ArooshyStoreScripts/Home/QuickAccesss.js",
            "~/Areas/Admin/Scripts/ArooshyStoreScripts/Layout/Layout.js",
            "~/Areas/Admin/Scripts/ArooshyStoreScripts/Layout/BackgroundTaskToaster.js"
            ));
            #endregion
            BundleTable.EnableOptimizations = true;
        }
    }
}
