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
    public class CheckOutController : BaseController
    {

        private readonly ICustomerSupplierRepository _repository;
        private readonly IProductCartRepository _cart;


        public CheckOutController(ICustomerSupplierRepository repository, IProductCartRepository cart)
        {
            _repository = repository;
            _cart = cart;
        }
        [HttpGet]
        public ActionResult CheckOutDetail()
        {
            var model = new CustomerSupplierViewModel(); 

            if (User != null)
            {
                model.CustomerSupplierId = User.UserId;
                model.CustomerSupplierName = User.Name;  
                model.Email = User.Email; 
            }
           
            return View(model);
        }
        [HttpPost]
        public ActionResult CheckOutDetail(CustomerSupplierViewModel user)
        {
            StatusMessageViewModel response;
            response = _repository.InsertUpdateCustomer(user);
            return Json(new { status = response.Status, message = response.Message, Id = response.Id });
        }
        [HttpGet]
        public ActionResult CheckOutReview(string CookieName, int? UserId)
        {
            if (string.IsNullOrEmpty(CookieName) && !UserId.HasValue)
            {
                return HttpNotFound("User ID or Cookie name is required.");
            }

            var cartItems = _cart.GetCartItemCountByCookieName(UserId, CookieName);

            if (cartItems == null || !cartItems.Any())
            {
                return HttpNotFound("No items found in the cart.");
            }

            return View(cartItems);
        }

        public ActionResult CheckOutComplete()
        {
            return View();
        }
    }
}

