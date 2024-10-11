using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IProductReviewRepository : IDisposable
    {
        List<ProductReviewViewModel> GetProductReviewsListAndCount(string whereCondition, string start, string length, string sorting);
        ProductReviewViewModel GetProductReviewById(int id);
        StatusMessageViewModel InsertUpdateProductReview(ProductReviewViewModel model);
        StatusMessageViewModel DeleteProductReview(int id, int loggedInUserId);
        List<ProductReviewViewModel> GetProductReviews(int productId);

        List<ProductReviewViewModel> GetAllReviews();


    }
}
