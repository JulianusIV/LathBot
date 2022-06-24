using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using LathBotFront.Interactions.PreExecutionChecks;
using System;
using System.Threading.Tasks;

namespace LathBotFront.Interactions
{
    public class DebateInteractions : ApplicationCommandModule
    {
        private static readonly CooldownSlash _embedCooldown = new CooldownSlash(300);

        [SlashCommand("embed", "Request permissions to embed links/attach files in Debate chat")]
        [EmbedBanned]
        public async Task Embed(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

            if (!await _embedCooldown.ExecuteCheckAsync(ctx))
            {
                var bucket = _embedCooldown.GetBucket(ctx);
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent($"This command is currently on cooldown! Available again {Formatter.Timestamp(bucket.ResetsAt)}."));
                return;
            }

            if (ctx.Channel.Id != 718162681554534511)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                    .WithContent("This command is only available in <#718162681554534511>."));
                return;
            }

            await ctx.Channel.AddOverwriteAsync(ctx.Member, Permissions.EmbedLinks | Permissions.AttachFiles);

            var message = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .WithContent("Done! You now have permissions to send ONE message containing links and/or files within the next 3 minutes."));

            var interactivity = ctx.Client.GetInteractivity();
            var res = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id, TimeSpan.FromMinutes(3));

            await ctx.Channel.DeleteOverwriteAsync(ctx.Member);

            if (res.TimedOut)
                await ctx.EditFollowupAsync(message.Id, new DiscordWebhookBuilder()
                    .WithContent("Permissions have been revoked again due to timeout."));
        }
    }
}
