using System;
using System.Collections.Generic;
using System.Text;

namespace LathBotBack.Logging
{
	public class LoggingPublisher
	{
		public event EventHandler<LoggingEventArgs> RaiseLogEvent;

		protected virtual void OnRaiseLogEvent(LoggingEventArgs e)
		{
			EventHandler<LoggingEventArgs> raiseEvent = RaiseLogEvent;
			if (raiseEvent != null)
			{
				raiseEvent(this, e);
			}
		}

		public void Log(string message)
		{
			OnRaiseLogEvent(new LoggingEventArgs(message));
		}
	}
}
