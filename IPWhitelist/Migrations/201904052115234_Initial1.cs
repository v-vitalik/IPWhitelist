namespace IPWhitelist.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IPAddressRanges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        StartAddress = c.String(),
                        EndAddress = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IPAddressRanges");
        }
    }
}
