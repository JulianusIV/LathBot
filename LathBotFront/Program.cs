namespace LathBotFront
{
	public class Program
	{
#pragma warning disable IDE0060 // Remove unused parameter
		public static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
		{
			Bot.Instance.RunAsync().GetAwaiter().GetResult();
		}
	}
}
