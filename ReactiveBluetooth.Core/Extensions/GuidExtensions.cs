using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Extensions
{
    public static class StringExtensions
    {
        public static Guid ToGuid(this string guid)
        {
            string standardUuidFormat = "{0}{1}-0000-1000-8000-00805f9b34fb";
            if (guid.Length == 4)
            {
                guid = string.Format(standardUuidFormat, "0000", guid);
            } else if (guid.Length == 8)
            {
                guid = string.Format(standardUuidFormat, guid);
            }
            return Guid.ParseExact(guid, "d");
        }
    }
}
