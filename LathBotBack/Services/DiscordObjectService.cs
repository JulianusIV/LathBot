using DSharpPlus;
using DSharpPlus.Entities;
using LathBotBack.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LathBotBack.Services
{
	class DiscordObjectService : BaseService<DiscordObjectService>
	{
		public DiscordGuild Lathland { get; private set; }

		public DiscordChannel QuestionsChannel { get; private set; }
		public DiscordChannel StaffChannel { get; private set; }
		public DiscordChannel GoodGuysChannel { get; private set; }
		public DiscordChannel ErrorLogChannel { get; private set; }
		public DiscordChannel TimerChannel { get; private set; }
		public DiscordChannel WarnsChannel { get; private set; }

		public DiscordMessage LathQuestions { get; set; }
		public DiscordMessage StaffQuestions { get; set; }

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

		public async override void Init(DiscordClient client)
		{
			Lathland = await client.GetGuildAsync(699555747591094344);

			QuestionsChannel = Lathland.GetChannel(721082217119612969);
			StaffChannel = Lathland.GetChannel(724313826786410508);
			GoodGuysChannel = Lathland.GetChannel(795654190143766578);
			ErrorLogChannel = Lathland.GetChannel(787423655566376970);
			TimerChannel = Lathland.GetChannel(771830187171250217);
			WarnsChannel = Lathland.GetChannel(722186358906421369);

			if (StartupService.Instance.IsInDesignMode)
				return;

			StaffChannel = Lathland.GetChannel(724313826786410508);
			DiscordMessage lastStaffMessage = await StaffChannel.GetMessageAsync((ulong)StaffChannel.LastMessageId);
			if (lastStaffMessage.Author.Id == 708083256439996497)
				StaffQuestions = lastStaffMessage;
			else
				StaffQuestions = await StaffChannel.SendMessageAsync(StaffQuestionsEmbed.Build()).ConfigureAwait(false);

			QuestionsChannel = Lathland.GetChannel(721082217119612969);
			DiscordMessage lastLathQuestion = await QuestionsChannel.GetMessageAsync((ulong)QuestionsChannel.LastMessageId);
			if (lastLathQuestion.Author.Id == 708083256439996497)
				LathQuestions = lastLathQuestion;
			else
				LathQuestions = await QuestionsChannel.SendMessageAsync(LathQuestionsEmbed).ConfigureAwait(false);
		}
	}
}
