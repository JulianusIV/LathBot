using DSharpPlus;
using LathBotBack.Base;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using System;
using System.Collections.Generic;
using System.Text;

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
