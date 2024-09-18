namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_table_tblInvoice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblInvoice",
                c => new
                    {
                        InvoiceNumber = c.String(nullable: false, maxLength: 50),
                        CustomerSupplierId = c.Int(),
                        InvoiceType = c.String(maxLength: 100),
                        InvoiceDate = c.DateTime(),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        DiscType = c.String(maxLength: 5),
                        DiscRate = c.Decimal(precision: 18, scale: 2),
                        DiscAmount = c.Decimal(precision: 18, scale: 2),
                        NetAmount = c.Decimal(precision: 18, scale: 2),
                        DeliveryCharges = c.Decimal(precision: 18, scale: 2),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.InvoiceNumber);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblInvoice");
        }
    }
}
