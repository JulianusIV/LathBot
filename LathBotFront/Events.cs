using System;
using System.IO;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Lavalink.EventArgs;

using LathBotBack;
using LathBotBack.Repos;
using LathBotBack.Models;
using LathBotBack.Config;
using LathBotBack.Logging;
using LathBotBack.Enums;
using LathBotBack.Base;
using LathBotBack.Services;

namespace LathBotFront
{
	internal class Events
	{
		internal static Task OnClientReady(DiscordClient sender, ReadyEventArgs _)
		{
			//DiscordActivity activity = new DiscordActivity("a Turtle.", ActivityType.Streaming)
			//{
			//	StreamUrl = "https://www.twitch.tv/lathland"
			//};
			DiscordActivity activity = new DiscordActivity("flamethrower music.", ActivityType.ListeningTo);
			sender.UpdateStatusAsync(activity);
			return Task.CompletedTask;
		}

		internal static Task Client_GuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs _1)
		{
			_ = Task.Run(async () =>
			{
				BaseService.InitAll(sender);

				int added = 0;
				UserRepository repo = new UserRepository(ReadConfig.Config.ConnectionString);

				bool result = repo.GetAll(out List<User> list);
				if (!result)
				{
					_ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error getting all users in database.");
					return;
				}
				IReadOnlyCollection<DiscordMember> allMembers = await DiscordObjectService.Instance.Lathland.GetAllMembersAsync();
				IEnumerable<DiscordMember> toAdd = allMembers.Where(x => !list.Any(y => y.DcID == x.Id));
				foreach (var item in toAdd)
				{
					User entity = new User { DcID = item.Id };
					DiscordMember mem = await DiscordObjectService.Instance.Lathland.GetMemberAsync(item.Id);
					result = repo.Create(ref entity);
					if (!result)
					{
						_ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync($"Error adding user {item.DisplayName}#{item.Discriminator} ({item.Id}) to the database");
						continue;
					}
					_ = DiscordObjectService.Instance.TimerChannel.SendMessageAsync($"Added user {mem.DisplayName}#{mem.Discriminator} ({mem.Id}) on startup");
					added++;
				}
				bool res = repo.CountAll(out int allInDb);
				string strAllInDb;
				if (!res)
				{
					await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error counting all users in database");
					strAllInDb = "unknown";
				}
				else
				{
					strAllInDb = allInDb.ToString();
				}
				await DiscordObjectService.Instance.TimerChannel.SendMessageAsync(
					added > 0 ?
					$"Added {added} Users, {strAllInDb} entries in database, {DiscordObjectService.Instance.Lathland.MemberCount} members in guild." :
#if DEBUG
					"Test configuration startup completed");
#else
					"Startup completed");
#endif
			});
			return Task.CompletedTask;
		}

		internal static Task ComponentTriggered(DiscordClient _1, ComponentInteractionCreateEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				if (e.Id == "lb_server_verification")
				{
					DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
					await member.GrantRoleAsync(e.Guild.GetRole(767050052257447936));
					await member.GrantRoleAsync(e.Guild.GetRole(699562710144385095));
					await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
						new DiscordInteractionResponseBuilder
						{
							Content = "You are now verified and can access the rest of the channels.\n\n" +
							"Make sure to visit <#767098427225145365> to unlock some more channels that you like.",
							IsEphemeral = true
						});
				}
			});
			return Task.CompletedTask;
		}

		internal static Task MessageCreated(DiscordClient _1, MessageCreateEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				if (e.Channel.Id == 713284112638672917 || e.Channel.Id == 720543453376937996)
				{
					await e.Channel.CrosspostMessageAsync(e.Message).ConfigureAwait(false);
					return;
				}
				if (e.Author.Id == 708083256439996497)
					return;
				if (e.Channel.IsPrivate)
					return;
				if (!StartupService.Instance.StartUpCompleted)
					return;
				if (e.Channel.Id == 838088490704568341 && e.Guild.GetMemberAsync(e.Author.Id).Result.Roles.Contains(e.Guild.GetRole(701446136208293969)))
				{
					string pattern = @"((http:\/\/|https:\/\/)?(www.)?(([a-zA-Z0-9-]){2,}\.){1,16}([a-zA-Z]){2,24}(\/([a-zA-Z-_\/\.0-9#:?=&;,]*)?)?)";
					Regex rg = new Regex(pattern);
					if (rg.Matches(e.Message.Content).Any())
					{
						await e.Message.DeleteAsync();
						await e.Channel.SendMessageAsync("No links allowed in here.");
					}
				}
				if (e.Channel.Id == DiscordObjectService.Instance.QuestionsChannel.Id || e.Channel.Id == DiscordObjectService.Instance.StaffChannel.Id)
				{
					await e.Channel.TriggerTypingAsync();
					if (e.Channel.Id == DiscordObjectService.Instance.QuestionsChannel.Id)
					{
						await DiscordObjectService.Instance.LathQuestions.DeleteAsync();
						DiscordObjectService.Instance.LathQuestions = await DiscordObjectService.Instance.QuestionsChannel.SendMessageAsync(DiscordObjectService.Instance.LathQuestionsEmbed);
					}
					else if (e.Channel.Id == DiscordObjectService.Instance.StaffChannel.Id)
					{
						await DiscordObjectService.Instance.StaffQuestions.DeleteAsync();
						DiscordObjectService.Instance.StaffQuestions = await DiscordObjectService.Instance.StaffChannel.SendMessageAsync(DiscordObjectService.Instance.StaffQuestionsEmbed);
					}
				}
				if (e.Message.Content.Contains("##"))
				{
					int begin = e.Message.Content.IndexOf("##") + 2;
					if (char.IsDigit(e.Message.Content.ElementAt(begin)))
					{
						int end = 0;
						for (int i = begin + 1; i < e.Message.Content.Length - 1; i++)
						{
							if (!char.IsDigit(e.Message.Content.ElementAt(i)))
							{
								end = i - 1;
								break;
							}
						}
						if (end == 0)
							end = e.Message.Content.Length - 1;
						await e.Message.RespondAsync("https://github.com/JulianusIV/LathBot/issues/" + e.Message.Content[begin..(end + 1)]);
					}

				}
			});
			return Task.CompletedTask;
		}

		internal static Task MessageUpdated(DiscordClient _1, MessageUpdateEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				if (!StartupService.Instance.StartUpCompleted)
					return;

				if (DiscordObjectService.Instance.LastEdits.ContainsKey(e.Channel.Id))
				{
					DiscordObjectService.Instance.LastEdits[e.Channel.Id] = e.MessageBefore;
				}
				else
				{
					DiscordObjectService.Instance.LastEdits.Add(e.Channel.Id, e.MessageBefore);
				}
				if (e.Guild.GetMemberAsync(e.Author.Id).Result.Roles.Contains(e.Guild.GetRole(701446136208293969)) && e.Channel.Id == 726046413816987709)
				{
					string pattern = @"((http:\/\/|https:\/\/)?(www.)?(([a-zA-Z0-9-]){2,}\.){1,16}([a-zA-Z]){2,24}(\/([a-zA-Z-_\/\.0-9#:?=&;,]*)?)?)";
					Regex rg = new Regex(pattern);
					if (rg.Matches(e.Message.Content).Any())
					{
						await e.Message.DeleteAsync();
						await e.Channel.SendMessageAsync("No links allowed in here, Not even in edits! >:(");
					}
				}
			});
			return Task.CompletedTask;
		}

		internal static Task MessageDeleted(DiscordClient _1, MessageDeleteEventArgs e)
		{
			_ = Task.Run(() =>
			{
				if (!StartupService.Instance.StartUpCompleted)
					return;
				if (e.Guild.Id != 699555747591094344)
					return;
				if (DiscordObjectService.Instance.LastDeletes.ContainsKey(e.Channel.Id))
				{
					DiscordObjectService.Instance.LastDeletes[e.Channel.Id] = e.Message;
				}
				else
				{
					DiscordObjectService.Instance.LastDeletes.Add(e.Channel.Id, e.Message);
				}
			});
			return Task.CompletedTask;
		}

		internal static Task MemberAdded(DiscordClient _1, GuildMemberAddEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				if (!StartupService.Instance.StartUpCompleted)
				{
					return;
				}
				UserRepository repo = new UserRepository(ReadConfig.Config.ConnectionString);
				bool result = repo.ExistsDcId(e.Member.Id, out bool exists);
				if (!result)
				{
					await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync($"Error adding member {e.Member.DisplayName}#{e.Member.Discriminator} ({e.Member.Id})");
					return;
				}
				if (!exists)
				{
					User user = new User { DcID = e.Member.Id };
					result = repo.Create(ref user);
					if (!result)
					{
						await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync($"Error adding member {e.Member.DisplayName}#{e.Member.Discriminator} ({e.Member.Id})");
						return;
					}
					await DiscordObjectService.Instance.TimerChannel.SendMessageAsync($"Added user {e.Member.DisplayName}#{e.Member.Discriminator} ({e.Member.Id}) on join");
				}
			});
			return Task.CompletedTask;
		}

		internal static Task ReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				if (!StartupService.Instance.StartUpCompleted)
				{
					return;
				}
				DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
				if (e.Message.Id == 767050733677314069 && !member.Roles.Contains(e.Guild.GetRole(767050052257447936)))
				{
					if (e.Emoji.ToString() == "✅")
					{
						await member.GrantRoleAsync(e.Guild.GetRole(767050052257447936));
						await member.GrantRoleAsync(e.Guild.GetRole(699562710144385095));
					}
				}
				else if (e.Message.Id == 767100276028342322)
				{
					switch (e.Emoji.ToString())
					{
						case "⭐":
							await member.GrantRoleAsync(e.Guild.GetRole(701454772900855819)); //Stellaris
							break;
						case "⛵":
							await member.GrantRoleAsync(e.Guild.GetRole(701454853095817316)); //FTD
							break;
						case "⛏️":
							await member.GrantRoleAsync(e.Guild.GetRole(713367380574732319)); //MC
							break;
						case "🌍":
							await member.GrantRoleAsync(e.Guild.GetRole(701454812721446912)); //TT
							break;
						case "🔨":
							await member.GrantRoleAsync(e.Guild.GetRole(766322672321560628)); //WH40K
							break;
						case "📜":
							await member.GrantRoleAsync(e.Guild.GetRole(718162129609556121)); //Debate
							break;
						case "🎭":
							await member.GrantRoleAsync(e.Guild.GetRole(725046250843668490)); //RP
							break;
						case "💮":
							await member.GrantRoleAsync(e.Guild.GetRole(734215957446131733)); //Anime
							break;
						case "❓":
							await member.GrantRoleAsync(e.Guild.GetRole(741342066021367938)); //DQuestion
							break;
						case "💬":
							await member.GrantRoleAsync(e.Guild.GetRole(765622563338453023)); //DQuote
							break;
						case "💰":
							await member.GrantRoleAsync(e.Guild.GetRole(767035347367362590)); //Cas
							break;
						case "🎨":
							await member.GrantRoleAsync(e.Guild.GetRole(767039219403063307)); //Art
							break;
						case "📢":
							await member.GrantRoleAsync(e.Guild.GetRole(794975835235942470)); //Ads
							break;
						case "💢":
							await member.GrantRoleAsync(e.Guild.GetRole(812755886413971499)); //Vent
							break;
						case "🕒":
							await member.GrantRoleAsync(e.Guild.GetRole(821483418491289622)); //History
							break;
						case "❗":
							await member.GrantRoleAsync(e.Guild.GetRole(848307821703200828)); //Facts
							break;
						case "👾":
							await member.GrantRoleAsync(e.Guild.GetRole(850029252812210207)); //Reassembly
							break;
						default:
							break;
					}
				}
				else if (e.Emoji.Name == "TheGoodGuys")
				{
					if (e.Channel.Id == 795654190143766578 || //Goodguyschannel
					e.Channel.Id == 713284112638672917 || //YT
					e.Channel.Id == 720543453376937996) //Twitter
						return;
					if (e.Message.Timestamp < DateTime.Now - TimeSpan.FromHours(2))
						return;
					var reacts = await e.Message.GetReactionsAsync(DiscordEmoji.FromGuildEmote(sender, 723564837338349578));
					if (reacts.Count >= GoodGuysService.Instance.GoodGuysReactionCount)
					{
						IReadOnlyList<DiscordMessage> messages = await DiscordObjectService.Instance.GoodGuysChannel.GetMessagesAsync(20);

						foreach (DiscordMessage entry in messages)
							if (entry.Content.Contains(e.Message.Id.ToString()))
								return;
						DiscordMessage message = await e.Channel.GetMessageAsync(e.Message.Id);
						DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
						{
							Author = new DiscordEmbedBuilder.EmbedAuthor
							{
								IconUrl = message.Author.AvatarUrl,
								Name = message.Author.Username
							},
							Description = message.Content,
							Color = ((DiscordMember)message.Author).Color
						};
						discordEmbed.AddField("Message jump link", $"[Doing!]({e.Message.JumpLink})");

						if (e.Message.Attachments.Count != 0)
						{
							discordEmbed.ImageUrl = e.Message.Attachments[0].Url;
						}
						await DiscordObjectService.Instance.GoodGuysChannel.SendMessageAsync(e.Message.Id.ToString(), discordEmbed.Build()).ConfigureAwait(false);
					}
				}
			});
			return Task.CompletedTask;
		}

		internal static Task ReactionRemoved(DiscordClient _1, MessageReactionRemoveEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				if (!StartupService.Instance.StartUpCompleted)
				{
					return;
				}
				DiscordMember member = await e.Guild.GetMemberAsync(e.User.Id);
				if (e.Message.Id == 767100276028342322)
				{
					switch (e.Emoji.ToString())
					{
						case "⭐":
							await member.RevokeRoleAsync(e.Guild.GetRole(701454772900855819)); //Stellaris
							break;
						case "⛵":
							await member.RevokeRoleAsync(e.Guild.GetRole(701454853095817316)); //FTD
							break;
						case "⛏️":
							await member.RevokeRoleAsync(e.Guild.GetRole(713367380574732319)); //MC
							break;
						case "🌍":
							await member.RevokeRoleAsync(e.Guild.GetRole(701454812721446912)); //TT
							break;
						case "🔨":
							await member.RevokeRoleAsync(e.Guild.GetRole(766322672321560628)); //WH40K
							break;
						case "📜":
							await member.RevokeRoleAsync(e.Guild.GetRole(718162129609556121)); //Debate
							break;
						case "🎭":
							await member.RevokeRoleAsync(e.Guild.GetRole(725046250843668490)); //RP
							break;
						case "💮":
							await member.RevokeRoleAsync(e.Guild.GetRole(734215957446131733)); //Anime
							break;
						case "❓":
							await member.RevokeRoleAsync(e.Guild.GetRole(741342066021367938));//DQuestion
							break;
						case "💬":
							await member.RevokeRoleAsync(e.Guild.GetRole(765622563338453023)); //DQuote
							break;
						case "💰":
							await member.RevokeRoleAsync(e.Guild.GetRole(767035347367362590)); //Cas
							break;
						case "🎨":
							await member.RevokeRoleAsync(e.Guild.GetRole(767039219403063307)); //Art
							break;
						case "📢":
							await member.RevokeRoleAsync(e.Guild.GetRole(794975835235942470)); //Ads
							break;
						case "💢":
							await member.RevokeRoleAsync(e.Guild.GetRole(812755886413971499)); //Vent
							break;
						case "🕒":
							await member.RevokeRoleAsync(e.Guild.GetRole(821483418491289622)); //History
							break;
						case "❗":
							await member.RevokeRoleAsync(e.Guild.GetRole(848307821703200828)); //Facts
							break;
						case "👾":
							await member.RevokeRoleAsync(e.Guild.GetRole(850029252812210207)); //Reassembly
							break;
						default:
							break;
					}
				}
			});
			return Task.CompletedTask;
		}

		internal static Task VoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				if (!StartupService.Instance.StartUpCompleted)
				{
					return;
				}
				if (e.Before == null || e.Guild.GetMemberAsync(sender.CurrentUser.Id).Result.VoiceState == null)
					return;
				if (e.Before.Channel == e.Guild.GetMemberAsync(sender.CurrentUser.Id).Result.VoiceState.Channel)
				{
					foreach (DiscordMember member in e.Guild.GetMemberAsync(sender.CurrentUser.Id).Result.VoiceState.Channel.Users)
					{
						if (!member.IsBot)
						{
							return;
						}
					}
					LavalinkNodeConnection node = sender.GetLavalink().ConnectedNodes.Values.First();
					LavalinkGuildConnection conn = node.GetGuildConnection(e.Guild);
					if (LavalinkService.Instance.Queues != null && LavalinkService.Instance.Queues.ContainsKey(e.Guild))
						LavalinkService.Instance.Queues.Remove(e.Guild);
					await conn.StopAsync();
					await conn.DisconnectAsync();
					e.Handled = true;
				}
			});
			return Task.CompletedTask;
		}

		internal static Task ClientErrored(DiscordClient _1, ClientErrorEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				await File.AppendAllTextAsync("error.txt", DateTime.Now + ":\n" + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine).ConfigureAwait(false);
				await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace).ConfigureAwait(false);
			});
			return Task.CompletedTask;
		}

		internal static Task CommandErrored(CommandsNextExtension _1, CommandErrorEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				await File.AppendAllTextAsync("error.txt", DateTime.Now + ":\n" + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine).ConfigureAwait(false);
				await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace).ConfigureAwait(false);
			});
			return Task.CompletedTask;
		}

		internal static Task PlaybackFinished(LavalinkGuildConnection sender, TrackFinishEventArgs e)
		{
			_ = Task.Run(async () =>
			{
				if (LavalinkService.Instance.Repeats?[sender.Guild] == Repeaters.single)
				{
					await sender.PlayAsync(e.Track);
					DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
					{
						Title = "Now playing:",
						Description = $"[{e.Track.Title}]({e.Track.Uri})",
						Color = DiscordColor.Red
					};
					try
					{
						await sender.Guild.GetChannel(788102512455450654).SendMessageAsync(embedBuilder.Build());
					}
					catch (NullReferenceException)
					{
						await sender.Guild.GetChannel(512370308976607250).SendMessageAsync(embedBuilder.Build());
					}
					return;
				}
				else if (LavalinkService.Instance.Queues?[sender.Guild]?.Count != 0 && LavalinkService.Instance.Queues?[sender.Guild]?.Count != null)
				{
					await sender.PlayAsync(LavalinkService.Instance.Queues[sender.Guild].First());
					DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
					{
						Title = "Now playing:",
						Description = $"[{LavalinkService.Instance.Queues[sender.Guild].First().Title}]({LavalinkService.Instance.Queues[sender.Guild].First().Uri})",
						Color = DiscordColor.Red
					};

					LavalinkService.Instance.Queues[sender.Guild].RemoveAt(LavalinkService.Instance.Queues[sender.Guild].IndexOf(LavalinkService.Instance.Queues[sender.Guild].First()));
					if (LavalinkService.Instance.Repeats[sender.Guild] == Repeaters.all)
					{
						LavalinkService.Instance.Queues[sender.Guild].Add(e.Track);
					}

					try
					{
						await sender.Guild.GetChannel(788102512455450654).SendMessageAsync(embedBuilder.Build());
					}
					catch (NullReferenceException)
					{
						await sender.Guild.GetChannel(512370308976607250).SendMessageAsync(embedBuilder.Build());
					}
				}
				else if (LavalinkService.Instance.Repeats[sender.Guild] != Repeaters.off)
				{
					await sender.PlayAsync(e.Track);
					DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
					{
						Title = "Now playing:",
						Description = $"[{e.Track.Title}]({e.Track.Uri})",
						Color = DiscordColor.Red
					};
					try
					{
						await sender.Guild.GetChannel(788102512455450654).SendMessageAsync(embedBuilder.Build());
					}
					catch (NullReferenceException)
					{
						await sender.Guild.GetChannel(512370308976607250).SendMessageAsync(embedBuilder.Build());
					}
					return;
				}
				else
				{
					await sender.DisconnectAsync();
					LavalinkService.Instance.Queues.Remove(sender.Guild);
					try
					{
						await sender.Guild.GetChannel(788102512455450654).SendMessageAsync("Queue ended, leaving VC.");
					}
					catch (NullReferenceException)
					{
						await sender.Guild.GetChannel(512370308976607250).SendMessageAsync("Queue ended, leaving VC.");
					}
				}
			});
			return Task.CompletedTask;
		}

		internal async static void TimerTick(object sender, ElapsedEventArgs e)
		{
			await OnTimerMethods.PardonWarns();

			await OnTimerMethods.RemindMutes();

			await OnTimerMethods.DailyFacts();
		}

		internal static void OnLog(object sender, LoggingEventArgs e)
		{
			DiscordObjectService.Instance.ErrorLogChannel?.SendMessageAsync(e.Message);
		}
	}
}
