using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using LathBotBack.Services;
using System.ComponentModel;
using System.Threading.Tasks;

namespace LathBotFront.Commands
{
    public class EmbedCommands
    {
        [Command("roleme")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Creates the embed for #role-assign")]
        public async Task RolemeEmbed(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(DiscordObjectService.Instance.Lathrix);
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Color = lathrix.Color,
                Title = "Role Assign",
                Description = "Get your roles here.\n" +
                    "Each role unlocks new channels (once you are verified) for you to see and/or send messages in.\n" +
                    "This will still be available after you successfully verified yourself in <#767049785223020556>"
            };

            var messageBuilder = new DiscordMessageBuilder().AddEmbed(embedBuilder);

            messageBuilder.AddActionRowComponent(
            [
                new(DiscordButtonStyle.Primary, "roleme_games", "Games", emoji: new DiscordComponentEmoji("🎮")),
                new(DiscordButtonStyle.Primary, "roleme_misc", "Misc", emoji: new DiscordComponentEmoji("🏷")),
                new(DiscordButtonStyle.Primary, "roleme_color", "Color", emoji: new DiscordComponentEmoji("🎨"))
            ]);
            await ctx.Channel.SendMessageAsync(messageBuilder);

            await ctx.RespondAsync("Done!");
        }

        [Command("updateroleme")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Creates the embed for #role-assign")]
        public async Task EditRolemeEmbed(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(DiscordObjectService.Instance.Lathrix);
            var embedBuilder = new DiscordEmbedBuilder()
            {
                Color = lathrix.Color,
                Title = "Role Assign",
                Description = "Get your roles here.\n" +
                    "Each role unlocks new channels (once you are verified) for you to see and/or send messages in.\n" +
                    "This will still be available after you successfully verified yourself in <#767049785223020556>"
            };

            var messageBuilder = new DiscordMessageBuilder().AddEmbed(embedBuilder);

            messageBuilder.AddActionRowComponent(
            [
                new(DiscordButtonStyle.Primary, "roleme_games", "Games", emoji: new DiscordComponentEmoji("🎮")),
                new(DiscordButtonStyle.Primary, "roleme_misc", "Misc", emoji: new DiscordComponentEmoji("🏷")),
                new(DiscordButtonStyle.Primary, "roleme_color", "Color", emoji: new DiscordComponentEmoji("🎨"))
            ]);

            DiscordMessage infoMessage = await ctx.Channel.GetMessageAsync(1014293811435942040);
            await infoMessage.ModifyAsync(messageBuilder);

            await ctx.RespondAsync("Done!");
        }

        /// <summary>
        /// creates the embed for #information
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("infoembed")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Creates the embed for #information")]
        public async Task InfoEmbed(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(DiscordObjectService.Instance.Lathrix);
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = lathrix.Color,
                Title = "Information",
                Description = "Greetings Sir's and Siret's, and welcome to the Lathland discord server!" +
                " You can talk to fellow Lathrixians, share art and post videos. " +
                "There is even a section where Lathrix will answer your questions! " +
                "Make sure to head over to role-assign and rules to ensure you have a nice experience.",
                ImageUrl = "https://pbs.twimg.com/media/EW-x7e1XkAAQqAw?format=jpg&name=small",
            };

            var messageBuilder = new DiscordMessageBuilder().AddEmbed(embedBuilder);

            foreach (var button in new DiscordButtonComponent[]
            {
                new DiscordLinkButtonComponent("https://www.youtube.com/user/Lathland", "YouTube", emoji: new DiscordComponentEmoji(978916644426494002)),
                new DiscordLinkButtonComponent("https://twitter.com/Lathrix", "Twitter", emoji: new DiscordComponentEmoji(978917283965583410)),
                //new DiscordLinkButtonComponent("https://www.twitch.tv/lathrix", "Twitch", emoji: new DiscordComponentEmoji(978917615823101952)),
                new DiscordLinkButtonComponent("https://docs.google.com/document/d/1Pq-7WVHn6uwcGWKH8fqznxRiJrRC4MeV7JmLUe-E2yA/edit#", "Role information document"),
                new DiscordLinkButtonComponent("https://docs.google.com/document/d/1FAJzwct6lgyuoxuVu_V8iSvzOVlP6_sqUuJYKZOhu8M/edit#", "Warn information document")
            })
            {
                messageBuilder.AddActionRowComponent(button);
            }

            await ctx.Channel.SendMessageAsync(messageBuilder);

            await ctx.RespondAsync("Done!");
        }

        /// <summary>
        /// updates the embed for #information
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("updateinfo")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Updates the embed for #rules")]
        public async Task UpdateInfoEmbed(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(DiscordObjectService.Instance.Lathrix);
            DiscordEmbedBuilder embedBuilder = new()
            {
                Color = lathrix.Color,
                Title = "Information",
                Description = "Greetings Sir's and Siret's, and welcome to the Lathland discord server!" +
                " You can talk to fellow Lathrixians, share art and post videos. " +
                "There is even a section where Lathrix will answer your questions! " +
                "Make sure to head over to role-assign and rules to ensure you have a nice experience.",
                ImageUrl = "https://pbs.twimg.com/media/EW-x7e1XkAAQqAw?format=jpg&name=small"
            };

            var messageBuilder = new DiscordMessageBuilder().AddEmbed(embedBuilder);

            messageBuilder.AddActionRowComponent(new DiscordLinkButtonComponent("https://www.youtube.com/user/Lathland", "YouTube", emoji: new DiscordComponentEmoji(978916644426494002)),
                new DiscordLinkButtonComponent("https://twitter.com/Lathrix", "Twitter", emoji: new DiscordComponentEmoji(978917283965583410)),
                //new DiscordLinkButtonComponent("https://www.twitch.tv/lathrix", "Twitch", emoji: new DiscordComponentEmoji(978917615823101952)),
                new DiscordLinkButtonComponent("https://docs.google.com/document/d/1Pq-7WVHn6uwcGWKH8fqznxRiJrRC4MeV7JmLUe-E2yA/edit#", "Role information document"),
                new DiscordLinkButtonComponent("https://docs.google.com/document/d/1FAJzwct6lgyuoxuVu_V8iSvzOVlP6_sqUuJYKZOhu8M/edit#", "Warn information document"));

            DiscordMessage infoMessage = await ctx.Channel.GetMessageAsync(769235106828386366);
            await infoMessage.ModifyAsync(messageBuilder);

            await ctx.RespondAsync("Done!");
        }

        /// <summary>
        /// creates the embed for #rules
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("rules")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Creates the embed for #information")]
        public async Task RuleEmbed(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(DiscordObjectService.Instance.Lathrix);
            DiscordEmbedBuilder embedBuilderRules = new()
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

            DiscordEmbedBuilder embedBuilderNsfw = new()
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

            await ctx.RespondAsync("Done!");
        }

        /// <summary>
        /// updates the embed for #rules
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [Command("updaterules")]
        [RequirePermissions(DiscordPermission.Administrator)]
        [Description("Updates the embed for #information")]
        public async Task UpdateRuleEmbed(SlashCommandContext ctx)
        {
            await ctx.DeferResponseAsync(true);

            DiscordMember lathrix = await ctx.Guild.GetMemberAsync(DiscordObjectService.Instance.Lathrix);
            DiscordEmbedBuilder embedBuilderRules = new()
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

            DiscordEmbedBuilder embedBuilderNsfw = new()
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

            await ctx.RespondAsync("Done!");
        }
    }
}
