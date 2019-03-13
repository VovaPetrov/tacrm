namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCounterpartiesIdtoOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CounterpartyId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CounterpartyId");
        }
    }
}
