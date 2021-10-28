using Newtonsoft.Json;

namespace LathBotBack.Config
{
	public struct Configuration
	{
		public string Token { get; internal set; }
		public string ConnectionString { get; internal set; }
		public string NasaApiKey { get; set; }
	}
}
