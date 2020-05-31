using Autofac.Util;
using System.Web.Mvc;

namespace Follower_Analyzer_for_Instagram.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Http404(string errorMsg)
        {
            ViewBag.ErrorMsg = errorMsg;
            return View("Http404");
        }
        public ActionResult Http500(string errorMsg)
        {
            ViewBag.ErrorMsg = errorMsg;
            return View("Http500");
        }
    }
}