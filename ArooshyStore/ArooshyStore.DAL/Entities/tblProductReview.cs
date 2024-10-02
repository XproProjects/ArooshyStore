using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblProductReview
    {
        [Key]
        public int ReviewId { get; set; }
        public int ReviewByCustomerId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }

        [StringLength(200)]
        public string ReviewByName { get; set; }
        [StringLength(200)]
        public string ReviewByEmail { get; set; }
        public string ReviewDetail { get; set; }
        
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
