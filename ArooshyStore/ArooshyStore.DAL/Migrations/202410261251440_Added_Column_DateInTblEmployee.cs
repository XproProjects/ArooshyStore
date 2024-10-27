namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Column_DateInTblEmployee : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.tblEmployee", "DateOfJoining", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.tblEmployee", "DateOfJoining", c => c.DateTime());
        }
    }
}
