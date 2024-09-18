using System;

namespace ArooshyStore.Domain.DomainModels
{
    public class ActionDomainModel
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
