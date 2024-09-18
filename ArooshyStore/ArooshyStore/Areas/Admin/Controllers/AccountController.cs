using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using ArooshyStore.App_Start;
using ArooshyStore.Authentication.FormsAuthentication;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.Domain.DomainModels;
using ArooshyStore.Models.ViewModels;
using AutoMapper;

namespace ArooshyStore.Areas.Admin.Controllers
{
    ///[Authorize]
    public class AccountController : BaseController
    {
        private readonly IAccountRepository _repository;
        private readonly IMapper _mapper;
        public AccountController(IAccountRepository repository)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;

        }
        // GET: Account
        [AllowAnonymous]
        public ActionResult Login()
        {
            #region Set Business Detail
            //Log.Debug("login");
            BusinessInfo business = BusinessInfo.GetInstance;
            List<string> businessString = business.GetBusinessInfo();
            ViewBag.BusinessName = businessString[0];
            ViewBag.BusinessLogo = businessString[1];
            //ViewBag.BannerImage = businessString[2];
            ViewBag.BannerImage = businessString[2];
            ViewBag.BgColor = businessString[3];
            #endregion
            #region Expired Preious Cookies
            ClearCookies();
            #endregion
            return View("Login5");
        }
        [HttpPost]
        public async Task<ActionResult> Login(UserViewModel login)
        {


            //This will pass object of UserViewModel to UserDomainModel model object.
            UserDomainModel dest = _mapper.Map<UserViewModel, UserDomainModel>(login);
            bool status = false;
            int userId = 0;
            string message = "";
            UserDomainModel user = await _repository.GetUserByUsernameAndPassword(dest);
            if (user != null)
            {
                status = true;
                userId = user.UserId;
                FormsAutenticationSettings(user);
            }
            else
            {
                status = false;
                message = "Oops !! Incorrect Username or Password.";
            }
            return new JsonResult { Data = new { status = status, message = message, userId = userId } };
        }
        [AllowAnonymous]
        public ActionResult Logout()
        {
            ClearCookies();
            return RedirectToAction("login", "account");
        }
        public void ClearCookies()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            // clear authentication cookie
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            authCookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Set(authCookie);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Set(cookie2);
        }
        public void FormsAutenticationSettings(UserDomainModel user)
        {
            CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
            serializeModel.UserId = user.UserId;
            serializeModel.Name = user.FullName;
            serializeModel.Email = user.Email;
            serializeModel.UserType = user.TypeName;
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string userData = serializer.Serialize(serializeModel);
            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                     1,
                     FormsAuthentication.FormsCookieName,
                     DateTime.Now,
                     DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                     false,
                     userData,
                     FormsAuthentication.FormsCookiePath);

            string encTicket = FormsAuthentication.Encrypt(authTicket);
            HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            faCookie.Expires = authTicket.Expiration;
            Response.Cookies.Add(faCookie);
        }
        public ActionResult GetUserPicture(int id)
        {
            string imagePath = _repository.GetUserImagePath(id, "User");
            string completePath = Server.MapPath(imagePath);
            byte[] image = System.IO.File.ReadAllBytes(completePath);
            if (image != null)
            {
                return File(image, "image/jpg");
            }
            else
            {
                return null;
            }
        }
        public ActionResult GetUserName()
        {
            string userName = _repository.GetUserName(User.UserId);
            return Content(userName);
        }
        public ActionResult GetUserEmail()
        {
            string email = _repository.GetUserEmail(User.UserId);
            return Content(email);
        }
        public ActionResult GetUserNameAndEmail()
        {
            string detail = _repository.GetUserNameAndEmail(User.UserId);
            return Content(detail);
        }
        #region Forgot Password
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            StatusMessageViewModel response = _repository.ForgotPassword(email);
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }
        public ActionResult ResetPassword(string id)
        {
            try
            {
                #region Set Business Detail
                BusinessInfo business = BusinessInfo.GetInstance;
                List<string> businessString = business.GetBusinessInfo();
                ViewBag.BusinessName = businessString[0];
                ViewBag.BusinessLogo = businessString[1];
                //ViewBag.BannerImage = businessString[2];
                ViewBag.BannerImage = businessString[2];
                ViewBag.BgColor = businessString[3];
                #endregion
                #region Expired Preious Cookies
                ClearCookies();
                #endregion
                int userId = Convert.ToInt32(id.Split('|')[0]);
                DateTime datetime = Convert.ToDateTime(id.Split('|')[1].ToString().Replace('_', ' '));
                DateTime datetimeNow = DateTime.Now;
                int a = DateTime.Compare(datetime, datetimeNow);
                if (DateTime.Compare(datetime, datetimeNow) >= 0)
                {
                    UserViewModel user = new UserViewModel();
                    user.UserId = userId;
                    return View(user);
                }
                else
                {
                    return RedirectToAction("linkexpired", "account");
                }
            }
            catch
            {
                return RedirectToAction("linkexpired", "account");
            }
        }
        [HttpPost]
        public async Task<ActionResult> ResetPassword(int userId, string password)
        {
            bool status = false;
            string message = "";
            if (password != null)
            {
                status = await _repository.ResetPassword(userId, password);
                if (status == true)
                {
                    message = "Success! Password Reset Successfully. Please Click on Login button below to login now with new Password.";
                }
                else
                {
                    message = "Oops! Failed to Reset Password. Please try again.";
                }
            }
            else
            {
                status = false;
                message = "Password is required.";
            }
            return new JsonResult { Data = new { status = status, message = message, userId = userId } };
        }
        public ActionResult LinkExpired()
        {
            return View();
        }
        #endregion
        #region Change Password
        public ActionResult CheckOldPassword()
        {
            if (User != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        public ActionResult CheckOwnerOldPassword()
        {
            if (User != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        [HttpPost]
        public async Task<ActionResult> CheckOldPassword(string password)
        {
            bool result = false;
            if (User != null)
            {
                if (password != null)
                {
                    bool check = await _repository.CheckOldPassword(User.UserId, password);
                    if (check)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChangePassword()
        {
            if (User != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        public ActionResult ChangeOwnerPassword()
        {
            if (User != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string password)
        {
            bool result = false;
            if (User != null)
            {
                if (password != null)
                {
                    result = await _repository.ChangePassword(User.UserId, password);
                }
                else
                {
                    result = false;

                }
            }
            else
            {
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Old Account Controller By Identity
        //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;

        //public AccountController()
        //{

        //}

        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}

        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set 
        //    { 
        //        _signInManager = value; 
        //    }
        //}

        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}

        ////
        //// GET: /Account/Login
        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        ////
        //// POST: /Account/Login
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // This doesn't count login failures towards account lockout
        //    // To enable password failures to trigger account lockout, change to shouldLockout: true
        //    var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid login attempt.");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}

        ////
        //// GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

        //            // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        ////
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        ////
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        ////
        //// POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        ////
        //// POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LogOff()
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    return RedirectToAction("Index", "Home");
        //}

        ////
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_userManager != null)
        //        {
        //            _userManager.Dispose();
        //            _userManager = null;
        //        }

        //        if (_signInManager != null)
        //        {
        //            _signInManager.Dispose();
        //            _signInManager = null;
        //        }
        //    }

        //    base.Dispose(disposing);
        //}

        //#region Helpers
        //// Used for XSRF protection when adding external logins
        //private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        //internal class ChallengeResult : HttpUnauthorizedResult
        //{
        //    public ChallengeResult(string provider, string redirectUri)
        //        : this(provider, redirectUri, null)
        //    {
        //    }

        //    public ChallengeResult(string provider, string redirectUri, string userId)
        //    {
        //        LoginProvider = provider;
        //        RedirectUri = redirectUri;
        //        UserId = userId;
        //    }

        //    public string LoginProvider { get; set; }
        //    public string RedirectUri { get; set; }
        //    public string UserId { get; set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        //        if (UserId != null)
        //        {
        //            properties.Dictionary[XsrfKey] = UserId;
        //        }
        //        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        //    }
        //}
        //#endregion
        #endregion
    }
}