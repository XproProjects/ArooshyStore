using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IEmployeeAttendanceRepository : IDisposable
    {
        List<EmployeeAttendanceViewModel> GetEmployeesForAttendance(DateTime attendanceDate);
        StatusMessageViewModel InsertUpdateEmployeeAttendance(string data, int loggedInUserId);
        List<EmployeeAttendanceViewModel> GetAttendanceDetail(DateTime attendateDate, int loggedInUserId);
    }
}
