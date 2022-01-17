using System.Collections.Generic;
using System.Drawing;
using SotnRandoTools.Configuration.Models;
using SotnRandoTools.Constants;

namespace SotnRandoTools.Configuration
{
	public class KhaosConfig
	{
		public KhaosConfig()
		{
			Alerts = true;
			ControlPannelQueueActions = true;
			Volume = 2;
			NamesFilePath = Paths.NamesFilePath;
			BotApiKey = "";
			WeakenFactor = 0.4F;
			CrippleFactor = 0.8F;
			HasteFactor = 3.2F;
			ThirstDrainPerSecond = 1;
			PandoraTrigger = 1000;
			PandoraMinItems = 15;
			PandoraMaxItems = 32;
			MeterOnReset = 20;
			QueueInterval = new System.TimeSpan(0, 0, 31);
			RomhackMode = false;
			DynamicInterval = true;
			KeepVladRelics = true;
			Actions = new List<Action>
			{
				new Action{Name="Khaos Status", Enabled = true, Meter = 3, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Gamble", Enabled = true, Meter = 2, AlertPath = Paths.LibrarianThankYouSound},
				new Action{Name="Khaos Burst", Enabled = true, Meter = 0, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Stats", Enabled = true, Meter = 8, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Equipment", Enabled = true, Meter = 10, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Relics", Enabled = true, Meter = 12, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Pandora's Box", Enabled = true, Meter = 12, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Blood Mana", Enabled = true, Meter = 3, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Cripple", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.SlowWhatSound},
				new Action{Name="Slam", Enabled = true, Meter = 4, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Subweapons Only", Enabled = true, Meter = 6, AlertPath = Paths.RichterLaughSound, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Thirst", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Respawn Bosses", Enabled = true, Meter = 6, AlertPath = Paths.HohoSound},
				new Action{Name="HnK", Enabled = true, Meter = 8, AlertPath = Paths.AlreadyDeadSound},
				new Action{Name="Khaos Horde", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 30), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.DracLaughSound},
				new Action{Name="Endurance", Enabled = true, Meter = 10, AlertPath = Paths.DeathLaughAlternateSound},
				new Action{Name="Weaken", Enabled = true, Meter = 12, AlertPath = Paths.DieSound},
				new Action{Name="Bankrupt", Enabled = true, Meter = 12, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Light Help", Enabled = true, Meter = 2, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Haste", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Vampire", Enabled = true,  Meter = 4, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Medium Help", Enabled = true, Meter = 4, AlertPath = Paths.FairyPotionSound},
				new Action{Name="ZA WARUDO", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.ZaWarudoSound},
				new Action{Name="Battle Orders", Enabled = true,  Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.BattleOrdersSound},
				new Action{Name="Magician", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Guilty Gear", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DragonInstallSound},
				new Action{Name="Heavy Help", Enabled = true, Meter = 8, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Melty Blood", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.MeltySound},
				new Action{Name="Lord", Enabled = true,  Meter = 10, Duration = new System.TimeSpan(0, 1, 30), AlertPath = Paths.SwordBroSound},
				new Action{Name="Four Beasts", Enabled = true,  Meter = 10, Duration = new System.TimeSpan(0, 1, 0)},	
			};
			LightHelpItemRewards = new string[]
			{
				"Herald shield",
				"Dark Shield",
				"Str. potion",
				"Attack potion",
				"Shield potion",
				"Meal ticket",
				"Holbein dagger",
				"Gram",
				"Luminus", 
				"Chakram",
				"Runesword",
				"Heaven sword",
				"Stone mask",
				"Wizard hat",
				"Platinum mail",
				"Diamond plate",
				"Fire mail",
				"Mirror cuirass",
				"Brilliant mail",
				"Joseph's cloak",
				"Royal cloak",
				"Twilight cloak",
				"Onyx",
				"Garnet",
				"Nauglamir",
				"Talisman",
				"Mystic pendant",
				"Gauntlet",
				"Moonstone"
			};
			MediumHelpItemRewards = new string[]
			{
				"Fire shield",
				"Iron shield",
				"Medusa shield",
				"Alucard shield",
				"Buffalo star",
				"Cross shuriken",
				"Shield rod",
				"Flame star",
				"Firebrand",
				"Thunderbrand",
				"Icebrand",
				"Obsidian sword",
				"Marsil",
				"Holy sword",
				"Mourneblade",
				"Osafune katana",
				"Topaz circlet",
				"Beryl circlet",
				"Ruby circlet",
				"Fury plate",
				"Elixir",
				"Library card",
				"Diamond",
				"Opal",
				"Mystic pendant",
				"Ring of Feanor",
				"King's stone",
				"Manna prism",
				"Dark armor",
				"Ring of Ares"
			};
			HeavyHelpItemRewards = new string[]
			{
				"Mablung Sword",
				"Masamune",
				"Fist of Tulkas",
				"Gurthang",
				"Alucard sword",
				"Vorpal blade",
				"Crissaegirm",
				"Yasatsuna",
				"Dragon helm",
				"Holy glasses",
				"Spike Breaker",
				"Dracula tunic",
				"God's Garb",
				"Ring of Varda",
				"Duplicator",
				"Covenant stone",
				"Gold Ring",
				"Silver Ring"
			};
		}
		public Point Location { get; set; }
		public bool Alerts { get; set; }
		public bool RomhackMode { get; set; }
		public bool ControlPannelQueueActions { get; set; }
		public int Volume { get; set; }
		public string NamesFilePath { get; set; }
		public string BotApiKey { get; set; }
		public List<Action> Actions { get; set; }
		public float WeakenFactor { get; set; }
		public float CrippleFactor { get; set; }
		public float HasteFactor { get; set; }
		public uint ThirstDrainPerSecond { get; set; }
		public int MeterOnReset { get; set; }
		public int PandoraMinItems { get; set; }
		public int PandoraMaxItems { get; set; }
		public int PandoraTrigger { get; set; }
		public System.TimeSpan QueueInterval { get; set; }
		public bool DynamicInterval { get; set; }
		public bool KeepVladRelics { get; set; }
		public string[] LightHelpItemRewards { get; set; }
		public string[] MediumHelpItemRewards { get; set; }
		public string[] HeavyHelpItemRewards { get; set; }
	}
}
