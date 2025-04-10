using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.Commands.ChoiceProviders
{
    public class BanMessageDeletionProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> deleteChoices =
        [
            new DiscordApplicationCommandOptionChoice("None - Dont delete any message", 0),
            new DiscordApplicationCommandOptionChoice("1 - Delete last one day of messages", 1),
            new DiscordApplicationCommandOptionChoice("2 - Delete last two days of messages", 2),
            new DiscordApplicationCommandOptionChoice("3 - Delete last three days of messages", 3),
            new DiscordApplicationCommandOptionChoice("4 - Delete last four days of messages", 4),
            new DiscordApplicationCommandOptionChoice("5 - Delete last five days of messages", 5),
            new DiscordApplicationCommandOptionChoice("6 - Delete last six days of messages", 6),
            new DiscordApplicationCommandOptionChoice("7 - Delete last seven days of messages", 7),
        ];

        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
            => ValueTask.FromResult(deleteChoices.AsEnumerable());
    }
}
