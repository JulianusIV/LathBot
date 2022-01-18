using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LathBotBack.Services;
using System;
using System.IO;
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

                await DiscordObjectService.Instance.LogsChannel.SendMessageAsync(embed);
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

                await DiscordObjectService.Instance.LogsChannel.SendMessageAsync(embed);
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
                    await DiscordObjectService.Instance.LogsChannel.SendMessageAsync(embed);

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

                await DiscordObjectService.Instance.LogsChannel.SendMessageAsync(embed);
            });
            return Task.CompletedTask;
        }

        internal static Task RoleUpdated(DiscordClient _1, GuildRoleUpdateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.RoleBefore.Name == e.RoleAfter.Name
                    && e.RoleBefore.IsHoisted == e.RoleAfter.IsHoisted
                    && e.RoleBefore.Color.Value == e.RoleAfter.Color.Value
                    && e.RoleBefore.Permissions == e.RoleAfter.Permissions
                    && e.RoleBefore.IsMentionable == e.RoleAfter.IsMentionable)
                    return;
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithTimestamp(DateTime.Now)
                    .WithDescription($":information_source: The role {e.RoleAfter.Mention} ({e.RoleAfter.Id}) was just updated")
                    .AddField("Name", e.RoleBefore.Name == e.RoleAfter.Name ? "No change" : $"{e.RoleBefore.Name} -> {e.RoleAfter.Name}", true)
                    .AddField("Hoist", e.RoleBefore.IsHoisted == e.RoleAfter.IsHoisted ? "No change" : $"{e.RoleBefore.IsHoisted} -> {e.RoleAfter.IsHoisted}", true)
                    .AddField("Color", e.RoleBefore.Color.Value == e.RoleAfter.Color.Value ? "No change" : $"{e.RoleBefore.Color} -> {e.RoleAfter.Color}", true)
                    .AddField("Permissions", e.RoleBefore.Permissions == e.RoleAfter.Permissions ? "No change" : $"{(int)e.RoleBefore.Permissions} -> {(int)e.RoleAfter.Permissions}", true)
                    .AddField("Mentionable", e.RoleBefore.IsMentionable == e.RoleAfter.IsMentionable ? "No change" : $"{e.RoleBefore.IsMentionable} -> {e.RoleAfter.IsMentionable}", true)
                    .WithColor(DiscordColor.Blurple);

                await DiscordObjectService.Instance.LogsChannel.SendMessageAsync(new DiscordMessageBuilder().WithEmbed(embed).WithAllowedMentions(Mentions.None));
            });
            return Task.CompletedTask;
        }

        internal static Task MessageEdited(DiscordClient _1, MessageUpdateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Channel.Id == 722905404354592900)
                    return;

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithTimestamp(e.Message.EditedTimestamp)
                    .WithDescription($"{e.Author.Mention} ({e.Author.Id}) edited a message in {e.Channel.Mention}\n" +
                        $"Message ID: {e.Message.Id}")
                    .AddField("Before", e.MessageBefore is null ? Formatter.Italic("Unknown") : e.MessageBefore.Content)
                    .AddField("After", e.Message.Content)
                    .AddField("Doing!", $"[Jump to message]({e.Message.JumpLink})")
                    .WithThumbnail(e.Author.AvatarUrl)
                    .WithColor(DiscordColor.Yellow);

                await DiscordObjectService.Instance.LogsChannel.SendMessageAsync(new DiscordMessageBuilder().WithEmbed(embed).WithAllowedMentions(Mentions.None));
            });
            return Task.CompletedTask;
        }

        internal static Task MessageDeleted(DiscordClient _1, MessageDeleteEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Channel.Id == 722905404354592900)
                    return;
                if (e.Message.Content is null)
                    return;

                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                    .WithTimestamp(DateTime.Now)
                    .WithDescription($"Message created by {e.Message.Author.Mention} ({e.Message.Author.Id}) was deleted in {e.Channel.Mention}\n" +
                        $"Message ID: {e.Message.Id}\n\n" +
                        e.Message.Content)
                    .WithThumbnail(e.Message.Author.AvatarUrl)
                    .WithColor(DiscordColor.Red);

                await DiscordObjectService.Instance.LogsChannel.SendMessageAsync(new DiscordMessageBuilder().WithEmbed(embed).WithAllowedMentions(Mentions.None));
            });
            return Task.CompletedTask;
        }

        internal static Task BulkMessagesDeleted(DiscordClient _1, MessageBulkDeleteEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Channel.Id == 722905404354592900 //senate
                    || e.Channel.Id == 838088490704568341 //muted
                    || e.Channel.Id == 792486366138073180) //parstapo
                    return;

                if (e.Messages.Count == 0)
                    await DiscordObjectService.Instance.LogsChannel.SendMessageAsync("Messages bulk deleted, but none in cache!");
                else
                {
                    string toSave = "";
                    for (int index = e.Messages.Count - 1; index >= 0; index--)
                    {
                        if (e.Messages[index].Content is null)
                            continue;
                        toSave += e.Messages[index].Timestamp +
                            " - " + $"{e.Messages[index].Author.Username}#{e.Messages[index].Author.Discriminator} ({e.Messages[index].Author.Id})" + ":\n" +
                            e.Messages[index].Content +
                            (e.Messages[index].IsEdited ? $" (edited at {e.Messages[index].EditedTimestamp})\n" : "\n");
                    }
                    File.WriteAllText("Bulklog.txt", toSave);
                    using FileStream stream = new FileStream("Bulklog.txt", FileMode.Open);
                    DiscordMessageBuilder builder = new DiscordMessageBuilder()
                        .WithFile(stream)
                        .WithContent("Bulk delete logs dumped to text file.\n" +
                            "Creation Time: " + Formatter.Timestamp(DateTime.Now, TimestampFormat.LongTime));

                    await DiscordObjectService.Instance.LogsChannel.SendMessageAsync(builder);
                }
            });
            return Task.CompletedTask;
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
