namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_lengthInCityName_tblCity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblCity", "CityName", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblCity", "CityName", c => c.String(maxLength: 50));
        }
    }
}
