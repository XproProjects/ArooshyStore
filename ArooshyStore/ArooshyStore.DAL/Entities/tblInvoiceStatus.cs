using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblInvoiceStatus
    {
        [Key]
        public int InvoiceStatusId { get; set; }
        [StringLength(100)]
        public string InvoiceNumber { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
