using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

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
