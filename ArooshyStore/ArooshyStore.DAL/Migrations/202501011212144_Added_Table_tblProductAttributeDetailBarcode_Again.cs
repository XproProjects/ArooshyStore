﻿namespace ArooshyStore.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_Table_tblProductAttributeDetailBarcode_Again : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblProductAttributeDetailBarcode",
                c => new
                    {
                        ProductAttributeDetailBarcodeId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(),
                        Attribute1 = c.Int(),
                        AttributeDetailId1 = c.Int(),
                        Attribute2 = c.Int(),
                        AttributeDetailId2 = c.Int(),
                        Barcode = c.String(maxLength: 50),
                        Status = c.Boolean(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedDate = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.ProductAttributeDetailBarcodeId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblProductAttributeDetailBarcode");
        }
    }
}