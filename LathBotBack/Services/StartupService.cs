using System;

using DSharpPlus;

using LathBotBack.Base;

namespace LathBotBack.Services
{
	public class StartupService : BaseService
	{
		#region Singleton
		private static StartupService instance;
		private static readonly object padlock = new object();
		public static StartupService Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
						instance = new StartupService();
					return instance;
				}
			}
		}
		#endregion

		public bool IsInDesignMode { get; private set; }
		public bool StartUpCompleted = false;
		public bool InitCompleted = false;
		public DateTime StartTime { get; set; }

		public override void Init(DiscordClient client)
		{
			StartTime = DateTime.Now;
			IsInDesignMode = false;
			StartUpCompleted = true;
		}
	}
}
