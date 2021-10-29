using System;

namespace LathBotBack.Models
{
	public class Mute
	{
		public int Id { get; set; }
		public int User { get; set; }
		public int Mod { get; set; }
		public DateTime Timestamp { get; set; }
		public int Duration { get; set; }
		//public DateTime LastCheck { get; set; }
	}
}
