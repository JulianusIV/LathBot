using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ArgumentModifiers;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MimeTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LathBotFront.Commands
{
    class TechnicalCommands
    {
        [Command("ping")]
        [Description("Pong")]
        public async Task Ping(CommandContext ctx)
            => await ctx.RespondAsync("My ping is on bloody " + ((int)ctx.Client.GetConnectionLatency(ctx.Guild.Id).TotalMilliseconds) + "ms");

        [Command("test")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task Test(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            using HttpClient httpClient = new();
            string content = await httpClient.GetStringAsync("https://api.nasa.gov/planetary/apod?api_key=" + ReadConfig.Config.NasaApiKey + "&thumbs=True");

            APODJsonObject json = JsonConvert.DeserializeObject<APODJsonObject>(content);

            var result = await httpClient.GetAsync(json.URL ?? json.ThumbnailUrl);
            var stream = result.Content.ReadAsStream();
            var contentType = result.Content.Headers.ContentType.MediaType;

            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Astronomy Picture of the day:\n" + json.Title)
                .WithDescription("**Explanation:\n**" + json.Explanation)
                .WithColor(new DiscordColor("e49a5e"))
                .WithImageUrl("attachment://apod" + MimeTypeMap.GetExtension(contentType))
                .AddField("Links:", json.HdUrl is null ? $"[Source Link]({json.URL})" : $"[Source Link]({json.HdUrl})\n[Low resolution source]({json.URL})")
                .WithFooter("Copyright: " + (json.Copyright is null ? "Public Domain" : json.Copyright) + "\nSource: NASA APOD API Endpoint");


            DiscordMessageBuilder builder = new DiscordMessageBuilder()
                .AddFile("apod" + MimeTypeMap.GetExtension(contentType), stream, AddFileOptions.CloseStream)
                .AddEmbed(embedBuilder);
            DiscordMessageBuilder builder2 = null;
            if (json.MediaType != "image")
                builder2 = new DiscordMessageBuilder().WithContent(json.URL.Replace("embed/", "watch?v=").Replace("?rel=0", ""));
            builder.WithAllowedMentions(Mentions.All);
            await ctx.RespondAsync(builder);
            if (builder2 is not null)
                await ctx.RespondAsync(builder2);
        }

        [Command("freeze")]
        [Description("Freeze a whole channel\n" +
            "Never to be supported channels:\nQuestions & Messages for Lathrix\nBooster-only")]
        [RequirePermissions(DiscordPermission.BanMembers)]
        public async Task ChFreeze(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            await this.FreezeChannel(ctx.Channel);
            await ctx.RespondAsync("```This channel is now frozen for moderation purposes.\n" +
                "You will be able to send messages again once the situation has been resolved```");
        }

        [Command("unfreeze")]
        [Description("Unfreeze a channel after it was previously frozen")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task ChUnFreeze(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            await this.UnfreezeChannel(ctx.Channel);
            await ctx.RespondAsync("```This channel is no longer frozen.\n" +
                "You can now send messages as normal```");
        }

        [Command("globalfreeze")]
        [Description("Freeze all public channels.")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task GlobalFreeze(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var toFreeze = new List<ulong>()
            {
                825415177415032943, //latest vid
				700346866595922020, //lath quotes
				700350465174405170, //gen1
				722915176067891230, //gen2
				699566205379543041, //memes
				721107980455641180, //chaos
				766463841059078205, //counting
				716457497464143993, //animals
				788102512455450654, //music
				699566218104799343, //bot commands
				765619584794361886, //ideas
				767037634710732831, //art
				815272200197636116, //photography
				718162681554534511, //debate
				812755782067290162, //vent
				759182971075952670, //cas1
				898738258371031041, //food
				833363182415904788, //study
				843189922377629746, //tech
				898731801063878676, //writing
				700265288641282068, //games gen
				701457089481932892, //ftd
				713369380125147168, //mc
				701457040769286174, //stellairs
				850029437998071838, //reassembly
				766322531757326346, //wh40k
				724313826786410508  //staff member comms
			};

            var channels = await ctx.Guild.GetChannelsAsync();

            foreach (var channel in channels.Where(x => toFreeze.Contains(x.Id)))
            {
                await this.FreezeChannel(channel);
                await channel.SendMessageAsync("```This channel is now frozen for moderation purposes.\n" +
                    "You will be able to send messages again once the situation has been resolved```");
            }

            await ctx.RespondAsync("Done!");
        }

        [Command("globalunfreeze")]
        [Description("Unfreeze all public channels.")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task GlobalUnfreeze(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var toFreeze = new List<ulong>()
            {
                825415177415032943, //latest vid
				700346866595922020, //lath quotes
				700350465174405170, //gen1
				722915176067891230, //gen2
				699566205379543041, //memes
				721107980455641180, //chaos
				766463841059078205, //counting
				716457497464143993, //animals
				788102512455450654, //music
				699566218104799343, //bot commands
				765619584794361886, //ideas
				767037634710732831, //art
				815272200197636116, //photography
				718162681554534511, //debate
				812755782067290162, //vent
				759182971075952670, //cas1
				898738258371031041, //food
				833363182415904788, //study
				843189922377629746, //tech
				898731801063878676, //writing
				700265288641282068, //games gen
				701457089481932892, //ftd
				713369380125147168, //mc
				701457040769286174, //stellairs
				850029437998071838, //reassembly
				766322531757326346, //wh40k
				724313826786410508  //staff member comms
			};

            var channels = await ctx.Guild.GetChannelsAsync();

            foreach (var channel in channels.Where(x => toFreeze.Contains(x.Id)))
            {
                await this.UnfreezeChannel(channel);
                await channel.SendMessageAsync("```This channel is no longer frozen.\n" +
                    "You can now send messages as normal```");
            }

            await ctx.RespondAsync("Done!");
        }

        [Command("togglequestions")]
        [Description("Closes/opens questions for lathrix channel")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task ToggleQuestions(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            var channel = await ctx.Guild.GetChannelAsync(721082217119612969);
            var perms = channel.PermissionOverwrites;
            var toChange = new List<ulong>()
            {
                850019046345801768, //amateur
				701844438153953420, //exp
				722064316479701083, //resp
				748533112404705281, //vet
				819296501451980820, //true
				707915540085342248, //god
				759456739669704784, //ungod
				769609970240061441, //dev
				749576790929965136, //booster
				702925135568437270  //booster
			};
            var open = perms.First(x => x.Id == 759456739669704784).Allowed.HasPermission(DiscordPermission.SendMessages);
            foreach (var change in toChange)
                await perms.First(x => x.Id == change).UpdateAsync(open ? DiscordPermissions.None : DiscordPermission.SendMessages,
                                                                   open ? DiscordPermission.SendMessages : DiscordPermissions.None);
            if (open)
                await channel.SendMessageAsync("Questions are currently closed due to a large amount piling up. " +
                    "Please be patient and wait until Lathrix has time to answer some again.");
            else
                await channel.SendMessageAsync("Questions are now open again!");

            await ctx.RespondAsync($"Questions channel is now {(open ? "opened" : "closed")}.");
        }

        [Command("toggleboard")]
        [Description("Disable/enable the goodguys board")]
        [RequirePermissions(DiscordPermission.KickMembers)]
        public async Task ToggleBoard(CommandContext ctx)
        {
            var repo = new VariableRepository(ReadConfig.Config.ConnectionString);
            repo.Read(4, out var status);
            var currentstatus = !bool.Parse(status.Value);
            status.Value = currentstatus.ToString();
            repo.Update(status);
            await ctx.RespondAsync($"Goodguys board is now {(!currentstatus ? "enabled" : "disabled")}.");
        }

        [Command("prune")]
        [Description("Kick all members without verified role")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task Prune(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();
            var members = ctx.Guild.GetAllMembersAsync();
            await foreach (DiscordMember member in members)
            {
                bool kick = true;
                foreach (DiscordRole role in member.Roles)
                {
                    if (role.Id == 767050052257447936 || role.Id == 699994970412679310)
                    {
                        kick = false;
                        break;
                    }
                }
                if (kick)
                    await member.RemoveAsync();
            }
            await ctx.RespondAsync("Done :(");
        }

        [Command("advert")]
        [Description("Send a request for a post to #〘📣〙advertisements")]
        public async Task Advert(SlashCommandContext ctx, [Description("The advertiser URL")] string url, [Description("A short description why one should check this out")][RemainingText] string adText)
        {
            await ctx.DeferResponseAsync(true);

            DiscordEmbedBuilder adEmbed = new()
            {
                Title = "Advert",
                Url = url,
                Description = adText + "\nClick on the title to go to the page!",
                Color = ctx.Member.Color,
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = ctx.Member.Username + "#" + ctx.User.Discriminator + " " + ctx.Member.Id,
                    IconUrl = ctx.Member.AvatarUrl
                }
            };

            DiscordRole plagueRole = await ctx.Guild.GetRoleAsync(796234634316873759);
            DiscordChannel plagueChannel = await ctx.Guild.GetChannelAsync(701499919248392222);
            DiscordMessageBuilder builder = new DiscordMessageBuilder()
                .WithContent($"Allow or deny this ad {plagueRole.Mention}")
                .AddEmbed(adEmbed.Build());
            builder.WithAllowedMention(RoleMention.All);
            DiscordMessage allowDenyMessage = await plagueChannel.SendMessageAsync(builder);

            await allowDenyMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"));
            await allowDenyMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:"));

            await ctx.RespondAsync("Request has been sent to the moderation team for review, please be patient!");

            InteractivityExtension interactivity = ctx.Client.ServiceProvider.GetService(typeof(InteractivityExtension)) as InteractivityExtension;

            var reaction = await interactivity.WaitForReactionAsync(x =>
            {
                if (x.Message != allowDenyMessage)
                    return false;
                var roles = ctx.Guild.GetMemberAsync(x.User.Id).Result.Roles;
                foreach (DiscordRole role in roles)
                    if (role.Id == 796234634316873759 || role.Id == 784852719449276467 || role.Id == 726212852267876435)
                        return true;
                return false;
            });

            if (reaction.Result.Emoji.ToString() == "❌")
            {
                await ctx.EditResponseAsync("Request has been denied!");
            }
            else if (reaction.Result.Emoji.ToString() == "✅")
            {
                await (await ctx.Guild.GetChannelAsync(787066481204527154)).SendMessageAsync((await ctx.Guild.GetRoleAsync(794975835235942470)).Mention, adEmbed.Build());
                await ctx.EditResponseAsync("Request has been approved!");
            }
            else if (reaction.TimedOut)
            {
                await ctx.EditResponseAsync("It seems like there is no moderator online right now.\nTry again later.");
            }
        }

        [Command("purgecell")]
        [Description("Purge parstapo-cell and log it to a file")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task PurgeCell(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);
            if (ctx.Channel.Id == 792486366138073180)
            {
                string channelContent = "";
                var messages = ctx.Channel.GetMessagesAsync(300).ToBlockingEnumerable();
                for (int index = messages.Count() - 1; index >= 0; index--)
                {
                    channelContent += messages.ElementAt(index).Timestamp +
                        " - " + $"{messages.ElementAt(index).Author.Username}#{messages.ElementAt(index).Author.Discriminator} ({messages.ElementAt(index).Author.Id})" + ": " +
                        messages.ElementAt(index).Content +
                        (messages.ElementAt(index).IsEdited ? $" (edited at {messages.ElementAt(index).EditedTimestamp})\n" : "\n");
                }
                File.WriteAllText("CellLog.txt", channelContent);
                await ctx.Channel.DeleteMessagesAsync(ctx.Channel.GetMessagesAsync(300));
                DiscordMessageBuilder builder = new();
                using FileStream stream = new("CellLog.txt", FileMode.Open);
                builder.AddFile(stream);
                await (await ctx.Guild.GetChannelAsync(722905404354592900)).SendMessageAsync(builder);
                await ctx.RespondAsync("Done!");
            }
            else
            {
                await ctx.RespondAsync("Not here...");
            }
        }

        [Command("boardcount")]
        [Description("Change how many reactions it takes to get on the good guys board")]
        [RequirePermissions(DiscordPermission.BanMembers)]
        public async Task BoardCount(CommandContext ctx, [Description("New limit")] int newCount)
        {
            await ctx.DeferResponseAsync();

            GoodGuysService.Instance.GoodGuysReactionCount = newCount;

            VariableRepository repo = new(ReadConfig.Config.ConnectionString);

            Variable entity = new() { ID = 1, Name = "Goodguys", Value = newCount.ToString() };

            bool result = repo.Update(entity);
            if (!result)
            {
                await ctx.RespondAsync("Error updating the database entry");
                return;
            }

            await ctx.RespondAsync("Set to " + newCount);
        }

        [Command("getcount")]
        [Description("see how much it currently takes to get on the GoodGuys board")]
        public async Task GetCount(CommandContext ctx)
            => await ctx.RespondAsync(GoodGuysService.Instance.GoodGuysReactionCount.ToString());

        [Command("clean")]
        [Description("Purge muted and log it to a file")]
        [RequirePermissions(DiscordPermission.BanMembers)]
        public async Task Clean(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);
            if (ctx.Channel.Id == 838088490704568341)
            {
                string channelContent = "";
                var messages = ctx.Channel.GetMessagesAsync(500).ToBlockingEnumerable();
                for (int index = messages.Count() - 1; index >= 0; index--)
                {
                    channelContent += messages.ElementAt(index).Timestamp +
                        " - " + $"{messages.ElementAt(index).Author.Username}#{messages.ElementAt(index).Author.Discriminator} ({messages.ElementAt(index).Author.Id})" + ": " +
                        messages.ElementAt(index).Content +
                        (messages.ElementAt(index).IsEdited ? $" (edited at {messages.ElementAt(index).EditedTimestamp})\n" : "\n");
                }
                File.WriteAllText("MuteLog.txt", channelContent);
                await ctx.Channel.DeleteMessagesAsync(ctx.Channel.GetMessagesAsync(500));
                DiscordMessageBuilder builder = new();
                using FileStream stream = new("MuteLog.txt", FileMode.Open);
                builder.AddFile(stream);
                await (await ctx.Guild.GetChannelAsync(838092779741642802)).SendMessageAsync(builder);
                DiscordMessage pin = await ctx.Channel.SendMessageAsync("-Rules-\n" +
                    "This channel is not for your own playground, do not use it for recreational purposes, it is only used for the plague guards, or senate to discuss your warn with you,\n" +
                    "-\n" +
                    "If there are or more than two of you muted don't have a conversation in the channel. if a plague guard or senate member starts a conversation with you then you may have a conversation.");
                await pin.PinAsync();
                await ctx.RespondAsync("Done!");
            }
            else
            {
                await ctx.RespondAsync("Not here...");
            }
        }

        [Command("eval")]
        [TextAlias("evalcs", "cseval", "roslyn")]
        [Description("Evaluates C# code.")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task EvalCS(TextCommandContext ctx, [RemainingText] string code)
        {
            if (ctx.User.Id != 387325006176059394)
            {
                await ctx.RespondAsync("I dont think i will");
                return;
            }
            var msg = ctx.Message;

            var cs1 = code.IndexOf("```") + 3;
            cs1 = code.IndexOf('\n', cs1) + 1;
            var cs2 = code.LastIndexOf("```");

            if (cs1 == -1 || cs2 == -1)
                throw new ArgumentException("You need to wrap the code into a code block.");

            var cs = code[cs1..cs2];

            msg = await ctx.Message.RespondAsync(embed: new DiscordEmbedBuilder()
                .WithColor(new DiscordColor("#FF007F"))
                .WithDescription("Evaluating...")
                .Build());

            try
            {
                var globals = new TestVariables(ctx.Message, ctx.Client, ctx);

                var sopts = ScriptOptions.Default;
                sopts = sopts.WithImports("System", "System.Collections.Generic", "System.Linq", "System.Text", "System.Threading.Tasks", "DSharpPlus", "DSharpPlus.Commands", "DSharpPlus.Interactivity", "Microsoft.Extensions.Logging");
                sopts = sopts.WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location)));

                var script = CSharpScript.Create(cs, sopts, typeof(TestVariables));
                script.Compile();
                var result = await script.RunAsync(globals);

                if (result != null && result.ReturnValue != null && !string.IsNullOrWhiteSpace(result.ReturnValue.ToString()))
                    await msg.ModifyAsync(embed: new DiscordEmbedBuilder { Title = "Evaluation Result", Description = result.ReturnValue.ToString(), Color = new DiscordColor("#007FFF") }.Build());
                else
                    await msg.ModifyAsync(embed: new DiscordEmbedBuilder { Title = "Evaluation Successful", Description = "No result was returned.", Color = new DiscordColor("#007FFF") }.Build());
            }
            catch (Exception ex)
            {
                await msg.ModifyAsync(embed: new DiscordEmbedBuilder { Title = "Evaluation Failure", Description = string.Concat("**", ex.GetType().ToString(), "**: ", ex.Message), Color = new DiscordColor("#FF0000") }.Build());
            }
        }

        private async Task FreezeChannel(DiscordChannel channel)
        {
            IReadOnlyList<DiscordOverwrite> perms = channel.PermissionOverwrites;
            List<ulong> rolemeIds =
            [
                767039219403063307, //art
                718162129609556121, //debate
                812755886413971499, //venting
                701454853095817316, //ftd
                713367380574732319, //mc
                701454772900855819, //stellairs
                850029252812210207, //reassembly
                766322672321560628, //wh40k
                765622563338453023, //daily quote
                741342066021367938, //daily question
                702960377390039152  //nomic
            ];
            if (perms.Any(x => rolemeIds.Contains(x.Id)))
                await perms.First(x => x.Id == 767050052257447936).UpdateAsync(DiscordPermissions.None, DiscordPermission.SendMessages);
            if (channel.Id == 766463841059078205) //counting
                await perms.First(x => x.Id == 767050052257447936).UpdateAsync(DiscordPermissions.None, DiscordPermission.SendMessages);
            else
                await perms.First(x => x.Id == 767050052257447936).UpdateAsync(DiscordPermission.ViewChannel, DiscordPermission.SendMessages);
        }

        private async Task UnfreezeChannel(DiscordChannel channel)
        {
            IReadOnlyList<DiscordOverwrite> perms = channel.PermissionOverwrites;
            List<ulong> rolemeIds =
            [
                767039219403063307, //art
                718162129609556121, //debate
                812755886413971499, //venting
                701454853095817316, //ftd
                713367380574732319, //mc
                701454772900855819, //stellairs
                850029252812210207, //reassembly
                766322672321560628, //wh40k
                765622563338453023, //daily quote
                741342066021367938, //daily question
                702960377390039152  //nomic
            ];
            if (perms.Any(x => rolemeIds.Contains(x.Id)))
                await perms.First(x => x.Id == 767050052257447936).UpdateAsync(DiscordPermission.SendMessages,
                    channel.Id == 718162681554534511 ? DiscordPermission.AttachFiles | DiscordPermission.EmbedLinks : DiscordPermissions.None); //if debate, no embeds or files
            if (channel.Id == 766463841059078205) //counting
                await perms.First(x => x.Id == 767050052257447936).UpdateAsync(DiscordPermissions.None, DiscordPermission.EmbedLinks | DiscordPermission.AttachFiles);
            else
                await perms.First(x => x.Id == 767050052257447936).UpdateAsync(DiscordPermission.ViewChannel | DiscordPermission.SendMessages, DiscordPermissions.None);
        }
    }
    public class TestVariables
    {
        public DiscordMessage Message { get; set; }
        public DiscordChannel Channel { get; set; }
        public DiscordGuild Guild { get; set; }
        public DiscordUser User { get; set; }
        public DiscordMember Member { get; set; }
        public CommandContext Context { get; set; }

        public TestVariables(DiscordMessage msg, DiscordClient client, CommandContext ctx)
        {
            this.Client = client;

            this.Message = msg;
            this.Channel = msg.Channel;
            this.Guild = this.Channel.Guild;
            this.User = this.Message.Author;
            if (this.Guild != null)
                this.Member = this.Guild.GetMemberAsync(this.User.Id).GetAwaiter().GetResult();
            this.Context = ctx;
        }

        public DiscordClient Client;
    }
}
