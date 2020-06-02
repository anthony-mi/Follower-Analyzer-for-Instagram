using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Services.InstagramAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Management;
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

            if (System.Web.HttpContext.Current.Session["PrimaryKey"] != null)
            {
                string currentUserPrimaryKey = System.Web.HttpContext.Current.Session["PrimaryKey"].ToString();

                if (!string.IsNullOrEmpty(currentUserPrimaryKey))
                {
                    _instaApi.SetCookies(GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));
                }
            }
        }

        private byte[] GetInstagramCookiesByUserPrimaryKey(string primaryKey)
        {
            //User user = repository.GetAsync<User>(u => u.InstagramPK == primaryKey).Result;
            var user = new ApplicationUser();

            using (var dbContext = new FollowerAnalyzerContext())
            {
                user = dbContext.ApplicationUsers.First(u => u.InstagramPK == primaryKey);
            }

            return user == null ? new byte[] { } : user.StateData;
        }

        public ActionResult Index(string status = "")
        {
            var viewModel = new IndexViewModel();
            viewModel.Username = string.Empty;
            viewModel.Posts = new List<InstagramPost>();
            ViewBag.RequestState = status;
            return View(viewModel);
        }

        public ActionResult About()
        {
            return View();
        }

        public void ShowError(string errorMsg)
        {
            this.ViewData["ShowError"] = errorMsg;
        }
   
        public async Task< JsonResult> AddUserToObservation(string userName)
        {
            var errors = new List<string>();
            var observableUser = new ObservableUser();

            if ((await _repository.GetListAsync<ObservableUser>()).ToList().Count() < 4)
            {
                observableUser.InstagramPK = _instaApi.GetPrimaryKeyByUsername(userName);
                observableUser.Username = userName;
                bool added = await _repository.CreateAsync(observableUser);

                if (!added)
                {
                    errors.Add( "Не удалось добавить пользователя!");
                    throw new HttpException("Не удалось добавить пользователя!");
                }
                else
                {
                    Response.StatusCode = 200;//ok
                }
            }
            else
            {
                errors.Add("Не удалось добавить пользователя!");
                errors.Add("Максимальное количесво пользователей за которыми возможно наблюдение [3]!"); 
                throw new HttpException("Не удалось добавить пользователя!");
            }

            return Json(new
            {
                observableUser = observableUser,
                Errors = errors
            });
        }

        public JsonResult GetSubscribersCurrentUser()
        {
            List<ApplicationUser> subscribers = _instaApi.GetUserFollowersByUsername(Session["UserName"].ToString());

            return Json(subscribers);
        }

        /// <summary>
        /// завершить наблюдения за пользователем
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> FinishUserMonitoring(int? id)
        {
            var observableUsers = (await _repository.GetListAsync<ObservableUser>()).ToList();
            var observableUser = observableUsers.Where(user => user.Id == id).FirstOrDefault();

            bool deleted = await _repository.DeleteAsync<ObservableUser>(observableUser);

            if (!deleted)
            {
                Response.StatusCode = 500;//Internal Server Error
            }
            else
            {
                Response.StatusCode = 200;//Ok
            }

            return Json(string.Empty);
        }

        public ActionResult TopTenLikes(string nameForLikes)
        {
            var userPK = _instaApi.GetPrimaryKeyByUsername(nameForLikes);
            var topTenPosts = new List<InstagramPost>();

            if (userPK == "" || userPK == null)
            {
                ShowError("Не удалось найти пользователя с таким именем!");
                return View(topTenPosts);
            }

            List<InstagramPost> posts = _instaApi.GetUserPostsByUsername(nameForLikes);

            if (posts == null || posts.Count() == 0)
            {
                ShowError("Не удалось найти публикации!");
                return View(topTenPosts);
            }

            var sortPosts = from post in posts orderby post.CountOfLikes descending select post;
            int counter = 0;

            foreach (var post in sortPosts)
            {
                if (post != null)
                {
                    topTenPosts.Add(post);
                }

                if (counter == 9)
                {
                    break;
                }
                counter++;
            }
            return PartialView(topTenPosts);
        }

        public ActionResult TopTenByComments(string nameForComments)
        {
            var userPK = _instaApi.GetPrimaryKeyByUsername(nameForComments);
            var topTenPosts = new List<InstagramPost>();

            if (userPK == "" || userPK == null)
            {
                ShowError("Не удалось найти пользователя с таким именем!");
                return View(topTenPosts);
            }

            List<InstagramPost> posts = _instaApi.GetUserPostsByUsername(nameForComments);

            if (posts == null || posts.Count() == 0)
            {
                ShowError("Не удалось найти публикации!");
                return View(topTenPosts);
            }

            var sortPosts = from post in posts orderby post.CountOfLikes descending select post;
            int counter = 0;

            foreach (var post in sortPosts)
            {
                if (post != null)
                {
                    topTenPosts.Add(post);
                }

                if (counter == 9)
                {
                    break;
                }
                counter++;
            }
            return PartialView(topTenPosts);
        }

        public ActionResult SortingPostsDescOrder()
        {
            var topTenPosts = new List<InstagramPost>();  
            string currentUserPrimaryKey = Session["PrimaryKey"].ToString();
            var posts = _instaApi.GetUserPostsByUsername(Session["UserName"].ToString(), GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));

            if (posts == null || posts.Count() == 0)
            {
                ShowError("Не удалось найти публикации!");
                return View(topTenPosts);
            }

            var sortPosts = from post in posts orderby post.CountOfComments select post;
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

        public ActionResult GetMostPopularPosts(string name)
        {
            var viewModel = new IndexViewModel();

            var userPK = _instaApi.GetPrimaryKeyByUsername(name);
           
            if (userPK == "" || userPK == null)
            {
                ShowError("Не удалось найти пользователя с таким именем!");
                return View(viewModel);
            }

            List<InstagramPost> posts = _instaApi.GetUserPostsByUsername(name);
            viewModel.Username = name;

            viewModel.Posts = new List<InstagramPost>();

            if (posts.Count == 0)
            {
                ShowError("Не удалось найти публикации!");
                return View(viewModel);
            }

            posts.Sort((post1, post2) => post1.CountOfLikes.CompareTo(post2.CountOfLikes));
            viewModel.Posts.Add(posts.Last());

            posts.Sort((post1, post2) => post1.CountOfComments.CompareTo(post2.CountOfComments));
            viewModel.Posts.Add(posts.Last());

            return PartialView(viewModel);
        }

        public async Task<ActionResult> GetSubscriptionsStatisticsAsync(string userPrimaryKey = null)
        {
            if (String.IsNullOrEmpty(userPrimaryKey))
                userPrimaryKey = System.Web.HttpContext.Current.Session["PrimaryKey"].ToString();
            var user = new ApplicationUser();
            user = await _repository.GetAsync<ApplicationUser>(x => x.InstagramPK == userPrimaryKey);
            var subscriptionsStatistics = new SubscriptionsStatisticsViewModel();
            // Get current followers list
            List<ApplicationUser> currentSubscriptionsList = await _instaApi.GetUserSubscriptionsByUsernameAsync(user.Username);
            // Get unsubscribed followers
            foreach (var subscription in user.Subscriptions)
            {
                if (!currentSubscriptionsList.Contains(subscription))
                    subscriptionsStatistics.UnsubscribedSubscriptions.Add(subscription);
            }
            // Get new followers
            foreach (var subscription in currentSubscriptionsList)
            {
                if (!user.Subscriptions.Contains(subscription))
                    subscriptionsStatistics.NewSubscriptions.Add(subscription);
            }
            //If there are changes, then save them in the database
            if (subscriptionsStatistics.NewSubscriptions.Count > 0 || subscriptionsStatistics.UnsubscribedSubscriptions.Count > 0)
            {
                user.Subscriptions = currentSubscriptionsList;
                await _repository.UpdateAsync<ApplicationUser>(user);
            }
            return PartialView(subscriptionsStatistics);
        }

        public async Task<ActionResult> GetObservableUserActivities(string userName)
        {
            var activities = new UserActivityViewModel();
            activities.UserName = userName;
            string primaryKey = _instaApi.GetPrimaryKeyByUsername(userName);
            activities.ProfilePictureUrl = await _instaApi.GetUserProfilePictureUriByPrimaryKeyAsync(primaryKey);
            var userActivities = (await _repository.GetListAsync<UserActivity>(x => x.InitiatorPrimaryKey == primaryKey)).ToList();
            activities.Activities = userActivities;
            return PartialView(activities);
        }

        [HttpGet]
        public ActionResult AddObservableUser()
        {
            return PartialView("_AddObservableUser");
        }

        [HttpPost]
        public async Task<ActionResult> AddObservableUser(ObservableUserViewModel observableUser)
        {
            // The current registered user who uses our application
            string primaryKey = System.Web.HttpContext.Current.Session["PrimaryKey"].ToString();
            var observer = await _repository.GetAsync<ApplicationUser>(u => u.InstagramPK == primaryKey);

            // The user we are going to add to the database
            var user = new ObservableUser();
            user.Username = observableUser.UserName;
            user.InstagramPK = _instaApi.GetPrimaryKeyByUsername(observableUser.UserName);
            user.Observers.Add(observer);

            // Checking the presence of the user we want to add to the database in the database
            var observable = await _repository.GetAsync<ObservableUser>(x => x.InstagramPK == user.InstagramPK);
            if (observable != null)
            {
                // If such a user exists, check the existence of the current user in the list of its observers
                if (observable.Observers.Contains(observer))
                {
                    // If such a observer exists, return a message about it.
                    return RedirectToAction("Index", new { status = "repeat" });
                }
                else
                {
                    /* If such a observer does not exist, add it to the collection of observers, 
                     update the data in the database and return a message about the successful operation.*/
                    observable.Observers.Add(observer);
                    await _repository.UpdateAsync<ObservableUser>(observable);
                    observer.ObservableAccounts.Add(observable);
                    await _repository.UpdateAsync<ApplicationUser>(observer);
                    return RedirectToAction("Index", new { status = "success" });
                }
            }

            // Trying to add a new observable user to the database
            if (!await _repository.CreateAsync<ObservableUser>(user))
                return RedirectToAction("Index", new { status = "bad" });

            // Update the data in the database
            observer.ObservableAccounts.Add(user);
            await _repository.UpdateAsync<ApplicationUser>(observer);

            // Start of activity analysis of a new observable user
            Startup.ActivityAnalizer.AddUserForObservation(observer, user);

            // Return a message about the successful operation
            return RedirectToAction("Index", new { status = "success" });
        }

        [HttpGet]
        public async Task<ActionResult> AddObservableAccountForObservableUser()
        {
            string instaPK = _instaApi.GetCurrentUserPrimaryKey();
            var currentUser = await _repository.GetAsync<ApplicationUser>(x => x.InstagramPK == instaPK);
            var observablePage = new ObservableAccountForObservableUserVM();

            foreach (var observableUser in currentUser.ObservableAccounts)
                observablePage.ObservableUsers.Add(new SelectListItem
                {
                    Text = observableUser.Username,
                    Value = observableUser.Username
                });
            return PartialView("_AddObservableAccountForObservableUser", observablePage);
        }

        [HttpPost]
        public async Task<ActionResult> AddObservableAccountForObservableUser(ObservableAccountForObservableUserVM observablePage)
        {
            // Receive the observable user
            var observableUser = new ObservableUser();
            observableUser = await _repository.GetAsync<ObservableUser>(x => x.Username == observablePage.observableUserName);

            // Checking the presence of the target content we want to add to the database in the database
            var page = await _repository.GetAsync<ObservableUser>(x => x.Username == observablePage.TargetContentName);
            if (page != null)
            {
                // If such content exists, check the existence of the observable user in the list of its ActivityInitiators
                if (observableUser != null && page.ActivityInitiators.Contains(observableUser))
                {
                    return RedirectToAction("Index", new { status = "repeat" });
                }
                else
                {
                    /* If such observable user does not exist, add it to the collection of ActivityInitiators, 
                     update the data in the database and return a message about the successful operation.*/
                    page.ActivityInitiators.Add(observableUser);
                    await _repository.UpdateAsync<ObservableUser>(page);
                    observableUser.ObservableUsers.Add(page);
                    await _repository.UpdateAsync<ObservableUser>(observableUser);
                    return RedirectToAction("Index", new { status = "success" });
                }
            }

            // If such content is not in the database, then we initialize the new content
            page = new ObservableUser();
            page.InstagramPK = _instaApi.GetPrimaryKeyByUsername(observablePage.TargetContentName);
            page.Username = observablePage.TargetContentName;
            page.ActivityInitiators.Add(observableUser);

            // Trying to add a new content to the database
            if (!await _repository.CreateAsync<ObservableUser>(page))
                return RedirectToAction("Index", new { status = "bad" });

            // Update the data in the database
            observableUser.ObservableUsers.Add(page);
            await _repository.UpdateAsync<ObservableUser>(observableUser);

            // Return a message about the successful operation
            return RedirectToAction("Index", new { status = "success" });
        }

        public async Task<ActionResult> GetStatisticsByLikers(string userName, string sortType = "descending")
        {
            // Check the userName parameter. If it is empty, then set the name of the current user.
            if (String.IsNullOrEmpty(userName))
                userName = System.Web.HttpContext.Current.Session["UserName"].ToString();
            string userPK = _instaApi.GetPrimaryKeyByUsername(userName);
            // Get user's posts
            List<InstagramPost> userPosts = await _instaApi.GetUserPostsByPrimaryKeyAsync(userPK);
            // Create a dictionary for storing likers (key - user, value - number of likes)
            Dictionary<User, int> Likers = new Dictionary<User, int>();
            // Check the likers of each post and fill out our dictionary
            foreach (var post in userPosts)
            {
                if (Likers.Count == 0)
                    foreach (var liker in post.Likers)
                        Likers.Add(liker, 1);

                foreach (var liker in post.Likers)
                    if (Likers.ContainsKey(liker))
                        Likers[liker]++;
                    else
                        Likers.Add(liker, 1);
            }
            // return a partial view with a sorted dictionary, depending on the parameter sortType
            if (sortType == "descending")
                return PartialView("_GetStatisticsByLikers", Likers.OrderBy(x => x.Value));
            else
                return PartialView("_GetStatisticsByCommenters", Likers.OrderByDescending(x => x.Value));
        }

        public async Task<ActionResult> GetStatisticsByCommenters(string userName, string sortType = "descending")
        {
            // Check the userName parameter. If it is empty, then set the name of the current user.
            if (String.IsNullOrEmpty(userName))
                userName = System.Web.HttpContext.Current.Session["UserName"].ToString();
            string userPK = _instaApi.GetPrimaryKeyByUsername(userName);
            // Get user's posts
            List<InstagramPost> userPosts = await _instaApi.GetUserPostsByPrimaryKeyAsync(userPK);
            // Create a dictionary for storing commenters (key - user, value - number of comments)
            Dictionary<User, int> Commenters = new Dictionary<User, int>();
            // Check the commenters of each post and fill out our dictionary
            foreach (var post in userPosts)
            {
                if (Commenters.Count == 0)
                    foreach (var commenter in post.Commenters)
                        Commenters.Add(commenter, 1);

                foreach (var commenter in post.Commenters)
                    if (Commenters.ContainsKey(commenter))
                        Commenters[commenter]++;
                    else
                        Commenters.Add(commenter, 1);
            }
            // return a partial view with a sorted dictionary, depending on the parameter sortType
            if (sortType == "descending")
                return PartialView("_GetStatisticsByLikers", Commenters.OrderBy(x => x.Value));
            else
                return PartialView("_GetStatisticsByCommenters", Commenters.OrderByDescending(x => x.Value));
        }
    }
}