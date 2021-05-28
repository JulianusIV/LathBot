using System.Threading.Tasks;

namespace LathBotFront
{
	public class Program
	{
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable IDE0060 // Remove unused parameter
		public static void Main(string[] args)
#pragma warning restore IDE0079 // Remove unnecessary suppression
#pragma warning restore IDE0060 // Remove unused parameter
		{
			//10 second startup delay to wait for database because docker-compose is an asshole
			Task.Delay(10000);

			//Startup
			Bot.Instance.RunAsync().GetAwaiter().GetResult();
		}
	}
}
