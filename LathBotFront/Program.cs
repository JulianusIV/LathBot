using System.Threading.Tasks;

namespace LathBotFront
{
	public class Program
	{
		public static void Main(string[] _)
		{
			//10 second startup delay to wait for database because docker-compose is an asshole
			Task.Delay(10000);

			//Startup
			Bot.Instance.RunAsync().GetAwaiter().GetResult();
		}
	}
}
