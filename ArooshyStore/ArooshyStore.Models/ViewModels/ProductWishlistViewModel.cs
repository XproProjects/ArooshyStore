using System;

namespace ArooshyStore.Models.ViewModels
{
    public class ProductWishlistViewModel
    {
        public int WishlistId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ProductName { get; set; } 
        public decimal SalePrice { get; set; }  
        public decimal CostPrice { get; set; }
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
