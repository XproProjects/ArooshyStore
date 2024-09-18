using System;

namespace ArooshyStore.Domain.DomainModels
{
    public class AssignActionDomainModel
    {
        public int AssignId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
