using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblProduct
    {
        [Key]
        public int ProductId { get; set; }
      
        [StringLength(200)]
        public string ProductName { get; set; }
        [StringLength(200)]
        public string ProductNameUrdu { get; set; }
        public string ProductDescription { get; set; }

        public Nullable<int> DeliveryInfoId { get; set; }

        public Nullable<int> UnitId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<decimal> SalePriceForWebsite { get; set; }
        public Nullable<decimal> SalePriceAfterExpired { get; set; }
        public Nullable<bool> IsExpired { get; set; }

        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsFeatured { get; set; }

        [StringLength(50)]
        public string ArticleNumber { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
