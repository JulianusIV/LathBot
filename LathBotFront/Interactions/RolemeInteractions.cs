using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.Interactions
{
    public class RolemeInteractions : ApplicationCommandModule
    {
        private readonly List<ulong> AllowedRoleIds = new List<ulong>
        {
            1046124300761043004
        };

        [SlashCommand("Roleme", "Assign yourself a role")]
        public async Task Roleme(InteractionContext ctx,
            [Option("Role", "The role you want to have assigned")]
            DiscordRole role)
        {
            await ctx.DeferAsync(true);
            if (!AllowedRoleIds.Contains(role.Id))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You can not assign yourself that role."));
                return;
            }

            if (ctx.Member.Roles.Any(x => role.Id == x.Id))
            {
                await ctx.Member.RevokeRoleAsync(role);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"You no longer have the role {role.Mention}"));
            }
            else
            {
                await ctx.Member.GrantRoleAsync(role);
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"You now have the role {role.Mention}"));
            }
        }
    }
}
