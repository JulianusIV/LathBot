﻿using DSharpPlus.Commands;
using DSharpPlus.Entities;
using LathBotBack.Config;
using LathBotBack.Models;
using LathBotBack.Repos;
using LathBotBack.Services;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LathBotFront.Commands
{
    public partial class InfoCommands
    {
        [Command("info")]
        [Description("Display some info about the bot")]
        public async Task Info(CommandContext ctx)
        {
            DiscordEmbedBuilder discordEmbed = new()
            {
                Color = new DiscordColor(101, 24, 201),
                Title = "LathBot#1753",
                Description = $"LathBot is a custom bot for the server Lathland, prefix is - or {ctx.Client.CurrentUser.Mention}\n" +
                    "For more info use -help, -tos, or -privacy",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Client.CurrentUser.AvatarUrl }
            };
            discordEmbed.AddField("Language", "C# using Visual Studio 2022");
            discordEmbed.AddField("Library", "DSharpPlus, Version:" + ctx.Client.VersionString);
            discordEmbed.AddField(".NET Core Version: ", PlatformServices.Default.Application.RuntimeFramework.Version.ToString(2));
            discordEmbed.AddField("Repository", "[GitHub](https://github.com/JulianusIV/LathBot)");
            TimeSpan uptime = DateTime.Now - StartupService.Instance.StartTime;
            discordEmbed.AddField("Uptime", $"Bot has been running since {uptime}");
            await ctx.RespondAsync(discordEmbed.Build());
        }

        [Command("tos")]
        [Description("The bot's TOS")]
        public async Task Tos(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            using Stream stream = typeof(InfoCommands).Assembly.GetManifestResourceStream("LathBotFront.Resources.LathBotTOS.txt");
            using StreamReader reader = new(stream);
            string result = reader.ReadToEnd();

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                .WithTitle("Terms of Service");

            while (!result.StartsWith("> ") && result.Contains('#'))
            {
                var title = $"{result.Substring(result.IndexOf("# ") + 2, result.IndexOf('\n') - 2)}";
                result = result[(result.IndexOf('\n') + 1)..];

                string desc;
                if (result.Contains("# "))
                {
                    desc = result[..result.IndexOf("# ")];
                    result = result[result.IndexOf("# ")..];
                }
                else
                {
                    desc = result[..result.IndexOf("> ")];
                    result = result[result.IndexOf("> ")..];
                }
                var descs = NewlineRegex().Split(desc);

                embed.AddField(title, descs[0]);

                for (int i = 1; i < descs.Length; i++)
                {
                    embed.AddField("ㅤ", descs[i]);
                }
            }

            embed.WithFooter(result[(result.IndexOf("> ") + 2)..]);

            await ctx.RespondAsync(embed);
        }

        [Command("privacy")]
        [Description("The bot's privacy policy")]
        public async Task Privacy(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            string part = "";
            int index = 0;
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LathBotFront.Resources.LathBotPP.txt");
            using StreamReader reader = new(stream);
            string result = reader.ReadToEnd();

            List<string> content = [];
            foreach (char character in result)
            {
                index++;
                if (part.Length < 4096)
                {
                    if (index < result.Length)
                        part += character;
                    else
                        content.Add(part);
                }
                else
                {
                    content.Add(part);
                    part = character.ToString();
                }
            }
            foreach (string cut in content)
            {
                await ctx.Channel.SendMessageAsync(new DiscordEmbedBuilder { Title = "Privacy Policy", Description = cut });
            }
        }

        [Command("stafftime")]
        [Description("Display timezones and current time of staff members")]
        public async Task StaffTimes(CommandContext ctx)
        {
            await ctx.DeferResponseAsync();

            DiscordEmbedBuilder discordEmbed = new()
            {
                Color = new DiscordColor(64, 255, 0),
                Title = "Staff timezones",
                Description = "Keep in mind that it being daytime in the users timezone does not obligate them to being available!"
            };
            DateTime thisTime = DateTime.Now;

            ModRepository repo = new(ReadConfig.Config.ConnectionString);
            UserRepository urepo = new(ReadConfig.Config.ConnectionString);
            bool result = repo.GetAll(out List<Mod> list);
            if (!result)
            {
                await ctx.RespondAsync("Error reading mods from database.");
                return;
            }

            var modList = list.ToDictionary(dbMod =>
            {
                urepo.Read(dbMod.DbId, out User entity);
                return ctx.Guild.GetMemberAsync(entity.DcID).GetAwaiter().GetResult();
            });

            modList = modList.OrderByDescending(x => x.Key.Hierarchy).ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in modList)
            {
                try
                {
                    TimeZoneInfo modTimeZone = TimeZoneInfo.FindSystemTimeZoneById(item.Value.Timezone);
                    DateTime modTime = TimeZoneInfo.ConvertTime(thisTime, TimeZoneInfo.Local, modTimeZone);
                    discordEmbed.AddField($"{item.Key.Username}#{item.Key.Discriminator}",
                        modTime.ToString("yyyy-MM-dd     **HH:mm**") + "     (" + (modTimeZone.IsDaylightSavingTime(modTime) ? modTimeZone.DaylightName : modTimeZone.StandardName) + ")");
                }
                catch
                {
                    await ctx.RespondAsync($"Error creating the embed for user {item.Key.DisplayName}#{item.Key.Discriminator} ({item.Key.Id}).");
                    continue;
                }
            }
            await ctx.RespondAsync(discordEmbed);
        }

        [GeneratedRegex("\r?\n\r?\n")]
        private static partial Regex NewlineRegex();
    }
}
