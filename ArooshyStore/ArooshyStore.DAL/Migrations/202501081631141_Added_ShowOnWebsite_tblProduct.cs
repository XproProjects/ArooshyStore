namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_ShowOnWebsite_tblProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblProduct", "ShowOnWebsite", c => c.Boolean());
            AddColumn("dbo.tblProductLog", "ShowOnWebsite", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblProductLog", "ShowOnWebsite");
            DropColumn("dbo.tblProduct", "ShowOnWebsite");
        }
    }
}
