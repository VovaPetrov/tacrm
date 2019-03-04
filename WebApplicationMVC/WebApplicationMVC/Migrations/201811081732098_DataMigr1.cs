namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataMigr1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsPaidOverWatch", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsPaidOverWatch");
        }
    }
}
