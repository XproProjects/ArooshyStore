using System;

namespace ArooshyStore.Models.ViewModels
{
    public class DeliveryChargesViewModel
    {
        public int DeliveryId { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }

        public decimal DeliveryCharges { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
