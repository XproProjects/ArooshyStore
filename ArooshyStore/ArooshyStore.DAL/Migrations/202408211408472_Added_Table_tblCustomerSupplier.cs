namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblCustomerSupplier : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblCustomerSupplier",
                c => new
                    {
                        CustomerSupplierId = c.Int(nullable: false, identity: true),
                        CustomerSupplierName = c.String(maxLength: 200),
                        CustomerSupplierType = c.String(maxLength: 50),
                        Contact1 = c.String(maxLength: 30),
                        Contact2 = c.String(maxLength: 30),
                        Email = c.String(maxLength: 200),
                        HouseNo = c.String(maxLength: 50),
                        Street = c.String(maxLength: 50),
                        ColonyOrVillageName = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 50),
                        CityId = c.Int(),
                        CompleteAddress = c.String(maxLength: 1000),
                        CreditDays = c.Int(),
                        CreditLimit = c.Decimal(precision: 18, scale: 2),
                        Remarks = c.String(maxLength: 1000),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.CustomerSupplierId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblCustomerSupplier");
        }
    }
}
