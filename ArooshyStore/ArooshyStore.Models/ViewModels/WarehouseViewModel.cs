﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class WarehouseViewModel
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public string Address { get; set; }
        public string Contact1 { get; set; }
        public string Contact2 { get; set; }
        public string Email { get; set; }
        public Nullable<bool> Status { get; set; }
        public String StatusString { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}
