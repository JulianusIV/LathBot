using System.Timers;

using DSharpPlus;

using LathBotBack.Base;
using LathBotBack.Logging;

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
