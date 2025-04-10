using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.EventHandlers
{
    public class Prevention
    {
        private static readonly Queue<(ulong, DateTime)> lastUsers = new();
        private static DateTime lastMessage = DateTime.MinValue;

        public static Task OnMessageCreated(DiscordClient _1, MessageCreatedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                // if channel is not venting disregard event
                if (e.Channel.Id != 812755782067290162) //venting
                    return;

                // after restart only
                if (lastUsers.Count == 0)
                {
                    // get last 10 messages 
                    var messages = e.Channel.GetMessagesAsync(10).ToBlockingEnumerable();
                    // sort messages by timestamp, if not already
                    foreach (var message in messages.OrderByDescending(x => x.Timestamp))
                        // add messages to queue
                        lastUsers.Enqueue((message.Author.Id, message.Timestamp.DateTime));
                }
                else
                {
                    // add new message to queue
                    lastUsers.Enqueue((e.Message.Author.Id, e.Message.Timestamp.DateTime));
                    // if queue is longer than 10 elements dequeue one
                    if (lastUsers.Count > 10)
                        lastUsers.Dequeue();
                }

                // if message was sent to smaug in last 30 mins disregard event
                if (lastMessage > DateTime.Now - TimeSpan.FromMinutes(30))
                    return;

                // filter out any messages that are older than 30 mins
                var toLookup = lastUsers.Where(x => x.Item2 > DateTime.Now - TimeSpan.FromMinutes(30));

                // if any of the last messages are from smaug, disregard event
                if (toLookup.Any(x => x.Item1 == 875851872815161406)) //smaug
                    return;

                // send dm to smaug
                var smaug = await e.Guild.GetMemberAsync(875851872815161406); //also smaug
                var channel = await smaug.CreateDmChannelAsync();
                await channel.SendMessageAsync($"Hey, your services might be needed in {e.Channel.Mention}");

                // update lastMessage timestamp
                lastMessage = DateTime.Now;
            });

            return Task.CompletedTask;
        }
    }
}
