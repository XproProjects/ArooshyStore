namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_typenullabe : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblAttributeDetail", "AttributeDetailName", c => c.String(maxLength: 200));
            AlterColumn("dbo.tblAttributeDetail", "AttributeId", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblAttributeDetail", "AttributeId", c => c.Int(nullable: false));
            AlterColumn("dbo.tblAttributeDetail", "AttributeDetailName", c => c.String(maxLength: 250));
        }
    }
}
