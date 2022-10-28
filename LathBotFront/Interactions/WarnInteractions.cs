﻿using DSharpPlus;
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
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Warn message")]
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

        [ContextMenu(ApplicationCommandType.UserContextMenu, "Warn user")]
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

        [SlashCommand("Mute", "Mute a user")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task Mute(InteractionContext ctx,
            [Option("Member", "Who you want to mute")]
            DiscordUser user,
            [Option("Duration", "When you want to be reminded tho unmute this member")]
            DurationChoices duration)
        {
            await ctx.DeferAsync();

            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
                member = await ctx.Guild.GetMemberAsync(user.Id);

            if (member.Id == 192037157416730625)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You can't mute Lathrix!"));
                return;
            }
            else if (ctx.Member.Hierarchy <= member.Hierarchy)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You can't mute someone higher or same rank as you!"));
                return;
            }
            else if (member.Roles.Contains(ctx.Guild.GetRole(701446136208293969)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("User is already muted."));
                return;
            }
            if (!await AreYouSure(ctx, member, "mute"))
                return;

            if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
                {
                    Color = ctx.Member.Color,
                    Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
                    Description = $"[Link to usage]({(await ctx.GetOriginalResponseAsync()).JumpLink})",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = ctx.Member.AvatarUrl,
                        Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
                    }
                };
                await ctx.Guild.GetChannel(722905404354592900).SendMessageAsync(discordEmbed);
            }

            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            MuteRepository mrepo = new MuteRepository(ReadConfig.Config.ConnectionString);
            AuditRepository repo = new AuditRepository(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(member.Id, out int id);
            urepo.GetIdByDcId(ctx.Member.Id, out int modId);
            mrepo.IsUserMuted(id, out bool exists);
            if (exists)
            {
                mrepo.GetMuteByUser(id, out Mute mute);
                mute.Mod = modId;
                mute.Duration = (int)duration;
                mute.Timestamp = DateTime.Now;
                mrepo.Update(mute);
            }
            else
            {
                Mute mute = new Mute
                {
                    User = id,
                    Mod = modId,
                    Duration = (int)duration,
                    Timestamp = DateTime.Now,
                };
                mrepo.Create(ref mute);
            }

            DiscordRole verificationRole = ctx.Guild.GetRole(767050052257447936);
            DiscordRole mutedRole = ctx.Guild.GetRole(701446136208293969);
            await member.RevokeRoleAsync(verificationRole);
            await member.GrantRoleAsync(mutedRole);

            repo.Read(modId, out Audit audit);
            audit.Mutes++;
            repo.Update(audit);

            await WarnBuilder.ResetLastPunish(member.Id);

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gray,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been muted",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.Member.Mention}").AddEmbed(embed));
            DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
            await warnsChannel.SendMessageAsync($"{member.Mention}", embed);
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

    public enum DurationChoices
    {
        [ChoiceName("2 - Two days")]
        TwoDays = 2,
        [ChoiceName("3 - Three days")]
        ThreeDays = 3,
        [ChoiceName("4 - Four days")]
        FourDays = 4,
        [ChoiceName("5 - Five days")]
        FiveDays = 5,
        [ChoiceName("6 - Six days")]
        SixDays = 6,
        [ChoiceName("7 - Seven days")]
        SevenDays = 7,
        [ChoiceName("8 - Eight days")]
        EightDays = 8,
        [ChoiceName("9 - Nine days")]
        NineDays = 9,
        [ChoiceName("10 - Ten days")]
        TenDays = 10,
        [ChoiceName("11 - Eleven days")]
        ElevenDays = 11,
        [ChoiceName("12 - Twelve days")]
        TwelveDays = 12,
        [ChoiceName("13 - Thirteen days")]
        ThirteenDays = 13,
        [ChoiceName("14 - Fourteen days")]
        FourteenDays = 14
    }
}
