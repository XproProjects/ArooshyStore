using System;

namespace ArooshyStore.Models.ViewModels
{
    public class DeliveryInfoViewModel
    {
        public int DeliveryInfoId { get; set; }
        public string DeliveryInfoName { get; set; }
        public string DeliveryInfoDetail { get; set; }
        public bool? Status { get; set; }
        public string StatusString { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
