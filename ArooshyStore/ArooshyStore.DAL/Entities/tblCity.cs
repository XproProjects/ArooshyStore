using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblCity
    {
        [Key]
        public int CityId { get; set; }
        [StringLength(200)]
        public string CityName { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
