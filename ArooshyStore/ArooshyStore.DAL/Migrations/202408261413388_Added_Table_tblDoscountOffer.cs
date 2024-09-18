namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblDoscountOffer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblDiscountOffer",
                c => new
                    {
                        OfferId = c.Int(nullable: false, identity: true),
                        DiscountName = c.String(maxLength: 300),
                        DiscPercent = c.Decimal(precision: 18, scale: 2),
                        SelectType = c.String(maxLength: 100),
                        CategoryId = c.Int(),
                        ProductId = c.Int(),
                        ExpiredOn = c.DateTime(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.OfferId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblDiscountOffer");
        }
    }
}
