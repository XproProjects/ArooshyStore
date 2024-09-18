using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IModuleRepository : IDisposable
    {
        List<ModuleViewModel> GetModulesListAndCount(string whereCondition, string start, string length, string sorting);
        ModuleViewModel GetModuleById(int id);
        StatusMessageViewModel InsertUpdateModule(ModuleViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteModule(int id, int loggedInUserId);
    }
}
