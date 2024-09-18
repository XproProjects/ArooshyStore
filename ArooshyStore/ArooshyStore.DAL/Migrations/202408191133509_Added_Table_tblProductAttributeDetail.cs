namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblProductAttributeDetail : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblProductAttributeDetail",
                c => new
                    {
                        ProductAttributeDetailId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(),
                        AttributeId = c.Int(),
                        AttributeDetailId = c.Int(),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ProductAttributeDetailId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblProductAttributeDetail");
        }
    }
}
