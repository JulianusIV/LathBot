namespace LathBotBack.Models
{
	public class Rule
	{
        public int RuleNum { get; set; }
        public string RuleText { get; set; }
        public int MinPoints { get; set; }
        public int MaxPoints { get; set; }

        public Rule(int ruleNum, string ruleText, int minPoints, int maxPoints)
        {
            RuleNum = ruleNum;
            RuleText = ruleText;
            MinPoints = minPoints;
            MaxPoints = maxPoints;
        }
    }
}
