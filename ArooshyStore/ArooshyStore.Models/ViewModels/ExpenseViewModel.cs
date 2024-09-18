using System;

namespace ArooshyStore.Models.ViewModels
{
    public class ExpenseViewModel
    {
        public int ExpenseId { get; set; }
        public string ExpenseName { get; set; }
        public int? ExpenseTypeId { get; set; }
        public string TypeName { get; set; }
        public string ExpenseTypeName { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public string PaidTo { get; set; }
        public string PaidFrom { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
