using System.Threading.Tasks;

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
		public async Task Register(CommandContext ctx)
		{
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			AuditRepository repo = new AuditRepository(ReadConfig.configJson.ConnectionString);
			bool result = urepo.GetIdByDcId(ctx.Member.Id, out int userId);
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
		}
	}
}
