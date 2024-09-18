using System;

namespace ArooshyStore.Domain.DomainModels
{
    public class UserTypeDomainModel
    {
        public int UserTypeId { get; set; }
        public string TypeName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
