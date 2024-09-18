namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_tbl_tblCarousel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblCarousel",
                c => new
                    {
                        CarouselId = c.Int(nullable: false, identity: true),
                        Line1 = c.String(maxLength: 500),
                        Line2 = c.String(maxLength: 500),
                        Line3 = c.String(maxLength: 500),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.CarouselId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblCarousel");
        }
    }
}
