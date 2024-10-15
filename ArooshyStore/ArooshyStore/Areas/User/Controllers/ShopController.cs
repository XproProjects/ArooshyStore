using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using Azure;
using Microsoft.AspNet.Identity;
using Microsoft.Build.Framework.XamlTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;

namespace ArooshyStore.Areas.User.Controllers
{
    public class ShopController : BaseController
    {
        private readonly IProductRepository _repository;
        public ShopController(IProductRepository repository)
        {
            _repository = repository;
        }
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult GetProductsList()
        {
            var shopProducts = _repository.GetProductsForShop();
            return PartialView("GetProductsList", shopProducts);
        }
        public ActionResult GetProductsGrid()
        {
            if (User != null)
            {
                var loggedInUserId = User.UserId;
                ViewBag.UserId = loggedInUserId;
            }
            var products = _repository.GetProductsForShop();
            return PartialView("GetProductsGrid", products);
        }
        public ActionResult ProductFilters()
        {
            var model = _repository.GetFiltersList(); 
            return PartialView(model);
        }
        public ActionResult SetSearchString(bool? categoryCheckbox, int[] category,bool? attributeCheckbox, int[] attribute,
            bool? discountCheckbox, int[] discount, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var filteredProducts = _repository.GetFilteredProducts(categoryCheckbox, category,attributeCheckbox, attribute, discountCheckbox, discount, minPrice, maxPrice, sortBy);
            return PartialView("GetProductsGrid", filteredProducts);
        }

    }
}