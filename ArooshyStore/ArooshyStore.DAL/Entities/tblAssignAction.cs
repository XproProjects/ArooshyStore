using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblAssignAction
    {
        [Key]
        public int AssignId { get; set; }
        public Nullable<int> ModuleId { get; set; }
        public Nullable<int> ActionId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
