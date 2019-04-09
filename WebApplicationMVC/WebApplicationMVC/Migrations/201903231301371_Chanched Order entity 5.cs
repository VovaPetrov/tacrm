namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChanchedOrderentity5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Performers", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Performers", "OrderId");
            CreateIndex("dbo.Performers", "UserId");
            AddForeignKey("dbo.Performers", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Performers", "UserId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Performers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Performers", "OrderId", "dbo.Orders");
            DropIndex("dbo.Performers", new[] { "UserId" });
            DropIndex("dbo.Performers", new[] { "OrderId" });
            AlterColumn("dbo.Performers", "UserId", c => c.String());
        }
    }
}
