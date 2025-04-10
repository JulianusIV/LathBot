using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            if (ctx.User.Id != 387325006176059394)
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
            if (ctx.User.Id != 387325006176059394)
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
            bool res = repo.CountAll(out int amount);
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
        [Description("Send a SQL Query to the database")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task SqlQuery(CommandContext ctx, [Description("Query")][RemainingText] string command)
        {
            await ctx.DeferResponseAsync();

            SqlConnection connection = new(ReadConfig.Config.ConnectionString);
            try
            {
                if (ctx.Member.Id != 387325006176059394)
                    return;
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
    }
}
