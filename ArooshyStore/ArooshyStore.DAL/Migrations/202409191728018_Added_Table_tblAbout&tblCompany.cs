namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblAbouttblCompany : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblAbout",
                c => new
                    {
                        AboutId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Service1Name = c.String(maxLength: 200),
                        Service1Icon = c.String(maxLength: 20),
                        Service1Description = c.String(maxLength: 500),
                        Service2Name = c.String(maxLength: 200),
                        Service2Icon = c.String(maxLength: 20),
                        Service2Description = c.String(maxLength: 500),
                        Service3Name = c.String(maxLength: 200),
                        Service3Icon = c.String(maxLength: 20),
                        Service3Description = c.String(maxLength: 500),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.AboutId);
            
            CreateTable(
                "dbo.tblCompany",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(maxLength: 200),
                        Contact1 = c.String(maxLength: 50),
                        Contact2 = c.String(maxLength: 50),
                        Email = c.String(maxLength: 200),
                        FacebookId = c.String(maxLength: 1000),
                        InstagramId = c.String(maxLength: 1000),
                        LinkedInId = c.String(maxLength: 1000),
                        Address = c.String(),
                        Longitude = c.String(maxLength: 200),
                        Latitude = c.String(maxLength: 200),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblCompany");
            DropTable("dbo.tblAbout");
        }
    }
}
