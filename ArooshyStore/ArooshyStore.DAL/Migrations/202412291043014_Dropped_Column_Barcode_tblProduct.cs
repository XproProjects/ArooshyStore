namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Dropped_Column_Barcode_tblProduct : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.tblProduct", "Barcode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tblProduct", "Barcode", c => c.String(maxLength: 50));
        }
    }
}
