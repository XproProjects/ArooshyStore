using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class DateFilterReportViewModel
    {
        public decimal YearlyNetSalary;

        public string FilterType { get; set; }
        public DateTime? MonthDate { get; set; }
        public DateTime? ByDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PayrollYear { get; set; }
        public string FromUserType { get; set; }
        public int[] CompanyId { get; set; }
        public int[] BranchId { get; set; }
        public int[] EmployeeId { get; set; }
        public string BranchName { get; set; }
        public string ExpenseStatus { get; set; }
        public List<EmployeeAttendanceViewModel> AttendancesList { get; set; }
        public List<EmployeeViewModel> EmploymentsList { get; set; }
    }
}
