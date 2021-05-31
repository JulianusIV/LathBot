using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;

using LathBotBack.Repos;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Services;
using DSharpPlus;

namespace LathBotFront.Commands
{
	public class WarnCommands : BaseCommandModule
	{
		[Command("initdb")]
		[Description("Fill empty Database with users")]
		[RequireRoles(RoleCheckMode.Any, "Bot Management")]
		public async Task InitDB(CommandContext ctx)
		{
			await ctx.Channel.TriggerTypingAsync();
			if (ctx.User.Id != 387325006176059394)
				return;
			IReadOnlyCollection<DiscordMember> members = await ctx.Guild.GetAllMembersAsync();
			foreach (DiscordMember member in members)
			{
				User user = new User
				{
					DcID = member.Id
				};
				UserRepository repo = new UserRepository(ReadConfig.configJson.ConnectionString);

				bool success = repo.Create(ref user);
				if (!success)
				{
					await ctx.RespondAsync($"Error creating user {member.Nickname}#{member.Discriminator} ({member.Id})!");
				}
			}
			await ctx.RespondAsync("Done! (maybe?)");
		}

		[Command("updatedb")]
		[Description("Fill remaining users in database")]
		[RequireRoles(RoleCheckMode.Any, "Bot Management")]
		public async Task UpdateDB(CommandContext ctx)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			if (ctx.User.Id != 387325006176059394)
				return;
			IReadOnlyCollection<DiscordMember> members = await ctx.Guild.GetAllMembersAsync();
			int count = 0;
			UserRepository repo = new UserRepository(ReadConfig.configJson.ConnectionString);
			foreach (DiscordMember member in members)
			{

				bool success = repo.ExistsDcId(member.Id, out bool exists);
				if (!success)
				{
					DiscordMember mem = await ctx.Guild.GetMemberAsync(member.Id);
					_ = ctx.RespondAsync($"Error checking user {member.Nickname}#{member.Discriminator} ({member.Id})!");
				}
				else if (!exists)
				{
					User user = new User
					{
						DcID = member.Id
					};
					bool result = repo.Create(ref user);
					if (!result)
					{
						DiscordMember mem = await ctx.Guild.GetMemberAsync(member.Id);
						_ = ctx.RespondAsync($"Error creating user {member.Nickname}#{member.Discriminator} ({member.Id})!");
					}
					else
					{
						DiscordMember mem = await ctx.Guild.GetMemberAsync(member.Id);
						_ = ctx.RespondAsync($"Added user {member.DisplayName}#{mem.Discriminator} ({mem.Id})");
						count++;
					}
				}
			}
			bool res = repo.CountAll(out int amount);
			if (!res)
			{
				await ctx.RespondAsync("Error counting entries in database");
				await ctx.RespondAsync($"Added {count} users, {members.Count} members");
			}
			else
			{
				await ctx.RespondAsync($"Added {count} users, {members.Count} members, {amount} entries");
			}
		}

		[Command("mute")]
		[Aliases("shuddup")]
		[RequireUserPermissions(Permissions.KickMembers)]
		[Description("Mute a user")]
		public async Task Mute(CommandContext ctx, [Description("The user that you want to mute")] DiscordMember member, [Description("When you will be reminded (2 - 14 days, default 7)")] int duration = 7)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			if (duration > 14 || duration < 2)
			{
				await ctx.Channel.SendMessageAsync($"You cant mute someone for {(duration < 2 ? "shorter than 2 days." : "longer than 14 days.")}");
				return;
			}
			if (member.Id == 192037157416730625)
			{
				await ctx.Channel.SendMessageAsync("You cant mute Lathrix!");
				return;
			}
			else if (ctx.Member.Hierarchy <= member.Hierarchy)
			{
				await ctx.Channel.SendMessageAsync("You cant mute someone higher or same rank as you!");
				return;
			}
			else if (member.Roles.Contains(ctx.Guild.GetRole(701446136208293969)))
			{
				await ctx.Channel.SendMessageAsync("User is already muted.");
				return;
			}
			if (await AreYouSure(ctx, member, "mute"))
				return;

			if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
			{
				DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
				{
					Color = ctx.Member.Color,
					Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
					Description = $"[Link to usage]({ctx.Message.JumpLink})",
					Footer = new DiscordEmbedBuilder.EmbedFooter
					{
						IconUrl = ctx.Member.AvatarUrl,
						Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
					}
				};
				await ctx.Guild.GetChannel(722905404354592900).SendMessageAsync(discordEmbed.Build());
			}
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			MuteRepository mrepo = new MuteRepository(ReadConfig.configJson.ConnectionString);
			AuditRepository repo = new AuditRepository(ReadConfig.configJson.ConnectionString);
			bool userResult = urepo.GetIdByDcId(member.Id, out int id);
			if (!userResult)
			{
				await ctx.RespondAsync("There was a problem reading a User, user has not been muted.");
				return;
			}
			else
			{
				userResult = urepo.GetIdByDcId(ctx.Member.Id, out int modId);
				if (!userResult)
				{
					await ctx.RespondAsync("There was a problem reading a Mod, user has not been muted.");
					return;
				}

				bool result = mrepo.IsUserMuted(id, out bool exists);
				if (!result)
				{
					await ctx.RespondAsync("Error looking up an existing mute for this user, user will be muted anyways.");
				}
				if (exists)
				{
					result = mrepo.GetMuteByUser(id, out Mute mute);
					if (!result)
					{
						await ctx.RespondAsync("Error gettign an existing mute for this user, user will not be muted.");
						return;
					}
					mute.Mod = modId;
					mute.Duration = duration;
					mute.Timestamp = DateTime.Now;
					mute.LastCheck = DateTime.Now;
					result = mrepo.Update(mute);
					if (!result)
					{
						await ctx.RespondAsync("Error updating an existing mute entry for this user, user will not be muted.");
						return;
					}
				}
				else
				{
					Mute mute = new Mute
					{
						User = id,
						Mod = modId,
						Duration = duration,
						Timestamp = DateTime.Now,
						LastCheck = DateTime.Now
					};
					result = mrepo.Create(ref mute);
					if (!result)
					{
						await ctx.RespondAsync("There was a problem creating a mute entry, user has not been muted.");
						return;
					}
				}
				DiscordRole verificationRole = ctx.Guild.GetRole(767050052257447936);
				DiscordRole mutedRole = ctx.Guild.GetRole(701446136208293969);
				await member.RevokeRoleAsync(verificationRole);
				await member.GrantRoleAsync(mutedRole);
				bool auditResult = repo.Read(modId, out Audit audit);
				if (!auditResult)
				{
					await ctx.RespondAsync("There was a problem reading an Audit");
				}
				else
				{
					audit.Mutes++;
					bool updateResult = repo.Update(audit);
					if (!updateResult)
					{
						await ctx.RespondAsync("There was a problem writing to the Audit table");
					}
				}
			}
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Color = DiscordColor.Gray,
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
				Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been muted",
				Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
			};
			DiscordEmbed embed = embedBuilder.Build();
			await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}", embed);
			DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
			await warnsChannel.SendMessageAsync($"{member.Mention}", embed).ConfigureAwait(false);
		}

		[Command("unmute")]
		[RequireUserPermissions(Permissions.KickMembers)]
		[Description("Unmute a muted user")]
		public async Task UnMute(CommandContext ctx, [Description("The user that you want to unmute")] DiscordMember member)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
			{
				DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
				{
					Color = ctx.Member.Color,
					Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
					Description = $"[Link to usage]({ctx.Message.JumpLink})",
					Footer = new DiscordEmbedBuilder.EmbedFooter
					{
						IconUrl = ctx.Member.AvatarUrl,
						Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
					}
				};
				await ctx.Guild.GetChannel(722905404354592900).SendMessageAsync(discordEmbed.Build());
			}
			IEnumerable<DiscordRole> roles = member.Roles;
			if ((!roles.Contains(ctx.Guild.GetRole(701446136208293969))) || (roles.Contains(ctx.Guild.GetRole(767050052257447936))))
			{
				_ = await ctx.Channel.SendMessageAsync("User is not muted.");
				return;
			}
			DiscordRole verificationRole = ctx.Guild.GetRole(767050052257447936);
			DiscordRole mutedRole = ctx.Guild.GetRole(701446136208293969);
			await member.RevokeRoleAsync(mutedRole);
			await member.GrantRoleAsync(verificationRole);
			AuditRepository repo = new AuditRepository(ReadConfig.configJson.ConnectionString);
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool userResult = urepo.GetIdByDcId(ctx.Member.Id, out int id);
			if (!userResult)
			{
				await ctx.RespondAsync("There was a problem reading a User");
			}
			else
			{
				bool auditResult = repo.Read(id, out Audit audit);
				if (!auditResult)
				{
					await ctx.RespondAsync("There was a problem reading an Audit");
				}
				else
				{
					audit.Unmutes++;
					bool updateResult = repo.Update(audit);
					if (!updateResult)
					{
						await ctx.RespondAsync("There was a problem reading to the Audit table");
					}
				}
			}
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Color = DiscordColor.White,
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
				Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been unmuted",
				Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
			};
			DiscordEmbed embed = embedBuilder.Build();
			await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}", embed);
			DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
			await warnsChannel.SendMessageAsync($"{member.Mention}", embed).ConfigureAwait(false);
		}

		[Command("kick")]
		[RequireUserPermissions(Permissions.KickMembers)]
		[Description("Kick a user")]
		public async Task Kick(CommandContext ctx, [Description("The user that you want to kick")] DiscordMember member)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			if (member.Id == 192037157416730625)
			{
				await ctx.Channel.SendMessageAsync("You cant kick Lathrix!");
				return;
			}
			else if (ctx.Member.Hierarchy <= member.Hierarchy)
			{
				await ctx.Channel.SendMessageAsync("You cant kick someone higher or same rank as you!");
				return;
			}
			if (await AreYouSure(ctx, member, "kick"))
				return;
			await member.RemoveAsync();
			AuditRepository repo = new AuditRepository(ReadConfig.configJson.ConnectionString);
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool userResult = urepo.GetIdByDcId(ctx.Member.Id, out int id);
			if (!userResult)
			{
				await ctx.RespondAsync("There was a problem reading a User");
			}
			else
			{
				bool auditResult = repo.Read(id, out Audit audit);
				if (!auditResult)
				{
					await ctx.RespondAsync("There was a problem reading an Audit");
				}
				else
				{
					audit.Kicks++;
					bool updateResult = repo.Update(audit);
					if (!updateResult)
					{
						await ctx.RespondAsync("There was a problem reading to th Audit table");
					}
				}
			}
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Color = DiscordColor.DarkButNotBlack,
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
				Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been kicked",
				Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
			};
			DiscordEmbed embed = embedBuilder.Build();
			await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}", embed);
			DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
			await warnsChannel.SendMessageAsync($"{member.Mention}", embed).ConfigureAwait(false);
		}

		[Command("ban")]
		[Aliases("yeet")]
		[RequireUserPermissions(Permissions.BanMembers)]
		[Description("Ban a user")]
		public async Task Ban(CommandContext ctx, [Description("The user that you want to ban")] DiscordUser user, [RemainingText][Description("Why the user is boing banned")] string reason)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			DiscordMember member = null;
			if (ctx.Guild.Members.ContainsKey(user.Id))
			{
				member = await ctx.Guild.GetMemberAsync(user.Id);
			}
			if (user.Id == 192037157416730625)
			{
				await ctx.RespondAsync("You cant ban Lathrix!").ConfigureAwait(false);
				return;
			}
			else if (ctx.Member.Hierarchy <= member?.Hierarchy)
			{
				await ctx.RespondAsync("You cant ban someone higher or same rank as you!").ConfigureAwait(false);
				return;
			}
			if (await AreYouSure(ctx, user, "ban"))
				return;
			if (string.IsNullOrEmpty(reason))
			{
				await ctx.RespondAsync("Please provide a reason");
				return;
			}
			await ctx.Guild.BanMemberAsync(user.Id, reason: reason);
			AuditRepository repo = new AuditRepository(ReadConfig.configJson.ConnectionString);
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool userResult = urepo.GetIdByDcId(ctx.Member.Id, out int id);
			if (!userResult)
			{
				await ctx.RespondAsync("There was a problem reading a User");
			}
			else
			{
				bool auditResult = repo.Read(id, out Audit audit);
				if (!auditResult)
				{
					await ctx.RespondAsync("There was a problem reading an Audit");
				}
				else
				{
					audit.Bans++;
					bool updateResult = repo.Update(audit);
					if (!updateResult)
					{
						await ctx.RespondAsync("There was a problem reading to th Audit table");
					}
				}
			}
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Color = DiscordColor.Black,
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl },
				Title = $"{user.Username}#{user.Discriminator} ({user.Id}) has been banned",
				Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" },
				Description = reason
			};
			DiscordEmbed embed = embedBuilder.Build();
			await ctx.RespondAsync($"{ctx.Member.Mention}", embed).ConfigureAwait(false);
			DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
			await warnsChannel.SendMessageAsync($"{user.Mention}", embed).ConfigureAwait(false);
		}

		[Command("warn")]
		[RequireUserPermissions(Permissions.KickMembers)]
		[Description("Warn a user (for more information go to #staff-information and look at the warn documentation)")]
		public async Task Warn(CommandContext ctx, [Description("The user that you want to warn")] DiscordMember member, [Description("a link to the warned message (will get deleted and logged)")] DiscordMessage messageLink = null)
		{
			await WarnAsync(ctx, member, messageLink);
		}

		[Command("warn")]
		[RequireUserPermissions(Permissions.KickMembers)]
		[Description("Warn a user (for more information go to #staff-information and look at the warn documentation)")]
		public async Task Warn(CommandContext ctx, [Description("a link to the warned message (will get deleted and logged)")] DiscordMessage message)
		{
			await WarnAsync(ctx, await ctx.Guild.GetMemberAsync(message.Author.Id), message);
		}

		[Command("pardon")]
		[Aliases("unwarn")]
		[RequireUserPermissions(Permissions.BanMembers)]
		[Description("Pardon a warn of a user (for more information go to #staff-information and look at the warn documentation)")]
		public async Task Pardon(CommandContext ctx, [Description("The user that you want to pardon a warn of")] DiscordMember member, [Description("The number of the warn that you want to pardon")] int warnNumber)
		{
			WarnRepository repo = new WarnRepository(ReadConfig.configJson.ConnectionString);
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool result = urepo.GetIdByDcId(member.Id, out int id);
			if (!result)
			{
				await ctx.RespondAsync("There has been an error reading the Id of the User");
				return;
			}
			result = repo.GetWarnByUserAndNum(id, warnNumber, out Warn warn);
			if (!result)
			{
				await ctx.RespondAsync("There has been an error getting the warn from the database or the warn does not exist.");
				return;
			}
			result = repo.Delete(warn.ID);
			if (!result)
			{
				await ctx.RespondAsync("There has been an error deleting the warn from the database");
				return;
			}
			result = repo.GetAllByUser(warn.User, out List<Warn> others);
			if (!result)
			{
				await ctx.RespondAsync("Error reading other warns from the database.");
			}

			int counter = 0;
			foreach (var item in others)
			{
				counter++;
				item.Number = counter;
				result = repo.Update(item);
				if (!result)
				{
					_ = DiscordObjectService.Instance.ErrorLogChannel.SendMessageAsync("Error updating the database.");
					break;
				}
			}
			#region Audit
			AuditRepository auditRepo = new AuditRepository(ReadConfig.configJson.ConnectionString);
			UserRepository userrepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool userResult = userrepo.GetIdByDcId(ctx.Member.Id, out int userid);
			if (!userResult)
			{
				await ctx.RespondAsync("There was a problem reading a User");
			}
			else
			{
				bool auditResult = auditRepo.Read(userid, out Audit audit);
				if (!auditResult)
				{
					await ctx.RespondAsync("There was a problem reading an Audit");
				}
				else
				{
					audit.Pardons++;
					bool updateResult = auditRepo.Update(audit);
					if (!updateResult)
					{
						await ctx.RespondAsync("There was a problem writing to the Audit table");
					}
				}
			}
			#endregion
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Color = DiscordColor.Green,
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
				Title = $"Pardoned warn number {warnNumber} of {member.DisplayName}#{member.Discriminator} ({member.Id})",
				Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = ctx.Member.DisplayName }
			};
			DiscordEmbed embed = embedBuilder.Build();
			DiscordChannel warningsChannel = ctx.Guild.GetChannel(722186358906421369);
			await warningsChannel.SendMessageAsync($"{member.Mention}", embed).ConfigureAwait(false);
		}

		[Command("warns")]
		[Description("Check your or someone elses warnings")]
		public async Task Warns(CommandContext ctx)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			WarnRepository repo = new WarnRepository(ReadConfig.configJson.ConnectionString);
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool result = urepo.GetIdByDcId(ctx.Member.Id, out int id);
			if (!result)
			{
				await ctx.RespondAsync("There has been a problem getting the databaseId of the user");
				return;
			}
			result = repo.GetAllByUser(id, out List<Warn> warns);
			if (!result)
			{
				await ctx.RespondAsync("There has been an error getting the warns from the database");
			}

			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Member.AvatarUrl },
				Title = $"You have {warns.Count} Warnings:",
			};
			int pointsLeft = 15;
			foreach (Warn warn in warns)
			{
				urepo.Read(warn.Mod, out User mod);
				DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
				embedBuilder.AddField($"{warn.Number}: {warn.Reason}",
					$"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator}");
				pointsLeft -= warn.Level;
			}
			embedBuilder.AddField($"{pointsLeft} points left", GetFinalMessage(pointsLeft));
			if (pointsLeft == 15)
				embedBuilder.Color = DiscordColor.Green;
			else if (pointsLeft > 10)
				embedBuilder.Color = DiscordColor.Yellow;
			else if (pointsLeft > 5)
				embedBuilder.Color = DiscordColor.Orange;
			else
				embedBuilder.Color = DiscordColor.Red;
			DiscordEmbed embed = embedBuilder.Build();
			await ctx.Channel.SendMessageAsync($"{ctx.Member.Mention}", embed);
		}

		[Command("warns")]
		[Description("Check your or someone elses warnings")]
		public async Task Warns(CommandContext ctx, [Description("The user that you want to check the warning of (optional)")] DiscordMember member)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			WarnRepository repo = new WarnRepository(ReadConfig.configJson.ConnectionString);
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			bool result = urepo.GetIdByDcId(member.Id, out int id);
			if (!result)
			{
				await ctx.RespondAsync("There has been a problem getting the databaseId of the user");
				return;
			}
			result = repo.GetAllByUser(id, out List<Warn> warns);
			if (!result)
			{
				await ctx.RespondAsync("There has been an error getting the warns from the database");
			}

			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
				Title = $"You have {warns.Count} Warnings:",
			};
			int pointsLeft = 15;
			foreach (Warn warn in warns)
			{
				urepo.Read(warn.Mod, out User mod);
				DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
				embedBuilder.AddField($"{warn.Number}: {warn.Reason}",
					$"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator}");
				pointsLeft -= warn.Level;
			}
			embedBuilder.AddField($"{pointsLeft} points left", GetFinalMessage(pointsLeft));
			if (pointsLeft == 15)
				embedBuilder.Color = DiscordColor.Green;
			else if (pointsLeft > 10)
				embedBuilder.Color = DiscordColor.Yellow;
			else if (pointsLeft > 5)
				embedBuilder.Color = DiscordColor.Orange;
			else
				embedBuilder.Color = DiscordColor.Red;
			DiscordEmbed embed = embedBuilder.Build();
			await ctx.Channel.SendMessageAsync(embed);
		}

		[Command("report")]
		[Description("Report a staff member to the senate. (Please don't abuse this system)")]
		public async Task Report(CommandContext ctx, [Description("The Id of the staff member you want to report.")] ulong ID)
		{
			await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
			DiscordGuild Lathland = await ctx.Client.GetGuildAsync(699555747591094344);
			DiscordMember member = await Lathland.GetMemberAsync(ID);
			if (!member.Roles.Contains(Lathland.GetRole(796234634316873759)) && !member.Roles.Contains(Lathland.GetRole(748646909354311751)))
			{
				await ctx.Channel.SendMessageAsync("```User is not staff or is part of the Senate.\n" + "If they are part of the Senate try messaging Chewybaca and/or another member of the senate```");
				return;
			}
			InteractivityExtension interactivity = ctx.Client.GetInteractivity();
			DiscordMessage message = await ctx.Channel.SendMessageAsync("```Please state a reason for your report!\n" +
				"If you don't say anything for 5 minutes i will have to ignore you.\n" +
				"Please don't abuse this system.```").ConfigureAwait(false);
			InteractivityResult<DiscordMessage> result = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User).ConfigureAwait(false);
			await message.DeleteAsync();
			string reportReason = result.Result.Content;
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Color = DiscordColor.Red,
				Title = $"Report from user {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})",
				Description = $"Reported user: {member.Username}#{member.Discriminator} ({member.Id})",
				Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = member.AvatarUrl },
				Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.User.AvatarUrl, Text = ctx.User.Username }
			};

			embedBuilder.AddField("Reason:", reportReason);

			DiscordEmbed embed = embedBuilder.Build();
			List<DiscordMember> senate = new List<DiscordMember>
			{
				await Lathland.GetMemberAsync(613366102306717712),//Chewy
                await Lathland.GetMemberAsync(387325006176059394),//Myself
                await Lathland.GetMemberAsync(289112287250350080)//Parth
            };
			foreach (DiscordMember senator in senate)
			{
				await senator.SendMessageAsync(embed).ConfigureAwait(false);
			}
			await ctx.Channel.SendMessageAsync("Report successfully sent, The senate will get back to you, until then please be patient.");
		}

		[Command("persist")]
		[RequireUserPermissions(Permissions.Administrator)]
		public async Task Persist(CommandContext ctx, [Description("Member that got warned")] DiscordMember member, [Description("The number of the warn")] int warnNumber)
		{
			await ctx.Channel.TriggerTypingAsync();
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			WarnRepository repo = new WarnRepository(ReadConfig.configJson.ConnectionString);
			bool result = urepo.GetIdByDcId(member.Id, out int userDbId);
			if (!result)
			{
				await ctx.RespondAsync("Failed to get the user from the database.");
				return;
			}
			result = repo.GetWarnByUserAndNum(userDbId, warnNumber, out Warn warn);
			if (!result)
			{
				await ctx.RespondAsync("Failed to get the warn from the database.");
				return;
			}
			if (warn.Level > 10)
			{
				await ctx.RespondAsync("Warn is already persistent by level");
				return;
			}
			else if (warn.Persistent)
			{
				await ctx.RespondAsync("Warn is already persistent");
				return;
			}
			warn.Persistent = true;
			result = repo.Update(warn);
			if (!result)
			{
				await ctx.RespondAsync("Failed to update the warn table.");
				return;
			}

			result = urepo.Read(warn.Mod, out User mod);
			if (!result)
			{
				await ctx.RespondAsync("Error reading the moderator from the database. (But warn has been persistet)");
				return;
			}
			DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder
			{
				Title = $"Persistet warn {warn.ID}",
				Description = warn.Reason,
				Color = DiscordColor.Red,
				Footer = new DiscordEmbedBuilder.EmbedFooter
				{
					IconUrl = moderator.AvatarUrl,
					Text = $"{moderator.DisplayName}#{moderator.Discriminator} ({moderator.Id})"
				}
			};
			builder.AddField("Level:", warn.Level.ToString());
			builder.AddField("Number:", warn.Number.ToString());
			builder.AddField("Time of the Warn:", warn.Time.ToString("yyyy-mm-ddTHH:mm:ss.ffff"));
			builder.AddField("Persistent:", warn.Persistent.ToString());
			await ctx.RespondAsync(builder);

			DiscordEmbedBuilder dmBuilder = new DiscordEmbedBuilder
			{
				Title = "Persistent warns",
				Description = "One of your warns has been persistet and will no longer expire. The warn will only ever be removed after manual review by an Admin.",
				Color = DiscordColor.Red,
				Footer = new DiscordEmbedBuilder.EmbedFooter
				{
					IconUrl = ctx.Member.AvatarUrl,
					Text = $"{ctx.Member.DisplayName}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
				}
			};

			dmBuilder.AddField("Reason: ", warn.Reason);
			dmBuilder.AddField("Level:", warn.Level.ToString());
			dmBuilder.AddField("Number:", warn.Number.ToString());
			dmBuilder.AddField("Time of the Warn:", warn.Time.ToString("yyyy-mm-dd HH:mm:ss.ffff"));
			await member.SendMessageAsync(dmBuilder);
		}

		[Command("allwarns")]
		[Description("Display all currently unpardoned warns")]
		[RequireUserPermissions(DSharpPlus.Permissions.KickMembers)]
		public async Task AllWarns(CommandContext ctx)
		{
			await ctx.Channel.TriggerTypingAsync();
			WarnRepository repo = new WarnRepository(ReadConfig.configJson.ConnectionString);
			bool result = repo.GetAll(out List<Warn> warns);
			if (!result)
			{
				await ctx.RespondAsync("There has been an error reading the warns from the database");
				return;
			}

			int index = 0;
			int indicator = 0;
			UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
			List<Page> pages = new List<Page>();
			DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
			{
				Title = $"Showing all warnings in the server",
				Color = ctx.Guild.GetMemberAsync(192037157416730625).Result.Color,
				Description = "Use -warns <User> to get more information on a specific user"
			};
			foreach (Warn warn in warns)
			{
				index++;
				indicator++;
				if (indicator == 10 || index == warns.Count)
				{
					result = urepo.Read(warn.Mod, out User mod);
					if (!result)
					{
						await ctx.RespondAsync("There was an error getting a user from the database");
						continue;
					}
					DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
					try
					{
						result = urepo.Read(warn.Mod, out User user);
						if (!result)
						{
							await ctx.RespondAsync("There was an error getting a user from the database");
							continue;
						}
						DiscordMember member = await ctx.Guild.GetMemberAsync(user.DcID);
						embedBuilder.AddField($"{index}: {warn.Reason}",
							$"Warned user: {member.DisplayName}#{member.Discriminator}\n" +
							$"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator}");
					}
					catch
					{
						result = urepo.Read(warn.Mod, out User user);
						if (!result)
						{
							await ctx.RespondAsync("There was an error getting a user from the database");
							continue;
						}
						embedBuilder.AddField($"{index}: {warn.Reason}",
							$"Warned user: {user.DcID}\n" +
							$"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator}");
					}

					pages.Add(new Page { Embed = embedBuilder.Build() });
					embedBuilder = new DiscordEmbedBuilder
					{
						Title = $"Showing all warnings in the server",
						Color = ctx.Guild.GetMemberAsync(192037157416730625).Result.Color,
						Description = "Use -warns <User> to get more information on a specific user"
					};
				}
				else
				{
					result = urepo.Read(warn.Mod, out User mod);
					if (!result)
					{
						await ctx.RespondAsync("There was an error getting a user from the database");
						continue;
					}
					DiscordMember moderator = await ctx.Guild.GetMemberAsync(mod.DcID);
					try
					{
						result = urepo.Read(warn.Mod, out User user);
						if (!result)
						{
							await ctx.RespondAsync("There was an error getting a user from the database");
							continue;
						}
						DiscordMember member = await ctx.Guild.GetMemberAsync(user.DcID);
						embedBuilder.AddField($"{index}: {warn.Reason}",
							$"Warned user: {member.DisplayName}#{member.Discriminator}\n" +
							$"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator}");
					}
					catch
					{
						result = urepo.Read(warn.Mod, out User user);
						if (!result)
						{
							await ctx.RespondAsync("There was an error getting a user from the database");
							continue;
						}
						embedBuilder.AddField($"{index}: {warn.Reason}",
							$"Warned user: {user.DcID}\n" +
							$"Points: -{warn.Level}; Date: {warn.Time}; Warned by {moderator.DisplayName}#{moderator.Discriminator}");
					}
				}
			}
			await ctx.Channel.SendPaginatedMessageAsync(ctx.User, pages);
		}

		[Command("sql")]
		[Description("send a SQL Query to the database")]
		[RequireRoles(RoleCheckMode.Any, "Bot Management")]
		public async Task SqlQuery(CommandContext ctx, [Description("Query")][RemainingText] string command)
		{
			SqlConnection connection = new SqlConnection(ReadConfig.configJson.ConnectionString);
			try
			{
				if (ctx.Member.Id != 387325006176059394)
					return;
				await ctx.TriggerTypingAsync().ConfigureAwait(false);
				command.Trim();
				SqlCommand cmd = new SqlCommand(command, connection);
				string resultString = "";
				connection.Open();
				SqlDataReader reader = cmd.ExecuteReader();
				while (reader.Read())
				{
					for (int index = 0; index < reader.FieldCount; index++)
						resultString += reader.GetSqlValue(index).ToString() + " ";
					resultString.Trim();
					if (resultString.Length >= 900)
					{
						await ctx.Channel.SendMessageAsync("```d\n" + resultString + "\n```");
						await ctx.Channel.TriggerTypingAsync().ConfigureAwait(false);
						resultString = "";
					}
					else
						resultString += "\n";
				}
				reader.Close();
				connection.Close();
				if (string.IsNullOrEmpty(resultString))
					await ctx.Channel.SendMessageAsync("```\nQuery done!\n```");
				else
					await ctx.Channel.SendMessageAsync("```c\n" + resultString + "\n```");
			}
			catch (Exception e)
			{
				await ctx.RespondAsync(e.Message);
			}
			finally
			{
				if (connection.State == System.Data.ConnectionState.Open)
				{
					connection.Close();
				}
			}
		}

		private string GetFinalMessage(int pointsLeft)
		{
			if (pointsLeft == 15)
				return "Nice job!";
			else if (pointsLeft > 10)
				return "You are in the clear!";
			else if (pointsLeft > 5)
				return "Be careful!";
			else
				return "You really gotta listen to the mods better!"; ;
		}

		private async Task<bool> AreYouSure(CommandContext ctx, DiscordUser user, string operation)
		{
			DiscordMember member = null;
			if (ctx.Guild.Members.ContainsKey(user.Id))
			{
				member = await ctx.Guild.GetMemberAsync(user.Id);
			}

			DiscordMessageBuilder builder = new DiscordMessageBuilder
			{
				Content = "Are you fucking sure about that?",
				Embed = new DiscordEmbedBuilder
				{
					Title = "Member you selected:",
					Description = member == null ? user.ToString() : member.ToString(),
					Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
					{
						Url = user.AvatarUrl
					},
					Color = member == null ? new DiscordColor("#FF0000") : member.Color
				}
			};
			List<DiscordComponent> components = new List<DiscordComponent>
			{
				new DiscordButtonComponent(ButtonStyle.Danger, "sure", "Yes I fucking am!"),
				new DiscordButtonComponent(ButtonStyle.Secondary, "abort", "NO ABORT, ABORT!")
			};
			builder.WithComponents(components);
			DiscordMessage message = await builder.SendAsync(ctx.Channel);
			InteractivityExtension interactivity = ctx.Client.GetInteractivity();
			var interactivityResult = await interactivity.WaitForButtonAsync(message, ctx.User);

			await message.DeleteAsync();
			if (interactivityResult.Result.Id == "abort")
			{
				await ctx.RespondAsync($"Okay i will not {operation} the user.");
				return true;
			}
			return false;
		}

		private async Task WarnAsync(CommandContext ctx, DiscordMember member, DiscordMessage messageLink = null)
		{
			if (member.Id == 192037157416730625)
			{
				await ctx.Channel.SendMessageAsync("You cant warn Lathrix!");
				return;
			}
			else if (ctx.Member.Roles.Contains(ctx.Guild.GetRole(748646909354311751)))
			{
				DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
				{
					Color = ctx.Member.Color,
					Title = $"Trial Plague {ctx.Member.Nickname} just used a moderation command",
					Description = $"[Link to usage]({ctx.Message.JumpLink})",
					Footer = new DiscordEmbedBuilder.EmbedFooter
					{
						IconUrl = ctx.Member.AvatarUrl,
						Text = $"{ctx.Member.Username}#{ctx.Member.Discriminator} ({ctx.Member.Id})"
					}
				};
				await ctx.Guild.GetChannel(722905404354592900).SendMessageAsync(discordEmbed.Build());
			}
			InteractivityExtension interactivity = ctx.Client.GetInteractivity();
			if (await ctx.Guild.GetMemberAsync(member.Id) == null)
				await ctx.Channel.SendMessageAsync($"User {member.DisplayName} is not on this server anymore, you can't warn them!");
			else
			{
				#region GetRule
				DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder
				{
					Content = "```" +
					$"Rule 1 {RuleService.rules[0].RuleText} - {RuleService.rules[0].MinPoints}-{RuleService.rules[0].MaxPoints} Points\n" +
					$"Rule 2 {RuleService.rules[1].RuleText} - {RuleService.rules[1].MinPoints}-{RuleService.rules[1].MaxPoints} Points\n" +
					$"Rule 3 {RuleService.rules[2].RuleText} - {RuleService.rules[2].MinPoints}-{RuleService.rules[2].MaxPoints} Points\n" +
					$"Rule 4 {RuleService.rules[3].RuleText} - {RuleService.rules[3].MinPoints}-{RuleService.rules[3].MaxPoints} Points\n" +
					$"Rule 5 {RuleService.rules[4].RuleText} - {RuleService.rules[4].MinPoints}-{RuleService.rules[4].MaxPoints} Points\n" +
					$"Rule 6 {RuleService.rules[5].RuleText} - {RuleService.rules[5].MinPoints}-{RuleService.rules[5].MaxPoints} Points\n" +
					$"Rule 7 {RuleService.rules[6].RuleText} - {RuleService.rules[6].MinPoints}-{RuleService.rules[6].MaxPoints} Points\n" +
					$"Rule 8 {RuleService.rules[7].RuleText} - {RuleService.rules[7].MinPoints}-{RuleService.rules[7].MaxPoints} Points\n" +
					$"Rule 9 {RuleService.rules[8].RuleText} - {RuleService.rules[8].MinPoints}-{RuleService.rules[8].MaxPoints} Points\n" +
					$"Rule 10 {RuleService.rules[9].RuleText} - {RuleService.rules[9].MinPoints}-{RuleService.rules[9].MaxPoints} Points\n" +
					$"Rule 11 {RuleService.rules[10].RuleText} - {RuleService.rules[10].MinPoints}-{RuleService.rules[10].MaxPoints} Points\n" +
					$"Rule 12 {RuleService.rules[11].RuleText} - {RuleService.rules[11].MinPoints}-{RuleService.rules[11].MaxPoints} Points\n" +
					$"Rule 13 {RuleService.rules[12].RuleText} - {RuleService.rules[12].MinPoints}-{RuleService.rules[12].MaxPoints} Points\n" +
					$"Other - {RuleService.rules[13].MinPoints}-{RuleService.rules[13].MaxPoints} Points" +
					"```"
				};
				for (int i = 0; i < Math.Round(value: (RuleService.rules.Length - 1) / (double)5, MidpointRounding.ToPositiveInfinity); i++)
				{
					List<DiscordComponent> row = new List<DiscordComponent>();
					for (int index = i * 5; index < ((((i * 5) + 5) > RuleService.rules.Length - 1) ? RuleService.rules.Length : (i * 5) + 5); index++)
					{
						row.Add(new DiscordButtonComponent
						(
							ButtonStyle.Primary,
							RuleService.rules[index].RuleNum.ToString(),
							$"Rule {RuleService.rules[index].RuleNum}: {RuleService.rules[index].ShortDesc}"
						));
					}
					messageBuilder.WithComponents(row);
				}
				DiscordMessage message = await ctx.Channel.SendMessageAsync(messageBuilder);

				var reaction = await interactivity.WaitForButtonAsync(message, ctx.Member).ConfigureAwait(false);
				Rule rule = RuleService.rules.Single(x => x.RuleNum.ToString() == reaction.Result.Id);
				await message.DeleteAsync();
				#endregion
				#region GetPoints
				int pointsDeducted;
				DiscordMessageBuilder discordMessage = new DiscordMessageBuilder
				{
					Content = $"For this rule you can reduce the users chances by {rule.MinPoints} - {rule.MaxPoints}"
				};
				for (int i = 0; i < 3; i++)
				{
					List<DiscordButtonComponent> buttons = new List<DiscordButtonComponent>();
					for (int index = i * 5; index < (i * 5) + 5; index++)
					{
						buttons.Add(new DiscordButtonComponent
						(
							ButtonStyle.Primary,
							(index + 1).ToString(),
							(index + 1).ToString(),
							(index + 1) < rule.MinPoints || (index + 1) > rule.MaxPoints)
						);
					}
					discordMessage.WithComponents(buttons);
				}
				DiscordMessage pointsMessage = await ctx.Channel.SendMessageAsync(discordMessage);
				var interactpointsMessage = await interactivity.WaitForButtonAsync(message, ctx.User);
				pointsDeducted = int.Parse(interactpointsMessage.Result.Id);
				#endregion
				#region GetSeverity
				int severity = (pointsDeducted) switch
				{
					1 => 1,
					2 => 1,
					3 => 1,
					4 => 1,
					5 => 1,
					6 => 2,
					7 => 2,
					8 => 2,
					9 => 2,
					10 => 2,
					11 => 3,
					12 => 3,
					13 => 3,
					14 => 3,
					15 => 3,
					_ => 0
				};
				#endregion
				#region GetReason
				bool tryagainReason = true;
				string reason = "/";
				while (tryagainReason)
				{
					DiscordMessage reasonMessage = await ctx.Channel.SendMessageAsync("If needed please state a reason, write ``NONE`` if you dont want to specify.").ConfigureAwait(false);
					InteractivityResult<DiscordMessage> reasonResult = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User).ConfigureAwait(false);
					if (reasonResult.Result.Content.Trim().ToUpper() == "NONE")
					{
						await reasonMessage.DeleteAsync();
						reason = "/";
						tryagainReason = false;
					}
					else if (reasonResult.Result.Content.Length >= 250)
					{
						DiscordMessage buffoon = await ctx.Channel.SendMessageAsync("Max reason length is 250 characters!").ConfigureAwait(false);
						await reasonMessage.DeleteAsync().ConfigureAwait(false);
						await Task.Delay(3000);
						await buffoon.DeleteAsync().ConfigureAwait(false);
					}
					else
					{
						reason = reasonResult.Result.Content;
						await reasonMessage.DeleteAsync().ConfigureAwait(false);
						tryagainReason = false;
					}
				}
				#endregion
				#region Write
				UserRepository repo = new UserRepository(ReadConfig.configJson.ConnectionString);
				bool result = repo.GetIdByDcId(member.Id, out int UserDbId);
				if (!result)
				{
					await ctx.RespondAsync("There was an error getting the user from the Database");
					return;
				}
				WarnRepository warnRepo = new WarnRepository(ReadConfig.configJson.ConnectionString);
				result = warnRepo.GetWarnAmount(UserDbId, out int WarnNumber);
				if (!result)
				{
					await ctx.RespondAsync("There was an error getting the previous warns from the Database");
					return;
				}
				result = repo.GetIdByDcId(ctx.Member.Id, out int ModDbId);
				if (!result)
				{
					await ctx.RespondAsync("There was an error getting the moderator from the database");
				}
				if (UserDbId == 0 || ModDbId == 0)
					return;
				Warn warn = new Warn
				{
					User = UserDbId,
					Mod = ModDbId,
					Reason = rule.RuleNum == 0 ? reason : $"Rule {rule.RuleNum}, {reason}",
					Number = WarnNumber + 1,
					Level = pointsDeducted,
					Time = DateTime.Now,
					Persistent = false

				};
				result = warnRepo.Create(ref warn);
				if (!result)
				{
					await ctx.RespondAsync("There was an error creating the database entry");
					return;
				}
				#endregion
				#region Audit
				AuditRepository auditRepo = new AuditRepository(ReadConfig.configJson.ConnectionString);
				UserRepository urepo = new UserRepository(ReadConfig.configJson.ConnectionString);
				bool userResult = urepo.GetIdByDcId(ctx.Member.Id, out int id);
				if (!userResult)
				{
					await ctx.RespondAsync("There was a problem reading a User");
				}
				else
				{
					bool auditResult = auditRepo.Read(id, out Audit audit);
					if (!auditResult)
					{
						await ctx.RespondAsync("There was a problem reading an Audit");
					}
					else
					{
						audit.Warns++;
						bool updateResult = auditRepo.Update(audit);
						if (!updateResult)
						{
							await ctx.RespondAsync("There was a problem reading to th Audit table");
						}
					}
				}
				#endregion
				result = warnRepo.GetRemainingPoints(UserDbId, out int pointsLeft);
				if (!result)
				{
					await ctx.RespondAsync("There was an error reading the remaining points");
				}
				#region WarnMessage
				if (severity == 1)
				{
					try
					{
						DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
						{
							Color = DiscordColor.Yellow,
							Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Height = 8, Width = 8, Url = member.AvatarUrl },
							Title = $"You have been warned for Rule {rule.RuleNum}:",
							Description = $"{rule.RuleText}\n" +
								"\n" +
								$"{reason}",
							Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
						};

						embedBuilder.AddField($"{pointsLeft} points remaining", "Please keep any talk of this to DM's");

						DiscordEmbed embed = embedBuilder.Build();

						DiscordChannel directChannel = await member.CreateDmChannelAsync();
						await directChannel.SendMessageAsync(embed).ConfigureAwait(false);
					}
					catch (Exception e)
					{
						DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
						{
							Color = DiscordColor.Yellow,
							Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Height = 8, Width = 8, Url = member.AvatarUrl },
							Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been warned for Rule {rule.RuleNum}:",
							Description = $"{rule.RuleText}\n" +
								"\n" +
								$"{reason}",
							Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
						};
						embedBuilder.AddField($"{pointsLeft} points remaining", "Please keep any talk of this to DM's");
						DiscordEmbed embed = embedBuilder.Build();

						DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
						await warnsChannel.SendMessageAsync($"{member.Mention}", embed).ConfigureAwait(false);

						SystemService.Instance.Logger.Log("Had to send low level warn to #warnings because of following error:\n" + e.Message);
					}
					finally
					{
						DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
						{
							Color = DiscordColor.Yellow,
							Title = $"Successfully warned {member.DisplayName}#{member.Discriminator} ({member.Id}).",
							Description = $"Rule {rule.RuleNum}:\n" +
								"\n" +
								$"{reason}\n" +
								"\n" +
								$"User has {pointsLeft} points left.",
							Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
						};
						DiscordEmbed embed = embedBuilder.Build();
						await ctx.Channel.SendMessageAsync(embed);
					}
				}
				else if (severity == 2)
				{
					DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
					{
						Color = DiscordColor.Orange,
						Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Height = 8, Width = 8, Url = member.AvatarUrl },
						Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been warned for Rule {rule.RuleNum}:",
						Description = $"{rule.RuleText}\n" +
								"\n" +
								$"{reason}",
						Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
					};
					embedBuilder.AddField($"{pointsLeft} points remaining", "Please keep any talk of this to DM's");
					DiscordEmbed embed = embedBuilder.Build();

					DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
					await warnsChannel.SendMessageAsync($"{member.Mention}", embed).ConfigureAwait(false);
				}
				else if (severity == 3)
				{
					DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder
					{
						Color = DiscordColor.Red,
						Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Height = 8, Width = 8, Url = member.AvatarUrl },
						Title = $"{member.DisplayName}#{member.Discriminator} ({member.Id}) has been warned for Rule {rule.RuleNum}:",
						Description = $"{rule.RuleText}\n" +
								"\n" +
								$"{reason}",
						Footer = new DiscordEmbedBuilder.EmbedFooter { IconUrl = ctx.Member.AvatarUrl, Text = $"{ctx.Member.DisplayName}" }
					};
					embedBuilder.AddField($"{pointsLeft} points remaining", "Please keep any talk of this to DM's");
					DiscordEmbed embed = embedBuilder.Build();

					DiscordChannel warnsChannel = ctx.Guild.GetChannel(722186358906421369);
					await warnsChannel.SendMessageAsync($"{member.Mention}", embed).ConfigureAwait(false);
				}
				else
				{
					await ctx.Channel.SendMessageAsync("Task successfully failed!");
				}
				#endregion
				if (messageLink != null)
				{
					#region Log
					DiscordEmbedBuilder discordEmbed = new DiscordEmbedBuilder
					{
						Author = new DiscordEmbedBuilder.EmbedAuthor
						{
							IconUrl = messageLink.Author.AvatarUrl,
							Name = messageLink.Author.Username
						},
						Description = messageLink.Content,
						Color = ((DiscordMember)messageLink.Author).Color
					};
					if (messageLink.Attachments.Count != 0)
					{
						try
						{
							discordEmbed.ImageUrl = messageLink.Attachments[0].Url;
							await ctx.Channel.SendMessageAsync(discordEmbed).ConfigureAwait(false);
						}
						catch
						{
							DiscordMessageBuilder builder = new DiscordMessageBuilder
							{
								Embed = discordEmbed
							};
							WebClient client = new WebClient();
							client.DownloadFile(messageLink.Attachments[0].Url, messageLink.Attachments[0].FileName);
							FileStream stream = new FileStream(messageLink.Attachments[0].FileName, FileMode.Open);
							builder.WithFile(stream);
							stream.Close();
							await ctx.Channel.SendMessageAsync(builder).ConfigureAwait(false);
							File.Delete(messageLink.Attachments[0].FileName);
						}
					}
					else
					{
						await ctx.Channel.SendMessageAsync(discordEmbed).ConfigureAwait(false);
					}
					#endregion
					await messageLink.DeleteAsync().ConfigureAwait(false);
				}
				if (pointsLeft < 11)
				{
					DiscordMessage punishMessage = await ctx.Channel.SendMessageAsync($"User has {pointsLeft} points left.\n" +
						$"By common practice the user should be muted{(pointsLeft < 6 ? ", kicked" : "")}{(pointsLeft < 1 ? ", or banned" : "")}.");
				}
			}
		}
	}
}