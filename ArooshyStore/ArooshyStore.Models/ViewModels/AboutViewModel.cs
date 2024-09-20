using System;

namespace ArooshyStore.Models.ViewModels
{
    public class AboutViewModel
    {
        public int AboutId { get; set; }
        public string Description { get; set; }
        public string Service1Name { get; set; }
        public string Service1Icon { get; set; }
        public string Service1Description { get; set; }
        public string Service2Name { get; set; }
        public string Service2Icon { get; set; }
        public string Service2Description { get; set; }
        public string Service3Name { get; set; }
        public string Service3Icon { get; set; }
        public string Service3Description { get; set; } 
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
