using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IExpenseRepository : IDisposable
    {
        List<ExpenseViewModel> GetExpensesListAndCount(string whereCondition, string start, string length, string sorting);
        ExpenseViewModel GetExpenseById(int id);
        StatusMessageViewModel InsertUpdateExpense(ExpenseViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteExpense(int id, int loggedInUserId);
    }
}
