using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using LathBotFront._2FA;
using LathBotFront.Commands.ChoiceProviders;
using LathBotFront.EventHandlers;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using WarnModule;

namespace LathBotFront.Commands
{
    public class WarnCommands
    {
        [Command("initdb")]
        [Description("Fill empty Database with users")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task InitDB(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            if (ctx.User.Id != DiscordObjectService.Instance.Owner)
                return;
            var members = ctx.Guild.GetAllMembersAsync();
            await foreach (DiscordMember member in members)
            {
                User user = new()
                {
                    DcID = member.Id
                };
                UserRepository repo = new(ReadConfig.Config.ConnectionString);

                bool success = repo.Create(ref user);
                if (!success)
                {
                    await ctx.RespondAsync($"Error creating user {member.Nickname}#{member.Discriminator} ({member.Id})!");
                }
            }
            await ctx.RespondAsync("Done! (maybe?)");
        }

        [Command("updatedb")]
        [Description("Fill remaining users in database")]
        public async Task UpdateDB(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            if (ctx.User.Id != DiscordObjectService.Instance.Owner)
                return;
            var members = ctx.Guild.GetAllMembersAsync().ToBlockingEnumerable();
            int count = 0;
            UserRepository repo = new(ReadConfig.Config.ConnectionString);
            foreach (DiscordMember member in members)
            {

                bool success = repo.ExistsDcId(member.Id, out bool exists);
                if (!success)
                {
                    DiscordMember mem = await ctx.Guild.GetMemberAsync(member.Id);
                    await ctx.RespondAsync($"Error checking user {member.Nickname}#{member.Discriminator} ({member.Id})!");
                }
                else if (!exists)
                {
                    User user = new()
                    {
                        DcID = member.Id
                    };
                    bool result = repo.Create(ref user);
                    if (!result)
                    {
                        DiscordMember mem = await ctx.Guild.GetMemberAsync(member.Id);
                        await ctx.RespondAsync($"Error creating user {member.Nickname}#{member.Discriminator} ({member.Id})!");
                    }
                    else
                    {
                        DiscordMember mem = await ctx.Guild.GetMemberAsync(member.Id);
                        await ctx.RespondAsync($"Added user {member.DisplayName}#{mem.Discriminator} ({mem.Id})");
                        count++;
                    }
                }
            }
            bool res = repo.CountAll(out long amount);
            if (!res)
            {
                await ctx.RespondAsync("Error counting entries in database");
                await ctx.RespondAsync($"Added {count} users, {members.Count()} members");
            }
            else
            {
                await ctx.RespondAsync($"Added {count} users, {members.Count()} members, {amount} entries");
            }
        }

        [Command("allwarns")]
        [Description("Display all currently unpardoned warns")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task AllWarns(CommandContext ctx)
        {
            if (ctx is SlashCommandContext slashContext)
                await slashContext.RespondAsync("Working on it...", true);

            WarnRepository repo = new(ReadConfig.Config.ConnectionString);
            bool result = repo.GetAll(out List<Warn> warns);
            if (!result)
            {
                await ctx.RespondAsync("There has been an error reading the warns from the database");
                return;
            }

            int index = 0;
            int indicator = 0;
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            List<Page> pages = [];
            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = $"Showing all warnings in the server",
                Color = ctx.Guild.GetMemberAsync(DiscordObjectService.Instance.Lathrix).Result.Color.PrimaryColor,
                Description = "Use -warns <User> to get more information on a specific user"
            };
            foreach (Warn warn in warns)
            {
                index++;
                indicator++;
                if (indicator == 10 || index == warns.Count)
                {
                    result = urepo.Read(warn.Mod, out User mod);
                    if (!result)
                    {
                        await ctx.RespondAsync("There was an error getting a user from the database");
                        continue;
                    }
                    DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
                    try
                    {
                        result = urepo.Read(warn.Mod, out User user);
                        if (!result)
                        {
                            await ctx.RespondAsync("There was an error getting a user from the database");
                            continue;
                        }
                        DiscordMember member = await ctx.Guild.GetMemberAsync(user.DcID);
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
                        embedBuilder.AddField($"{index}: {warn.Reason}",
                            $"Warned user: {member.DisplayName}#{member.Discriminator}\n" +
                            $"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator} Expires after: {expiresAfter}");
                    }
                    catch
                    {
                        result = urepo.Read(warn.Mod, out User user);
                        if (!result)
                        {
                            await ctx.RespondAsync("There was an error getting a user from the database");
                            continue;
                        }
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
                        embedBuilder.AddField($"{index}: {warn.Reason}",
                            $"Warned user: {user.DcID}\n" +
                            $"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator} Expires after: {expiresAfter}");
                    }

                    pages.Add(new Page { Embed = embedBuilder.Build() });
                    embedBuilder = new DiscordEmbedBuilder
                    {
                        Title = $"Showing all warnings in the server",
                        Color = ctx.Guild.GetMemberAsync(DiscordObjectService.Instance.Lathrix).Result.Color.PrimaryColor,
                        Description = "Use -warns <User> to get more information on a specific user"
                    };
                }
                else
                {
                    result = urepo.Read(warn.Mod, out User mod);
                    if (!result)
                    {
                        await ctx.RespondAsync("There was an error getting a user from the database");
                        continue;
                    }
                    DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
                    try
                    {
                        result = urepo.Read(warn.Mod, out User user);
                        if (!result)
                        {
                            await ctx.RespondAsync("There was an error getting a user from the database");
                            continue;
                        }
                        DiscordMember member = await ctx.Guild.GetMemberAsync(user.DcID);
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
                        embedBuilder.AddField($"{index}: {warn.Reason}",
                            $"Warned user: {member.DisplayName}#{member.Discriminator}\n" +
                            $"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator} Expires after: {expiresAfter}");
                    }
                    catch
                    {
                        result = urepo.Read(warn.Mod, out User user);
                        if (!result)
                        {
                            await ctx.RespondAsync("There was an error getting a user from the database");
                            continue;
                        }
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
                        embedBuilder.AddField($"{index}: {warn.Reason}",
                            $"Warned user: {user.DcID}\n" +
                            $"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator} Expires after: {expiresAfter}");
                    }
                }
            }
            _ = ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages, PaginationBehaviour.WrapAround, ButtonPaginationBehavior.DeleteMessage);
        }

        [Command("sql")]
        [Description("Send a SQL Query to the database")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task SqlQuery(CommandContext ctx, [Description("Query")][RemainingText] string command)
        {
            await ctx.DeferResponseAsync();

            MySqlConnection connection = new(ReadConfig.Config.ConnectionString);
            try
            {
                if (ctx.Member.Id != DiscordObjectService.Instance.Owner)
                    return;
                command = command.Trim();
                MySqlCommand cmd = new(command, connection);
                string resultString = "";
                connection.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int index = 0; index < reader.FieldCount; index++)
                        resultString += reader.GetValue(index).ToString() + " ";
                    resultString = resultString.Trim();
                    if (resultString.Length >= 1900)
                    {
                        await ctx.Channel.SendMessageAsync("```d\n" + resultString + "\n```");
                        await ctx.Channel.TriggerTypingAsync();
                        resultString = "";
                    }
                    else
                        resultString += "\n";
                }
                reader.Close();
                connection.Close();
                if (!string.IsNullOrEmpty(resultString))
                    await ctx.Channel.SendMessageAsync("```c\n" + resultString + "\n```");
                await ctx.RespondAsync("Query Done!");
            }
            catch (Exception e)
            {
                await ctx.RespondAsync(e.Message);
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        [Command("Warn message")]
        [SlashCommandTypes(DiscordApplicationCommandType.MessageContextMenu)]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task WarnMessage(SlashCommandContext ctx, DiscordMessage target)
        {
            if (target.Author.Id == DiscordObjectService.Instance.Lathrix)
            {
                await ctx.RespondAsync("You can't warn Lathrix!");
                return;
            }

            DiscordModalBuilder modal = RuleService.Instance.WarnModal;
            modal.WithTitle($"Warn {target.Author.GlobalName}");
            ValueTask modalTask = ctx.RespondWithModalAsync(modal);
            ValueTask<DiscordMember> memberTask = this.PKGet(ctx.Guild, target.Author.IsBot, target.Id, target.Author.Id);

            InteractivityExtension interactivity = ctx.Client.ServiceProvider.GetService(typeof(InteractivityExtension)) as InteractivityExtension;
            await modalTask;
            var result = await interactivity.WaitForModalAsync("warnModal", ctx.User);
            if (result.TimedOut)
                return;
            await result.Result.Interaction.DeferAsync(true);

            string ruleNum = (result.Result.Values["rule"] as SelectMenuModalSubmission).Values[0];
            Rule rule = RuleService.Rules.Single(x => x.RuleNum.ToString() == ruleNum);
            int points = int.Parse((result.Result.Values["warnamount"] as SelectMenuModalSubmission).Values[0]);
            string reason = (result.Result.Values["reason"] as TextInputModalSubmission).Value;
            int severity = ((points - 1) / 5) + 1;

            DiscordMember member = await memberTask;
            int dbResult = await this.DbOps(member, ctx.Member, rule, reason, points, result);
            if (dbResult < 0)
                return;

            new WarnRepository(ReadConfig.Config.ConnectionString).GetRemainingPoints(dbResult, out int remaining);

            DiscordEmbedBuilder warnEmbed = new()
            {
                Color = severity switch
                {
                    1 => DiscordColor.Yellow,
                    2 => DiscordColor.Orange,
                    3 => DiscordColor.Red,
                    _ => DiscordColor.Black,
                },
                Thumbnail = new()
                {
                    Height = 8,
                    Width = 8,
                    Url = member.AvatarUrl,
                },
                Title = $"{member.DisplayName} ({member.Id}) has been warned for Rule {ruleNum}:",
                Description = $"{rule.RuleText}\n" +
                    "\n" +
                    $"{reason}",
                Footer = new()
                {
                    IconUrl = ctx.Member.AvatarUrl,
                    Text = ctx.Member.DisplayName
                }
            };
            warnEmbed.AddField($"{remaining} points remaining", "Please keep any talk of this to DM's");
            DiscordChannel warnsChannel = await ctx.Guild.GetChannelAsync(DiscordObjectService.Instance.WarnsChannel.Id);
            await warnsChannel.SendMessageAsync(member.Mention, warnEmbed);

            DiscordEmbedBuilder logEmbed = new()
            {
                Color = DiscordColor.Yellow,
                Title = $"Successfully warned {member.DisplayName} ({member.Id})",
                Description = $"Rule {rule.RuleNum}:\n" +
                    "\n" +
                    $"{reason}" +
                    "\n" +
                    $"User has {remaining} points left.",
                Footer = new()
                {
                    IconUrl = ctx.Member.AvatarUrl,
                    Text = ctx.Member.DisplayName
                }
            };

            DiscordChannel warnLogChannel = await ctx.Guild.GetChannelAsync(DiscordObjectService.Instance.WarnLogChannel.Id);
            await warnLogChannel.SendMessageAsync(logEmbed);

            DiscordEmbedBuilder messageLogEmbed = new()
            {
                Author = new()
                {
                    IconUrl = member.AvatarUrl,
                    Name = member.Username
                },
                Description = target.Content,
                Color = member.Color.PrimaryColor
            };
            if (target.Attachments is not null && target.Attachments.Any())
            {
                DiscordMessageBuilder messageLog = new();
                Dictionary<string, Stream> attachments = [];
                foreach (var attachment in target.Attachments)
                {
                    using HttpClient httpClient = new();

                    attachments.Add(attachment.FileName, await httpClient.GetStreamAsync(attachment.Url));
                    if (attachment.MediaType.Contains("image") && string.IsNullOrEmpty(messageLogEmbed.ImageUrl))
                        messageLogEmbed.WithImageUrl("attachment://" + attachment.FileName);
                }
                messageLog.AddFiles(attachments);
                await warnLogChannel.SendMessageAsync(messageLog.AddEmbed(messageLogEmbed).WithAllowedMentions(Mentions.None));
                foreach (var attachment in attachments)
                    attachment.Value.Close();
            }
            else
                await warnLogChannel.SendMessageAsync(messageLogEmbed);
            await target.DeleteAsync();

            if (remaining < 11)
                await result.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.Member.Mention} User has {remaining} points left.\n" +
                    $"By common practice the user should be muted{(remaining < 6 ? ", kicked" : "")}{(remaining < 1 ? ", or banned" : "")}."));
            else
                await result.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Done!"));
        }

        [Command("Warn user")]
        [SlashCommandTypes(DiscordApplicationCommandType.UserContextMenu)]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task WarnUser(SlashCommandContext ctx, DiscordUser target)
        {
            if (target.Id == DiscordObjectService.Instance.Lathrix)
            {
                await ctx.RespondAsync("You can't warn Lathrix!");
                return;
            }
            DiscordModalBuilder modal = RuleService.Instance.WarnModal;
            modal.WithTitle($"Warn {target.GlobalName}");
            await ctx.RespondWithModalAsync(modal);

            InteractivityExtension interactivity = ctx.Client.ServiceProvider.GetService(typeof(InteractivityExtension)) as InteractivityExtension;
            var result = await interactivity.WaitForModalAsync("warnModal", ctx.User);
            if (result.TimedOut)
                return;
            await result.Result.Interaction.DeferAsync(true);

            string ruleNum = (result.Result.Values["rule"] as SelectMenuModalSubmission).Values[0];
            Rule rule = RuleService.Rules.Single(x => x.RuleNum.ToString() == ruleNum);
            int points = int.Parse((result.Result.Values["warnamount"] as SelectMenuModalSubmission).Values[0]);
            string reason = (result.Result.Values["reason"] as TextInputModalSubmission).Value;
            int severity = ((points - 1) / 5) + 1;
            DiscordMember targetMember = await ctx.Guild.GetMemberAsync(target.Id);

            int dbResult = await this.DbOps(targetMember, ctx.Member, rule, reason, points, result);
            if (dbResult < 0)
                return;

            new WarnRepository(ReadConfig.Config.ConnectionString).GetRemainingPoints(dbResult, out int remaining);

            DiscordEmbedBuilder warnEmbed = new()
            {
                Color = severity switch
                {
                    1 => DiscordColor.Yellow,
                    2 => DiscordColor.Orange,
                    3 => DiscordColor.Red,
                    _ => DiscordColor.Black,
                },
                Thumbnail = new()
                {
                    Height = 8,
                    Width = 8,
                    Url = targetMember.AvatarUrl,
                },
                Title = $"{targetMember.DisplayName} ({targetMember.Id}) has been warned for Rule {ruleNum}:",
                Description = $"{rule.RuleText}\n" +
                    "\n" +
                    $"{reason}",
                Footer = new()
                {
                    IconUrl = ctx.Member.AvatarUrl,
                    Text = ctx.Member.DisplayName
                }
            };
            warnEmbed.AddField($"{remaining} points remaining", "Please keep any talk of this to DM's");
            DiscordChannel warnsChannel = await ctx.Guild.GetChannelAsync(DiscordObjectService.Instance.WarnsChannel.Id);
            await warnsChannel.SendMessageAsync(targetMember.Mention, warnEmbed);

            DiscordEmbedBuilder logEmbed = new()
            {
                Color = DiscordColor.Yellow,
                Title = $"Successfully warned {targetMember.DisplayName} ({targetMember.Id})",
                Description = $"Rule {rule.RuleNum}:\n" +
                    "\n" +
                    $"{reason}" +
                    "\n" +
                    $"User has {remaining} points left.",
                Footer = new()
                {
                    IconUrl = ctx.Member.AvatarUrl,
                    Text = ctx.Member.DisplayName
                }
            };

            DiscordChannel warnLogChannel = await ctx.Guild.GetChannelAsync(DiscordObjectService.Instance.WarnLogChannel.Id);
            await warnLogChannel.SendMessageAsync(logEmbed);

            if (remaining < 11)
                await result.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.Member.Mention} User has {remaining} points left.\n" +
                    $"By common practice the user should be muted{(remaining < 6 ? ", kicked" : "")}{(remaining < 1 ? ", or banned" : "")}."));
            else
                await result.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Done!"));
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

            if (member.Id == DiscordObjectService.Instance.Lathrix)
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
            if (!await this.AreYouSure(ctx, user, "mute"))
                return;

            if (ctx.Member.Roles.Contains(await ctx.Guild.GetRoleAsync(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new()
                {
                    Color = ctx.Member.Color.PrimaryColor,
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
                    Color = ctx.Member.Color.PrimaryColor,
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
            if (!roles.Contains(await ctx.Guild.GetRoleAsync(701446136208293969)) || roles.Contains(await ctx.Guild.GetRoleAsync(767050052257447936)))
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("User is not muted."));
                return;
            }

            if (!await this.AreYouSure(ctx, user, "unmute"))
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
            if (await this.AreYouSure(ctx, user, "kick"))
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
            if (!await this.AreYouSure(ctx, user, "ban"))
                return;

            await ctx.Guild.BanMemberAsync(user, TimeSpan.FromDays(deleteMessageDays), reason);
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
            await ctx.FollowupAsync(new DiscordMessageBuilder().WithContent($"Done!"));
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

            var responseBuilder = new DiscordModalBuilder()
                .WithCustomId("report_reason")
                .WithTitle("Reason")
                .AddTextInput(textInput, "Please state a reason for your report.");

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

            embedBuilder.AddField("Reason:", (reason as TextInputModalSubmission).Value);

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

            var textInput = new DiscordTextInputComponent(
                "2famodal",
                placeholder: "000000",
                required: true,
                min_length: 6,
                max_length: 6
            );

            var responseBuilder = new DiscordModalBuilder()
                .WithCustomId("2famodal")
                .WithTitle("2FA")
                .AddTextInput(textInput, "Please input your 2FA Code.");

            await ctx.RespondWithModalAsync(responseBuilder);

            InteractivityExtension interactivity = (InteractivityExtension)ctx.ServiceProvider.GetService(typeof(InteractivityExtension));
            var res = await interactivity.WaitForModalAsync("2famodal");

            await res.Result.Interaction.DeferAsync(true);
            var reason = res.Result.Values["2famodal"];

            if (reason is null || reason is not TextInputModalSubmission)
            {
                await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Please provide a 2FA code!"));
                return false;
            }

            var pin = GoogleAuthenticator.GeneratePin(Encoding.UTF8.GetBytes(twoFAKey));
            if ((reason as TextInputModalSubmission).Value != pin)
            {
                await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Pin does not match!"));
                return false;
            }

            await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Pin matches!"));
            return true;
        }

        private async Task<bool> AreYouSure(CommandContext ctx, DiscordUser user, string operation)
        {
            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
                member = await ctx.Guild.GetMemberAsync(user.Id);

            DiscordFollowupMessageBuilder builder = new DiscordFollowupMessageBuilder()
                .AddEmbed(new DiscordEmbedBuilder
                {
                    Title = "Are you fucking sure about that?",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = user.AvatarUrl
                    },
                    Color = member == null ? new DiscordColor("#FF0000") : member.Color.PrimaryColor
                }
                .AddField("Member you selected:", member == null ? user.ToString() : member.ToString()));
            List<DiscordButtonComponent> components =
            [
                new(DiscordButtonStyle.Danger, "sure", "Yes I fucking am!"),
                new(DiscordButtonStyle.Secondary, "abort", "NO ABORT, ABORT!")
            ];
            builder.AsEphemeral();
            builder.AddActionRowComponent(components);
            DiscordMessage message = await ctx.FollowupAsync(builder);
            var interactivityResult = await message.WaitForButtonAsync(ctx.User, TimeSpan.FromMinutes(1));

            if (interactivityResult.Result.Id == "abort")
            {
                await ctx.EditFollowupAsync(message.Id, new DiscordMessageBuilder().WithContent($"Okay i will not {operation} the user."));
                return false;
            }
            return true;
        }

        private async ValueTask<DiscordMember> PKGet(DiscordGuild guild, bool isBot, ulong messageId, ulong fallback)
        {
            if (isBot)
            {
                HttpResponseMessage response = await Bot.Instance.PKClient.GetAsync($"https://api.pluralkit.me/v2/messages/{messageId}");
                if (response.IsSuccessStatusCode)
                {
                    PKMessageModel pkMessage = await response.Content.ReadFromJsonAsync<PKMessageModel>();
                    ulong userId = ulong.Parse(pkMessage.Sender);
                    return await guild.GetMemberAsync(userId);
                }
            }
            return await guild.GetMemberAsync(fallback);
        }

        private async ValueTask<int> DbOps(
            DiscordMember member,
            DiscordMember mod,
            Rule rule,
            string reason,
            int points,
            InteractivityResult<ModalSubmittedEventArgs> result)
        {
            UserRepository userRepo = new(ReadConfig.Config.ConnectionString);
            WarnRepository warnRepo = new(ReadConfig.Config.ConnectionString);
            AuditRepository auditRepo = new(ReadConfig.Config.ConnectionString);

            bool dbResult = userRepo.GetIdByDcId(member.Id, out int memberDbId);
            if (!dbResult)
            {
                await result.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("There was an error getting the user from the Database"));
                return -1;
            }
            dbResult = warnRepo.GetWarnAmount(memberDbId, out int warnNumber);
            if (!dbResult)
            {
                await result.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("There was an error getting the previous warns from the Database"));
                return -1;
            }
            _ = userRepo.GetIdByDcId(mod.Id, out int modDbId);
            if (memberDbId == 0 || modDbId == 0)
                return -1;

            Warn warn = new()
            {
                User = memberDbId,
                Mod = modDbId,
                Reason = rule.RuleNum == 0 ? reason : $"Rule {rule.RuleNum}, {reason}",
                Number = warnNumber + 1,
                Level = points,
                Time = DateTime.Now,
                Persistent = false
            };
            dbResult = warnRepo.Create(ref warn);
            if (!dbResult)
            {
                await result.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("There was an error creating the database entry"));
                return -1;
            }
            dbResult = auditRepo.Read(modDbId, out Audit audit);
            if (dbResult)
            {
                audit.Warns++;
                _ = auditRepo.Update(audit);
            }
            userRepo.UpdateLastPunish(memberDbId, DateTime.Now);
            return memberDbId;
        }
    }
}
