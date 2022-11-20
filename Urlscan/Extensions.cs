using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Urlscan
{
    internal static class Extensions
    {
        public static DateTime ToDate(this long value)
            => DateTime.UnixEpoch.AddSeconds(value);

        public static long ToUnixSeconds(this DateTime value)
            => (long)(value - DateTime.UnixEpoch).TotalSeconds;

        public static string ToKebabCase(this string str) =>
            string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "-" + x.ToString() : x.ToString())).ToLower();
    }
}