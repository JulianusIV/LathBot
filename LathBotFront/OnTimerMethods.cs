﻿using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LathBotFront
{
    public class OnTimerMethods
    {
        public static async Task PardonWarns()
        {
            WarnRepository repo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            bool result = repo.GetAll(out List<Warn> list);
            if (!result)
            {
                _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error getting all warns in TimerTick.");
                return;
            }
            int pardoned = 0;
            foreach (var item in list)
            {
                result = urepo.Read(item.User, out User entity);
                DateTime timeToUse = entity.LastPunish is null ? item.Time : (DateTime)entity.LastPunish;

                var expirationTimeExpression = (item.ExpirationTime is not null && timeToUse <= DateTime.Now - TimeSpan.FromDays((double)item.ExpirationTime));
                var sevOneExpression = item.Level > 0 && item.Level < 6 && timeToUse <= DateTime.Now - TimeSpan.FromDays(14);
                var sevTwoExpression = item.Level > 5 && item.Level < 11 && timeToUse <= DateTime.Now - TimeSpan.FromDays(56);
                var fullExpression = ((item.ExpirationTime is not null && expirationTimeExpression) ||
                    item.ExpirationTime is null && (sevOneExpression || sevTwoExpression)) &&
                    !item.Persistent;
                if (fullExpression)
                {
                    pardoned++;
                    result = repo.Delete(item.ID);
                    if (!result)
                    {
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync($"Error deleting warn {item.ID} from {item.Time}, level {item.Level}.");
                    }
                    result = repo.GetAllByUser(item.User, out List<Warn> others);
                    if (!result)
                    {
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error reading other warns from the database.");
                    }

                    int counter = 0;
                    foreach (var warn in others)
                    {
                        counter++;
                        warn.Number = counter;
                        result = repo.Update(warn);
                        if (!result)
                        {
                            _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error updating the database.");
                            break;
                        }
                    }


                    result = urepo.Read(item.Mod, out User modEntity);
                    if (!result)
                    {
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error reading a user from the database");
                        continue;
                    }
                    try
                    {
                        DiscordMember member = await DiscordObjectService.Instance.Lathland.GetMemberAsync(entity.DcID);
                        DiscordMember moderator = await DiscordObjectService.Instance.Lathland.GetMemberAsync(modEntity.DcID);
                        DiscordEmbedBuilder embedBuilder = new()
                        {
                            Color = DiscordColor.Green,
                            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
                            Title = $"Pardoned warn number {item.Number} of {member.DisplayName}#{member.Discriminator} ({member.Id})",
                            Description = $"Warn from {item.Time}.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = moderator.AvatarUrl, Text = moderator.DisplayName }
                        };
                        DiscordEmbed embed = embedBuilder.Build();

                        _ = DiscordObjectService.Instance.WarnsChannel.SendMessageAsync(embed);
                    }
                    catch
                    {
                        DiscordEmbedBuilder embedBuilder = new()
                        {
                            Color = DiscordColor.Green,
                            Title = $"Pardoned warn of {entity.DcID}",
                            Description = $"Warn from {item.Time}.",
                            Footer = new DiscordEmbedBuilder.EmbedFooter { Text = modEntity.DcID.ToString() }
                        };
                        DiscordEmbed embed = embedBuilder.Build();

                        _ = DiscordObjectService.Instance.WarnsChannel.SendMessageAsync(embed);
                    }
                }
            }
            if (pardoned > 0)
            {
                _ = DiscordObjectService.Instance.TimerChannel.SendMessageAsync($"Timer ticked, {pardoned} warns pardoned.");
            }
            else
            {
                _ = DiscordObjectService.Instance.TimerChannel.SendMessageAsync("Timer ticked, no warns pardoned.");
            }
        }

        public static async Task RemindMutes()
        {
            MuteRepository repo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            bool result = repo.GetAll(out List<Mute> list);
            if (!result)
            {
                _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error getting all mutes in TimerTick.");
                return;
            }
            foreach (var item in list)
            {
                if (item.Timestamp + TimeSpan.FromDays(item.Duration) <= DateTime.Now)
                {
                    result = urepo.Read(item.Mod, out User dbMod);
                    if (!result)
                    {
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error getting a mod from the database.");
                        continue;
                    }
                    DiscordMember mod = null;
                    try
                    {
                        mod = await DiscordObjectService.Instance.Lathland.GetMemberAsync(dbMod.DcID);
                    }
                    catch (NotFoundException)
                    {
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error getting a mod from discord.");
                    }
                    result = urepo.Read(item.User, out User dbUser);
                    if (!result)
                    {
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error getting a user from the database.");
                        continue;
                    }
                    DiscordMember user = null;
                    try
                    {
                        user = await DiscordObjectService.Instance.Lathland.GetMemberAsync(dbUser.DcID);
                    }
                    catch (NotFoundException)
                    {
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error getting a user from discord.");
                    }
                    if (user is null)
                    {
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error in mute handling, user was null");
                    }
                    else if (user.Roles.Contains(DiscordObjectService.Instance.Lathland.GetRole(701446136208293969)))
                    {
                        if (!((await mod.CreateDmChannelAsync()).GetMessagesAsync(5).ToBlockingEnumerable()).Any(x => x.Content.Contains("You will be reminded again tomorrow.") && x.CreationTimestamp > DateTime.Now - TimeSpan.FromHours(24)))
                        {
                            await mod.SendMessageAsync($"The user {user.DisplayName}#{user.Discriminator} ({user.Id}) you muted at {item.Timestamp:yyyy-MM-dd hh:mm} for {item.Duration} days, is now muted for {(DateTime.Now - item.Timestamp):dd} days.\n" +
                                $"You will be reminded again tomorrow.");
                            if ((item.Duration < 8 && (item.Timestamp + TimeSpan.FromDays(item.Duration + 2)) < DateTime.Now) ||
                                (item.Duration > 7 && (item.Timestamp + TimeSpan.FromDays(item.Duration + 1)) < DateTime.Now) ||
                                (item.Duration == 14 && (item.Timestamp + TimeSpan.FromDays(item.Duration)) < DateTime.Now))
                            {
                                await DiscordObjectService.Instance.Lathland.GetChannel(722905404354592900).SendMessageAsync($"The user {user.DisplayName}#{user.Discriminator} ({user.Id}), muted by {mod.DisplayName}#{mod.Discriminator} ({mod.Id}) at {item.Timestamp:yyyy-MM-dd hh:mm} for {item.Duration} days, is now muted for {(DateTime.Now - item.Timestamp):dd} days.");
                            }
                        }
                    }
                    else if (!user.Roles.Contains(DiscordObjectService.Instance.Lathland.GetRole(701446136208293969)))
                    {
                        result = repo.Delete(item.Id);
                        if (!result)
                        {
                            _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error deleting a Mute from database.");
                            continue;
                        }
                        _ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Successfully deleted a mute from the database due to missing muted role.");
                    }
                }
            }
        }

        public static async Task APOD()
        {
            var lastmessageList = DiscordObjectService.Instance.APODChannel.GetMessagesAsync(1).ToBlockingEnumerable();
            DiscordMessage lastmessage = lastmessageList.ElementAt(0);
            if ((DateTime.Now - lastmessage.Timestamp) > TimeSpan.FromHours(24))
            {
                using HttpClient httpClient = new();
                string content = await httpClient.GetStringAsync("https://api.nasa.gov/planetary/apod?api_key=" + ReadConfig.Config.NasaApiKey + "&thumbs=True");

                APODJsonObject json = JsonConvert.DeserializeObject<APODJsonObject>(content);

                DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
                {
                    Title = "Astronomy Picture of the day:\n" + json.Title,
                    Description = "**Explanation:\n**" + json.Explanation,
                    ImageUrl = json.HdUrl is null ? json.ThumbnailUrl : json.HdUrl,
                    Color = new DiscordColor("e49a5e"),
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        Text = "Copyright: " + (json.Copyright is null ? "Public Domain" : json.Copyright) + "\nSource: NASA APOD API Endpoint"
                    }
                }.AddField("Links:", json.HdUrl is null ? $"[Source Link]({json.URL})" : $"[Source Link]({json.HdUrl})\n[Low resolution source]({json.URL})");

                DiscordMessageBuilder builder = new DiscordMessageBuilder()
                    .AddEmbed(embedBuilder)
                    .WithContent(DiscordObjectService.Instance.Lathland.GetRole(848307821703200828).Mention);
                DiscordMessageBuilder builder2 = null;
                if (json.MediaType != "image")
                    builder2 = new DiscordMessageBuilder().WithContent(json.URL.Replace("embed/", "watch?v=").Replace("?rel=0", ""));
                builder.WithAllowedMentions(Mentions.All);
                await DiscordObjectService.Instance.APODChannel.SendMessageAsync(builder);
                if (builder2 is not null)
                    await DiscordObjectService.Instance.APODChannel.SendMessageAsync(builder2);
            }
        }
    }

    struct APODJsonObject
    {
        [JsonProperty("copyright")]
        public string Copyright { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("explanation")]
        public string Explanation { get; set; }
        [JsonProperty("hdurl")]
        public string HdUrl { get; set; }
        [JsonProperty("media_type")]
        public string MediaType { get; set; }
        [JsonProperty("service_version")]
        public string ServiceVersion { get; set; }
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("url")]
        public string URL { get; set; }
    }
}
