namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Column_PasswordIntblCustomerSupplier : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblCustomerSupplier", "Password", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblCustomerSupplier", "Password");
        }
    }
}
