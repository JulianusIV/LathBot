﻿using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using LathBotFront.Interactions.PreExecutionChecks;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LathBotFront.Interactions
{
    public class DebateInteractions
    {
        private static readonly CooldownSlash _embedCooldown = new(300);

        [Command("embed")]
        [Description("Request permissions to embed links/attach files in Debate chat")]
        [EmbedBanned]
        public async Task Embed(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            if (!await _embedCooldown.ExecuteCheckAsync(ctx))
            {
                var bucket = _embedCooldown.GetBucket(ctx);
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent($"This command is currently on cooldown! Available again {Formatter.Timestamp(bucket.ResetsAt)}."));
                return;
            }

            if (ctx.Channel.Id != 718162681554534511)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("This command is only available in <#718162681554534511>."));
                return;
            }

            await ctx.Channel.AddOverwriteAsync(ctx.Member, DiscordPermission.EmbedLinks | DiscordPermission.AttachFiles);

            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Done! You now have permissions to send ONE message containing links and/or files within the next 3 minutes."));

            var res = await ctx.Channel.GetNextMessageAsync(ctx.Member, TimeSpan.FromMinutes(3));

            await ctx.Channel.DeleteOverwriteAsync(ctx.Member);

            if (res.TimedOut)
                await ctx.EditResponseAsync(new DiscordMessageBuilder().WithContent("Permissions have been revoked again due to timeout."));
        }
    }
}
