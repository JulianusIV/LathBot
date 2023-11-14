using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WarnModule;

namespace LathBotFront.Commands
{
    public class WarnCommands : BaseCommandModule
    {
        [Command("initdb")]
        [Description("Fill empty Database with users")]
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        public async Task InitDB(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            if (ctx.User.Id != 387325006176059394)
                return;
            IReadOnlyCollection<DiscordMember> members = await ctx.Guild.GetAllMembersAsync();
            foreach (DiscordMember member in members)
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
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        public async Task UpdateDB(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            if (ctx.User.Id != 387325006176059394)
                return;
            IReadOnlyCollection<DiscordMember> members = await ctx.Guild.GetAllMembersAsync();
            int count = 0;
            UserRepository repo = new(ReadConfig.Config.ConnectionString);
            foreach (DiscordMember member in members)
            {

                bool success = repo.ExistsDcId(member.Id, out bool exists);
                if (!success)
                {
                    DiscordMember mem = await ctx.Guild.GetMemberAsync(member.Id);
                    _ = ctx.RespondAsync($"Error checking user {member.Nickname}#{member.Discriminator} ({member.Id})!");
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
                        _ = ctx.RespondAsync($"Error creating user {member.Nickname}#{member.Discriminator} ({member.Id})!");
                    }
                    else
                    {
                        DiscordMember mem = await ctx.Guild.GetMemberAsync(member.Id);
                        _ = ctx.RespondAsync($"Added user {member.DisplayName}#{mem.Discriminator} ({mem.Id})");
                        count++;
                    }
                }
            }
            bool res = repo.CountAll(out int amount);
            if (!res)
            {
                await ctx.RespondAsync("Error counting entries in database");
                await ctx.RespondAsync($"Added {count} users, {members.Count} members");
            }
            else
            {
                await ctx.RespondAsync($"Added {count} users, {members.Count} members, {amount} entries");
            }
        }

        [Command("report")]
        [Description("Report a staff member to the senate. (Please don't abuse this system)")]
        public async Task Report(CommandContext ctx, [Description("The Id of the staff member you want to report.")] ulong ID)
        {
            await ctx.Channel.TriggerTypingAsync();
            DiscordGuild Lathland = await ctx.Client.GetGuildAsync(699555747591094344);
            DiscordMember member = await Lathland.GetMemberAsync(ID);
            if (!member.Roles.Contains(Lathland.GetRole(796234634316873759)) && !member.Roles.Contains(Lathland.GetRole(748646909354311751)))
            {
                await ctx.Channel.SendMessageAsync("```User is not staff or is part of the Senate.\n" + "If they are part of the Senate try messaging Chewybaca and/or another member of the senate```");
                return;
            }
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            DiscordMessage message = await ctx.Channel.SendMessageAsync("```Please state a reason for your report!\n" +
                "If you don't say anything for 5 minutes i will have to ignore you.\n" +
                "Please don't abuse this system.```");
            InteractivityResult<DiscordMessage> result = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User);
            await message.DeleteAsync();
            string reportReason = result.Result.Content;
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = DiscordColor.Red,
                Title = $"Report from user {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})",
                Description = $"Reported user: {member.Username}#{member.Discriminator} ({member.Id})",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.User.AvatarUrl, Text = ctx.User.Username }
            };

            embedBuilder.AddField("Reason:", reportReason);

            DiscordEmbed embed = embedBuilder.Build();
            List<DiscordMember> senate = new()
            {
                await Lathland.GetMemberAsync(387325006176059394),//Myself
                await Lathland.GetMemberAsync(289112287250350080)//Parth
            };
            foreach (DiscordMember senator in senate)
            {
                await senator.SendMessageAsync(embed);
            }
            await ctx.Channel.SendMessageAsync("Report successfully sent, The senate will get back to you, until then please be patient.");
        }

        [Command("allwarns")]
        [Description("Display all currently unpardoned warns")]
        [RequireUserPermissions(Permissions.KickMembers)]
        public async Task AllWarns(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
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
            List<Page> pages = new();
            DiscordEmbedBuilder embedBuilder = new()
            {
                Title = $"Showing all warnings in the server",
                Color = ctx.Guild.GetMemberAsync(192037157416730625).Result.Color,
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
                        Color = ctx.Guild.GetMemberAsync(192037157416730625).Result.Color,
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
        [Description("send a SQL Query to the database")]
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        public async Task SqlQuery(CommandContext ctx, [Description("Query")][RemainingText] string command)
        {
            SqlConnection connection = new(ReadConfig.Config.ConnectionString);
            try
            {
                if (ctx.Member.Id != 387325006176059394)
                    return;
                await ctx.TriggerTypingAsync();
                command = command.Trim();
                SqlCommand cmd = new(command, connection);
                string resultString = "";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int index = 0; index < reader.FieldCount; index++)
                        resultString += reader.GetSqlValue(index).ToString() + " ";
                    resultString = resultString.Trim();
                    if (resultString.Length >= 900)
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
                if (string.IsNullOrEmpty(resultString))
                    await ctx.Channel.SendMessageAsync("```\nQuery done!\n```");
                else
                    await ctx.Channel.SendMessageAsync("```c\n" + resultString + "\n```");
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

        private async Task<bool> AreYouSure(CommandContext ctx, DiscordUser user, string operation)
        {
            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
            {
                member = await ctx.Guild.GetMemberAsync(user.Id);
            }

            DiscordMessageBuilder builder = new()
            {
                Content = "Are you fucking sure about that?",
                Embed = new DiscordEmbedBuilder
                {
                    Title = "Member you selected:",
                    Description = member == null ? user.ToString() : member.ToString(),
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = user.AvatarUrl
                    },
                    Color = member == null ? new DiscordColor("#FF0000") : member.Color
                }
            };
            List<DiscordComponent> components = new()
            {
                new DiscordButtonComponent(ButtonStyle.Danger, "sure", "Yes I fucking am!"),
                new DiscordButtonComponent(ButtonStyle.Secondary, "abort", "NO ABORT, ABORT!")
            };
            builder.AddComponents(components);
            DiscordMessage message = await builder.SendAsync(ctx.Channel);
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            var interactivityResult = await interactivity.WaitForButtonAsync(message, ctx.User, TimeSpan.FromMinutes(1));

            await message.DeleteAsync();
            if (interactivityResult.Result.Id == "abort")
            {
                await ctx.RespondAsync($"Okay i will not {operation} the user.");
                return true;
            }
            return false;
        }

        private async Task WarnAsync(CommandContext ctx, DiscordMember member, DiscordMessage messageLink = null)
        {
            var warnBuilder = new WarnBuilder(ctx.Client,
                ctx.Channel,
                ctx.Guild,
                ctx.Member,
                member,
                messageLink);
            if (!await warnBuilder.PreExecutionChecks())
                return;

            await warnBuilder.RequestRule();
            await warnBuilder.RequestPoints();
            await warnBuilder.RequestReason();
            if (!await warnBuilder.WriteToDatabase())
                return;
            if (!await warnBuilder.WriteAuditToDb())
                return;
            await warnBuilder.ReadRemainingPoints();
            await warnBuilder.SendWarnMessage();
            if (messageLink is not null)
                await warnBuilder.LogMessage();
            await warnBuilder.SendPunishMessage();
            await WarnBuilder.ResetLastPunish(member.Id);
        }
    }
}
