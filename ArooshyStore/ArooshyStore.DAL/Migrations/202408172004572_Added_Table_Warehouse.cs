namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_Warehouse : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblWarehouse",
                c => new
                    {
                        WarehouseId = c.Int(nullable: false, identity: true),
                        WarehouseName = c.String(maxLength: 100),
                        Address = c.String(maxLength: 1000),
                        Contact1 = c.String(maxLength: 50),
                        Contact2 = c.String(maxLength: 50),
                        Email = c.String(maxLength: 200),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.WarehouseId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblWarehouse");
        }
    }
}
