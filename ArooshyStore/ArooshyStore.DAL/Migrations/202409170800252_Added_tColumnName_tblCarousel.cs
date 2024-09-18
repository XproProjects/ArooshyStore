namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_tColumnName_tblCarousel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblCarousel", "CarouselName", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblCarousel", "CarouselName");
        }
    }
}
