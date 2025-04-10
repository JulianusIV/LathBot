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
            this.Mod = modId;
            this.Warns = 0;
            this.Pardons = 0;
            this.Mutes = 0;
            this.Unmutes = 0;
            this.Kicks = 0;
            this.Bans = 0;
            this.Timeouts = 0;
        }
    }
}
