using System;

namespace ArooshyStore.Models.ViewModels
{
    public class ProductCartAttributeDetailViewModel
    {
        public int ProductCartAttributeDetailId { get; set; }
        public int? ProductId { get; set; }
        public int? CartId { get; set; }
        public int? AttributeDetailId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
