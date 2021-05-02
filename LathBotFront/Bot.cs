using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Net;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;

using Newtonsoft.Json;
using LathBotFront.Commands;
using LathBotBack.Config;

namespace LathBotFront
{
	public class Bot
	{
		#region singleton
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

		public readonly bool IsInDesignMode = true;

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
			Client.Ready += OnClientReady;
			Client.GuildDownloadCompleted += Client_GuildDownloadCompleted;

			Client.UseInteractivity(new InteractivityConfiguration
			{
				Timeout = TimeSpan.FromMinutes(5),
				PollBehaviour = PollBehaviour.KeepEmojis
			});

			ConnectionEndpoint endpoint = new ConnectionEndpoint
			{
				Hostname = "173.212.204.145",
				Port = 2333
			};

			if (IsInDesignMode)
				endpoint.Hostname = "192.168.0.136";

			LavalinkConfiguration lavalinkConfig = new LavalinkConfiguration
			{
				Password = "youshallnotpass",
				RestEndpoint = endpoint,
				SocketEndpoint = endpoint
			};

			LavalinkExtension lavalink = Client.UseLavalink();

			CommandsNextConfiguration commandsConfig = new CommandsNextConfiguration
			{
				StringPrefixes = new string[] { ReadConfig.configJson.Prefix },
				EnableMentionPrefix = true
			};
			Commands = Client.UseCommandsNext(commandsConfig);

			//Register commands
			Commands.RegisterCommands<TechnicalCommands>();

			await Client.ConnectAsync();

			if (!IsInDesignMode)
			{
				LavalinkNodeConnection lavaNode = await lavalink.ConnectAsync(lavalinkConfig);
			}

			await Task.Delay(-1);
		}

		private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
		{
			//DiscordActivity activity = new DiscordActivity("a Turtle.", ActivityType.Streaming)
			//{
			//	StreamUrl = "https://www.twitch.tv/lathland"
			//};
			DiscordActivity activity = new DiscordActivity("flamethrower music.", ActivityType.ListeningTo);
			Client.UpdateStatusAsync(activity);
			return Task.CompletedTask;
		}

		private Task Client_GuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
		{
			return Task.CompletedTask;
		}
	}
}
