using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using LathBotBack;
using LathBotBack.Services;

namespace LathBotFront.Commands
{
	public class RuleCommands : BaseCommandModule
    {
        [Command("rule")]
        [Aliases("r")]
        public async Task Rule(CommandContext ctx, uint ruleNum)
        {
            if (ruleNum > 13 || ruleNum < 0)
            {
                await ctx.RespondAsync($"Rule number {ruleNum} does not exist here ~~yet~~!");
                return;
            }
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(101, 24, 201),
                Title = $"Rule {RuleService.rules[ruleNum - 1].RuleNum}:",
                Description = RuleService.rules[ruleNum - 1].RuleText
            };
            await ctx.RespondAsync(builder.Build());
        }
    }
}
