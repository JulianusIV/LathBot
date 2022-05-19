using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using System.Linq;
using System;
using LathBotBack.Services;

namespace LathBotFront.Interactions
{
    public class ModerationInteractions : ApplicationCommandModule
    {
        [SlashCommand("offtopic", "Move a offtopic conversation to another channel")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task Offtopic(InteractionContext ctx, 
            [Option("Channel", "The channel to move to.")]DiscordChannel channel,
            [Option("Amount", "The amount of messages to copy/move over (min 5, max 100)")]long amount = 20,
            [Option("DeleteSource", "Whether to delete the messages in the source channel.")]bool deleteSource = false)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if (amount > 100 || amount < 5)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Amount must be less than 100 and more than 5!"));
                return;
            }

            var messages = (await ctx.Channel.GetMessagesAsync((int)amount)).Reverse();

            await channel.SendMessageAsync($"Copying over offtopic messages from {ctx.Channel.Mention}");
            var webhook = await channel.CreateWebhookAsync($"offtopic-move-{Guid.NewGuid()}");
            foreach (var message in messages)
            {
                if (string.IsNullOrEmpty(message.Content))
                    continue;

                await webhook.ExecuteAsync(new DiscordWebhookBuilder()
                    .WithContent(message.Content)
                    .WithAvatarUrl(message.Author.GetAvatarUrl(ImageFormat.Auto))
                    .WithUsername((message.Author as DiscordMember).DisplayName));
            }
            try
            {
                if (deleteSource)
                    await ctx.Channel.DeleteMessagesAsync(messages.Where(x => !string.IsNullOrEmpty(x.Content)));
            }
            catch (Exception e)
            {
                SystemService.Instance.Logger.Log("Could not delete delete message during ``offtopic`` command because of the following error:\n" + e.Message);
                deleteSource = false;
            }
            await webhook.DeleteAsync();
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent($"Your current conversation is offtopic, please move to {channel.Mention}. " +
                             $"Your most recent messages have been {(deleteSource ? "moved" : "copied")}"));
        }
    }
}
