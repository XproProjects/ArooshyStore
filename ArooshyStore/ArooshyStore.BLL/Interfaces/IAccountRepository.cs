using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArooshyStore.Domain.DomainModels;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IAccountRepository : IDisposable
    {
        Task<UserDomainModel> GetUserByUsernameAndPassword(UserDomainModel model);
        string GetUserImagePath(int userId, string type);
        Task<bool> CheckOldPassword(int userId, string oldPassword);
        Task<bool> ChangePassword(int userId, string password);
        Task<UserDomainModel> GetUserByUserId(int userId, int branchId);
        string GetUserName(int userId);
        string GetUserEmail(int userId);
        string GetUserNameAndEmail(int userId);
        StatusMessageViewModel ForgotPassword(string email);
        Task<bool> ResetPassword(int userId, string password);
    }
}
