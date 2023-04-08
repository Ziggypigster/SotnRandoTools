using System.Collections.Generic;
using System.Drawing;
using SotnRandoTools.Configuration.Models;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Enums;

namespace SotnRandoTools.Configuration
{
	public class KhaosConfig
	{
		public KhaosConfig()
		{
			DefaultSettings();
			DefaultActions();

			merchantArmorRewards = new string[]
			{
				"Alucart shield",
				"Knight shield",
				"Shaman shield",
				"AxeLord shield",
				"Skull shield",
				"Circlet",
				"Cat-eye circl.",
				"Coral circlet",
				"Opal circlet",
				//"Alucart mail",
				"Healing mail",
				"Ice mail",
				"Holy mail",
				"Mojo mail",
				"Walk armor",
				"Cloth cape",
				"Reverse cloak",
				"Blood cloak",
				"Crystal cloak",
				"Joseph's cloak",
				"Staroulite",
				"Gauntlet",
				"Ring of Arcana",
				"Ring of Feanor",
				"Moonstone",
				"Ankh of Life",
				"Talisman",
				"Secret boots"
			};
			merchantFoodRewards = new string[]
			{
				"Frankfurter",
				"Barley tea",
				"Pizza",
				"Peanuts",
				"Pot Roast",
				"Turkey",
				"Sirloin"
			};
			merchantItemRewards = new string[]
			{
				"Potion",
				"High potion",
				"Elixir",
				"Manna prism",
				"Heart Refresh",
				"Antivenom",
				"Uncurse",
				"Hammer",
				"Attack potion",
				"Shield potion",
				"Smart potion",
				"Diamond",
				"Library card",
				"Life apple"
			};
			merchantWeaponRewards = new string[]
			{
				"Jewel knuckles",
				"Iron Fist",
				"Basilard",
				"Combat knife",
				"Holbein dagger",
				"Stone sword",
				"Shotel",
				"Sword of Hador",
				"Harper",
				"Alucart sword",
				"Terminus Est",
				"Badelaire",
				"Holy rod",
				"Muramasa",
				"Katana",
				"Dark Blade",
				"Chakram",
				"Heaven sword",
				"Runesword"
			};
			minorItemRewards = new string[]
			{
				"High potion",
				"Heart Refresh",
				"Magic Missile",
				"Boomerang",
				"Bwaka knife",
				"Fire boomerang",
				"monster vial 2",
				"Iron ball",
				"Neutron bomb"
			};
			moderateItemRewards = new string[]
			{
				"Manna prism", 
				"TNT", 
				"Shuriken", 
				"Javelin",
				"Flame star",
				"monster vial 1", 
				"monster vial 3", 
				"Pentagram", 
				"Power of Sire"
			};
			majorItemRewards = new string[]
			{
				"Attack potion",
				"Dynamite", 
				"Cross shuriken", 
				"Buffalo star", 
				"Bat Pentagram"
			};
			superItemRewards = new string[]
			{
				"Duplicator",
				"Str. potion",
				"Life apple",
				"Elixir",
				"Library card",
			};
			minorEquipmentRewards = new string[]
			{
				"Herald shield",
				"Dark shield",
				"Holbein dagger",
				"Gram",
				"Luminus",
				"Chakram",
				"Runesword",
				"Heaven sword",
				"Stone mask",
				"Wizard hat",
				"Ice mail",
				"Platinum mail",
				"Diamond plate",
				"Mirror cuirass",
				"Brilliant mail",
				"Elven cloak",
				"Blood cloak",
				"Royal cloak",
				"Twilight cloak",
				"Onyx",
				"Garnet",
				"Opal",
				"Diamond",
				"Nauglamir",
				"Talisman",
				"Mystic pendant",
				"Gauntlet",
				"Moonstone"
			};
			moderateEquipmentRewards = new string[]
			{
				"Fire shield",
				"Iron shield",
				"Medusa shield",
				"Alucard shield",
				"Shield rod",
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
				"Dark armor",
				"Fire mail",
				"Lightning mail",
				"Fury plate",
				"Mojo mail",
				"Mystic pendant",
				"Ring of Feanor",
				"Moonstone",
				"King's stone",
				"Ring of Ares"
			};
			majorEquipmentRewards = new string[]
			{
				"Mourneblade",
				"Mablung Sword",
				"Masamune",
				"Fist of Tulkas",
				"Gurthang",
				"Alucard sword",
				"Vorpal blade",
				"Crissaegrim",
				"Yasatsuna",
				"Dragon helm",
				"Alucard mail",
				"Dracula tunic",
				"God's Garb",
				"Ring of Varda",
				"Duplicator",
				"Covenant stone"
			};
			progressionItemRewards = new string[]
			{
				"Mourneblade",
				"Mablung Sword",
				"Masamune",
				"Fist of Tulkas",
				"Gurthang",
				"Alucard sword",
				"Vorpal blade",
				"Crissaegrim",
				"Yasatsuna",
				"Dragon helm",
				"Ring of Varda",
				"Duplicator",
			};
		}
		#region General Tab Variables
		public Point Location { get; set; }
		public bool Alerts { get; set; }
		public bool ControlPannelQueueActions { get; set; }
		public int Volume { get; set; }
		public string NamesFilePath { get; set; }
		public string BotApiKey { get; set; }
		public System.TimeSpan QueueInterval { get; set; }

		public int QuickSettings { get; set; }
		public bool spiritOrbOn { get; set; }
		public bool faerieScrollOn { get; set; }
		public bool cubeOfZoeOn { get; set; }

		public bool DisableLogs { get; set; }
		public bool OpenEntranceDoor { get; set; }
		public bool DisableMayhemMeter { get; set; }
		public bool PermaAxeArmor { get; set; }
		public bool AxeArmorTips { get; set; }
		public bool BoostAxeArmor{ get; set; }
		public bool BoostFamiliars { get; set; }

		public bool ReviveRichter { get; set; }
		public bool ContinuousWingsmash { get; set; }
		public bool DynamicInterval { get; set; }
		public bool RomhackMode { get; set; }
		#endregion

		#region Actions
		public List<Action> Actions { get; set; }
		public List<Action> AutoMayhemBlessings { get; set; }
		public List<Action> AutoMayhemNeutrals { get; set; }
		public List<Action> AutoMayhemCurses { get; set; }
		#endregion

		#region Auto-Mayhem Variables
		public int autoMayhemDifficulty { get; set; }
		public int autoCommandSpeed { get; set; }
		public int autoCommandConsistency { get; set; }
		public int autoMoodSwings { get; set; }
		public bool autoAllowPerfectMayhem { get; set; }
		public bool autoAllowMayhemPity { get; set; }
		public bool autoAllowMayhemRage { get; set; }
		public bool autoAllowSmartLogic { get; set; }
		public bool autoAllowBlessings { get; set; }
		public bool autoAllowNeutrals { get; set; }
		public bool autoAllowCurses { get; set; }

		public bool autoEnableSmartLogic { get; set; }
		public int autoPerfectMayhemTrigger { get; set; }

		public int autoBlessingWeight { get; set; }
		public int autoBlessingMood { get; set; }
		public int autoBlessingMin { get; set; }
		public int autoBlessingMax { get; set; }

		public int autoNeutralWeight { get; set; }
		public int autoNeutralMood { get; set; }
		public int autoNeutralMin { get; set; }
		public int autoNeutralMax { get; set; }

		public int autoCurseWeight { get; set; }
		public int autoCurseMood { get; set; }
		public int autoCurseMin { get; set; }
		public int autoCurseMax { get; set; }
		#endregion

		public int NeutralMinLevel { get; set; }
		public int NeutralStartLevel { get; set; }
		public int NeutralMaxLevel { get; set; }
		public bool AllowNeutralLevelReset { get; set; }
		public bool NerfUnderwater { get; set; }
		public bool RespawnRichter { get; set; }
		public bool KindAndFair { get; set; }
		public bool ProgressionGivesVlad { get; set; }
		public bool KeepVladRelics { get; set; }

		public bool RestrictedItemSwap { get; set; }
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
		public string[] merchantArmorRewards { get; set; }
		public string[] merchantFoodRewards { get; set; }
		public string[] merchantItemRewards { get; set; }
		public string[] merchantWeaponRewards { get; set; }
		public string[] minorEquipmentRewards { get; set; }
		public string[] moderateEquipmentRewards { get; set; }
		public string[] majorEquipmentRewards { get; set; }
		public string[] minorItemRewards { get; set; }
		public string[] moderateItemRewards { get; set; }
		public string[] majorItemRewards { get; set; }
		public string[] superItemRewards { get; set; }
		public string[] progressionItemRewards { get; set; }
		public float StatsDownFactor { get; set; }
		public float SpeedFactor { get; set; }
		public float UnderwaterFactor { get; set; }
		public uint RegenGainPerSecond { get; set; }
		public int PandemoniumMinItems { get; set; }
		public int PandemoniumMaxItems { get; set; }

		public int RichterColor { get; set; }
		public int SpawnEntityID { get; set; }

		public void SetQuickSettings() 
		{
			switch (QuickSettings)
			{
				case 1: //Mayhem
					spiritOrbOn = true;
					faerieScrollOn = true;
					cubeOfZoeOn = false;

					DisableMayhemMeter = false;
					PermaAxeArmor = false;
					BoostAxeArmor = false;
					AxeArmorTips = false;
					BoostFamiliars = true;
					ContinuousWingsmash = false;
					OpenEntranceDoor = false;
					break;
				case 2: //Axe Armor
					spiritOrbOn = true;
					faerieScrollOn = false;
					cubeOfZoeOn = false;

					DisableMayhemMeter = true;
					PermaAxeArmor = true;
					BoostAxeArmor = true;
					AxeArmorTips = true;
					BoostFamiliars = false;
					ContinuousWingsmash = false;
					OpenEntranceDoor = false;
					break;
				case 3: //Axe Armor w/ Cube
					spiritOrbOn = true;
					faerieScrollOn = false;
					cubeOfZoeOn = true;

					DisableMayhemMeter = true;
					PermaAxeArmor = true;
					BoostAxeArmor = true;
					AxeArmorTips = true;
					BoostFamiliars = false;
					ContinuousWingsmash = false;
					OpenEntranceDoor = false;
					break;
				default:
					break;
			}
		}

		public void DefaultSettings()
		{
			#region General Tab
			Alerts = true;
			ControlPannelQueueActions = true;
			Volume = 5;
			NamesFilePath = Paths.NamesFilePath;
			BotApiKey = "";
			QueueInterval = new System.TimeSpan(0, 0, 10);
			MeterOnReset = 25;

			DynamicInterval = true;
			RomhackMode = false;
			DisableLogs = false;

			QuickSettings = 1;
			SetQuickSettings();
			
			#endregion

			#region Auto-Mayhem Tab
			autoMayhemDifficulty = 3;
			autoCommandSpeed = 2;
			autoCommandConsistency = 2;
			autoMoodSwings = 2;

			autoPerfectMayhemTrigger = 1000;
			autoAllowPerfectMayhem = true;
			autoAllowMayhemPity = true;
			autoAllowMayhemRage = true;

			autoEnableSmartLogic = true;
			autoAllowBlessings = true;
			autoAllowNeutrals = true;
			autoAllowCurses = true;

			autoBlessingWeight = 5;
			autoBlessingMood = 1;
			autoBlessingMin = 1;
			autoBlessingMax = 3;

			autoNeutralWeight = 5;
			autoNeutralMood = 1;
			autoNeutralMin = 1;
			autoNeutralMax = 3;

			autoCurseWeight = 5;
			autoCurseMood = 1;
			autoCurseMin = 1;
			autoCurseMax = 3;
			#endregion

			#region Settings Tab
			NeutralMinLevel = 1;
			NeutralStartLevel = 1;
			NeutralMaxLevel = 3;
			AllowNeutralLevelReset = true;

			NerfUnderwater = false;
			EnforceMinStats = true;
			RespawnRichter = true;
			KindAndFair = false;
			RestrictedItemSwap = true;
			RestrictedRelicSwap = true;
			KeepVladRelics = true;

			StatsDownFactor = 0.4F;
			UnderwaterFactor = 0.8F;
			SpeedFactor = 3.2F;
			RegenGainPerSecond = 1u;
			PandemoniumMinItems = 16;
			PandemoniumMaxItems = 32;
			#endregion

			#region Difficulty Tab
			CloneBossHPModifier = 2;
			CloneBossDMGModifier = 2u;
			SingleBossHPModifier = 2;
			SingleBossDMGModifier = 2u;
			GalamothBossHPModifier = 2;
			GalamothBossDMGModifier = 2u;
			GalamothIsRepositioned = true;
			GalamothDefNerf = true;
			ShaftOrbHPModifier = 2;
			SuperBossHPModifier = 2;
			SuperBossDMGModifier = 2u;
			AmbushHPModifier = 2;
			AmbushDMGModifier = 2u;
			SuperAmbushHPModifier = 2;
			SuperAmbushDMGModifier = 2u;
			BlessingModifier = 2;
			CurseModifier = 2;
			#endregion
		}

		public void DefaultActions()
		{
			
			AutoMayhemBlessings = new List<Action>();
			AutoMayhemNeutrals = new List<Action>();
			AutoMayhemCurses = new List<Action>();

			Actions = new List<Action>
			{
				//Debug Commands
				new Action{Command = "logcurrentroom", Name="Log Current Room", AutoMayhemEnabled = false, Enabled = true, Meter = 0, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 0, 0), StartsOnCooldown = false },
				new Action{Command = "rewind", Name="Rewind", AutoMayhemEnabled = false, Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 0, 1), Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 8, 0), StartsOnCooldown = true},
				new Action{Command = "library", Name="Library", AutoMayhemEnabled = false, Enabled = true, Meter = 10, Duration = new System.TimeSpan(0, 0, 7), Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 10, 0), StartsOnCooldown = false},
				new Action{Command = "minstats", Name="Min Stats", AutoMayhemEnabled = false, Enabled = true, Meter = 12, Duration = new System.TimeSpan(0, 0, 1), Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 10, 0), StartsOnCooldown = false},

				//Normal Commands
				new Action{Command = "merchant", Name="Merchant", AutoMayhemEnabled = true, Enabled = true, Meter = 4, AlertPath = Paths.PainTradeSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 0, 0), StartsOnCooldown = false },
				new Action{Command = "maxmayhem", Name="Max Mayhem", AutoMayhemEnabled = true, Enabled = true, Meter = 0, AlertPath = Paths.MaxMayhemSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 2, 0), StartsOnCooldown = true},
				new Action{Command = "heartsonly", Name="Hearts Only", AutoMayhemEnabled = true, Enabled = true, Meter = 6,  Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.HeartsOnlySound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 1, 15), StartsOnCooldown = false},
				new Action{Command = "unarmed", Name="Unarmed", AutoMayhemEnabled = true, Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 10), AlertPath = Paths.UnarmedSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 1, 15), StartsOnCooldown = false},
				new Action{Command = "turbomode", Name="Turbo Mode", AutoMayhemEnabled = true, Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 10), AlertPath = Paths.TurboModeSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 1, 15), StartsOnCooldown = false},
				new Action{Command = "rushdown", Name="Rushdown", AutoMayhemEnabled = true, Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 10), AlertPath = Paths.RushdownSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 1, 15), StartsOnCooldown = false},
				new Action{Command = "swapstats", Name="Swap Stats", AutoMayhemEnabled = true, Enabled = true, Meter = 8, AlertPath = Paths.SwapStatsSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 0, 10), StartsOnCooldown = false},
				new Action{Command = "swapequipment", Name="Swap Equipment", AutoMayhemEnabled = true, Enabled = true, Meter = 8, AlertPath = Paths.SwapEquipmentSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 6, 0), StartsOnCooldown = false},
				new Action{Command = "swaprelics", Name="Swap Relics", AutoMayhemEnabled = false, Enabled = true, Meter = 12, AlertPath = Paths.SwapRelicsSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 35, 0), StartsOnCooldown = true},
				new Action{Command = "pandemonium", Name="Pandemonium", AutoMayhemEnabled = false, Enabled = true, Meter = 12, AlertPath = Paths.PandemoniumSound, Type = (int)Khaos.Enums.ActionType.Neutral, Cooldown = new System.TimeSpan(0, 35, 0), StartsOnCooldown = true},
				new Action{Command = "minortrap", Name="Minor Trap", AutoMayhemEnabled = true, Enabled = true, Meter = 2, AlertPath = Paths.MinorTrapSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 0, 0), StartsOnCooldown = false},
				new Action{Command = "slam", Name="Slam", AutoMayhemEnabled = true, Enabled = true, Meter = 3, AlertPath = Paths.SlamSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 1, 0), StartsOnCooldown = false},
				new Action{Command = "slamjam", Name="Slam Jam", AutoMayhemEnabled = false, Enabled = true, Meter = 3, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.SlamJamSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 8, 0), StartsOnCooldown = true},
				new Action{Command = "hpformp", Name="HP for MP", AutoMayhemEnabled = true, Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.HPForMPSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 2, 30), StartsOnCooldown = false},
				new Action{Command = "underwater", Name="Underwater", AutoMayhemEnabled = true, Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 0, 45), AlertPath = Paths.UnderwaterSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 2, 30), StartsOnCooldown = false},
				new Action{Command = "trap", Name="Trap", AutoMayhemEnabled = true, Enabled = true, Meter = 6, AlertPath = Paths.ModerateTrapSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 0, 0), StartsOnCooldown = false},
				new Action{Command = "moderatetrap", Name="Moderate Trap", AutoMayhemEnabled = true, Enabled = true, Meter = 6, AlertPath = Paths.ModerateTrapSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 0, 0), StartsOnCooldown = false},
				new Action{Command = "hex", Name="Hex", AutoMayhemEnabled = true, Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.HexSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 0, 10), StartsOnCooldown = false},
				new Action{Command = "getjuggled", Name="Get Juggled", AutoMayhemEnabled = true, Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.GetJuggledSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 6, 0), StartsOnCooldown = false},
				new Action{Command = "majortrap", Name="Major Trap", AutoMayhemEnabled = true, Enabled = true, Meter = 8, AlertPath = Paths.MajorTrapSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 0, 0), StartsOnCooldown = false},
				new Action{Command = "ambush", Name="Ambush", AutoMayhemEnabled = true, Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 15), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.AmbushSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 5, 10), StartsOnCooldown = false},
				new Action{Command = "toughbosses", Name="Tough Bosses", AutoMayhemEnabled = true, Enabled = true, Meter = 10, AlertPath = Paths.ToughBossesSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 4, 0), StartsOnCooldown = true},
				new Action{Command = "statsdown", Name="Stats Down", AutoMayhemEnabled = true, Enabled = true, Meter = 12, AlertPath = Paths.StatsDownSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 6, 0), StartsOnCooldown = true},
				new Action{Command = "confiscate", Name="Confiscate", AutoMayhemEnabled = true, Enabled = true, Meter = 12, AlertPath = Paths.ConfiscateSound, Type = (int)Khaos.Enums.ActionType.Curse, Cooldown = new System.TimeSpan(0, 12, 0), StartsOnCooldown = true},
				new Action{Command = "potions", Name="Potions", AutoMayhemEnabled = true, Enabled = true, Meter = 2, AlertPath = Paths.PotionsSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 0, 0), StartsOnCooldown = false},
				new Action{Command = "speed", Name="Speed", AutoMayhemEnabled = true, Enabled = true, Meter = 3, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.SpeedSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 0, 10), StartsOnCooldown = false},
				new Action{Command = "regen", Name="Regen", AutoMayhemEnabled = true, Enabled = true, Meter = 4, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.RegenSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 0, 10), StartsOnCooldown = false},
				new Action{Command = "minorboon", Name="Minor Boon", AutoMayhemEnabled = false, Enabled = true, Meter = 4, AlertPath = Paths.MinorBoonSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 20, 0), StartsOnCooldown = false},
				new Action{Command = "minorequipment", Name="Minor Equipment", AutoMayhemEnabled = true, Enabled = true, Meter = 4, AlertPath = Paths.MinorEquipmentSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 2, 0), StartsOnCooldown = false},
				new Action{Command = "minoritems", Name="Minor Items", AutoMayhemEnabled = true, Enabled = true, Meter = 4, AlertPath = Paths.MinorItemsSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 1, 30), StartsOnCooldown = false},
				new Action{Command = "minorstats", Name="Minor Stats", AutoMayhemEnabled = true, Enabled = true, Meter = 4, AlertPath = Paths.MinorStatsSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 1, 0), StartsOnCooldown = false},
				new Action{Command = "items", Name="Items", AutoMayhemEnabled = false, Enabled = true, Meter = 6, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 6, 0), StartsOnCooldown = true},
				new Action{Command = "equipment", Name="Equipment", AutoMayhemEnabled = false, Enabled = true, Meter = 6, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 6, 0), StartsOnCooldown = false},
				new Action{Command = "boon", Name="Boon", AutoMayhemEnabled = false, Enabled = true, Meter = 6, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 6, 0), StartsOnCooldown = false},
				new Action{Command = "moderateboon", Name="Moderate Boon", AutoMayhemEnabled = false, Enabled = true, Meter = 6, AlertPath = Paths.ModerateBoonSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 30, 0), StartsOnCooldown = false},
				new Action{Command = "moderateequipment", Name="Moderate Equipment", AutoMayhemEnabled = true, Enabled = true, Meter = 6, AlertPath = Paths.ModerateEquipmentSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 2, 0), StartsOnCooldown = false},
				new Action{Command = "moderateitems", Name="Moderate Items", AutoMayhemEnabled = true, Enabled = true, Meter = 6, AlertPath = Paths.ModerateItemsSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 2, 30), StartsOnCooldown = false},
				new Action{Command = "moderatestats", Name="Moderate Stats", AutoMayhemEnabled = true, Enabled = true, Meter = 6, AlertPath = Paths.ModerateStatsSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 1, 30), StartsOnCooldown = false},
				new Action{Command = "timestop", Name="Time Stop", AutoMayhemEnabled = true, Enabled = true, Meter = 6, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.TimeStopSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 0, 0), StartsOnCooldown = false},
				new Action{Command = "buff", Name="Buff", AutoMayhemEnabled = true, Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.TimeStopSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 3, 0), StartsOnCooldown = false},
				new Action{Command = "spellcaster", Name="Spellcaster", AutoMayhemEnabled = true, Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.SpellcasterSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 0, 20), StartsOnCooldown = false},
				new Action{Command = "facetank", Name="Face Tank", AutoMayhemEnabled = true, Enabled = true,  Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.FaceTankSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 1, 0), StartsOnCooldown = false},
				new Action{Command = "extrarange", Name="Extra Range", AutoMayhemEnabled = true, Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0), AlertPath = Paths.ExtraRangeSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 0, 20), StartsOnCooldown = false},
				new Action{Command = "lucky", Name="Lucky", AutoMayhemEnabled = true, Enabled = true, Meter = 8, Duration = new System.TimeSpan(0, 1, 0),AlertPath = Paths.LuckySound,Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 0, 20), StartsOnCooldown = false},
				new Action{Command = "summoner", Name="Summoner", AutoMayhemEnabled = true, Enabled = true,  Meter = 8, Duration = new System.TimeSpan(0, 1, 15), Interval = new System.TimeSpan(0, 0, 1), AlertPath = Paths.SummonerSound, Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 5, 0), StartsOnCooldown = true},
				new Action{Command = "majorboon", Name="Major Boon", AutoMayhemEnabled = false, Enabled = true, Meter = 10, AlertPath = Paths.MajorBoonSound,Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 60, 0), StartsOnCooldown = true},
				new Action{Command = "majorequipment", Name="Major Equipment", AutoMayhemEnabled = true, Enabled = true, Meter = 10, AlertPath = Paths.MajorEquipmentSound,Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 4, 0), StartsOnCooldown = true},
				new Action{Command = "majoritems", Name="Major Items", AutoMayhemEnabled = true, Enabled = true, Meter = 10, AlertPath = Paths.MajorItemsSound,Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 5, 0), StartsOnCooldown = true},
				new Action{Command = "majorstats", Name="Major Stats", AutoMayhemEnabled = true, Enabled = true, Meter = 10, AlertPath = Paths.MajorStatsSound,Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 3, 0), StartsOnCooldown = true},
				new Action{Command = "progression", Name="Progression", AutoMayhemEnabled = false, Enabled = true, Meter = 12, AlertPath = Paths.ProgressionSound,Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(1, 0, 0), StartsOnCooldown = true},
				new Action{Command = "progressionstats", Name="Progression Stats", AutoMayhemEnabled = true, Enabled = true, Meter = 12, AlertPath = Paths.ProgressionStatsSound,Type = (int)Khaos.Enums.ActionType.Blessing, Cooldown = new System.TimeSpan(0, 30, 0), StartsOnCooldown = true},
			};
		}
		#endregion

		#region Legacy

		public int MeterOnReset { get; set; }
		#endregion
	}
}