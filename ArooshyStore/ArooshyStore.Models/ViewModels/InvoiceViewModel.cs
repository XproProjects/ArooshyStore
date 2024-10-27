using System;
using System.Collections.Generic;

namespace ArooshyStore.Models.ViewModels
{
    public class InvoiceViewModel
    {
        public string InvoiceNumber { get; set; }
        public string AttributeDetailName { get; set; }
        public int? CustomerSupplierId { get; set; }
        public string InvoiceType { get; set; }
        public string Status { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string DiscType { get; set; }
        public decimal? DiscRate { get; set; }
        public decimal? DiscAmount { get; set; }
        public decimal? NetAmount { get; set; }
        public decimal? DeliveryCharges { get; set; }
        public string IsNewOrEdit { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Qty { get; set; }
        public int? DiscountOfferId { get; set; }
        public int? MasterCategoryId { get; set; }
        public int? ChildCategoryId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string DiscountName { get; set; }
        public string AttributeName { get; set; }
        public string CustomerName { get; set; }
        public int? AttributeId { get; set; }
        public int? AttributeDetailId { get; set; }
        public string CompanyContact { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyEmail { get; set; }

        public List<InvoiceDetailViewModel> InvoiceDetailsList { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}