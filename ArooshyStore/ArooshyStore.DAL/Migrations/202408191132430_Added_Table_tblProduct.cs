namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblProduct : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblProduct",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        ProductName = c.String(maxLength: 200),
                        ProductNameUrdu = c.String(maxLength: 200),
                        Barcode = c.String(maxLength: 50),
                        UnitId = c.Int(),
                        CategoryId = c.Int(),
                        CostPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SalePrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblProduct");
        }
    }
}
