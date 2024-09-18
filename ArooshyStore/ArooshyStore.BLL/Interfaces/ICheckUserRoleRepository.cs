using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ICheckUserRoleRepository : IDisposable
    {
        int CheckModuleRoleId(int? userId, string moduleName);

        int CheckActionRoleId(int? userId, string moduleName, string actionName);
        int CheckActionRoleIdForForm(int? userId, string moduleName, string actionName, string formName, int modelId);
        List<ModuleViewModel> ActionsList(int? userId, string moduleName);
        List<ModuleViewModel> ModulesList(int? userId);
    }
}
