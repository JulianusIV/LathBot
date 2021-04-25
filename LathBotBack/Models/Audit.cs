using System;
using System.Collections.Generic;
using System.Text;

namespace LathBotBack.Models
{
	public class Audit
	{
		public int Mod{ get; set; }
		public int Warns { get; set; }
		public int Pardons { get; set; }
		public int Mutes { get; set; }
		public int Unmutes { get; set; }
		public int Kicks { get; set; }
		public int Bans { get; set; }
	}
}
