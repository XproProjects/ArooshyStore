using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class AttributeViewModel
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public int? AttributeDetailId { get; set; }
        public string AttributeDetailName { get; set; }
        public Nullable<bool> Status { get; set; }
        public string StatusString { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
        public List<ProductAttributeDetailViewModel> AttributeDetails { get; set; }

    }
}
