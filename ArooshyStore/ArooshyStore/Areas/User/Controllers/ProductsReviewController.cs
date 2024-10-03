using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using Azure;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ArooshyStore.Areas.User.Controllers
{
    public class ProductsReviewController : BaseController
    {
        private readonly IProductReviewRepository _repository;


        public ProductsReviewController(IProductReviewRepository repository)
        {
            _repository = repository;
        }
        public ActionResult Index(int productId)
        {
            var model = new ProductReviewViewModel
            {
                ProductId = productId
            };

            if (User != null)
            {
                var userId = User.UserId;
                model.ReviewByCustomerId = userId;
                model.ReviewByName = User.Name; 
                model.ReviewByEmail = User.Email; 
            }

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Index(ProductReviewViewModel user)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            var reviews = _repository.InsertUpdateProductReview(user);
            if (reviews != null) 
            {
                return new JsonResult { Data = new { status = response.Status, message = response.Message } };
            }
            else
            {
                return Json(new { status = false, message = "An error occurred while saving the review." });
            }
        }

        public ActionResult ProductReviews(int productId)
        {
            var reviews = _repository.GetProductReviews(productId);

            return PartialView(reviews);
        }



    }
}