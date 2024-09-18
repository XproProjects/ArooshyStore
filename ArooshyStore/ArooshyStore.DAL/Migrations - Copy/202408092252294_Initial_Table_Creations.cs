namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Table_Creations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblAction",
                c => new
                    {
                        ActionId = c.Int(nullable: false, identity: true),
                        ActionName = c.String(maxLength: 200),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ActionId);
            
            CreateTable(
                "dbo.tblAssignAction",
                c => new
                    {
                        AssignId = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(),
                        ActionId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.AssignId);
            
            CreateTable(
                "dbo.tblDocument",
                c => new
                    {
                        DocumentId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(),
                        TypeId = c.String(maxLength: 100),
                        DocumentExtension = c.String(maxLength: 20),
                        DocumentType = c.String(maxLength: 100),
                        Remarks = c.String(maxLength: 100),
                        DocumentUrl = c.String(maxLength: 500),
                        TypeOfDocument = c.String(maxLength: 500),
                        UserId = c.Int(),
                        IsFileLocked = c.Boolean(),
                    })
                .PrimaryKey(t => t.DocumentId);
            
            CreateTable(
                "dbo.tblErrorsLog",
                c => new
                    {
                        ErrorId = c.Int(nullable: false, identity: true),
                        ErrorLineNumber = c.String(maxLength: 50),
                        ErrorDescription = c.String(maxLength: 500),
                        ErrorSource = c.String(maxLength: 100),
                        ErrorClass = c.String(maxLength: 200),
                        ErrorAction = c.String(maxLength: 200),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ErrorId);
            
            CreateTable(
                "dbo.tblInfo",
                c => new
                    {
                        InfoId = c.Int(nullable: false, identity: true),
                        FullName = c.String(maxLength: 250),
                        Contact1 = c.String(maxLength: 20),
                        Contact2 = c.String(maxLength: 20),
                        Email = c.String(maxLength: 100),
                        Cnic = c.String(maxLength: 30),
                        Gender = c.String(maxLength: 10),
                        DOB = c.DateTime(storeType: "date"),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                        EmergencyContact1 = c.String(maxLength: 20),
                        EmergencyContact2 = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.InfoId);
            
            CreateTable(
                "dbo.tblLogging",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        LogDateTime = c.DateTime(),
                        LogByUserId = c.Int(),
                        LogType = c.String(maxLength: 20),
                        FormType = c.String(maxLength: 20),
                        FormId = c.Int(),
                        OldData = c.String(),
                        NewData = c.String(),
                    })
                .PrimaryKey(t => t.LogId);
            
            CreateTable(
                "dbo.tblModule",
                c => new
                    {
                        ModuleId = c.Int(nullable: false, identity: true),
                        ModuleName = c.String(maxLength: 200),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ModuleId);
            
            CreateTable(
                "dbo.tblSecurityGroup",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        GroupName = c.String(maxLength: 1000),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.GroupId);
            
            CreateTable(
                "dbo.tblSecurityGroupDetail",
                c => new
                    {
                        GroupDetalId = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(),
                        ModuleId = c.Int(),
                        ActionId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.GroupDetalId);
            
            CreateTable(
                "dbo.tblUser",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        InfoId = c.Int(),
                        UserTypeId = c.Int(),
                        Password = c.String(maxLength: 200),
                        IsActive = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.tblUserRole",
                c => new
                    {
                        UserRoleId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(),
                        ModuleId = c.Int(),
                        ActionId = c.Int(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.UserRoleId);
            
            CreateTable(
                "dbo.tblUserType",
                c => new
                    {
                        UserTypeId = c.Int(nullable: false, identity: true),
                        TypeName = c.String(maxLength: 50),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.UserTypeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblUserType");
            DropTable("dbo.tblUserRole");
            DropTable("dbo.tblUser");
            DropTable("dbo.tblSecurityGroupDetail");
            DropTable("dbo.tblSecurityGroup");
            DropTable("dbo.tblModule");
            DropTable("dbo.tblLogging");
            DropTable("dbo.tblInfo");
            DropTable("dbo.tblErrorsLog");
            DropTable("dbo.tblDocument");
            DropTable("dbo.tblAssignAction");
            DropTable("dbo.tblAction");
        }
    }
}
