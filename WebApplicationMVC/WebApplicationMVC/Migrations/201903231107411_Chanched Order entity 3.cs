namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChanchedOrderentity3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Owners", "OrderId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Owners", "OrderId");
        }
    }
}
