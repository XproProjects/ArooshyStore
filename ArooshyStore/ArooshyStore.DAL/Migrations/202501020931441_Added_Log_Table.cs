namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Log_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblAttributeDetailLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        AttributeDetailId = c.Int(),
                        AttributeDetailName = c.String(maxLength: 200),
                        Status = c.Boolean(),
                        AttributeId = c.Int(),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.tblProductAttributeDetailBarcodeLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        ProductAttributeDetailBarcodeId = c.Int(),
                        ProductId = c.Int(),
                        AttributeId1 = c.Int(),
                        AttributeDetailId1 = c.Int(),
                        AttributeId2 = c.Int(),
                        AttributeDetailId2 = c.Int(),
                        Barcode = c.String(maxLength: 50),
                        Status = c.Boolean(),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.tblProductAttributeDetailLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        ProductAttributeDetailId = c.Int(),
                        ProductId = c.Int(),
                        AttributeId = c.Int(),
                        AttributeDetailId = c.Int(),
                        Status = c.Boolean(),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.tblProductLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        ProductId = c.Int(),
                        ProductName = c.String(maxLength: 200),
                        ProductNameUrdu = c.String(maxLength: 200),
                        ProductDescription = c.String(),
                        DeliveryInfoId = c.Int(),
                        UnitId = c.Int(),
                        CategoryId = c.Int(),
                        CostPrice = c.Decimal(precision: 18, scale: 2),
                        SalePrice = c.Decimal(precision: 18, scale: 2),
                        SalePriceForWebsite = c.Decimal(precision: 18, scale: 2),
                        SalePriceAfterExpired = c.Decimal(precision: 18, scale: 2),
                        IsExpired = c.Boolean(),
                        Status = c.Boolean(),
                        IsFeatured = c.Boolean(),
                        ArticleNumber = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblProductLog");
            DropTable("dbo.tblProductAttributeDetailLog");
            DropTable("dbo.tblProductAttributeDetailBarcodeLog");
            DropTable("dbo.tblAttributeDetailLog");
        }
    }
}
