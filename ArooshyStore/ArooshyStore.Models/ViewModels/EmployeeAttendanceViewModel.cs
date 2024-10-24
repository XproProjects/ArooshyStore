using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArooshyStore.Models.ViewModels
{
    public class EmployeeAttendanceViewModel
    {
        public int AttendanceId { get; set; }
        public DateTime? AttendanceDate { get; set; }
        public string Attendance { get; set; }
        public TimeSpan? CheckInTime { get; set; }
        public TimeSpan? CheckOutTime { get; set; }
        public int? Minutes { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
        public DateTime? InTimeDateTime { get; set; }
        public DateTime? OutTimeDateTime { get; set; }
        public List<EmployeeAttendanceViewModel> DateList { get; set; }
        public int TotalPresents { get; set; }
        public int TotalAbsents { get; set; }
        public int TotalLeaves { get; set; }
        public int TotalHolidays { get; set; }
        public int WorkingDays { get; set; }
    }
}
