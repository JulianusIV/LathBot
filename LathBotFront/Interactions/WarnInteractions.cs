using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

using WarnModule;

namespace LathBotFront.Interactions
{
    public class WarnInteractions : ApplicationCommandModule
    {
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Warn Message")]
        public async Task WarnMessage(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);

            if (!ctx.Member.Permissions.HasFlag(Permissions.KickMembers))
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("No you dumbass!"));
                return;
            }

            WarnBuilder warnBuilder = new WarnBuilder(
                ctx.Client,
                ctx.Guild.GetChannel(764251867135475713),
                ctx.Guild,
                ctx.Member,
                await ctx.Guild.GetMemberAsync(ctx.TargetMessage.Author.Id),
                ctx.TargetMessage);

            if (!await warnBuilder.PreExecutionChecks())
                return;
            var id = await warnBuilder.RequestRuleEphemeral(ctx);
            var interaction = await warnBuilder.RequestPointsEphemeral(ctx, id);
            await warnBuilder.RequestReasonEphemeral(ctx, interaction);
            if (!await warnBuilder.WriteToDatabase())
                return;
            if (!await warnBuilder.WriteAuditToDb())
                return;
            await warnBuilder.ReadRemainingPoints();
            await warnBuilder.SendWarnMessage();
            await warnBuilder.LogMessage();
            await warnBuilder.SendPunishMessageEphemeral(ctx, id);
        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "Warn User")]
        public async Task WarnUser(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);

            if (!ctx.Member.Permissions.HasFlag(Permissions.KickMembers))
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("No you dumbass!"));
                return;
            }

            WarnBuilder warnBuilder = new WarnBuilder(
                ctx.Client,
                ctx.Guild.GetChannel(764251867135475713),
                ctx.Guild,
                ctx.Member,
                ctx.TargetMember);

            if (!await warnBuilder.PreExecutionChecks())
                return;
            var id = await warnBuilder.RequestRuleEphemeral(ctx);
            var interaction = await warnBuilder.RequestPointsEphemeral(ctx, id);
            await warnBuilder.RequestReasonEphemeral(ctx, interaction);
            if (!await warnBuilder.WriteToDatabase())
                return;
            if (!await warnBuilder.WriteAuditToDb())
                return;
            await warnBuilder.ReadRemainingPoints();
            await warnBuilder.SendWarnMessage();
            await warnBuilder.SendPunishMessageEphemeral(ctx, id);
        }
    }
}
