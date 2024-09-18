using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblErrorsLog
    {
        [Key]
        public int ErrorId { get; set; }
        [StringLength(50)]
        public string ErrorLineNumber { get; set; }
        [StringLength(500)]
        public string ErrorDescription { get; set; }
        [StringLength(100)]
        public string ErrorSource { get; set; }
        [StringLength(200)]
        public string ErrorClass { get; set; }
        [StringLength(200)]
        public string ErrorAction { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
