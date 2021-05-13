using System.IO;
using System.Text;

using Newtonsoft.Json;

namespace LathBotBack.Config
{
	public static class ReadConfig
	{
		public static ConfigJson configJson;

		public static void Read()
		{
			string json = string.Empty;

			using (FileStream fs = File.OpenRead("config.json"))
			using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
				json = sr.ReadToEnd();

			configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
		}
	}
}
