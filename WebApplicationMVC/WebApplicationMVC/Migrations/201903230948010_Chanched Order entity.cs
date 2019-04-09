namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChanchedOrderentity : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "CreatedDate", c => c.DateTime(nullable: false));
            CreateIndex("dbo.Orders", "MetaId");
            CreateIndex("dbo.Orders", "BranchId");
            CreateIndex("dbo.Orders", "SourceId");
            CreateIndex("dbo.Orders", "StatusId");
            CreateIndex("dbo.Orders", "PriceListId");
            CreateIndex("dbo.Orders", "PropsId");
            AddForeignKey("dbo.Orders", "BranchId", "dbo.Branches", "Id");
            AddForeignKey("dbo.Orders", "MetaId", "dbo.Metas", "Id");
            AddForeignKey("dbo.Orders", "PriceListId", "dbo.PriceLists", "Id");
            AddForeignKey("dbo.Orders", "PropsId", "dbo.Props", "Id");
            AddForeignKey("dbo.Orders", "SourceId", "dbo.Sources", "Id");
            AddForeignKey("dbo.Orders", "StatusId", "dbo.Status", "Id");
            DropColumn("dbo.Orders", "OwnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "OwnerId", c => c.Int(nullable: false));
            DropForeignKey("dbo.Orders", "StatusId", "dbo.Status");
            DropForeignKey("dbo.Orders", "SourceId", "dbo.Sources");
            DropForeignKey("dbo.Orders", "PropsId", "dbo.Props");
            DropForeignKey("dbo.Orders", "PriceListId", "dbo.PriceLists");
            DropForeignKey("dbo.Orders", "MetaId", "dbo.Metas");
            DropForeignKey("dbo.Orders", "BranchId", "dbo.Branches");
            DropIndex("dbo.Orders", new[] { "PropsId" });
            DropIndex("dbo.Orders", new[] { "PriceListId" });
            DropIndex("dbo.Orders", new[] { "StatusId" });
            DropIndex("dbo.Orders", new[] { "SourceId" });
            DropIndex("dbo.Orders", new[] { "BranchId" });
            DropIndex("dbo.Orders", new[] { "MetaId" });
            AlterColumn("dbo.Orders", "CreatedDate", c => c.DateTime());
        }
    }
}
