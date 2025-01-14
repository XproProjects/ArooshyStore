using System;
using System.ComponentModel.DataAnnotations;


namespace ArooshyStore.DAL.Entities
{
    public class tblDiscountOffer
    {
        [Key]
        public int OfferId { get; set; }
        [StringLength(300)]
        public string DiscountName { get; set; }
        public Nullable<System.DateTime> ExpiredOn { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
