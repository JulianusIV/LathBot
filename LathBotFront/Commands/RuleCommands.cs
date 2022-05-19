using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using LathBotBack;
using LathBotBack.Services;
using System.Linq;

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
            var rule = RuleService.Rules.First(x => x.RuleNum == ruleNum);
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(101, 24, 201),
                Title = $"Rule {rule.RuleNum} ({rule.ShortDesc}):",
                Description = rule.RuleText
            };
            await ctx.RespondAsync(builder.Build());
        }
    }
}
