using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class CustomerLoginViewModel
    {
        public int CustomerSupplierId { get; set; }
        public string Email { get; set; }
        public string HouseNo { get; set; }
        public string Password { get; set; }
        public int IsChangePassword { get; set; }
        public int? InfoId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}