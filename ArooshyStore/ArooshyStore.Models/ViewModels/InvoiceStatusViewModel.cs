﻿using System;

namespace ArooshyStore.Models.ViewModels
{
    public class InvoiceStatusViewModel
    {
        public int InvoiceStatusId { get; set; }
        public string InvoiceNumber { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}