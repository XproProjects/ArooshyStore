using System;
using System.Collections.Generic;

namespace ArooshyStore.Models.ViewModels
{
    public class SalaryViewModel
    {
        public int SalaryId { get; set; }
        public DateTime? ForMonth { get; set; }
        public int? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeAddress { get; set; }
        public DateTime? EmployeeDateOfJoining { get; set; }
        public string ImagePath { get; set; }
        public decimal? BasicSalary { get; set; }
        public int? TotalPresent { get; set; }
        public int? TotalAbsent { get; set; }
        public int? TotalLeave { get; set; }
        public int? TotalPaidLeave { get; set; }
        public int? TotalUnpaidLeave { get; set; }
        public int? TotalWorkingDays { get; set; }
        public decimal? GrossSalary { get; set; }
        public decimal? AdvanceSalary { get; set; }
        public decimal? BonusAmount { get; set; }
        public decimal? DeductionAmount { get; set; }
        public decimal? Loan { get; set; }
        public decimal? NetSalary { get; set; }
        public string Remarks { get; set; }
        public bool? IsPaid { get; set; }
        public string IsPaidString { get; set; }
        public decimal? PaidAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
        public decimal TotalNetSalary { get; set; }

    }
}
