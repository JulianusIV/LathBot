using DSharpPlus.SlashCommands;
using LathBotBack.Config;
using LathBotBack.Repos;
using System.Threading.Tasks;

namespace LathBotFront.Interactions.PreExecutionChecks
{
    public class EmbedBannedAttribute : SlashCheckBaseAttribute
    {
        public override Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            var repo = new UserRepository(ReadConfig.Config.ConnectionString);
            repo.GetIdByDcId(ctx.Member.Id, out int id);
            repo.Read(id, out var user);
            return Task.FromResult(!user.EmbedBanned);
        }
    }
}
