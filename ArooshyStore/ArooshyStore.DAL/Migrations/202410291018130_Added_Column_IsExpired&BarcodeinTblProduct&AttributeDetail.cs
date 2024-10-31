namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Column_IsExpiredBarcodeinTblProductAttributeDetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblProduct", "SalePriceForWebsite", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.tblProduct", "SalePriceAfterExpired", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.tblProduct", "IsExpired", c => c.Boolean());
            AddColumn("dbo.tblProductAttributeDetail", "Barcode", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblProductAttributeDetail", "Barcode");
            DropColumn("dbo.tblProduct", "IsExpired");
            DropColumn("dbo.tblProduct", "SalePriceAfterExpired");
            DropColumn("dbo.tblProduct", "SalePriceForWebsite");
        }
    }
}
