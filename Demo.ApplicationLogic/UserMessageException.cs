using System;

namespace Tampleworks.WindowsApplicationBlock.Demo.ApplicationLogic
{
    public class UserMessageException : Exception
    {
        public UserMessageException(string message) : base(message) { }

        public UserMessageException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
