namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblTagsForProducts2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblTagsForProducts",
                c => new
                    {
                        ProductTagId = c.Int(nullable: false, identity: true),
                        TagId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ProductTagId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblTagsForProducts");
        }
    }
}
