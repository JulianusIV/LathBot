using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using LathBotBack.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LathBotFront.Commands
{
    public class EmbedCommands : BaseCommandModule
    {
        [Command("roleme")]
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        [Description("Creates the embed for #role-assign")]
        public async Task RolemeEmbed(CommandContext ctx)
        {
            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(192037157416730625);
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Color = lathrix.Color,
                Title = "Role Assign",
                Description = "Get your roles here.\n" +
                    "Each role unlocks new Channels (once you are verified) for you to see and/or send messages in.\n" +
                    "this will still be available after your successfully verified yourself in <#767049785223020556>"
            };

            var messageBuilder = new DiscordMessageBuilder()
            {
                Embed = embedBuilder.Build()
            };

            messageBuilder.AddComponents(new List<DiscordComponent>()
            {
                new DiscordButtonComponent(ButtonStyle.Primary, "roleme_games", "Games", emoji: new DiscordComponentEmoji("🎮")),
                new DiscordButtonComponent(ButtonStyle.Primary, "roleme_misc", "Misc", emoji: new DiscordComponentEmoji("🏷"))
            });
            await ctx.Channel.SendMessageAsync(messageBuilder);
        }

        /// <summary>
        /// creates the embed for #information
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("infoembed")]
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        [Description("Creates the embed for #information")]
        public async Task InfoEmbed(CommandContext ctx)
        {
            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(192037157416730625);
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "Information",
                Description = "Greetings Sir's and Siret's, and welcome to the Lathland discord server!" +
                " You can talk to fellow Lathrixians, share art and post videos. " +
                "There is even a section where Lathrix will answer your questions! " +
                "Make sure to head over to role-assign and rules to ensure you have a nice experience.",
                ImageUrl = "https://pbs.twimg.com/media/EW-x7e1XkAAQqAw?format=jpg&name=small",
            };

            var messageBuilder = new DiscordMessageBuilder()
            {
                Embed = embedBuilder,
            };

            foreach (var button in new DiscordComponent[]
            {
                new DiscordLinkButtonComponent("https://www.youtube.com/user/Lathland", "YouTube", emoji: new DiscordComponentEmoji(978916644426494002)),
                new DiscordLinkButtonComponent("https://twitter.com/Lathrix", "Twitter", emoji: new DiscordComponentEmoji(978917283965583410)),
                //new DiscordLinkButtonComponent("https://www.twitch.tv/lathrix", "Twitch", emoji: new DiscordComponentEmoji(978917615823101952)),
                new DiscordLinkButtonComponent("https://docs.google.com/document/d/1Pq-7WVHn6uwcGWKH8fqznxRiJrRC4MeV7JmLUe-E2yA/edit#", "Role information document"),
                new DiscordLinkButtonComponent("https://docs.google.com/document/d/1FAJzwct6lgyuoxuVu_V8iSvzOVlP6_sqUuJYKZOhu8M/edit#", "Warn information document")
            })
            {
                messageBuilder.AddComponents(button);
            }

            await ctx.Channel.SendMessageAsync(messageBuilder);
        }

        /// <summary>
        /// updates the embed for #information
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("updateinfo")]
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        [Description("Updates the embed for #rules")]
        public async Task UpdateInfoEmbed(CommandContext ctx)
        {
            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(192037157416730625);
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "Information",
                Description = "Greetings Sir's and Siret's, and welcome to the Lathland discord server!" +
                " You can talk to fellow Lathrixians, share art and post videos. " +
                "There is even a section where Lathrix will answer your questions! " +
                "Make sure to head over to role-assign and rules to ensure you have a nice experience.",
                ImageUrl = "https://pbs.twimg.com/media/EW-x7e1XkAAQqAw?format=jpg&name=small"
            };

            var messageBuilder = new DiscordMessageBuilder()
            {
                Embed = embedBuilder,
            };

            foreach (var button in new DiscordComponent[]
            {
                new DiscordLinkButtonComponent("https://www.youtube.com/user/Lathland", "YouTube", emoji: new DiscordComponentEmoji(978916644426494002)),
                new DiscordLinkButtonComponent("https://twitter.com/Lathrix", "Twitter", emoji: new DiscordComponentEmoji(978917283965583410)),
                //new DiscordLinkButtonComponent("https://www.twitch.tv/lathrix", "Twitch", emoji: new DiscordComponentEmoji(978917615823101952)),
                new DiscordLinkButtonComponent("https://docs.google.com/document/d/1Pq-7WVHn6uwcGWKH8fqznxRiJrRC4MeV7JmLUe-E2yA/edit#", "Role information document"),
                new DiscordLinkButtonComponent("https://docs.google.com/document/d/1FAJzwct6lgyuoxuVu_V8iSvzOVlP6_sqUuJYKZOhu8M/edit#", "Warn information document")
            })
            {
                messageBuilder.AddComponents(button);
            }

            DiscordMessage infoMessage = await ctx.Channel.GetMessageAsync(769235106828386366);
            await infoMessage.ModifyAsync(messageBuilder);
        }

        /// <summary>
        /// creates the embed for #rules
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("rules")]
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        [Description("Creates the embed for #information")]
        public async Task RuleEmbed(CommandContext ctx)
        {
            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(192037157416730625);
            DiscordEmbedBuilder embedBuilderRules = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "Rules",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "The rules here are not meant to be exhaustive, but those are sensible guidelines. " +
                    "In the end, the moderation team is human exception handlers and may handle disruptive situations not covered by the rules. " +
                    "Abide by their decisions. If you disagree, please keep any discussion in DMs or private channels as to not unnecessarily clog chat. " +
                    "The Plague Guards do not need to justify themselves in front of anyone but the Senate and Lathrix if he chooses to interfere."
                }
            };

            embedBuilderRules.AddField("Rule 1:", RuleService.Rules[0].RuleText);
            embedBuilderRules.AddField("Rule 2:", RuleService.Rules[1].RuleText);
            embedBuilderRules.AddField("Rule 3:", RuleService.Rules[2].RuleText);
            embedBuilderRules.AddField("Rule 4:", RuleService.Rules[3].RuleText);
            embedBuilderRules.AddField("Rule 5:", RuleService.Rules[4].RuleText);
            embedBuilderRules.AddField("Rule 6:", RuleService.Rules[5].RuleText);
            embedBuilderRules.AddField("Rule 7:", RuleService.Rules[6].RuleText);
            embedBuilderRules.AddField("Rule 8:", RuleService.Rules[7].RuleText);
            embedBuilderRules.AddField("Rule 9:", RuleService.Rules[8].RuleText);
            embedBuilderRules.AddField("Rule 10:", RuleService.Rules[9].RuleText);
            embedBuilderRules.AddField("Rule 11:", RuleService.Rules[10].RuleText);
            embedBuilderRules.AddField("Rule 12:", RuleService.Rules[11].RuleText);
            embedBuilderRules.AddField("Rule 13:", RuleService.Rules[12].RuleText);
            embedBuilderRules.AddField("Rule 0:", RuleService.Rules[13].RuleText);
            embedBuilderRules.AddField("Annotation", "Having a invisible nickname " +
                "AND profile picture as well as copying someones nick and pfp (impersonation) " +
                "will be seen as trying to avoid staff and is warnable!\n" +
                "Any communication outside of <#838088490704568341> while muted can and will be seen as punishment evasion and is as such also warnable!");

            DiscordEmbed embedRules = embedBuilderRules.Build();
            await ctx.Channel.SendMessageAsync(embedRules);

            DiscordEmbedBuilder embedBuilderNsfw = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "NSFW definition / guideline",
                Description = "To clarify the NSFW rules, anything that includes:\n" +
                "\n" +
                "**-Nudity,**\n" +
                "**-Gore,**\n" +
                "**-Sexually Suggestive Content**\n" +
                "\n" +
                "Will not be allowed."
            };

            DiscordEmbed embedNsfw = embedBuilderNsfw.Build();
            await ctx.Channel.SendMessageAsync(embedNsfw);
        }

        /// <summary>
        /// updates the embed for #rules
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("updaterules")]
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        [Description("Updates the embed for #information")]
        public async Task UpdateRuleEmbed(CommandContext ctx)
        {
            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(192037157416730625);
            DiscordEmbedBuilder embedBuilderRules = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "Rules",
                Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = "The rules here are not meant to be exhaustive, but those are sensible guidelines. " +
                        "In the end, the moderation team is human exception handlers and may handle disruptive situations not covered by the rules. " +
                        "Abide by their decisions. If you disagree, please keep any discussion in DMs or private channels as to not unnecessarily clog chat. " +
                        "The Plague Guards do not need to justify themselves in front of anyone but the Senate and Lathrix if he chooses to interfere."
                }
            };

            embedBuilderRules.AddField("Rule 1:", RuleService.Rules[0].RuleText);
            embedBuilderRules.AddField("Rule 2:", RuleService.Rules[1].RuleText);
            embedBuilderRules.AddField("Rule 3:", RuleService.Rules[2].RuleText);
            embedBuilderRules.AddField("Rule 4:", RuleService.Rules[3].RuleText);
            embedBuilderRules.AddField("Rule 5:", RuleService.Rules[4].RuleText);
            embedBuilderRules.AddField("Rule 6:", RuleService.Rules[5].RuleText);
            embedBuilderRules.AddField("Rule 7:", RuleService.Rules[6].RuleText);
            embedBuilderRules.AddField("Rule 8:", RuleService.Rules[7].RuleText);
            embedBuilderRules.AddField("Rule 9:", RuleService.Rules[8].RuleText);
            embedBuilderRules.AddField("Rule 10:", RuleService.Rules[9].RuleText);
            embedBuilderRules.AddField("Rule 11:", RuleService.Rules[10].RuleText);
            embedBuilderRules.AddField("Rule 12:", RuleService.Rules[11].RuleText);
            embedBuilderRules.AddField("Rule 13:", RuleService.Rules[12].RuleText);
            embedBuilderRules.AddField("Rule 0:", RuleService.Rules[13].RuleText);
            embedBuilderRules.AddField("Annotation", "Having a invisible nickname " +
                "AND profile picture as well as copying someones nick and pfp (impersonation) " +
                "will be seen as trying to avoid staff and is warnable!\n" +
                "Any communication outside of <#838088490704568341> while muted can and will be seen as punishment evasion and is as such also warnable!");

            DiscordMessage rulesMessage = await ctx.Channel.GetMessageAsync(769235312907649044);
            await rulesMessage.ModifyAsync("", embedBuilderRules.Build());

            DiscordEmbedBuilder embedBuilderNsfw = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "NSFW definition / guideline",
                Description = "To clarify the NSFW rules, anything that includes:\n" +
                "\n" +
                "**-Nudity,**\n" +
                "**-Gore,**\n" +
                "**-Sexually Suggestive Content**\n" +
                "\n" +
                "Will not be allowed."
            };

            DiscordMessage nsfwMessage = await ctx.Channel.GetMessageAsync(769235313968676864);
            await nsfwMessage.ModifyAsync("", embedBuilderNsfw.Build());
        }
    }
}
