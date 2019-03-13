namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedcounterpartyentities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PerformerCounterparties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CounterpartyId = c.String(),
                        PerformerId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SourceCounterparties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CounterpartyId = c.String(),
                        SourceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SourceCounterparties");
            DropTable("dbo.PerformerCounterparties");
        }
    }
}
