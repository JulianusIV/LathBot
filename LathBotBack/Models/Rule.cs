namespace LathBotBack.Models
{
    public class Rule(int ruleNum, string ruleText, int minPoints, int maxPoints, string shortDesc)
    {
        public int RuleNum { get; set; } = ruleNum;
        public string RuleText { get; set; } = ruleText;
        public int MinPoints { get; set; } = minPoints;
        public int MaxPoints { get; set; } = maxPoints;
        public string ShortDesc { get; set; } = shortDesc;
    }
}
