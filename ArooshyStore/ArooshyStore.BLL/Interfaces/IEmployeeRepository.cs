using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IEmployeeRepository : IDisposable
    {
        List<EmployeeViewModel> GetEmployeesListAndCount(string whereCondition, string start, string length, string sorting);
        EmployeeViewModel GetEmployeeById(int id);
        StatusMessageViewModel InsertUpdateEmployee(EmployeeViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteEmployee(int id, int loggedInUserId);
    }
}
