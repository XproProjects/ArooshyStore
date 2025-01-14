using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArooshyStore.DAL.Entities
{
    public class tblInvoiceLog
    {
        [Key]
        public int LogId { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public int? LogByUserId { get; set; }
        [StringLength(50)]
        public string LogType { get; set; }
        [StringLength(50)]
        public string InvoiceNumber { get; set; }
        public Nullable<int> CustomerSupplierId { get; set; }
        [StringLength(100)]
        public string InvoiceType { get; set; }
        public Nullable<System.DateTime> InvoiceDate { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        [StringLength(5)]
        public string DiscType { get; set; }
        public Nullable<decimal> DiscRate { get; set; }
        public Nullable<decimal> DiscAmount { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
        public Nullable<decimal> DeliveryCharges { get; set; }
        [StringLength(50)]
        public string CashOrCredit { get; set; }
    }
}
