using System.Timers;

using DSharpPlus;

using LathBotBack.Base;
using LathBotBack.Logging;

namespace LathBotBack.Services
{
	public class SystemService : BaseService
	{
		#region Singleton
		private static SystemService instance;
		private static readonly object padlock = new object();
		public static SystemService Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
						instance = new SystemService();
					return instance;
				}
			}
		}
		#endregion

		public Timer WarnTimer = new Timer(3600000);

		public LoggingPublisher Logger = new LoggingPublisher();

		public override void Init(DiscordClient client)
		{
			WarnTimer.Start();
		}
	}
}
