using System;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.DAL.Entities
{
    public class tblLogging
    {
        [Key]
        public int LogId { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public Nullable<int> LogByUserId { get; set; }
        [StringLength(20)]
        public string LogType { get; set; }
        [StringLength(20)]
        public string FormType { get; set; }
        public int? FormId { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
    }
}
