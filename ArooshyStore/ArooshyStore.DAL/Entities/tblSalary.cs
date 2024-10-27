using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArooshyStore.DAL.Entities
{
    public class tblSalary
    {
        [Key]
        public int SalaryId { get; set; }
        [Column(TypeName = "Date")]
        public Nullable<System.DateTime> ForMonth { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<decimal> BasicSalary { get; set; }
        public Nullable<int> TotalPresent { get; set; }
        public Nullable<int> TotalAbsent { get; set; }
        public Nullable<int> TotalLeave { get; set; }
        public Nullable<int> TotalPaidLeave { get; set; }
        public Nullable<int> TotalUnpaidLeave { get; set; }
        public Nullable<int> TotalWorkingDays { get; set; }
        public Nullable<decimal> GrossSalary { get; set; }
        public Nullable<decimal> AdvanceSalary { get; set; }
        public Nullable<decimal> Loan { get; set; }
        public Nullable<decimal> NetSalary { get; set; }
        [StringLength(1000)]
        public string Remarks { get; set; }
        public Nullable<bool> IsPaid { get; set; }
        public Nullable<decimal> PaidAmount { get; set; }
        public Nullable<decimal> BonusAmount { get; set; }
        public Nullable<decimal> DeductionAmount { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
