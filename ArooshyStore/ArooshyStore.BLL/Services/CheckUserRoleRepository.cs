using System;
using System.Collections.Generic;
using System.Linq;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Services
{
    public class CheckUserRoleRepository : ICheckUserRoleRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CheckUserRoleRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region Check Module Role
        public int CheckModuleRoleId(int? userId, string moduleName)
        {
            int checkId = (from r in _unitOfWork.Db.Set<tblUserRole>()
                           join m in _unitOfWork.Db.Set<tblModule>() on r.ModuleId equals m.ModuleId
                           where m.ModuleName.ToLower() == moduleName.ToLower() && r.UserId == userId
                           select r.UserRoleId).FirstOrDefault();
            return checkId;
        }
        #endregion
        #region Check Action Role
        public int CheckActionRoleId(int? userId, string moduleName, string actionName)
        {
            int checkId = (from r in _unitOfWork.Db.Set<tblUserRole>()
                           join a in _unitOfWork.Db.Set<tblAction>() on r.ActionId equals a.ActionId
                           join m in _unitOfWork.Db.Set<tblModule>() on r.ModuleId equals m.ModuleId
                           where r.UserId == userId && m.ModuleName.ToLower() == moduleName.ToLower() && a.ActionName.ToLower() == actionName.ToLower()
                           select r.UserRoleId).FirstOrDefault();
            return checkId;
        }
        #endregion
        #region Check Action Role With Form
        public int CheckActionRoleIdForForm(int? userId, string moduleName, string actionName, string formName, int modelId)
        {
            int formBranchId = 0;
            int checkId = 0;
            checkId = (from r in _unitOfWork.Db.Set<tblUserRole>()
                       join a in _unitOfWork.Db.Set<tblAction>() on r.ActionId equals a.ActionId
                       join m in _unitOfWork.Db.Set<tblModule>() on r.ModuleId equals m.ModuleId
                       where r.UserId == userId && m.ModuleName.ToLower() == moduleName.ToLower() && a.ActionName.ToLower() == actionName.ToLower()
                       select r.UserRoleId).FirstOrDefault();
            if (modelId > 0 && checkId > 0)
            {
                //if (formName.ToLower() == "employee")
                //{
                //    formBranchId = _unitOfWork.Db.Set<tblPR_Employee>().Where(x => x.EmployeeId == modelId).Select(x => x.BranchId).FirstOrDefault() ?? 0;
                //}
                //else if (formName.ToLower() == "ArooshyStore")
                //{
                //    formBranchId = _unitOfWork.Db.Set<tblPR_Salary>().Where(x => x.SalaryId == modelId).Select(x => x.BranchId).FirstOrDefault() ?? 0;
                //}
                //else if (formName.ToLower() == "td 59")
                //{
                //    formBranchId = _unitOfWork.Db.Set<tblPR_TD59>().Where(x => x.FormId == modelId).Select(x => x.BranchId).FirstOrDefault() ?? 0;
                //}
                //else if (formName.ToLower() == "td 63")
                //{
                //    formBranchId = _unitOfWork.Db.Set<tblPR_TD63>().Where(x => x.FormId == modelId).Select(x => x.BranchId).FirstOrDefault() ?? 0;
                //}
                //else
                //{
                    checkId = 0;
                //}
            }
            return checkId;
        }
        #endregion
        #region Actions List allowed to User
        public List<ModuleViewModel> ActionsList(int? userId, string moduleName)
        {
            List<ModuleViewModel> actions = new List<ModuleViewModel>();
            actions = (from r in _unitOfWork.Db.Set<tblUserRole>()
                       join a in _unitOfWork.Db.Set<tblAction>() on r.ActionId equals a.ActionId
                       join m in _unitOfWork.Db.Set<tblModule>() on r.ModuleId equals m.ModuleId
                       where r.UserId == userId && m.ModuleName.ToLower() == moduleName.ToLower()
                       orderby m.ModuleName
                       select new ModuleViewModel
                       {
                           ActionId = a.ActionId,
                           ActionName = a.ActionName,
                       }).ToList();
            return actions;
        }
        #endregion
        #region Get Users Modules List
        public List<ModuleViewModel> ModulesList(int? userId)
        {
            List<ModuleViewModel> modules = new List<ModuleViewModel>();
            modules = (from r in _unitOfWork.Db.Set<tblUserRole>()
                       join m in _unitOfWork.Db.Set<tblModule>() on r.ModuleId equals m.ModuleId
                       where r.UserId == userId
                       orderby m.ModuleName
                       select new ModuleViewModel
                       {
                           ModuleId = m.ModuleId,
                           ModuleName = m.ModuleName,
                       }).ToList();
            return modules;
        }
        #endregion
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
