using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Shared
{
    public class LmsArgumentException : LmsException
    {
        public LmsArgumentException(string argument) : base($"Ivalid arg: {argument}") { }
        public LmsArgumentException(string argument, string message) : base($"Ivalid arg: {argument}. {message}") { }
        public LmsArgumentException(string argument, string message, Exception innerException) : base($"Ivalid arg: {argument}. {message}", innerException) { }
    }
}
