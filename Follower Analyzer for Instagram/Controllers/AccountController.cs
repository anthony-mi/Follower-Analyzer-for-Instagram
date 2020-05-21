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

namespace Follower_Analyzer_for_Instagram.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IRepository _repository;
        private FollowerAnalyzerContext _followerAnalyzerDbContext;

        public AccountController(IRepository repo)
        {
            _repository = repo;
            _followerAnalyzerDbContext = new FollowerAnalyzerContext();
        }

        public AccountController()
        {
        }

        //
        // GET: /Account/Login
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            byte[] instagramUserCookies = null;

            InstagramAPI api = new InstagramAPI();
            bool authenticated = api.TryAuthenticate(model.Username, model.Password, out instagramUserCookies);

            if(authenticated)
            {
                User newUser = new User();
                string primaryKey = api.GetCurrentUserPrimaryKey();
                
                if(string.IsNullOrEmpty(primaryKey))
                {
                    ModelState.AddModelError("", "Authentication error. Try later.");
                    return View(model);
                }

                // TODO: initialize user properties
                newUser.InstagramPK = primaryKey;
                newUser.StateData = instagramUserCookies;

                User foundUser = 
                    _followerAnalyzerDbContext.Users.FirstOrDefault(u => u.InstagramPK == primaryKey);

                if(foundUser != default(User))
                {
                    newUser.LastUpdateDate = DateTime.Now;

                    _followerAnalyzerDbContext.Users.Add(newUser);
                }
                else
                {
                    foundUser.StateData = instagramUserCookies;
                }

                await _followerAnalyzerDbContext.SaveChangesAsync();

                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        #region Helpers
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

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

        ~AccountController()
        {
            _followerAnalyzerDbContext?.Dispose();
        }
    }
}