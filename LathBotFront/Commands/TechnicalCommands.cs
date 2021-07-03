using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;

using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Services;
using LathBotBack.Commands.TimeZoneConverter;
using DSharpPlus.EventArgs;

namespace LathBotFront.Commands
{
	class TechnicalCommands : BaseCommandModule
	{
		[Command("ping")]
		[Description("Pong")]
		public async Task Ping(CommandContext ctx)
		{
			await ctx.Channel.SendMessageAsync("My ping is on bloody " + ctx.Client.Ping + "ms");
		}

		[Command("test")]
		[RequireRoles(RoleCheckMode.Any, "Bot Management")]
		public async Task Test(CommandContext ctx)
		{
			DiscordColor color = new DiscordColor("#6518c9");//ctx.Guild.GetMemberAsync(192037157416730625).Result.Color;
			DiscordMessageBuilder builder = new DiscordMessageBuilder
			{
				Embed = new DiscordEmbedBuilder
				{
					Title = "Verification",
					Description = "When you are done reading <#699559050106372116> and <#726553696389038190>, click the button beneath this message to access the other channels.",
					Color = color
				}
			};
			List<DiscordComponent> components = new List<DiscordComponent>();
			var comp = new DiscordButtonComponent(ButtonStyle.Success, "lb_server_verification", "Verify", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✔️")));
			components.Add(comp);
			builder.AddComponents(components);
			await ctx.Channel.SendMessageAsync(builder);
		}

		[Command("freeze")]
		[Description("Freeze a whole channel when you get too many people spamming to control by other measures")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task ChFreeze(CommandContext ctx)
		{
			IReadOnlyList<DiscordOverwrite> perms = ctx.Channel.PermissionOverwrites;
			foreach (DiscordOverwrite perm in perms)
			{
				if (perm.Id == 767050052257447936)
				{
					await perm.UpdateAsync(Permissions.AccessChannels, Permissions.SendMessages);
					break;
				}
			}
			await ctx.Channel.SendMessageAsync("```This channel is now frozen for moderation purposes.\n" +
				"You will be able to send messages again once the situation has been resolved```").ConfigureAwait(false);
		}

		[Command("unfreeze")]
		[Description("Unfreeze a channel after it was previously frozen")]
		[RequireUserPermissions(Permissions.Administrator)]
		public async Task ChUnFreeze(CommandContext ctx)
		{
			IReadOnlyList<DiscordOverwrite> perms = ctx.Channel.PermissionOverwrites;
			foreach (DiscordOverwrite perm in perms)
			{
				if (perm.Id == 767050052257447936)
				{
					await perm.UpdateAsync(Permissions.AccessChannels | Permissions.SendMessages, Permissions.None);
					break;
				}
			}
			await ctx.Channel.SendMessageAsync("```This channel is no longer frozen.\n" +
				"You can now send messages as normal```").ConfigureAwait(false);
		}

		[Command("convert")]
		[Aliases("time")]
		[Description("Convert from one timezone to the others")]
		public async Task TimeConvert(CommandContext ctx, [Description("The time that you want to get converted(24 hours model). Format: hh:mm")] string time, [Description("The timezone that you want to convert from")] string timeZone)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			if (timeZone.ToUpper() == "UTC" || timeZone.ToUpper() == "GMT" || timeZone.ToUpper() == "WEZ" || timeZone.ToUpper() == "GMT" || timeZone.ToUpper() == "AZODT" || timeZone.ToUpper() == "IST" || timeZone.ToUpper() == "EGST" || timeZone.ToUpper() == "SLT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTC(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+1" || timeZone.ToUpper() == "GMT+1" || timeZone.ToUpper() == "MEZ" || timeZone.ToUpper() == "CET" || timeZone.ToUpper() == "WAT" || timeZone.ToUpper() == "WEST" || timeZone.ToUpper() == "WESZ" || timeZone.ToUpper() == "BST" || timeZone.ToUpper() == "IST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusOne(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+2" || timeZone.ToUpper() == "GMT+2" || timeZone.ToUpper() == "EET" || timeZone.ToUpper() == "OEZ" || timeZone.ToUpper() == "CEST" || timeZone.ToUpper() == "CEDT" || timeZone.ToUpper() == "MESZ" || timeZone.ToUpper() == "CAT" || timeZone.ToUpper() == "SAST" || timeZone.ToUpper() == "USZ1" || timeZone.ToUpper() == "WAST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusTwo(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+3" || timeZone.ToUpper() == "GMT+3" || timeZone.ToUpper() == "AST" || timeZone.ToUpper() == "EAT" || timeZone.ToUpper() == "EEST" || timeZone.ToUpper() == "IDT" || timeZone.ToUpper() == "MSK" || timeZone.ToUpper() == "SYST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusThree(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+4" || timeZone.ToUpper() == "GMT+4" || timeZone.ToUpper() == "AMST" || timeZone.ToUpper() == "AZT" || timeZone.ToUpper() == "GET" || timeZone.ToUpper() == "GST" || timeZone.ToUpper() == "ICT" || timeZone.ToUpper() == "MUT" || timeZone.ToUpper() == "RET" || timeZone.ToUpper() == "SCT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusFour(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+5" || timeZone.ToUpper() == "GMT+5" || timeZone.ToUpper() == "CAST" || timeZone.ToUpper() == "TFT" || timeZone.ToUpper() == "HMT" || timeZone.ToUpper() == "MVT" || timeZone.ToUpper() == "PKT" || timeZone.ToUpper() == "TJT" || timeZone.ToUpper() == "TMT" || timeZone.ToUpper() == "UZT" || timeZone.ToUpper() == "WKST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusFive(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+6" || timeZone.ToUpper() == "GMT+6" || timeZone.ToUpper() == "BDT" || timeZone.ToUpper() == "BTT" || timeZone.ToUpper() == "EKST" || timeZone.ToUpper() == "BIOT" || timeZone.ToUpper() == "MAWT" || timeZone.ToUpper() == "OMST" || timeZone.ToUpper() == "VOST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusSix(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+7" || timeZone.ToUpper() == "GMT+7" || timeZone.ToUpper() == "KRAT" || timeZone.ToUpper() == "NOVT" || timeZone.ToUpper() == "ICT" || timeZone.ToUpper() == "WIB" || timeZone.ToUpper() == "DAVT" || timeZone.ToUpper() == "KOVT" || timeZone.ToUpper() == "CXT" || timeZone.ToUpper() == "BST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusSeven(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+8" || timeZone.ToUpper() == "GMT+8" || timeZone.ToUpper() == "ACIT" || timeZone.ToUpper() == "AWST" || timeZone.ToUpper() == "BDT" || timeZone.ToUpper() == "CST" || timeZone.ToUpper() == "HKST" || timeZone.ToUpper() == "IRKT" || timeZone.ToUpper() == "MBT" || timeZone.ToUpper() == "MYT" || timeZone.ToUpper() == "MNT" || timeZone.ToUpper() == "PIT" || timeZone.ToUpper() == "PHT" || timeZone.ToUpper() == "SST" || timeZone.ToUpper() == "SGT" || timeZone.ToUpper() == "SIT" || timeZone.ToUpper() == "TWT" || timeZone.ToUpper() == "WITA" || timeZone.ToUpper() == "KOVST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusEight(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+9" || timeZone.ToUpper() == "GMT+9" || timeZone.ToUpper() == "WIT" || timeZone.ToUpper() == "JST" || timeZone.ToUpper() == "KST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusNine(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+10" || timeZone.ToUpper() == "GMT+10" || timeZone.ToUpper() == "DTAT" || timeZone.ToUpper() == "AEST" || timeZone.ToUpper() == "TRUT" || timeZone.ToUpper() == "PGT" || timeZone.ToUpper() == "VLAT" || timeZone.ToUpper() == "CHST" || timeZone.ToUpper() == "YAPT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusTen(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+11" || timeZone.ToUpper() == "GMT+11" || timeZone.ToUpper() == "KOST" || timeZone.ToUpper() == "SRET" || timeZone.ToUpper() == "NCT" || timeZone.ToUpper() == "PONT" || timeZone.ToUpper() == "SBT" || timeZone.ToUpper() == "VUT" || timeZone.ToUpper() == "NFT" || timeZone.ToUpper() == "AEDT" || timeZone.ToUpper() == "LHDT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusEleven(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC+12" || timeZone.ToUpper() == "GMT+12" || timeZone.ToUpper() == "WFT" || timeZone.ToUpper() == "TVT" || timeZone.ToUpper() == "FJT" || timeZone.ToUpper() == "GILT" || timeZone.ToUpper() == "NRT" || timeZone.ToUpper() == "MHT" || timeZone.ToUpper() == "PETT" || timeZone.ToUpper() == "NZST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCPlusTwelve(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-12" || timeZone.ToUpper() == "GMT-12" || timeZone.ToUpper() == "IDLW" || timeZone.ToUpper() == "BIT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusTwelve(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-11" || timeZone.ToUpper() == "GMT-11" || timeZone.ToUpper() == "WST" || timeZone.ToUpper() == "WSST" || timeZone.ToUpper() == "NUT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusEleven(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-10" || timeZone.ToUpper() == "GMT-10" || timeZone.ToUpper() == "HAST" || timeZone.ToUpper() == "TAHT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusTen(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-9" || timeZone.ToUpper() == "GMT-9" || timeZone.ToUpper() == "HADT" || timeZone.ToUpper() == "GAMT" || timeZone.ToUpper() == "AKST" || timeZone.ToUpper() == "YST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusNine(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-8" || timeZone.ToUpper() == "GMT-8" || timeZone.ToUpper() == "PST" || timeZone.ToUpper() == "CIST" || timeZone.ToUpper() == "AKDT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusEight(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-7" || timeZone.ToUpper() == "GMT-7" || timeZone.ToUpper() == "MST" || timeZone.ToUpper() == "PDT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusSeven(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-6" || timeZone.ToUpper() == "GMT-6" || timeZone.ToUpper() == "CST" || timeZone.ToUpper() == "GALT" || timeZone.ToUpper() == "PIT" || timeZone.ToUpper() == "MDT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusSix(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-5" || timeZone.ToUpper() == "GMT-5" || timeZone.ToUpper() == "EAST" || timeZone.ToUpper() == "EST" || timeZone.ToUpper() == "ECT" || timeZone.ToUpper() == "ACT" || timeZone.ToUpper() == "COT" || timeZone.ToUpper() == "PET" || timeZone.ToUpper() == "CDT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusFive(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-4" || timeZone.ToUpper() == "GMT-4" || timeZone.ToUpper() == "BOT" || timeZone.ToUpper() == "FKST" || timeZone.ToUpper() == "AST" || timeZone.ToUpper() == "PYT" || timeZone.ToUpper() == "BWST" || timeZone.ToUpper() == "SLT" || timeZone.ToUpper() == "GYT" || timeZone.ToUpper() == "JFST" || timeZone.ToUpper() == "EDT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusFour(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-3" || timeZone.ToUpper() == "GMT-3" || timeZone.ToUpper() == "ART" || timeZone.ToUpper() == "BRT" || timeZone.ToUpper() == "CLST" || timeZone.ToUpper() == "WGT" || timeZone.ToUpper() == "GFT" || timeZone.ToUpper() == "PMST" || timeZone.ToUpper() == "ROTT" || timeZone.ToUpper() == "SRT" || timeZone.ToUpper() == "UYT" || timeZone.ToUpper() == "ADT" || timeZone.ToUpper() == "BWDT" || timeZone.ToUpper() == "FKDT" || timeZone.ToUpper() == "JFDT" || timeZone.ToUpper() == "PYDT" || timeZone.ToUpper() == "SLST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusThree(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-2" || timeZone.ToUpper() == "GMT-2" || timeZone.ToUpper() == "GST" || timeZone.ToUpper() == "BEST" || timeZone.ToUpper() == "PMDT" || timeZone.ToUpper() == "CGST" || timeZone.ToUpper() == "UYST" || timeZone.ToUpper() == "ARDT" || timeZone.ToUpper() == "BRST")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusTwo(time)).ConfigureAwait(false);
			else if (timeZone.ToUpper() == "UTC-1" || timeZone.ToUpper() == "GMT-1" || timeZone.ToUpper() == "AZOST" || timeZone.ToUpper() == "CVT")
				await ctx.Channel.SendMessageAsync(TimeZones.UTCMinusOne(time)).ConfigureAwait(false);
		}

		[Command("prune")]
		[Description("Kick all members without verified role")]
		[RequireUserPermissions(Permissions.Administrator)]
		public async Task Prune(CommandContext ctx)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			IReadOnlyCollection<DiscordMember> members = await ctx.Guild.GetAllMembersAsync();
			foreach (DiscordMember member in members)
			{
				bool kick = true;
				foreach (DiscordRole role in member.Roles)
				{
					if (role.Id == 767050052257447936 || role.Id == 699994970412679310)
					{
						kick = false;
						break;
					}
				}
				if (kick)
					await member.RemoveAsync().ConfigureAwait(false);
			}
			await ctx.Channel.SendMessageAsync("Done :(").ConfigureAwait(false);
		}

		[Command("advert")]
		[Description("Send a request for a post to #〘📣〙advertisements")]
		public async Task Advert(CommandContext ctx, [Description("The advertiser URL")] string url, [Description("A short description why one should check this out")][RemainingText] string adText)
		{
			DiscordEmbedBuilder adEmbed = new DiscordEmbedBuilder
			{
				Title = "Advert",
				Url = url,
				Description = adText + "\nClick on the title to go to the page!",
				Color = ctx.Member.Color,
				Footer = new DiscordEmbedBuilder.EmbedFooter
				{
					Text = ctx.Member.Username + "#" + ctx.User.Discriminator + " " + ctx.Member.Id,
					IconUrl = ctx.Member.AvatarUrl
				}
			};

			DiscordRole plagueRole = ctx.Guild.GetRole(796234634316873759);
			DiscordChannel plagueChannel = ctx.Guild.GetChannel(701499919248392222);
			DiscordMessageBuilder builder = new DiscordMessageBuilder
			{
				Content = $"Allow or deny this ad {plagueRole.Mention}",
				Embed = adEmbed.Build()
			};
			builder.WithAllowedMention(RoleMention.All);
			DiscordMessage allowDenyMessage = await plagueChannel.SendMessageAsync(builder).ConfigureAwait(false);

			await allowDenyMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":white_check_mark:")).ConfigureAwait(false);
			await allowDenyMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:")).ConfigureAwait(false);

			InteractivityExtension interactivity = ctx.Client.GetInteractivity();

			var reaction = await interactivity.WaitForReactionAsync(x =>
			{
				if (x.Message != allowDenyMessage)
					return false;
				var roles = ctx.Guild.GetMemberAsync(x.User.Id).Result.Roles;
				foreach (DiscordRole role in roles)
					if (role.Id == 796234634316873759 || role.Id == 784852719449276467 || role.Id == 726212852267876435)
						return true;
				return false;
			}).ConfigureAwait(false);

			if (reaction.Result.Emoji.ToString() == "❌")
			{
				await ctx.Channel.SendMessageAsync("Request has been denied!").ConfigureAwait(false);
				await ctx.Message.DeleteAsync().ConfigureAwait(false);
			}
			else if (reaction.Result.Emoji.ToString() == "✅")
			{
				await ctx.Guild.GetChannel(787066481204527154).SendMessageAsync(ctx.Guild.GetRole(794975835235942470).Mention, adEmbed.Build()).ConfigureAwait(false);
				await ctx.Channel.SendMessageAsync("Request has been approved!").ConfigureAwait(false);
			}
			else if (reaction.TimedOut)
			{
				await ctx.Channel.SendMessageAsync("It seems like there is no moderator online right now.\nTry again later.").ConfigureAwait(false);
			}
		}

		[Command("purgecell")]
		[Description("Purge parstapo-cell and log it to a file")]
		[RequireUserPermissions(Permissions.Administrator)]
		public async Task PurgeCell(CommandContext ctx)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			if (ctx.Channel.Id == 792486366138073180)
			{
				string channelContent = "";
				IReadOnlyList<DiscordMessage> messages = await ctx.Channel.GetMessagesAsync(300);
				for (int index = messages.Count - 1; index >= 0; index--)
				{
					channelContent += messages[index].Timestamp +
						" - " + $"{messages[index].Author.Username}#{messages[index].Author.Discriminator} ({messages[index].Author.Id})" + ": " +
						messages[index].Content +
						(messages[index].IsEdited ? $" (edited at {messages[index].EditedTimestamp})\n" : "\n");
				}
				File.WriteAllText("CellLog.txt", channelContent);
				await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(300)).ConfigureAwait(false);
				DiscordMessageBuilder builder = new DiscordMessageBuilder();
				FileStream stream = new FileStream("CellLog.txt", FileMode.Open);
				builder.WithFile(stream);
				await ctx.Guild.GetChannel(722905404354592900).SendMessageAsync(builder);
			}
		}

		[Command("boardcount")]
		[Description("Change how many reactions it takes to get on the good guys board")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task BoardCount(CommandContext ctx, [Description("New limit")] int newCount)
		{
			GoodGuysService.Instance.GoodGuysReactionCount = newCount;

			VariableRepository repo = new VariableRepository(ReadConfig.configJson.ConnectionString);

			Variable entity = new Variable { ID = 1, Name = "Goodguys", Value = newCount.ToString() };

			bool result = repo.Update(entity);
			if (!result)
			{
				await ctx.RespondAsync("Error updating the database entry");
				return;
			}

			await ctx.Channel.SendMessageAsync("Set to " + newCount).ConfigureAwait(false);
		}

		[Command("getcount")]
		[Description("see how much it currently takes to get on the GoodGuys board")]
		public async Task GetCount(CommandContext ctx)
		{
			await ctx.RespondAsync(GoodGuysService.Instance.GoodGuysReactionCount.ToString());
		}

		[Command("clean")]
		[Description("Purge muted and log it to a file")]
		[RequireUserPermissions(Permissions.BanMembers)]
		public async Task Clean(CommandContext ctx)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			if (ctx.Channel.Id == 838088490704568341)
			{
				string channelContent = "";
				IReadOnlyList<DiscordMessage> messages = await ctx.Channel.GetMessagesAsync(500);
				for (int index = messages.Count - 1; index >= 0; index--)
				{
					channelContent += messages[index].Timestamp +
						" - " + $"{messages[index].Author.Username}#{messages[index].Author.Discriminator} ({messages[index].Author.Id})" + ": " +
						messages[index].Content +
						(messages[index].IsEdited ? $" (edited at {messages[index].EditedTimestamp})\n" : "\n");
				}
				File.WriteAllText("MuteLog.txt", channelContent);
				await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync(500)).ConfigureAwait(false);
				DiscordMessageBuilder builder = new DiscordMessageBuilder();
				FileStream stream = new FileStream("MuteLog.txt", FileMode.Open);
				builder.WithFile(stream);
				await ctx.Guild.GetChannel(838092779741642802).SendMessageAsync(builder);
				DiscordMessage pin = await ctx.Channel.SendMessageAsync("-Rules-\n" +
					"This channel is not for your own playground, do not use it for recreational purposes, it is only used for the plague guards, or senate to discuss your warn with you,\n" +
					"-\n" +
					"If there are or more than two of you muted don't have a conversation in the channel. if a plague guard or senate member starts a conversation with you then you may have a conversation.");
				await pin.PinAsync();
			}
		}
	}
}
