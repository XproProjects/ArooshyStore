using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IExpenseTypeRepository : IDisposable
    {
        List<ExpenseTypeViewModel> GetExpenseTypesListAndCount(string whereCondition, string start, string length, string sorting);
        ExpenseTypeViewModel GetExpenseTypeById(int id);
        StatusMessageViewModel InsertUpdateExpenseType(ExpenseTypeViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteExpenseType(int id, int loggedInUserId);
    }
}
