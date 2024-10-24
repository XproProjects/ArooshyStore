namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_column_StatusType_tblInvoiceStatus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblInvoiceStatus", "Status", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblInvoiceStatus", "Status", c => c.Boolean());
        }
    }
}
