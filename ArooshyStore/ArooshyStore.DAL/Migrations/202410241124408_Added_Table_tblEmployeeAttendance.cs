namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblEmployeeAttendance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblEmployeeAttendance",
                c => new
                    {
                        AttendanceId = c.Int(nullable: false, identity: true),
                        AttendanceDate = c.DateTime(storeType: "date"),
                        Attendance = c.String(maxLength: 2),
                        CheckInTime = c.Time(precision: 7),
                        CheckOutTime = c.Time(precision: 7),
                        Minutes = c.Int(),
                        EmployeeId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.AttendanceId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblEmployeeAttendance");
        }
    }
}
