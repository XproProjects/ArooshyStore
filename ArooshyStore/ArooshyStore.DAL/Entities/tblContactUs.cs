using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblContactus
    {
        [Key]
        public int ContactUsId { get; set; }
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(200)]
        public string Subject { get; set; }
        [StringLength(20)]
        public string PhoneNo { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
