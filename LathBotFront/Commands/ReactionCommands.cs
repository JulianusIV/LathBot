using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using LathBotBack.Services;
using System;

namespace LathBotFront.Commands
{
	class ReactionCommands : BaseCommandModule
    {
        [Command("blood")]
        [Description("Give someone blood so they can [insert order here].")]
        public async Task Blood(CommandContext ctx, [Description("The person you want to give blood to.")] DiscordMember target, [Description("What they should do with the blood.")][RemainingText] string reason)
        {
            DiscordMessageBuilder builder = new DiscordMessageBuilder
            {
                Content = $"Hey {target.Mention}, you just got some blood from {ctx.Member.Mention} so you can{reason}!"
            };
            await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
        }

        [Command("slap")]
        [Description("Slap someone, but be careful who you choose to slap, they might slap harder!\n" +
            "Inspired by a xelA tag originally creatd by @ThaTheoMans#7006.")]
        public async Task Slap(CommandContext ctx, [Description("Who you want to slap.")] DiscordMember target)
        {
            if (target.Id == 708083256439996497) //LathBot
            {
                if (ctx.Member.Id != 387325006176059394) //myself
                    await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you can't slap me! *slap*").ConfigureAwait(false);
                else
                    await ctx.Channel.SendMessageAsync($"God dammit, *slaps myself* I've made a mistake again! *slaps myself*").ConfigureAwait(false);
            }
            else if (target.Id == 387325006176059394) //myself
            {
                await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you can't slap my creator or imma slap you! *slap*").ConfigureAwait(false);
            }
            else if (target.Id == 192037157416730625) //Lathrix
            {
                await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* why would you slap a god? *slap*").ConfigureAwait(false);
            }
            else if (target.Id == 241445303960600576) //Theo
            {
                if (ctx.Member.Id == 387325006176059394)
                    await ctx.Channel.SendMessageAsync($"You might think you are immune to slaps {target.Mention}, *slap* but {ctx.Member.Mention} is immune to counter slapping! *slap*").ConfigureAwait(false);
                else
                    await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* do you want to be nuzzled by a catboy?! *slap* ÒwÓ");
            }
            else if (target.Id == 700373370491109489) //Ava
            {
                if (ctx.Member.Id == 387325006176059394)
                    await ctx.Channel.SendMessageAsync($"You might think you are immune to slaps {target.Mention}, *slap* but {ctx.Member.Mention} is immune to counter slapping! *slap*").ConfigureAwait(false);
                else
                    await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you wanna get ~~mini~~modded? *slap*").ConfigureAwait(false);
            }
            else if (target.Id == 289112287250350080) //Parth
            {
                await ctx.Channel.SendMessageAsync($"God dammit {ctx.Member.Mention}, *slap* you wanna get yelled at? *slap*").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"God dammit {target.Mention}, *slap* you've made a mistake again! *slap*").ConfigureAwait(false);
            }
        }

        [Command("poke")]
        [Description("Poke someone! Made possible by Dodo™")]
        public async Task Poke(CommandContext ctx, [Description("Who you want to poke.")] DiscordUser target)
        {
            if (target.Id == 192037157416730625) //Lathrix
            {
                await ctx.Channel.SendMessageAsync($"Don`t poke Lath {ctx.Member.Mention}... he might be drunk and fall over... (god i hope i dont get killed for that joke)").ConfigureAwait(false);
            }
            else
            {
                await ctx.Channel.SendMessageAsync($"Hey {target.Mention} you just got poked by {ctx.Member.DisplayName}!").ConfigureAwait(false);
            }

        }

        [Command("repeat")]
        [Description("Let the Bot repeat something.")]
        public async Task Repeat(CommandContext ctx, [Description("What the bot should repeat.")][RemainingText] string repetition)
        {
            if (ctx.Channel.Id == 718162681554534511)
                return;
            if (repetition.ToUpper() == "I'M STUPID" || repetition.ToUpper() == "IM STUPID")
            {
                await ctx.Channel.SendMessageAsync("Tell us something new...").ConfigureAwait(false);
            }
            else
            {
                DiscordMessageBuilder builder = new DiscordMessageBuilder
                {
                    Content = repetition,
                };
                await ctx.Channel.SendMessageAsync(builder);
            }
        }

        [Command("chewy")]
        [Description("Chewy!")]
        public async Task Chewy(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Boss man! (even tho the whole senate technically are all equal but i am keeping this for the laughs)");
        }

        [Command("repent")]
        [Description("REPENT FOR YOUR SINS HERETIC!")]
        public async Task Repent(CommandContext ctx, [RemainingText] string message)
        {
            DiscordMessageBuilder builder = new DiscordMessageBuilder
            {
                Content = $"Thou shalt now REPENT {message}"
            };
            await ctx.Channel.SendMessageAsync(builder);
        }

        [Command("snipe")]
        [Description("Snipe a deleted message")]
        public async Task Snipe(CommandContext ctx)
		{
            if (!DiscordObjectService.Instance.LastDeletes.ContainsKey(ctx.Channel.Id))
                return;
            if (!DiscordObjectService.Instance.LastDeletes.TryGetValue(ctx.Channel.Id, out var lastDelete))
			{
                SystemService.Instance.Logger.Log($"Error while trying to get last deleted message in {ctx.Channel.Id}");
                return;
			}
            if (lastDelete.Author.Id == 387325006176059394 || lastDelete.Author.IsBot)
			{
                await ctx.RespondAsync("No");
                return;
			}
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Title = "Boom headshot!",
                Description = lastDelete.Content,
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = lastDelete.Author.AvatarUrl,
                    Name = lastDelete.Author.Username
                },
                Timestamp = lastDelete.Timestamp,
                Color = DiscordColor.Blurple,
            };
            await ctx.Channel.SendMessageAsync(builder);
		}

        [Command("snipeedit")]
        [Aliases("se")]
        [Description("Snipe an edited message")]
        public async Task SnipeEdit(CommandContext ctx)
        {
			if (!DiscordObjectService.Instance.LastEdits.ContainsKey(ctx.Channel.Id))
			{
                return;
			}
            if (!DiscordObjectService.Instance.LastEdits.TryGetValue(ctx.Channel.Id, out var lastEdit))
            {
                SystemService.Instance.Logger.Log($"Error while trying to get last edited message in {ctx.Channel.Id}");
                return;
            }
            if (lastEdit.Author.Id == 387325006176059394 || lastEdit.Author.IsBot)
            {
                await ctx.RespondAsync("No");
                return;
            }
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
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
