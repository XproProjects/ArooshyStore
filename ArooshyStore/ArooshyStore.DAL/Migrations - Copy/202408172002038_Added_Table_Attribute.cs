namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_Attribute : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblAttribute",
                c => new
                    {
                        AttributeId = c.Int(nullable: false, identity: true),
                        AttributeName = c.String(maxLength: 50),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.AttributeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblAttribute");
        }
    }
}
