using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class ProductAttributeDetailViewModel
    {
        public int? ProductAttributeDetailId { get; set; }
        public string ProductAttributeDetailName { get; set; }
        public int? ProductAttributeDetailBarcodeId { get; set; }
        public int? ProductId { get; set; }
        public int? AttributeId { get; set; }
        public int? AttributeDetailId { get; set; }
        public bool? Status { get; set; }
        public string StatusString { get; set; }
        public string AttributeDetailName { get; set; }
        public string AttributeName { get; set; }
        public int? AttributeId1 { get; set; }
        public string AttributeName1 { get; set; }
        public int? AttributeId2 { get; set; }
        public string AttributeName2 { get; set; }
        public int? AttributeDetailId1 { get; set; }
        public string AttributeDetailName1 { get; set; }
        public int? AttributeDetailId2 { get; set; }
        public string AttributeDetailName2 { get; set; }
        public string ProductName { get; set; }
        public string ArticleNumber { get; set; }
        public string Barcode { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public string StockType { get; set; }
        public int Stock { get; set; }
        public string ReferenceId { get; set; }
        public int WarehouseId { get; set; }
        public int TotalRecords { get; set; }
        public string AllAttributes { get; set; }
        public string IsChecked { get; set; }
        public List<ProductAttributeDetailViewModel> ProductAttributeDetailList { get; set; }
    }
}
