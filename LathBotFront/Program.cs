using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
