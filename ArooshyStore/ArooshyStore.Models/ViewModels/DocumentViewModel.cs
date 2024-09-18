using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArooshyStore.Models.ViewModels
{
    public class DocumentViewModel
    {
        public int DocumentId { get; set; }
        public string TypeId { get; set; }
        public string Remarks { get; set; }
        public DateTime? Date { get; set; }
        public string DocumentExtension { get; set; }
        public string DocumentType { get; set; }
        public string ImagePath { get; set; }
    }
}