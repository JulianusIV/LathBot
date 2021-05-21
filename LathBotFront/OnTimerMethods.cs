using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using LathBotBack;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront
{
	public class OnTimerMethods
	{
		public async static Task PardonWarns()
		{
			WarnRepository repo = new WarnRepository(ReadConfig.configJson.ConnectionString);
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool result = repo.GetAll(out List<Warn> list);
			if (!result)
			{
				_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error getting all warns in TimerTick.");
				return;
			}
			int pardoned = 0;
			foreach (var item in list)
			{
				if ((item.Level > 0 && item.Level < 6 && item.Time <= DateTime.Now - TimeSpan.FromDays(14) ||
					item.Level > 5 && item.Level < 11 && item.Time <= DateTime.Now - TimeSpan.FromDays(56)) &&
					!item.Persistent)
				{
					pardoned++;
					result = repo.Delete(item.ID);
					if (!result)
					{
						_ = Holder.Instance.ErrorLogChannel.SendMessageAsync($"Error deleting warn {item.ID} from {item.Time}, level {item.Level}.");
					}
					result = repo.GetAllByUser(item.User, out List<Warn> others);
					if (!result)
					{
						_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error reading other warns from the database.");
					}

					int counter = 0;
					foreach (var warn in others)
					{
						counter++;
						warn.Number = counter;
						result = repo.Update(warn);
						if (!result)
						{
							_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error updating the database.");
							break;
						}
					}

					result = urepo.Read(item.User, out User entity);
					if (!result)
					{
						_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error reading a user from the database");
						continue;
					}
					result = urepo.Read(item.Mod, out User modEntity);
					if (!result)
					{
						_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error reading a user from the database");
						continue;
					}
					try
					{
						DiscordMember member = await Holder.Instance.Lathland.GetMemberAsync(entity.DcID);
						DiscordMember moderator = await Holder.Instance.Lathland.GetMemberAsync(modEntity.DcID);
						DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
						{
							Color = DiscordColor.Green,
							Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
							Title = $"Pardoned warn number {item.Number} of {member.DisplayName}#{member.Discriminator} ({member.Id})",
							Description = $"Warn from {item.Time}.",
							Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = moderator.AvatarUrl, Text = moderator.DisplayName }
						};
						DiscordEmbed embed = embedBuilder.Build();

						_ = Holder.Instance.WarnsChannel.SendMessageAsync(embed).ConfigureAwait(false);
					}
					catch
					{
						DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
						{
							Color = DiscordColor.Green,
							Title = $"Pardoned warn of {entity.DcID}",
							Description = $"Warn from {item.Time}.",
							Footer = new DiscordEmbedBuilder.EmbedFooter { Text = modEntity.DcID.ToString() }
						};
						DiscordEmbed embed = embedBuilder.Build();

						_ = Holder.Instance.WarnsChannel.SendMessageAsync(embed).ConfigureAwait(false);
					}
				}
			}
			if (pardoned >= 0)
			{
				_ = Holder.Instance.TimerChannel.SendMessageAsync($"Timer ticked, {pardoned} warns pardoned.");
			}
			else
			{
				_ = Holder.Instance.TimerChannel.SendMessageAsync("Timer ticked, no warns pardoned.");
			}
		}

		public async static Task RemindMutes()
		{
			MuteRepository repo = new MuteRepository(ReadConfig.configJson.ConnectionString);
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool result = repo.GetAll(out List<Mute> list);
			if (!result)
			{
				_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error getting all mutes in TimerTick.");
				return;
			}
			foreach (var item in list)
			{
				if (item.Timestamp + TimeSpan.FromDays(item.Duration) >= DateTime.Now)
				{
					result = urepo.Read(item.Mod, out User dbMod);
					if (!result)
					{
						_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error getting a mod from the database.");
						continue;
					}
					DiscordMember mod = await Holder.Instance.Lathland.GetMemberAsync(dbMod.DcID);
					result = urepo.Read(item.Mod, out User dbUser);
					if (!result)
					{
						_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error getting a mod from the database.");
						continue;
					}
					DiscordMember user = await Holder.Instance.Lathland.GetMemberAsync(dbMod.DcID);
					if (user.Roles.Contains(Holder.Instance.Lathland.GetRole(701446136208293969)))
					{
						await mod.SendMessageAsync($"The user {user.DisplayName}#{user.Discriminator} ({user.Id}) you muted at {item.Timestamp:yyyy-MM-dd hh:mm} for {item.Duration}, is now muted for {DateTime.Now - item.Timestamp} days." +
							$"You will be reminded again tomorrow.");
					}
					else
					{
						result = repo.Delete(item.Id);
						if (!result)
						{
							_ = Holder.Instance.ErrorLogChannel.SendMessageAsync("Error deleting a Mute from database.");
							continue;
						}
					}
				}
			}
		}
	}
}