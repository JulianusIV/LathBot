using System;
using System.Text;
using System.Collections.Generic;

namespace LathBotBack.Logging
{
	public class LoggingPublisher
	{
		public event EventHandler<LoggingEventArgs> RaiseLogEvent;

		protected virtual void OnRaiseLogEvent(LoggingEventArgs e)
            => RaiseLogEvent?.Invoke(this, e);

		public void Log(string message)
		{
			OnRaiseLogEvent(new LoggingEventArgs(message));
		}
	}
}
