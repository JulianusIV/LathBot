using System;
using System.IO;
#if !DEBUG
using System.Threading; 
#endif

namespace LathBotFront
{
    public class Program
    {
        public static void Main(string[] _)
        {
#if DEBUG
            foreach (var line in File.ReadAllLines("settings.env"))
            {
                Environment.SetEnvironmentVariable(line[..line.IndexOf('=')], line[(line.IndexOf('=') + 1)..]);
            }
#endif

            //60 second startup delay to wait for database because docker-compose is an asshole
#if !DEBUG
			Thread.Sleep(60000);
#endif

            //Startup
            Bot.Instance.RunAsync().GetAwaiter().GetResult();
        }
    }
}
