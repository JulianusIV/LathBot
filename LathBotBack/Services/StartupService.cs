using DSharpPlus;
using LathBotBack.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LathBotBack.Services
{
	class StartupService : BaseService<StartupService>
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
