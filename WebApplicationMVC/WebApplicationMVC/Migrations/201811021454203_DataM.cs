namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataM : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Unreads", "Count");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Unreads", "Count", c => c.Int(nullable: false));
        }
    }
}
