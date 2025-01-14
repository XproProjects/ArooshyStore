namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Columns_tblInvoice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblInvoice", "CashOrCredit", c => c.String(maxLength: 50));
            AddColumn("dbo.tblInvoiceDetail", "ProductAttributeDetailBarcodeId", c => c.Int());
            AddColumn("dbo.tblInvoiceDetail", "OfferDetailId", c => c.Int());
            DropColumn("dbo.tblInvoiceDetail", "MasterCategoryId");
            DropColumn("dbo.tblInvoiceDetail", "ChildCategoryId");
            DropColumn("dbo.tblInvoiceDetail", "AttributeId");
            DropColumn("dbo.tblInvoiceDetail", "AttributeDetailId");
            DropColumn("dbo.tblInvoiceDetail", "UnitId");
            DropColumn("dbo.tblInvoiceDetail", "DiscountOfferId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tblInvoiceDetail", "DiscountOfferId", c => c.Int());
            AddColumn("dbo.tblInvoiceDetail", "UnitId", c => c.Int());
            AddColumn("dbo.tblInvoiceDetail", "AttributeDetailId", c => c.Int());
            AddColumn("dbo.tblInvoiceDetail", "AttributeId", c => c.Int());
            AddColumn("dbo.tblInvoiceDetail", "ChildCategoryId", c => c.Int());
            AddColumn("dbo.tblInvoiceDetail", "MasterCategoryId", c => c.Int());
            DropColumn("dbo.tblInvoiceDetail", "OfferDetailId");
            DropColumn("dbo.tblInvoiceDetail", "ProductAttributeDetailBarcodeId");
            DropColumn("dbo.tblInvoice", "CashOrCredit");
        }
    }
}
