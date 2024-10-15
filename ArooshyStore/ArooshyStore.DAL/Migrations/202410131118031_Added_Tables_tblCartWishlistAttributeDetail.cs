namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Tables_tblCartWishlistAttributeDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblProductCart",
                c => new
                    {
                        CartId = c.Int(nullable: false, identity: true),
                        CookieName = c.String(maxLength: 200),
                        ProductId = c.Int(),
                        Quantity = c.Int(),
                        DiscountId = c.Int(),
                        UserId = c.Int(),
                        ActualSalePrice = c.Decimal(precision: 18, scale: 2),
                        DiscountAmount = c.Decimal(precision: 18, scale: 2),
                        GivenSalePrice = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.CartId);
            
            CreateTable(
                "dbo.tblProductCartAttributeDetail",
                c => new
                    {
                        ProductCartAttributeDetailId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(),
                        CartId = c.Int(),
                        AttributeDetailId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ProductCartAttributeDetailId);
            
            CreateTable(
                "dbo.tblProductWishlist",
                c => new
                    {
                        WishlistId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        ProductId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.WishlistId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblProductWishlist");
            DropTable("dbo.tblProductCartAttributeDetail");
            DropTable("dbo.tblProductCart");
        }
    }
}
