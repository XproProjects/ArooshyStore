using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ICategoryRepository : IDisposable
    {
        List<CategoryViewModel> GetCategoriesListAndCount(string whereCondition, string start, string length, string sorting);
        CategoryViewModel GetCategoryById(int id);
        StatusMessageViewModel InsertUpdateCategory(CategoryViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteCategory(int id, int loggedInUserId);
        List<CategoryViewModel> GetBrowseCategories();
        List<CategoryViewModel> GetMasterCategories();
        List<HeaderViewModel> GetCategoriesForHeader();
    }
}
