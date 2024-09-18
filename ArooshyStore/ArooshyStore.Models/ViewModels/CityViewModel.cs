using System;

namespace ArooshyStore.Models.ViewModels
{
    public class CityViewModel
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
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
