using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblProductAttributeDetailBarcode
    {
        [Key]
        public int ProductAttributeDetailBarcodeId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> AttributeId1 { get; set; }
        public Nullable<int> AttributeDetailId1 { get; set; }
        public Nullable<int> AttributeId2 { get; set; }
        public Nullable<int> AttributeDetailId2 { get; set; }
        [StringLength(50)]
        public string Barcode { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
