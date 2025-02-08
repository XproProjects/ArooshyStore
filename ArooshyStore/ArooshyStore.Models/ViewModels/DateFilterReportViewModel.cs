using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class DateFilterReportViewModel
    {
        public string FilterType { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? ChildCategoryId { get; set; }
        public int? ProductId { get; set; }
        public int? ProductAttributeDetailBarcodeId { get; set; }
        public DateTime? MonthDate { get; set; }
        public DateTime? ByDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string InvoiceType { get; set; }
        public string Status { get; set; }
        public List<InvoiceDetailViewModel> CategoriesList { get; set; }
        public List<InvoiceDetailViewModel> ProductsList { get; set; }
        public List<InvoiceViewModel> InvoicesList { get; set; }
        public List<InvoiceViewModel> InvoiceStatusesList { get; set; }
        public List<InvoiceDetailViewModel> InvoiceDetailsList { get; set; }
    }
}
