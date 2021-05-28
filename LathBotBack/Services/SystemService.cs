using DSharpPlus;
using LathBotBack.Base;
using LathBotBack.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace LathBotBack.Services
{
	class SystemService : BaseService<SystemService>
	{
		public Timer WarnTimer = new Timer(3600000);

		public LoggingPublisher Logger = new LoggingPublisher();

		public override void Init(DiscordClient client)
		{
			WarnTimer.Start();
		}
	}
}
