using System.Web;
using System.Web.Optimization;

namespace SevenWonders.WebAPI
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/static/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/static/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/static/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/static/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/static/Scripts/bootstrap.js",
                      "~/static/Scripts/respond.js",
                      "~/static/Scripts/select2.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/static/Content/bootstrap.css",
                      "~/static/Content/site.css",
                      "~/static/Content/vendor/font-awesome/css/font-awesome.min.css",
                      "~/static/Content/vendor/magnific-popup/magnific-popup.css",
                      "~/static/Content/creative.css"));

            bundles.Add(new ScriptBundle("~/bundles/Statistic").Include(
                "~/static/scripts/Statistic.js",
                "~/static/Scripts/Table.js"));
        }
    }
}
