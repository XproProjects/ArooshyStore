using System;

namespace ArooshyStore.Domain.DomainModels
{
    public class ErrorsLogDomainModel
    {
        public int ErrorId { get; set; }
        public string ErrorLineNumber { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorSource { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
