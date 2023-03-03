using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LathBotFront.EventHandlers
{
    public class RoleAssign
    {
        private static readonly ulong[] greetingsEnabledRoleIds = new ulong[]
        {
            748533112404705281, //vet
            819296501451980820, //true
            707915540085342248, //godly
            759456739669704784, //ungodly
            749576790929965136, //booster
            702925135568437270, //booster
            822957506674163792  //mod team
        };

        private static readonly ulong[] gamesRoleIds = new ulong[]
        {
            978954327907532811,  //airships
            1046124300761043004, //dnd
            701454853095817316,  //ftd
            713367380574732319,  //minecraft
            850029252812210207,  //reassembly
            701454772900855819,  //stellaris
            1014261454624542810, //tt
            766322672321560628   //wh40k
        };

        public static readonly ulong[] miscRoleIds = new ulong[]
        {
            794975835235942470, //ads
            767039219403063307, //art
            765622563338453023, //dquotes
            741342066021367938, //dquestions
            848307821703200828, //dap
            718162129609556121, //debate
            812755886413971499, //vent
            1014280593929928797 //greet
        };

        public static readonly ulong[] ageRequiredRoleIds = new ulong[]
        {
            718162129609556121, //debate
            767039219403063307, //art
            978954327907532811,  //airships
            1046124300761043004, //dnd
            701454853095817316,  //ftd
            713367380574732319,  //minecraft
            850029252812210207,  //reassembly
            701454772900855819,  //stellaris
            1014261454624542810, //tt
            766322672321560628,   //wh40k
            812755886413971499 //vent
        };

        internal static Task ComponentTriggered(DiscordClient _1, ComponentInteractionCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Id == "roleme_games")
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var builder = new DiscordWebhookBuilder()
                        .WithContent("Select your game roles");

                    var member = await e.Guild.GetMemberAsync(e.User.Id);
                    var options = new List<DiscordSelectComponentOption>()
                    {
                        new DiscordSelectComponentOption("Airships: Conquer the Skies", "airships", "Get access to the Airships: Conquer the Skies game channel", member.Roles.Any(x => x.Id == 978954327907532811), new DiscordComponentEmoji("🛩️")),
                        new DiscordSelectComponentOption("Dungeons and Dragons", "dnd", "Get access to the Dungeons and Dragons game channel and vc", member.Roles.Any(x => x.Id == 1046124300761043004), new DiscordComponentEmoji("🐉")),
                        new DiscordSelectComponentOption("From the Depths", "ftd", "Get access to the From the Depths game channel", member.Roles.Any(x => x.Id == 701454853095817316), new DiscordComponentEmoji("⛵")),
                        new DiscordSelectComponentOption("Minecraft", "minecraft", "Get access to the Minecraft game channel", member.Roles.Any(x => x.Id == 713367380574732319), new DiscordComponentEmoji("⛏️")),
                        new DiscordSelectComponentOption("Reassembly", "reassembly", "Get access to the Reassembly game channel", member.Roles.Any(x => x.Id == 701454772900855819), new DiscordComponentEmoji("👾")),
                        new DiscordSelectComponentOption("Stellairs", "stellaris", "Get access to the Stellairs game channel", member.Roles.Any(x => x.Id == 701454772900855819), new DiscordComponentEmoji("⭐")),
                        new DiscordSelectComponentOption("Terra Tech", "tt", "Get access to the Terra Tech game channel", member.Roles.Any(x => x.Id == 1014261454624542810), new DiscordComponentEmoji("🤖")),
                        new DiscordSelectComponentOption("Warhammer 40k", "wh40k", "Get access to the Warhammer 40k game channel", member.Roles.Any(x => x.Id == 766322672321560628), new DiscordComponentEmoji("🔨"))
                    };

                    builder.AddComponents(new DiscordSelectComponent("roleme_games_dropdown", "Select your roles", options, false, 0, options.Count));

                    await e.Interaction.EditOriginalResponseAsync(builder);
                }
                else if (e.Id == "roleme_misc")
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var builder = new DiscordWebhookBuilder()
                        .WithContent("Select your misc roles");

                    var member = await e.Guild.GetMemberAsync(e.User.Id);
                    var options = new List<DiscordSelectComponentOption>()
                    {
                        new DiscordSelectComponentOption("Advertisements", "ads", "Get pinged for new advertisements", member.Roles.Any(x => x.Id == 794975835235942470), new DiscordComponentEmoji("📢")),
                        new DiscordSelectComponentOption("Art and Photography", "art", "Get access to the Art and the Photography channels", member.Roles.Any(x => x.Id == 767039219403063307), new DiscordComponentEmoji("🎨")),
                        new DiscordSelectComponentOption("Daily Quotes", "quote", "Get pinged for Daily Quotes", member.Roles.Any(x => x.Id == 765622563338453023), new DiscordComponentEmoji("💬")),
                        new DiscordSelectComponentOption("Daily Question", "question", "Get pinged for Daily Questions", member.Roles.Any(x => x.Id == 741342066021367938), new DiscordComponentEmoji("❓")),
                        new DiscordSelectComponentOption("Daily Astronomy Pictures", "dap", "Get pinged for Daily Astronomy Pictures", member.Roles.Any(x => x.Id == 848307821703200828), new DiscordComponentEmoji("❗")),
                        new DiscordSelectComponentOption("Debate", "debate", "Get access to the Debate channel and voice channel", member.Roles.Any(x => x.Id == 718162129609556121), new DiscordComponentEmoji("📜")),
                        new DiscordSelectComponentOption("Venting", "venting", "Get access to the Venting channel", member.Roles.Any(x => x.Id == 812755886413971499), new DiscordComponentEmoji("💢"))
                    };


                    if (member.Roles.Any(x => greetingsEnabledRoleIds.Contains(x.Id)))
                        options.Add(new DiscordSelectComponentOption("Greetings", "greetings", "Gives you permissions to send messages in Greetings", member.Roles.Any(x => x.Id == 1014280593929928797), new DiscordComponentEmoji("👋")));

                    builder.AddComponents(new DiscordSelectComponent("roleme_misc_dropdown", "Select your roles", options, false, 0, options.Count));

                    await e.Interaction.EditOriginalResponseAsync(builder);
                }
                else if (e.Id == "roleme_18+")
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var member = await e.Guild.GetMemberAsync(e.User.Id);

                    var message = new DiscordWebhookBuilder();
                    if (member.Roles.Any(x => x.Id == 1081151306351251521)) //18+ role
                    {
                        var anyProtectedRoles = member.Roles.Any(x => ageRequiredRoleIds.Any(y => y == x.Id));
                        if (anyProtectedRoles)
                            message.WithContent("You already have the 18+ role, if you want to get rid of it you first have to get rid of the following roles:\n" + string.Join('\n', member.Roles.Where(x => ageRequiredRoleIds.Any(y => y == x.Id)).Select(x => x.Mention)));
                        else
                            message.WithContent("You already have the 18+ role, if you want to get rid of it press the button below.");
                        message.AddComponents(new List<DiscordComponent>()
                        {
                            new DiscordButtonComponent(ButtonStyle.Danger, "roleme_18+_revoke", "Remove my 18+ role", anyProtectedRoles, new DiscordComponentEmoji("🔞"))
                        });
                    }
                    else
                    {
                        message.WithContent("All channels tagged as NSFW due to their content (for example guns) are gated behind the 18+ role.\n" +
                            "BY SELECTING THIS ROLE YOU CONFIRM THAT YOU ARE 18 YEARS OR OLDER.\n" +
                            "If you select the 18+ Role but confirm that you are under 18 in any chat,\n" +
                            "unfortunately this will require us to ban you immediately due to breaking Discord TOS.\n" +
                            "There will be no warnings or chances to argue your case in this situation!");
                        message.AddComponents(new List<DiscordComponent>()
                        {
                            new DiscordButtonComponent(ButtonStyle.Danger, "roleme_18+_confirm", "Yes i am 18+ years old", emoji: new DiscordComponentEmoji("🔞"))
                        });
                    }


                    await e.Interaction.EditOriginalResponseAsync(message);
                }
                else if (e.Id == "roleme_18+_confirm")
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var member = await e.Guild.GetMemberAsync(e.User.Id);
                    if (!member.Roles.Any(x => x.Id == 1081151306351251521))//18+ role
                    {
                        await member.GrantRoleAsync(e.Guild.GetRole(1081151306351251521));//18+ role

                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully added the 18+ role for you."));
                    }
                    else
                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("You already have that role."));
                }
                else if (e.Id == "roleme_18+_revoke")
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var member = await e.Guild.GetMemberAsync(e.User.Id);

                    if (member.Roles.Any(x => x.Id == 1081151306351251521))//18+ role
                    {
                        await member.RevokeRoleAsync(e.Guild.GetRole(1081151306351251521));//18+ role

                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully removed the 18+ role for you."));
                    }
                    else
                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("You dont have that role."));
                }
                else if (e.Id == "roleme_games_dropdown")
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var member = await e.Guild.GetMemberAsync(e.User.Id);

                    if (!member.Roles.Any(x => x.Id == 1081151306351251521))//18+ role
                    {
                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("I'm sorry, but i cant grant you any of these roles, please confirm you are 18 or older first."));
                        return;
                    }

                    var currentRoleIds = gamesRoleIds.Where(x => member.Roles.Any(y => y.Id == x));
                    var newRoleIds = new List<ulong>();
                    foreach (var selection in e.Values)
                    {
                        ulong roleid = selection switch
                        {
                            "airships" => 978954327907532811,
                            "ftd" => 701454853095817316,
                            "dnd" => 1046124300761043004,
                            "minecraft" => 713367380574732319,
                            "reassembly" => 850029252812210207,
                            "stellaris" => 701454772900855819,
                            "tt" => 1014261454624542810,
                            "wh40k" => 766322672321560628,
                            _ => 0
                        };
                        if (roleid != 0)
                            newRoleIds.Add(roleid);
                    }

                    foreach (var roleid in currentRoleIds.Where(x => !newRoleIds.Contains(x)))
                        await member.RevokeRoleAsync(e.Guild.GetRole(roleid));
                    foreach (var roleid in newRoleIds.Where(x => !currentRoleIds.Contains(x)))
                        await member.GrantRoleAsync(e.Guild.GetRole(roleid));

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully changed your roles!"));
                }
                else if (e.Id == "roleme_misc_dropdown")
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var member = await e.Guild.GetMemberAsync(e.User.Id);

                    var has18role = member.Roles.Any(x => x.Id == 1081151306351251521);//18+ role

                    var currentRoleIds = miscRoleIds.Where(x => member.Roles.Any(y => y.Id == x));
                    var newRoleIds = new List<ulong>();
                    var illegalRoleIds = new List<ulong>();
                    foreach (var selection in e.Values)
                    {
                        ulong roleid = selection switch
                        {
                            "ads" => 794975835235942470,
                            "art" => 767039219403063307,
                            "quote" => 765622563338453023,
                            "question" => 741342066021367938,
                            "dap" => 848307821703200828,
                            "debate" => 718162129609556121,
                            "venting" => 812755886413971499,
                            "greetings" => 1014280593929928797,
                            _ => 0
                        };
                        if (roleid != 0 && (!ageRequiredRoleIds.Any(x => x == roleid) || has18role))
                            newRoleIds.Add(roleid);
                        else if (roleid != 0)
                            illegalRoleIds.Add(roleid);
                    }

                    foreach (var roleid in currentRoleIds.Where(x => !newRoleIds.Contains(x)))
                        await member.RevokeRoleAsync(e.Guild.GetRole(roleid));
                    foreach (var roleid in newRoleIds.Where(x => !currentRoleIds.Contains(x)))
                        await member.GrantRoleAsync(e.Guild.GetRole(roleid));

                    if (illegalRoleIds.Any())
                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Sorry, but I cannot grant you the roles\n" + string.Join('\n', illegalRoleIds.Select(x => e.Guild.GetRole(x).Mention)) +
                            "\nPlease confirm you are 18 or older first.\nAll other selected roles have been added."));
                    else
                        await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully changed your roles!"));
                }
            });
            return Task.CompletedTask;
        }
    }
}
