﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using LathBotFront.Commands;
using LathBotFront.EventHandlers;
using LathBotFront.Interactions;
using System;
using System.Threading.Tasks;
using UptimeKumaHeartbeat;

namespace LathBotFront
{
    public class Bot
    {
        #region Singleton
        private static Bot instance = null;
        private static readonly object padlock = new();
        public static Bot Instance
        {
            get
            {
                lock (padlock)
                {
                    instance ??= new Bot();
                    return instance;
                }
            }
        }
        #endregion

        public DiscordClient Client { get; private set; }

        public InteractivityExtension Interactivity { get; private set; }

        public CommandsNextExtension Commands { get; private set; }

        public SlashCommandsExtension SlashCommands { get; set; }

        public HeartbeatData HeartbeatData { get; set; }

        public async Task RunAsync()
        {
            ReadConfig.Read();
            var varrepo = new VariableRepository(ReadConfig.Config.ConnectionString);
            bool result;
#if DEBUG
            result = varrepo.Read(3, out Variable prefix); //get testPrefix if in designmode
#else
			result = varrepo.Read(2, out Variable prefix); //otherwise get default prefix
#endif
            DiscordConfiguration config = new()
            {
                Token = ReadConfig.Config.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
#if DEBUG
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
#endif
                Intents = DiscordIntents.All
            };

            Client = new DiscordClient(config);

            //Register client events
            Client.SessionCreated += Events.OnClientReady;
            Client.GuildDownloadCompleted += Events.Client_GuildDownloadCompleted;
            Client.MessageCreated += Events.MessageCreated;
            Client.MessageCreated += Prevention.OnMessageCreated;
            Client.MessageUpdated += Events.MessageUpdated;
            Client.MessageDeleted += Events.MessageDeleted;
            Client.GuildMemberAdded += Events.MemberAdded;
            Client.MessageReactionAdded += Events.ReactionAdded;
            Client.ClientErrored += Events.ClientErrored;
            Client.ComponentInteractionCreated += Events.ComponentTriggered;

            //register handlers in event handler folder
            Client.ComponentInteractionCreated += RoleAssign.ComponentTriggered;

#if !DEBUG
            //Register client events for logging
            Client.GuildBanAdded += Logger.BanAdded;
            Client.GuildBanRemoved += Logger.BanRemoved;
            Client.GuildMemberUpdated += Logger.MemberUpdated;
            Client.ChannelUpdated += Logger.ChannelUpdated;
            Client.GuildRoleUpdated += Logger.RoleUpdated;
            Client.MessageUpdated += Logger.MessageEdited;
            Client.MessageDeleted += Logger.MessageDeleted;
            Client.MessagesBulkDeleted += Logger.BulkMessagesDeleted;
            Client.VoiceStateUpdated += Logger.VoiceUpdate;
            Client.ThreadCreated += Logger.ThreadCreated;
            Client.ThreadDeleted += Logger.ThreadDeleted;
            //Library broken as fuck here, so not yet enabled
            //Client.ThreadUpdated += Logger.ThreadUpdated;  
#endif


            //Register timer events
            SystemService.Instance.WarnTimer.Elapsed += Events.TimerTick;

            //Register Logger events
            SystemService.Instance.Logger.RaiseLogEvent += Events.OnLog;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(5),
                PollBehaviour = PollBehaviour.KeepEmojis,
            });

            CommandsNextConfiguration commandsConfig = new()
            {
                StringPrefixes = new string[] { prefix.Value },
                EnableMentionPrefix = true
            };
            Commands = Client.UseCommandsNext(commandsConfig);

            //Register commands
            Commands.RegisterCommands<AuditCommands>();
            Commands.RegisterCommands<EmbedCommands>();
            Commands.RegisterCommands<InfoCommands>();
            Commands.RegisterCommands<ReactionCommands>();
            Commands.RegisterCommands<RuleCommands>();
            Commands.RegisterCommands<TechnicalCommands>();
            Commands.RegisterCommands<WarnCommands>();
            //Commands.RegisterCommands<EventCommands>();

            //Register command events
            Commands.CommandErrored += Events.CommandErrored;

            SlashCommands = Client.UseSlashCommands();

            //Register interactions
            SlashCommands.RegisterCommands<WarnInteractions>(699555747591094344);
            SlashCommands.RegisterCommands<ModerationInteractions>(699555747591094344);
            SlashCommands.RegisterCommands<DebateInteractions>(699555747591094344);

            //Register interaction events
            SlashCommands.ContextMenuErrored += Events.ContextMenuErrored;
            SlashCommands.AutocompleteErrored += Events.AutoCompleteErrored;
            SlashCommands.SlashCommandErrored += Events.SlashCommandErrored;

            await Client.ConnectAsync();

            HeartbeatManager heartbeatManager = new();
            HeartbeatData = new("", "");
            await heartbeatManager.StartHeartbeatsAsync(ReadConfig.Config.UptimeKumaUrl, HeartbeatData);

            await Task.Delay(-1);
        }
    }
}
