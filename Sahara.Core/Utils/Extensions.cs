using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.Utils
{
    public static class Extensions
    {
        public static IEnumerable<string> Explode(this string thisString)
        {
            var ret = new List<string>();
            var cursor = 0;
            var member = "";
            var isInNestedArray = false;
            while (cursor < thisString.Length)
            {
                var ch = thisString[cursor];
                if (ch == ',' && !isInNestedArray)
                {
                    ret.Add(member.Trim());
                    member = "";
                    cursor++;
                    continue;
                }

                if (!isInNestedArray && ch == '{')
                {
                    isInNestedArray = true;
                }

                if (isInNestedArray && ch == '}')
                {
                    isInNestedArray = false;
                }

                member += thisString[cursor++];
            }
            ret.Add(member.Trim());
            return ret;
        }

        /// <summary>
        /// http://blog.codinghorror.com/determining-build-date-the-hard-way/
        /// </summary>
        public static DateTime GetLinkerTime(this Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }
    }
}
