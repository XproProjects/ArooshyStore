namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Column_Status_tblDiscountOffer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblDiscountOffer", "Status", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblDiscountOffer", "Status");
        }
    }
}
