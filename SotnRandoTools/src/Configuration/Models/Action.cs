using System;

namespace SotnRandoTools.Configuration.Models
{
	public class Action
	{
		public Action()
		{
			this.Enabled = true;
		}
		
		public string Name { get; set; }
		public string Command { get; set; }
		public short Meter { get; set; }
		public bool AutoMayhemEnabled { get; set; }
		public bool Enabled { get; set; }
		public int Type { get; set; }
		public bool StartsOnCooldown { get; set; }
		public TimeSpan Cooldown { get; set; }
		public DateTime? LastUsedAt { get; set; }
		public string AlertPath { get; set; }
		public TimeSpan Duration { get; set; }
		public TimeSpan Interval { get; set; }

		public void setCooldown()
		{
			LastUsedAt = DateTime.Now;
		}
		public bool IsOnCooldown()
		{
			if (LastUsedAt is null)
			{
				return false;
			}
			else if (DateTime.Now - LastUsedAt > Cooldown)
			{
				return false;
			}

			return true;
		}
	}
}
