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
			#region General Tab
			Alerts = true;
			ControlPannelQueueActions = true;
			Volume = 5;
			NamesFilePath = Paths.NamesFilePath;
			BotApiKey = "";

			QueueInterval = new System.TimeSpan(0, 0, 10);
			MeterOnReset = 25;
			ContinuousWingsmash = false;
			DynamicInterval = true;
			RomhackMode = false;
			BoostFamiliars = true;
			GalamothIsRepositioned = true;
			GalamothDefNerf = true;
			#endregion

			#region Settings Tab
			NeutralMinLevel = 1;
			NeutralStartLevel = 1;
			NeutralMaxLevel = 3;
			AllowNeutralLevelReset = true;

			EnforceMinStats = true;
			RestrictedRelicSwap = true;
			KeepVladRelics = true;

			StatsDownFactor = 0.4F;
			UnderwaterFactor = 0.8F;
			SpeedFactor = 3.2F;
			RegenGainPerSecond = 1;
			PandoraTrigger = 1200;
			PandemoniumMinItems = 16;
			PandemoniumMaxItems = 32;
			#endregion

			#region Difficulty Tab
			CloneBossHPModifier = 2;
			CloneBossDMGModifier = 2;
			SingleBossHPModifier = 2;
			SingleBossDMGModifier = 2;
			GalamothBossHPModifier = 2;
			GalamothBossDMGModifier = 2;
			ShaftOrbHPModifier = 2;
			SuperBossHPModifier = 2;
			SuperBossDMGModifier = 2;
			AmbushHPModifier = 2;
			AmbushDMGModifier = 2;
			SuperAmbushHPModifier = 2;
			SuperAmbushDMGModifier = 2;
			BlessingModifier = 2;
			CurseModifier = 2;
			#endregion

			#region Legacy Defaults
			WeakenFactor = 0.5F;
			CrippleFactor = 0.8F;
			HasteFactor = 3.2F;
			ThirstDrainPerSecond = 1;
			PandoraTrigger = 1000;
			PandoraMinItems = 16;
			PandoraMaxItems = 32;
			#endregion
			Actions = new List<Action>
			{
				//Mayhem
				new Action{Name="Pain Trade", Enabled = true, Meter = 4, AlertPath = Paths.LibrarianThankYouSound},
				new Action{Name="Max Mayhem", Enabled = true, Meter = 0, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Hearts Only", Enabled = true, Meter = 6,  Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.RichterLaughSound},
				new Action{Name="Unarmed", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.AlucardWhatSound },
				new Action{Name="Turbo Mode", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.AlucardWhatSound },
				new Action{Name="Rushdown", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Swap Stats", Enabled = true, Meter = 8, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Swap Equipment", Enabled = true, Meter = 8, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Swap Relics", Enabled = true, Meter = 12, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Pandemonium", Enabled = true, Meter = 12, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Minor Trap", Enabled = true, Meter = 2, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Slam", Enabled = true, Meter = 3, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Slam Jam", Enabled = true, Meter = 3, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="HP for MP", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Underwater", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.SlowWhatSound},
				new Action{Name="Hex", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Get Juggled", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.AlreadyDeadSound},
				new Action{Name="Ambush", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 30), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.DracLaughSound},
				new Action{Name="Tough Bosses", Enabled = true, Meter = 10, AlertPath = Paths.DeathLaughAlternateSound},
				new Action{Name="Stats Down", Enabled = true, Meter = 12, AlertPath = Paths.DieSound},
				new Action{Name="Confiscate", Enabled = true, Meter = 12, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Minor Boon", Enabled = true, Meter = 2, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Speed", Enabled = true, Meter = 3, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Regen", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Moderate Boon", Enabled = true, Meter = 4, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Time Stop", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.ZaWarudoSound},
				new Action{Name="Spellcaster", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Face Tank", Enabled = true,  Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.BattleOrdersSound},
				new Action{Name="Extra Range", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.MeltySound},
				new Action{Name="Summoner", Enabled = true,  Meter = 8, Duration = new System.TimeSpan(0, 1, 30), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.SwordBroSound},
				new Action{Name="Major Boon", Enabled = true, Meter = 10, AlertPath = Paths.FairyPotionSound},
				//Legacy
				new Action{Name="Khaos Status", Enabled = true, Meter = 3, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Gamble", Enabled = true, Meter = 2, AlertPath = Paths.LibrarianThankYouSound},
				new Action{Name="Khaos Burst", Enabled = true, Meter = 0, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Stats", Enabled = true, Meter = 8, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Equipment", Enabled = true, Meter = 10, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Khaos Relics", Enabled = true, Meter = 12, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Pandora's Box", Enabled = true, Meter = 12, AlertPath = Paths.AlucardWhatSound},
				new Action{Name="Blood Mana", Enabled = true, Meter = 3, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Cripple", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.SlowWhatSound},
				new Action{Name="Subweapons Only", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.RichterLaughSound},
				new Action{Name="Honest Gamer", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.RichterLaughSound,},
				new Action{Name="Thirst", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DeathLaughSound},
				new Action{Name="Respawn Bosses", Enabled = true, Meter = 6, AlertPath = Paths.HohoSound},
				new Action{Name="HnK", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.AlreadyDeadSound},
				new Action{Name="Khaos Horde", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 30), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.DracLaughSound},
				new Action{Name="Endurance", Enabled = true, Meter = 10, AlertPath = Paths.DeathLaughAlternateSound},
				new Action{Name="Weaken", Enabled = true, Meter = 12, AlertPath = Paths.DieSound},
				new Action{Name="Bankrupt", Enabled = true, Meter = 12, AlertPath = Paths.DeathLaughSound},
				new Action{Name="Light Help", Enabled = true, Meter = 2, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Haste", Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Vampire", Enabled = true,  Meter = 4, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Medium Help", Enabled = true, Meter = 4, AlertPath = Paths.FairyPotionSound},
				new Action{Name="ZaWarudo", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.ZaWarudoSound},
				new Action{Name="Battle Orders", Enabled = true,  Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.BattleOrdersSound},
				new Action{Name="Magician", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0)},
				new Action{Name="Guilty Gear", Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.DragonInstallSound},
				new Action{Name="Heavy Help", Enabled = true, Meter = 8, AlertPath = Paths.FairyPotionSound},
				new Action{Name="Melty Blood", Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.MeltySound},
				new Action{Name="Lord", Enabled = true,  Meter = 10, Duration = new System.TimeSpan(0, 1, 30), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.SwordBroSound},
				new Action{Name="Four Beasts", Enabled = true,  Meter = 10, Duration = new System.TimeSpan(0, 1, 0)},	
			};

			painTradeItemRewards = new string[]
			{
				"Herald shield",
				"Dark shield",
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
			moderateBoonItemRewards = new string[]
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
			majorBoonItemRewards = new string[]
			{
				"Mablung Sword",
				"Masamune",
				"Fist of Tulkas",
				"Gurthang",
				"Alucard sword",
				"Vorpal blade",
				"Crissaegrim",
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
			LightHelpItemRewards = new string[]
{
				"Leather shield",
				"Shaman shield",
				"Pot Roast",
				"Holbein dagger",
				"Heart Refresh",
				"Str. potion",
				"Attack potion",
				"Wizard hat",
				"Mirror cuirass",
				"Brilliant mail",
				"Aquamarine",
				"Lapis lazuli",
				"Mystic pendant",
				"Gauntlet"
};
			MediumHelpItemRewards = new string[]
			{
				"Fire shield",
				"Iron shield",
				"Medusa shield",
				"Alucard shield",
				"Shield rod",
				"Buffalo star",
				"Flame star",
				"Obsidian sword",
				"Marsil",
				"Elixir",
				"Holy sword",
				"Mourneblade",
				"Fury plate",
				"Twilight cloak",
				"Library card",
				"Diamond",
				"Onyx",
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
				"Crissaegrim",
				"Yasatsuna",
				"Dragon helm",
				"Holy glasses",
				"Spike Breaker",
				"Dracula tunic",
				"Ring of Varda",
				"Duplicator",
				"Covenant stone",
				"Gold Ring",
				"Silver Ring"
			};
		}
		public Point Location { get; set; }
		public bool Alerts { get; set; }
		public bool ControlPannelQueueActions { get; set; }
		public int Volume { get; set; }
		public string NamesFilePath { get; set; }
		public string BotApiKey { get; set; }
		public List<Action> Actions { get; set; }

		public System.TimeSpan QueueInterval { get; set; }
	

		public bool BoostFamiliars { get; set; }
		public bool ContinuousWingsmash { get; set; }
		public bool DynamicInterval { get; set; }
		public bool RomhackMode { get; set; }


		public int NeutralMinLevel { get; set; }
		public int NeutralStartLevel { get; set; }
		public int NeutralMaxLevel { get; set; }
		public bool AllowNeutralLevelReset { get; set; }
		public bool KeepVladRelics { get; set; }

		public bool RestrictedRelicSwap { get; set; }

		public bool EnforceMinStats { get; set; }

		public int CloneBossHPModifier { get; set; }
		public uint CloneBossDMGModifier { get; set; }
		public int SingleBossHPModifier { get; set; }
		public uint SingleBossDMGModifier { get; set; }
		public int GalamothBossHPModifier { get; set; }
		public uint GalamothBossDMGModifier { get; set; }
		public bool GalamothIsRepositioned { get; set; }
		public bool GalamothDefNerf { get; set; }

		public int ShaftOrbHPModifier { get; set; }
		public int SuperBossHPModifier { get; set; }
		public uint SuperBossDMGModifier { get; set; }
		public int AmbushHPModifier { get; set; }
		public uint AmbushDMGModifier { get; set; }
		public int SuperAmbushHPModifier { get; set; }
		public uint SuperAmbushDMGModifier { get; set; }
		public int BlessingModifier { get; set; }
		public int CurseModifier { get; set; }

		#region Mayhem
		public string[] painTradeItemRewards { get; set; }
		public string[] moderateBoonItemRewards { get; set; }
		public string[] majorBoonItemRewards { get; set; }
		public float StatsDownFactor { get; set; }
		public float SpeedFactor { get; set; }
		public float UnderwaterFactor { get; set; }
		public uint RegenGainPerSecond { get; set; }
		public int PandemoniumMinItems { get; set; }
		public int PandemoniumMaxItems { get; set; }
		#endregion

		#region Legacy
		public float WeakenFactor { get; set; }
		public float CrippleFactor { get; set; }
		public float HasteFactor { get; set; }
		public uint ThirstDrainPerSecond { get; set; }
		public int MeterOnReset { get; set; }
		public int PandoraMinItems { get; set; }
		public int PandoraMaxItems { get; set; }
		public int PandoraTrigger { get; set; }
		public void Default() {
			#region General Tab
			Alerts = true;
			ControlPannelQueueActions = true;
			Volume = 5;
			NamesFilePath = Paths.NamesFilePath;
			BotApiKey = "";

			QueueInterval = new System.TimeSpan(0, 0, 10);
			MeterOnReset = 25;
			ContinuousWingsmash = false;
			DynamicInterval = true;
			RomhackMode = false;
			BoostFamiliars = true;
			GalamothIsRepositioned = true;
			GalamothDefNerf = true;
			#endregion

			#region Settings Tab
			NeutralMinLevel = 1;
			NeutralStartLevel = 1;
			NeutralMaxLevel = 3;
			AllowNeutralLevelReset = true;

			EnforceMinStats = true;
			RestrictedRelicSwap = true;
			KeepVladRelics = true;

			StatsDownFactor = 0.4F;
			UnderwaterFactor = 0.8F;
			SpeedFactor = 3.2F;
			RegenGainPerSecond = 1;
			PandoraTrigger = 1200;
			PandemoniumMinItems = 16;
			PandemoniumMaxItems = 32;
			#endregion

			#region Difficulty Tab
			CloneBossHPModifier = 2;
			CloneBossDMGModifier = 2;
			SingleBossHPModifier = 2;
			SingleBossDMGModifier = 2;
			GalamothBossHPModifier = 2;
			GalamothBossDMGModifier = 2;
			ShaftOrbHPModifier = 2;
			SuperBossHPModifier = 2;
			SuperBossDMGModifier = 2;
			AmbushHPModifier = 2;
			AmbushDMGModifier = 2;
			SuperAmbushHPModifier = 2;
			SuperAmbushDMGModifier = 2;
			BlessingModifier = 2;
			CurseModifier = 2;
			#endregion
		}
		public string[] LightHelpItemRewards { get; set; }
		public string[] MediumHelpItemRewards { get; set; }
		public string[] HeavyHelpItemRewards { get; set; }
		#endregion
	}
}
