namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblUnit_2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblUnit",
                c => new
                    {
                        UnitId = c.Int(nullable: false, identity: true),
                        UnitName = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.UnitId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblUnit");
        }
    }
}
