using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IActionRepository : IDisposable
    {
        List<ActionViewModel> GetActionsListAndCount(string whereCondition, string start, string length, string sorting);
        ActionViewModel GetActionById(int id);
        StatusMessageViewModel InsertUpdateAction(ActionViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteAction(int id, int loggedInUserId);
        List<ModuleViewModel> GetActionsListForAssign(int id);
        StatusMessageViewModel InsertUpdateAssign(int moduleId, string data, int loggedInUserId);
    }
}
