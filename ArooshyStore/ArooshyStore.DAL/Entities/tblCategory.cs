using System;
using System.ComponentModel.DataAnnotations;


namespace ArooshyStore.DAL.Entities
{
    public class tblCategory
    {

        [Key]
        public int CategoryId { get; set; }
        [StringLength(200)]
        public string CategoryName { get; set; }
        public Nullable<int> ParentCategoryId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
        public Nullable<bool> Status { get; set; }
    }
}
