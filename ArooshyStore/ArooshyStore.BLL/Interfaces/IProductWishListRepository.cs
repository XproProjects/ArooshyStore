using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IProductWishlistRepository : IDisposable
    {
        List<ProductWishlistViewModel> GetWishlistItemsByUserId(int userId);
        StatusMessageViewModel InsertUpdateProductWishlist(ProductWishlistViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteProductWishlist(int id, int loggedInUserId);
    }
}
