using System;

namespace LathBotBack.Models
{
	public class User
	{
		public int ID { get; set; }
		public ulong DcID { get; set; }
        public bool EmbedBanned { get; set; }
        public DateTime? LastPunish { get; set; }
    }
}
