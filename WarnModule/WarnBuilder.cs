﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WarnModule
{
    public class WarnBuilder(DiscordClient client, DiscordChannel warnChannel, DiscordGuild guild, DiscordMember mod, DiscordMember member, DiscordMessage messageLink = null)
    {
        public DiscordChannel WarnChannel { get; set; } = warnChannel;
        public DiscordGuild Guild { get; set; } = guild;
        public DiscordMember Mod { get; set; } = mod;
        public DiscordMember Member { get; set; } = member;
        public DiscordMessage MessageLink { get; set; } = messageLink;
        public InteractivityExtension Interactivity { get; set; } = client.GetInteractivity();
        public Rule Rule { get; set; }
        public int PointsDeducted { get; set; }
        public string Reason { get; set; }

        private int MemberDbId;
        private int PointsLeft;

        public async Task<bool> PreExecutionChecks()
        {
            if (Member.Id == 192037157416730625)
            {
                await WarnChannel.SendMessageAsync("You cant warn Lathrix!");
                return false;
            }
            if (await Guild.GetMemberAsync(Member.Id) == null)
            {
                await WarnChannel.SendMessageAsync($"User {Member.DisplayName} is not on this server anymore, you can't warn them!");
                return false;
            }
            if (Mod.Roles.Contains(Guild.GetRole(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new()
                {
                    Color = Mod.Color,
                    Title = $"Trial Plague {Mod.Nickname} just used a moderation command",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = Mod.AvatarUrl,
                        Text = $"{Mod.Username}#{Mod.Discriminator} ({Mod.Id})"
                    }
                };
                await Guild.GetChannel(722905404354592900).SendMessageAsync(discordEmbed.Build());
            }
            return true;
        }

        public async Task RequestRule(ContextMenuContext ctx = null)
        {
            List<DiscordSelectComponentOption> options = [];
            foreach (var item in RuleService.Rules)
            {
                if (item.RuleNum == 0)
                {
                    options.Add(new DiscordSelectComponentOption(
                        $"OTHER",
                        item.RuleNum.ToString()));
                    continue;
                }
                var option = new DiscordSelectComponentOption(
                    $"Rule {item.RuleNum}: {item.ShortDesc}",
                    item.RuleNum.ToString(),
                    item.RuleText.Length > 99 ? item.RuleText[..95] + "..." : item.RuleText);
                options.Add(option);
            }
            DiscordSelectComponent selectMenu = new("warnSelect", "Select a Rule!", options);

            DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder()
                .AddComponents(selectMenu)
                .WithContent("­");
            DiscordMessage message = await WarnChannel.SendMessageAsync(messageBuilder);

            if (ctx is not null)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder
                {
                    Content = message.JumpLink.ToString()
                });
            }

            var reaction = await Interactivity.WaitForSelectAsync(message, Mod, "warnSelect", TimeSpan.FromMinutes(2));
            Rule = RuleService.Rules.Single(x => x.RuleNum.ToString() == reaction.Result.Values.First());
            await message.DeleteAsync();
        }

        public async Task<ulong> RequestRuleEphemeral(ContextMenuContext ctx)
        {
            List<DiscordSelectComponentOption> options = [];
            foreach (var item in RuleService.Rules)
            {
                if (item.RuleNum == 0)
                {
                    options.Add(new DiscordSelectComponentOption(
                        $"OTHER",
                        item.RuleNum.ToString()));
                    continue;
                }
                var option = new DiscordSelectComponentOption(
                    $"Rule {item.RuleNum}: {item.ShortDesc}",
                    item.RuleNum.ToString(),
                    item.RuleText.Length > 99 ? item.RuleText[..95] + "..." : item.RuleText);
                options.Add(option);
            }
            DiscordSelectComponent selectMenu = new("warnSelect", "Select a Rule!", options);

            var message = await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder()
                .AddComponents(selectMenu)
                .WithContent("­")
                .AsEphemeral(true));

            var reaction = await Interactivity.WaitForSelectAsync(message, Mod, "warnSelect", TimeSpan.FromMinutes(2));
            Rule = RuleService.Rules.Single(x => x.RuleNum.ToString() == reaction.Result.Values.First());
            await reaction.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
            return message.Id;
        }

        public async Task RequestPoints()
        {
            DiscordMessageBuilder discordMessage = new()
            {
                Content = $"For this rule you can reduce the users chances by {Rule.MinPoints} - {Rule.MaxPoints}"
            };
            for (int i = 0; i < 3; i++)
            {
                List<DiscordButtonComponent> buttons = [];
                for (int index = i * 5; index < (i * 5) + 5; index++)
                {
                    buttons.Add(new DiscordButtonComponent
                    (
                        DiscordButtonStyle.Primary,
                        (index + 1).ToString(),
                        (index + 1).ToString(),
                        (index + 1) < Rule.MinPoints || (index + 1) > Rule.MaxPoints)
                    );
                }
                discordMessage.AddComponents(buttons);
            }
            DiscordMessage pointsMessage = await WarnChannel.SendMessageAsync(discordMessage);
            var interactpointsMessage = await Interactivity.WaitForButtonAsync(pointsMessage, Mod, TimeSpan.FromMinutes(2));
            PointsDeducted = int.Parse(interactpointsMessage.Result.Id);
            await pointsMessage.DeleteAsync();
        }

        public async Task<DiscordInteraction> RequestPointsEphemeral(ContextMenuContext ctx, ulong messageID)
        {
            DiscordWebhookBuilder webhook = new()
            {
                Content = $"For this rule you can reduce the users chances by {Rule.MinPoints} - {Rule.MaxPoints}"
            };
            for (int i = 0; i < 3; i++)
            {
                List<DiscordButtonComponent> buttons = [];
                for (int index = i * 5; index < (i * 5) + 5; index++)
                {
                    buttons.Add(new DiscordButtonComponent
                    (
                        DiscordButtonStyle.Primary,
                        (index + 1).ToString(),
                        (index + 1).ToString(),
                        (index + 1) < Rule.MinPoints || (index + 1) > Rule.MaxPoints)
                    );
                }
                webhook.AddComponents(buttons);
            }
            DiscordMessage pointsMessage = await ctx.EditFollowupAsync(messageID, webhook);
            var interactpointsMessage = await Interactivity.WaitForButtonAsync(pointsMessage, Mod, TimeSpan.FromMinutes(2));
            PointsDeducted = int.Parse(interactpointsMessage.Result.Id);

            foreach (var item in webhook.Components)
                foreach (DiscordButtonComponent button in item.Components.Cast<DiscordButtonComponent>())
                    button.Disable();

            await ctx.EditFollowupAsync(messageID, webhook);
            return interactpointsMessage.Result.Interaction;
        }

        public static int CalculateSeverity(int pointsDeducted)
        {
            return pointsDeducted switch
            {
                1 => 1,
                2 => 1,
                3 => 1,
                4 => 1,
                5 => 1,
                6 => 2,
                7 => 2,
                8 => 2,
                9 => 2,
                10 => 2,
                11 => 3,
                12 => 3,
                13 => 3,
                14 => 3,
                15 => 3,
                _ => 0
            };
        }

        public async Task RequestReason()
        {
            bool tryagain = true;
            Reason = "/";
            while (tryagain)
            {
                DiscordMessage reasonMessage = await WarnChannel.SendMessageAsync("If needed please state a reason, write ``NONE`` if you dont want to specify.");
                InteractivityResult<DiscordMessage> reasonResult = await Interactivity.WaitForMessageAsync(x => x.Channel == WarnChannel && x.Author == Mod);
                if (reasonResult.Result.Content.Trim().Equals("NONE", StringComparison.CurrentCultureIgnoreCase))
                {
                    await reasonMessage.DeleteAsync();
                    Reason = "/";
                    tryagain = false;
                }
                else if (reasonResult.Result.Content.Length >= 250)
                {
                    DiscordMessage buffoon = await WarnChannel.SendMessageAsync("Max reason length is 250 characters!");
                    await reasonMessage.DeleteAsync();
                    await Task.Delay(3000);
                    await buffoon.DeleteAsync();
                }
                else
                {
                    Reason = reasonResult.Result.Content;
                    await reasonMessage.DeleteAsync();
                    tryagain = false;
                }
            }
        }

        public async Task RequestReasonEphemeral(ContextMenuContext ctx, DiscordInteraction interaction)
        {
            var textInput = new DiscordTextInputComponent("If needed state a reason.",
                "reason",
                "NONE",
                required: true,
                style: DiscordTextInputStyle.Paragraph,
                max_length: 250);

            var responseBuilder = new DiscordInteractionResponseBuilder()
                .WithCustomId("reason")
                .WithTitle("Reason")
                .AddComponents(textInput);

            await interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, responseBuilder);

            var res = await ctx.Client.GetInteractivity().WaitForModalAsync("reason");

            await res.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
            Reason = res.Result.Values["reason"] ?? "/";
            Reason = Reason.Equals("NONE", StringComparison.CurrentCultureIgnoreCase) ? "/" : Reason;
        }

        public async Task<bool> WriteToDatabase()
        {
            UserRepository repo = new(ReadConfig.Config.ConnectionString);
            bool result = repo.GetIdByDcId(Member.Id, out MemberDbId);
            if (!result)
            {
                await WarnChannel.SendMessageAsync("There was an error getting the user from the Database");
                return false;
            }
            WarnRepository warnRepo = new(ReadConfig.Config.ConnectionString);
            result = warnRepo.GetWarnAmount(MemberDbId, out int WarnNumber);
            if (!result)
            {
                await WarnChannel.SendMessageAsync("There was an error getting the previous warns from the Database");
                return false;
            }
            result = repo.GetIdByDcId(Mod.Id, out int ModDbId);
            if (!result)
            {
                await WarnChannel.SendMessageAsync("There was an error getting the moderator from the database");
            }
            if (MemberDbId == 0 || ModDbId == 0)
                return false;
            Warn warn = new()
            {
                User = MemberDbId,
                Mod = ModDbId,
                Reason = Rule.RuleNum == 0 ? Reason : $"Rule {Rule.RuleNum}, {Reason}",
                Number = WarnNumber + 1,
                Level = PointsDeducted,
                Time = DateTime.Now,
                Persistent = false

            };
            result = warnRepo.Create(ref warn);
            if (!result)
            {
                await WarnChannel.SendMessageAsync("There was an error creating the database entry");
                return false;
            }
            return true;
        }

        public async Task<bool> WriteAuditToDb()
        {
            AuditRepository auditRepo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            bool userResult = urepo.GetIdByDcId(Mod.Id, out int id);
            if (!userResult)
            {
                await WarnChannel.SendMessageAsync("There was a problem reading a User");
                return false;
            }
            else
            {
                bool auditResult = auditRepo.Read(id, out Audit audit);
                if (!auditResult)
                {
                    await WarnChannel.SendMessageAsync("There was a problem reading an Audit");
                    return false;
                }
                else
                {
                    audit.Warns++;
                    bool updateResult = auditRepo.Update(audit);
                    if (!updateResult)
                    {
                        await WarnChannel.SendMessageAsync("There was a problem reading to the Audit table");
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task ReadRemainingPoints()
        {
            if (!new WarnRepository(ReadConfig.Config.ConnectionString).GetRemainingPoints(MemberDbId, out PointsLeft))
            {
                await WarnChannel.SendMessageAsync("There was an error reading the remaining points");
            }
        }

        public async Task SendWarnMessage()
        {
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = CalculateSeverity(PointsDeducted) switch
                {
                    1 => DiscordColor.Yellow,
                    2 => DiscordColor.Orange,
                    3 => DiscordColor.Red,
                    _ => DiscordColor.Black,
                },
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Height = 8, Width = 8, Url = Member.AvatarUrl },
                Title = $"{Member.DisplayName}#{Member.Discriminator} ({Member.Id}) has been warned for Rule {Rule.RuleNum}:",
                Description = $"{Rule.RuleText}\n" +
                        "\n" +
                        $"{Reason}",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = Mod.AvatarUrl, Text = $"{Mod.DisplayName}" }
            };
            embedBuilder.AddField($"{PointsLeft} points remaining", "Please keep any talk of this to DM's");
            DiscordEmbed embed = embedBuilder.Build();

            DiscordChannel warnsChannel = Guild.GetChannel(722186358906421369);
            await warnsChannel.SendMessageAsync(Member.Mention, embed);

            DiscordEmbedBuilder logEmbedBuilder = new()
            {
                Color = DiscordColor.Yellow,
                Title = $"Successfully warned {Member.DisplayName}#{Member.Discriminator} ({Member.Id}).",
                Description = $"Rule {Rule.RuleNum}:\n" +
                            "\n" +
                            $"{Reason}\n" +
                            "\n" +
                            $"User has {PointsLeft} points left.",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = Mod.AvatarUrl, Text = $"{Mod.DisplayName}" }
            };
            DiscordEmbed logEmbed = logEmbedBuilder.Build();
            await WarnChannel.SendMessageAsync(logEmbed);
        }

        public async Task LogMessage()
        {
            DiscordEmbedBuilder discordEmbed = new()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = MessageLink.Author.AvatarUrl,
                    Name = MessageLink.Author.Username
                },
                Description = MessageLink.Content,
                Color = (await Guild.GetMemberAsync(MessageLink.Author.Id)).Color
            };
            if (MessageLink.Attachments.Count != 0)
            {
                var msgBuilder = new DiscordMessageBuilder();

                Dictionary<string, Stream> attachments = [];
                if (MessageLink.Attachments is not null && MessageLink.Attachments.Any())
                {
                    foreach (var attachment in MessageLink.Attachments)
                    {
                        using HttpClient httpClient = new();

                        attachments.Add(attachment.FileName, await httpClient.GetStreamAsync(attachment.Url));
                        if (attachment.MediaType.Contains("image") && string.IsNullOrEmpty(discordEmbed.ImageUrl))
                            discordEmbed.WithImageUrl("attachment://" + attachment.FileName);
                    }
                    msgBuilder.AddFiles(attachments);
                }

                await WarnChannel.SendMessageAsync(msgBuilder.AddEmbed(discordEmbed).WithAllowedMentions(Mentions.None));
                foreach (var attachment in attachments)
                    attachment.Value.Close();
            }
            else
            {
                await WarnChannel.SendMessageAsync(discordEmbed);
            }
            await MessageLink.DeleteAsync();
        }

        public async Task SendPunishMessage()
        {
            if (PointsLeft < 11)
            {
                await WarnChannel.SendMessageAsync($"{Mod.Mention} User has {PointsLeft} points left.\n" +
                    $"By common practice the user should be muted{(PointsLeft < 6 ? ", kicked" : "")}{(PointsLeft < 1 ? ", or banned" : "")}.");
            }
        }

        public static Task ResetLastPunish(ulong userID)
        {
            UserRepository repo = new(ReadConfig.Config.ConnectionString);

            repo.GetIdByDcId(userID, out var dbId);

            repo.UpdateLastPunish(dbId, DateTime.Now);

            return Task.CompletedTask;
        }
    }
}
