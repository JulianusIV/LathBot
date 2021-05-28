using DSharpPlus;
using LathBotBack.Base;
using LathBotBack.Models;

namespace LathBotBack.Services
{
	public class RuleService : BaseService<RuleService>
	{
		public static Rule[] rules;

		public override void Init(DiscordClient client)
		{
			rules = new Rule[14]
			{
				new Rule(1, "Follow the discord [ToS](https://discord.com/terms) and [community guidelines](https://discord.com/guidelines).", 1, 15),
				new Rule(2, "Do not ping Lathrix", 1, 1),
				new Rule(3, "Use appropriate channels and follow the channel rules (description/pins).", 1, 5),
				new Rule(4, "Be honest with everyone.", 1, 1),
				new Rule(5, "Respect everyone", 1, 5),
				new Rule(6, "Don't post NSFW content.", 3, 15),
				new Rule(7, "Conversations about current Religious, Political, Military or any other \"touchy\" subject will not be tolerated. Some jokes you find funny others may not. This does not fully apply to the \"Debate\" channel.", 1, 5),
				new Rule(8, "Don't link pirate/hack/other illegal sites.", 3, 5),
				new Rule(9, "Don't ask for roles", 1, 2),
				new Rule(10, "Don't mini-mod", 1, 2),
				new Rule(11, "Don't use macros", 1, 3),
				new Rule(12, "Don't share personal information about others", 1, 10),
				new Rule(13, "Holding bias as a staff member is prohibited, or if you are a member who is trying to make staff bias. This will be known as corruption and should be reported to Chewybaca and/or a member of the Senate immediately.", 3, 10),
				new Rule(0, "", 1, 10)
			};
		}
	}
}
