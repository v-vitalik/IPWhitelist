namespace IPWhitelist.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using IPWhitelist.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<IPWhitelist.Models.IPAddressesContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IPAddressesContext context)
        {
            //context.Ranges.AddOrUpdate(x => x.Id,
            //    new IPAddressRange() { Id = 1, Name = "Range1", StartAddress = "198.55.55.55", EndAddress = "198.55.55.60" },
            //    new IPAddressRange() { Id = 2, Name = "Range2", StartAddress = "198.55.55.125", EndAddress = "198.55.55.160" });
        }
    }
}
