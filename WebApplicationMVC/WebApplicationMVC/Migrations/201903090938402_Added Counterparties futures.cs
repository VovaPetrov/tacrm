namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCounterpartiesfutures : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnalyticsCounterparties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CounterpartyId = c.String(),
                        AnalyticsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AnalyticsCounterparties");
        }
    }
}
