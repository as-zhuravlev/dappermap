using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Shared
{
    public class LmsException : Exception
    {
        public LmsException(string message) : base(message) { }

        public LmsException(string message, Exception innerException) : base(message, innerException) { }
    }
}
