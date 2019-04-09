using System;
using System.ComponentModel.DataAnnotations;

namespace IPWhitelist.Models
{
    public class WhitelistIP
    {
        [Key]
        public int Id { get; set; }

        public string RuleName { get; set; }

        public bool IsActive { get; set; }

        public byte[] StartIP { get; set; }

        public byte[] EndIP { get; set; }
    }
}