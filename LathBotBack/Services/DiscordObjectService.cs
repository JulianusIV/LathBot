using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using LathBotBack.Base;
using System.Collections.Generic;
using System.Threading;

namespace LathBotBack.Services
{
    public class DiscordObjectService : BaseService
    {
        #region Singleton
        private static DiscordObjectService instance;
        private static readonly Lock padlock = new();
        public static DiscordObjectService Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new DiscordObjectService();
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

        public readonly DiscordEmbedBuilder LathQuestionsEmbed = new()
        {
            Title = "To make sure Lathrix does not get the same question multiple times please look at the pins.",
            Description = "This channel is not to be used to request things from Lathrix.\n" +
                    "Do not ask about old craft/remaking an old craft.\n" +
                    "Please ask questions in english only.\n" +
                    "Failure to follow these rules may result in a punishment.",
            Color = new DiscordColor(101, 24, 201)
        };
        public readonly DiscordEmbedBuilder StaffQuestionsEmbed = new()
        {
            Title = "This channel is for asking questions, that the staff will answer.",
            Description = "To make sure they do not get the same question multiple times please look at the pins.\n" +
                    "Do not answer questions for others.\n" +
                    "This channel should not, under any circumstances, be used to request things from the staff.\n" +
                    "Please ask questions in english only.\n" +
                    "Failure to follow these rules may result in a punishment",
            Color = new DiscordColor(27, 116, 226)
        };

        public override async void Init(DiscordClient client)
        {
            this.Lathland = client.GetGuildAsync(699555747591094344).GetAwaiter().GetResult();

            this.QuestionsChannel = await this.Lathland.GetChannelAsync(721082217119612969);
            this.StaffChannel = await this.Lathland.GetChannelAsync(724313826786410508);
            this.GoodGuysChannel = await this.Lathland.GetChannelAsync(795654190143766578);
            this.ErrorLogChannel = await this.Lathland.GetChannelAsync(787423655566376970);
            this.TimerChannel = await this.Lathland.GetChannelAsync(771830187171250217);
            this.WarnsChannel = await this.Lathland.GetChannelAsync(722186358906421369);
            this.APODChannel = await this.Lathland.GetChannelAsync(848240982880550932);
            this.StaffChannel = await this.Lathland.GetChannelAsync(724313826786410508);
            this.LogsChannel = await this.Lathland.GetChannelAsync(700009728151126036);
            this.QuestionsChannel = await this.Lathland.GetChannelAsync(721082217119612969);

            DiscordMessage lastStaffMessage = null;
            try
            {
                lastStaffMessage = this.StaffChannel.GetMessageAsync((ulong)this.StaffChannel.LastMessageId).GetAwaiter().GetResult();
            }
            catch (NotFoundException)
            {
            }
            if (lastStaffMessage?.Author.Id == 708083256439996497)
                this.StaffQuestions = lastStaffMessage;
            else
                this.StaffQuestions = this.StaffChannel.SendMessageAsync(this.StaffQuestionsEmbed.Build()).GetAwaiter().GetResult();

            DiscordMessage lastLathQuestion = null;
            try
            {
                lastLathQuestion = this.QuestionsChannel.GetMessageAsync((ulong)this.QuestionsChannel.LastMessageId).GetAwaiter().GetResult();
            }
            catch (NotFoundException)
            {
            }
            if (lastLathQuestion?.Author.Id == 708083256439996497)
                this.LathQuestions = lastLathQuestion;
            else
                this.LathQuestions = this.QuestionsChannel.SendMessageAsync(this.LathQuestionsEmbed).GetAwaiter().GetResult();

            this.LastEdits = [];
            this.LastDeletes = [];
        }
    }
}
