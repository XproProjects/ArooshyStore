namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblContactUs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblContactus",
                c => new
                    {
                        ContactUsId = c.Int(nullable: false, identity: true),
                        txtName = c.String(maxLength: 200),
                        txtEmail = c.String(maxLength: 200),
                        txtSubject = c.String(maxLength: 300),
                        txtMessage = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ContactUsId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblContactus");
        }
    }
}
