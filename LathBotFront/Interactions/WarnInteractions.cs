using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotFront._2FA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarnModule;

namespace LathBotFront.Interactions
{
    public class WarnInteractions : ApplicationCommandModule
    {
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Warn message")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task WarnMessage(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);

            if (!ctx.Member.Permissions.HasFlag(Permissions.KickMembers))
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("No you dumbass!"));
                return;
            }

            WarnBuilder warnBuilder = new(
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
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task WarnUser(ContextMenuContext ctx)
        {
            await ctx.DeferAsync(true);

            if (!ctx.Member.Permissions.HasFlag(Permissions.KickMembers))
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("No you dumbass!"));
                return;
            }

            WarnBuilder warnBuilder = new(
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
            MuteDurationChoices duration)
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
            if (!await AreYouSure(ctx.Interaction, user, ctx.Client, "mute"))
                return;

            if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new()
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
                mute.Duration = (int)duration;
                mute.Timestamp = DateTime.Now;
                mrepo.Update(mute);
            }
            else
            {
                Mute mute = new()
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

            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Gray,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been muted",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Done!"));
            await ctx.Guild.GetChannel(764251867135475713).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await ctx.Guild.GetChannel(722186358906421369).SendMessageAsync($"{member.Mention}", embedBuilder);
        }

        [SlashCommand("Unmute", "Unmute a muted user")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task Unmute(InteractionContext ctx,
            [Option("Member", "Who you want to unmute")]
            DiscordUser user)
        {
            await ctx.DeferAsync();

            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
                member = await ctx.Guild.GetMemberAsync(user.Id);

            if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new()
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
                await ctx.Guild.GetChannel(722905404354592900).SendMessageAsync(discordEmbed.Build());
            }

            IEnumerable<DiscordRole> roles = member.Roles;
            if ((!roles.Contains(ctx.Guild.GetRole(701446136208293969))) || (roles.Contains(ctx.Guild.GetRole(767050052257447936))))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("User is not muted."));
                return;
            }

            if (!await AreYouSure(ctx.Interaction, user, ctx.Client, "unmute"))
                return;

            DiscordRole verificationRole = ctx.Guild.GetRole(767050052257447936);
            DiscordRole mutedRole = ctx.Guild.GetRole(701446136208293969);
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
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Done!"));
            await ctx.Guild.GetChannel(764251867135475713).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await ctx.Guild.GetChannel(722186358906421369).SendMessageAsync($"{member.Mention}", embedBuilder);
        }

        [SlashCommand("CheckMuted", "Check how long a user is muted for")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task CheckMuted(InteractionContext ctx,
            [Option("User", "The user that you want to check")]
            DiscordUser user)
        {
            if (ctx.Channel.ParentId != 700009634643050546 &&
                ctx.Channel.Id != 838088490704568341 &&
                ctx.Channel.Id != 724313826786410508)
                await ctx.DeferAsync(true);
            else
                await ctx.DeferAsync();

            var member = await ctx.Guild.GetMemberAsync(user.Id);

            if (!member.Roles.Contains(ctx.Guild.GetRole(701446136208293969)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Member is not even muted smh."));
                return;
            }

            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            MuteRepository repo = new(ReadConfig.Config.ConnectionString);

            urepo.GetIdByDcId(member.Id, out int id);
            repo.GetMuteByUser(id, out Mute entity);
            urepo.Read(entity.Mod, out User mod);
            var moderator = await ctx.Guild.GetMemberAsync(mod.DcID);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
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

        [SlashCommand("Muted", "Check when you got muted")]
        public async Task Muted(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            if (!ctx.Member.Roles.Contains(ctx.Guild.GetRole(701446136208293969)))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You are not muted smh."));
                return;
            }
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            MuteRepository repo = new(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(ctx.Member.Id, out int id);
            repo.GetMuteByUser(id, out Mute entity);
            urepo.Read(entity.Mod, out User mod);
            DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder
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

        [SlashCommand("Kick", "Kick a user")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task Kick(InteractionContext ctx,
            [Option("Member", "Who you want to kick")]
            DiscordUser user)
        {
            await ctx.DeferAsync();
            if (user.Id == 192037157416730625)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You cant kick Lathrix!"));
                return;
            }
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            if (ctx.Member.Hierarchy <= member.Hierarchy)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You cant kick someone higher or same rank as you!"));
                return;
            }
            if (await AreYouSure(ctx.Interaction, user, ctx.Client, "kick"))
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
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Done!"));
            await ctx.Guild.GetChannel(764251867135475713).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await ctx.Guild.GetChannel(722186358906421369).SendMessageAsync($"{member.Mention}", embedBuilder);
        }

        [SlashCommand("Ban", "Ban a user")]
        [SlashCommandPermissions(Permissions.BanMembers)]
        public async Task Ban(InteractionContext ctx,
            [Option("Member", "Who you want to ban")]
            DiscordUser user,
            [Option("DeleteMessageDays", "How many days of messages to remove (0-7)")]
            BanMessageDeletionChoices deleteMessageDays,
            [Option("Reason", "Why the user is being banned")]
            string reason)
        {
            if (user.Id == 192037157416730625)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You cant ban Lathrix!"));
                return;
            }
            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
                member = await ctx.Guild.GetMemberAsync(user.Id);

            if (ctx.Member.Hierarchy <= member?.Hierarchy)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You cant ban someone higher or same rank as you!"));
                return;
            }
            if (string.IsNullOrEmpty(reason))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Please provide a reason"));
                return;
            }
            var result = await Ensure2FA(ctx);
            if (!result.Item1)
                return;
            if (!await AreYouSure(result.Item2, user, ctx.Client, "ban"))
                return;

            await ctx.Guild.BanMemberAsync(user.Id, (int)deleteMessageDays, reason);
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
            await result.Item2.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"Done!"));
            await ctx.Guild.GetChannel(764251867135475713).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await ctx.Guild.GetChannel(722186358906421369).SendMessageAsync($"{user.Mention}", embedBuilder);
        }

        [SlashCommand("Pardon", "Pardon a warn of a user")]
        [SlashCommandPermissions(Permissions.BanMembers)]
        public async Task Pardon(InteractionContext ctx,
            [Option("Member", "The member you want to pardon a warn of")]
            DiscordUser user,
            [Option("Warn", "The warn you want to pardon", true)]
            [Autocomplete(typeof(Autocomplete.UserWarnAutocompleteProvider))]
            long warnNumber)
        {
            await ctx.DeferAsync();

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
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Done!"));
            await ctx.Guild.GetChannel(764251867135475713).SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embedBuilder));
            await ctx.Guild.GetChannel(722186358906421369).SendMessageAsync($"{user.Mention}", embedBuilder);
        }

        [SlashCommand("Warns", "Check your or someone elses warnings")]
        public async Task Warns(InteractionContext ctx,
            [Option("Member", "The member that you want to check")]
            DiscordUser user = null)
        {
            await ctx.DeferAsync();
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

            var webhook = new DiscordWebhookBuilder().AddEmbed(embedBuilder);
            if (user is null)
                webhook.WithContent(ctx.Member.Mention);
            await ctx.EditResponseAsync(webhook);
        }

        [SlashCommand("Report", "Report a staff member to the senate.")]
        public async Task Report(InteractionContext ctx,
            [Option("Member", "The staff member you want to report")]
            DiscordUser user)
        {
            var member = await ctx.Guild.GetMemberAsync(user.Id);
            if (!member.Roles.Contains(ctx.Guild.GetRole(796234634316873759)) && !member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("```User is not staff or is part of the Senate.\n" + "If they are part of the Senate try messaging another member of the senate instead!```").AsEphemeral());
                return;
            }
            var textInput = new TextInputComponent("Please state a reason for your report.",
                "report_reason",
                required: true,
                style: TextInputStyle.Paragraph);

            var responseBuilder = new DiscordInteractionResponseBuilder()
                .WithCustomId("report_reason")
                .WithTitle("Reason")
                .AddComponents(textInput);

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);

            var res = await ctx.Client.GetInteractivity().WaitForModalAsync("report_reason");

            await res.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate, new DiscordInteractionResponseBuilder().AsEphemeral());
            var reason = res.Result.Values["report_reason"];

            if (reason is null)
            {
                await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Please state a reason!"));
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
            var senate = (await ctx.Guild.GetAllMembersAsync()).Where(x => x.Roles.Any(y => y.Id == 784852719449276467));
            foreach (DiscordMember senator in senate)
                await senator.SendMessageAsync(embed);

            await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Report successfully sent. The senate will get back to you, until then please be patient."));
        }

        [SlashCommand("Persist", "Persist a warn of a user")]
        [SlashCommandPermissions(Permissions.Administrator)]
        public async Task Persist(InteractionContext ctx,
            [Option("Member", "The member you want to persist a warn of")]
            DiscordUser user,
            [Option("Warn", "The warn you want to persist", true)]
            [Autocomplete(typeof(Autocomplete.UserWarnAutocompleteProvider))]
            long warnNumber)
        {
            await ctx.DeferAsync();

            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            WarnRepository repo = new(ReadConfig.Config.ConnectionString);

            urepo.GetIdByDcId(user.Id, out int userDbId);
            repo.GetWarnByUserAndNum(userDbId, (int)warnNumber, out Warn warn);
            if (warn.Level > 10)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Warn is already persistent by level"));
                return;
            }
            else if (warn.Persistent)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Warn is already persistent"));
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
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(builder));

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

        private async Task<(bool, DiscordInteraction)> Ensure2FA(BaseContext ctx)
        {
            UserRepository userrepo = new(ReadConfig.Config.ConnectionString);
            ModRepository repo = new(ReadConfig.Config.ConnectionString);
            userrepo.GetIdByDcId(ctx.Member.Id, out int dbId);
            repo.GetModById(dbId, out Mod mod);

            if (mod.TwoFAKey is null || mod.TwoFAKey.Length <= 0)
                return (true, ctx.Interaction);

            var twoFAKey = AesEncryption.DecryptStringToBytes(mod.TwoFAKey, mod.TwoFAKeySalt);

            var textInput = new TextInputComponent("Please input your 2FA Code.",
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

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, responseBuilder);

            var res = await ctx.Client.GetInteractivity().WaitForModalAsync("2famodal");

            await res.Result.Interaction.DeferAsync(true);
            var reason = res.Result.Values["2famodal"];

            if (reason is null)
            {
                await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Please provide a 2FA code!"));
                return (false, null);
            }

            var pin = GoogleAuthenticator.GeneratePin(Encoding.UTF8.GetBytes(twoFAKey));
            if (reason != pin)
            {
                await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Pin does not match!"));
                return (false, null);
            }

            return (true, res.Result.Interaction);
        }

        private async Task<bool> AreYouSure(DiscordInteraction ctx, DiscordUser user, DiscordClient client, string operation)
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
            List<DiscordComponent> components = new()
            {
                new DiscordButtonComponent(ButtonStyle.Danger, "sure", "Yes I fucking am!"),
                new DiscordButtonComponent(ButtonStyle.Secondary, "abort", "NO ABORT, ABORT!")
            };
            builder.AddComponents(components);
            DiscordMessage message = await ctx.EditOriginalResponseAsync(builder);
            InteractivityExtension interactivity = client.GetInteractivity();
            var interactivityResult = await interactivity.WaitForButtonAsync(message, ctx.User, TimeSpan.FromMinutes(1));

            if (interactivityResult.Result.Id == "abort")
            {
                await ctx.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"Okay i will not {operation} the user."));
                return false;
            }
            return true;
        }
    }

    public enum MuteDurationChoices
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

    public enum BanMessageDeletionChoices
    {
        [ChoiceName("None - Dont delete any messages")]
        None = 0,
        [ChoiceName("1 - Delete last one day of messages")]
        OneDay = 1,
        [ChoiceName("2 - Delete last two days of messages")]
        TwoDays = 2,
        [ChoiceName("3 - Delete last three days of messages")]
        ThreeDays = 3,
        [ChoiceName("4 - Delete last four days of messages")]
        FourDays = 4,
        [ChoiceName("5 - Delete last five days of messages")]
        FiveDays = 5,
        [ChoiceName("6 - Delete last six days of messages")]
        SixDays = 6,
        [ChoiceName("7 - Delete last seven days of messages")]
        SevenDays = 7
    }
}
