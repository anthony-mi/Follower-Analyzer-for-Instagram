using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Follower_Analyzer_for_Instagram.Models;
using Follower_Analyzer_for_Instagram.Models.DBInfrastructure;
using Follower_Analyzer_for_Instagram.Services.InstagramAPI;
using System.Web.Security;

namespace Follower_Analyzer_for_Instagram.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IRepository _repository;
        private IInstagramAPI _instagramAPI;

        public AccountController(IRepository repo, IInstagramAPI instagramAPI)
        {
            _repository = repo;
            InitializeInstaAPI(instagramAPI);
        }

        private void InitializeInstaAPI(IInstagramAPI instaApi)
        {
            _instagramAPI = instaApi;

            if(System.Web.HttpContext.Current.Session["PrimaryKey"] == null)
            {
                return;
            }

            string currentUserPrimaryKey = System.Web.HttpContext.Current.Session["PrimaryKey"].ToString();

            if (!string.IsNullOrEmpty(currentUserPrimaryKey))
            {
                _instagramAPI.SetCookies(GetInstagramCookiesByUserPrimaryKey(currentUserPrimaryKey));
            }
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

        public AccountController()
        {
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            byte[] instagramUserCookies = null;

            bool authenticated = _instagramAPI.TryAuthenticate(model.Username, model.Password, out instagramUserCookies);

            if(authenticated)
            {
                User newUser = new User();
                string primaryKey = _instagramAPI.GetCurrentUserPrimaryKey();
                
                if(string.IsNullOrEmpty(primaryKey))
                {
                    ModelState.AddModelError("", "Authentication error. Try later.");
                    return View(model);
                }

                newUser.InstagramPK = primaryKey;
                newUser.StateData = instagramUserCookies;

                User foundUser = await _repository.GetAsync<User>(u => u.InstagramPK == primaryKey);

                if (foundUser == null)
                {
                    newUser.LastUpdateDate = DateTime.Now;
                    await _repository.CreateAsync<User>(newUser);
                }
                else
                {
                    foundUser.StateData = instagramUserCookies;
                    await _repository.UpdateAsync<User>(foundUser);
                }

                Session["PrimaryKey"] = primaryKey;
                Session["Authorized"] = true;
                Session["UserName"] = model.Username;

                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        //
        // GET: /Account/LogOff
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Logout()
        {
            await _instagramAPI.LogoutAsync();
            Session.Abandon();

           return RedirectToAction("Login", "Account");
        }

        

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}