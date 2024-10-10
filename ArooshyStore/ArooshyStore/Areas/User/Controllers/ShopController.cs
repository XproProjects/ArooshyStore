using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using Azure;
using Microsoft.AspNet.Identity;
using Microsoft.Build.Framework.XamlTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

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
            var products = _repository.GetProductsForShop();
            return PartialView("GetProductsGrid", products);
        }
        public ActionResult ProductFilters()
        {
            var model = _repository.GetFiltersList(); 
            return PartialView(model);
        }
        public ActionResult SetSearchString(bool? categoryCheckbox, int[] category,
                                    bool? attributeCheckbox, int[] attribute,
                                    bool? discountCheckbox, int[] discount,
                                    decimal? minPrice, decimal? maxPrice)
        {
            var searchString = new List<string>();

            if (categoryCheckbox == true && category != null && category.Length > 0)
            {
                string condition = string.Join(",", category.Select(p => p.ToString()));
                searchString.Add($"isnull(s.CategoryId, 0) in ({condition})");
            }

            if (attributeCheckbox == true && attribute != null && attribute.Length > 0)
            {
                string condition = string.Join(",", attribute.Select(p => p.ToString()));
                searchString.Add($"isnull(s.AttributeId, 0) in ({condition})");
            }

            if (discountCheckbox == true && discount != null && discount.Length > 0)
            {
                string condition = string.Join(",", discount.Select(p => p.ToString()));
                searchString.Add($"isnull(s.OfferId, 0) in ({condition})");
            }

            if (minPrice.HasValue)
            {
                searchString.Add($"s.Price >= {minPrice.Value}");
            }

            if (maxPrice.HasValue)
            {
                searchString.Add($"s.Price <= {maxPrice.Value}");
            }

            string finalSearchString = string.Join(" AND ", searchString);

            var filteredProducts = _repository.GetFilteredProducts(finalSearchString);

            return PartialView("GetProductsGrid", filteredProducts);
        }


    }
}