using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            await warnBuilder.SendPunishMessage();
            await WarnBuilder.ResetLastPunish(ctx.TargetMessage.Author.Id);
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
            await warnBuilder.SendPunishMessage();
            await WarnBuilder.ResetLastPunish(ctx.TargetUser.Id);
        }
        private async Task<bool> AreYouSure(BaseContext ctx, DiscordUser user, string operation)
        {
            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
                member = await ctx.Guild.GetMemberAsync(user.Id);

            DiscordWebhookBuilder builder = new DiscordWebhookBuilder()
                .AddEmbed(new DiscordEmbedBuilder
                {
                    Title = "Are you fucking sure about that?",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = user.AvatarUrl
                    },
                    Color = member == null ? new DiscordColor("#FF0000") : member.Color
                }
                .AddField("Member you selected:", member == null ? user.ToString() : member.ToString()));
            List<DiscordComponent> components = new List<DiscordComponent>
            {
                new DiscordButtonComponent(ButtonStyle.Danger, "sure", "Yes I fucking am!"),
                new DiscordButtonComponent(ButtonStyle.Secondary, "abort", "NO ABORT, ABORT!")
            };
            builder.AddComponents(components);
            DiscordMessage message = await ctx.EditResponseAsync(builder);
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            var interactivityResult = await interactivity.WaitForButtonAsync(message, ctx.User, TimeSpan.FromMinutes(1));

            if (interactivityResult.Result.Id == "abort")
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Okay i will not {operation} the user."));
                return false;
            }
            return true;
        }
    }
}
