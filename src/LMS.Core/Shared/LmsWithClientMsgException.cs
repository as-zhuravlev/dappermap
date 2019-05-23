using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Core.Shared
{
    public class LmsWithClientMsgException : LmsException
    {
        public LmsWithClientMsgException (string message, string clientMessage, Exception innerException = null) : base(message)
        {
            ClientMessage = clientMessage;
        }
        
        public string ClientMessage { get; }
    
        public LmsWithClientMsgException (string message, Exception innerException = null) : base(message, innerException)
        {
            ClientMessage = message;
        }
    }
}
