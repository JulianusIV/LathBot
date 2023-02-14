using DSharpPlus;
using LathBotBack.Base;
using LathBotBack.Logging;
using System;
using System.Timers;

namespace LathBotBack.Services
{
    public class SystemService : BaseService
    {
        #region Singleton
        private static SystemService instance;
        private static readonly object padlock = new();
        public static SystemService Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new SystemService();
                    return instance;
                }
            }
        }
        #endregion

        public Timer WarnTimer = new(3600000);

        public LoggingPublisher Logger = new();

        public override void Init(DiscordClient client)
        {
            WarnTimer.Start();
        }
    }
}
