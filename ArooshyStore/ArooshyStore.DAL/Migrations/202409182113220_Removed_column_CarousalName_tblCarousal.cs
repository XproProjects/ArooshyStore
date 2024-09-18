namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removed_column_CarousalName_tblCarousal : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tblCarousel", "CarouselName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tblCarousel", "CarouselName", c => c.String(maxLength: 200));
        }
    }
}
