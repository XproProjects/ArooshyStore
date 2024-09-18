using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IErrorLogRepository : IDisposable
    {
        List<ErrorLogViewModel> GetErrorLogsListAndCount(string whereCondition, string start, string length, string sorting);
     
    }
}
