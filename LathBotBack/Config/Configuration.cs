namespace LathBotBack.Config
{
    public struct Configuration
    {
        public string Token { get; internal set; }
        public string ConnectionString { get; internal set; }
        public string NasaApiKey { get; set; }

        public string LavaLinkPass { get; set; }
        public string RijndaelInputKey { get; set; }
        public string UptimeKumaUrl { get; set; }
    }
}
