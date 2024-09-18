using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ArooshyStore.BLL.EmailService;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.PasswordEncrypt;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Domain.DomainModels;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private string siteUrl = ConfigurationManager.AppSettings["SiteUrl"].ToString();
        private string linkExpiredTime = ConfigurationManager.AppSettings["LinkExpiredTime"].ToString();
        public AccountRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<UserDomainModel> GetUserByUsernameAndPassword(UserDomainModel model)
        {
            UserDomainModel user = null;
            try
            {
                string hashPassword = PasswordEncryptService.GetHash(model.Password);
                user = await _unitOfWork.Db.Set<tblUser>()
                                .Join(_unitOfWork.Db.Set<tblInfo>(), u => u.InfoId, i => i.InfoId, (u, i) => new { u, i })
                                .Join(_unitOfWork.Db.Set<tblUserType>(), ut => ut.u.UserTypeId, t => t.UserTypeId, (ut, t) => new { ut, t })
                                .Where(x => x.ut.i.Email.ToLower() == model.Email.ToLower().Trim() && x.ut.u.Password == hashPassword && x.ut.u.IsActive == true)
                                .Select(m => new UserDomainModel
                                {
                                    UserId = m.ut.u.UserId,
                                    FullName = m.ut.i.FullName,
                                    Email = m.ut.i.Email,
                                    TypeName = m.t.TypeName
                                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "AccountRepository", "GetUserByUsernameAndPassword");
            }
            return user;
        }
        public string GetUserImagePath(int userId, string type)
        {
            string path = "";
            try
            {
                path = _unitOfWork.Db.Set<tblDocument>().Where(x => x.TypeId == userId.ToString() && x.DocumentType == type).Select(x => "/Areas/Admin/FormsDocuments/User/" + x.DocumentId + "." + x.DocumentExtension).FirstOrDefault() ?? "/Areas/Admin/Content/dummy.png";
            }
            catch (Exception ex)
            {
                path = "";
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "AccountRepository", "GetUserImagePath");
            }
            return path;
        }

        public string GetUserName(int userId)
        {
            return (from u in _unitOfWork.Db.Set<tblUser>() join i in _unitOfWork.Db.Set<tblInfo>() on u.InfoId equals i.InfoId where u.UserId == userId select i.FullName).FirstOrDefault() ?? "";
        }

        public string GetUserEmail(int userId)
        {
            return (from u in _unitOfWork.Db.Set<tblUser>() join i in _unitOfWork.Db.Set<tblInfo>() on u.InfoId equals i.InfoId where u.UserId == userId select i.Email).FirstOrDefault() ?? "";
        }
        public string GetUserNameAndEmail(int userId)
        {
            return (from u in _unitOfWork.Db.Set<tblUser>() join i in _unitOfWork.Db.Set<tblInfo>() on u.InfoId equals i.InfoId where u.UserId == userId select i.FullName + "|" + i.Email).FirstOrDefault() ?? "";
        }
        public async Task<bool> CheckOldPassword(int userId, string oldPassword)
        {
            bool status = false;
            try
            {
                string hashPassword = PasswordEncryptService.GetHash(oldPassword);
                status = await _unitOfWork.Db.Set<tblUser>().Where(x => x.UserId == userId).AnyAsync(x => x.Password == hashPassword);
            }
            catch (Exception ex)
            {
                status = false;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "AccountRepository", "CheckOldPassword");
            }
            return status;
        }
        public async Task<bool> ChangePassword(int userId, string password)
        {
            bool status = false;
            try
            {
                var user = await _unitOfWork.Db.Set<tblUser>().Where(x => x.UserId == userId).FirstOrDefaultAsync();
                string hashPassword = PasswordEncryptService.GetHash(password);
                user.Password = hashPassword;
                await _unitOfWork.Db.SaveChangesAsync();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "AccountRepository", "ChangePassword");
            }
            return status;
        }
        public async Task<UserDomainModel> GetUserByUserId(int userId, int branchId)
        {
            UserDomainModel user = null;
            try
            {
                user = await _unitOfWork.Db.Set<tblUser>()
                                .Join(_unitOfWork.Db.Set<tblInfo>(), u => u.InfoId, i => i.InfoId, (u, i) => new { u, i })
                                .Join(_unitOfWork.Db.Set<tblUserType>(), ut => ut.u.UserTypeId, t => t.UserTypeId, (ut, t) => new { ut, t })
                                .Where(x => x.ut.u.UserId == userId)
                                .Select(m => new UserDomainModel
                                {
                                    UserId = m.ut.u.UserId,
                                    FullName = m.ut.i.FullName,
                                    Email = m.ut.i.Email,
                                    TypeName = m.t.TypeName
                                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "AccountRepository", "GetUserByUserId");
            }
            return user;
        }
        public StatusMessageViewModel ForgotPassword(string email)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            var userInfo = (from u in _unitOfWork.Db.Set<tblUser>()
                            join i in _unitOfWork.Db.Set<tblInfo>() on u.InfoId equals i.InfoId
                            where i.Email.ToLower().Trim() == email.ToLower().Trim()
                            && u.IsActive == true
                            select new UserDomainModel
                            {
                                UserId = u.UserId,
                                FullName = i.FullName,
                                Email = i.Email,
                            }).FirstOrDefault();
            if (userInfo != null)
            {
                #region Send Email
                string subject = "ArooshyStore Forgot Password";
                string body = "";

                Random random = new Random();
                int randomNum = random.Next(1, 10000);

                DateTime dateTime = new DateTime();
                TimeSpan minutes = TimeSpan.FromMinutes(Convert.ToInt32(linkExpiredTime));
                dateTime = DateTime.Now.Add(minutes);

                var url = siteUrl + "/admin/account/resetpassword/?id=" + userInfo.UserId + "|" + dateTime.ToString().Replace(' ', '_') + "|" + randomNum;
                body += "<span>Hi " + userInfo.FullName + "</span><br />" +
                    "<span> Please click on the link below to reset your password.</span><br /><br />" +
                    "" + url + "<br /><br />" +
                    "<span><b>Regards:<b></span><br />" +
                    "<span>ArooshyStore</span>";

                string toEmailAddress = userInfo.Email;
                string message = EmailServiceRepository.SendEmailString(toEmailAddress, subject, body);
                if (message == "Success")
                {
                    response.Status = true;
                    response.Message = "Success! A Password Reset Link has been sent to your Email.";
                }
                else
                {
                    response.Status = false;
                    response.Message = message;
                }
                #endregion
            }
            else
            {
                response.Status = false;
                response.Message = "Oops! Email does not exist in our database.";
            }
            return response;
        }
        public async Task<bool> ResetPassword(int userId, string password)
        {
            bool status = false;
            try
            {
                var user = await _unitOfWork.Db.Set<tblUser>().Where(x => x.UserId == userId).FirstOrDefaultAsync();
                string hashPassword = PasswordEncryptService.GetHash(password);
                user.Password = hashPassword;
                await _unitOfWork.Db.SaveChangesAsync();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "AccountRepository", "ResetPassword");
            }
            return status;
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //public void Dispose()
        //{
        //}
    }
}
