using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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
    public class AuditCommands : BaseCommandModule
    {
        [Command("register")]
        [RequireRoles(RoleCheckMode.Any, "Bot Management")]
        public async Task Register(CommandContext ctx, DiscordMember mod)
        {
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
        public async Task Audit(CommandContext ctx) 
            => await ctx.RespondAsync(DoAudit(ctx, ctx.Member));

        [Command("Audit")]
        public async Task Audit(CommandContext ctx, DiscordMember mod) 
            => await ctx.RespondAsync(DoAudit(ctx, mod));

        [Command("auditall")]
        [RequireUserPermissions(Permissions.Administrator)]
        public async Task AuditAll(CommandContext ctx)
        {
            var members = await ctx.Guild.GetAllMembersAsync();
            var mods = members.Where(x => x
                .PermissionsIn(ctx.Guild.GetChannel(700350465174405170))
                .HasPermission(Permissions.KickMembers) && !x.IsBot);

            var pages = new List<Page>();
            foreach (var item in mods)
            {
                var builder = DoAudit(ctx, item);
                if (builder.Embed == null)
                {
                    await ctx.RespondAsync(builder);
                    return;
                }
                pages.Add(new Page { Embed = builder.Embed });
            }
            _ = ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, PaginationBehaviour.WrapAround, ButtonPaginationBehavior.DeleteMessage);
        }

        private DiscordMessageBuilder DoAudit(CommandContext ctx, DiscordMember mod)
        {
            if (!mod.PermissionsIn(ctx.Guild.GetChannel(700350465174405170)).HasPermission(Permissions.KickMembers))
            {
                return new DiscordMessageBuilder { Content = "Member is not a mod (anymore)." };
            }
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            AuditRepository repo = new(ReadConfig.Config.ConnectionString);
            bool result = urepo.GetIdByDcId(mod.Id, out int id);
            if (!result)
            {
                return new DiscordMessageBuilder { Content = $"Error getting user {mod.Id} from the database" };
            }
            result = repo.Read(id, out Audit audit);
            if (!result)
            {
                return new DiscordMessageBuilder { Content = $"Error getting an audit for {mod.Id} from the database" };
            }
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
            return new DiscordMessageBuilder { Embed = builder.Build() };
        }
    }
}
