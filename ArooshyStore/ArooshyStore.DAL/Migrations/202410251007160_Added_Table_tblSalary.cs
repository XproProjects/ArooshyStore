namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblSalary : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblSalary",
                c => new
                    {
                        SalaryId = c.Int(nullable: false, identity: true),
                        ForMonth = c.DateTime(storeType: "date"),
                        EmployeeId = c.Int(),
                        BasicSalary = c.Decimal(precision: 18, scale: 2),
                        TotalPresent = c.Int(),
                        TotalAbsent = c.Int(),
                        TotalLeave = c.Int(),
                        TotalPaidLeave = c.Int(),
                        TotalUnpaidLeave = c.Int(),
                        TotalWorkingDays = c.Int(),
                        GrossSalary = c.Decimal(precision: 18, scale: 2),
                        AdvanceSalary = c.Decimal(precision: 18, scale: 2),
                        Loan = c.Decimal(precision: 18, scale: 2),
                        NetSalary = c.Decimal(precision: 18, scale: 2),
                        Remarks = c.String(maxLength: 1000),
                        IsPaid = c.Boolean(),
                        PaidAmount = c.Decimal(precision: 18, scale: 2),
                        BonusAmount = c.Decimal(precision: 18, scale: 2),
                        DeductionAmount = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.SalaryId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblSalary");
        }
    }
}
