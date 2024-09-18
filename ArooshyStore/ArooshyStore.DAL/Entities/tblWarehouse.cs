using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblWarehouse
    {
        [Key]
        public int WarehouseId { get; set; }
        [StringLength(100)]
        public string WarehouseName { get; set; }
        [StringLength(1000)]
        public string Address { get; set; }
        [StringLength(50)]
        public string Contact1 { get; set; }
        [StringLength(50)]
        public string Contact2 { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
