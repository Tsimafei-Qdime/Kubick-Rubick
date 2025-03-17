using System.Web;
using System.Web.Optimization;

namespace BestStudents
{
    public class BundleConfig
    {
        // Дополнительные сведения об объединении см. на странице https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            // Используйте версию Modernizr для разработчиков, чтобы учиться работать. Когда вы будете готовы перейти к работе,
            // готово к выпуску, используйте средство сборки по адресу https://modernizr.com, чтобы выбрать только необходимые тесты.

            bundles.Add(new StyleBundle("~/Content/css").Include(
                     "~/Content/Main.css",
                     "~/Content/Registration.css",
                     "~/Content/Account.css",
                     "~/Content/Shop.css",
                     "~/Content/AdminPanel.css",
                     "~/Content/Autorisation.css"));
        }
    }
}