using ArooshyStore.BLL.EmailService;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;
using ArooshyStore.BLL.BusinessInfo;
using System.Collections.Generic;
using ArooshyStore.Authentication.FormsAuthentication;
using ArooshyStore.Domain.DomainModels;
using System.Web.Script.Serialization;
using ArooshyStore.Areas.Admin.Authentication.Identity;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace ArooshyStore.Areas.User.Controllers
{
    public class CustomerAccountController : BaseController
    {
        private readonly ICustomerSupplierRepository _repository;
        private readonly IProductWishlistRepository _wishlist;
        private readonly IProductCartRepository _cart;


        public CustomerAccountController(ICustomerSupplierRepository repository, IProductWishlistRepository wishlist, IProductCartRepository cart)
        {
            _repository = repository;
            _wishlist = wishlist;
            _cart = cart;
        }
        public ActionResult Register()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Register(CustomerSupplierViewModel user)
        {
            StatusMessageViewModel response;
            response = _repository.InsertUpdateCustomer(user);
            return Json(new { status = response.Status, message = response.Message, Id = response.Id });
        }

        public ActionResult Login()
        {
            if (User != null)
            {
                return RedirectToAction("AccountDashboard", new { id = User.UserId });

            }
            else
            {
                return View();

            }

        }
        [HttpPost]
        public ActionResult Login(CustomerSupplierViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customerSupplier = _repository.GetCustomerSupplierByEmailAndPassword(model);

                if (customerSupplier != null)
                {
                    FormsAutenticationSettings(customerSupplier);
                    return RedirectToAction("AccountDashboard", new { id = customerSupplier.UserId });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            ClearCookies();
            return RedirectToAction("Login", "CustomerAccount");
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
        public ActionResult AccountDashboard(int id)
        {
            var customerSupplier = _repository.GetCustomerById(id);
            return View(customerSupplier);
        }
        public ActionResult AccountDetail(int id)
        {
            var customerSupplier = _repository.GetCustomerById(id);
            return View(customerSupplier);
        }

        public ActionResult CustomerAddress(int id)
        {
            var customerSupplier = _repository.GetCustomerById(id);
            return View(customerSupplier);
        }


        #region Forgot Password
        public ActionResult ForgotPassword()
        {
            return View();

        }
        [HttpPost]
        public ActionResult ForgotPassword(string ForgotEmail)
        {
            StatusMessageViewModel response = _repository.ForgotPassword(ForgotEmail); // Use the correct email parameter
            return Json(new { status = response.Status, message = response.Message });
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            try
            {
                int userId = Convert.ToInt32(id.Split('|')[0]);
                DateTime datetime = Convert.ToDateTime(id.Split('|')[1].ToString().Replace('_', ' '));
                DateTime datetimeNow = DateTime.Now;

                if (DateTime.Compare(datetime, datetimeNow) >= 0)
                {
                    CustomerSupplierViewModel user = new CustomerSupplierViewModel { CustomerSupplierId = userId };
                    return View(user);
                }
                else
                {
                    return RedirectToAction("linkexpired", "customeraccount");
                }
            }
            catch
            {
                return RedirectToAction("linkexpired", "customeraccount");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(int userId, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                bool status = await _repository.ResetPassword(userId, password);
                string message = status ? "Success! Password Reset Successfully." : "Failed to Reset Password.";
                return Json(new { status, message });
            }
            return Json(new { status = false, message = "Password is required." });
        }


        public ActionResult LinkExpired()
        {
            return View();
        }
        #endregion
        public ActionResult ChangePassword()
        {
            if (User != null)
            {
                var model = new CustomerSupplierViewModel
                {
                    CustomerSupplierId = User.UserId
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "customeraccount");
            }
        }
        [HttpPost]
        public async Task<ActionResult> ChangePassword(string password)
        {
            bool result = false;

            if (User != null && !string.IsNullOrEmpty(password))
            {
                result = await _repository.ChangePassword(User.UserId, password);
            }

            if (result)
            {
                return Json(new { status = true });
            }
            else
            {
                return Json(new { status = false, message = "Failed to change the password." });
            }
        }

        #region Wishlist items

        [HttpPost]
        public ActionResult InsertUpdateWishlist(ProductWishlistViewModel user)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();

            if (User != null)
            {
                response = _wishlist.InsertUpdateProductWishlist(user, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }
        public ActionResult Wishlist()
        {
            if (User.Identity.IsAuthenticated)
            {
                int userId = User.UserId; 
                System.Diagnostics.Debug.WriteLine($"User ID: {userId}"); 
                var model = _wishlist.GetWishlistItemsByUserId(userId);
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "CustomerAccount");
            }
        }
        [HttpPost]
        public ActionResult DeleteWishlistProduct(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                ProductWishlistViewModel user = new ProductWishlistViewModel();
                user.WishlistId = id;
                response = _wishlist.DeleteProductWishlist(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }
        #endregion

        #region Cart items
        [HttpPost]
        public ActionResult InsertUpdateCart(ProductCartViewModel user, string data)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            response = _cart.InsertUpdateProductCart(user, data);
           return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }
        [HttpGet]
        public ActionResult CartItems(int userId)
        {
            var model = _cart.GetCartItemsByUserId(userId);
            return View(model);
        }


        [HttpPost]
        public ActionResult DeleteCartProduct(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                ProductCartViewModel user = new ProductCartViewModel();
                user.CartId = id;
                response = _cart.DeleteProductCart(id);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }
        #endregion
    }

}

   