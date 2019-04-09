namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChanchedOrderentity4 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Owners", "OrderId");
            AddForeignKey("dbo.Owners", "OrderId", "dbo.Orders", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Owners", "OrderId", "dbo.Orders");
            DropIndex("dbo.Owners", new[] { "OrderId" });
        }
    }
}
