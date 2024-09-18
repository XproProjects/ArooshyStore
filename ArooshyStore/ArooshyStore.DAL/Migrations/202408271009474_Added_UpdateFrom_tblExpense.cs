namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_UpdateFrom_tblExpense : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblExpense", "PaidFrom", c => c.String(maxLength: 200));
            DropColumn("dbo.tblExpense", "PaidFrpm");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tblExpense", "PaidFrpm", c => c.String(maxLength: 200));
            DropColumn("dbo.tblExpense", "PaidFrom");
        }
    }
}
