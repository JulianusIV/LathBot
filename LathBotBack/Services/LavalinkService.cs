using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

using LathBotBack.Base;
using LathBotBack.Enums;

namespace LathBotBack.Services
{
	class LavalinkService : BaseService<LavalinkService>
	{
		public Dictionary<DiscordGuild, List<LavalinkTrack>> Queues;
		public Dictionary<DiscordGuild, Repeaters> Repeats;

		public override void Init(DiscordClient client) { }
	}
}
