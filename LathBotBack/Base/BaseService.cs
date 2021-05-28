using DSharpPlus;

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

		public abstract void Init(DiscordClient client);
	}
}
