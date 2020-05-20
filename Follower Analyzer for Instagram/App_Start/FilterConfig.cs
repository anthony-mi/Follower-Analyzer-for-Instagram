using System.Web;
using System.Web.Mvc;

namespace Follower_Analyzer_for_Instagram
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
