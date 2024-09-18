namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblExpenseType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblExpenseType",
                c => new
                    {
                        ExpenseTypeId = c.Int(nullable: false, identity: true),
                        TypeName = c.String(maxLength: 200),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ExpenseTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblExpenseType");
        }
    }
}
