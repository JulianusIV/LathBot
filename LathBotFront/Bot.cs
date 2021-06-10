using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using LathBotBack.Config;
using LathBotBack.Services;
using LathBotFront.Commands;

namespace LathBotFront
{
	public class Bot
	{
		#region Singleton
		private static Bot instance = null;
		private static readonly object padlock = new object();
		public static Bot Instance
		{
			get
			{
				lock (padlock)
				{
					if (instance == null)
						instance = new Bot();
					return instance;
				}
			}
		}
		#endregion

		public DiscordClient Client { get; private set; }

		public InteractivityExtension Interactivity { get; private set; }

		public CommandsNextExtension Commands { get; private set; }

		public async Task RunAsync()
		{
			ReadConfig.Read();

			DiscordConfiguration config = new DiscordConfiguration
			{
				Token = ReadConfig.configJson.Token,
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
				Intents = DiscordIntents.All
			};

			Client = new DiscordClient(config);

			//Register client events
			Client.Ready += Events.OnClientReady;
			Client.GuildDownloadCompleted += Events.Client_GuildDownloadCompleted;
			Client.MessageCreated += Events.MessageCreated;
			Client.MessageUpdated += Events.MessageUpdated;
			Client.MessageDeleted += Events.MessageDeleted;
			Client.GuildMemberAdded += Events.MemberAdded;
			Client.MessageReactionAdded += Events.ReactionAdded;
			Client.MessageReactionRemoved += Events.ReactionRemoved;
			Client.VoiceStateUpdated += Events.VoiceStateUpdated;
			Client.ClientErrored += Events.ClientErrored;
			Client.ComponentInteractionCreated += Events.ComponentTriggered;
			
			//Register timer events
			SystemService.Instance.WarnTimer.Elapsed += Events.TimerTick;

			//Register Logger events
			SystemService.Instance.Logger.RaiseLogEvent += Events.OnLog;

			Client.UseInteractivity(new InteractivityConfiguration
			{
				Timeout = TimeSpan.FromMinutes(5),
				PollBehaviour = PollBehaviour.KeepEmojis
			});

			CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
			{
				StringPrefixes = new string[] { ReadConfig.configJson.Prefix },
				EnableMentionPrefix = true
			};
			Commands = Client.UseCommandsNext(commandsConfig);

			//Register commands
			Commands.RegisterCommands<AuditCommands>();
			Commands.RegisterCommands<EmbedCommands>();
			Commands.RegisterCommands<InfoCommands>();
			Commands.RegisterCommands<LavalinkCommands>();
			Commands.RegisterCommands<ReactionCommands>();
			Commands.RegisterCommands<RuleCommands>();
			Commands.RegisterCommands<TechnicalCommands>();
			Commands.RegisterCommands<WarnCommands>();

			//Register command events
			Commands.CommandErrored += Events.CommandErrored;

			await Client.ConnectAsync();

			LavalinkNodeConnection lavaNode = null;
			if (!StartupService.Instance.IsInDesignMode)
			{
				lavaNode = await ConnectLavaNodeAsync();
			}

			if (lavaNode != null)
			{
				//Register lava commands
				lavaNode.PlaybackFinished += Events.PlaybackFinished;
			}

			await Task.Delay(-1);
		}

		private async Task<LavalinkNodeConnection> ConnectLavaNodeAsync()
		{
			ConnectionEndpoint endpoint = new ConnectionEndpoint
			{
				Hostname = "server.local",
				Port = 2333
			};

			LavalinkConfiguration lavalinkConfig = new LavalinkConfiguration
			{
				Password = "youshallnotpass",
				RestEndpoint = endpoint,
				SocketEndpoint = endpoint,
				SocketAutoReconnect = false
			};

			LavalinkExtension lavalink = Client.UseLavalink();

			LavalinkNodeConnection res = null;
			try
			{
				res = await lavalink.ConnectAsync(lavalinkConfig);
			}
			catch (WebSocketException e)
			{
				SystemService.Instance.Logger.Log("Failed to start connection with Lavalink server:\n" + e.Message);
			}

			return res;
		}
	}
}
