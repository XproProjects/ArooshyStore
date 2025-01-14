using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArooshyStore.DAL.Entities
{
    public class tblInvoiceDetailLog
    {
        [Key]
        public int LogId { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public int? LogByUserId { get; set; }
        [StringLength(50)]
        public string LogType { get; set; }
        [StringLength(50)]
        public string InvoiceLineNumber { get; set; }
        [StringLength(50)]
        public string InvoiceNumber { get; set; }
        public Nullable<int> WarehouseId { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> ProductAttributeDetailBarcodeId { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<int> OfferDetailId { get; set; }
        [StringLength(5)]
        public string DiscType { get; set; }
        public Nullable<decimal> DiscRate { get; set; }
        public Nullable<decimal> DiscAmount { get; set; }
        public Nullable<decimal> NetAmount { get; set; }
    }
}
