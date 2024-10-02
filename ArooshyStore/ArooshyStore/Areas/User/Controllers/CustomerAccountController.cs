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

namespace ArooshyStore.Areas.User.Controllers
{
    public class CustomerAccountController : BaseController
    {
        private readonly ICustomerSupplierRepository _repository;


        public CustomerAccountController(ICustomerSupplierRepository repository)
        {
            _repository = repository;
        }
        public ActionResult Register()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Register(CustomerSupplierViewModel user)
        {
            StatusMessageViewModel response;

            if (User != null)
            {
                response = _repository.InsertUpdateCustomer(user);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }

            return Json(new { status = response.Status, message = response.Message, Id = response.Id });
        }

        public ActionResult Login()
        {
            if(User!=null)
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
                return View();
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
    }

}

   