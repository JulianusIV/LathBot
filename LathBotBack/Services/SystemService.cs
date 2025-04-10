using DSharpPlus;
using LathBotBack.Base;
using LathBotBack.Logging;
using System.Threading;

namespace LathBotBack.Services
{
    public class SystemService : BaseService
    {
        #region Singleton
        private static SystemService instance;
        private static readonly Lock padlock = new();
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

        public System.Timers.Timer WarnTimer = new(3600000);

        public LoggingPublisher Logger = new();

        public override void Init(DiscordClient client)
        {
            this.WarnTimer.Start();
        }
    }
}
