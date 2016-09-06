using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBluetooth.Core.Exceptions
{
    public class AdvertiseException : Exception
    {
        public AdvertiseException(string message) : base(message)
        {
        }
    }
}
