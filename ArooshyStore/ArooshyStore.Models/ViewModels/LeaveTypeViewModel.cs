using System;
using System.Collections.Generic;

namespace ArooshyStore.Models.ViewModels
{
    public class LeaveTypeViewModel
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int? AfterJoiningDays { get; set; }
        public string Type { get; set; }
        public bool? IsPaidLeave { get; set; }
        public string IsPaidLeaveString { get; set; }
        public bool? IsWeekoffAsLeave { get; set; }
        public string IsWeekoffAsLeaveString { get; set; }
        public bool? IsHolidayAsLeave { get; set; }
        public string IsHolidayAsLeaveString { get; set; }
        public bool? IsDisplayLeaveBalance { get; set; }
        public string IsDisplayLeaveBalanceString { get; set; }
        public bool? IsHalfPaidLeave { get; set; }
        public string IsHalfPaidLeaveString { get; set; }
        public bool? IsMedicalLeave { get; set; }
        public string IsMedicalLeaveString { get; set; }
        public bool? IsRestrictedToApplyInNoticePeriod { get; set; }
        public string IsRestrictedToApplyInNoticePeriodString { get; set; }
        public decimal? SalaryDeduction { get; set; }
        public bool? Status { get; set; }
        public string StatusString { get; set; }
        public int? BranchId { get; set; }
        public string BranchName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
