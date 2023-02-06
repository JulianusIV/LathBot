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
        public Timer TakeYourMeds = new();
        public Timer TakeYourMeds2 = new();

        public LoggingPublisher Logger = new();

        public override void Init(DiscordClient client)
        {
            WarnTimer.Start();

#if DEBUG
            TimeZoneInfo insertTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
#else
			TimeZoneInfo insertTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
#endif
            DateTime insertTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, insertTimeZone);
            DateTime wantedTime = insertTime.Date + TimeSpan.FromHours(7.5);
            DateTime wantedTime2 = insertTime.Date + TimeSpan.FromHours(19);

            if (insertTime > wantedTime)
                wantedTime += TimeSpan.FromDays(1);
            if (insertTime > wantedTime2)
                wantedTime2 += TimeSpan.FromDays(1);

            TakeYourMeds.Interval = (wantedTime - insertTime).TotalMilliseconds;
            TakeYourMeds.AutoReset = false;
            TakeYourMeds.Start();
            TakeYourMeds2.Interval = (wantedTime2 - insertTime).TotalMilliseconds;
            TakeYourMeds2.AutoReset = false;
            TakeYourMeds2.Start();
        }
    }
}
