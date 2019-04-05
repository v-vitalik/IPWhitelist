using System.ComponentModel.DataAnnotations;
using System.Net;

namespace IPWhitelist.Models
{
    public class IPAddressRange
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string StartAddress { get; set; }

        public string EndAddress { get; set; }

        public bool ValidAddresses()
        {
            IPAddress address;
            return IPAddress.TryParse(StartAddress, out address) && IPAddress.TryParse(EndAddress, out address);
        }
    }
}