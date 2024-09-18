using System;
using System.ComponentModel.DataAnnotations;


namespace ArooshyStore.DAL.Entities
{
    public class tblCarousel
    {

        [Key]
        public int CarouselId { get; set; }
        [StringLength(200)]
        public string CarouselName { get; set; }
        [StringLength(500)]
        public string Line1 { get; set; }
        [StringLength(500)]
        public string Line2 { get; set; }
        [StringLength(500)]
        public string Line3 { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }


    }
}
