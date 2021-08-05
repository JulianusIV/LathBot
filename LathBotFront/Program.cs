using System.Threading;

namespace LathBotFront
{
	public class Program
	{
		public static void Main(string[] _)
		{
			//60 second startup delay to wait for database because docker-compose is an asshole
			Thread.Sleep(60000);

			//Startup
			Bot.Instance.RunAsync().GetAwaiter().GetResult();
		}
	}
}
