namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblProductReview : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblProductReview",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        ReviewByCustomerId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        ReviewByName = c.String(maxLength: 200),
                        ReviewByEmail = c.String(maxLength: 200),
                        ReviewDetail = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ReviewId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblProductReview");
        }
    }
}
