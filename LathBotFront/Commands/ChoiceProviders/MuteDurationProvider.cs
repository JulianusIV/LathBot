using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.Commands.ChoiceProviders
{
    public class MuteDurationProvider : IChoiceProvider
    {
        private static readonly IReadOnlyList<DiscordApplicationCommandOptionChoice> durations =
        [
            new DiscordApplicationCommandOptionChoice("2 - Two days", 2),
            new DiscordApplicationCommandOptionChoice("3 - Three days", 3),
            new DiscordApplicationCommandOptionChoice("4 - Four days", 4),
            new DiscordApplicationCommandOptionChoice("5 - Five days", 5),
            new DiscordApplicationCommandOptionChoice("6 - Six days", 6),
            new DiscordApplicationCommandOptionChoice("7 - Seven days", 7),
            new DiscordApplicationCommandOptionChoice("8 - Eight days", 8),
            new DiscordApplicationCommandOptionChoice("9 - Nine days", 9),
            new DiscordApplicationCommandOptionChoice("10 - Ten days", 10),
            new DiscordApplicationCommandOptionChoice("11 - Eleven days", 11),
            new DiscordApplicationCommandOptionChoice("12 - Twelve days", 12),
            new DiscordApplicationCommandOptionChoice("13 - Thirteen days", 13),
            new DiscordApplicationCommandOptionChoice("14 - Fourteen days", 14)
        ];

        public ValueTask<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideAsync(CommandParameter parameter)
            => ValueTask.FromResult(durations.AsEnumerable());
    }
}
