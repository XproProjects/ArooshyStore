namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_AttributeDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblAttributeDetail",
                c => new
                    {
                        AttributeDetailId = c.Int(nullable: false, identity: true),
                        AttributeDetailName = c.String(maxLength: 250),
                        Status = c.Boolean(),
                        AttributeId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.AttributeDetailId)
                .ForeignKey("dbo.tblAttribute", t => t.AttributeId, cascadeDelete: true)
                .Index(t => t.AttributeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.tblAttributeDetail", "AttributeId", "dbo.tblAttribute");
            DropIndex("dbo.tblAttributeDetail", new[] { "AttributeId" });
            DropTable("dbo.tblAttributeDetail");
        }
    }
}
