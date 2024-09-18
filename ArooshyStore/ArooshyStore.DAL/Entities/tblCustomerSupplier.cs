using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblCustomerSupplier
    {
        [Key]
        public int CustomerSupplierId { get; set; }
      
        [StringLength(200)]
        public string CustomerSupplierName { get; set; }
        [StringLength(50)]
        public string CustomerSupplierType { get; set; }
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
        [StringLength(1000)]
        public string CompleteAddress { get; set; }
        public Nullable<int> CreditDays { get; set; }
        public Nullable<decimal> CreditLimit { get; set; }
        [StringLength(1000)]
        public string Remarks { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
