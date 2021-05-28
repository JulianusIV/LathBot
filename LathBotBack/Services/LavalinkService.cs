using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using LathBotBack.Base;
using LathBotBack.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LathBotBack.Services
{
	class LavalinkService : BaseService<LavalinkService>
	{
		public Dictionary<DiscordGuild, List<LavalinkTrack>> Queues;
		public Dictionary<DiscordGuild, Repeaters> Repeats;

		public override void Init(DiscordClient client) { }
	}
}
