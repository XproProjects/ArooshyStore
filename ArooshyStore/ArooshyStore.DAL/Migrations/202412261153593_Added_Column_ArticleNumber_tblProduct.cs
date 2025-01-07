namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Column_ArticleNumber_tblProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblProduct", "ArticleNumber", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblProduct", "ArticleNumber");
        }
    }
}
