using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblProductAttributeDetail
    {
        [Key]
        public int ProductAttributeDetailId { get; set; }       
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> AttributeId { get; set; }
        public Nullable<int> AttributeDetailId { get; set; }
        [StringLength(100)]
        public string Barcode { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
