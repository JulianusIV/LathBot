namespace LathBotBack.Models
{
    public class Audit
    {
        public int Mod { get; set; }
        public int Warns { get; set; }
        public int Pardons { get; set; }
        public int Mutes { get; set; }
        public int Unmutes { get; set; }
        public int Kicks { get; set; }
        public int Bans { get; set; }
        public int Timeouts { get; set; }

        public Audit()
        {

        }

        public Audit(int modId)
        {
            Mod = modId;
            Warns = 0;
            Pardons = 0;
            Mutes = 0;
            Unmutes = 0;
            Kicks = 0;
            Bans = 0;
            Timeouts = 0;
        }
    }
}
