using System;

namespace LathBotBack.Logging
{
    public class LoggingEventArgs : EventArgs
    {
        public LoggingEventArgs(string message)
            => Message = message;

        public string Message { get; set; }
    }
}
