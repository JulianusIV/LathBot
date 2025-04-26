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
        private static readonly ulong[] greetingsEnabledRoleIds =
        [
            748533112404705281, //vet
            819296501451980820, //true
            707915540085342248, //godly
            759456739669704784, //ungodly
            749576790929965136, //booster
            702925135568437270, //booster
            822957506674163792  //mod team
        ];

        private static readonly ulong[] gamesRoleIds =
        [
            1046124300761043004, //dnd
            701454853095817316,  //ftd
            713367380574732319,  //minecraft
            850029252812210207,  //reassembly
            1104070430811226163, //rpg
            701454772900855819,  //stellaris
            1014261454624542810, //tt
            766322672321560628   //wh40k
        ];

        private static readonly ulong[] colorRoleIds =
        [
            759782089830301706, //purple
            759439195478949928, //light blue
            964262185561899038, //betta blue
            822500283208564746, //pastel pink
            783133133053886484, //green
            772955935550210058, //blue
            772915100574941294, //classic lathrixian pink
            759439186687557662, //blood red
            759786932888404039, //gray
            772957722924154900, //gold
            783062295647485952, //brown
            784859772838608916, //silver
            788177276155068466, //orange
            898738889601208340, //darker grey
            759439706634584064 //ugly green
        ];

        public static readonly ulong[] miscRoleIds =
        [
            794975835235942470, //ads
            767039219403063307, //art
            765622563338453023, //dquotes
            741342066021367938, //dquestions
            848307821703200828, //dap
            718162129609556121, //debate
            812755886413971499, //vent
            1014280593929928797 //greet
        ];

        internal static Task ComponentTriggered(DiscordClient _1, ComponentInteractionCreatedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Id == "roleme_games")
                {
                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var builder = new DiscordWebhookBuilder()
                        .WithContent("Select your game roles");

                    var member = await e.Guild.GetMemberAsync(e.User.Id);
                    var options = new List<DiscordSelectComponentOption>()
                    {
                        new("Dungeons and Dragons", "dnd", "Get access to the Dungeons and Dragons game channel and vc", member.Roles.Any(x => x.Id == 1046124300761043004), new DiscordComponentEmoji("🐉")),
                        new("From the Depths", "ftd", "Get access to the From the Depths game channel", member.Roles.Any(x => x.Id == 701454853095817316), new DiscordComponentEmoji("⛵")),
                        new("Minecraft", "minecraft", "Get access to the Minecraft game channel", member.Roles.Any(x => x.Id == 713367380574732319), new DiscordComponentEmoji("⛏️")),
                        new("Reassembly", "reassembly", "Get access to the Reassembly game channel", member.Roles.Any(x => x.Id == 701454772900855819), new DiscordComponentEmoji("👾")),
                        new("RPG-Bot", "rpg", "Get access to the Isekaid RPG Bot game channel", member.Roles.Any(x => x.Id == 1104070430811226163), new DiscordComponentEmoji("⚔️")),
                        new("Stellairs", "stellaris", "Get access to the Stellairs game channel", member.Roles.Any(x => x.Id == 701454772900855819), new DiscordComponentEmoji("⭐")),
                        new("Terra Tech", "tt", "Get access to the Terra Tech game channel", member.Roles.Any(x => x.Id == 1014261454624542810), new DiscordComponentEmoji("🤖")),
                        new("Warhammer 40k", "wh40k", "Get access to the Warhammer 40k game channel", member.Roles.Any(x => x.Id == 766322672321560628), new DiscordComponentEmoji("🔨"))
                    };

                    builder.AddActionRowComponent(new DiscordSelectComponent("roleme_games_dropdown", "Select your roles", options, false, 0, options.Count));

                    await e.Interaction.EditOriginalResponseAsync(builder);
                }
                else if (e.Id == "roleme_color")
                {
                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var builder = new DiscordWebhookBuilder()
                        .WithContent("Select your color roles");
                    var member = await e.Guild.GetMemberAsync(e.User.Id);
                    var options = new List<DiscordSelectComponentOption>()
                    {
                        new("Purple", "purple", "Get the Purple color role", member.Roles.Any(x => x.Id == 759782089830301706), new DiscordComponentEmoji("🟣")),
                        new("Light Blue", "light_blue", "Get the Light Blue color role", member.Roles.Any(x => x.Id == 759439195478949928), new DiscordComponentEmoji("🔵")),
                        new("Betta Blue", "betta_blue", "Get the Betta Blue color role", member.Roles.Any(x => x.Id == 964262185561899038), new DiscordComponentEmoji("🔵")),
                        new("Pastel Pink", "pastel_pink", "Get the Pastel Pink color role", member.Roles.Any(x => x.Id == 822500283208564746), new DiscordComponentEmoji("🩷")),
                        new("Green", "green", "Get the Green color role", member.Roles.Any(x => x.Id == 783133133053886484), new DiscordComponentEmoji("🟢")),
                        new("Blue", "blue", "Get the Blue color role", member.Roles.Any(x => x.Id == 772955935550210058), new DiscordComponentEmoji("🔵")),
                        new("Classic Lathrixian Pink", "classic_lathrixian_pink", "Get the Classic Lathrixian Pink color role", member.Roles.Any(x => x.Id == 772915100574941294), new DiscordComponentEmoji("🧓")),
                        new("Blood Red", "blood_red", "Get the Blood Red color role", member.Roles.Any(x => x.Id == 759439186687557662), new DiscordComponentEmoji("🔴")),
                        new("Gray", "gray", "Get the Gray color role", member.Roles.Any(x => x.Id == 759786932888404039), new DiscordComponentEmoji("🩶")),
                        new("Gold", "gold", "Get the Gold color role", member.Roles.Any(x => x.Id == 772957722924154900), new DiscordComponentEmoji("🟡")),
                        new("Brown", "brown", "Get the Brown color role", member.Roles.Any(x => x.Id == 783062295647485952), new DiscordComponentEmoji("💩")),
                        new("Silver", "silver", "Get the Silver color role", member.Roles.Any(x => x.Id == 784859772838608916), new DiscordComponentEmoji("🩶")),
                        new("Orange", "orange", "Get the Orange color role", member.Roles.Any(x => x.Id == 788177276155068466), new DiscordComponentEmoji("🟠")),
                        new("Darker Grey", "darker_grey", "Get the Darker Grey color role", member.Roles.Any(x => x.Id == 898738889601208340), new DiscordComponentEmoji("🩶")),
                        new("Ugly Green", "ugly_green", "Get the Ugly Green color role", member.Roles.Any(x => x.Id == 759439706634584064), new DiscordComponentEmoji("🟢"))
                    };

                    builder.AddActionRowComponent(new DiscordSelectComponent("roleme_color_dropdown", "Select your roles", options, false, 0, options.Count));

                    await e.Interaction.EditOriginalResponseAsync(builder);
                }
                else if (e.Id == "roleme_misc")
                {
                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var builder = new DiscordWebhookBuilder()
                        .WithContent("Select your misc roles");

                    var member = await e.Guild.GetMemberAsync(e.User.Id);
                    var options = new List<DiscordSelectComponentOption>()
                    {
                        new("Advertisements", "ads", "Get pinged for new advertisements", member.Roles.Any(x => x.Id == 794975835235942470), new DiscordComponentEmoji("📢")),
                        new("Art and Photography", "art", "Get access to the Art and the Photography channels", member.Roles.Any(x => x.Id == 767039219403063307), new DiscordComponentEmoji("🎨")),
                        new("Daily Quotes", "quote", "Get pinged for Daily Quotes", member.Roles.Any(x => x.Id == 765622563338453023), new DiscordComponentEmoji("💬")),
                        new("Daily Question", "question", "Get pinged for Daily Questions", member.Roles.Any(x => x.Id == 741342066021367938), new DiscordComponentEmoji("❓")),
                        new("Daily Astronomy Pictures", "dap", "Get pinged for Daily Astronomy Pictures", member.Roles.Any(x => x.Id == 848307821703200828), new DiscordComponentEmoji("❗")),
                        new("Debate", "debate", "Get access to the Debate channel and voice channel", member.Roles.Any(x => x.Id == 718162129609556121), new DiscordComponentEmoji("📜")),
                        new("Venting", "venting", "Get access to the Venting channel", member.Roles.Any(x => x.Id == 812755886413971499), new DiscordComponentEmoji("💢"))
                    };


                    if (member.Roles.Any(x => greetingsEnabledRoleIds.Contains(x.Id)))
                        options.Add(new DiscordSelectComponentOption("Greetings", "greetings", "Gives you permissions to send messages in Greetings", member.Roles.Any(x => x.Id == 1014280593929928797), new DiscordComponentEmoji("👋")));

                    builder.AddActionRowComponent(new DiscordSelectComponent("roleme_misc_dropdown", "Select your roles", options, false, 0, options.Count));

                    await e.Interaction.EditOriginalResponseAsync(builder);
                }
                else if (e.Id == "roleme_games_dropdown")
                {
                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var member = await e.Guild.GetMemberAsync(e.User.Id);

                    var currentRoleIds = gamesRoleIds.Where(x => member.Roles.Any(y => y.Id == x));
                    var newRoleIds = new List<ulong>();
                    foreach (var selection in e.Values)
                    {
                        ulong roleid = selection switch
                        {
                            "ftd" => 701454853095817316,
                            "dnd" => 1046124300761043004,
                            "minecraft" => 713367380574732319,
                            "reassembly" => 850029252812210207,
                            "rpg" => 1104070430811226163,
                            "stellaris" => 701454772900855819,
                            "tt" => 1014261454624542810,
                            "wh40k" => 766322672321560628,
                            _ => 0
                        };
                        if (roleid != 0)
                            newRoleIds.Add(roleid);
                    }

                    foreach (var roleid in currentRoleIds.Where(x => !newRoleIds.Contains(x)))
                        await member.RevokeRoleAsync(await e.Guild.GetRoleAsync(roleid));
                    foreach (var roleid in newRoleIds.Where(x => !currentRoleIds.Contains(x)))
                        await member.GrantRoleAsync(await e.Guild.GetRoleAsync(roleid));

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully changed your roles!"));
                }
                else if (e.Id == "roleme_color_dropdown")
                {
                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var member = await e.Guild.GetMemberAsync(e.User.Id);

                    var currentRoleIds = colorRoleIds.Where(x => member.Roles.Any(y => y.Id == x));
                    var newRoleIds = new List<ulong>();
                    foreach (var selection in e.Values)
                    {
                        ulong roleid = selection switch
                        {
                            "purple" => 759782089830301706,
                            "light_blue" => 759439195478949928,
                            "betta_blue" => 964262185561899038,
                            "pastel_pink" => 822500283208564746,
                            "green" => 783133133053886484,
                            "blue" => 772955935550210058,
                            "classic_lathrixian_pink" => 772915100574941294,
                            "blood_red" => 759439186687557662,
                            "gray" => 759786932888404039,
                            "gold" => 772957722924154900,
                            "brown" => 783062295647485952,
                            "silver" => 784859772838608916,
                            "orange" => 788177276155068466,
                            "darker_grey" => 898738889601208340,
                            "ugly_green" => 759439706634584064,
                            _ => 0
                        };
                        if (roleid != 0)
                            newRoleIds.Add(roleid);
                    }

                    foreach (var roleid in currentRoleIds.Where(x => !newRoleIds.Contains(x)))
                        await member.RevokeRoleAsync(await e.Guild.GetRoleAsync(roleid));
                    foreach (var roleid in newRoleIds.Where(x => !currentRoleIds.Contains(x)))
                        await member.GrantRoleAsync(await e.Guild.GetRoleAsync(roleid));

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully changed your roles!"));
                }
                else if (e.Id == "roleme_misc_dropdown")
                {
                    await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.DeferredChannelMessageWithSource, new DiscordInteractionResponseBuilder().AsEphemeral());

                    var member = await e.Guild.GetMemberAsync(e.User.Id);

                    var currentRoleIds = miscRoleIds.Where(x => member.Roles.Any(y => y.Id == x));
                    var newRoleIds = new List<ulong>();
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
                        if (roleid != 0)
                            newRoleIds.Add(roleid);
                    }

                    foreach (var roleid in currentRoleIds.Where(x => !newRoleIds.Contains(x)))
                        await member.RevokeRoleAsync(await e.Guild.GetRoleAsync(roleid));
                    foreach (var roleid in newRoleIds.Where(x => !currentRoleIds.Contains(x)))
                        await member.GrantRoleAsync(await e.Guild.GetRoleAsync(roleid));

                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent("Successfully changed your roles!"));
                }
            });
            return Task.CompletedTask;
        }
    }
}
