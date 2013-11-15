using System.Web;
using System.Web.Optimization;

namespace QuizSystem.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/Common").Include(
                        "~/Scripts/common-scripts.js",
                        "~/Scripts/js-validator.js",
                        "~/Scripts/repository-script.js"));

            bundles.Add(new ScriptBundle("~/bundles/AuthorEdit").Include(
                        "~/Scripts/quiz-author-edit-questions.js",
                        "~/Scripts/quiz-author-edit.js"));

            bundles.Add(new ScriptBundle("~/bundles/AuthorIndex").Include(
                       "~/Scripts/quiz-author-index.js"));

            bundles.Add(new ScriptBundle("~/bundles/VoteAction").Include(
                      "~/Scripts/vote-action-script.js"));

            bundles.Add(new ScriptBundle("~/bundles/QuizSolve").Include(
                     "~/Scripts/quiz-solve-script.js"));

            bundles.Add(new ScriptBundle("~/bundles/QuizIndex").Include(
                     "~/Scripts/quiz-all-index.js"));

            bundles.Add(new ScriptBundle("~/bundles/QuizDetails").Include(
                    "~/Scripts/quiz-details-script.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/styles.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
