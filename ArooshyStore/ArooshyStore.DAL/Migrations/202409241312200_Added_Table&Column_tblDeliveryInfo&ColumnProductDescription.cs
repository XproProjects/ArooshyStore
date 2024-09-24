namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_TableColumn_tblDeliveryInfoColumnProductDescription : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblDeliveryInfo",
                c => new
                    {
                        DeliveryInfoId = c.Int(nullable: false, identity: true),
                        DeliveryInfoName = c.String(maxLength: 200),
                        DeliveryInfoDetail = c.String(),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.DeliveryInfoId);
            
            AddColumn("dbo.tblProduct", "ProductDescription", c => c.String());
            AddColumn("dbo.tblProduct", "DeliveryInfoId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblProduct", "DeliveryInfoId");
            DropColumn("dbo.tblProduct", "ProductDescription");
            DropTable("dbo.tblDeliveryInfo");
        }
    }
}
