namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblExpense : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblExpense",
                c => new
                    {
                        ExpenseId = c.Int(nullable: false, identity: true),
                        ExpenseName = c.String(maxLength: 200),
                        ExpenseTypeId = c.Int(),
                        ExpenseDate = c.DateTime(),
                        ExpenseAmount = c.Decimal(precision: 18, scale: 2),
                        PaidTo = c.String(maxLength: 200),
                        PaidFrpm = c.String(maxLength: 200),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ExpenseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblExpense");
        }
    }
}
