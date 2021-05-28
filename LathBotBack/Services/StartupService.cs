using System;

using DSharpPlus;

using LathBotBack.Base;

namespace LathBotBack.Services
{
	public class StartupService : BaseService<StartupService>
	{
		public bool IsInDesignMode { get; private set; }
		public bool StartUpCompleted = false;
		public DateTime StartTime { get; set; }

		public override void Init(DiscordClient client)
		{
			StartTime = DateTime.Now;
			IsInDesignMode = false;
			StartUpCompleted = true;
		}
	}
}
