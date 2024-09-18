using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblInvoiceDetail
    {
        [Key]
        [StringLength(50)]
        public string InvoiceLineNumber { get; set; }
        [StringLength(50)]
        public string InvoiceNumber { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public Nullable<int> MasterCategoryId { get; set; }
        public Nullable<int> ChildCategoryId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> AttributeId { get; set; }
        public Nullable<int> AttributeDetailId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> DiscountOfferId { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        [StringLength(5)]
        public string DiscType { get; set; }
        public Nullable<decimal> DiscRate { get; set; }
        public Nullable<decimal> DiscAmount { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
