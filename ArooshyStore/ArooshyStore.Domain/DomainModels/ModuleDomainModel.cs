using System;
using System.Collections.Generic;

namespace ArooshyStore.Domain.DomainModels
{
    public class ModuleDomainModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int AssignId { get; set; }
        public int UserId { get; set; }
        public int UserRoleId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public List<ModuleDomainModel> ModuleList { get; set; }
        public List<ModuleDomainModel> ActionList { get; set; }
    }
}
