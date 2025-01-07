using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArooshyStore.DAL.Entities
{
    public class tblProductStockLog
    {
        [Key]
        public int LogId { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public int? LogByUserId { get; set; }
        [StringLength(50)]
        public string LogType { get; set; }
        public Nullable<int> ProductStockId { get; set; }
        [StringLength(50)]
        public string StockType { get; set; }
        public Nullable<int> ProductAttributeDetailBarcodeId { get; set; }
        public Nullable<int> InQty { get; set; }
        public Nullable<int> OutQty { get; set; }
        [StringLength(50)]
        public string ReferenceId { get; set; }
        public Nullable<int> WarehouseId { get; set; }
    }
}
