namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Column_Status_tblCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblCategory", "Status", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblCategory", "Status");
        }
    }
}
