using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;

using LathBotBack;
using LathBotBack.Enums;

namespace LathBotFront.Commands
{
    public class LavalinkCommands : BaseCommandModule
    {
        [Command("play")]
        [Description("Search YouTube for music")]
        public async Task Play(CommandContext ctx, [Description("The search request")][RemainingText] string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a VC.");
                return;
            }
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            await node.ConnectAsync(ctx.Member.VoiceState.Channel);
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected.");
                return;
            }
            LavalinkLoadResult loadResult = await node.Rest.GetTracksAsync(search, LavalinkSearchType.Youtube);
            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}");
                return;
            }
            DiscordEmbedBuilder resultsEmbed = new DiscordEmbedBuilder
            {
                Title = "I found this for you on Youtube:",
                Description = "Respond with the number you would like to play!",
                Color = DiscordColor.Red
            };
            int index = 0;
            foreach (LavalinkTrack result in loadResult.Tracks)
            {
                index++;
                resultsEmbed.AddField($"{index}:", $"[{result.Title}]({result.Uri})");
                if (index == 10)
                    break;
            }
            DiscordMessage selectOne = await ctx.RespondAsync(resultsEmbed.Build());
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            var selection = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User);
            LavalinkTrack track = loadResult.Tracks.ElementAt(int.Parse(selection.Result.Content) - 1);
            if (Holder.Instance.Queues != null && Holder.Instance.Queues.ContainsKey(ctx.Guild))
            {
                Holder.Instance.Queues[ctx.Guild].Add(track);
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
                {
                    Title = "Added to playlist:",
                    Description = $"[{track.Title}]({track.Uri})",
                    Color = DiscordColor.Red
                };
                await ctx.RespondAsync(embedBuilder.Build());
            }
            else if (conn.CurrentState.CurrentTrack != null)
            {
                if (Holder.Instance.Queues == null)
                {
                    Holder.Instance.Queues = new Dictionary<DiscordGuild, List<LavalinkTrack>>();
                }
                Holder.Instance.Queues.Add(ctx.Guild, new List<LavalinkTrack> { track });
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
                {
                    Title = "Added to playlist:",
                    Description = $"[{track.Title}]({track.Uri})",
                    Color = DiscordColor.Red
                };
                await ctx.RespondAsync(embedBuilder.Build());
            }
            else
            {
                await conn.PlayAsync(track);
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
                {
                    Title = "Now playing:",
                    Description = $"[{track.Title}]({track.Uri})",
                    Color = DiscordColor.Red
                };
                await ctx.RespondAsync(embedBuilder.Build());
            }
            await selectOne.DeleteAsync();
            await selection.Result.DeleteAsync();
        }

        [Command("playsc")]
        [Description("Search SoundCloud for music")]
        public async Task PlaySoundCloud(CommandContext ctx, [Description("The search request")][RemainingText] string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a VC.");
                return;
            }
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            await node.ConnectAsync(ctx.Member.VoiceState.Channel);
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected.");
                return;
            }
            LavalinkLoadResult loadResult = await node.Rest.GetTracksAsync(search, LavalinkSearchType.SoundCloud);
            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed
                || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}");
                return;
            }
            DiscordEmbedBuilder resultsEmbed = new DiscordEmbedBuilder
            {
                Title = "I found this for you on Youtube:",
                Description = "Respond with the number you would like to play!",
                Color = DiscordColor.Orange
            };
            int index = 0;
            foreach (LavalinkTrack result in loadResult.Tracks)
            {
                index++;
                resultsEmbed.AddField($"{index}:", $"[{result.Title}]({result.Uri})");
                if (index == 10)
                    break;
            }
            DiscordMessage selectOne = await ctx.RespondAsync(resultsEmbed.Build());
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            var selection = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User);
            LavalinkTrack track = loadResult.Tracks.ElementAt(int.Parse(selection.Result.Content));
            if (Holder.Instance.Queues.ContainsKey(ctx.Guild))
            {
                Holder.Instance.Queues[ctx.Guild].Add(track);
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
                {
                    Title = "Added to playlist:",
                    Description = $"[{track.Title}]({track.Uri})",
                    Color = DiscordColor.Orange
                };
                await ctx.RespondAsync(embedBuilder.Build());
            }
            else if (conn.CurrentState.CurrentTrack != null)
            {
                Holder.Instance.Queues.Add(ctx.Guild, new List<LavalinkTrack> { track });
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
                {
                    Title = "Added to playlist:",
                    Description = $"[{track.Title}]({track.Uri})",
                    Color = DiscordColor.Orange
                };
                await ctx.RespondAsync(embedBuilder.Build());
            }
            else
            {
                await conn.PlayAsync(track);
                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
                {
                    Title = "Now playing:",
                    Description = $"[{track.Title}]({track.Uri})",
                    Color = DiscordColor.Orange
                };
                await ctx.RespondAsync(embedBuilder.Build());
            }
            await selectOne.DeleteAsync();
            await selection.Result.DeleteAsync();
        }

        [Command("import")]
        [Description("Import a playlist from YouTube to the queue")]
        public async Task Import(CommandContext ctx, [Description("Playlist URL")] Uri playlistURL)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a vc.");
                return;
            }
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            await node.ConnectAsync(ctx.Member.VoiceState.Channel);
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected.");
                return;
            }
            LavalinkLoadResult loadResult = await conn.GetTracksAsync(playlistURL);
            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed || loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Playlist search failed for {playlistURL}");
                return;
            }

            foreach (LavalinkTrack track in loadResult.Tracks)
            {
                if (Holder.Instance.Queues != null && Holder.Instance.Queues.ContainsKey(ctx.Guild))
                    Holder.Instance.Queues[ctx.Guild].Add(track);
                else if (conn.CurrentState.CurrentTrack != null)
                {
                    Holder.Instance.Queues = new Dictionary<DiscordGuild, List<LavalinkTrack>>
                    {
                        { ctx.Guild, new List<LavalinkTrack> { track } }
                    };
                }
                else
                {
                    await conn.PlayAsync(track);
                    DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
                    {
                        Title = "Now playing:",
                        Description = $"[{track.Title}]({track.Uri})",
                        Color = DiscordColor.Red
                    };
                    await ctx.RespondAsync(embedBuilder.Build());
                    await Task.Delay(1000);
                }
            }
            await ctx.RespondAsync("Done (i think).");
        }

        [Command("shuffle")]
        [Description("Shuffle the queue")]
        public async Task Shuffle(CommandContext ctx)
        {
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState?.Guild);
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel != conn.Channel)
            {
                await ctx.RespondAsync("We are not in one VC");
                return;
            }
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("Nothing playing.");
                return;
            }
            Random random = new Random();
            for (int index = Holder.Instance.Queues[conn.Guild].Count; index > 0; index--)
            {
                int rngRes = random.Next(0, index);
                LavalinkTrack temp = Holder.Instance.Queues[conn.Guild][0];
                Holder.Instance.Queues[conn.Guild][0] = Holder.Instance.Queues[conn.Guild][rngRes];
                Holder.Instance.Queues[conn.Guild][rngRes] = temp;
            }
        }

        [Command("queue")]
        [Description("Display queue")]
        public async Task Queue(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a vc.");
                return;
            }
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("Nothing playing");
                return;
            }
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Title = "Queue: ",
                Description = "Showing all in queue",
                Color = DiscordColor.Red
            };
            builder.AddField($"1: Channel: {conn.CurrentState.CurrentTrack.Author}",
                $"[{conn.CurrentState.CurrentTrack.Title}]({conn.CurrentState.CurrentTrack.Uri}) - {conn.CurrentState.CurrentTrack.Length}");
            List<Page> pages = new List<Page>();
            int index = 1;
            int counter = 1;
            if (!Holder.Instance.Queues.ContainsKey(conn.Guild))
            {
                await ctx.RespondAsync(builder.Build());
                return;
            }
            foreach (LavalinkTrack track in Holder.Instance.Queues[conn.Guild])
            {
                index++;
                counter++;
                builder.AddField($"{index}: Channel: {track.Author}", $"[{track.Title}]({track.Uri}) - {track.Length}");
                if (counter == 10)
                {
                    pages.Add(new Page { Embed = builder.Build() });
                    builder.ClearFields();
                    counter = 0;
                }
                else if (index == Holder.Instance.Queues[conn.Guild].Count + 1)
                {
                    pages.Add(new Page { Embed = builder.Build() });
                }
            }
            await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages);
        }

        [Command("np")]
        [Description("Display currently playing song")]
        public async Task NowPlaying(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a vc.");
                return;
            }
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("Nothing playing");
                return;
            }
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder
            {
                Title = "Playing: ",
                Description = "Showing currently playing song",
                Color = DiscordColor.Red
            };
            builder.AddField($"1: Channel: {conn.CurrentState.CurrentTrack.Author}",
                $"[{conn.CurrentState.CurrentTrack.Title}]({conn.CurrentState.CurrentTrack.Uri}) - {conn.CurrentState.CurrentTrack.Length}");
            await ctx.RespondAsync(builder.Build());
            return;
        }

        [Command("skip")]
        [Description("Skip currently playing song")]
        public async Task Skip(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a vc.");
                return;
            }
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("Nothing playing");
                return;
            }
            await conn.StopAsync();
        }

        [Command("repeat")]
        [Description("Repeat one or all tracks of current queue.")]
        public async Task Repeat(CommandContext ctx, [RemainingText][Description("Repeatmode (\"single\" or \"all\")")] string mode)
		{
			if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
			{
                await ctx.RespondAsync("You are not in a vc.");
                return;
			}
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("Nothing playing");
                return;
            }
            await ctx.TriggerTypingAsync();
			var repeatMode = mode switch
			{
				string a when a.ToLower().Contains("all") => Repeaters.all,
				string b when b.ToLower().Contains("single") => Repeaters.single,
				_ => Repeaters.all,
			};
			Holder.Instance.Repeats.Remove(ctx.Guild);
            Holder.Instance.Repeats.Add(ctx.Guild, repeatMode);
		}

        [Command("pause")]
        [Description("Pause playback")]
        public async Task Pause(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a vc.");
                return;
            }
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("Nothing playing");
                return;
            }
            await conn.PauseAsync();
            await ctx.RespondAsync("Paused");
        }

        [Command("resume")]
        [Description("Resume playback")]
        public async Task Resume(CommandContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a vc.");
                return;
            }
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("Nothing playing");
                return;
            }
            await conn.ResumeAsync();
            await ctx.RespondAsync("Resuming");
        }

        [Command("stop")]
        [Aliases("leave")]
        [Description("Stop playback and leave channel")]
        //[RequireRoles(RoleCheckMode.Any, "Bot Management", "Community Manager (OWN)", "Senate of Lathland (ADM)", "Plague Guard (Mods)")]
        public async Task Stop(CommandContext ctx)
        {
            LavalinkNodeConnection node = ctx.Client.GetLavalink().ConnectedNodes.Values.First();
            LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
			if (ctx.Member.VoiceState.Channel != conn.Channel)
			{
                await ctx.RespondAsync("You are not in a VC with me.");
                return;
			}
            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink not connected");
                return;
            }
            if (conn.CurrentState.CurrentTrack == null)
            {
                await ctx.RespondAsync("Nothing playing");
                return;
            }
            try
            {
                Holder.Instance.Queues.Remove(ctx.Guild);
            }
            catch { }
            await conn.StopAsync();
            await conn.DisconnectAsync();
            await ctx.RespondAsync("Bye");
        }
    }
}
