using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Shared
{
    public class LmsUniqueViolationException : LmsException
    {
        public LmsUniqueViolationException( string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
