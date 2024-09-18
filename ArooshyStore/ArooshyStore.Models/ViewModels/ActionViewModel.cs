using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArooshyStore.Models.ViewModels
{
    public class ActionViewModel
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
    }
}