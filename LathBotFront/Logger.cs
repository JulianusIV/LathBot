using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront
{
    public class Logger
    {
        internal static Task BanAdded(DiscordClient _1, GuildBanAddEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                DiscordAuditLogBanEntry auditLog = (await e.Guild.GetAuditLogsAsync(limit: 5, action_type: AuditLogActionType.Ban))
                    .First(x => ((DiscordAuditLogBanEntry)x).Target == e.Member)
                    as DiscordAuditLogBanEntry;

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithTimestamp(auditLog.CreationTimestamp)
                    .WithDescription($":hammer: {Formatter.Bold("User banned:")} {e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})\n" +
                    $"\n{Formatter.Bold("Reason:")} {auditLog.Reason}")
                    .WithColor(DiscordColor.DarkRed);

                await e.Guild.GetChannel(512370308976607250).SendMessageAsync(embed);
            });
            return Task.CompletedTask;
        }

        internal static Task BanRemoved(DiscordClient _1, GuildBanRemoveEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                DiscordAuditLogBanEntry auditLog = (await e.Guild.GetAuditLogsAsync(limit: 5, action_type: AuditLogActionType.Unban))
                    .First(x => ((DiscordAuditLogBanEntry)x).Target == e.Member)
                    as DiscordAuditLogBanEntry;

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithTimestamp(auditLog.CreationTimestamp)
                    .WithDescription($":leaves: {Formatter.Bold("User unbanned:")} {e.Member.Username}#{e.Member.Discriminator} ({e.Member.Id})\n" +
                    $"{Formatter.Bold("By user:")} {auditLog.UserResponsible.Username}#{e.Member.Discriminator} ({e.Member.Id})\n" +
                    $"\n{Formatter.Bold("Reason:")} {auditLog.Reason}")
                    .WithColor(DiscordColor.Green);

                await e.Guild.GetChannel(512370308976607250).SendMessageAsync(embed);
            });
            return Task.CompletedTask;
        }

        internal static Task MemberUpdated(DiscordClient _1, GuildMemberUpdateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithTimestamp(DateTime.Now);
                if (e.NicknameAfter != e.NicknameBefore)
                {
                    embed.WithDescription($":information_source: {e.Member.Mention} ({e.Member.Id}) changed their nickname:")
                        .WithThumbnail(e.Member.AvatarUrl)
                        .AddField("Before", e.NicknameBefore is null ? e.Member.Username : e.NicknameBefore, true)
                        .AddField("After", e.NicknameAfter is null ? e.Member.Username : e.NicknameAfter, true)
                        .WithColor(DiscordColor.Blurple);

                }
                else if (e.RolesAfter.Count > e.RolesBefore.Count)
                {
                    string description = $":add: {e.Member.Mention} ({e.Member.Id}) was granted additional roles:";

                    foreach (var item in e.RolesAfter)
                    {
                        if (!e.RolesBefore.Contains(item))
                            description += Environment.NewLine + item.Name;
                    }

                    embed.WithDescription(description)
                        .WithThumbnail(e.Member.AvatarUrl)
                        .WithColor(DiscordColor.Green);
                }
                else if (e.RolesAfter.Count < e.RolesBefore.Count)
                {
                    string description = $":remove: {e.Member.Mention} ({e.Member.Id}) was revoked some roles:";

                    foreach (var item in e.RolesBefore)
                    {
                        if (!e.RolesAfter.Contains(item))
                            description += Environment.NewLine + item.Name;
                    }

                    embed.WithDescription(description)
                        .WithThumbnail(e.Member.AvatarUrl)
                        .WithColor(DiscordColor.Red);
                }
                if (!(embed.Description is null))
                    await e.Guild.GetChannel(512370308976607250).SendMessageAsync(embed);

            });
            return Task.CompletedTask;
        }

        internal static Task ChannelUpdated(DiscordClient _1, ChannelUpdateEventArgs e)
        {
            //TODO Permissions

            _ = Task.Run(async () =>
            {
                if (e.ChannelBefore.Id == 722905404354592900)
                    return;
                if (e.ChannelBefore.Name == e.ChannelAfter.Name
                    && e.ChannelBefore.Parent.Name == e.ChannelAfter.Parent.Name
                    && e.ChannelBefore.Topic == e.ChannelAfter.Topic
                    && e.ChannelBefore.IsNSFW == e.ChannelAfter.IsNSFW
                    && e.ChannelBefore.PerUserRateLimit == e.ChannelAfter.PerUserRateLimit)
                    return;
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithTimestamp(DateTime.Now)
                    .WithDescription($":information_source: The channel {e.ChannelAfter.Mention} ({e.ChannelAfter.Id}) was just updated")
                    .AddField("Name", e.ChannelBefore.Name == e.ChannelAfter.Name ? "No change" : $"{e.ChannelBefore.Name} -> {e.ChannelAfter.Name}", true)
                    .AddField("Category", e.ChannelBefore.Parent.Name == e.ChannelAfter.Parent.Name ? "No change" : $"{e.ChannelBefore.Parent.Name} -> {e.ChannelAfter.Parent.Name}", true)
                    .AddField("Topic", e.ChannelBefore.Topic == e.ChannelAfter.Topic ? "No change" : $"{e.ChannelBefore.Topic} -> {e.ChannelAfter.Topic}", true)
                    .AddField("NSFW", e.ChannelBefore.IsNSFW == e.ChannelAfter.IsNSFW ? "No change" : $"{e.ChannelBefore.IsNSFW} -> {e.ChannelAfter.IsNSFW}", true)
                    .AddField("Slowmode (seconds)", e.ChannelBefore.PerUserRateLimit == e.ChannelAfter.PerUserRateLimit ? "No change" : $"{e.ChannelBefore.PerUserRateLimit} -> {e.ChannelAfter.PerUserRateLimit}", true)
                    .WithColor(DiscordColor.Blurple);

                await e.Guild.GetChannel(512370308976607250).SendMessageAsync(embed);
            });
            return Task.CompletedTask;
        }

        internal static Task RoleUpdated(DiscordClient _1, GuildRoleUpdateEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static Task MessageEdited(DiscordClient _1, MessageUpdateEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static Task MessageDeleted(DiscordClient _1, MessageDeleteEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static Task BulkMessagesDeleted(DiscordClient _1, MessageBulkDeleteEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static Task EmojiUpdated(DiscordClient _1, GuildEmojisUpdateEventArgs e)
        {
            throw new NotImplementedException();
        }

        internal static Task VoiceUpdate(DiscordClient _1, VoiceStateUpdateEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
