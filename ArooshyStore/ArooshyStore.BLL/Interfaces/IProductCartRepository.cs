using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IProductCartRepository : IDisposable
    {
        List<ProductCartViewModel> GetCartItemsByUserId(int userId);
        StatusMessageViewModel InsertUpdateProductCart(ProductCartViewModel model, string AttributeDetailData);
        StatusMessageViewModel DeleteProductCart(int id);
    }
}
