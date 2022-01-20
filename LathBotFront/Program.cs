using System;
using System.IO;

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
            //Startup
            Bot.Instance.RunAsync().GetAwaiter().GetResult();
        }
    }
}
