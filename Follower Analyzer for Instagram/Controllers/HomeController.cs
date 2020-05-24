using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Models.DBInfrastructure;
using Follower_Analyzer_for_Instagram.Models.ViewModels;
using Follower_Analyzer_for_Instagram.Services.InstagramAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var viewModel = new IndexViewModel();
            viewModel.Username = string.Empty;
            viewModel.Posts = new List<InstagramPost>();

            return View(viewModel);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult TopTenLikes(string userName)
        {
            string currentUserPrimaryKey = Session["PrimaryKey"].ToString();
            var posts = instaApi.GetUserPostsByUsername(userName, GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));
            var sortPosts = from post in posts orderby post.CountOfLikes select post;
            var topTenPosts = new List<InstagramPost>();
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
            string currentUserPrimaryKey = Session["PrimaryKey"].ToString();
            var posts = instaApi.GetUserPostsByUsername(userName, GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));
            var sortPosts = from post in posts orderby post.CountOfComments select post;
            var topTenPosts = new List<InstagramPost>();
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

        public ActionResult SortingPostsDescOrder()
        {
            string currentUserPrimaryKey = Session["PrimaryKey"].ToString();
            var posts = instaApi.GetUserPostsByUsername(Session["UserName"].ToString(), GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));
            var sortPosts = from post in posts orderby post.CountOfComments select post;
            var topTenPosts = new List<InstagramPost>();
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
            return View("ViewName", topTenPosts);
        }

        [HttpGet]
        public async Task<ActionResult> GetMostPopularPosts(IndexViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            string currentUserPrimaryKey = Session["PrimaryKey"].ToString();
            List<InstagramPost> posts = instaApi.GetUserPostsByUsername(viewModel.Username, GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));
            viewModel.Posts = new List<InstagramPost>();

            if (posts.Count == 0)
            {
                return View("Index", viewModel);
            }

            // Не та сортировка, Паша
            //var sortedByLikesPosts = from post in posts orderby post.CountOfLikes select post;
            var sortedByLikesPosts = from post in posts orderby post.CountOfLikes select post;
            viewModel.Posts.Add(sortedByLikesPosts.Last());

            var sortedByCommentsPosts = from post in posts orderby post.CountOfComments select post;
            viewModel.Posts.Add(sortedByCommentsPosts.Last());

            return View("Index", viewModel);
        }

        private byte[] GetInstagramCookiesByUserPrimaryKey(string primaryKey)
        {
            //User user = repository.GetAsync<User>(u => u.InstagramPK == primaryKey).Result;
            User user = new User();

            using (FollowerAnalyzerContext dbContext = new FollowerAnalyzerContext())
            {
                user = dbContext.Users.First(u => u.InstagramPK == primaryKey);
            }

            return user == null ? new byte[] { } : user.StateData;
        }
    }
}