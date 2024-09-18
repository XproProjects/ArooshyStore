using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblSecurityGroupDetail
    {
        [Key]
        public int GroupDetalId { get; set; }
        public Nullable<int> GroupId { get; set; }
        public Nullable<int> ModuleId { get; set; }
        public Nullable<int> ActionId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
