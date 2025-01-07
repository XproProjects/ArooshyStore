using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArooshyStore.DAL.Entities
{
    public class tblProductAttributeDetailLog
    {
        [Key]
        public int LogId { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public int? LogByUserId { get; set; }
        [StringLength(50)]
        public string LogType { get; set; }
        public int? ProductAttributeDetailId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> AttributeId { get; set; }
        public Nullable<int> AttributeDetailId { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}
