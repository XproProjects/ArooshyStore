namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Invoice_LogTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblInvoiceDetailLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        InvoiceLineNumber = c.String(maxLength: 50),
                        InvoiceNumber = c.String(maxLength: 50),
                        WarehouseId = c.Int(),
                        ProductId = c.Int(),
                        ProductAttributeDetailBarcodeId = c.Int(),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        Qty = c.Decimal(precision: 18, scale: 2),
                        Rate = c.Decimal(precision: 18, scale: 2),
                        OfferDetailId = c.Int(),
                        DiscType = c.String(maxLength: 5),
                        DiscRate = c.Decimal(precision: 18, scale: 2),
                        DiscAmount = c.Decimal(precision: 18, scale: 2),
                        NetAmount = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.tblInvoiceLog",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 50),
                        InvoiceNumber = c.String(maxLength: 50),
                        CustomerSupplierId = c.Int(),
                        InvoiceType = c.String(maxLength: 100),
                        InvoiceDate = c.DateTime(),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        DiscType = c.String(maxLength: 5),
                        DiscRate = c.Decimal(precision: 18, scale: 2),
                        DiscAmount = c.Decimal(precision: 18, scale: 2),
                        NetAmount = c.Decimal(precision: 18, scale: 2),
                        DeliveryCharges = c.Decimal(precision: 18, scale: 2),
                        CashOrCredit = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblInvoiceLog");
            DropTable("dbo.tblInvoiceDetailLog");
        }
    }
}
