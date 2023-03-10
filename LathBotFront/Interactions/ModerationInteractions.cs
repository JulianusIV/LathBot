using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using LathBotFront._2FA;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
            [Autocomplete(typeof(Autocomplete.UserWarnAutocompleteProvider))]
            long warnNumber,
            [Option("ChangeBy", "By how much to extend or shorten the time the user is warned for")] long changeBy,
            [Option("Add", "True = Add days to the sentence, False = Remove days from the sentence")] bool add = true)
        {
            if (!ctx.Member.Permissions.HasPermission(Permissions.Administrator))
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder()
                        .AsEphemeral()
                        .WithContent("No!"));
                return;
            }

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

            await (await ((DiscordMember)member).CreateDmChannelAsync()).SendMessageAsync($"Your warn number {warn.Number} has been changed to expire after {warn.ExpirationTime}!");

            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent($"Updated warn to expire {warn.ExpirationTime} days after warn creation."));
        }

        [SlashCommand("embedban", "Ban a user from posting links/embeds/attachments in Debate")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task EmbedBan(
            InteractionContext ctx,
            [Option("member", "The member to embedban")]
            DiscordUser member
            )
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

            var repo = new UserRepository(ReadConfig.Config.ConnectionString);

            repo.GetIdByDcId(member.Id, out var id);
            repo.Read(id, out var user);

            if (user.EmbedBanned)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("User is already banned from posting embeds in Debate."));
                return;
            }
            user.EmbedBanned = true;
            repo.Update(user);
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().WithContent("Done!"));
        }

        [SlashCommand("AddTwoFA", "Add 2FA")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task AddTwoFA(InteractionContext ctx)
        {
            await ctx.DeferAsync(true);

            UserRepository userrepo = new(ReadConfig.Config.ConnectionString);
            ModRepository repo = new(ReadConfig.Config.ConnectionString);
            userrepo.GetIdByDcId(ctx.Member.Id, out int dbId);
            repo.GetModById(dbId, out Mod mod);

            if (mod.TwoFAKey is not null && mod.TwoFAKey.Length > 0)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You already have 2FA set up, message JulianusIV if you need to reset it."));
                return;
            }
            mod.TwoFAKeySalt = Guid.NewGuid().ToString("D");
            var twoFAKey = Guid.NewGuid().ToString("D");
            mod.TwoFAKey = AesEncryption.EncryptStringToBytes(twoFAKey, mod.TwoFAKeySalt);
            repo.Update(mod);

            var image = GoogleAuthenticator.GenerateProvisioningImage(ctx.Member.Username, Encoding.UTF8.GetBytes(twoFAKey));
            using var stream = new MemoryStream(image);
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Title = "Add 2FA",
                Description = "Scan this QR code with your Google Authenticator app, or similar app, that supports that authentication standard, or copy the link above to add it to a 2FA app.",

                Color = new DiscordColor(27, 116, 226),
            }.WithImageUrl("attachment://qrcode.png");
            var webhookBuilder = new DiscordWebhookBuilder().AddEmbed(embedBuilder).AddFile("qrcode.png", stream).WithContent(GoogleAuthenticator.GenerateProvisiongingString(ctx.Member.Username, Encoding.UTF8.GetBytes(twoFAKey)));
            await ctx.EditResponseAsync(webhookBuilder);
        }

        [SlashCommand("Test2FA", "Test 2FA")]
        [SlashCommandPermissions(Permissions.KickMembers)]
        public async Task Test2FA(InteractionContext ctx)
        {
            UserRepository userrepo = new(ReadConfig.Config.ConnectionString);
            ModRepository repo = new(ReadConfig.Config.ConnectionString);
            userrepo.GetIdByDcId(ctx.Member.Id, out int dbId);
            repo.GetModById(dbId, out Mod mod);

            if (mod.TwoFAKey is null || mod.TwoFAKey.Length <= 0)
            {
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("2FA not set up!").AsEphemeral());
                return;
            }
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

            await res.Result.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
            var reason = res.Result.Values["2famodal"];

            if (reason is null)
            {
                await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Please provide a 2FA code!"));
                return;
            }

            var pin = GoogleAuthenticator.GeneratePin(Encoding.UTF8.GetBytes(twoFAKey));
            if (reason != pin)
            {
                await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Pin does not match!"));
                return;
            }

            await res.Result.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Successful!"));
        }
    }
}
