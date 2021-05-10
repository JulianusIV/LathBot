using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using LathBotBack;

namespace LathBotFront.Commands
{
	public class InfoCommands : BaseCommandModule
	{
		[Command("info")]
		[Description("Display some info about the bot")]
		public async Task Info(CommandContext ctx)
		{
			DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
			{
				Color = new DiscordColor(101, 24, 201),
				Title = "LathBot#1753",
				Description = $"LathBot is a custom bot for the server Lathland, prefix is - or {ctx.Client.CurrentUser.Mention}\n" +
					"For more info use -help, -tos, or -privacy",
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Client.CurrentUser.AvatarUrl }
			};
			discordEmbed.AddField("Language", "C# using Visual Studio 2019");
			discordEmbed.AddField("Library", "DSharpPlus, Version 4.0.0-nightly-00801");
			TimeSpan uptime = DateTime.Now - Holder.Instance.StartTime;
			discordEmbed.AddField("Uptime", $"Bot has been running since {uptime}");
			await ctx.Channel.SendMessageAsync(discordEmbed.Build()).ConfigureAwait(false);
		}

		[Command("tos")]
		[Description("The bot's TOS")]
		public async Task Tos(CommandContext ctx)
		{
			string part = "";
			int index = 0;
			string result;
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Resources.LathBotTOS.txt"))
			using (StreamReader reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}
			foreach (char character in result)
			{
				index++;
				if (part.Length < 2000)
				{
					if (index < result.Length)
						part += character;
					else
						await ctx.Channel.SendMessageAsync(part + character).ConfigureAwait(false);
				}
				else
				{
					await ctx.Channel.SendMessageAsync(part).ConfigureAwait(false);
					part = character.ToString();
				}
			}

		}

		[Command("privacy")]
		[Description("The bot's privacy policy")]
		public async Task Privacy(CommandContext ctx)
		{
			string part = "";
			int index = 0;
			string result;
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Resources.LathBotPP.txt"))
			using (StreamReader reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}
			foreach (char character in result)
			{
				index++;
				if (part.Length < 2000)
				{
					if (index < result.Length)
						part += character;
					else
						await ctx.Channel.SendMessageAsync(part + character).ConfigureAwait(false);
				}
				else
				{
					await ctx.Channel.SendMessageAsync(part).ConfigureAwait(false);
					part = character.ToString();
				}
			}
		}

		[Command("stafftime")]
		[Description("Display timezones and current time of staff members")]
		public async Task StaffTimes(CommandContext ctx)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
			{
				Color = new DiscordColor(64, 255, 0),
				Title = "Staff timezones",
				Description = "Keep in mind that it being daytime in the users timezone does not obligate them to being available!"
			};
			DateTime thisTime = DateTime.Now;

			#region fields
			try
			{
				DiscordMember chewy = await ctx.Guild.GetMemberAsync(613366102306717712).ConfigureAwait(false);
				TimeZoneInfo chewyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
				DateTime chewyTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, chewyTimeZone);
				discordEmbed.AddField($"{chewy.Username}#{chewy.Discriminator}", chewyTime.ToString("yyyy-mm-dd     **HH:mm**") + "     (" + (chewyTimeZone.IsDaylightSavingTime(chewyTime) ?
					chewyTimeZone.DaylightName.Substring(0, 6) : chewyTimeZone.StandardName.Substring(0, 6)) + ")");
			}
			catch
			{
			}

			try
			{
				DiscordMember julian = await ctx.Guild.GetMemberAsync(387325006176059394).ConfigureAwait(false);
				TimeZoneInfo julianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
				DateTime julianTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, julianTimeZone);
				discordEmbed.AddField($"{julian.Username}#{julian.Discriminator}", julianTime.ToString("yyyy-mm-dd     **HH:mm**") + "     (" + (julianTimeZone.IsDaylightSavingTime(julianTime) ?
					julianTimeZone.DaylightName.Substring(0, 6) : julianTimeZone.StandardName.Substring(0, 6)) + ")");
			}
			catch
			{
			}

			try
			{
				DiscordMember parth = await ctx.Guild.GetMemberAsync(289112287250350080).ConfigureAwait(false);
				TimeZoneInfo parthTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
				DateTime parthTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, parthTimeZone);
				discordEmbed.AddField($"{parth.Username}#{parth.Discriminator}", parthTime.ToString("yyyy-mm-dd     **HH:mm**") + "     (" + (parthTimeZone.IsDaylightSavingTime(parthTime) ?
					parthTimeZone.DaylightName.Substring(0, 6) : parthTimeZone.StandardName.Substring(0, 6)) + ")");
			}
			catch
			{
			}

			try
			{
				DiscordMember ava = await ctx.Guild.GetMemberAsync(700373370491109489).ConfigureAwait(false);
				TimeZoneInfo avaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
				DateTime avaTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, avaTimeZone);
				discordEmbed.AddField($"{ava.Username}#{ava.Discriminator}", avaTime.ToString("yyyy-mm-dd     **HH:mm**") + "     (" + (avaTimeZone.IsDaylightSavingTime(avaTime) ?
					avaTimeZone.DaylightName.Substring(0, 6) : avaTimeZone.StandardName.Substring(0, 6)) + ")");
			}
			catch
			{
			}

			try
			{
				DiscordMember dodo = await ctx.Guild.GetMemberAsync(228763974299156482).ConfigureAwait(false);
				TimeZoneInfo dodoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");
				DateTime dodoTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, dodoTimeZone);
				discordEmbed.AddField($"{dodo.Username}#{dodo.Discriminator}", dodoTime.ToString("yyyy-mm-dd     **HH:mm**") + "     (" + (dodoTimeZone.IsDaylightSavingTime(dodoTime) ?
					dodoTimeZone.DaylightName.Substring(0, 6) : dodoTimeZone.StandardName.Substring(0, 6)) + ")");
			}
			catch
			{
			}

			try
			{
				DiscordMember theo = await ctx.Guild.GetMemberAsync(241445303960600576).ConfigureAwait(false);
				TimeZoneInfo theoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Africa/Johannesburg");
				DateTime theoTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, theoTimeZone);
				discordEmbed.AddField($"{theo.Username}#{theo.Discriminator}", theoTime.ToString("yyyy-mm-dd     **HH:mm**") + "     (" + (theoTimeZone.IsDaylightSavingTime(theoTime) ?
					theoTimeZone.DaylightName.Substring(0, 6) : theoTimeZone.StandardName.Substring(0, 6)) + ")");
			}
			catch
			{
			}

			try
			{
				DiscordMember emr = await ctx.Guild.GetMemberAsync(619694655805718568).ConfigureAwait(false);
				TimeZoneInfo emrTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
				DateTime emrTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, emrTimeZone);
				discordEmbed.AddField($"{emr.Username}#{emr.Discriminator}", emrTime.ToString("yyyy-mm-dd     **HH:mm**") + "     (" + (emrTimeZone.IsDaylightSavingTime(emrTime) ?
					emrTimeZone.DaylightName.Substring(0, 6) : emrTimeZone.StandardName.Substring(0, 6)) + ")");
			}
			catch
			{
			}

			try
			{
				DiscordMember generic = await ctx.Guild.GetMemberAsync(619694655805718568).ConfigureAwait(false);
				TimeZoneInfo genericTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
				DateTime genericTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, genericTimeZone);
				discordEmbed.AddField($"{generic.Username}#{generic.Discriminator}", genericTime.ToString("yyyy-mm-dd     **HH:mm**") + "     (" + (genericTimeZone.IsDaylightSavingTime(genericTime) ?
					genericTimeZone.DaylightName.Substring(0, 6) : genericTimeZone.StandardName.Substring(0, 6)) + ")");
			}
			catch
			{
			}
			#endregion

			await ctx.Channel.SendMessageAsync(discordEmbed).ConfigureAwait(false);
		}
	}
}
