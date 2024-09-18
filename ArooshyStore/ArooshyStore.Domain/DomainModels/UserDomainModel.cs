using System;

namespace ArooshyStore.Domain.DomainModels
{
    public class UserDomainModel
    {
        public int UserId { get; set; }
        public int? InfoId { get; set; }
        public int? UserTypeId { get; set; }
        public string TypeName { get; set; }
        public string Type { get; set; }
        public string Password { get; set; }
        public bool? IsActive { get; set; }
        public string FullName { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Email { get; set; }
        public string Cnic { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
