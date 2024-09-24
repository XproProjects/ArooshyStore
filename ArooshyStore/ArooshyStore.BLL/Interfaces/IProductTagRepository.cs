using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IProductTagRepository : IDisposable
    {
        List<ProductTagViewModel> GetProductTagsListAndCount(string whereCondition, string start, string length, string sorting);
        ProductTagViewModel GetProductTagById(int id);
        StatusMessageViewModel InsertUpdateProductTag(ProductTagViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteProductTag(int id, int loggedInUserId);
    }
}
