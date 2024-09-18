namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_fieldId_tblAttributeDetail : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.tblAttributeDetail", "AttributeId", "dbo.tblAttribute");
            DropIndex("dbo.tblAttributeDetail", new[] { "AttributeId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.tblAttributeDetail", "AttributeId");
            AddForeignKey("dbo.tblAttributeDetail", "AttributeId", "dbo.tblAttribute", "AttributeId", cascadeDelete: true);
        }
    }
}
