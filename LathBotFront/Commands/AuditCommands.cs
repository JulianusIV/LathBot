using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;

namespace LathBotFront.Commands
{
	public class AuditCommands : BaseCommandModule
	{
		[Command("register")]
		[RequireRoles(RoleCheckMode.Any, "Bot Management")]
		public async Task Register(CommandContext ctx, DiscordMember mod)
		{
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			AuditRepository repo = new AuditRepository(ReadConfig.configJson.ConnectionString);
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
		[RequireRoles(RoleCheckMode.Any, "Bot Management")]
		public async Task Audit(CommandContext ctx)
		{
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			AuditRepository repo = new AuditRepository(ReadConfig.configJson.ConnectionString);
			bool result = urepo.GetIdByDcId(ctx.Member.Id, out int id);
			if (!result)
			{
				await ctx.RespondAsync("Error getting the user from the database");
				return;
			}
			result = repo.Read(id, out Audit audit);
			if (!result)
			{
				await ctx.RespondAsync("Error getting the audit from the database");
				return;
			}
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder
			{
				Title = "Moderator:",
				Description = $"{ctx.Member.DisplayName}#{ctx.Member.Discriminator} ({ctx.Member.Id})",
				Color = ctx.Member.Color,
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
				{
					Url = ctx.Member.AvatarUrl
				}
			};
			builder.AddField("Warn Amount:", audit.Warns.ToString());
			builder.AddField("Pardon Amount:", audit.Pardons.ToString());
			builder.AddField("Mute Amount:", audit.Mutes.ToString());
			builder.AddField("Unmute Amount:", audit.Unmutes.ToString());
			builder.AddField("Kick Amount:", audit.Kicks.ToString());
			builder.AddField("Ban Amount:", audit.Bans.ToString());
			await ctx.RespondAsync(builder);
		}
	}
}
