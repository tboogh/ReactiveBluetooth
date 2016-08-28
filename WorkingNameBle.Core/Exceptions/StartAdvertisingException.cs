using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkingNameBle.Core.Exceptions
{
    public class StartAdvertisingException : Exception
    {
        public StartAdvertisingException(string message) : base(message)
        {
        }
    }
}
