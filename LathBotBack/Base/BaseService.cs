using DSharpPlus;

using LathBotBack.Services;

namespace LathBotBack.Base
{
	public abstract class BaseService<T> where T : class, new()
	{
		#region Singleton
		private static T instance;
		private static readonly object padlock = new object();
		public static T Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance != null)
						instance = new T();
					return instance;
				}
			}
		}
		#endregion

		public void InitAll(DiscordClient client)
		{
			if (SystemService.instance != null)
				return;
			DiscordObjectService.Instance.Init(client);
			GoodGuysService.Instance.Init(client);
			LavalinkService.Instance.Init(client);
			RuleService.Instance.Init(client);
			StartupService.Instance.Init(client);
			SystemService.Instance.Init(client);
		}

		public abstract void Init(DiscordClient client);
	}
}
