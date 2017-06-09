using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using PPI.Core.Web.Models;
using PPI.Core.Domain.Entities;
using PPI.Core.Domain.Abstract;
using PPI.Core.Web.Infrastructure;
using System.Linq;
using PPI.Core.Web.Models.ViewModel;
using System;
using System.Net.Mail;

namespace PPI.Core.Web.Controllers
{


    [Authorize]
    public class AccountController : BaseController
    {

        public AccountController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        [Log]
        public UserManager<ApplicationUser> UserManager { get; private set; }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        [Log]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [Log]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //Check box for remember me, update the model with the real value
                bool Rem = (Request["checkbox-RememberMe"] == "on") ? true : false;
                model.RememberMe = Rem;
                ///
                var user = await UserManager.FindAsync(model.UserName, model.Password);

                if (user != null)
                {
                    //If the user exists check if the account is active or not
                    if (user.Active) {
                        await SignInAsync(user, model.RememberMe);
                        //Look up the site associated with this user and set the cookie,                                                            
                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("", "User is not active, please contact a system administrator.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private List<SelectListItem> UsersRoles()
        {
            var Roles = UnitOfWork.IAspNetRoleRepository.AsQueryable();
            var RolesList = new List<SelectListItem>();
            foreach (var item in Roles)
            {
                var newItem = new SelectListItem();
                newItem.Text = item.Name;
                newItem.Value = item.Id;
                RolesList.Add(newItem);
            }
            return RolesList;
        }

        private List<SelectListItem> SitesLists()
        {
            var Sites = UnitOfWork.ISiteRepository.AsQueryable();
            var SitesList = new List<SelectListItem>();
            foreach (var item in Sites)
            {
                var newItem = new SelectListItem();
                newItem.Text = item.FriendlyName;
                newItem.Value = item.Id.ToString();
                SitesList.Add(newItem);
            }
            return SitesList;
        }
        //
        // GET: /Account/Register
        [Log]
        [Authorize(Roles = "Admin")]
        public ActionResult Register()
        {
            ViewData["Site"] = new SelectList(SitesLists(), "Value", "Text");
            return View(new PPI.Core.Web.Models.RegisterViewModel());
        }

        //
        // POST: /Account/Register
        [Log]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [MvcHaack.Ajax.ValidateJsonAntiForgeryToken]
        public async Task<ActionResult> Register(string email, string userName, string password, string confirmPassword, string usersite, List<string> roles)
        {
            //Check if username is not in the database already
            if (userRegisterd(userName)) {
                return Json(new { error = true, message = "The username is already registered in the system, please try again with another username." });
            } else if(validUsername(userName)) {
                //User does not exist and is longer than 3 characterswe can keep moving forward
                if (passwordsEqual(password, confirmPassword) && password.Length >= 6)
                {
                    //Check the role and the e-mails
                    if(roles.Count > 0)
                    {
                        //Check if e-mails are valid
                        if (validateEmail(email))
                        {
                            var user = new ApplicationUser() { UserName = userName, Active = true };
                            var result = await UserManager.CreateAsync(user, password);
                            
                            if (result.Succeeded)
                            {
                                foreach (var r in roles)
                                {
                                    user.Roles.Add(new IdentityUserRole() { RoleId = r, UserId = user.Id });
                                }
                                UserManager.Update(user);

                                PPI.Core.Domain.Entities.SiteUser SiteUser = new PPI.Core.Domain.Entities.SiteUser();
                                SiteUser.AspNetUsersId = user.Id;
                                SiteUser.SiteId = Convert.ToInt32(usersite);
                                
                                UnitOfWork.ISiteUserRepository.Add(SiteUser);
                                UnitOfWork.Commit();
                                await SignInAsync(user, isPersistent: false);
                                return Json(new { error = false , message = "User created successfully."});
                            }
                            else
                            {
                                return Json(new { error = true, message = "Error creating user." });
                            }
                        }
                        else
                        {
                            return Json(new { error = true, message = "Please insert a valid e-mail." });
                        }
                    }
                    else
                    {
                        return Json(new { error = true, message = "Select 1 more or for the user." });
                    }

                }
                else
                {
                    return Json(new { error = true, message = "Inserted passwords need to match and passwords need to be 6 characters or more." });
                }
            }
            else
            {
                return Json(new { error = true, message = "Username needs to be more than 3 characters." });
            }

            
        }

        private void removeCurrentSites(string id)
        {
            List<SiteUser> lstUserToRemove = new List<SiteUser>();
            IEnumerable<SiteUser> lstU = UnitOfWork.ISiteUserRepository.GetAll();
            foreach (SiteUser item in lstU)
            {
                if(item.AspNetUsersId.Equals(id))
                {
                    lstUserToRemove.Add(item);
                }
            }
            //After we get the list of siteuser to remove we delete them
            foreach(SiteUser item in lstUserToRemove) {
                UnitOfWork.ISiteUserRepository.Delete(item);
           }
            UnitOfWork.Commit();
        }

        //Checks for e-mail address and returns value depending if the e-mail address is valid or not
        private bool validateEmail(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool passwordsEqual(string password, string confirmPassword)
        {
            return password.Equals(confirmPassword);
        }

        private bool validUsername(string userName)
        {
            return userName.Length > 3;
        }

        private bool userRegisterd(string userName)
        {
            ApplicationDbContext dbr = new ApplicationDbContext();
            ApplicationUser u = dbr.Users.Where(r => r.UserName.Equals(userName)).FirstOrDefault();
            return u != null;
        }

        //
        // POST: /Account/Disassociate
        [Log]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        [Log]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [Log]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [Log]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [Log]
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [Log]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        [Log]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [Log]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [Log]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [Log]
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }
        [Log]
        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        //Receives a user id and an action (deactivate, activate) and performs the action
        public ActionResult changeStatusUser(int id, string action)
        {
            try {
                ApplicationDbContext idb = new ApplicationDbContext();
                ApplicationUser u = idb.Users.Find(id);
                if (action.ToUpper().Equals("DEACTIVATE"))
                    u.Active = false;
                if (action.ToUpper().Equals("ACTIVATE"))
                    u.Active = true;

                idb.SaveChanges();
                idb.Dispose();
                return Json(new
                {
                    error = false,
                    message = "User " + u.UserName + " has been " + action
                });
            }
            catch
            {
                return Json(new
                {
                    error = true,
                    message = "Error " + action + " user"
                });
            }
        }

        //Return a list of all users in a view
        public ActionResult AdministerUsers(AdministerUsersViewModel auvm) {
            AdministerUsersViewModel avm = new AdministerUsersViewModel();
            if (auvm.idSelectedRole > 0)
            {
                avm.changeSelectedRole(auvm.idSelectedRole.ToString());
            }
            return View(avm);
        }


        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewData["Site"] = new SelectList(SitesLists(), "Value", "Text");
            return View(new EditUserViewModel(id));
        }

        [HttpPost]
        public ActionResult makeUserChanges(List<string> selectedRoles, List<string> currentRoles, string email, string userid,string usersite)
        {
            try
            {
                //EditUserViewModel.saveChanges(selectedRoles, currentRoles, email,userid,usersite);
                PPI.Core.Domain.Entities.SiteUser SiteUser = new PPI.Core.Domain.Entities.SiteUser();
                removeCurrentSites(userid); //Remove user site if the user is already in one, users can only have 1 user site
                //After removing the last user site we can add new ones in
                SiteUser.AspNetUsersId = userid;
                SiteUser.SiteId = Convert.ToInt32(usersite);
                UnitOfWork.ISiteUserRepository.Add(SiteUser);
                UnitOfWork.Commit();
                return Json(new { error = false });
            }
            catch
            {
                return Json(new { error = true });
            }
        }


        //Receives user Id and actino through ajax and changes the status of the user depending on the data sent
        [HttpPost]
        public ActionResult changeStatus(string userId, string action)
        {
            if (userId == null || action == null)
                return Json(new
                {
                    error = true
                });
            else
            {
                try
                {
                    ApplicationDbContext dbr = new ApplicationDbContext();
                    ApplicationUser u = dbr.Users.Where(m => m.Id.Equals(userId)).FirstOrDefault();
                    if (action.ToUpper().Equals("ACTIVATE"))
                        u.Active = true;
                    else if (action.ToUpper().Equals("DEACTIVATE"))
                        u.Active = false;
                    dbr.SaveChanges();
                    dbr.Dispose();
                    return Json(new { error = false });
                }
                catch
                {
                    return Json(new { error = true });
                }
            }
        }


        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            PPI.Core.Web.Infrastructure.Utility.ClearCookies();
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Administration");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}