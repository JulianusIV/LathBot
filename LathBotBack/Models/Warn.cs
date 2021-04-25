using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace LathBotBack.Models
{
	public class Warn
	{
		public int ID { get; set; }
		public int User { get; set; }
		public int Mod { get; set; }
		public string Reason { get; set; }
		public int Number { get; set; }
		public int Level { get; set; }
		public DateTime Time { get; set; }
	}
}
