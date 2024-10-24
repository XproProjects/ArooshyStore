namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblInvoiceStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblInvoiceStatus",
                c => new
                    {
                        InvoiceStatusId = c.Int(nullable: false, identity: true),
                        InvoiceNumber = c.String(maxLength: 100),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.InvoiceStatusId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblInvoiceStatus");
        }
    }
}
