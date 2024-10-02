using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class ContactusViewModel
    {
        public int ContactUsId { get; set; }
        public string txtName { get; set; }
        public string txtEmail { get; set; }
        public string txtSubject { get; set; }
        public string txtMessage { get; set; }
        public string txtPhone { get; set; }

        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
