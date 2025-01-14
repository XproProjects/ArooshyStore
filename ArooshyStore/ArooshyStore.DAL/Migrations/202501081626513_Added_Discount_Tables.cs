namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Discount_Tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblDiscountOfferDetail",
                c => new
                    {
                        OfferDetailId = c.Int(nullable: false, identity: true),
                        OfferId = c.Int(),
                        ProductId = c.Int(),
                        DiscountType = c.String(maxLength: 10),
                        DiscountRate = c.Decimal(precision: 18, scale: 2),
                        ExpiredOn = c.DateTime(),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.OfferDetailId);
            
            CreateTable(
                "dbo.tblDiscountOfferDetailLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        OfferDetailId = c.Int(),
                        OfferId = c.Int(),
                        ProductId = c.Int(),
                        DiscountType = c.String(maxLength: 10),
                        DiscountRate = c.Decimal(precision: 18, scale: 2),
                        ExpiredOn = c.DateTime(),
                        Status = c.Boolean(),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.tblDiscountOfferLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        OfferId = c.Int(),
                        DiscountName = c.String(maxLength: 300),
                        ExpiredOn = c.DateTime(),
                        Status = c.Boolean(),
                    })
                .PrimaryKey(t => t.LogId);
            
            DropColumn("dbo.tblDiscountOffer", "DiscPercent");
            DropColumn("dbo.tblDiscountOffer", "SelectType");
            DropColumn("dbo.tblDiscountOffer", "CategoryId");
            DropColumn("dbo.tblDiscountOffer", "ProductId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tblDiscountOffer", "ProductId", c => c.Int());
            AddColumn("dbo.tblDiscountOffer", "CategoryId", c => c.Int());
            AddColumn("dbo.tblDiscountOffer", "SelectType", c => c.String(maxLength: 100));
            AddColumn("dbo.tblDiscountOffer", "DiscPercent", c => c.Decimal(precision: 18, scale: 2));
            DropTable("dbo.tblDiscountOfferLog");
            DropTable("dbo.tblDiscountOfferDetailLog");
            DropTable("dbo.tblDiscountOfferDetail");
        }
    }
}
