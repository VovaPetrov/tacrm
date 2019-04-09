namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedmoreOwnersLegalClientsOverwatches : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LegalClients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(),
                        EDRPOY = c.String(),
                        LegalAddress = c.String(),
                        Position = c.String(),
                        SubscriberName = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderOverwatches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        OverwatchId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderOwners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        OwnerId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Overwatches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OverWatch = c.DateTime(),
                        FullNameWatcher = c.String(),
                        IsPaidOverWatch = c.Boolean(nullable: false),
                        PriceOverWatch = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Orders", "IsLegalClient", c => c.Boolean(nullable: false));
            AddColumn("dbo.Owners", "Part", c => c.String());
            DropColumn("dbo.Orders", "OverWatch");
            DropColumn("dbo.Orders", "FullNameWatcher");
            DropColumn("dbo.Orders", "IsPaidOverWatch");
            DropColumn("dbo.Orders", "PriceOverWatch");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "PriceOverWatch", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.Orders", "IsPaidOverWatch", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "FullNameWatcher", c => c.String());
            AddColumn("dbo.Orders", "OverWatch", c => c.DateTime());
            DropColumn("dbo.Owners", "Part");
            DropColumn("dbo.Orders", "IsLegalClient");
            DropTable("dbo.Overwatches");
            DropTable("dbo.OrderOwners");
            DropTable("dbo.OrderOverwatches");
            DropTable("dbo.LegalClients");
        }
    }
}
