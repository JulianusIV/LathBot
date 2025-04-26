using DSharpPlus;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
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
        public Rule Rule { get; set; }
        public int PointsDeducted { get; set; }
        public string Reason { get; set; }

        private int MemberDbId;
        private int PointsLeft;

        public async Task<bool> PreExecutionChecks()
        {
            if (this.Member.Id == DiscordObjectService.Instance.Lathrix)
            {
                await this.WarnChannel.SendMessageAsync("You cant warn Lathrix!");
                return false;
            }
            if (await this.Guild.GetMemberAsync(this.Member.Id) == null)
            {
                await this.WarnChannel.SendMessageAsync($"User {this.Member.DisplayName} is not on this server anymore, you can't warn them!");
                return false;
            }
            if (this.Mod.Roles.Contains(await this.Guild.GetRoleAsync(748646909354311751)))
            {
                DiscordEmbedBuilder discordEmbed = new()
                {
                    Color = this.Mod.Color,
                    Title = $"Trial Plague {this.Mod.Nickname} just used a moderation command",
                    Footer = new DiscordEmbedBuilder.EmbedFooter
                    {
                        IconUrl = this.Mod.AvatarUrl,
                        Text = $"{this.Mod.Username}#{this.Mod.Discriminator} ({this.Mod.Id})"
                    }
                };
                await (await this.Guild.GetChannelAsync(722905404354592900)).SendMessageAsync(discordEmbed.Build());
            }
            return true;
        }

        public async Task RequestRule(SlashCommandContext ctx = null)
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
                .AddActionRowComponent(selectMenu)
                .WithContent("­");
            DiscordMessage message = await this.WarnChannel.SendMessageAsync(messageBuilder);

            if (ctx is not null)
                await ctx.FollowupAsync(message.JumpLink.ToString(), true);

            var reaction = await message.WaitForSelectAsync(this.Mod, "warnSelect", TimeSpan.FromMinutes(2));
            this.Rule = RuleService.Rules.Single(x => x.RuleNum.ToString() == reaction.Result.Values.First());
            await message.DeleteAsync();
        }

        public async Task<ulong> RequestRuleEphemeral(SlashCommandContext ctx)
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

            var message = await ctx.FollowupAsync(new DiscordFollowupMessageBuilder()
                .AddActionRowComponent(selectMenu)
                .WithContent("­")
                .AsEphemeral());

            var reaction = await message.WaitForSelectAsync(this.Mod, "warnSelect", TimeSpan.FromMinutes(2));
            this.Rule = RuleService.Rules.Single(x => x.RuleNum.ToString() == reaction.Result.Values.First());
            await reaction.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
            return message.Id;
        }

        public async Task RequestPoints()
        {
            DiscordMessageBuilder discordMessage = new()
            {
                Content = $"For this rule you can reduce the users chances by {this.Rule.MinPoints} - {this.Rule.MaxPoints}"
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
                        (index + 1) < this.Rule.MinPoints || (index + 1) > this.Rule.MaxPoints)
                    );
                }
                discordMessage.AddActionRowComponent(buttons);
            }
            DiscordMessage pointsMessage = await this.WarnChannel.SendMessageAsync(discordMessage);
            var interactpointsMessage = await pointsMessage.WaitForButtonAsync(this.Mod, TimeSpan.FromMinutes(2));
            this.PointsDeducted = int.Parse(interactpointsMessage.Result.Id);
            await pointsMessage.DeleteAsync();
        }

        public async Task<DiscordInteraction> RequestPointsEphemeral(SlashCommandContext ctx, ulong messageID)
        {
            DiscordWebhookBuilder webhook = new()
            {
                Content = $"For this rule you can reduce the users chances by {this.Rule.MinPoints} - {this.Rule.MaxPoints}"
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
                        (index + 1) < this.Rule.MinPoints || (index + 1) > this.Rule.MaxPoints)
                    );
                }
                webhook.AddActionRowComponent(buttons);
            }
            DiscordMessage pointsMessage = await ctx.EditFollowupAsync(messageID, webhook);
            var interactpointsMessage = await pointsMessage.WaitForButtonAsync(this.Mod, TimeSpan.FromMinutes(2));
            this.PointsDeducted = int.Parse(interactpointsMessage.Result.Id);

            foreach (var item in webhook.Components)
                foreach (DiscordButtonComponent button in ((DiscordActionRowComponent)item).Components.Cast<DiscordButtonComponent>())
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
            this.Reason = "/";
            while (tryagain)
            {
                DiscordMessage reasonMessage = await this.WarnChannel.SendMessageAsync("If needed please state a reason, write ``NONE`` if you dont want to specify.");
                InteractivityResult<DiscordMessage> reasonResult = await this.WarnChannel.GetNextMessageAsync(this.Mod);
                if (reasonResult.Result.Content.Trim().Equals("NONE", StringComparison.CurrentCultureIgnoreCase))
                {
                    await reasonMessage.DeleteAsync();
                    this.Reason = "/";
                    tryagain = false;
                }
                else if (reasonResult.Result.Content.Length >= 250)
                {
                    DiscordMessage buffoon = await this.WarnChannel.SendMessageAsync("Max reason length is 250 characters!");
                    await reasonMessage.DeleteAsync();
                    await Task.Delay(3000);
                    await buffoon.DeleteAsync();
                }
                else
                {
                    this.Reason = reasonResult.Result.Content;
                    await reasonMessage.DeleteAsync();
                    tryagain = false;
                }
            }
        }

        public async Task RequestReasonEphemeral(SlashCommandContext ctx, DiscordInteraction interaction)
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
                .AddTextInputComponent(textInput);

            await interaction.CreateResponseAsync(DiscordInteractionResponseType.Modal, responseBuilder);
            InteractivityExtension interactivity = (InteractivityExtension)client.ServiceProvider.GetService(typeof(InteractivityExtension));
            var res = await interactivity.WaitForModalAsync("reason");

            await res.Result.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredMessageUpdate);
            this.Reason = res.Result.Values["reason"] ?? "/";
            this.Reason = this.Reason.Equals("NONE", StringComparison.CurrentCultureIgnoreCase) ? "/" : this.Reason;
        }

        public async Task<bool> WriteToDatabase()
        {
            UserRepository repo = new(ReadConfig.Config.ConnectionString);
            bool result = repo.GetIdByDcId(this.Member.Id, out this.MemberDbId);
            if (!result)
            {
                await this.WarnChannel.SendMessageAsync("There was an error getting the user from the Database");
                return false;
            }
            WarnRepository warnRepo = new(ReadConfig.Config.ConnectionString);
            result = warnRepo.GetWarnAmount(this.MemberDbId, out int WarnNumber);
            if (!result)
            {
                await this.WarnChannel.SendMessageAsync("There was an error getting the previous warns from the Database");
                return false;
            }
            result = repo.GetIdByDcId(this.Mod.Id, out int ModDbId);
            if (!result)
            {
                await this.WarnChannel.SendMessageAsync("There was an error getting the moderator from the database");
            }
            if (this.MemberDbId == 0 || ModDbId == 0)
                return false;
            Warn warn = new()
            {
                User = this.MemberDbId,
                Mod = ModDbId,
                Reason = this.Rule.RuleNum == 0 ? this.Reason : $"Rule {this.Rule.RuleNum}, {this.Reason}",
                Number = WarnNumber + 1,
                Level = this.PointsDeducted,
                Time = DateTime.Now,
                Persistent = false

            };
            result = warnRepo.Create(ref warn);
            if (!result)
            {
                await this.WarnChannel.SendMessageAsync("There was an error creating the database entry");
                return false;
            }
            return true;
        }

        public async Task<bool> WriteAuditToDb()
        {
            AuditRepository auditRepo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            bool userResult = urepo.GetIdByDcId(this.Mod.Id, out int id);
            if (!userResult)
            {
                await this.WarnChannel.SendMessageAsync("There was a problem reading a User");
                return false;
            }
            else
            {
                bool auditResult = auditRepo.Read(id, out Audit audit);
                if (!auditResult)
                {
                    await this.WarnChannel.SendMessageAsync("There was a problem reading an Audit");
                    return false;
                }
                else
                {
                    audit.Warns++;
                    bool updateResult = auditRepo.Update(audit);
                    if (!updateResult)
                    {
                        await this.WarnChannel.SendMessageAsync("There was a problem reading to the Audit table");
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task ReadRemainingPoints()
        {
            if (!new WarnRepository(ReadConfig.Config.ConnectionString).GetRemainingPoints(this.MemberDbId, out this.PointsLeft))
            {
                await this.WarnChannel.SendMessageAsync("There was an error reading the remaining points");
            }
        }

        public async Task SendWarnMessage()
        {
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = CalculateSeverity(this.PointsDeducted) switch
                {
                    1 => DiscordColor.Yellow,
                    2 => DiscordColor.Orange,
                    3 => DiscordColor.Red,
                    _ => DiscordColor.Black,
                },
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Height = 8, Width = 8, Url = this.Member.AvatarUrl },
                Title = $"{this.Member.DisplayName}#{this.Member.Discriminator} ({this.Member.Id}) has been warned for Rule {this.Rule.RuleNum}:",
                Description = $"{this.Rule.RuleText}\n" +
                        "\n" +
                        $"{this.Reason}",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = this.Mod.AvatarUrl, Text = $"{this.Mod.DisplayName}" }
            };
            embedBuilder.AddField($"{this.PointsLeft} points remaining", "Please keep any talk of this to DM's");
            DiscordEmbed embed = embedBuilder.Build();

            DiscordChannel warnsChannel = await this.Guild.GetChannelAsync(DiscordObjectService.Instance.WarnsChannel.Id);
            await warnsChannel.SendMessageAsync(this.Member.Mention, embed);

            DiscordEmbedBuilder logEmbedBuilder = new()
            {
                Color = DiscordColor.Yellow,
                Title = $"Successfully warned {this.Member.DisplayName}#{this.Member.Discriminator} ({this.Member.Id}).",
                Description = $"Rule {this.Rule.RuleNum}:\n" +
                            "\n" +
                            $"{this.Reason}\n" +
                            "\n" +
                            $"User has {this.PointsLeft} points left.",
                Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = this.Mod.AvatarUrl, Text = $"{this.Mod.DisplayName}" }
            };
            DiscordEmbed logEmbed = logEmbedBuilder.Build();
            await this.WarnChannel.SendMessageAsync(logEmbed);
        }

        public async Task LogMessage()
        {
            DiscordEmbedBuilder discordEmbed = new()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor
                {
                    IconUrl = this.MessageLink.Author.AvatarUrl,
                    Name = this.MessageLink.Author.Username
                },
                Description = this.MessageLink.Content,
                Color = (await this.Guild.GetMemberAsync(this.MessageLink.Author.Id)).Color
            };
            if (this.MessageLink.Attachments.Count != 0)
            {
                var msgBuilder = new DiscordMessageBuilder();

                Dictionary<string, Stream> attachments = [];
                if (this.MessageLink.Attachments is not null && this.MessageLink.Attachments.Any())
                {
                    foreach (var attachment in this.MessageLink.Attachments)
                    {
                        using HttpClient httpClient = new();

                        attachments.Add(attachment.FileName, await httpClient.GetStreamAsync(attachment.Url));
                        if (attachment.MediaType.Contains("image") && string.IsNullOrEmpty(discordEmbed.ImageUrl))
                            discordEmbed.WithImageUrl("attachment://" + attachment.FileName);
                    }
                    msgBuilder.AddFiles(attachments);
                }

                await this.WarnChannel.SendMessageAsync(msgBuilder.AddEmbed(discordEmbed).WithAllowedMentions(Mentions.None));
                foreach (var attachment in attachments)
                    attachment.Value.Close();
            }
            else
            {
                await this.WarnChannel.SendMessageAsync(discordEmbed);
            }
            await this.MessageLink.DeleteAsync();
        }

        public async Task SendPunishMessage()
        {
            if (this.PointsLeft < 11)
            {
                await this.WarnChannel.SendMessageAsync($"{this.Mod.Mention} User has {this.PointsLeft} points left.\n" +
                    $"By common practice the user should be muted{(this.PointsLeft < 6 ? ", kicked" : "")}{(this.PointsLeft < 1 ? ", or banned" : "")}.");
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
