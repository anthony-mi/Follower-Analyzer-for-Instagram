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
            _instagramAPI = instagramAPI;
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

                // TODO: initialize user properties
                newUser.InstagramPK = primaryKey;
                newUser.StateData = instagramUserCookies;

                User foundUser = await _repository.GetAsync<User>(u => u.InstagramPK == primaryKey);

                if(foundUser == null)
                {
                    newUser.LastUpdateDate = DateTime.Now;
                    await _repository.CreateAsync<User>(newUser);
                }
                else
                {
                    foundUser.StateData = instagramUserCookies;
                }
                Session["PrimaryKey"] = primaryKey;
                Session["Authorized"] = true;
                ViewBag.UserName = model.Username;

                return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        public async Task<ActionResult> Logout()
        {
          bool f =  await _instagramAPI.Logout(_repository);

            Session.Abandon();//delete primary key

            if (f)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
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