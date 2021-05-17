using System;
using System.Collections.Generic;
using System.Text;

namespace LathBotBack.Models
{
	public class Mute
	{
		public int Id { get; set; }
		public int User { get; set; }
		public DateTime Timestamp { get; set; }
		public int Duration { get; set; }
	}
}
