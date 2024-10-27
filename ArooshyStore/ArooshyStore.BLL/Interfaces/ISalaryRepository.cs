using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ISalaryRepository : IDisposable
    {
        List<SalaryViewModel> GetSalaryListAndCount(string whereCondition, string start, string length, string sorting);
        SalaryViewModel GetSalaryById(int id);
        StatusMessageViewModel InsertUpdateSalary(SalaryViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteSalary(int id, int loggedInUserId);
    }
}
