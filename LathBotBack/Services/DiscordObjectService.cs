using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using LathBotBack.Base;
using System.Collections.Generic;

namespace LathBotBack.Services
{
    public class DiscordObjectService : BaseService
    {
        #region Singleton
        private static DiscordObjectService instance;
        private static readonly object padlock = new object();
        public static DiscordObjectService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                        instance = new DiscordObjectService();
                    return instance;
                }
            }
        }
        #endregion

        public DiscordGuild Lathland { get; private set; }

        public DiscordChannel QuestionsChannel { get; private set; }
        public DiscordChannel StaffChannel { get; private set; }
        public DiscordChannel GoodGuysChannel { get; private set; }
        public DiscordChannel ErrorLogChannel { get; private set; }
        public DiscordChannel TimerChannel { get; private set; }
        public DiscordChannel WarnsChannel { get; private set; }
        public DiscordChannel APODChannel { get; private set; }
        public DiscordChannel LogsChannel { get; private set; }

        public DiscordMessage LathQuestions { get; set; }
        public DiscordMessage StaffQuestions { get; set; }
        public Dictionary<ulong, DiscordMessage> LastDeletes { get; set; }
        public Dictionary<ulong, DiscordMessage> LastEdits { get; set; }

        public readonly DiscordEmbedBuilder LathQuestionsEmbed = new DiscordEmbedBuilder
        {
            Title = "To make sure Lathrix does not get the same question multiple times please look at the pins.",
            Description = "This channel is not to be used to request things from Lathrix.\n" +
                    "Do not ask about old craft/remaking an old craft.\n" +
                    "Please ask questions in english only.\n" +
                    "Failure to follow these rules may result in a punishment.",
            Color = new DiscordColor(101, 24, 201)
        };
        public readonly DiscordEmbedBuilder StaffQuestionsEmbed = new DiscordEmbedBuilder
        {
            Title = "This channel is for asking questions, that the staff will answer.",
            Description = "To make sure they do not get the same question multiple times please look at the pins.\n" +
                    "Do not answer questions for others.\n" +
                    "This channel should not, under any circumstances, be used to request things from the staff.\n" +
                    "Please ask questions in english only.\n" +
                    "Failure to follow these rules may result in a punishment",
            Color = new DiscordColor(27, 116, 226)
        };

        public override void Init(DiscordClient client)
        {
            Lathland = client.GetGuildAsync(699555747591094344).GetAwaiter().GetResult();

            QuestionsChannel = Lathland.GetChannel(721082217119612969);
            StaffChannel = Lathland.GetChannel(724313826786410508);
            GoodGuysChannel = Lathland.GetChannel(795654190143766578);
            ErrorLogChannel = Lathland.GetChannel(787423655566376970);
            TimerChannel = Lathland.GetChannel(771830187171250217);
            WarnsChannel = Lathland.GetChannel(722186358906421369);
            APODChannel = Lathland.GetChannel(848240982880550932);
            StaffChannel = Lathland.GetChannel(724313826786410508);
            LogsChannel = Lathland.GetChannel(700009728151126036);
            QuestionsChannel = Lathland.GetChannel(721082217119612969);

            DiscordMessage lastStaffMessage = null;
            try
            {
                lastStaffMessage = StaffChannel.GetMessageAsync((ulong)StaffChannel.LastMessageId).GetAwaiter().GetResult();
            }
            catch (NotFoundException)
            {
            }
            if (lastStaffMessage?.Author.Id == 708083256439996497)
                StaffQuestions = lastStaffMessage;
            else
                StaffQuestions = StaffChannel.SendMessageAsync(StaffQuestionsEmbed.Build()).GetAwaiter().GetResult();

            DiscordMessage lastLathQuestion = null;
            try
            {
                lastLathQuestion = QuestionsChannel.GetMessageAsync((ulong)QuestionsChannel.LastMessageId).GetAwaiter().GetResult();
            }
            catch (NotFoundException)
            {
            }
            if (lastLathQuestion?.Author.Id == 708083256439996497)
                LathQuestions = lastLathQuestion;
            else
                LathQuestions = QuestionsChannel.SendMessageAsync(LathQuestionsEmbed).GetAwaiter().GetResult();

            LastEdits = new Dictionary<ulong, DiscordMessage>();
            LastDeletes = new Dictionary<ulong, DiscordMessage>();
        }
    }
}
