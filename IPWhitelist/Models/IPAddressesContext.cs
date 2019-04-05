using System.Data.Entity;

namespace IPWhitelist.Models
{
    public class IPAddressesContext : DbContext
    {
        public DbSet<IPAddressRange> Ranges { get; set; }
    }
}