namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedmultiplypayingtoOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prices", "IsPaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prices", "IsPaid");
        }
    }
}
