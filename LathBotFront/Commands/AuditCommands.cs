using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.Commands
{
    public class AuditCommands
    {
        [Command("register")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task Register(CommandContext ctx, DiscordMember mod)
        {
            await ctx.DeferResponseAsync();

            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            AuditRepository repo = new(ReadConfig.Config.ConnectionString);
            bool result = urepo.GetIdByDcId(mod.Id, out int userId);
            if (!result)
            {
                await ctx.RespondAsync("Error reading the user from the database.");
                return;
            }
            result = repo.Create(new Audit(userId));
            if (!result)
            {
                await ctx.RespondAsync("Error creating the User.");
                return;
            }
            await ctx.RespondAsync("Successfully created an entry for the user.");
        }

        [Command("audit")]
        public async Task Audit(CommandContext ctx, DiscordMember mod = null)
            => await ctx.RespondAsync(await this.DoAudit(ctx, mod ?? ctx.Member));

        [Command("auditall")]
        [RequirePermissions(DiscordPermission.Administrator)]
        public async Task AuditAll(CommandContext ctx)
        {
            if (ctx is SlashCommandContext slashContext)
                await slashContext.RespondAsync("Working on it...", true);

            var members = ctx.Guild.GetAllMembersAsync().ToBlockingEnumerable();
            var channel = await ctx.Guild.GetChannelAsync(700350465174405170);
            var mods = members.Where(x => x
                .PermissionsIn(channel)
                .HasPermission(DiscordPermission.KickMembers) && !x.IsBot);

            var pages = new List<Page>();
            foreach (var item in mods)
            {
                var builder = await this.DoAudit(ctx, item);
                if (!builder.Embeds.Any())
                {
                    await ctx.RespondAsync(builder);
                    return;
                }
                pages.Add(new Page { Embed = builder.Embeds[0] });
            }

            _ = ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, PaginationBehaviour.WrapAround, ButtonPaginationBehavior.DeleteMessage);
        }

        private async Task<DiscordMessageBuilder> DoAudit(CommandContext ctx, DiscordMember mod)
        {
            var channel = await ctx.Guild.GetChannelAsync(700350465174405170);
            if (!mod.PermissionsIn(channel).HasPermission(DiscordPermission.KickMembers))
                return new DiscordMessageBuilder { Content = "Member is not a mod (anymore)." };

            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            AuditRepository repo = new(ReadConfig.Config.ConnectionString);
            bool result = urepo.GetIdByDcId(mod.Id, out int id);
            if (!result)
                return new DiscordMessageBuilder { Content = $"Error getting user {mod.Id} from the database" };

            result = repo.Read(id, out Audit audit);
            if (!result)
                return new DiscordMessageBuilder { Content = $"Error getting an audit for {mod.Id} from the database" };

            DiscordEmbedBuilder builder = new()
            {
                Title = "Moderator:",
                Description = $"{mod.DisplayName}#{mod.Discriminator} ({mod.Id})",
                Color = mod.Color,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = mod.AvatarUrl
                }
            };
            builder.AddField("Warn Amount:", audit.Warns.ToString());
            builder.AddField("Pardon Amount:", audit.Pardons.ToString());
            builder.AddField("Mute Amount:", audit.Mutes.ToString());
            builder.AddField("Timeout Amount:", audit.Timeouts.ToString());
            builder.AddField("Unmute Amount:", audit.Unmutes.ToString());
            builder.AddField("Kick Amount:", audit.Kicks.ToString());
            builder.AddField("Ban Amount:", audit.Bans.ToString());
            return new DiscordMessageBuilder().AddEmbed(builder.Build());
        }
    }
}
