using Newtonsoft.Json;

namespace LathBotFront
{
	public struct ConfigJson
	{
		[JsonProperty("token")]
		public string Token { get; private set; }
		[JsonProperty("prefix")]
		public string Prefix { get; private set; }
	}
}
