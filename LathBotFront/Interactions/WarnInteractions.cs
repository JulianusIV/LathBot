using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

using WarnModule;

namespace LathBotFront.Interactions
{
    public class WarnInteractions : ApplicationCommandModule
    {
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Warn Message")]
        [SlashRequireOwner]
        public async Task WarnMessage(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);

            WarnBuilder warnBuilder = new WarnBuilder(
                ctx.Client,
                ctx.Guild.GetChannel(512370308976607250),
                ctx.Guild,
                ctx.Member,
                await ctx.Guild.GetMemberAsync(ctx.TargetMessage.Author.Id),
                ctx.TargetMessage);

            if (!await warnBuilder.PreExecutionChecks())
                return;
            await warnBuilder.RequestRule(ctx);
            await warnBuilder.RequestPoints();
            await warnBuilder.RequestReason();
            if (!await warnBuilder.WriteToDatabase())
                return;
            if (!await warnBuilder.WriteAuditToDb())
                return;
            await warnBuilder.ReadRemainingPoints();
            await warnBuilder.SendWarnMessage();
            if (!(warnBuilder.MessageLink is null))
                await warnBuilder.LogMessage();
            await warnBuilder.SendPunishMessage();
        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "Warn User")]
        [SlashRequireOwner]
        public async Task WarnUser(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);

            WarnBuilder warnBuilder = new WarnBuilder(
                ctx.Client,
                ctx.Guild.GetChannel(512370308976607250),
                ctx.Guild,
                ctx.Member,
                ctx.TargetMember);

            if (!await warnBuilder.PreExecutionChecks())
                return;
            await warnBuilder.RequestRule(ctx);
            await warnBuilder.RequestPoints();
            await warnBuilder.RequestReason();
            if (!await warnBuilder.WriteToDatabase())
                return;
            if (!await warnBuilder.WriteAuditToDb())
                return;
            await warnBuilder.ReadRemainingPoints();
            await warnBuilder.SendWarnMessage();
            if (!(warnBuilder.MessageLink is null))
                await warnBuilder.LogMessage();
            await warnBuilder.SendPunishMessage();
        }
    }
}
