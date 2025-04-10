using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.Interactions.Autocomplete
{
    public class UserWarnAutocompleteProvider : IAutoCompleteProvider
    {
        public ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
        {
            var repo = new WarnRepository(ReadConfig.Config.ConnectionString);
            var urepo = new UserRepository(ReadConfig.Config.ConnectionString);
            urepo.GetIdByDcId((ulong)ctx.Options.First(x => x.Type == DiscordApplicationCommandOptionType.User).Value, out int dbId);
            repo.GetAllByUser(dbId, out List<Warn> warns);

            var choices = new List<DiscordAutoCompleteChoice>();
            foreach (var warn in warns.Where(x => !x.Persistent && x.Level < 11))
            {
                choices.Add(new DiscordAutoCompleteChoice($"Warn {warn.Number}: " + (warn.Reason.Length > 40 ? string.Concat(warn.Reason.Take(37)) + "..." : warn.Reason), warn.Number));
            }
            return ValueTask.FromResult(choices.Where(x =>
            {
                try
                {
                    return ctx.UserInput.Contains(x.Value.ToString());
                }
                catch
                {
                    return true;
                }
            }));
        }
    }
}
