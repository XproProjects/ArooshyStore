using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblInvoice
    {
        [Key]
        [StringLength(50)]
        public string InvoiceNumber { get; set; }      
        public Nullable<int> CustomerSupplierId { get; set; }
        [StringLength(100)]
        public string InvoiceType { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        [StringLength(5)]
        public string DiscType { get; set; }
        public Nullable<decimal> DiscRate { get; set; }
        public Nullable<decimal> DiscAmount { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
        public Nullable<decimal> DeliveryCharges { get; set; }
        [StringLength(50)]
        public string CashOrCredit { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
