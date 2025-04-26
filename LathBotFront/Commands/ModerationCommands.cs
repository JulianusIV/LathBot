using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using LathBotFront._2FA;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarnModule;

namespace LathBotFront.Commands
{
    public class ModerationCommands
    {
        [Command("offtopic")]
        [Description("Move a offtopic conversation to another channel")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task Offtopic(CommandContext ctx,
            [Parameter("Channel"), Description("The channel to move to.")] DiscordChannel channel,
            [Parameter("Amount"), Description("The amount of messages to copy/move over (min 5, max 100)")] long amount = 20,
            [Parameter("DeleteSource"), Description("Whether to delete the messages in the source channel.")] bool deleteSource = false)
        {
            await ctx.DeferResponseAsync();

            if (amount > 100 || amount < 5)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Amount must be less than 100 and more than 5!"));
                return;
            }

            var messages = ctx.Channel.GetMessagesAsync((int)amount).ToBlockingEnumerable();

            await channel.SendMessageAsync($"Copying over offtopic messages from {ctx.Channel.Mention}");
            var webhook = await channel.CreateWebhookAsync($"offtopic-move-{Guid.NewGuid()}");
            foreach (var message in messages)
            {
                if (string.IsNullOrEmpty(message.Content))
                    continue;

                await webhook.ExecuteAsync(new DiscordWebhookBuilder()
                    .WithContent(message.Content)
                    .WithAvatarUrl(message.Author.GetAvatarUrl(MediaFormat.Auto))
                    .WithUsername((message.Author as DiscordMember).DisplayName));
            }
            try
            {
                if (deleteSource)
                    await ctx.Channel.DeleteMessagesAsync(messages.Where(x => !string.IsNullOrEmpty(x.Content)).ToList().AsReadOnly());
            }
            catch (Exception e)
            {
                SystemService.Instance.Logger.Log("Could not delete delete message during ``offtopic`` command because of the following error:\n" + e.Message);
                deleteSource = false;
            }
            await webhook.DeleteAsync();
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"Your current conversation is offtopic, please move to {channel.Mention}. " +
                $"Your most recent messages have been {(deleteSource ? "moved" : "copied")}"));
        }

        [Command("change_warn_time"), Description("Change the amount of time a users warn will take to expire.")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task ChangeWarnTime(CommandContext ctx,
            [Parameter("Member"), Description("The member")] DiscordUser member,
            [Parameter("Warn"), Description("The warn to change")]
            [SlashAutoCompleteProvider(typeof(Autocomplete.UserWarnAutocompleteProvider))]
            long warnNumber,
            [Parameter("ChangeBy"), Description("By how much to extend or shorten the time the user is warned for")] long changeBy,
            [Parameter("Add"), Description("True = Add days to the sentence, False = Remove days from the sentence")] bool add = true)
        {
            await ctx.DeferResponseAsync();
            var repo = new WarnRepository(ReadConfig.Config.ConnectionString);
            var urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId(member.Id, out int dbId);
            repo.GetWarnByUserAndNum(dbId, (int)warnNumber, out Warn warn);

            if (warn.ExpirationTime is null)
                warn.ExpirationTime = (WarnBuilder.CalculateSeverity(warn.Level) == 1 ? 14 : 56) + (add ? (int)changeBy : -(int)changeBy);
            else if (add)
                warn.ExpirationTime += (int)changeBy;
            else
                warn.ExpirationTime -= (int)changeBy;

            repo.Update(warn);

            await (await ((DiscordMember)member).CreateDmChannelAsync()).SendMessageAsync($"Your warn number {warn.Number} has been changed to expire after {warn.ExpirationTime}!");

            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"Updated warn to expire {warn.ExpirationTime} days after warn creation."));
        }

        [Command("embedban"), Description("Ban a user from posting links/embeds/attachments in Debate")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task EmbedBan(
            CommandContext ctx,
            [Parameter("member"), Description("The member to embedban")]
            DiscordUser member
            )
        {
            await ctx.DeferResponseAsync();

            var repo = new UserRepository(ReadConfig.Config.ConnectionString);

            repo.GetIdByDcId(member.Id, out var id);
            repo.Read(id, out var user);

            if (user.EmbedBanned)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("User is already banned from posting embeds in Debate."));
                return;
            }
            user.EmbedBanned = true;
            repo.Update(user);
            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Done!"));
        }

        [Command("add_2fa"), Description("Add 2FA")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task AddTwoFA(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            UserRepository userrepo = new(ReadConfig.Config.ConnectionString);
            ModRepository repo = new(ReadConfig.Config.ConnectionString);
            userrepo.GetIdByDcId(ctx.Member.Id, out int dbId);
            repo.GetModById(dbId, out Mod mod);

            if (mod.TwoFAKey is not null && mod.TwoFAKey.Length > 0)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("You already have 2FA set up, message JulianusIV if you need to reset it."));
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
            var messageBuilder = new DiscordMessageBuilder().AddEmbed(embedBuilder).AddFile("qrcode.png", stream).WithContent(GoogleAuthenticator.GenerateProvisiongingString(ctx.Member.Username, Encoding.UTF8.GetBytes(twoFAKey)));
            await ctx.RespondAsync(messageBuilder);
        }

        [Command("test_2fa"), Description("Test 2FA")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task Test2FA(SlashCommandContext ctx)
        {
            UserRepository userrepo = new(ReadConfig.Config.ConnectionString);
            ModRepository repo = new(ReadConfig.Config.ConnectionString);
            userrepo.GetIdByDcId(ctx.Member.Id, out int dbId);
            repo.GetModById(dbId, out Mod mod);

            if (mod.TwoFAKey is null || mod.TwoFAKey.Length <= 0)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("2FA not set up!"));
                return;
            }
            var twoFAKey = AesEncryption.DecryptStringToBytes(mod.TwoFAKey, mod.TwoFAKeySalt);


            var textInput = new DiscordTextInputComponent("Please input your 2FA Code.",
                "2famodal",
                placeholder: "000000",
                required: true,
                min_length: 6,
                max_length: 6
                );

            var responseBuilder = new DiscordInteractionResponseBuilder()
                .WithCustomId("2famodal")
                .WithTitle("2FA")
                .AddTextInputComponent(textInput);

            await ctx.RespondWithModalAsync(responseBuilder);

            InteractivityExtension interactivity = (InteractivityExtension)ctx.ServiceProvider.GetService(typeof(InteractivityExtension));
            var res = await interactivity.WaitForModalAsync("2famodal");

            await res.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());
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
