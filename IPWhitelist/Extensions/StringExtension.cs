using System;

namespace IPWhitelist.Extensions
{
    public static class StringExtension
    {
        public static bool LessOrEqualTo(this string address1, string address2)
        {
            var ip1 = address1.Split('.');
            var ip2 = address2.Split('.');
            for (int i = 0; i < ip1.Length; i++)
            {
                if (int.Parse(ip1[i]) > int.Parse(ip2[i]))
                    return false;
            }
            return true;
        }

        public static bool MoreOrEqualTo(this string address1, string address2)
        {
            var ip1 = address1.Split('.');
            var ip2 = address2.Split('.');
            for (int i = 0; i < ip1.Length; i++)
            {
                if (int.Parse(ip1[i]) < int.Parse(ip2[i]))
                    return false;
            }
            return true;
        }

        public static byte[] GetBytes(this string address)
        {
            byte[] res = new byte[4];
            var numbers = address.Split('.');
            for (int i = 0; i < numbers.Length; i++)
            {
                res[i] = BitConverter.GetBytes(int.Parse(numbers[i]))[0];
            }
            return res;
        }
    }
}