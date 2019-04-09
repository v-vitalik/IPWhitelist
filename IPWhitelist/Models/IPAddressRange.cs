using System.ComponentModel.DataAnnotations;
using System.Net;
using IPWhitelist.Extensions;

namespace IPWhitelist.Models
{
    public class IPAddressRange
    {
        [Key]
        public int Id { get; set; }

        public string RuleName { get; set; }

        public string StartAddress { get; set; }

        public string EndAddress { get; set; }
        
        public bool IsActive { get; set; }

        public bool ValidAddresses()
        {
            IPAddress startAddress;
            IPAddress endAddress;
            if (IPAddress.TryParse(StartAddress, out startAddress) && IPAddress.TryParse(EndAddress, out endAddress))
                return StartAddress.LessOrEqualTo(EndAddress);
            return false;
        }
    }
}