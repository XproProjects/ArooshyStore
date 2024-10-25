namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_ColumnDesignation_tblEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.tblEmployee", "DesignationId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.tblEmployee", "DesignationId");
        }
    }
}
