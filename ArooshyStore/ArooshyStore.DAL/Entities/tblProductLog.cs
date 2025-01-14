using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArooshyStore.DAL.Entities
{
    public class tblProductLog
    {
        [Key]
        public int LogId { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public int? LogByUserId { get; set; }
        [StringLength(50)]
        public string LogType { get; set; }
        public int? ProductId { get; set; }
        [StringLength(200)]
        public string ProductName { get; set; }
        [StringLength(200)]
        public string ProductNameUrdu { get; set; }
        public string ProductDescription { get; set; }
        public Nullable<int> DeliveryInfoId { get; set; }
        public Nullable<int> UnitId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public Nullable<decimal> SalePriceForWebsite { get; set; }
        public Nullable<decimal> SalePriceAfterExpired { get; set; }
        public Nullable<bool> IsExpired { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<bool> IsFeatured { get; set; }
        public Nullable<bool> ShowOnWebsite { get; set; }
        [StringLength(50)]
        public string ArticleNumber { get; set; }
    }
}
