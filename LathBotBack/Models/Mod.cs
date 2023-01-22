namespace LathBotBack.Models
{
    public class Mod
    {
        public int Id { get; set; }
        public int DbId { get; set; }
        public string Timezone { get; set; }
        public byte[] TwoFAKey { get; set; }
        public string TwoFAKeySalt { get; set; }
    }
}
