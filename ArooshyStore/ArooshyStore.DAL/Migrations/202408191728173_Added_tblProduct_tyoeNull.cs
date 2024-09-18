namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_tblProduct_tyoeNull : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblProduct", "CostPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.tblProduct", "SalePrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblProduct", "SalePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.tblProduct", "CostPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
