using DSharpPlus.Commands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using LathBotBack.Services;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.Commands
{
    public class RuleCommands
    {
        [Command("rule")]
        [TextAlias("r")]
        public async Task Rule(CommandContext ctx, uint ruleNum)
        {
            if (ruleNum > 13 || ruleNum < 0)
            {
                await ctx.RespondAsync($"Rule number {ruleNum} does not exist here ~~yet~~!");
                return;
            }
            var rule = RuleService.Rules.First(x => x.RuleNum == ruleNum);
            DiscordEmbedBuilder builder = new()
            {
                Color = new DiscordColor(101, 24, 201),
                Title = $"Rule {rule.RuleNum} ({rule.ShortDesc}):",
                Description = rule.RuleText
            };
            await ctx.RespondAsync(builder.Build());
        }
    }
}
