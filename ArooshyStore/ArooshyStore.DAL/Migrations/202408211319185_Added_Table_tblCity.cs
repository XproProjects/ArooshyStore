namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblCity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblCity",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        CityName = c.String(maxLength: 50),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.CityId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblCity");
        }
    }
}
