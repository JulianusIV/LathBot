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
using LathBotBack.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
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
                User user = new User
                {
                    DcID = member.Id
                };
                UserRepository repo = new UserRepository(ReadConfig.Config.ConnectionString);

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
            UserRepository repo = new UserRepository(ReadConfig.Config.ConnectionString);
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
                    User user = new User
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

        [Command("mute")]
        [Aliases("shuddup")]
        [RequireUserPermissions(Permissions.KickMembers)]
        [Description("Mute a user")]
        public async Task Mute(CommandContext ctx, [Description("The user that you want to mute")] DiscordMember member, [Description("When you will be reminded (2 - 14 days, default 7)")] int duration)
        {
            await ctx.Channel.TriggerTypingAsync();
            if (duration > 14 || duration < 2)
            {
                await ctx.Channel.SendMessageAsync($"You cant mute someone for {(duration < 2 ? "shorter than 2 days." : "longer than 14 days.")}");
                return;
            }
            if (member.Id == 192037157416730625)
            {
                await ctx.Channel.SendMessageAsync("You cant mute Lathrix!");
                return;
            }
            else if (ctx.Member.Hierarchy <= member.Hierarchy)
            {
                await ctx.Channel.SendMessageAsync("You cant mute someone higher or same rank as you!");
                return;
            }
            else if (member.Roles.Contains(ctx.Guild.GetRole(701446136208293969)))
            {
                await ctx.Channel.SendMessageAsync("User is already muted.");
                return;
            }
            if (await AreYouSure(ctx, member, "mute"))
                return;

            if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
                {
                    Color = ctx.Member.Color,
                    Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
                    Description = $"[Link to usage]({ctx.Message.JumpLink})",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = ctx.Member.AvatarUrl,
                        Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
                    }
                };
                await ctx.Guild.GetChannel(722905404354592900).SendMessageAsync(discordEmbed.Build());
            }
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            MuteRepository mrepo = new MuteRepository(ReadConfig.Config.ConnectionString);
            AuditRepository repo = new AuditRepository(ReadConfig.Config.ConnectionString);
            bool userResult = urepo.GetIdByDcId(member.Id, out int id);
            if (!userResult)
            {
                await ctx.RespondAsync("There was a problem reading a User, user has not been muted.");
                return;
            }
            else
            {
                userResult = urepo.GetIdByDcId(ctx.Member.Id, out int modId);
                if (!userResult)
                {
                    await ctx.RespondAsync("There was a problem reading a Mod, user has not been muted.");
                    return;
                }

                bool result = mrepo.IsUserMuted(id, out bool exists);
                if (!result)
                {
                    await ctx.RespondAsync("Error looking up an existing mute for this user, user will be muted anyways.");
                }
                if (exists)
                {
                    result = mrepo.GetMuteByUser(id, out Mute mute);
                    if (!result)
                    {
                        await ctx.RespondAsync("Error gettign an existing mute for this user, user will not be muted.");
                        return;
                    }
                    mute.Mod = modId;
                    mute.Duration = duration;
                    mute.Timestamp = DateTime.Now;
                    result = mrepo.Update(mute);
                    if (!result)
                    {
                        await ctx.RespondAsync("Error updating an existing mute entry for this user, user will not be muted.");
                        return;
                    }
                }
                else
                {
                    Mute mute = new Mute
                    {
                        User = id,
                        Mod = modId,
                        Duration = duration,
                        Timestamp = DateTime.Now,
                    };
                    result = mrepo.Create(ref mute);
                    if (!result)
                    {
                        await ctx.RespondAsync("There was a problem creating a mute entry, user has not been muted.");
                        return;
                    }
                }
                DiscordRole verificationRole = ctx.Guild.GetRole(767050052257447936);
                DiscordRole mutedRole = ctx.Guild.GetRole(701446136208293969);
                await member.RevokeRoleAsync(verificationRole);
                await member.GrantRoleAsync(mutedRole);
                bool auditResult = repo.Read(modId, out Audit audit);
                if (!auditResult)
                {
                    await ctx.RespondAsync("There was a problem reading an Audit");
                }
                else
                {
                    audit.Mutes++;
                    bool updateResult = repo.Update(audit);
                    if (!updateResult)
                    {
                        await ctx.RespondAsync("There was a problem writing to the Audit table");
                    }
                }
            }
            await WarnBuilder.ResetLastPunish(member.Id);
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gray,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been muted",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}", embed);
            DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
            await warnsChannel.SendMessageAsync($"{member.Mention}", embed);
        }

        [Command("unmute")]
        [RequireUserPermissions(Permissions.KickMembers)]
        [Description("Unmute a muted user")]
        public async Task UnMute(CommandContext ctx, [Description("The user that you want to unmute")] DiscordMember member)
        {
            await ctx.Channel.TriggerTypingAsync();
            if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
                {
                    Color = ctx.Member.Color,
                    Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
                    Description = $"[Link to usage]({ctx.Message.JumpLink})",
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
                _ = await ctx.Channel.SendMessageAsync("User is not muted.");
                return;
            }
            if (await AreYouSure(ctx, member, "unmute"))
                return;
            DiscordRole verificationRole = ctx.Guild.GetRole(767050052257447936);
            DiscordRole mutedRole = ctx.Guild.GetRole(701446136208293969);
            await member.RevokeRoleAsync(mutedRole);
            await member.GrantRoleAsync(verificationRole);
            AuditRepository repo = new AuditRepository(ReadConfig.Config.ConnectionString);
            MuteRepository mrepo = new MuteRepository(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);

            if (!urepo.GetIdByDcId(member.Id, out int userId))
            {
                await ctx.RespondAsync("Failed reading a User - member has been unmuted anyways.");
            }
            else
            {
                if (!mrepo.GetMuteByUser(userId, out Mute entity))
                {
                    await ctx.RespondAsync("Failed reading a mute from the database - member has been unmuted anyways.");
                }
                else
                {
                    if (!mrepo.Delete(entity.Id))
                    {
                        await ctx.RespondAsync("Failed to delete a mute from the database - member has been unmuted anyways.");
                    }
                }
            }

            bool userResult = urepo.GetIdByDcId(ctx.Member.Id, out int id);
            if (!userResult)
            {
                await ctx.RespondAsync("There was a problem reading a User");
            }
            else
            {
                bool auditResult = repo.Read(id, out Audit audit);
                if (!auditResult)
                {
                    await ctx.RespondAsync("There was a problem reading an Audit");
                }
                else
                {
                    audit.Unmutes++;
                    bool updateResult = repo.Update(audit);
                    if (!updateResult)
                    {
                        await ctx.RespondAsync("There was a problem reading to the Audit table");
                    }
                }
            }
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.White,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been unmuted",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}", embed);
            DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
            await warnsChannel.SendMessageAsync($"{member.Mention}", embed);
        }

        [Command("muted")]
        [Description("Check how long a user is currently muted for.")]
        public async Task Muted(CommandContext ctx, [Description("The member you want to check.")] DiscordMember member)
        {
            if (!ctx.Member.Permissions.HasFlag(Permissions.KickMembers))
                return;
            if (!member.Roles.Contains(ctx.Guild.GetRole(701446136208293969)))
            {
                await ctx.RespondAsync("Member is not even muted smh.");
                return;
            }
            if (ctx.Channel.ParentId != 700009634643050546 &&
                ctx.Channel.Id != 838088490704568341 &&
                ctx.Channel.Id != 724313826786410508)
            {
                await ctx.RespondAsync("This command is not available in public channels");
                return;
            }
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            MuteRepository repo = new MuteRepository(ReadConfig.Config.ConnectionString);
            if (!urepo.GetIdByDcId(member.Id, out int id))
            {
                await ctx.RespondAsync("There was a problem getting a userID from the database.");
                return;
            }
            if (!repo.GetMuteByUser(id, out Mute entity))
            {
                await ctx.RespondAsync("There was a problem getting a mute from the database.");
                return;
            }
            if (!urepo.Read(entity.Mod, out User mod))
            {
                await ctx.RespondAsync("There was a problem getting a mod from the database.");
            }
            DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);

            await ctx.RespondAsync(new DiscordEmbedBuilder
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
                .AddField("Proposed unmute time:", $"<t:{((DateTimeOffset)entity.Timestamp + TimeSpan.FromDays(entity.Duration)).ToUnixTimeSeconds()}:R> ({entity.Duration} days after mute)"));
        }

        [Command("muted")]
        [Description("Check when you got muted.")]
        public async Task Muted(CommandContext ctx)
        {
            if (!ctx.Member.Roles.Contains(ctx.Guild.GetRole(701446136208293969)))
            {
                await ctx.RespondAsync("Your are not muted smh.");
                return;
            }
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            MuteRepository repo = new MuteRepository(ReadConfig.Config.ConnectionString);
            if (!urepo.GetIdByDcId(ctx.Member.Id, out int id))
            {
                await ctx.RespondAsync("There was a problem getting a userID from the database.");
                return;
            }
            if (!repo.GetMuteByUser(id, out Mute entity))
            {
                await ctx.RespondAsync("There was a problem getting a mute from the database.");
                return;
            }
            if (!urepo.Read(entity.Mod, out User mod))
            {
                await ctx.RespondAsync("There was a problem getting a mod from the database.");
            }
            DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);

            await ctx.RespondAsync(new DiscordEmbedBuilder
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
                .AddField("Latest unmute date (without Senate decision for longer mute):", $"<t:{((DateTimeOffset)entity.Timestamp + TimeSpan.FromDays(14)).ToUnixTimeSeconds()}:R>"));
        }

        [Command("timeout")]
        [RequireUserPermissions(Permissions.KickMembers)]
        [Description("Put a user in timeout for 15/30/45/60 min")]
        public async Task Timeout(CommandContext ctx, [Description("The user that you want to time out")] DiscordMember member, [Description("Why you want to put them in timeout")][RemainingText] string reason)
        {
            await ctx.Channel.TriggerTypingAsync();
            if (member.Id == 192037157416730625)
            {
                await ctx.Channel.SendMessageAsync("You cant timeout Lathrix!");
                return;
            }
            if (ctx.Member.Hierarchy <= member.Hierarchy)
            {
                await ctx.Channel.SendMessageAsync("You cant timeout someone higher or same rank as you!");
                return;
            }
            if (member.Roles.Contains(ctx.Guild.GetRole(701446136208293969)))
            {
                await ctx.Channel.SendMessageAsync("User is already muted or timed out.");
                return;
            }
            if (string.IsNullOrEmpty(reason))
            {
                await ctx.RespondAsync("Please specify a reason!");
                return;
            }

            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder
            {
                Content = "How long should the user be put in timeout?"
            };
            messageBuilder.AddComponents(new DiscordComponent[]
            {
                new DiscordButtonComponent(ButtonStyle.Success, "15", "15 min", emoji:new DiscordComponentEmoji("🕒")),
                new DiscordButtonComponent(ButtonStyle.Primary, "30", "30 min", emoji:new DiscordComponentEmoji("🕧")),
                new DiscordButtonComponent(ButtonStyle.Secondary, "45", "45 min", emoji:new DiscordComponentEmoji("🕘")),
                new DiscordButtonComponent(ButtonStyle.Danger, "60", "60 min", emoji:new DiscordComponentEmoji("🕛"))
            });

            DiscordMessage message = await ctx.RespondAsync(messageBuilder);
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            var res = await interactivity.WaitForButtonAsync(message, ctx.User, TimeSpan.FromMinutes(1));

            int duration = int.Parse(res.Result.Id);
            await message.DeleteAsync();

            if (await AreYouSure(ctx, member, "timeout"))
                return;

            if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
                {
                    Color = ctx.Member.Color,
                    Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
                    Description = $"[Link to usage]({ctx.Message.JumpLink})",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = ctx.Member.AvatarUrl,
                        Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
                    }
                };
                await ctx.Guild.GetChannel(722905404354592900).SendMessageAsync(discordEmbed.Build());
            }
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            AuditRepository repo = new AuditRepository(ReadConfig.Config.ConnectionString);
            DiscordRole verificationRole = ctx.Guild.GetRole(767050052257447936);
            DiscordRole mutedRole = ctx.Guild.GetRole(701446136208293969);
            await member.RevokeRoleAsync(verificationRole);
            await member.GrantRoleAsync(mutedRole);
            await ctx.Guild.GetChannel(838088490704568341).AddOverwriteAsync(member, deny: Permissions.AccessChannels | Permissions.SendMessages);

            bool result = urepo.GetIdByDcId(ctx.Member.Id, out int modId);
            if (!result)
            {
                await ctx.RespondAsync("There was a problem reading a Mod, user has been muted anyways.");
                return;
            }
            bool auditResult = repo.Read(modId, out Audit audit);
            if (!auditResult)
            {
                await ctx.RespondAsync("There was a problem reading an Audit");
            }
            else
            {
                audit.Timeouts++;
                bool updateResult = repo.Update(audit);
                if (!updateResult)
                {
                    await ctx.RespondAsync("There was a problem writing to the Audit table");
                }
            }
            await WarnBuilder.ResetLastPunish(member.Id);
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gray,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"Timeout",
                Description = $"You have been put in timeout - that means you will not be able to speak in any channel for {duration} minutes!",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            embedBuilder.AddField("Reason:", reason);
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.Channel.SendMessageAsync($"Timed out user for {duration} minutes.");

            try
            {
                await member.SendMessageAsync(embed);
            }
            catch
            {
                DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
                await warnsChannel.SendMessageAsync($"{member.Mention}", embed);
            }

            Thread.Sleep(TimeSpan.FromMinutes(duration));

            await member.GrantRoleAsync(verificationRole);
            await member.RevokeRoleAsync(mutedRole);
            foreach (var item in ctx.Guild.GetChannel(838088490704568341).PermissionOverwrites)
            {
                try
                {
                    var mem = await item.GetMemberAsync();
                    if (mem.Id == member.Id)
                    {
                        await item.DeleteAsync();
                        break;
                    }
                }
                catch
                {
                }
            }

            await ctx.Channel.SendMessageAsync($"Timeout for {member} over after {duration} minutes.");

            embed = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Gray,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"Timeout",
                Description = $"Your timeout is now over.",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };

            try
            {
                await member.SendMessageAsync(embed);
            }
            catch
            {
                DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
                await warnsChannel.SendMessageAsync($"{member.Mention}", embed);
            }
        }

        [Command("kick")]
        [RequireUserPermissions(Permissions.KickMembers)]
        [Description("Kick a user")]
        public async Task Kick(CommandContext ctx, [Description("The user that you want to kick")] DiscordMember member)
        {
            await ctx.Channel.TriggerTypingAsync();
            if (member.Id == 192037157416730625)
            {
                await ctx.Channel.SendMessageAsync("You cant kick Lathrix!");
                return;
            }
            else if (ctx.Member.Hierarchy <= member.Hierarchy)
            {
                await ctx.Channel.SendMessageAsync("You cant kick someone higher or same rank as you!");
                return;
            }
            if (await AreYouSure(ctx, member, "kick"))
                return;
            await member.RemoveAsync();
            AuditRepository repo = new AuditRepository(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            bool userResult = urepo.GetIdByDcId(ctx.Member.Id, out int id);
            if (!userResult)
            {
                await ctx.RespondAsync("There was a problem reading a User");
            }
            else
            {
                bool auditResult = repo.Read(id, out Audit audit);
                if (!auditResult)
                {
                    await ctx.RespondAsync("There was a problem reading an Audit");
                }
                else
                {
                    audit.Kicks++;
                    bool updateResult = repo.Update(audit);
                    if (!updateResult)
                    {
                        await ctx.RespondAsync("There was a problem reading to th Audit table");
                    }
                }
            }
            await WarnBuilder.ResetLastPunish(member.Id);
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.DarkButNotBlack,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been kicked",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
            };
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}", embed);
            DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
            await warnsChannel.SendMessageAsync($"{member.Mention}", embed);
        }

        [Command("ban")]
        [Aliases("yeet")]
        [RequireUserPermissions(Permissions.BanMembers)]
        [Description("Ban a user")]
        public async Task Ban(CommandContext ctx, [Description("The user that you want to ban")] DiscordUser user, [Description("How many days of messages to remove (0-7)")]int deleteMessageDays, [RemainingText][Description("Why the user is being banned")] string reason)
        {
            await ctx.Channel.TriggerTypingAsync();
            DiscordMember member = null;
            if (ctx.Guild.Members.ContainsKey(user.Id))
            {
                member = await ctx.Guild.GetMemberAsync(user.Id);
            }
            if (user.Id == 192037157416730625)
            {
                await ctx.RespondAsync("You cant ban Lathrix!");
                return;
            }
            else if (ctx.Member.Hierarchy <= member?.Hierarchy)
            {
                await ctx.RespondAsync("You cant ban someone higher or same rank as you!");
                return;
            }
            if (string.IsNullOrEmpty(reason))
            {
                await ctx.RespondAsync("Please provide a reason");
                return;
            }
            if (deleteMessageDays > 7 || deleteMessageDays < 0)
            {
                await ctx.RespondAsync("Days to delete has to be between 0 and 7");
                return;
            }
            if (await AreYouSure(ctx, user, "ban"))
                return;
            await ctx.Guild.BanMemberAsync(user.Id, deleteMessageDays, reason);
            AuditRepository repo = new AuditRepository(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            bool userResult = urepo.GetIdByDcId(ctx.Member.Id, out int id);
            if (!userResult)
            {
                await ctx.RespondAsync("There was a problem reading a User");
            }
            else
            {
                bool auditResult = repo.Read(id, out Audit audit);
                if (!auditResult)
                {
                    await ctx.RespondAsync("There was a problem reading an Audit");
                }
                else
                {
                    audit.Bans++;
                    bool updateResult = repo.Update(audit);
                    if (!updateResult)
                    {
                        await ctx.RespondAsync("There was a problem reading to th Audit table");
                    }
                }
            }
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Black,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl },
                Title = $"{user.Username}#{user.Discriminator} ({user.Id}) has been banned",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" },
                Description = reason
            };
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.RespondAsync($"{ctx.Member.Mention}", embed);
            DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
            await warnsChannel.SendMessageAsync($"{user.Mention}", embed);
        }

        [Command("warn")]
        [RequireUserPermissions(Permissions.KickMembers)]
        [Description("Warn a user (for more information go to #staff-information and look at the warn documentation)")]
        public async Task Warn(CommandContext ctx, [Description("The user that you want to warn")] DiscordMember member, [Description("a link to the warned message (will get deleted and logged)")] DiscordMessage messageLink = null)
        {
            await WarnAsync(ctx, member, messageLink);
        }

        [Command("warn")]
        [RequireUserPermissions(Permissions.KickMembers)]
        [Description("Warn a user (for more information go to #staff-information and look at the warn documentation)")]
        public async Task Warn(CommandContext ctx, [Description("a link to the warned message (will get deleted and logged)")] DiscordMessage message)
        {
            await WarnAsync(ctx, await ctx.Guild.GetMemberAsync(message.Author.Id), message);
        }

        [Command("pardon")]
        [Aliases("unwarn")]
        [RequireUserPermissions(Permissions.BanMembers)]
        [Description("Pardon a warn of a user (for more information go to #staff-information and look at the warn documentation)")]
        public async Task Pardon(CommandContext ctx, [Description("The user that you want to pardon a warn of")] DiscordMember member, [Description("The number of the warn that you want to pardon")] int warnNumber)
        {
            WarnRepository repo = new WarnRepository(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            bool result = urepo.GetIdByDcId(member.Id, out int id);
            if (!result)
            {
                await ctx.RespondAsync("There has been an error reading the Id of the User");
                return;
            }
            result = repo.GetWarnByUserAndNum(id, warnNumber, out Warn warn);
            if (!result)
            {
                await ctx.RespondAsync("There has been an error getting the warn from the database or the warn does not exist.");
                return;
            }
            result = repo.Delete(warn.ID);
            if (!result)
            {
                await ctx.RespondAsync("There has been an error deleting the warn from the database");
                return;
            }
            result = repo.GetAllByUser(warn.User, out List<Warn> others);
            if (!result)
            {
                await ctx.RespondAsync("Error reading other warns from the database.");
            }

            int counter = 0;
            foreach (var item in others)
            {
                counter++;
                item.Number = counter;
                result = repo.Update(item);
                if (!result)
                {
                    _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error updating the database.");
                    break;
                }
            }
            #region Audit
            AuditRepository auditRepo = new AuditRepository(ReadConfig.Config.ConnectionString);
            UserRepository userrepo = new UserRepository(ReadConfig.Config.ConnectionString);
            bool userResult = userrepo.GetIdByDcId(ctx.Member.Id, out int userid);
            if (!userResult)
            {
                await ctx.RespondAsync("There was a problem reading a User");
            }
            else
            {
                bool auditResult = auditRepo.Read(userid, out Audit audit);
                if (!auditResult)
                {
                    await ctx.RespondAsync("There was a problem reading an Audit");
                }
                else
                {
                    audit.Pardons++;
                    bool updateResult = auditRepo.Update(audit);
                    if (!updateResult)
                    {
                        await ctx.RespondAsync("There was a problem writing to the Audit table");
                    }
                }
            }
            #endregion
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Green,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Title = $"Pardoned warn number {warnNumber} of {member.DisplayName}#{member.Discriminator} ({member.Id})",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = ctx.Member.DisplayName }
            };
            DiscordEmbed embed = embedBuilder.Build();
            DiscordChannel warningsChannel = ctx.Guild.GetChannel(722186358906421369);
            await warningsChannel.SendMessageAsync(member.Mention, embed);
            await ctx.Channel.SendMessageAsync(ctx.Member.Mention, embed);
        }

        [Command("warns")]
        [Description("Check your or someone elses warnings")]
        public async Task Warns(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            WarnRepository repo = new WarnRepository(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            bool result = urepo.GetIdByDcId(ctx.Member.Id, out int id);
            if (!result)
            {
                await ctx.RespondAsync("There has been a problem getting the databaseId of the user");
                return;
            }
            result = repo.GetAllByUser(id, out List<Warn> warns);
            if (!result)
            {
                await ctx.RespondAsync("There has been an error getting the warns from the database");
            }
            urepo.Read(id, out var entity);
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl },
                Title = $"You have {warns.Count} warnings:",
                Description = $"Time of last punishment: {Formatter.Timestamp((DateTime)entity.LastPunish, TimestampFormat.ShortDateTime)}." +
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
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}", embed);
        }

        [Command("warns")]
        [Description("Check your or someone elses warnings")]
        public async Task Warns(CommandContext ctx, [Description("The user that you want to check the warning of (optional)")] DiscordUser user)
        {
            await ctx.Channel.TriggerTypingAsync();
            WarnRepository repo = new WarnRepository(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            bool result = urepo.GetIdByDcId(user.Id, out int id);
            if (!result)
            {
                await ctx.RespondAsync("There has been a problem getting the databaseId of the user");
                return;
            }
            result = repo.GetAllByUser(id, out List<Warn> warns);
            if (!result)
            {
                await ctx.RespondAsync("There has been an error getting the warns from the database");
            }
            urepo.Read(id, out var entity);
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl },
                Title = $"The user has {warns.Count} warnings:",
                Description = $"Time of last punishment: {Formatter.Timestamp((DateTime)entity.LastPunish, TimestampFormat.ShortDateTime)}." +
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
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.Channel.SendMessageAsync(embed);
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
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Red,
                Title = $"Report from user {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})",
                Description = $"Reported user: {member.Username}#{member.Discriminator} ({member.Id})",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.User.AvatarUrl, Text = ctx.User.Username }
            };

            embedBuilder.AddField("Reason:", reportReason);

            DiscordEmbed embed = embedBuilder.Build();
            List<DiscordMember> senate = new List<DiscordMember>
            {
                await Lathland.GetMemberAsync(613366102306717712),//Chewy
                await Lathland.GetMemberAsync(387325006176059394),//Myself
                await Lathland.GetMemberAsync(289112287250350080)//Parth
            };
            foreach (DiscordMember senator in senate)
            {
                await senator.SendMessageAsync(embed);
            }
            await ctx.Channel.SendMessageAsync("Report successfully sent, The senate will get back to you, until then please be patient.");
        }

        [Command("persist")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task Persist(CommandContext ctx, [Description("Member that got warned")] DiscordMember member, [Description("The number of the warn")] int warnNumber)
        {
            await ctx.Channel.TriggerTypingAsync();
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            WarnRepository repo = new WarnRepository(ReadConfig.Config.ConnectionString);
            bool result = urepo.GetIdByDcId(member.Id, out int userDbId);
            if (!result)
            {
                await ctx.RespondAsync("Failed to get the user from the database.");
                return;
            }
            result = repo.GetWarnByUserAndNum(userDbId, warnNumber, out Warn warn);
            if (!result)
            {
                await ctx.RespondAsync("Failed to get the warn from the database.");
                return;
            }
            if (warn.Level > 10)
            {
                await ctx.RespondAsync("Warn is already persistent by level");
                return;
            }
            else if (warn.Persistent)
            {
                await ctx.RespondAsync("Warn is already persistent");
                return;
            }
            warn.Persistent = true;
            result = repo.Update(warn);
            if (!result)
            {
                await ctx.RespondAsync("Failed to update the warn table.");
                return;
            }

            result = urepo.Read(warn.Mod, out User mod);
            if (!result)
            {
                await ctx.RespondAsync("Error reading the moderator from the database. (But warn has been persistet)");
                return;
            }
            await WarnBuilder.ResetLastPunish(member.Id);

            DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
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
            await ctx.RespondAsync(builder);

            DiscordEmbedBuilder dmBuilder = new DiscordEmbedBuilder
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
            await member.SendMessageAsync(dmBuilder);
        }

        [Command("allwarns")]
        [Description("Display all currently unpardoned warns")]
        [RequireUserPermissions(Permissions.KickMembers)]
        public async Task AllWarns(CommandContext ctx)
        {
            await ctx.Channel.TriggerTypingAsync();
            WarnRepository repo = new WarnRepository(ReadConfig.Config.ConnectionString);
            bool result = repo.GetAll(out List<Warn> warns);
            if (!result)
            {
                await ctx.RespondAsync("There has been an error reading the warns from the database");
                return;
            }

            int index = 0;
            int indicator = 0;
            UserRepository urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            List<Page> pages = new List<Page>();
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
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
            SqlConnection connection = new SqlConnection(ReadConfig.Config.ConnectionString);
            try
            {
                if (ctx.Member.Id != 387325006176059394)
                    return;
                await ctx.TriggerTypingAsync();
                command.Trim();
                SqlCommand cmd = new SqlCommand(command, connection);
                string resultString = "";
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    for (int index = 0; index < reader.FieldCount; index++)
                        resultString += reader.GetSqlValue(index).ToString() + " ";
                    resultString.Trim();
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

            DiscordMessageBuilder builder = new DiscordMessageBuilder
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
            List<DiscordComponent> components = new List<DiscordComponent>
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
            if (!(messageLink is null))
                await warnBuilder.LogMessage();
            await warnBuilder.SendPunishMessage();
            await WarnBuilder.ResetLastPunish(member.Id);
        }
    }
}
