using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IProductReviewRepository : IDisposable
    {
        StatusMessageViewModel InsertUpdateProductReview(ProductReviewViewModel model);
        List<ProductReviewViewModel> GetProductReviews(int productId);

        List<ProductReviewViewModel> GetAllReviews();


    }
}
