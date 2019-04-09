namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChanchedOrderentity2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "CounterpartyId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Orders", "CounterpartyId");
            AddForeignKey("dbo.Orders", "CounterpartyId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "CounterpartyId", "dbo.AspNetUsers");
            DropIndex("dbo.Orders", new[] { "CounterpartyId" });
            AlterColumn("dbo.Orders", "CounterpartyId", c => c.String());
        }
    }
}
