using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;

using LathBotFront.Commands.Events;

namespace LathBotFront.Commands
{
	[Group("event")]
    [Aliases("e")]
    [Description("All the event commands, use -help event <subcommand> for more info")]
    public class EventCommands : BaseCommandModule
    {
        [Command("setmode")]
        [Aliases("set")]
        [Description("Enable or disable eventmode")]
        [RequireUserPermissions(Permissions.KickMembers)]
        public async Task SetEventmode(CommandContext ctx, [Description("True/False")] bool setTo)
        {
            if (EventParams.Instance.IsInEventMode == setTo)
            {
                await ctx.Channel.SendMessageAsync($"Eventmode is already {setTo}").ConfigureAwait(false);
                return;
            }

            EventParams.Instance.IsInEventMode = setTo;

            if (setTo && setTo == EventParams.Instance.IsInEventMode)
			{
                EventParams.Instance.Submissions.Clear();
                await ctx.Guild.GetChannel(EventParams.Instance.SubmissionsChannelId)
                    .AddOverwriteAsync(ctx.Guild.GetRole(767050052257447936), Permissions.None, Permissions.SendMessages | Permissions.AccessChannels);
			}
            else if (setTo == EventParams.Instance.IsInEventMode)
                await ctx.Guild.GetChannel(EventParams.Instance.SubmissionsChannelId)
                    .AddOverwriteAsync(ctx.Guild.GetRole(767050052257447936), Permissions.AccessChannels, Permissions.SendMessages);
            await ctx.Channel.SendMessageAsync($"Eventmode now set to {setTo}").ConfigureAwait(false);
        }

        [Command("mode")]
        [Description("Outputs if the bot is currently in eventmode")]
        public async Task GetEventmode(CommandContext ctx)
        {
            if (EventParams.Instance.IsInEventMode == true)
                await ctx.Channel.SendMessageAsync("Eventmode is on!").ConfigureAwait(false);
            else if (EventParams.Instance.IsInEventMode == false)
                await ctx.Channel.SendMessageAsync("Eventmode is off!").ConfigureAwait(false);
            else
                await ctx.Channel.SendMessageAsync("Database error while reading eventmode, please contact JulianusIV#1281 (387325006176059394)");
        }

        [Command("submit")]
        [Aliases("sub")]
        [Description("Submit something to the currently ongoing event\n" +
            "(use this command while uploading a file to upload it to event-submissions\n" +
            "you cannot edit your files, only your provided Text")]
        public async Task Submit(CommandContext ctx, [Description("Submission text (editable using -event edit)")][RemainingText] string submission)
        {
            await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
            if (EventParams.Instance.IsInEventMode is null || EventParams.Instance.IsInEventMode == false)
            {
                await ctx.Channel.SendMessageAsync("Submissions are not open!");
                return;
            }
			if (string.IsNullOrEmpty(submission))
			{
                await ctx.Channel.SendMessageAsync("Please add a submission text!");
                await ctx.Message.DeleteAsync();
                return;
			}
            DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
            {
                Color = ctx.Member.Color,
                Title = $"Submission from {ctx.Member.Username}#{ctx.Member.Discriminator}",
                Description = $"Submission text given by {ctx.Member.Mention}:\n" + submission,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Member.AvatarUrl,
                    Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
                }
            };
            if (EventParams.Instance.Submissions.ContainsKey(ctx.Member.Id))
            {
                await ctx.Channel.SendMessageAsync("You seem to already have submitted, if you want to edit it use ``-event edit``!").ConfigureAwait(false);
                await ctx.Message.DeleteAsync().ConfigureAwait(false);
                return;
            }
            else
            {
                if (ctx.Message.Attachments.Count > 0)
                {
                    using var client = new WebClient();
                    Dictionary<string, Stream> files = new Dictionary<string, Stream>();
                    foreach (DiscordAttachment attachment in ctx.Message.Attachments)
                    {
                        client.DownloadFile(new Uri(attachment.Url), attachment.FileName);
                        FileStream stream = new FileStream(attachment.FileName, FileMode.Open);
                        files.Add(attachment.FileName, stream);
                    }

                    DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder
                    {
                        Content = "",
                        Embed = discordEmbed.Build(),
                    };
                    messageBuilder.WithFiles(files);

                    DiscordMessage submissionMessage = await ctx.Guild.GetChannel(EventParams.Instance.SubmissionsChannelId).SendMessageAsync(messageBuilder).ConfigureAwait(false);

                    EventParams.Instance.Submissions.Add(ctx.Member.Id, submissionMessage);

                    foreach (KeyValuePair<string, Stream> file in files)
                    {
                        File.Delete(file.Key);
                        file.Value.Close();
                    }
                }
                else
                    EventParams.Instance.Submissions.Add(ctx.Member.Id, await ctx.Guild.GetChannel(EventParams.Instance.SubmissionsChannelId).SendMessageAsync(discordEmbed.Build()).ConfigureAwait(false));
            }
            await ctx.Channel.SendMessageAsync("Submission successful! Use ``-event edit`` to edit your submission.").ConfigureAwait(false);
            await ctx.Message.DeleteAsync().ConfigureAwait(false);
        }

        [Command("edit")]
        [Description("Edit a previously submitted item while the event is still ongoing")]
        public async Task Edit(CommandContext ctx, [Description("Submission text")][RemainingText] string edit)
        {
            await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
            if (EventParams.Instance.IsInEventMode is null || EventParams.Instance.IsInEventMode == false)
            {
                await ctx.Channel.SendMessageAsync("Submissions have been closed!").ConfigureAwait(false);
                return;
            }
            if (!EventParams.Instance.Submissions.ContainsKey(ctx.Member.Id))
            {
                await ctx.Channel.SendMessageAsync("You did not yet submit anything for this event, use ``-event submit`` instead!").ConfigureAwait(false);
                return;
            }
            if (string.IsNullOrEmpty(edit))
            {
                await ctx.Channel.SendMessageAsync("Please add a submission text!");
                await ctx.Message.DeleteAsync();
                return;
            }
            DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
            {
                Color = ctx.Member.Color,
                Title = $"Submission from {ctx.Member.Username}#{ctx.Member.Discriminator}",
                Description = $"Submission text given by {ctx.Member.Mention}:\n" + edit,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    IconUrl = ctx.Member.AvatarUrl,
                    Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
                }
            };
            await EventParams.Instance.Submissions[ctx.Member.Id].ModifyAsync("", discordEmbed.Build()).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync("Submission successfully edited!").ConfigureAwait(false);
            await ctx.Message.DeleteAsync().ConfigureAwait(false);
        }

        [Command("delete")]
        [Aliases("d")]
        [Description("Delete a submission")]
        [RequireUserPermissions(Permissions.KickMembers)]
        public async Task Delete(CommandContext ctx, [Description("Link to the bots submission message in event-submissions")] DiscordMessage submission)
        {
            await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
            if (!EventParams.Instance.Submissions.ContainsValue(submission))
            {
                await ctx.Channel.SendMessageAsync("Submission does not exist you buffoon!").ConfigureAwait(false);
                return;
            }
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            DiscordMessage message = await ctx.Channel.SendMessageAsync($"Do you really want to delete the submission? {submission.JumpLink}").ConfigureAwait(false);
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode(ctx.Client, "✅"));
            await message.CreateReactionAsync(DiscordEmoji.FromUnicode(ctx.Client, "❌"));
            var result = await interactivity.WaitForReactionAsync(x => x.Message == message && x.User == ctx.User).ConfigureAwait(false);
            if (result.Result.Emoji.Name == "✅")
            {
                foreach (KeyValuePair<ulong, DiscordMessage> sub in EventParams.Instance.Submissions)
                {
                    if (sub.Value == submission)
                    {
                        await EventParams.Instance.Submissions[sub.Key].DeleteAsync().ConfigureAwait(false);
                        EventParams.Instance.Submissions.Remove(sub.Key);
                        break;
                    }
                }
                await ctx.Channel.SendMessageAsync("Done!").ConfigureAwait(false);
            }
            else
                await ctx.Channel.SendMessageAsync("Okay not deleting the submission!").ConfigureAwait(false);
        }
    }
}
