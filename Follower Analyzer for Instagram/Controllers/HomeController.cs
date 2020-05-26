using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Models.DBInfrastructure;
using Follower_Analyzer_for_Instagram.Models.ViewModels;
using Follower_Analyzer_for_Instagram.Services.InstagramAPI;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
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

        private byte[] GetInstagramCookiesByUserPrimaryKey(string primaryKey)
        {
            //User user = repository.GetAsync<User>(u => u.InstagramPK == primaryKey).Result;
            var user = new User();

            using (var dbContext = new FollowerAnalyzerContext())
            {
                user = dbContext.Users.First(u => u.InstagramPK == primaryKey);
            }

            return user == null ? new byte[] { } : user.StateData;
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

        public async Task<ActionResult> test()
        {
            return View("TestListUsersObserv",await _repository.GetListAsync<User>()) ;
        }
   
        public JsonResult AddUserToObservation()//-
        {
           
            return Json(new {
                User="Ivan",
                Error = "User not added!" });//test
             //Добавление нового пользователя для наблюдения
        }

        public JsonResult GetSubscribersCurrentUser()//+
        {
           List<User> subscribers = _instaApi.GetUserFollowersByUsername(Session["UserName"].ToString());
          
            return Json(subscribers);
        }

        public ActionResult UsersUnderObservation()//-
        {
            return View();//список пользователей под наблюденим
        }

        public ActionResult FinishUserMonitoring(int? id)//-
        {


            return View();//завершение наблюдения за пользователем
        }

        public ActionResult GetAnalysisReport(int? id)//-
        {


            return View();// возращает страницу с отчетом о наблюдение за  пользователем
        }

        public ActionResult TopTenLikes(string userName)
        {
            var posts = _instaApi.GetUserPostsByUsername(userName);
            var sortPosts = from post in posts orderby post.CountOfLikes select post;
            var topTenPosts = new List<InstagramPost>();
            int counter = 0;

            foreach (var post in sortPosts)
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

            foreach (var post in sortPosts)
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
            var posts = _instaApi.GetUserPostsByUsername(Session["UserName"].ToString(), GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));
            var sortPosts = from post in posts orderby post.CountOfComments select post;
            var topTenPosts = new List<InstagramPost>();
            int counter = 0;

            foreach (var post in sortPosts)
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
        public ActionResult GetMostPopularPosts(IndexViewModel viewModel)
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

        public async Task<ActionResult> GetFollowersStatisticsAsync(string userPrimaryKey = null)
        {
            if(String.IsNullOrEmpty(userPrimaryKey))
                userPrimaryKey = System.Web.HttpContext.Current.Session["PrimaryKey"].ToString();
            var user = new User();
            user = await _repository.GetAsync<User>(x => x.InstagramPK == userPrimaryKey);
            var followersStatistics = new FollowersStatisticsViewModel();
            // Get current followers list
            List<User> currentFollowersList = await _instaApi.GetUserFollowersByUsernameAsync(user.Username);
            // Get unsubscribed followers
            foreach(var follower in user.Followers)
            {
                if (!currentFollowersList.Contains(follower))
                    followersStatistics.UnsubscribedFollowers.Add(follower);
            }
            // Get new followers
            foreach (var follower in currentFollowersList)
            {
                if (!user.Followers.Contains(follower))
                    followersStatistics.NewFollowers.Add(follower);
            }
            //If there are changes, then save them in the database
            if (followersStatistics.NewFollowers.Count > 0 || followersStatistics.UnsubscribedFollowers.Count > 0)
            { 
                user.Followers = currentFollowersList;
                await _repository.UpdateAsync<User>(user);
            }
            return PartialView("_FollowersStatistics", followersStatistics);
        }
    }
}