using System;
namespace ArooshyStore.Models.ViewModels
{
    public class DiscountOfferViewModel
    {
        public int OfferId { get; set; }
        public int OfferDetailId { get; set; }
        public string DiscountName { get; set; }
        public DateTime? ExpiredOn { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountRate { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string ArticleNumber { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
        public bool? Status { get; set; }
        public string StatusString { get; set; }
        public string ImagePath { get; set; }
        public int DocumentId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
