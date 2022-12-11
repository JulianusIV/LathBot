using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using LathBotBack.Base;
using LathBotBack.Config;
using LathBotBack.Enums;
using LathBotBack.Logging;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using LathBotFront.Interactions.PreExecutionChecks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

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

        internal static Task SlashCommandErrored(SlashCommandsExtension _1, SlashCommandErrorEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Exception is SlashExecutionChecksFailedException checkException)
                    if (checkException.FailedChecks.Any(x => x.GetType() == typeof(EmbedBannedAttribute)))
                        await e.Context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .AsEphemeral()
                            .WithContent("You are banned from using this command"));

                await File.AppendAllTextAsync("error.txt", DateTime.Now + ":\n" + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine);
                await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
            });
            return Task.CompletedTask;
        }

        internal static Task AutoCompleteErrored(SlashCommandsExtension _1, AutocompleteErrorEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await File.AppendAllTextAsync("error.txt", DateTime.Now + ":\n" + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine);
                await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
            });
            return Task.CompletedTask;
        }

        internal static Task MessageCreated(DiscordClient _1, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Channel.Id == 713284112638672917 || e.Channel.Id == 720543453376937996)
                {
                    await e.Channel.CrosspostMessageAsync(e.Message);
                    return;
                }
                if (e.Author.Id == 708083256439996497)
                    return;
                if (e.Channel.IsPrivate)
                    return;
                if (!StartupService.Instance.StartUpCompleted)
                    return;
                if (e.Message.Attachments.Any(x => x.FileName.ToLower().EndsWith(".webm")))
                {
                    await e.Message.DeleteAsync();
                    await e.Channel.SendMessageAsync("webm files are currently instantly deleted due to the risk of a trojan, sorry.");
                    return;
                }
                if (e.Channel.Id == 765619584794361886) //ideas for daily
                {
                    _ = e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👍"));
                    _ = e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👎"));
                }
                if (e.Channel.Id == 741340775530496013 && e.MentionedRoles.Contains(e.Guild.GetRole(741342066021367938)))
                {
                    try
                    {
                        var oldThread = e.Channel.Threads.Where(x => !x.ThreadMetadata.IsArchived).FirstOrDefault();
                        if (!(oldThread is null))
                            await oldThread.ModifyAsync(x =>
                            {
                                x.AutoArchiveDuration = AutoArchiveDuration.Hour;
                                x.Locked = true;
                            });

                        var thread = await e.Message.CreateThreadAsync("text-answers", AutoArchiveDuration.Week);
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👍"));
                        await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("👎"));
                    }
                    catch (Exception e)
                    {
                        SystemService.Instance.Logger.Log(e.Message + Environment.NewLine + e.StackTrace);
                    }
                }
                if (((e.Channel.Id == 838088490704568341) || 
                    (e.Channel.Id == 718162681554534511 && !e.Channel.PermissionOverwrites.Any(x => x.Id == e.Author.Id))) &&
                    !(await e.Guild.GetMemberAsync(e.Author.Id)).Permissions.HasPermission(Permissions.KickMembers))
                {
                    string pattern = @"((http:\/\/|https:\/\/)?(www.)?(([a-zA-Z0-9-]){2,}\.){1,16}([a-zA-Z]){2,24}(\/([a-zA-Z-_\/\.0-9#:?=&;,]*)?)?)";
                    Regex rg = new Regex(pattern);
                    if (rg.Matches(e.Message.Content).Any())
                    {
                        await e.Message.DeleteAsync($"Autoremove link in #{e.Channel.Name}");
                        await e.Channel.SendMessageAsync("No links allowed in here." + (e.Channel.Id == 718162681554534511 ? "\nUse /embed to get permissions." : ""));
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
                if (e.Channel.Id == 873215194988437574)
                {

                }
            });
            return Task.CompletedTask;
        }

        internal static Task ContextMenuErrored(SlashCommandsExtension _1, ContextMenuErrorEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await File.AppendAllTextAsync("error.txt", DateTime.Now + ":\n" + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine);
                await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
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
                if (DiscordObjectService.Instance.LastDeletes.ContainsKey(e.Channel.Id) && !e.Message.Author.IsBot)
                {
                    DiscordObjectService.Instance.LastDeletes[e.Channel.Id] = e.Message;
                }
                else if (!e.Message.Author.IsBot)
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
                else if (e.Emoji.Name == "TheGoodGuys")
                {
                    if (e.Channel.Id == 795654190143766578 || //Goodguyschannel
                    e.Channel.Id == 713284112638672917 || //YT
                    e.Channel.Id == 720543453376937996) //Twitter
                        return;
                    if (e.Message.Timestamp < DateTime.Now - TimeSpan.FromHours(4))
                        return;
                    if (!GoodGuysService.Instance.GoodGuysStatus)
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


                        var messageBuilder = new DiscordMessageBuilder();

                        Dictionary<string, Stream> attachments = new Dictionary<string, Stream>();
                        if (!(e.Message.Attachments is null) && e.Message.Attachments.Any())
                        {
                            foreach (var attachment in e.Message.Attachments)
                            {
                                attachments.Add(attachment.FileName, WebRequest.Create(attachment.Url).GetResponse().GetResponseStream());
                                if (attachment.MediaType.Contains("image") && string.IsNullOrEmpty(discordEmbed.ImageUrl))
                                    discordEmbed.WithImageUrl("attachment://" + attachment.FileName);
                            }
                            messageBuilder.WithFiles(attachments);
                        }

                        await DiscordObjectService.Instance.GoodGuysChannel.SendMessageAsync(messageBuilder.WithContent(e.Message.Id.ToString()).WithEmbed(discordEmbed).WithAllowedMentions(Mentions.None));
                        foreach (var attachment in attachments)
                            attachment.Value.Close();
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
                await File.AppendAllTextAsync("error.txt", DateTime.Now + ":\n" + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine);
                await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
            });
            return Task.CompletedTask;
        }

        internal static Task CommandErrored(CommandsNextExtension _1, CommandErrorEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await File.AppendAllTextAsync("error.txt", DateTime.Now + ":\n" + e.Exception.Message + Environment.NewLine + e.Exception.StackTrace + Environment.NewLine);
                await DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
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

        internal static async void TimerTick(object sender, ElapsedEventArgs e)
        {
            try
            {
                await OnTimerMethods.PardonWarns();

                await OnTimerMethods.RemindMutes();

                await OnTimerMethods.APOD();
            }
            catch (Exception ex)
            {
                SystemService.Instance.Logger.Log(ex.Message);
            }
        }

        internal static async void TakeYourMeds(object sender, ElapsedEventArgs e)
        {
            try
            {
                var timer = (Timer)sender;
                timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
                if (!timer.Enabled)
                    timer.Start();
                var insertMember = await DiscordObjectService.Instance.Lathland.GetMemberAsync(993302594275524738);
                await insertMember.SendMessageAsync("Yo time to drug up! ( also good mornin :D )");
            }
            catch (Exception ex)
            {
                SystemService.Instance.Logger.Log("Error in drug reminder:\n" + ex.Message);
            }
        }

        internal static void OnLog(object sender, LoggingEventArgs e)
            => DiscordObjectService.Instance.ErrorLogChannel?.SendMessageAsync(e.Message);
    }
}
