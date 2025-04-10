using DSharpPlus;
using LathBotBack.Base;
using System;
using System.Diagnostics;
using System.Threading;

namespace LathBotBack.Services
{
    public class StartupService : BaseService
    {
        #region Singleton
        private static StartupService instance;
        private static readonly Lock padlock = new();
        public static StartupService Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new StartupService();
                    return instance;
                }
            }
        }
        #endregion

        public bool StartUpCompleted = false;
        public bool InitCompleted = false;
        public DateTime StartTime
        {
            get => Process.GetCurrentProcess().StartTime;
        }

        public override void Init(DiscordClient client)
            => this.StartUpCompleted = true;
    }
}
