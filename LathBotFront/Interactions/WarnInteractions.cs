﻿using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotFront._2FA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarnModule;

namespace LathBotFront.Interactions
{
    public class WarnInteractions
    {
        [Command("Warn message")]
        [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task WarnMessage(SlashCommandContext ctx, DiscordMessage target)
        {
            await ctx.DeferResponseAsync(true);

            if (!ctx.Member.Permissions.HasFlag(DiscordPermission.KickMembers))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("No you dumbass!"));
                return;
            }

            WarnBuilder warnBuilder = new(
                ctx.Client,
                await ctx.Guild.GetChannelAsync(764251867135475713),
                ctx.Guild,
                ctx.Member,
                await ctx.Guild.GetMemberAsync(target.Author.Id),
                target);

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

            await WarnBuilder.ResetLastPunish(target.Author.Id);
        }

        [Command("Warn user")]
        [SlashCommandTypes(DiscordApplicationCommandType.UserContextMenu)]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task WarnUser(SlashCommandContext ctx, DiscordUser target)
        {
            await ctx.DeferResponseAsync(true);

            if (!ctx.Member.Permissions.HasFlag(DiscordPermission.KickMembers))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("No you dumbass!"));
                return;
            }

            WarnBuilder warnBuilder = new(
                ctx.Client,
                await ctx.Guild.GetChannelAsync(764251867135475713),
                ctx.Guild,
                ctx.Member,
                await ctx.Guild.GetMemberAsync(target.Id));

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
            await WarnBuilder.ResetLastPunish(target.Id);
        }

        [Command("mute"), Description("Mute a user")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task Mute(CommandContext ctx,
            [Parameter("Member"), Description("Who you want to mute")]
            DiscordUser user,
            [Parameter("Duration"), Description("When you want to be reminded tho unmute this member")]
            [SlashChoiceProvider<MuteDurationProvider>]
            int duration)
        {
            await ctx.DeferResponseAsync();

            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
                member = await ctx.Guild.GetMemberAsync(user.Id);

            if (member.Id == 192037157416730625)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("You can't mute Lathrix!"));
                return;
            }
            else if (ctx.Member.Hierarchy <= member.Hierarchy)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("You can't mute someone higher or same rank as you!"));
                return;
            }
            else if (member.Roles.Contains(await ctx.Guild.GetRoleAsync(701446136208293969)))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("User is already muted."));
                return;
            }
            if (!await this.AreYouSure(ctx, user, ctx.Client, "mute"))
                return;

            if (ctx.Member.Roles.Contains(await ctx.Guild.GetRoleAsync(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new()
                {
                    Color = ctx.Member.Color,
                    Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
                    Description = $"[Link to usage]({(await ctx.GetResponseAsync()).JumpLink})",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = ctx.Member.AvatarUrl,
                        Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
                    }
                };
                await (await ctx.Guild.GetChannelAsync(722905404354592900)).SendMessageAsync(discordEmbed);
            }

            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            MuteRepository mrepo = new(ReadConfig.Config.ConnectionString);
            AuditRepository repo = new(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(member.Id, out int id);
            urepo.GetIdByDcId(ctx.Member.Id, out int modId);
            mrepo.IsUserMuted(id, out bool exists);
            if (exists)
            {
                mrepo.GetMuteByUser(id, out Mute mute);
                mute.Mod = modId;
                mute.Duration = duration;
                mute.Timestamp = DateTime.Now;
                mrepo.Update(mute);
            }
            else
            {
                Mute mute = new()
                {
                    User = id,
                    Mod = modId,
                    Duration = duration,
                    Timestamp = DateTime.Now,
                };
                mrepo.Create(ref mute);
            }

            DiscordRole verificationRole = await ctx.Guild.GetRoleAsync(767050052257447936);
            DiscordRole mutedRole = await ctx.Guild.GetRoleAsync(701446136208293969);
            await member.RevokeRoleAsync(verificationRole);
            await member.GrantRoleAsync(mutedRole);

            repo.Read(modId, out Audit audit);
            audit.Mutes++;
            repo.Update(audit);

            await WarnBuilder.ResetLastPunish(member.Id);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Gray,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been muted",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            await ctx.EditResponseAsync(new DiscordMessageBuilder().WithContent($"Done!"));
            await (await ctx.Guild.GetChannelAsync(764251867135475713)).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await (await ctx.Guild.GetChannelAsync(722186358906421369)).SendMessageAsync($"{member.Mention}", embedBuilder);
        }

        [Command("unmute"), Description("Unmute a muted user")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task Unmute(CommandContext ctx,
            [Parameter("Member"), Description("Who you want to unmute")]
            DiscordUser user)
        {
            await ctx.DeferResponseAsync();

            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
                member = await ctx.Guild.GetMemberAsync(user.Id);

            if (ctx.Member.Roles.Contains(await ctx.Guild.GetRoleAsync(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new()
                {
                    Color = ctx.Member.Color,
                    Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
                    Description = $"[Link to usage]({(await ctx.GetResponseAsync()).JumpLink})",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = ctx.Member.AvatarUrl,
                        Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
                    }
                };
                await (await ctx.Guild.GetChannelAsync(722905404354592900)).SendMessageAsync(discordEmbed.Build());
            }

            IEnumerable<DiscordRole> roles = member.Roles;
            if ((!roles.Contains(await ctx.Guild.GetRoleAsync(701446136208293969))) || roles.Contains(await ctx.Guild.GetRoleAsync(767050052257447936)))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("User is not muted."));
                return;
            }

            if (!await this.AreYouSure(ctx, user, ctx.Client, "unmute"))
                return;

            DiscordRole verificationRole = await ctx.Guild.GetRoleAsync(767050052257447936);
            DiscordRole mutedRole = await ctx.Guild.GetRoleAsync(701446136208293969);
            await member.RevokeRoleAsync(mutedRole);
            await member.GrantRoleAsync(verificationRole);

            AuditRepository repo = new(ReadConfig.Config.ConnectionString);
            MuteRepository mrepo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);

            urepo.GetIdByDcId(member.Id, out int userId);
            mrepo.GetMuteByUser(userId, out Mute entity);
            mrepo.Delete(entity.Id);
            urepo.GetIdByDcId(ctx.Member.Id, out int id);
            repo.Read(id, out Audit audit);
            audit.Unmutes++;
            repo.Update(audit);

            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.White,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been unmuted",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            await ctx.EditResponseAsync(new DiscordMessageBuilder().WithContent($"Done!"));
            await (await ctx.Guild.GetChannelAsync(764251867135475713)).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await (await ctx.Guild.GetChannelAsync(722186358906421369)).SendMessageAsync($"{member.Mention}", embedBuilder);
        }

        [Command("check_muted"), Description("Check how long a user is muted for")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task CheckMuted(SlashCommandContext ctx,
            [Parameter("User"), Description("The user that you want to check")]
            DiscordUser user)
        {
            if (ctx.Channel.Id == 838088490704568341 || // muted
                ctx.Channel.Id == 724313826786410508 || // staff member comm
                ctx.Channel.ParentId != 700009634643050546) // staff category
                await ctx.DeferResponseAsync(true);
            else
                await ctx.DeferResponseAsync();

            var member = await ctx.Guild.GetMemberAsync(user.Id);

            if (!member.Roles.Contains(await ctx.Guild.GetRoleAsync(701446136208293969)))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Member is not even muted smh."));
                return;
            }

            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            MuteRepository repo = new(ReadConfig.Config.ConnectionString);

            urepo.GetIdByDcId(member.Id, out int id);
            repo.GetMuteByUser(id, out Mute entity);
            urepo.Read(entity.Mod, out User mod);
            var moderator = await ctx.Guild.GetMemberAsync(mod.DcID);

            await ctx.RespondAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Title = $"Muted user: {member.DisplayName}#{member.Discriminator} ({member.Id})",
                Description = $"Responsible moderator: {moderator.DisplayName}#{moderator.Discriminator} ({moderator.Id})",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = member.AvatarUrl
                },
                Color = DiscordColor.Gray,
            }
                .AddField("Muted at:", $"<t:{((DateTimeOffset)entity.Timestamp).ToUnixTimeSeconds()}:F>")
                .AddField("Proposed unmute time:", $"<t:{((DateTimeOffset)entity.Timestamp + TimeSpan.FromDays(entity.Duration)).ToUnixTimeSeconds()}:R> ({entity.Duration} days after mute)")));
        }

        [Command("muted"), Description("Check when you got muted")]
        public async Task Muted(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            if (!ctx.Member.Roles.Contains(await ctx.Guild.GetRoleAsync(701446136208293969)))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("You are not muted smh."));
                return;
            }
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            MuteRepository repo = new(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(ctx.Member.Id, out int id);
            repo.GetMuteByUser(id, out Mute entity);
            urepo.Read(entity.Mod, out User mod);
            DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);

            await ctx.RespondAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder
            {
                Title = $"Muted user: {ctx.Member.DisplayName}#{ctx.Member.Discriminator} ({ctx.Member.Id})",
                Description = $"Responsible moderator: {moderator.DisplayName}#{moderator.Discriminator} ({moderator.Id})",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = ctx.Member.AvatarUrl
                },
                Color = DiscordColor.Gray,
            }
                .AddField("Muted at:", $"<t:{((DateTimeOffset)entity.Timestamp).ToUnixTimeSeconds()}:F>")
                .AddField("Muted since: ", $"<t:{((DateTimeOffset)entity.Timestamp).ToUnixTimeSeconds()}:R>")
                .AddField("Earliest unmute date:", $"<t:{((DateTimeOffset)entity.Timestamp + TimeSpan.FromDays(2)).ToUnixTimeSeconds()}:R>")
                .AddField("Latest unmute date (without Senate decision for longer mute):", $"<t:{((DateTimeOffset)entity.Timestamp + TimeSpan.FromDays(14)).ToUnixTimeSeconds()}:R>")));
        }

        [Command("kick"), Description("Kick a user")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task Kick(CommandContext ctx,
            [Parameter("Member"), Description("Who you want to kick")]
            DiscordUser user)
        {
            await ctx.DeferResponseAsync();
            if (user.Id == 192037157416730625)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("You cant kick Lathrix!"));
                return;
            }
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            if (ctx.Member.Hierarchy <= member.Hierarchy)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("You cant kick someone higher or same rank as you!"));
                return;
            }
            if (await this.AreYouSure(ctx, user, ctx.Client, "kick"))
                return;
            await member.RemoveAsync();
            AuditRepository repo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(ctx.Member.Id, out int id);
            repo.Read(id, out Audit audit);
            audit.Kicks++;
            repo.Update(audit);
            await WarnBuilder.ResetLastPunish(user.Id);
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.DarkButNotBlack,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been kicked",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            await ctx.EditResponseAsync(new DiscordMessageBuilder().WithContent($"Done!"));
            await (await ctx.Guild.GetChannelAsync(764251867135475713)).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await (await ctx.Guild.GetChannelAsync(722186358906421369)).SendMessageAsync($"{member.Mention}", embedBuilder);
        }

        [Command("ban"), Description("Ban a user")]
        [RequirePermissions(DiscordPermission.BanMembers)]
        public async Task Ban(SlashCommandContext ctx,
            [Parameter("Member"), Description("Who you want to ban")]
            DiscordUser user,
            [Parameter("DeleteMessageDays"), Description("How many days of messages to remove (0-7)")]
            [SlashChoiceProvider<BanMessageDeletionProvider>]
            int deleteMessageDays,
            [Parameter("Reason"), Description("Why the user is being banned")]
            string reason)
        {
            if (user.Id == 192037157416730625)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("You cant ban Lathrix!"));
                return;
            }
            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
                member = await ctx.Guild.GetMemberAsync(user.Id);

            if (ctx.Member.Hierarchy <= member?.Hierarchy)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("You cant ban someone higher or same rank as you!"));
                return;
            }
            if (string.IsNullOrEmpty(reason))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Please provide a reason"));
                return;
            }
            if (!await this.Ensure2FA(ctx))
                return;
            if (!await this.AreYouSure(ctx, user, ctx.Client, "ban"))
                return;

            await ctx.Guild.BanMemberAsync(user, TimeSpan.FromDays((int)deleteMessageDays), reason);
            AuditRepository repo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(ctx.Member.Id, out int id);
            repo.Read(id, out Audit audit);
            audit.Bans++;
            repo.Update(audit);
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Black,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl },
                Title = $"{user.Username}#{user.Discriminator} ({user.Id}) has been banned",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" },
                Description = reason
            };
            await ctx.EditResponseAsync(new DiscordMessageBuilder().WithContent($"Done!"));
            await (await ctx.Guild.GetChannelAsync(764251867135475713)).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await (await ctx.Guild.GetChannelAsync(722186358906421369)).SendMessageAsync($"{user.Mention}", embedBuilder);
        }

        [Command("pardon"), Description("Pardon a warn of a user")]
        [RequirePermissions(DiscordPermission.BanMembers)]
        public async Task Pardon(CommandContext ctx,
            [Parameter("Member"), Description("The member you want to pardon a warn of")]
            DiscordUser user,
            [Parameter("Warn"), Description("The warn you want to pardon")]
            [SlashAutoCompleteProvider(typeof(Autocomplete.UserWarnAutocompleteProvider))]
            long warnNumber)
        {
            await ctx.DeferResponseAsync();

            WarnRepository repo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(user.Id, out int id);
            repo.GetWarnByUserAndNum(id, (int)warnNumber, out Warn warn);
            repo.Delete(warn.ID);
            repo.GetAllByUser(warn.User, out List<Warn> others);
            int counter = 0;
            foreach (var item in others)
            {
                counter++;
                item.Number = counter;
                repo.Update(item);
            }
            AuditRepository auditRepo = new(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(ctx.Member.Id, out int userid);
            auditRepo.Read(userid, out Audit audit);
            audit.Pardons++;
            auditRepo.Update(audit);
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Green,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl },
                Title = $"Pardoned warn number {warnNumber} of {user.Username}#{user.Discriminator} ({user.Id})",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = ctx.Member.DisplayName }
            };
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"Done!"));
            await (await ctx.Guild.GetChannelAsync(764251867135475713)).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await (await ctx.Guild.GetChannelAsync(722186358906421369)).SendMessageAsync($"{user.Mention}", embedBuilder);
        }

        [Command("warns"), Description("Check your or someone elses warnings")]
        public async Task Warns(CommandContext ctx,
            [Parameter("Member"), Description("The member that you want to check")]
            DiscordUser user = null)
        {
            await ctx.DeferResponseAsync();
            WarnRepository repo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(user is null ? ctx.Member.Id : user.Id, out int id);
            repo.GetAllByUser(id, out List<Warn> warns);
            urepo.Read(id, out var entity);
            DiscordEmbedBuilder embedBuilder = new()
            {
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user is null ? ctx.Member.AvatarUrl : user.AvatarUrl },
                Title = $"{(user is null ? "You have" : "The user has")} {warns.Count} warnings:",
                Description = entity.LastPunish is null ? null : $"Time of last punishment: {Formatter.Timestamp((DateTime)entity.LastPunish, TimestampFormat.ShortDateTime)}." +
                    $"{Environment.NewLine}This time is used for the expiration times!"
            };
            int pointsLeft = 15;
            foreach (Warn warn in warns)
            {
                urepo.Read(warn.Mod, out User mod);
                DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
                int severity = WarnBuilder.CalculateSeverity(warn.Level);
                string expiresAfter = warn.ExpirationTime is null ? severity switch
                {
                    1 => "14 days",
                    2 => "56 days",
                    3 => "never",
                    _ => "unknown"
                } : warn.ExpirationTime.ToString() + " days";
                if (warn.Persistent)
                    expiresAfter = "never";
                embedBuilder.AddField($"{warn.Number}: {warn.Reason}",
                    $"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator}; Expires after: {expiresAfter}");
                pointsLeft -= warn.Level;
            }
            embedBuilder.AddField($"{pointsLeft} points left", "­");

            if (pointsLeft == 15)
                embedBuilder.Color = DiscordColor.Green;
            else if (pointsLeft > 10)
                embedBuilder.Color = DiscordColor.Yellow;
            else if (pointsLeft > 5)
                embedBuilder.Color = DiscordColor.Orange;
            else
                embedBuilder.Color = DiscordColor.Red;

            await ctx.RespondAsync(embedBuilder);
        }

        [Command("report"), Description("Report a staff member to the senate.")]
        public async Task Report(SlashCommandContext ctx,
            [Parameter("Member"), Description("The staff member you want to report")]
            DiscordUser user)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            if (!member.Roles.Contains(await ctx.Guild.GetRoleAsync(796234634316873759)) &&
                !member.Roles.Contains(await ctx.Guild.GetRoleAsync(748646909354311751)))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder()
                    .WithContent("```User is not staff or is part of the Senate.\n" +
                        "If they are part of the Senate try messaging another member of the senate instead!```"));
                return;
            }
            var textInput = new DiscordTextInputComponent("Please state a reason for your report.",
                "report_reason",
                required: true,
                style: DiscordTextInputStyle.Paragraph);

            var responseBuilder = new DiscordInteractionResponseBuilder()
                .WithCustomId("report_reason")
                .WithTitle("Reason")
                .AddComponents(textInput);

            await ctx.RespondWithModalAsync(responseBuilder);

            InteractivityExtension interactivity = (InteractivityExtension)ctx.ServiceProvider.GetService(typeof(InteractivityExtension));
            var res = await interactivity.WaitForModalAsync("report_reason");

            await res.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral());
            var reason = res.Result.Values["report_reason"];

            if (reason is null)
            {
                await ctx.FollowupAsync("Please state a reason!", true);
                return;
            }

            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Red,
                Title = $"Report from user {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})",
                Description = $"Reported user: {member.Username}#{member.Discriminator} ({member.Id})",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.User.AvatarUrl, Text = ctx.User.Username }
            };

            embedBuilder.AddField("Reason:", reason);

            DiscordEmbed embed = embedBuilder.Build();
            var senate = ctx.Guild.GetAllMembersAsync().ToBlockingEnumerable().Where(x => x.Roles.Any(y => y.Id == 784852719449276467));
            foreach (DiscordMember senator in senate)
                await senator.SendMessageAsync(embed);

            await ctx.FollowupAsync("Report successfully sent. The senate will get back to you, until then please be patient.", true);
        }

        [Command("persist"), Description("Persist a warn of a user")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task Persist(CommandContext ctx,
            [Parameter("Member"), Description("The member you want to persist a warn of")]
            DiscordUser user,
            [Parameter("Warn"), Description("The warn you want to persist")]
            [SlashAutoCompleteProvider(typeof(Autocomplete.UserWarnAutocompleteProvider))]
            long warnNumber)
        {
            await ctx.DeferResponseAsync();

            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            WarnRepository repo = new(ReadConfig.Config.ConnectionString);

            urepo.GetIdByDcId(user.Id, out int userDbId);
            repo.GetWarnByUserAndNum(userDbId, (int)warnNumber, out Warn warn);
            if (warn.Level > 10)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Warn is already persistent by level"));
                return;
            }
            else if (warn.Persistent)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Warn is already persistent"));
                return;
            }

            warn.Persistent = true;
            repo.Update(warn);
            urepo.Read(warn.Mod, out User mod);

            await WarnBuilder.ResetLastPunish(user.Id);

            DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
            DiscordEmbedBuilder builder = new()
            {
                Title = $"Persistet warn {warn.ID}",
                Description = warn.Reason,
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = moderator.AvatarUrl,
                    Text = $"{moderator.DisplayName}#{moderator.Discriminator} ({moderator.Id})"
                }
            };
            builder.AddField("Level:", warn.Level.ToString());
            builder.AddField("Number:", warn.Number.ToString());
            builder.AddField("Time of the Warn:", warn.Time.ToString("yyyy-mm-ddTHH:mm:ss.ffff"));
            builder.AddField("Persistent:", warn.Persistent.ToString());
            await ctx.RespondAsync(new DiscordMessageBuilder().AddEmbed(builder));

            DiscordEmbedBuilder dmBuilder = new()
            {
                Title = "Persistent warns",
                Description = "One of your warns has been persistet and will no longer expire. The warn will only ever be removed after manual review by an Admin.",
                Color = DiscordColor.Red,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Member.AvatarUrl,
                    Text = $"{ctx.Member.DisplayName}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
                }
            };

            dmBuilder.AddField("Reason: ", warn.Reason);
            dmBuilder.AddField("Level:", warn.Level.ToString());
            dmBuilder.AddField("Number:", warn.Number.ToString());
            dmBuilder.AddField("Time of the Warn:", warn.Time.ToString("yyyy-mm-dd HH:mm:ss.ffff"));
            await ((DiscordMember)user).SendMessageAsync(dmBuilder);
        }

        private async Task<bool> Ensure2FA(SlashCommandContext ctx)
        {
            UserRepository userrepo = new(ReadConfig.Config.ConnectionString);
            ModRepository repo = new(ReadConfig.Config.ConnectionString);
            userrepo.GetIdByDcId(ctx.Member.Id, out int dbId);
            repo.GetModById(dbId, out Mod mod);

            if (mod.TwoFAKey is null || mod.TwoFAKey.Length <= 0)
            {
                await ctx.EditResponseAsync(new DiscordMessageBuilder().WithContent("Enable your 2FA, dumbass."));
                return false;
            }

            var twoFAKey = AesEncryption.DecryptStringToBytes(mod.TwoFAKey, mod.TwoFAKeySalt);

            var textInput = new DiscordTextInputComponent("Please input your 2FA Code.",
                "2famodal",
                placeholder: "000000",
                required: true,
                min_length: 6,
                max_length: 6
            );

            var responseBuilder = new DiscordInteractionResponseBuilder()
                .WithCustomId("2famodal")
                .WithTitle("2FA")
                .AddComponents(textInput);

            await ctx.RespondWithModalAsync(responseBuilder);

            InteractivityExtension interactivity = (InteractivityExtension)ctx.ServiceProvider.GetService(typeof(InteractivityExtension));
            var res = await interactivity.WaitForModalAsync("2famodal");

            await res.Result.Interaction.DeferAsync(true);
            var reason = res.Result.Values["2famodal"];

            if (reason is null)
            {
                await ctx.FollowupAsync("Please provide a 2FA code!", true);
                return false;
            }

            var pin = GoogleAuthenticator.GeneratePin(Encoding.UTF8.GetBytes(twoFAKey));
            if (reason != pin)
            {
                await ctx.FollowupAsync("Pin does not match!", true);
                return false;
            }

            return true;
        }

        private async Task<bool> AreYouSure(CommandContext ctx, DiscordUser user, DiscordClient client, string operation)
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
            List<DiscordComponent> components =
            [
                new DiscordButtonComponent(DiscordButtonStyle.Danger, "sure", "Yes I fucking am!"),
                new DiscordButtonComponent(DiscordButtonStyle.Secondary, "abort", "NO ABORT, ABORT!")
            ];
            builder.AddComponents(components);
            DiscordMessage message = await ctx.EditResponseAsync(builder);
            var interactivityResult = await message.WaitForButtonAsync(ctx.User, TimeSpan.FromMinutes(1));

            if (interactivityResult.Result.Id == "abort")
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Okay i will not {operation} the user."));
                return false;
            }
            return true;
        }
    }

    public class MuteDurationProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> durations =
        [
            new DiscordApplicationCommandOptionChoice("2 - Two days", 2),
            new DiscordApplicationCommandOptionChoice("3 - Three days", 3),
            new DiscordApplicationCommandOptionChoice("4 - Four days", 4),
            new DiscordApplicationCommandOptionChoice("5 - Five days", 5),
            new DiscordApplicationCommandOptionChoice("6 - Six days", 6),
            new DiscordApplicationCommandOptionChoice("7 - Seven days", 7),
            new DiscordApplicationCommandOptionChoice("8 - Eight days", 8),
            new DiscordApplicationCommandOptionChoice("9 - Nine days", 9),
            new DiscordApplicationCommandOptionChoice("10 - Ten days", 10),
            new DiscordApplicationCommandOptionChoice("11 - Eleven days", 11),
            new DiscordApplicationCommandOptionChoice("12 - Twelve days", 12),
            new DiscordApplicationCommandOptionChoice("13 - Thirteen days", 13),
            new DiscordApplicationCommandOptionChoice("14 - Fourteen days", 14)
        ];

        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
            => ValueTask.FromResult(durations.AsEnumerable());
    }

    public class BanMessageDeletionProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> deleteChoices =
        [
            new DiscordApplicationCommandOptionChoice("None - Dont delete any message", 0),
            new DiscordApplicationCommandOptionChoice("1 - Delete last one day of messages", 1),
            new DiscordApplicationCommandOptionChoice("2 - Delete last two days of messages", 2),
            new DiscordApplicationCommandOptionChoice("3 - Delete last three days of messages", 3),
            new DiscordApplicationCommandOptionChoice("4 - Delete last four days of messages", 4),
            new DiscordApplicationCommandOptionChoice("5 - Delete last five days of messages", 5),
            new DiscordApplicationCommandOptionChoice("6 - Delete last six days of messages", 6),
            new DiscordApplicationCommandOptionChoice("7 - Delete last seven days of messages", 7),
        ];

        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
            => ValueTask.FromResult(deleteChoices.AsEnumerable());
    }
}
