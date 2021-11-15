using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using WarnModule;

namespace LathBotFront.Interactions
{
    public class WarnInteractions : ApplicationCommandModule
    {
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Warn")]
        public async Task Warn(ContextMenuContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);

            WarnBuilder warnBuilder = new WarnBuilder(
                ctx.Client,
                ctx.Guild.GetChannel(512370308976607250),
                ctx.Guild,
                ctx.Member,
                await ctx.Guild.GetMemberAsync(ctx.TargetMessage.Author.Id),
                ctx.TargetMessage);
        }
    }
}
