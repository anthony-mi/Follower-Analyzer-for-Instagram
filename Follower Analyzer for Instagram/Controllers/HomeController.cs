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
        private IRepository _repository;
        private IInstagramAPI _instaApi;

        public HomeController(IRepository repository, IInstagramAPI instaApi)
        {
            _repository = repository;
            InitializeInstaAPI(instaApi);
        }

        private void InitializeInstaAPI(IInstagramAPI instaApi)
        {
            _instaApi = instaApi;

            if (System.Web.HttpContext.Current.Session["PrimaryKey"] == null)
            {
                return;
            }

            string currentUserPrimaryKey = System.Web.HttpContext.Current.Session["PrimaryKey"].ToString();

            if (!string.IsNullOrEmpty(currentUserPrimaryKey))
            {
                _instaApi.SetCookies(GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));
            }
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
            var posts = _instaApi.GetUserPostsByUsername(userName);
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
            var posts = _instaApi.GetUserPostsByUsername(userName);
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

        [HttpGet]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetMostPopularPosts(IndexViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            List<InstagramPost> posts = _instaApi.GetUserPostsByUsername(viewModel.Username);
            viewModel.Posts = new List<InstagramPost>();

            if (posts.Count == 0)
            {
                return View("Index", viewModel);
            }

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