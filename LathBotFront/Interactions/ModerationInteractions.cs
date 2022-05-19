using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using WarnModule;

namespace LathBotFront.Interactions
{
    public class ModerationInteractions : ApplicationCommandModule
    {
        [SlashCommand("offtopic", "Move a offtopic conversation to another channel")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task Offtopic(InteractionContext ctx,
            [Option("Channel", "The channel to move to.")] DiscordChannel channel,
            [Option("Amount", "The amount of messages to copy/move over (min 5, max 100)")] long amount = 20,
            [Option("DeleteSource", "Whether to delete the messages in the source channel.")] bool deleteSource = false)
        {
            if (!ctx.Member.Permissions.HasPermission(Permissions.KickMembers))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, 
                    new DiscordInteractionResponseBuilder()
                        .AsEphemeral()
                        .WithContent("No!"));
                return;
            }

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

        [SlashCommand("ChangeWarnTime", "Change the amount of time a users warn will take to expire.")]
        [SlashCommandPermissions(Permissions.Administrator)]
        public async Task ChangeWarnTime(InteractionContext ctx,
            [Option("Member", "The member")] DiscordUser member,
            [Option("Warn", "The warn to change", true)]
            [Autocomplete(typeof(Autocomplete.ChangeWarnTimeAutocompleteProvider))]
            long warnNumber,
            [Option("ChangeBy", "By how much to extend or shorten the time the user is warned for")] long changeBy,
            [Option("Add", "True = Add days to the sentence, False = Remove days from the sentence")] bool add = true)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            var repo = new WarnRepository(ReadConfig.Config.ConnectionString);
            var urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(member.Id, out int dbId);
            repo.GetWarnByUserAndNum(dbId, (int)warnNumber, out Warn warn);

            if (warn.ExpirationTime is null)
                warn.ExpirationTime = (WarnBuilder.CalculateSeverity(warn.Level) == 1 ? 14 : 56) + (add ? (int)changeBy : (-(int)changeBy));
            else if (add)
                warn.ExpirationTime += (int)changeBy;
            else
                warn.ExpirationTime -= (int)changeBy;

            repo.Update(warn);
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"Updated warn to expire {warn.ExpirationTime} days after warn creation."));
        }
    }
}
