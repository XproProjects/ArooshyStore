using System;
using System.ComponentModel.DataAnnotations;


namespace ArooshyStore.DAL.Entities
{
    public class tblCompany
    {

        [Key]
        public int CompanyId { get; set; }
        [StringLength(200)]
        public string CompanyName { get; set; }
        [StringLength(50)]
        public string Contact1 { get; set; }
        [StringLength(50)]
        public string Contact2 { get; set; }
        [StringLength(200)]
        public string Email { get; set; }
        [StringLength(1000)]
        public string FacebookId { get; set; }
        [StringLength(1000)]
        public string InstagramId { get; set; }
        [StringLength(1000)]
        public string LinkedInId { get; set; }
        public string Address { get; set; }
        [StringLength(200)]
        public string Longitude { get; set; }
        [StringLength(200)]
        public string Latitude { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }


    }
}
