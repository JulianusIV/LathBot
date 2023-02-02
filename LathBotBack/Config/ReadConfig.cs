using System;

namespace LathBotBack.Config
{
    public static class ReadConfig
    {
        public static Configuration Config;

        public static void Read()
        {
            Config = new Configuration
            {
                Token = Environment.GetEnvironmentVariable("Token"),
                ConnectionString = Environment.GetEnvironmentVariable("ConnectionString"),
                NasaApiKey = Environment.GetEnvironmentVariable("NASAApiKey"),
                LavaLinkPass = Environment.GetEnvironmentVariable("LavaLinkPass"),
                RijndaelInputKey = Environment.GetEnvironmentVariable("RijndaelInputKey"),
                UptimeKumaUrl = Environment.GetEnvironmentVariable("UptimeKumaUrl")
            };
        }
    }
}
