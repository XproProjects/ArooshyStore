using System;

namespace ArooshyStore.Models.ViewModels
{
    public class ProductCartViewModel
    {
        public int CartId { get; set; }
        public string CookieName { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public int? DiscountId { get; set; }
        public int? UserId { get; set; }
        public decimal ActualSalePrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GivenSalePrice { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImagePath { get; set; }
        public int DocumentId { get; set; }
        public int? InfoId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
