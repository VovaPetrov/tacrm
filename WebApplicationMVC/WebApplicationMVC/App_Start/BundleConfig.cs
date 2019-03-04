using System.Web;
using System.Web.Optimization;

namespace WebApplicationMVC
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/usersJs").Include(
                       "~/Scripts/users.js"));

            bundles.Add(new StyleBundle("~/bundles/users").Include(
                       "~/Content/users.css"));

            bundles.Add(new ScriptBundle("~/bundles/calendarJs").Include(
                       "~/Content/FullCalendar/fullcalendar.min.js"));

            bundles.Add(new StyleBundle("~/bundles/calendarCss").Include(
                       "~/Content/FullCalendar/fullcalendar.min.css"));

            bundles.Add(new StyleBundle("~/bundles/calendarPage").Include(
                      "~/Content/calendar.css"));

            bundles.Add(new ScriptBundle("~/bundles/calendar").Include(
                       "~/Scripts/calendar.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                      "~/Content/FullCalendar/lib/moment.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/mission").Include(
                        "~/Scripts/Mission.js"));

            bundles.Add(new StyleBundle("~/bundles/contacts").Include(
                        "~/Content/contacts.css"));

            bundles.Add(new StyleBundle("~/bundles/messenger").Include(
                        "~/Content/Messenger.css"));

            bundles.Add(new StyleBundle("~/bundles/tasks").Include(
                        "~/Content/tasks.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
