using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using Azure;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class ProductsReviewController : BaseController
    {
        private readonly IProductReviewRepository _repository;


        public ProductsReviewController(IProductReviewRepository repository)
        {
            _repository = repository;
        }
       
        public ActionResult AllReviews()
        {
            var reviews = _repository.GetAllReviews(); 
            return View(reviews); 
        }


    }
}