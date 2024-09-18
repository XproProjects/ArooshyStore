using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IUserRepository : IDisposable
    {
        List<UserViewModel> GetUsersListAndCount(string whereCondition, string start, string length, string sorting);
        UserViewModel GetUserById(int id);
        int GetUsers();

        StatusMessageViewModel InsertUpdateUser(UserViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteUser(int id, int loggedInUserId);
        List<ModuleViewModel> GetModulesForAssign(int userId);
        StatusMessageViewModel InsertUpdateAssignModule(int userId, string assignType, string data, int loggedInUserId);
    }
}
