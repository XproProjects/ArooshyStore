using System;

namespace ArooshyStore.Domain.DomainModels
{
    public class DocumentDomainModel
    {
        public int DocumentId { get; set; }
        public DateTime? Date { get; set; }
        public string TypeId { get; set; }
        public string DocumentExtension { get; set; }
        public string DocumentType { get; set; }
        public string Remarks { get; set; }
        public int? UserId { get; set; }
        public string ImagePath { get; set; }
        public bool? Status { get; set; }
        public string Message { get; set; }
    }
}
