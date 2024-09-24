namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblDeliveryCharges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblDeliveryCharges",
                c => new
                    {
                        DeliveryId = c.Int(nullable: false, identity: true),
                        CityId = c.Int(nullable: false),
                        DeliveryCharges = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DeliveryId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblDeliveryCharges");
        }
    }
}
