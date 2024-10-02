namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_ColumnPhone_tblContactUs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblContactus", "Name", c => c.String(maxLength: 200));
            AddColumn("dbo.tblContactus", "Email", c => c.String(maxLength: 200));
            AddColumn("dbo.tblContactus", "Subject", c => c.String(maxLength: 200));
            AddColumn("dbo.tblContactus", "PhoneNo", c => c.String(maxLength: 20));
            AddColumn("dbo.tblContactus", "Message", c => c.String());
            DropColumn("dbo.tblContactus", "txtName");
            DropColumn("dbo.tblContactus", "txtEmail");
            DropColumn("dbo.tblContactus", "txtSubject");
            DropColumn("dbo.tblContactus", "txtMessage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.tblContactus", "txtMessage", c => c.String());
            AddColumn("dbo.tblContactus", "txtSubject", c => c.String(maxLength: 300));
            AddColumn("dbo.tblContactus", "txtEmail", c => c.String(maxLength: 200));
            AddColumn("dbo.tblContactus", "txtName", c => c.String(maxLength: 200));
            DropColumn("dbo.tblContactus", "Message");
            DropColumn("dbo.tblContactus", "PhoneNo");
            DropColumn("dbo.tblContactus", "Subject");
            DropColumn("dbo.tblContactus", "Email");
            DropColumn("dbo.tblContactus", "Name");
        }
    }
}
