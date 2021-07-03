using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using LathBotBack;
using LathBotBack.Services;

namespace LathBotFront.Commands
{
	public class EmbedCommands : BaseCommandModule
    {
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
            DiscordEmbedBuilder embedBuilderLinks = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "Links",
                Description = "- [Youtube](https://www.youtube.com/user/Lathland)\n" +
                "- [Twitter](https://twitter.com/Lathrix)\n" +
                //"- [- Twitch](https://www.twitch.tv/lathrix)\n" +
                "- [Role Information Document](https://docs.google.com/document/d/1Pq-7WVHn6uwcGWKH8fqznxRiJrRC4MeV7JmLUe-E2yA/edit#)\n" +
                "- [Warn Information Document](https://docs.google.com/document/d/1FAJzwct6lgyuoxuVu_V8iSvzOVlP6_sqUuJYKZOhu8M/edit#)",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Height = 10, Width = 10, Url = lathrix.AvatarUrl }
            };
            DiscordEmbed embedLinks = embedBuilderLinks.Build();
            await ctx.Channel.SendMessageAsync(embedLinks).ConfigureAwait(false);

            DiscordEmbedBuilder embedBuilderInfo = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "Information",
                Description = "Greetings Sir's and Siret's, and welcome to the Lathland discord server!" +
                " You can talk to fellow Lathrixians, share art and post videos. " +
                "There is even a section where Lathrix will answer your questions! " +
                "Make sure to head over to role-assign and rules to ensure you have a nice experience.",
                ImageUrl = "https://pbs.twimg.com/media/EW-x7e1XkAAQqAw?format=jpg&name=small"
            };

            DiscordEmbed embedInfo = embedBuilderInfo.Build();
            await ctx.Channel.SendMessageAsync(embedInfo).ConfigureAwait(false);
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
            DiscordEmbedBuilder embedBuilderLinks = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "Links",
                Description = "- [Youtube](https://www.youtube.com/user/Lathland)\n" +
                "- [Twitter](https://twitter.com/Lathrix)\n" +
                //"- [- Twitch](https://www.twitch.tv/lathrix)\n" +
                "- [Role Information Document](https://docs.google.com/document/d/1Pq-7WVHn6uwcGWKH8fqznxRiJrRC4MeV7JmLUe-E2yA/edit#)\n" +
                "- [Warn Information Document](https://docs.google.com/document/d/1FAJzwct6lgyuoxuVu_V8iSvzOVlP6_sqUuJYKZOhu8M/edit#)",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Height = 10, Width = 10, Url = lathrix.AvatarUrl }
            };

            DiscordMessage linksMessage = await ctx.Channel.GetMessageAsync(769235082878648340);
            await linksMessage.ModifyAsync("", embedBuilderLinks.Build()).ConfigureAwait(false);

            DiscordEmbedBuilder embedBuilderInfo = new DiscordEmbedBuilder
            {
                Color = lathrix.Color,
                Title = "Information",
                Description = "Greetings Sir's and Siret's, and welcome to the Lathland discord server!" +
                " You can talk to fellow Lathrixians, share art and post videos. " +
                "There is even a section where Lathrix will answer your questions! " +
                "Make sure to head over to role-assign and rules to ensure you have a nice experience.",
                ImageUrl = "https://pbs.twimg.com/media/EW-x7e1XkAAQqAw?format=jpg&name=small"
            };

            DiscordEmbed embedInfo = embedBuilderInfo.Build();
            DiscordMessage infoMessage = await ctx.Channel.GetMessageAsync(769235106828386366);
            await infoMessage.ModifyAsync("", embedInfo).ConfigureAwait(false);
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

            embedBuilderRules.AddField("Rule 1:", RuleService.rules[0].RuleText);
            embedBuilderRules.AddField("Rule 2:", RuleService.rules[1].RuleText);
            embedBuilderRules.AddField("Rule 3:", RuleService.rules[2].RuleText);
            embedBuilderRules.AddField("Rule 4:", RuleService.rules[3].RuleText);
            embedBuilderRules.AddField("Rule 5:", RuleService.rules[4].RuleText);
            embedBuilderRules.AddField("Rule 6:", RuleService.rules[5].RuleText);
            embedBuilderRules.AddField("Rule 7:", RuleService.rules[6].RuleText);
            embedBuilderRules.AddField("Rule 8:", RuleService.rules[7].RuleText);
            embedBuilderRules.AddField("Rule 9:", RuleService.rules[8].RuleText);
            embedBuilderRules.AddField("Rule 10:", RuleService.rules[9].RuleText);
            embedBuilderRules.AddField("Rule 11:", RuleService.rules[10].RuleText);
            embedBuilderRules.AddField("Rule 12:", RuleService.rules[11].RuleText);
            embedBuilderRules.AddField("Rule 13:", RuleService.rules[12].RuleText);
            embedBuilderRules.AddField("Annotation", "Having a invisible nickname " +
                "AND profile picture as well as copying someones nick and pfp (impersonation) " +
                "will be seen as trying to avoid staff and is warnable!\n" +
                "Any communication outside of <#838088490704568341> while muted can and will be seen as punishment evasion and is as such also warnable!");

            DiscordEmbed embedRules = embedBuilderRules.Build();
            await ctx.Channel.SendMessageAsync(embedRules).ConfigureAwait(false);

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
            await ctx.Channel.SendMessageAsync(embedNsfw).ConfigureAwait(false);
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

            embedBuilderRules.AddField("Rule 1:", RuleService.rules[0].RuleText);
            embedBuilderRules.AddField("Rule 2:", RuleService.rules[1].RuleText);
            embedBuilderRules.AddField("Rule 3:", RuleService.rules[2].RuleText);
            embedBuilderRules.AddField("Rule 4:", RuleService.rules[3].RuleText);
            embedBuilderRules.AddField("Rule 5:", RuleService.rules[4].RuleText);
            embedBuilderRules.AddField("Rule 6:", RuleService.rules[5].RuleText);
            embedBuilderRules.AddField("Rule 7:", RuleService.rules[6].RuleText);
            embedBuilderRules.AddField("Rule 8:", RuleService.rules[7].RuleText);
            embedBuilderRules.AddField("Rule 9:", RuleService.rules[8].RuleText);
            embedBuilderRules.AddField("Rule 10:", RuleService.rules[9].RuleText);
            embedBuilderRules.AddField("Rule 11:", RuleService.rules[10].RuleText);
            embedBuilderRules.AddField("Rule 12:", RuleService.rules[11].RuleText);
            embedBuilderRules.AddField("Rule 13:", RuleService.rules[12].RuleText);
            embedBuilderRules.AddField("Annotation", "Having a invisible nickname " +
                "AND profile picture as well as copying someones nick and pfp (impersonation) " +
                "will be seen as trying to avoid staff and is warnable!\n" +
                "The same applies to using alt accounts during mutes.");

            DiscordMessage rulesMessage = await ctx.Channel.GetMessageAsync(769235312907649044);
            await rulesMessage.ModifyAsync("", embedBuilderRules.Build()).ConfigureAwait(false);

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
            await nsfwMessage.ModifyAsync("", embedBuilderNsfw.Build()).ConfigureAwait(false);
        }
    }
}
