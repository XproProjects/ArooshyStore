using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblExpense
    {
        [Key]
        public int ExpenseId { get; set; }
        [StringLength(200)]
        public string ExpenseName { get; set; }
        public Nullable<int> ExpenseTypeId { get; set; }
        public Nullable<System.DateTime> ExpenseDate { get; set; }
        public Nullable<decimal> ExpenseAmount { get; set; }
        [StringLength(200)]
        public string PaidTo { get; set; }
        [StringLength(200)]
        public string PaidFrom { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
