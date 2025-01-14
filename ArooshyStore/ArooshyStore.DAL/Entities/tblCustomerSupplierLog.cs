using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArooshyStore.DAL.Entities
{
    public class tblCustomerSupplierLog
    {
        [Key]
        public int LogId { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public int? LogByUserId { get; set; }
        [StringLength(50)]
        public string LogType { get; set; }
        public Nullable<int> CustomerSupplierId { get; set; }
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
        [StringLength(200)]
        public string Password { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}
