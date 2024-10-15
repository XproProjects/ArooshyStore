using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblProductCartAttributeDetail
    {
        [Key]
        public int ProductCartAttributeDetailId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> CartId { get; set; }
        public Nullable<int> AttributeDetailId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
