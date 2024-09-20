using System;
using System.ComponentModel.DataAnnotations;


namespace ArooshyStore.DAL.Entities
{
    public class tblAbout
    {

        [Key]
        public int AboutId { get; set; }
        public string Description { get; set; }
        [StringLength(200)]
        public string Service1Name { get; set; }
        [StringLength(20)]
        public string Service1Icon { get; set; }
        [StringLength(500)]
        public string Service1Description { get; set; }
        [StringLength(200)]
        public string Service2Name { get; set; }
        [StringLength(20)]
        public string Service2Icon { get; set; }
        [StringLength(500)]
        public string Service2Description { get; set; }

        [StringLength(200)]
        public string Service3Name { get; set; }
        [StringLength(20)]
        public string Service3Icon { get; set; }
        [StringLength(500)]
        public string Service3Description { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }


    }
}
