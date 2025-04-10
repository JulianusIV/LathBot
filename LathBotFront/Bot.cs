using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.TextCommands;
using DSharpPlus.Commands.Processors.TextCommands.Parsing;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using LathBotFront.EventHandlers;
using LathBotFront.Interactions.PreExecutionChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UptimeKumaHeartbeat;

namespace LathBotFront
{
    public class Bot
    {
        #region Singleton
        private static Bot instance = null;
        private static readonly Lock padlock = new();
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

            this.Client = DiscordClientBuilder.CreateDefault(ReadConfig.Config.Token, DiscordIntents.All)
#if DEBUG
                .SetLogLevel(LogLevel.Debug)
#endif
                .ConfigureEventHandlers(b =>
                    b.HandleSessionCreated(Events.OnClientReady)
                    .HandleGuildDownloadCompleted(Events.Client_GuildDownloadCompleted)
                    .HandleMessageCreated(Events.MessageCreated)
                    .HandleMessageCreated(Prevention.OnMessageCreated)
                    .HandleMessageUpdated(Events.MessageUpdated)
                    .HandleMessageDeleted(Events.MessageDeleted)
                    .HandleGuildMemberAdded(Events.MemberAdded)
                    .HandleMessageReactionAdded(Events.ReactionAdded)
                    .HandleComponentInteractionCreated(Events.ComponentTriggered)
                    .HandleComponentInteractionCreated(RoleAssign.ComponentTriggered)
#if !DEBUG
                    .HandleGuildBanAdded(Logger.BanAdded)
                    .HandleGuildBanRemoved(Logger.BanRemoved)
                    .HandleGuildMemberUpdated(Logger.MemberUpdated)
                    .HandleChannelUpdated(Logger.ChannelUpdated)
                    .HandleGuildRoleUpdated(Logger.RoleUpdated)
                    .HandleMessageUpdated(Logger.MessageEdited)
                    .HandleMessageDeleted(Logger.MessageDeleted)
                    .HandleMessagesBulkDeleted(Logger.BulkMessagesDeleted)
                    .HandleVoiceStateUpdated(Logger.VoiceUpdate)
                    .HandleThreadCreated(Logger.ThreadCreated)
                    .HandleThreadDeleted(Logger.ThreadDeleted)
                    .HandleThreadUpdated(Logger.ThreadUpdated)
#endif
                )
                .UseInteractivity(new InteractivityConfiguration
                {
                    Timeout = TimeSpan.FromMinutes(5),
                    PollBehaviour = PollBehaviour.KeepEmojis
                })
                .UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) =>
                {
                    extension.AddCommands(Assembly.GetExecutingAssembly());
                    TextCommandProcessor textCommandProcessor = new(new()
                    {
                        PrefixResolver = new DefaultPrefixResolver(true, prefix.Value).ResolvePrefixAsync
                    });
                    extension.AddProcessor(textCommandProcessor);
                    extension.CommandErrored += Events.SlashCommandErrored;
                    extension.AddCheck<EmbedBannedCheck>();
                }, new CommandsConfiguration
                {
                    RegisterDefaultCommandProcessors = true,
                    UseDefaultCommandErrorHandler = false
                })
                .Build();

            //Register timer events
            SystemService.Instance.WarnTimer.Elapsed += Events.TimerTick;

            //Register Logger events
            SystemService.Instance.Logger.RaiseLogEvent += Events.OnLog;

            await this.Client.ConnectAsync();

            HeartbeatManager heartbeatManager = new();
            this.HeartbeatData = new("", "");
            await heartbeatManager.StartHeartbeatsAsync(ReadConfig.Config.UptimeKumaUrl, this.HeartbeatData);

            await Task.Delay(-1);
        }
    }
}
