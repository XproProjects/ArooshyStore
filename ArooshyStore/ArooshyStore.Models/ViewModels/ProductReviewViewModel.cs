using System;

namespace ArooshyStore.Models.ViewModels
{
    public class ProductReviewViewModel
    {
        public int ReviewId { get; set; }
        public int ReviewByCustomerId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string ReviewByName { get; set; }
        public string ReviewByEmail { get; set; }
        public string ReviewDetail { get; set; }
        public string ImagePath { get; set; }
        public int DocumentId { get; set; }
        public int? InfoId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
