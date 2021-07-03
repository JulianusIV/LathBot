using DSharpPlus;

using LathBotBack.Base;
using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;
using System.Runtime.CompilerServices;

namespace LathBotBack.Services
{
	public class GoodGuysService : BaseService
	{
		#region Singleton
		private static GoodGuysService instance;
		private static readonly object padlock = new object();
		public static GoodGuysService Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
						instance = new GoodGuysService();
					return instance;
				}
			}
		}
		#endregion

		public int GoodGuysReactionCount
		{
			get
			{
				VariableRepository repo = new VariableRepository(ReadConfig.configJson.ConnectionString);
				bool result = repo.Read(1, out Variable entity);
				if (result)
				{
					return int.Parse(entity.Value);
				}
				return _goodGuysReactionCount;
			}
			set
			{
				_goodGuysReactionCount = value;
			}
		}
		private int _goodGuysReactionCount = 4;

		public override void Init(DiscordClient client) { }
	}
}
