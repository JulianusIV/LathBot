using System;

namespace LathBotBack.Logging
{
    public class LoggingEventArgs(string message) : EventArgs
    {
        public string Message { get; set; } = message;
    }
}
