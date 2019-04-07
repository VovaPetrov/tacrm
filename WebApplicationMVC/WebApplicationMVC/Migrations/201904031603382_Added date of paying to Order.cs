namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddeddateofpayingtoOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Prices", "Date", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Prices", "Date");
        }
    }
}
