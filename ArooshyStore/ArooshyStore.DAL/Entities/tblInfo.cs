using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArooshyStore.DAL.Entities
{
    public class tblInfo
    {
        [Key]
        public int InfoId { get; set; }
        [StringLength(250)]
        public string FullName { get; set; }
        [StringLength(20)]
        public string Contact1 { get; set; }
        [StringLength(20)]
        public string Contact2 { get; set; }
        [StringLength(100)]
        public string Email { get; set; }
        [StringLength(30)]
        public string Cnic { get; set; }
        [StringLength(10)]
        public string Gender { get; set; }
        [Column(TypeName = "Date")]
        public Nullable<System.DateTime> DOB { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        [StringLength(20)]
        public string EmergencyContact1 { get; set; }
        [StringLength(20)]
        public string EmergencyContact2 { get; set; }
    }
}
