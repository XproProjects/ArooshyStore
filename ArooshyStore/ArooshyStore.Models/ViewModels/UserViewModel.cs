using System;

namespace ArooshyStore.Models.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public int? InfoId { get; set; }
        public int? UserTypeId { get; set; }
        public string Password { get; set; }
        public bool? IsActive { get; set; }
        public string StatusString { get; set; }
        public int IsChangePassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Email { get; set; }
        public string Cnic { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string TypeName { get; set; }
        public string UserIdWithTypeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ImagePath { get; set; }
        public int DocumentId { get; set; }
        public int TotalRecords { get; set; }
    }
}