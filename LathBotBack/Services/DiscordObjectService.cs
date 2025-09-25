using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using LathBotBack.Base;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public DiscordChannel WarnLogChannel { get; private set; }

        public ulong Lathrix { get; private set; }
        public ulong Owner { get; private set; }
        public ulong APODRole { get; set; }

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
            DiscordObjectRepository discordObjectRepo = new(ReadConfig.Config.ConnectionString);
            if (!discordObjectRepo.GetAll(out List<DiscordObject> discordObjects))
                throw new Exception("Could not get DiscordObjects from database.");
            var dictionary = discordObjects.ToDictionary(x => x.ObjectName, x => x.ObjectId);

            this.Lathland = await client.GetGuildAsync(dictionary["MainGuildId"]);

            this.QuestionsChannel = await this.Lathland.GetChannelAsync(dictionary["QuestionsChannel"]);
            this.StaffChannel = await this.Lathland.GetChannelAsync(dictionary["StaffChannel"]);
            this.GoodGuysChannel = await this.Lathland.GetChannelAsync(dictionary["GoodGuysChannel"]);
            this.ErrorLogChannel = await this.Lathland.GetChannelAsync(dictionary["ErrorLogChannel"]);
            this.TimerChannel = await this.Lathland.GetChannelAsync(dictionary["TimerChannel"]);
            this.WarnsChannel = await this.Lathland.GetChannelAsync(dictionary["WarnsChannel"]);
            this.APODChannel = await this.Lathland.GetChannelAsync(dictionary["APODChannel"]);
            this.LogsChannel = await this.Lathland.GetChannelAsync(dictionary["LogsChannel"]);
            this.WarnLogChannel = await this.Lathland.GetChannelAsync(dictionary["WarnLogChannel"]);

            this.Lathrix = dictionary["Lathrix"];
            this.Owner = dictionary["Owner"];
            this.APODRole = dictionary["APODRole"];

            DiscordMessage lastStaffMessage = null;
            try
            {
                if (this.StaffChannel.LastMessageId is not null)
                    lastStaffMessage = this.StaffChannel.GetMessageAsync((ulong)this.StaffChannel.LastMessageId).GetAwaiter().GetResult();
            }
            catch (NotFoundException)
            {
            }
            if (lastStaffMessage?.Author.Id == client.CurrentUser.Id)
                this.StaffQuestions = lastStaffMessage;
            else
                this.StaffQuestions = this.StaffChannel.SendMessageAsync(this.StaffQuestionsEmbed.Build()).GetAwaiter().GetResult();

            DiscordMessage lastLathQuestion = null;
            try
            {
                if (this.QuestionsChannel.LastMessageId is not null)
                    lastLathQuestion = this.QuestionsChannel.GetMessageAsync((ulong)this.QuestionsChannel.LastMessageId).GetAwaiter().GetResult();
            }
            catch (NotFoundException)
            {
            }
            if (lastLathQuestion?.Author.Id == client.CurrentUser.Id)
                this.LathQuestions = lastLathQuestion;
            else
                this.LathQuestions = this.QuestionsChannel.SendMessageAsync(this.LathQuestionsEmbed).GetAwaiter().GetResult();

            this.LastEdits = [];
            this.LastDeletes = [];
        }
    }
}
