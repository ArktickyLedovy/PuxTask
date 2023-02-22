using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuxTask.Common.Exceptions
{
    public class NullCachedRecordException : Exception
    {
        public NullCachedRecordException() { }
        public NullCachedRecordException(string message) : base(message) { }
        public NullCachedRecordException (string message, Exception innerException) : base(message, innerException) { }
    }
}
