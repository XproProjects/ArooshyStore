namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Stock_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblProductStock",
                c => new
                    {
                        ProductStockId = c.Int(nullable: false, identity: true),
                        StockType = c.String(maxLength: 50),
                        ProductAttributeDetailBarcodeId = c.Int(),
                        InQty = c.Int(),
                        OutQty = c.Int(),
                        ReferenceId = c.String(maxLength: 50),
                        WarehouseId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ProductStockId);
            
            CreateTable(
                "dbo.tblProductStockLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        ProductStockId = c.Int(),
                        StockType = c.String(maxLength: 50),
                        ProductAttributeDetailBarcodeId = c.Int(),
                        InQty = c.Int(),
                        OutQty = c.Int(),
                        ReferenceId = c.String(maxLength: 50),
                        WarehouseId = c.Int(),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblProductStockLog");
            DropTable("dbo.tblProductStock");
        }
    }
}
