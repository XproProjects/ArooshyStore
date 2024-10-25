using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IProductCartRepository : IDisposable
    {
        List<ProductCartViewModel> GetCartItemCountByCookieName(int? UserId,string CookieName);
        List<ProductCartViewModel> GetLatestCartItemsByCookieName(string userIdOrCookieName);
        StatusMessageViewModel InsertUpdateProductCart(ProductCartViewModel model, string AttributeDetailData);
        StatusMessageViewModel DeleteProductCart(int id);
         int GetCartItemCount(string cookieName);

    }
}
