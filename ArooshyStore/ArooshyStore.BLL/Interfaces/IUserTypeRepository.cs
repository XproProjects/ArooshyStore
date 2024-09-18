using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IUserTypeRepository : IDisposable
    {
        List<UserTypeViewModel> GetUserTypesListAndCount(string whereCondition, string start, string length, string sorting);
        UserTypeViewModel GetUserTypeById(int id);
        StatusMessageViewModel InsertUpdateUserType(UserTypeViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteUserType(int id, int loggedInUserId);
    }
}
