using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using LathBotBack.Base;
using LathBotBack.Enums;
using System.Collections.Generic;

namespace LathBotBack.Services
{
    public class LavalinkService : BaseService
    {
        #region Singleton
        private static LavalinkService instance;
        private static readonly object padlock = new();
        public static LavalinkService Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new LavalinkService();
                    return instance;
                }
            }
        }
        #endregion

        public Dictionary<DiscordGuild, List<LavalinkTrack>> Queues;
        public Dictionary<DiscordGuild, Repeaters> Repeats;

        public override void Init(DiscordClient client) { }
    }
}
