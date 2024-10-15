using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblProductCart
    {
        [Key]
        public int CartId  { get; set; }
        [StringLength(200)]
        public string CookieName { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<int> DiscountId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<decimal> ActualSalePrice { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> GivenSalePrice { get; set; }

        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
