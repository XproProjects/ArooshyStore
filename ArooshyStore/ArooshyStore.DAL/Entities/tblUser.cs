using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblUser
    {
        [Key]
        public int UserId { get; set; }
        public Nullable<int> InfoId { get; set; }
        public Nullable<int> UserTypeId { get; set; }
        [StringLength(200)]
        public string Password { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
