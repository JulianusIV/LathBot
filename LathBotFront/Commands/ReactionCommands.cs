using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using LathBotBack.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LathBotFront.Commands
{
    class ReactionCommands : BaseCommandModule
    {
        [Command("_-")]
        public async Task Face(CommandContext ctx)
        {
            await ctx.Member.TimeoutAsync(ctx.Message.Timestamp + TimeSpan.FromMinutes(1));
            await ctx.RespondAsync("😒");
        }

        [Command("slap")]
        [Description("Slap someone, but be careful who you choose to slap, they might slap harder!\n" +
            "Inspired by a xelA tag originally creatd by @ThaTheoMans#7006.")]
        public async Task Slap(CommandContext ctx, [Description("Who you want to slap.")] DiscordMember target)
        {
            if (target.Id == 708083256439996497) //LathBot
            {
                if (ctx.Member.Id != 387325006176059394) //myself
                    await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you can't slap me! *slap*");
                else
                    await ctx.Channel.SendMessageAsync($"God dammit, *slaps myself* I've made a mistake again! *slaps myself*");
            }
            else if (target.Id == 387325006176059394) //myself
            {
                await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you can't slap my creator or imma slap you! *slap*");
            }
            else if (target.Id == 192037157416730625) //Lathrix
            {
                await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* why would you slap a god? *slap*");
            }
            else if (target.Id == 241445303960600576) //Theo, Lucky
            {
                if (ctx.Member.Id == 387325006176059394)
                    await ctx.Channel.SendMessageAsync($"You might think you are immune to slaps {target.Mention}, *slap* but {ctx.Member.Mention} is immune to counter slapping! *slap*");
                else
                    await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you dont slap the creator of slapping");
            }
            else if (target.Id == 280850661485314049)
            {
                if (ctx.Member.Id == 387325006176059394)
                    await ctx.Channel.SendMessageAsync($"You might think you are immune to slaps {target.Mention}, *slap* but {ctx.Member.Mention} is immune to counter slapping! *slap*");
                else
                    await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* do you want to be nuzzled by a catboy?! *slap* ÒwÓ");
            }
            else if (target.Id == 700373370491109489) //Femke
            {
                if (ctx.Member.Id == 387325006176059394)
                    await ctx.Channel.SendMessageAsync($"You might think you are immune to slaps {target.Mention}, *slap* but {ctx.Member.Mention} is immune to counter slapping! *slap*");
                else
                    await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you wanna get ~~mini~~modded? *slap*");
            }
            else if (target.Id == 289112287250350080) //Parth
            {
                await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you wanna get yelled at? *slap*");
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"God dammit {target.Mention}, *slap* you've made a mistake again! *slap*");
            }
        }

        [Command("repeat")]
        [Description("Let the Bot repeat something.")]
        public async Task Repeat(CommandContext ctx, [Description("What the bot should repeat.")][RemainingText] string repetition)
        {
            if (ctx.Channel.Id == 718162681554534511 || ctx.Channel.Id == 812755782067290162) //debate and venting
                return;
            if (repetition.ToUpper() == "I'M STUPID" || repetition.ToUpper() == "IM STUPID")
            {
                await ctx.Channel.SendMessageAsync("Tell us something new...");
            }
            else
            {
                DiscordMessageBuilder builder = new()
                {
                    Content = repetition,
                };
                await ctx.Channel.SendMessageAsync(builder);
            }
        }

        [Command("parth")]
        [Description("Parth!")]
        public async Task Parth(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Boss man! (even tho the whole senate technically are all equal but i am keeping this for the laughs)");
        }

        [Command("pat")]
        [Description("Pat someone")]
        public async Task Pat(CommandContext ctx, [Description("Who to pat")] DiscordMember member)
        {
            if (member.Id == 192037157416730625)
                await ctx.RespondAsync("Not gonna ping Lath, but im guessing he would pat back.");
            else if (member.Id == 395566758989135882)//neb
                await ctx.RespondAsync(new DiscordMessageBuilder().WithSticker(ctx.Guild.Stickers[964256150054899742]));
            else if (member.Id == 700373370491109489)//femke
                await ctx.RespondAsync(new DiscordMessageBuilder().WithSticker(ctx.Guild.Stickers[1013888253272801310]));
            else if (member.Id == 387325006176059394)//julian
                await ctx.RespondAsync($"Julian!, you just got pat by {ctx.Member.Mention}");
            else if (member.Id == ctx.Client.CurrentUser.Id)
                await ctx.RespondAsync(new DiscordMessageBuilder().WithSticker(ctx.Guild.Stickers[967807862007029880]));
            else
                await ctx.RespondAsync($"{member.Mention} you just got pat by {ctx.Member.Mention}.");

        }

        [Command("snipe")]
        [Description("Snipe a deleted message")]
        public async Task Snipe(CommandContext ctx)
        {
            if (ctx.Channel.Id == 812755782067290162) //venting
                return;

            if (!DiscordObjectService.Instance.LastDeletes.ContainsKey(ctx.Channel.Id))
                return;
            if (!DiscordObjectService.Instance.LastDeletes.TryGetValue(ctx.Channel.Id, out var lastDelete))
            {
                SystemService.Instance.Logger.Log($"Error while trying to get last deleted message in {ctx.Channel.Id}");
                return;
            }
            if ((lastDelete.Author.Id == 387325006176059394 && ctx.Member.Id != 387325006176059394) || lastDelete.Author.IsBot || lastDelete.Content.Contains("submit"))
            {
                await ctx.RespondAsync("No");
                return;
            }
            DiscordEmbedBuilder builder = new()
            {
                Title = "Boom headshot!",
                Description = lastDelete.Content,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = lastDelete.Author.AvatarUrl,
                    Name = lastDelete.Author.Username
                },
                Timestamp = lastDelete.Timestamp,
                Color = DiscordColor.Blurple
            };

            var message = new DiscordMessageBuilder();

            Dictionary<string, Stream> attachments = new();
            if (lastDelete.Attachments is not null && lastDelete.Attachments.Any())
            {
                using HttpClient httpClient = new();
                foreach (DiscordAttachment attachment in lastDelete.Attachments)
                {
                    attachments.Add(attachment.FileName, await httpClient.GetStreamAsync(attachment.ProxyUrl));
                    if (attachment.MediaType.Contains("image") && string.IsNullOrEmpty(builder.ImageUrl))
                        builder.WithImageUrl("attachment://" + attachment.FileName);
                }
                message.AddFiles(attachments);
            }

            await ctx.RespondAsync(message.WithEmbed(builder).WithAllowedMentions(Mentions.None));
            foreach (var attachment in attachments)
                attachment.Value.Close();
        }

        [Command("snipeedit")]
        [Aliases("se")]
        [Description("Snipe an edited message")]
        public async Task SnipeEdit(CommandContext ctx)
        {
            if (ctx.Channel.Id == 812755782067290162) //venting
                return;

            if (!DiscordObjectService.Instance.LastEdits.ContainsKey(ctx.Channel.Id))
            {
                return;
            }
            if (!DiscordObjectService.Instance.LastEdits.TryGetValue(ctx.Channel.Id, out var lastEdit))
            {
                SystemService.Instance.Logger.Log($"Error while trying to get last edited message in {ctx.Channel.Id}");
                return;
            }
            if ((lastEdit.Author.Id == 387325006176059394 && ctx.Member.Id != 387325006176059394) || lastEdit.Author.IsBot)
            {
                await ctx.RespondAsync("No");
                return;
            }
            DiscordEmbedBuilder builder = new()
            {
                Title = "Boom headshot!",
                Description = lastEdit.Content,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = lastEdit.Author.AvatarUrl,
                    Name = lastEdit.Author.Username
                },
                Timestamp = lastEdit.Timestamp,
                Color = DiscordColor.Blurple,
            };
            await ctx.Channel.SendMessageAsync(builder);
        }
    }
}
