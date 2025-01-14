using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArooshyStore.DAL.Entities
{
    public class tblDiscountOfferDetail
    {
        [Key]
        public int OfferDetailId { get; set; }
        public Nullable<int> OfferId { get; set; }
        public Nullable<int> ProductId { get; set; }
        [StringLength(10)]
        public string DiscountType { get; set; }
        public Nullable<decimal> DiscountRate { get; set; }
        public Nullable<System.DateTime> ExpiredOn { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
