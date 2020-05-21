using Follower_Analyzer_for_Instagram.Models;
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
        private IInstagramAPI instaApi;
        public HomeController(IRepository repo, IInstagramAPI instaApi)
        {
            repository = repo;
            this.instaApi = instaApi;
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

        public ActionResult TopTenLikes(string userName)
        {
            var posts = instaApi.GetUserPostByUsername(userName);
            var sortPosts = from post in posts orderby post.CountOfLikes select post;
            ICollection<InstagramPost> topTenPosts = new List<InstagramPost>();
            int counter = 0;

            foreach (InstagramPost post in sortPosts)
            {
                if (post != null)
                {
                    topTenPosts.Add(post);
                }

                if (counter == 10)
                {
                    break;
                }
                counter++;
            }
            return View("ListPosts", topTenPosts);
        }

        public ActionResult TopTenByComments(string userName)
        {
            var posts = instaApi.GetUserPostByUsername(userName);
            var sortPosts = from post in posts orderby post.CountOfComments select post;
            ICollection<InstagramPost> topTenPosts = new List<InstagramPost>();
            int counter = 0;

            foreach (InstagramPost post in sortPosts)
            {
                if (post != null)
                {
                    topTenPosts.Add(post);
                }

                if (counter == 10)
                {
                    break;
                }
                counter++;
            }
            return View("ListPosts", topTenPosts);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}