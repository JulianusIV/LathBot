using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.Commands
{
	public class TechnicalCommands : BaseCommandModule
	{
		[Command("ping")]
		public async Task Ping(CommandContext ctx)
		{
			await ctx.TriggerTypingAsync();
			await ctx.RespondAsync("Pong");
		}
	}
}
