namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDateofexpert : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "DateOfExpert", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "DateOfExpert");
        }
    }
}
