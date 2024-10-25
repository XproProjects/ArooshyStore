namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Tables_tblEmployeeDesignation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblDesignation",
                c => new
                    {
                        DesignationId = c.Int(nullable: false, identity: true),
                        DesignationName = c.String(maxLength: 200),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DesignationId);
            
            CreateTable(
                "dbo.tblEmployee",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        EmployeeName = c.String(maxLength: 200),
                        Contact1 = c.String(maxLength: 30),
                        Contact2 = c.String(maxLength: 30),
                        Email = c.String(maxLength: 200),
                        HouseNo = c.String(maxLength: 50),
                        Street = c.String(maxLength: 50),
                        ColonyOrVillageName = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 50),
                        CityId = c.Int(),
                        CompleteAddress = c.String(maxLength: 1000),
                        Gender = c.String(maxLength: 20),
                        MaritalStatus = c.String(maxLength: 20),
                        Salary = c.Decimal(precision: 18, scale: 2),
                        DateOfJoining = c.DateTime(),
                        SalaryType = c.String(maxLength: 50),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblEmployee");
            DropTable("dbo.tblDesignation");
        }
    }
}
