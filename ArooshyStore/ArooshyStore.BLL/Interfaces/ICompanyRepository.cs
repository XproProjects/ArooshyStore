using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ICompanyRepository : IDisposable
    {
        List<CompanyViewModel> GetCompanysListAndCount(string whereCondition, string start, string length, string sorting);
        CompanyViewModel GetCompanyById(int id);
        StatusMessageViewModel InsertUpdateCompany(CompanyViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteCompany(int id, int loggedInUserId);
        CompanyViewModel GetFooterDataForCompany();
    }
}
