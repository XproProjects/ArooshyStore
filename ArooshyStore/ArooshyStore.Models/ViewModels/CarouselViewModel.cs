using System;
using System.Collections.Generic;

namespace ArooshyStore.Models.ViewModels
{
    public class CarouselViewModel
    {
        public int CarouselId { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string BackgroundColor { get; set; }
        public bool? Status { get; set; }
        public string StatusString { get; set; }
        public string ImagePath { get; set; }
        public int DocumentId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
        public List<CarouselViewModel> Carousels { get; set; }
    }
}
