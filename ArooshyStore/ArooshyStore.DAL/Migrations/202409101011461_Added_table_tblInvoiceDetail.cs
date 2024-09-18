namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_table_tblInvoiceDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblInvoiceDetail",
                c => new
                    {
                        InvoiceLineNumber = c.String(nullable: false, maxLength: 50),
                        InvoiceNumber = c.String(maxLength: 50),
                        WarehouseId = c.Int(),
                        MasterCategoryId = c.Int(),
                        ChildCategoryId = c.Int(),
                        ProductId = c.Int(),
                        AttributeId = c.Int(),
                        AttributeDetailId = c.Int(),
                        UnitId = c.Int(),
                        DiscountOfferId = c.Int(),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        Qty = c.Decimal(precision: 18, scale: 2),
                        Rate = c.Decimal(precision: 18, scale: 2),
                        DiscType = c.String(maxLength: 5),
                        DiscRate = c.Decimal(precision: 18, scale: 2),
                        DiscAmount = c.Decimal(precision: 18, scale: 2),
                        NetAmount = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.InvoiceLineNumber);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblInvoiceDetail");
        }
    }
}
