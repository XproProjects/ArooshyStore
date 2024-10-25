using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArooshyStore.DAL.Entities
{
    public class tblEmployeeAttendance
    {
        [Key]
        public int AttendanceId { get; set; }
        [Column(TypeName = "Date")]
        public Nullable<System.DateTime> AttendanceDate { get; set; }
        [StringLength(2)]
        public string Attendance { get; set; }
        public Nullable<TimeSpan> CheckInTime { get; set; }
        public Nullable<TimeSpan> CheckOutTime { get; set; }
        public Nullable<int> Minutes { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public Nullable<int> UpdatedBy { get; set; }
    }
}
