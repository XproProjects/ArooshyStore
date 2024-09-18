using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArooshyStore.DAL.Entities
{
    public class tblAttributeDetail
    {
        [Key]
        public int AttributeDetailId { get; set; }
        [StringLength(200)]
        public string AttributeDetailName { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<int>  AttributeId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
