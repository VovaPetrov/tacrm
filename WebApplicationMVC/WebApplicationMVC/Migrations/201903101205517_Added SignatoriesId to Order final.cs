namespace WebApplicationMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSignatoriesIdtoOrderfinal : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SignatoryOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        SignatoryId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SignatoryOrders");
        }
    }
}
