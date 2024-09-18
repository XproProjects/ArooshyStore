using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblDocument
    {
        [Key]
        public int DocumentId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        [StringLength(100)]
        public string TypeId { get; set; }
        [StringLength(20)]
        public string DocumentExtension { get; set; }
        [StringLength(100)]
        public string DocumentType { get; set; }
        [StringLength(100)]
        public string Remarks { get; set; }
        [StringLength(500)]
        public string DocumentUrl { get; set; }
        [StringLength(500)]
        public string TypeOfDocument { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<bool> IsFileLocked { get; set; }
    }
}
