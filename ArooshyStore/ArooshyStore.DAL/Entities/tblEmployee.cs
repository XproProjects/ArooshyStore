using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArooshyStore.DAL.Entities
{
    public class tblEmployee
    {
        [Key]
        public int EmployeeId { get; set; }
      
        [StringLength(200)]
        public string EmployeeName { get; set; }
        [StringLength(30)]
        public string Contact1 { get; set; }
        [StringLength(30)]
        public string Contact2 { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(50)]
        public string HouseNo { get; set; }
        [StringLength(50)]
        public string Street { get; set; }
        [StringLength(50)]
        public string ColonyOrVillageName { get; set; }
        [StringLength(50)]
        public string PostalCode { get; set; }
        public Nullable<int> CityId { get; set; }
        public Nullable<int> DesignationId { get; set; }

        [StringLength(1000)]
        public string CompleteAddress { get; set; }
        [StringLength(20)]
        public string Gender { get; set; }
        [StringLength(20)]
        public string MaritalStatus { get; set; }
        public Nullable<decimal> Salary { get; set; }
        [Column(TypeName = "Date")]
        public Nullable<System.DateTime> DateOfJoining { get; set; }
        [StringLength(50)]
        public string SalaryType { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
