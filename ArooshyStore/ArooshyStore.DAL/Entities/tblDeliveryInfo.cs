using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblDeliveryInfo
    {
        [Key]
        public int DeliveryInfoId { get; set; }
        [StringLength(200)]
        public string DeliveryInfoName { get; set; }
        public string DeliveryInfoDetail { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
