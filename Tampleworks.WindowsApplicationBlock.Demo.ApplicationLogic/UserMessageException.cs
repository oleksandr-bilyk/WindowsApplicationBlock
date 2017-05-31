using System;

namespace Tampleworks.WindowsApplicationBlock.Demo.AppLogic
{
    public class UserMessageException : Exception
    {
        public UserMessageException(string message) : base(message) { }

        public UserMessageException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
