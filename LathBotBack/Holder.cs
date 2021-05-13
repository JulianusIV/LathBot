using System;
using System.Timers;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;

namespace LathBotBack
{
	public class Holder
	{
		#region Singleton
		private static Holder instance = null;
		private static readonly object padlock = new object();
		public static Holder Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
						instance = new Holder();
					return instance;
				}
			}
		}
		#endregion

		public static Rule[] rules = new Rule[14]
		{
			new Rule(1, "Follow the discord [ToS](https://discord.com/terms) and [community guidelines](https://discord.com/guidelines).", 1, 15),
			new Rule(2, "Do not ping Lathrix", 1, 1),
			new Rule(3, "Use appropriate channels and follow the channel rules (description/pins).", 1, 5),
			new Rule(4, "Be honest with everyone.", 1, 1),
			new Rule(5, "Respect everyone", 1, 5),
			new Rule(6, "Don't post NSFW content.", 3, 15),
			new Rule(7, "Conversations about current Religious, Political, Military or any other \"touchy\" subject will not be tolerated. Some jokes you find funny others may not. This does not fully apply to the \"Debate\" channel.", 1, 5),
			new Rule(8, "Don't link pirate/hack/other illegal sites.", 3, 5),
			new Rule(9, "Don't ask for roles", 1, 2),
			new Rule(10, "Don't mini-mod", 1, 2),
			new Rule(11, "Don't use macros", 1, 3),
			new Rule(12, "Don't share personal information about others", 1, 10),
			new Rule(13, "Holding bias as a staff member is prohibited, or if you are a member who is trying to make staff bias. This will be known as corruption and should be reported to Chewybaca and/or a member of the Senate immediately.", 3, 10),
			new Rule(0, "", 1, 10)
		};

		public bool IsInDesignMode { get; private set; }
		public bool StartUpCompleted = false;
		public int GoodGuysReactionCount 
		{
			get
			{
				VariableRepository repo = new VariableRepository(ReadConfig.configJson.ConnectionString);
				bool result = repo.Read(2, out Variable entity);
				if (result)
				{
					return int.Parse(entity.Value);
				}
				return _goodGuysReactionCount;
			}
			set
			{
				_goodGuysReactionCount = value;
			}
		}
		private int _goodGuysReactionCount = 4;

		public DateTime StartTime { get; set; }

		public Dictionary<DiscordGuild, List<LavalinkTrack>> Queues;

		public DiscordGuild Lathland { get; private set; }

		public DiscordChannel QuestionsChannel { get; private set; }
		public DiscordChannel StaffChannel { get; private set; }
		public DiscordChannel GoodGuysChannel { get; private set; }
		public DiscordChannel ErrorLogChannel { get; private set; }
		public DiscordChannel TimerChannel { get; private set; }
		public DiscordChannel WarnsChannel { get; private set; }

		public DiscordMessage LathQuestions { get; set; }
		public DiscordMessage StaffQuestions { get; set; }

		public Timer WarnTimer = new Timer(3600000);

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

		public async void Init(DiscordClient client)
		{
			StartTime = DateTime.Now;

			WarnTimer.Start();

			Lathland = await client.GetGuildAsync(699555747591094344);

			QuestionsChannel = Lathland.GetChannel(721082217119612969);
			StaffChannel = Lathland.GetChannel(724313826786410508);
			GoodGuysChannel = Lathland.GetChannel(795654190143766578);
			ErrorLogChannel = Lathland.GetChannel(787423655566376970);
			TimerChannel = Lathland.GetChannel(771830187171250217);
			WarnsChannel = Lathland.GetChannel(722186358906421369);

			if (IsInDesignMode)
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
