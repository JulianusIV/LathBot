using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using LathBotBack.Config;
using LathBotBack.Repos;
using System.Threading.Tasks;

namespace LathBotFront.Interactions.PreExecutionChecks
{
    public class EmbedBannedAttribute : ContextCheckAttribute { }

    public class EmbedBannedCheck : IContextCheck<EmbedBannedAttribute>
    {
        public ValueTask<string> ExecuteCheckAsync(EmbedBannedAttribute attribute, CommandContext context)
        {
            var repo = new UserRepository(ReadConfig.Config.ConnectionString);
            repo.GetIdByDcId(context.Member.Id, out int id);
            repo.Read(id, out var user);
            if (user.EmbedBanned)
                return ValueTask.FromResult("User is banned from using this command");
            else
                return ValueTask.FromResult<string>(null);
        }
    }
}
