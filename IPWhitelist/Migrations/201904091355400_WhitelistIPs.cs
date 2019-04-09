namespace IPWhitelist.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WhitelistIPs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WhitelistIPs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RuleName = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        StartIP = c.Binary(),
                        EndIP = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WhitelistIPs");
        }
    }
}
