using System.Collections.Generic;
using IPWhitelist.Extensions;
using IPWhitelist.Models;

namespace IPWhitelist.Cache
{
    public static class MemoryCacher
    {
        private static Dictionary<string, IPAddressRange> _cache;

        static MemoryCacher()
        {
            _cache = new Dictionary<string, IPAddressRange>();
        }

        public static bool Contains(string key)
        {
            return _cache.ContainsKey(key);
        }

        public static void Add(string key, IPAddressRange value)
        {
            _cache.Add(key, value);
        }

        public static void UpdateOrDelete(IPAddressRange ipRange)
        {
            List<string> deletionKeys = new List<string>();
            foreach (var range in _cache)
            {
                if (ipRange.Id == range.Value.Id)
                {
                    if (range.Key.MoreOrEqualTo(ipRange.StartAddress) && range.Key.LessOrEqualTo(ipRange.EndAddress))
                    {
                        range.Value.Name = ipRange.Name;
                        range.Value.StartAddress = ipRange.StartAddress;
                        range.Value.EndAddress = ipRange.EndAddress;
                    }
                    else
                        deletionKeys.Add(range.Key);
                }
            }
            DeleteKeys(deletionKeys);
        }

        public static void DeleteIfContains(IPAddressRange ipRange)
        {
            List<string> deletionKeys = new List<string>();
            foreach (var range in _cache)
            {
                if (ipRange.Id == range.Value.Id)
                    deletionKeys.Add(range.Key);
            }
            DeleteKeys(deletionKeys);
        }

        private static void DeleteKeys(IEnumerable<string> deletionKeys)
        {
            foreach (var key in deletionKeys)
            {
                _cache.Remove(key);
            }
        }
    }
}