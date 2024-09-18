using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class ErrorLogViewModel
    {
        public int ErrorId { get; set; }
        public string ErrorLineNumber { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorClass { get; set; }
        public string ErrorAction { get; set; }
        public string ErrorSource { get; set; }
       
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
