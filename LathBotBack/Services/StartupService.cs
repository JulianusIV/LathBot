using DSharpPlus;
using LathBotBack.Base;
using System;
using System.Diagnostics;

namespace LathBotBack.Services
{
    public class StartupService : BaseService
    {
        #region Singleton
        private static StartupService instance;
        private static readonly object padlock = new object();
        public static StartupService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new StartupService();
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
        {
            StartUpCompleted = true;
        }
    }
}
