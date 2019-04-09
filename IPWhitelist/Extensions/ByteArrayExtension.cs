using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IPWhitelist.Extensions
{
    public static class ByteArrayExtension
    {
        public static string IPAddressToString(this byte[] ipAddress)
        {
            StringBuilder ipAddressString = new StringBuilder();
            for(int i = 0; i < ipAddress.Length; i++)
            {
                ipAddressString.Append(ipAddress[i]);
                if (i < ipAddress.Length - 1)
                    ipAddressString.Append('.');
            }
            return ipAddressString.ToString();
        }
    }
}