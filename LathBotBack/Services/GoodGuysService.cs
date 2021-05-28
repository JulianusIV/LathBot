using DSharpPlus;

using LathBotBack.Base;
using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;

namespace LathBotBack.Services
{
	public class GoodGuysService : BaseService<GoodGuysService>
	{
		public int GoodGuysReactionCount
		{
			get
			{
				VariableRepository repo = new VariableRepository(ReadConfig.configJson.ConnectionString);
				bool result = repo.Read(2, out Variable entity);
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
