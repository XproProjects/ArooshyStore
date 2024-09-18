using System;

namespace ArooshyStore.Models.ViewModels
{
    public class InvoiceDetailViewModel
    {
        public string InvoiceLineNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public int? WarehouseId { get; set; }
        public int? MasterCategoryId { get; set; }
        public int? ChildCategoryId { get; set; }
        public int? ProductId { get; set; }
        public int? AttributeId { get; set; }
        public int? AttributeDetailId { get; set; }
        public int? UnitId { get; set; }
        public int? DiscountOfferId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public string DiscType { get; set; }
        public decimal? DiscRate { get; set; }
        public decimal? DiscAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
