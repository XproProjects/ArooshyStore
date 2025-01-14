using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArooshyStore.DAL.Entities
{
    public class tblDiscountOfferLog
    {
        [Key]
        public int LogId { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public int? LogByUserId { get; set; }
        [StringLength(50)]
        public string LogType { get; set; }
        public Nullable<int> OfferId { get; set; }
        [StringLength(300)]
        public string DiscountName { get; set; }
        public Nullable<System.DateTime> ExpiredOn { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}
