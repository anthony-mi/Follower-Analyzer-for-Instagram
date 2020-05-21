using Follower_Analyzer_for_Instagram.Models.DBInfrastructure;
using Follower_Analyzer_for_Instagram.Services.InstagramAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Follower_Analyzer_for_Instagram.Controllers
{
    public class HomeController : Controller
    {
        IRepository repository;
        public HomeController(IRepository repo)
        {
            repository = repo;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult TopTenTikes(string UserName)
        {




            return View();
        }

        public ActionResult TopTenByComments(string UserName)
        {
            InstagramAPI InstaApi = new InstagramAPI();
        //  var userInfo = InstaApi.UserProcessor.GetUserInfoByUsername(UserName);

            return View();
        }

        public ActionResult TopTenTikes(string UserName)
        {




            return View();
        }

        public ActionResult TopTenByComments(string UserName)
        {
       

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}