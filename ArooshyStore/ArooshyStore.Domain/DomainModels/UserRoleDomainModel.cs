using System;

namespace ArooshyStore.Domain.DomainModels
{
    public class UserRoleDomainModel
    {
        public int UserRoleId { get; set; }
        public int? UserId { get; set; }
        public int? ModuleId { get; set; }
        public int? ActionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
