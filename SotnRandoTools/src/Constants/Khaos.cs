using System.Collections.Generic;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Models;
using MapLocation = SotnRandoTools.RandoTracker.Models.MapLocation;

namespace SotnRandoTools.Constants
{
	public static class Khaos
	{
		/*
		public static Relic[] ProgressionRelics =
		{
			// Handled by smart logic
			Relic.FormOfMist,
			Relic.GravityBoots,
			Relic.JewelOfOpen,
			Relic.LeapStone,
			Relic.MermanStatue,
			Relic.PowerOfMist,
			Relic.SoulOfBat,
			Relic.SoulOfWolf

			Relic.HeartOfVlad,
			Relic.ToothOfVlad,
			Relic.RingOfVlad,
			Relic.EyeOfVlad,
			Relic.ToothOfVlad
		};*/
		public static Relic[] MinorBoonRelics =
		{
			Relic.BatCard,
			Relic.CubeOfZoe,
			Relic.GhostCard,
			Relic.HolySymbol,
			Relic.LeapStone,
			Relic.SoulOfWolf,
			Relic.SpiritOrb,
			Relic.SwordCard
		};
		public static Relic[] ModerateBoonRelics =
		{
			Relic.DemonCard,
			Relic.FaerieCard,
			Relic.NoseDevilCard,
			Relic.LeapStone,
			Relic.SoulOfWolf,
			Relic.SpriteCard,
		};
		public static Relic[] MajorBoonRelics =
		{
			Relic.FormOfMist,
			Relic.GravityBoots,
			Relic.LeapStone,
			Relic.JewelOfOpen,
			Relic.MermanStatue,
			Relic.PowerOfMist,
			Relic.SoulOfWolf,
			Relic.SoulOfBat
		};

		public static List<Relic[]> FlightRelics = new List<Relic[]>{
			new Relic[] {Relic.SoulOfBat},
			new Relic[] {Relic.LeapStone, Relic.GravityBoots},
			new Relic[] {Relic.FormOfMist, Relic.PowerOfMist},
			new Relic[] {Relic.SoulOfWolf, Relic.GravityBoots},
		};
		public static List<MapLocation> LoadingRooms = new List<MapLocation>
		{
			new MapLocation{X =17,Y = 36, SecondCastle = 0},
			new MapLocation{X =21,Y = 26, SecondCastle = 0},
			new MapLocation{X =20,Y = 36, SecondCastle = 0},
			new MapLocation{X =30,Y = 25, SecondCastle = 0},
			new MapLocation{X =26,Y = 22, SecondCastle = 0},
			new MapLocation{X =13,Y = 22, SecondCastle = 0},
			new MapLocation{X = 4,Y = 28, SecondCastle = 0},
			new MapLocation{X =36,Y = 21, SecondCastle = 0},
			new MapLocation{X =17,Y = 19, SecondCastle = 0},
			new MapLocation{X =29,Y = 12, SecondCastle = 0},
			new MapLocation{X =39,Y = 12, SecondCastle = 0},
			new MapLocation{X =39,Y = 10, SecondCastle = 0},
			new MapLocation{X =60,Y = 14, SecondCastle = 0},
			new MapLocation{X =60,Y = 17, SecondCastle = 0},
			new MapLocation{X =59,Y = 21, SecondCastle = 0},
			new MapLocation{X =60,Y = 25, SecondCastle = 0},
			new MapLocation{X =40,Y = 26, SecondCastle = 0},
			new MapLocation{X =15,Y = 41, SecondCastle = 0},
			new MapLocation{X =28,Y = 38, SecondCastle = 0},
			new MapLocation{X =34,Y = 44, SecondCastle = 0},
			new MapLocation{X =32,Y = 49, SecondCastle = 0},
			new MapLocation{X =16,Y = 38, SecondCastle = 0},

			new MapLocation{X =24,Y = 51, SecondCastle = 1},
			new MapLocation{X =24,Y = 53, SecondCastle = 1},
			new MapLocation{X = 3,Y = 49, SecondCastle = 1},
			new MapLocation{X = 3,Y = 46, SecondCastle = 1},
			new MapLocation{X = 4,Y = 42, SecondCastle = 1},
			new MapLocation{X = 3,Y = 38, SecondCastle = 1},
			new MapLocation{X =23,Y = 37, SecondCastle = 1},
			new MapLocation{X =35,Y = 25, SecondCastle = 1},
			new MapLocation{X =29,Y = 19, SecondCastle = 1},
			new MapLocation{X =31,Y = 14, SecondCastle = 1},
			new MapLocation{X =48,Y = 22, SecondCastle = 1},
			new MapLocation{X =47,Y = 25, SecondCastle = 1},
			new MapLocation{X =43,Y = 27, SecondCastle = 1},
			new MapLocation{X =46,Y = 27, SecondCastle = 1},
			new MapLocation{X =59,Y = 35, SecondCastle = 1},
			new MapLocation{X =42,Y = 37, SecondCastle = 1},
			new MapLocation{X =33,Y = 38, SecondCastle = 1},
			new MapLocation{X =37,Y = 41, SecondCastle = 1},
			new MapLocation{X =50,Y = 41, SecondCastle = 1},
			new MapLocation{X =46,Y = 44, SecondCastle = 1},
			new MapLocation{X =27,Y = 42, SecondCastle = 1},
			new MapLocation{X =34,Y = 51, SecondCastle = 1},
		};
		public static List<MapLocation> SuccubusRoom = new List<MapLocation>
		{
			new MapLocation{X = 0, Y = 0, SecondCastle = 0}
		};
		public static List<MapLocation> ShopRoom = new List<MapLocation>
		{
			new MapLocation{X = 49, Y = 20, SecondCastle = 0}
		};
		public static List<MapLocation> AxeArmorPreventFormShiftRooms = new List<MapLocation>
		{
			new MapLocation{X = 43, Y = 47, SecondCastle = 1},
			new MapLocation{X = 44, Y = 47, SecondCastle = 1},
		};
		public static List<MapLocation> ClockRoom = new List<MapLocation>
		{
			new MapLocation{X = 32, Y = 26, SecondCastle = 0},
			new MapLocation{X = 31, Y = 37, SecondCastle = 1}
		};
		public static List<MapLocation> ReverseElevator = new List<MapLocation>
		{
			new MapLocation{X = 31, Y = 35, SecondCastle = 1},
			new MapLocation{X = 31, Y = 36, SecondCastle = 1},
		};
		public static List<MapLocation> Elevator = new List<MapLocation>
		{
			new MapLocation{X = 32, Y = 27, SecondCastle = 0},
			new MapLocation{X = 32, Y = 28, SecondCastle = 0},
		};
		public static List<MapLocation> RichterRooms = new List<MapLocation>
		{
			new MapLocation{X = 31, Y = 8, SecondCastle = 0},
			new MapLocation{X = 32, Y = 8, SecondCastle = 0},
			new MapLocation{X = 33, Y = 8, SecondCastle = 0},
			new MapLocation{X = 34, Y = 8, SecondCastle = 0},
		};

		public static List<MapLocation> ReduceFastFallRooms = new List<MapLocation>
		{
			new MapLocation{X = 30, Y = 12, SecondCastle = 0}, //AxeArmor Hallway
			new MapLocation{X = 31, Y = 12, SecondCastle = 0},
			new MapLocation{X = 32, Y = 12, SecondCastle = 0},
			new MapLocation{X = 33, Y = 12, SecondCastle = 0},
			new MapLocation{X = 34, Y = 12, SecondCastle = 0},
			new MapLocation{X = 35, Y = 12, SecondCastle = 0},
			new MapLocation{X = 36, Y = 12, SecondCastle = 0},
			new MapLocation{X = 27, Y = 51, SecondCastle = 1}, //Reverse Castle AxeArmor
			new MapLocation{X = 28, Y = 51, SecondCastle = 1},
			new MapLocation{X = 29, Y = 51, SecondCastle = 1},
			new MapLocation{X = 30, Y = 51, SecondCastle = 1},
			new MapLocation{X = 31, Y = 51, SecondCastle = 1},
			new MapLocation{X = 32, Y = 51, SecondCastle = 1},
			new MapLocation{X = 36, Y = 53, SecondCastle = 1}, //Reverse Clocktower
			new MapLocation{X = 44, Y = 53, SecondCastle = 1},
			new MapLocation{X = 49, Y = 51, SecondCastle = 1},
			new MapLocation{X = 42, Y = 13, SecondCastle = 1}, //Reverse Galamoth Hallway to Boss?
		};
		public static List<MapLocation> EntranceCutsceneRooms = new List<MapLocation>
		{
			new MapLocation{X = 0, Y = 44, SecondCastle = 0},
			new MapLocation{X = 1, Y = 44, SecondCastle = 0},
			new MapLocation{X = 2, Y = 44, SecondCastle = 0},
			new MapLocation{X = 3, Y = 44, SecondCastle = 0},
			new MapLocation{X = 4, Y = 44, SecondCastle = 0},
			new MapLocation{X = 5, Y = 44, SecondCastle = 0},
			new MapLocation{X = 6, Y = 44, SecondCastle = 0},
			new MapLocation{X = 7, Y = 44, SecondCastle = 0},
			new MapLocation{X = 8, Y = 44, SecondCastle = 0},
			new MapLocation{X = 9, Y = 44, SecondCastle = 0},
			new MapLocation{X = 10, Y = 44, SecondCastle = 0},
			new MapLocation{X = 11, Y = 44, SecondCastle = 0},
			new MapLocation{X = 12, Y = 44, SecondCastle = 0},
			new MapLocation{X = 13, Y = 44, SecondCastle = 0},
			new MapLocation{X = 14, Y = 44, SecondCastle = 0},
			new MapLocation{X = 15, Y = 44, SecondCastle = 0},
			new MapLocation{X = 16, Y = 44, SecondCastle = 0},
			new MapLocation{X = 17, Y = 44, SecondCastle = 0},
			new MapLocation{X = 18, Y = 44, SecondCastle = 0},
		};
		public static List<MapLocation> SwitchRoom = new List<MapLocation>
		{
			new MapLocation{X = 46, Y = 24, SecondCastle = 0}
		};
		public static List<MapLocation> UnderwaterBugRooms = new List<MapLocation>
		{ // X is Left to Right, Y is top to bottom
			new MapLocation{X = 40, Y = 12, SecondCastle = 0}, //First Castle Warps + Hallways
			new MapLocation{X = 39, Y = 12, SecondCastle = 0},
			new MapLocation{X = 37, Y = 21, SecondCastle = 0},
			new MapLocation{X = 36, Y = 21, SecondCastle = 0},
			new MapLocation{X = 15, Y = 38, SecondCastle = 0},
			new MapLocation{X = 16, Y = 38, SecondCastle = 0},
			new MapLocation{X = 35, Y = 44, SecondCastle = 0},
			new MapLocation{X = 34, Y = 44, SecondCastle = 0},
			new MapLocation{X = 59, Y = 17, SecondCastle = 0},
			new MapLocation{X = 60, Y = 17, SecondCastle = 0},
			new MapLocation{X = 23, Y = 51, SecondCastle = 0}, //2nd Castle Warps + Hallways
			new MapLocation{X = 24, Y = 42, SecondCastle = 1},
			new MapLocation{X = 26, Y = 42, SecondCastle = 1},
			new MapLocation{X = 27, Y = 38, SecondCastle = 1},
			new MapLocation{X = 46, Y = 25, SecondCastle = 1},
			new MapLocation{X = 47, Y = 25, SecondCastle = 1},
			new MapLocation{X = 28, Y = 19, SecondCastle = 1},
			new MapLocation{X = 29, Y = 19, SecondCastle = 1},
			new MapLocation{X = 4, Y = 46, SecondCastle = 1},
			new MapLocation{X = 3, Y = 46, SecondCastle = 1},
		};
		public static List<MapLocation> AxeArmorOlroxRooms = new List<MapLocation>
		{
			new MapLocation{X = 22, Y = 16, SecondCastle = 0}, //Olrox
		};
		public static List<MapLocation> AxeArmorReverseOlroxRooms = new List<MapLocation>
		{
			new MapLocation{X = 41, Y = 47, SecondCastle = 1}, //Reverse Olrox
		};
		public static List<MapLocation> CavernSwitchRoom = new List<MapLocation>
		{
			new MapLocation{X = 22, Y = 37, SecondCastle = 0}, //Cavern Switch Room
		};
		public static List<MapLocation> JewelSwordRooms = new List<MapLocation>
		{
			new MapLocation{X = 11, Y = 40, SecondCastle = 0},
			new MapLocation{X = 12, Y = 40, SecondCastle = 0},
			new MapLocation{X = 13, Y = 40, SecondCastle = 0},
			new MapLocation{X = 10, Y = 41, SecondCastle = 0}, // Actual Jewel Sword Location
			new MapLocation{X = 11, Y = 41, SecondCastle = 0},
			new MapLocation{X = 12, Y = 41, SecondCastle = 0},
			new MapLocation{X = 13, Y = 41, SecondCastle = 0},
			new MapLocation{X = 50, Y = 22, SecondCastle = 1},
			new MapLocation{X = 51, Y = 22, SecondCastle = 1},
			new MapLocation{X = 52, Y = 22, SecondCastle = 1},
			new MapLocation{X = 53, Y = 22, SecondCastle = 1},// Actual Reverse Jewel Sword Location
			new MapLocation{X = 50, Y = 23, SecondCastle = 1},
			new MapLocation{X = 51, Y = 23, SecondCastle = 1},
			new MapLocation{X = 52, Y = 23, SecondCastle = 1},
		};

		public static List<MapLocation> BatLightRooms = new List<MapLocation>
		{
			new MapLocation{X = 41, Y = 50, SecondCastle = 0},
			new MapLocation{X = 42, Y = 50, SecondCastle = 0},
			new MapLocation{X = 43, Y = 50, SecondCastle = 0},
		};
		public static List<MapLocation> MistGateRooms = new List<MapLocation>
		{
			new MapLocation{X = 46, Y = 21, SecondCastle = 0}, //Library
			new MapLocation{X = 47, Y = 21, SecondCastle = 0}, //Library
			new MapLocation{X = 13, Y = 15, SecondCastle = 0}, //Silver Ring
			new MapLocation{X = 14, Y = 15, SecondCastle = 0}, //Silver Ring
			new MapLocation{X = 60, Y = 27, SecondCastle = 0}, //Outer Wall
			new MapLocation{X = 21, Y = 22, SecondCastle = 0}, //Coliseum
			new MapLocation{X = 22, Y = 22, SecondCastle = 0}, //Coliseum

			new MapLocation{X = 16, Y = 42, SecondCastle = 1}, //Reverse Library
			new MapLocation{X = 17, Y = 42, SecondCastle = 1}, //Reverse Library
			new MapLocation{X = 50, Y = 48, SecondCastle = 1}, //Reverse Silver Ring
			new MapLocation{X = 3, Y = 36, SecondCastle = 1}, //Reverse Outer Wall
			new MapLocation{X = 41, Y = 41, SecondCastle = 1}, //Reverse Coliseum
			new MapLocation{X = 42, Y = 41, SecondCastle = 1}, //Reverse Coliseum
		};
		public static List<MapLocation> RewindBanRoom = new List<MapLocation>
		{
			new MapLocation{X = 40, Y = 39, SecondCastle = 0}, //Scylla Room 
			new MapLocation{X = 57, Y = 23, SecondCastle = 0}, //Doppleganger Room (Left)
			new MapLocation{X = 58, Y = 23, SecondCastle = 0}, //Doppleganger Room (Right)
			new MapLocation{X = 23, Y = 13, SecondCastle = 0}, //Hippogryph Room (Left)
			new MapLocation{X = 24, Y = 13, SecondCastle = 0}, //Hippogryph Room (Right)
			new MapLocation{X = 5, Y = 40, SecondCastle = 1}, //Reverse Doppleganger Room
			new MapLocation{X = 6, Y = 40, SecondCastle = 1}, //Reverse Doppleganger Room
			//new MapLocation{X = 31, Y = 30, SecondCastle = 1}, //???
		};
		public static List<MapLocation> RewindUnbanRoom = new List<MapLocation>
		{
			new MapLocation{X = 39, Y = 39, SecondCastle = 0}, //Outside Scylla Room
			new MapLocation{X = 50, Y = 20, SecondCastle = 0}, //Room Outside Librarian
		};

		public static List<MapLocation> SummonerBanZone = new List<MapLocation>
		{
			new MapLocation{X = 29, Y = 37, SecondCastle = 0},
			new MapLocation{X = 28, Y = 37, SecondCastle = 0},
			new MapLocation{X = 27, Y = 37, SecondCastle = 0},
			new MapLocation{X = 26, Y = 37, SecondCastle = 0},
		};

		public static List<MapLocation> HolyGlassesZone = new List<MapLocation>
		{
			new MapLocation{X = 32, Y = 28, SecondCastle = 0},
			new MapLocation{X = 32, Y = 29, SecondCastle = 0},
			new MapLocation{X = 32, Y = 30, SecondCastle = 0},
			new MapLocation{X = 31, Y = 30, SecondCastle = 0},
			new MapLocation{X = 31, Y = 31, SecondCastle = 0},
			new MapLocation{X = 31, Y = 32, SecondCastle = 0},
			new MapLocation{X = 32, Y = 32, SecondCastle = 0},
			new MapLocation{X = 33, Y = 32, SecondCastle = 0},
			new MapLocation{X = 33, Y = 31, SecondCastle = 0},
			new MapLocation{X = 33, Y = 30, SecondCastle = 0},
		};

		public static List<MapLocation> OlroxZone = new List<MapLocation>
		{
			new MapLocation{X = 24, Y = 16, SecondCastle = 0},
			new MapLocation{X = 23, Y = 16, SecondCastle = 0},
			new MapLocation{X = 22, Y = 16, SecondCastle = 0},
			new MapLocation{X = 21, Y = 16, SecondCastle = 0},
			new MapLocation{X = 20, Y = 16, SecondCastle = 0},
			new MapLocation{X = 20, Y = 17, SecondCastle = 0},
			new MapLocation{X = 19, Y = 17, SecondCastle = 0},
			new MapLocation{X = 19, Y = 16, SecondCastle = 0},
			new MapLocation{X = 18, Y = 16, SecondCastle = 0},
			new MapLocation{X = 17, Y = 16, SecondCastle = 0},
		};

		public static List<MapLocation> LesserDemonZone = new List<MapLocation>
		{
			new MapLocation{X = 45, Y = 20, SecondCastle = 0},
			new MapLocation{X = 46, Y = 20, SecondCastle = 0},
			new MapLocation{X = 47, Y = 20, SecondCastle = 0},
			new MapLocation{X = 48, Y = 20, SecondCastle = 0},
			new MapLocation{X = 48, Y = 19, SecondCastle = 0},
			new MapLocation{X = 47, Y = 19, SecondCastle = 0}
		};

		public static List<MapLocation> AkmodanZone = new List<MapLocation>
		{
			new MapLocation{X = 39, Y = 47, SecondCastle = 1},
			new MapLocation{X = 40, Y = 47, SecondCastle = 1},
			new MapLocation{X = 41, Y = 47, SecondCastle = 1},
			new MapLocation{X = 42, Y = 47, SecondCastle = 1},
			new MapLocation{X = 43, Y = 47, SecondCastle = 1},
			new MapLocation{X = 43, Y = 46, SecondCastle = 1},
			new MapLocation{X = 44, Y = 46, SecondCastle = 1},
			new MapLocation{X = 44, Y = 47, SecondCastle = 1},
			new MapLocation{X = 45, Y = 47, SecondCastle = 1},
			new MapLocation{X = 46, Y = 47, SecondCastle = 1},
			new MapLocation{X = 47, Y = 47, SecondCastle = 1},
		};

		public static List<MapLocation> GalamothRooms = new List<MapLocation>
		{
			new MapLocation{X = 44, Y = 12, SecondCastle = 1},
			new MapLocation{X = 45, Y = 12, SecondCastle = 1},
			new MapLocation{X = 44, Y = 13, SecondCastle = 1},
			new MapLocation{X = 45, Y = 13, SecondCastle = 1},
		};

		public static List<SearchableActor> AcceptedAmbushEnemies = new List<SearchableActor>
		{
			new SearchableActor { Name="Zombie", Hp = 1, Damage = 14, AiId = 25776 },
			new SearchableActor { Name="Bat", Hp = 1, Damage = 16, AiId = 25188 },
			new SearchableActor { Name="Bone Scimitar", Hp = 18 , Damage = 5, AiId = 23212  },
			new SearchableActor { Name="Bloody Zombie", Hp = 24 , Damage = 10, AiId = 42580  },
			new SearchableActor { Hp = 48, Damage = 13, AiId = 4612 },
			new SearchableActor { Name="Bone Scimitar Alchemy Lab", Hp = 18, Damage = 5, AiId = 14308  },
			new SearchableActor { Name="Blood Skeleton", Hp = 9, Damage = 8, AiId = 31064  },
			new SearchableActor { Name="Skeleton", Hp = 9, Damage = 2, AiId = 24516 },
			new SearchableActor { Name="Spittle Bone", Hp = 18, Damage = 7, AiId = 26412 },
			new SearchableActor { Name="Axe Knight", Hp = 32, Damage = 6, AiId = 17852  },
			new SearchableActor { Name="Axe Knight Marble Gallery", Hp = 32, Damage = 6, AiId = 46300  },
			new SearchableActor { Name="Ouija Table", Hp = 20, Damage = 4, AiId = 48588 },
			new SearchableActor { Name="Slinger", Hp = 12, Damage = 5, AiId = 30320  },
			new SearchableActor { Name="Marionette", Hp = 20, Damage = 8, AiId = 26360 },
			new SearchableActor { Hp = 5, Damage = 4, AiId = 48588  },
			new SearchableActor { Name="Flea Man", Hp = 11, Damage = 9, AiId = 51080  },
			new SearchableActor { Name="Skeleton Marble Gallery", Hp = 9, Damage = 2, AiId = 52040  },
			new SearchableActor { Name="Skeleton Outer Wall", Hp = 9, Damage = 2, AiId = 54896 },
			new SearchableActor { Name="Spear Guard Outer Wall", Hp = 20, Damage = 12, AiId = 14964  },
			new SearchableActor { Name="Bone Musket Outer Wall", Hp = 24, Damage = 12, AiId = 60200  },
			new SearchableActor { Name="Blue Medusa Head Outer Wall", Hp = 12, Damage = 12, AiId = 22572 },
			new SearchableActor { Name="Gold Medusa Head Outer Wall", Hp = 12, Damage = 7, AiId = 22536 },
			new SearchableActor { Name="Dhuron", Hp = 32, Damage = 7, AiId = 49236 },
			new SearchableActor { Name="Flea Man Library", Hp = 11, Damage = 9, AiId = 772  },
			new SearchableActor { Name="Thronweed Library", Hp = 12, Damage = 10, AiId = 56172 },
			new SearchableActor { Name="Flea Armor", Hp = 18, Damage = 17, AiId = 64000 },
			//new SearchableActor { Name="Skeleton Ape Outer Wall", Hp = 10, Damage = 10, AiId = 18916 },
			new SearchableActor { Name="Skeleton Archer Outer Wall", Hp = 10, Damage = 12, AiId = 1432  },
			new SearchableActor { Name="Phantom Skull", Hp = 15, Damage = 14, AiId = 59616 },
			new SearchableActor { Name="Gold Medusa Head Clock Tower", Hp = 12, Damage = 7, AiId = 916  },
			new SearchableActor { Name="Blue Medusa Head Clock Tower", Hp = 12, Damage = 12, AiId = 952  },
			new SearchableActor { Name="Harpy", Hp = 26, Damage = 18, AiId = 43308 },
			new SearchableActor { Name="Karasuman Crow", Hp = 20, Damage = 32, AiId = 49128 },
			new SearchableActor {Hp = 20, Damage = 32, AiId = 50472 },
			new SearchableActor { Name="Flea Rider", Hp = 17, Damage = 18, AiId = 34488 },
			new SearchableActor { Name="Blue Axe Knight Keep", Hp = 42, Damage = 10, AiId = 38568 },
			new SearchableActor { Name="Black Crow", Hp = 15, Damage = 10, AiId = 16344  },
			new SearchableActor { Name="Winged Guard", Hp = 15, Damage = 12, AiId = 14276 },
			new SearchableActor { Name="Bone Halberd", Hp = 30, Damage = 12, AiId = 12196  },
			new SearchableActor { Name="Bat Chapel", Hp = 1, Damage = 16, AiId = 15756 },
			new SearchableActor { Name="Baby Hippogryph", Hp = 20, Damage = 10, AiId = 16340 },
			new SearchableActor { Name="Skelerang Chapel", Hp = 18, Damage = 12, AiId = 18060 },
			new SearchableActor { Name="Bloody Zombie Alchemy Lab", Hp = 24, Damage = 10, AiId = 21864 },
			new SearchableActor { Name="Skelerang Olrox's Quarters", Hp = 18, Damage = 12, AiId = 11068 },
			new SearchableActor { Name="Blade Soldier", Hp = 16, Damage = 15, AiId = 18404 },
			new SearchableActor {Hp = 24, Damage = 12, AiId = 20436 },
			new SearchableActor { Name="Bloody Zombie Olrox's Quarters", Hp = 24, Damage = 10, AiId = 15440 },
			//new SearchableActor { Name="Olrox Bat", Hp = 13, Damage = 21, AiId = 64244 },
			new SearchableActor { Name="Olrox Skull", Hp = 15, Damage = 23, AiId = 62980 },
			new SearchableActor { Name="Spear Guard Caverns", Hp = 20, Damage = 12, AiId = 49068 },
			new SearchableActor { Name="Bat Caverns", Hp = 1, Damage = 16, AiId = 36428 },
			new SearchableActor { Name="Toad", Hp = 10, Damage = 14, AiId = 31116 },
			//new SearchableActor { Name="Trapped Spear Guard Caverns", Hp = 20, Damage = 12, AiId = 33464 },
			new SearchableActor { Name="Frog", Hp = 2, Damage = 13, AiId = 33204 },
			new SearchableActor { Name="Gremlin Mines", Hp = 100, Damage = 20, AiId = 38856 },
			new SearchableActor { Name="Gremlin Caverns", Hp = 100, Damage = 20, AiId = 8932 },
			new SearchableActor { Name="Lossoth", Hp = 99, Damage = 18, AiId = 64232 },
			new SearchableActor { Name="Thornweed Caverns", Hp = 12, Damage = 10, AiId = 22344 },
			new SearchableActor { Name="Granfaloon Zombie", Hp = 10, Damage = 20, AiId = 17872 },
			new SearchableActor {Hp = 22, Damage = 28, AiId = 10100 },
			new SearchableActor { Name="Bomb Knight", Hp = 46, Damage = 37, AiId = 48728 },
			new SearchableActor { Name="Gold Medusa Head Inverted Clock Tower", Hp = 12, Damage = 7, AiId = 45404 },
			new SearchableActor { Name="Tombstone", Hp = 5, Damage = 40, AiId = 54652 },
			new SearchableActor { Name="Balloon Pod", Hp = 3, Damage = 55, AiId = 18024 },
			new SearchableActor { Name="Black Panther", Hp = 35, Damage = 45, AiId = 24640 },
			new SearchableActor { Name="Imp", Hp = 43, Damage = 10, AiId = 14584 },
			new SearchableActor { Name="Blue Medusa Head Death Wing's Lair", Hp = 12, Damage = 12, AiId = 45836 },
			new SearchableActor { Name="Gold Medusa Head Death Wing's Lair", Hp = 12, Damage = 7, AiId = 45800 },
			new SearchableActor { Name="Ghost Dancer", Hp = 30, Damage = 56, AiId = 43916 },
			new SearchableActor { Name="Werewolf Inverted Colosseum", Hp = 280, Damage = 40, AiId = 29328 },
			//new SearchableActor { Name="Zombie Trevor", Hp = 180, Damage = 40, AiId = 37884 },
			new SearchableActor {Hp = 10, Damage = 66, AiId = 14076 },
			new SearchableActor {Hp = 43, Damage = 10, AiId = 2536 },
			new SearchableActor {Hp = 12, Damage = 7, AiId = 9876 },
			new SearchableActor { Name="Corpseweed", Hp = 12, Damage = 10, AiId = 27132 },
			new SearchableActor { Name="Schmoo", Hp = 50, Damage = 40, AiId = 64648 },
			new SearchableActor { Name="Blue Venus Weed Rose", Hp = 1, Damage = 70, AiId = 33952 },
			new SearchableActor {Hp = 46, Damage = 37, AiId = 39840 },
			new SearchableActor {Hp = 10, Damage = 100, AiId = 38772 },
			new SearchableActor { Name="Gaibon Inverted Mine", Hp = 200, Damage = 7, AiId = 27600 },
			new SearchableActor { Name="Slogra Inverted Mine", Hp = 200, Damage = 6, AiId = 23676 },
			new SearchableActor { Name="Death Sickle", Hp = 0, Damage = 55, AiId = 50968 },
			new SearchableActor { Name="Bat Inverted Mine", Hp = 1, Damage = 16, AiId = 43228 },
			new SearchableActor { Name="Thornweed Inverted Mine", Hp = 12, Damage = 10, AiId = 33052 },
			new SearchableActor { Name="Bat Floating Catacombs", Hp = 1, Damage = 16, AiId = 15412 },
			new SearchableActor { Name="Blood Skeleton Floating Catacombs", Hp = 9, Damage = 8, AiId = 7536  },
			new SearchableActor { Name="Skeleton Floating Catacombs", Hp = 9, Damage = 2, AiId = 8684  },
			//1.1.4 Additions
			new SearchableActor { Name="Merman 1", Hp = 10, Damage = 14, AiId = 11852 },
			new SearchableActor { Name="Merman 2", Hp = 10, Damage = 13, AiId = 19232 },
			new SearchableActor { Name="Wereskeleton", Hp = 33, Damage = 20, AiId = 19008  },
			new SearchableActor { Name="Bone Arc (Skeleton)", Hp = 140, Damage = 20, AiId = 36064  },
			new SearchableActor { Name="Jack O'Bones", Hp = 20, Damage = 40, AiId = 33188 },
			new SearchableActor { Name="Flying Zombie", Hp = 190, Damage = 37, AiId = 31388 },
		};
		public static List<SearchableActor> AcceptedRomhackAmbushEnemies = new List<SearchableActor>
		{
			new SearchableActor { AiId = 25776 },
			new SearchableActor { AiId = 25188 },
			new SearchableActor { AiId = 23212 },
			new SearchableActor { AiId = 42580 },
			new SearchableActor { AiId = 4612  },
			new SearchableActor { AiId = 14308 },
			new SearchableActor { AiId = 31064 },
			new SearchableActor { AiId = 24516 },
			new SearchableActor { AiId = 26412 },
			new SearchableActor { AiId = 17852 },
			new SearchableActor { AiId = 46300 },
			new SearchableActor { AiId = 48588 },
			new SearchableActor { AiId = 30320 },
			new SearchableActor { AiId = 26360 },
			new SearchableActor { AiId = 48588 },
			new SearchableActor { AiId = 51080 },
			new SearchableActor { AiId = 52040 },
			new SearchableActor { AiId = 54896 },
			new SearchableActor { AiId = 14964 },
			new SearchableActor { AiId = 60200 },
			new SearchableActor { AiId = 22572 },
			new SearchableActor { AiId = 22536 },
			new SearchableActor { AiId = 49236 },
			new SearchableActor { AiId = 772   },
			new SearchableActor { AiId = 56172 },
			new SearchableActor { AiId = 64000 },
			new SearchableActor { AiId = 1432  },
			new SearchableActor { AiId = 59616 },
			new SearchableActor { AiId = 916   },
			new SearchableActor { AiId = 952   },
			new SearchableActor { AiId = 43308 },
			new SearchableActor { AiId = 49128 },
			new SearchableActor { AiId = 50472 },
			new SearchableActor { AiId = 34488 },
			new SearchableActor { AiId = 38568 },
			new SearchableActor { AiId = 16344 },
			new SearchableActor { AiId = 14276 },
			new SearchableActor { AiId = 12196 },
			new SearchableActor { AiId = 15756 },
			new SearchableActor { AiId = 16340 },
			new SearchableActor { AiId = 18060 },
			new SearchableActor { AiId = 21864 },
			new SearchableActor { AiId = 11068 },
			new SearchableActor { AiId = 18404 },
			new SearchableActor { AiId = 20436 },
			new SearchableActor { AiId = 15440 },
			//new SearchableActor { Name="Olrox Bat", Hp = 13, Damage = 21, AiId = 64244 },
			new SearchableActor { AiId = 62980 },
			new SearchableActor { AiId = 49068 },
			new SearchableActor { AiId = 36428 },
			new SearchableActor { AiId = 31116 },
			//new SearchableActor { Name="Trapped Spear Guard Caverns", Hp = 20, Damage = 12, AiId = 33464 },
			new SearchableActor { AiId = 33204 },
			new SearchableActor { AiId = 38856 },
			new SearchableActor { AiId = 8932  },
			new SearchableActor { AiId = 64232 },
			new SearchableActor { AiId = 22344 },
			new SearchableActor { AiId = 17872 },
			new SearchableActor { AiId = 10100 },
			new SearchableActor { AiId = 48728 },
			new SearchableActor { AiId = 45404 },
			new SearchableActor { AiId = 54652 },
			new SearchableActor { AiId = 18024 },
			new SearchableActor { AiId = 24640 },
			new SearchableActor { AiId = 14584 },
			new SearchableActor { AiId = 45836 },
			new SearchableActor { AiId = 45800 },
			new SearchableActor { AiId = 43916 },
			new SearchableActor { AiId = 29328 },
			//new SearchableActor { Name="Zombie Trevor", Hp = 180, Damage = 40, AiId = 37884 },
			new SearchableActor { AiId = 14076 },
			new SearchableActor { AiId = 2536  },
			new SearchableActor { AiId = 9876  },
			new SearchableActor { AiId = 27132 },
			new SearchableActor { AiId = 64648 },
			new SearchableActor { AiId = 33952 },
			new SearchableActor { AiId = 39840 },
			new SearchableActor { AiId = 38772 },
			new SearchableActor { AiId = 27600 },
			new SearchableActor { AiId = 23676 },
			new SearchableActor { AiId= 50968  },
			new SearchableActor { AiId = 43228 },
			new SearchableActor { AiId = 33052 },
			new SearchableActor { AiId = 15412 },
			new SearchableActor { AiId = 7536  },
			new SearchableActor { AiId = 8684  },
			//1.1.4 Additions
			new SearchableActor { AiId = 11852 },
			new SearchableActor { AiId = 19232 },
			new SearchableActor { AiId = 19008 },
			new SearchableActor { AiId = 36064 },
			new SearchableActor { AiId = 33188 },
			new SearchableActor { AiId = 31388 },
		};
		public static List<SearchableActor> EnduranceBosses = new List<SearchableActor>
		{
			//new SearchableActor {Name = "Slogra", Hp = 200, Damage = 6, AiId = 18296},  //It always detects Slogra, experimenting with Gaibon clone instead
			new SearchableActor {Name = "Gaibon", Hp = 200, Damage = 7, AiId = 22392},
			new SearchableActor {Name = "Doppleganger 10", Hp = 120, Damage = 7, AiId = 14260},
			new SearchableActor {Name = "Minotaur", Hp = 300, Damage = 20, AiId = 9884},
			new SearchableActor {Name = "Werewolf", Hp = 260, Damage = 20, AiId = 14428},
			new SearchableActor {Name = "Lesser Demon", Hp = 400, Damage = 20, AiId = 56036},
			new SearchableActor {Name = "Karasuman", Hp = 500, Damage = 20, AiId = 43920},
			//new SearchableActor {Name = "Hippogryph", Hp = 800, Damage = 18, AiId = 7188},  //Can trigger the door closing and locking the player on the wrong side.
			new SearchableActor {Name = "Olrox", Hp = 666, Damage = 20, AiId = 54072},
			new SearchableActor {Name = "Succubus", Hp = 666, Damage = 25, AiId = 8452},
			new SearchableActor {Name = "Cerberus", Hp = 800, Damage = 20, AiId = 19772},
			//new SearchableActor {Name = "Granfaloon", Hp = 400, Damage = 30, AiId = 6264},  //Only spawns core, no tentacles or shell
			new SearchableActor {Name = "Richter", Hp = 400, Damage = 25, AiId = 27332},
			new SearchableActor {Name = "Darkwing Bat", Hp = 600, Damage = 35, AiId = 40376},
			//new SearchableActor {Name = "Creature", Hp = 1100, Damage = 30, AiId = 31032},//Hammer doesn't have hitbox and body only does 1 damage
			new SearchableActor {Name = "Doppleganger 40", Hp = 777, Damage = 35, AiId = 11664},
			//new SearchableActor {Name = "Death", Hp = 888, Damage = 35, AiId = 46380},
			new SearchableActor {Name = "Medusa", Hp = 1100, Damage = 35, AiId = 6044},
			new SearchableActor {Name = "Akmodan", Hp = 1200, Damage = 40, AiId = 16564},
			new SearchableActor {Name = "Sypha", Hp = 1000, Damage = 9, AiId = 30724},
			new SearchableActor {Name = "Shaft", Hp = 1300, Damage = 40, AiId = 43772}
		};
		public static List<SearchableActor> EnduranceAlternateBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Hippogryph", Hp = 800, Damage = 18, AiId = 7188},
			new SearchableActor {Name = "Scylla", Hp = 200, Damage = 16, AiId = 10988},
			new SearchableActor {Name = "Granfaloon", Hp = 400, Damage = 30, AiId = 6264},
			new SearchableActor {Name = "Creature", Hp = 1100, Damage = 30, AiId = 31032},
			new SearchableActor {Name = "Death", Hp = 888, Damage = 35, AiId = 46380},
			new SearchableActor {Name = "Beelzebub", Hp = 2000, Damage = 60, AiId = 11356},
			new SearchableActor {Name = "Dracula", Hp = 10000, Damage = 39, AiId = 56220},
		};
		public static List<SearchableActor> EnduranceRomhackBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Gaibon", AiId = 22392},
			new SearchableActor {Name = "Doppleganger 10", AiId = 14260},
			new SearchableActor {Name = "Minotaur", AiId = 9884},
			new SearchableActor {Name = "Werewolf", AiId = 14428},
			new SearchableActor {Name = "Lesser Demon", AiId = 56036},
			new SearchableActor {Name = "Karasuman", AiId = 43920},
			new SearchableActor {Name = "Olrox", AiId = 54072},
			new SearchableActor {Name = "Succubus", AiId = 8452},
			new SearchableActor {Name = "Cerberus", AiId = 19772},
			new SearchableActor {Name = "Richter", AiId = 27332},
			new SearchableActor {Name = "Darkwing Bat", AiId = 40376},
			new SearchableActor {Name = "Creature", AiId = 31032},
			new SearchableActor {Name = "Doppleganger 40", AiId = 11664},
			new SearchableActor {Name = "Medusa", AiId = 6044},
			new SearchableActor {Name = "Akmodan", AiId = 16564},
			new SearchableActor {Name = "Sypha", AiId = 30724},
			new SearchableActor {Name = "Shaft", AiId = 43772}
		};
		public static List<SearchableActor> EnduranceAlternateRomhackBosses = new List<SearchableActor>
		{
			new SearchableActor {Name = "Hippogryph", AiId = 7188},
			new SearchableActor {Name = "Scylla", AiId = 10988},
			new SearchableActor {Name = "Granfaloon", AiId = 6264},
			new SearchableActor {Name = "Creature", AiId = 31032},
			new SearchableActor {Name = "Death", AiId = 46380},
			new SearchableActor {Name = "Beelzebub", AiId = 11356},
			new SearchableActor {Name = "Dracula", AiId = 56220},
		};
		public static SearchableActor GalamothTorsoActor = new SearchableActor { Hp = 12000, Damage = 50, AiId = 23936 };
		public static SearchableActor GalamothHeadActor = new SearchableActor { Hp = 32767, Damage = 50, AiId = 31516 };
		public static SearchableActor GalamothPartsActors = new SearchableActor { Hp = 12000, Damage = 50, AiId = 31516 };
		public static SearchableActor ShaftOrbActor = new SearchableActor { Hp = 10, Damage = 0, AiId = 0 };

		public static readonly string[] AcceptedMusicTrackTitles =
   {
			"lost painting",
			"cursed sanctuary",
			"requiem for the gods",
			"rainbow cemetary",
			"wood carving partita",
			"crystal teardrops",
			"marble galery",
			"dracula castle",
			"the tragic prince",
			"tower of evil mist",
			"doorway of spirits",
			"dance of pearls",
			"abandoned pit",
			"heavenly doorway",
			"festival of servants",
			"dance of illusions",
			"prologue",
			"wandering ghosts",
			"doorway to the abyss",
			"metamorphosis",
			"metamorphosis 2",
			"dance of gold",
			"enchanted banquet",
			"prayer",
			"death's ballad",
			"blood relations",
			"finale toccata",
			"black banquet",
			"silence",
			"nocturne",
			"moonlight nocturne"
		};
		public static readonly Dictionary<string, string> AlternateTrackTitles = new Dictionary<string, string>
		{
			{ "deaths ballad", "death's ballad" },
			{ "death ballad", "death's ballad" },
			{ "poetic death", "death's ballad" },
			{ "illusionary dance", "dance of illusions" },
			{ "dracula", "dance of illusions" },
			{ "nocturne in the moonlight", "moonlight nocturne" },
			{ "dracula's castle", "dracula castle" },
			{ "draculas castle", "dracula castle" },
			{ "castle entrance", "dracula castle" },
			{ "entrance", "dracula castle" },
			{ "tower of mist", "tower of evil mist" },
			{ "outer wall", "tower of evil mist" },
			{ "library", "wood carving partita" },
			{ "alchemy lab", "dance of gold" },
			{ "alchemy laboratory", "dance of gold" },
			{ "chapel", "requiem for the gods" },
			{ "royal chapel", "requiem for the gods" },
			{ "crystal teardrop", "crystal teardrops" },
			{ "caverns", "crystal teardrops" },
			{ "underground caverns", "crystal teardrops" },
			{ "departer way", "abandoned pit" },
			{ "pit", "abandoned pit" },
			{ "mines", "abandoned pit" },
			{ "mine", "abandoned pit" },
			{ "catacombs", "rainbow cemetary" },
			{ "lost paintings", "lost painting" },
			{ "antichapel", "lost painting" },
			{ "reverse caverns", "lost painting" },
			{ "forbidden library", "lost painting" },
			{ "waltz of pearls", "dance of pearls" },
			{ "olrox's quarters", "dance of pearls" },
			{ "olroxs quarters", "dance of pearls" },
			{ "olrox quarters", "dance of pearls" },
			{ "cursed zone", "cursed sanctuary" },
			{ "floating catacombs", "cursed sanctuary" },
			{ "reverse catacombs", "cursed sanctuary" },
			{ "demonic banquet", "enchanted banquet" },
			{ "medusa", "enchanted banquet" },
			{ "succubus", "enchanted banquet" },
			{ "colosseum", "wandering ghosts" },
			{ "wandering ghost", "wandering ghosts" },
			{ "pitiful scion", "the tragic prince" },
			{ "clock tower", "the tragic prince" },
			{ "tragic prince", "the tragic prince" },
			{ "alucard", "the tragic prince" },
			{ "door to the abyss", "doorway to the abyss" },
			{ "doorway to heaven", "heavenly doorway" },
			{ "keep", "heavenly doorway" },
			{ "castle keep", "heavenly doorway" },
			{ "divine bloodlines", "blood relations" },
			{ "strange bloodlines", "blood relations" },
			{ "richter belmont", "blood relations" },
			{ "richter", "blood relations" },
		};


		public static string[] BuggyQuickSwapWeapons = {
			"Shuriken",
			"Cross shuriken",
			"Buffalo star",
			"Flame star",
			"Boomerang",
			"Javelin",
			"Heaven sword",
			"Alucard sword",
			"Chakram",
			"Fire boomerang",
			"Osafune katana",
			"Masamune",
			"Runesword",
			"Claymore",
			"Flamberge",
			"Zweihander",
			"Obsidian sword",
			"Estoc",
			"Shotel"
		};

		public static string[] TwoHandedWeapons = {
			"Muramasa",
			"Namakura",
			"Red rust",
			"Takemitsu",
			"Nunchuku",
			"Claymore",
			"Sword of dawn",
			"Flamberge",
			"Katana",
			"Zweihander",
			"Estoc",
			"Obsidian sword",
			"Osafune katana",
			"Masamune",
			"Yasutsuna",
			"Great sword",
		};

		public static string[] ThrustWeapons = {
			"Claymore",
			"Flamberge",
			"Estoc",
			"Zweihander",
			"Obsidian sword",
		};

		public static int[] alucardColors =
		{
			//33024, 8100 - Default coloring for reference
			//33126, Used in speed overdrive
			//601, //      - Aqua
			33026,//8102 - All Black, Blue Outline
			33030,//8106 - Sunny
			33137,//8171 - Classic Alucard
			33152,//8180 - Red Highlight Bat Color
			33154,//8182 - Halloween-O'Card
			33155,//8183 - Normal Blue
			33193,//81A9 - Orange with Yellow Outline
			33216,//81C0 - Stained Glass Horrendous
			33266,//81F2 - Dark Yellow, Red Cape
			33456,//82B0 - Purple as Heck
			33467,//82BB - PurpleCard w/ Red Cape
			
		};

		public static int[] alucardColorsFirstCastle =
		{
			//600,  //     - Deep Blue
			33024,//8100 - Blue/Yellow Cape
			33028,//8104 - Redish
			33031,//8107 - Golden Bat
			33153,//8181 - Grey + Red
			33172,//8194 - Horrendous Blue and Orange
			33173,//8195 - Dark Grey, Blue Cape
			33183,//819F - Grey
			33269,//81F5 - Ice
		};

		public static int[] alucardColorsSecondCastle =
		{
			33027,//8103 - All Black, Grey Outline
			33032,//8108 - Dark Yellow
			33033,//8109 - Gold Bat Alt.
			33157,//8185 - Teal + Grey
			33177,//8199 - Less Horrendous Blue and Orange
			33182,//819E - Dark Grey
			33198,//81AE - Feeling Dark Orange
			
			//33088,//8140 - Purple Tint Alucard	
			//33090,//8142 - Grey / Green / Blue	
			//33092,//8144 - Feeling Orange
			//33093,//8145 - Feeling Blue
			//33232,//81D0 - Alt. Vanilla Look
			//33234,//81D2 -Grey w/ Pink
			//33265,//81F1 - Feeling Purple
			//33274,//81FA- Dark Grey, Used in Hex
			//33281,//8201 - Clay w/ Hidden Red Cape
			//33281,//8208 - Clay w/ Hidden Red Cape
			//33320,//8228 - Vanilla w/ Green Cape
			//33336,//8238 - Hot Dog Cape
			//33337,//8239 - Feeling Blue
			//33339,//823B - Feeling Pale Blue
			//33355,//8237 - Cyan Yellow
			//33337,//8239 - Bright Yellow
			//33357,//824D - Feeling Green
			//33365,//8255 - Muddy Purple
			//33383,//8267 - Feeling Gold
			//33472,//82C0 - Ghost Alucard
			//33496,//82D7 - Game Boy Color-Card
			//33513,//82E9 - Shapeshift: Blue or Purple w/ Yellow Cape
			//33518,//82EE - Green Slime?
		};

		public static int[] richterColors =
		{
			//33056,// 8120 - Default coloring for reference
			309,//0135 - Richter Alternate //
			330,//014A - 80s Punk //Not legal Ricter Color?
			33057,// 8121 - Green Richter
			33058,// 8122 - Sunburst Richter
			33060,// 8124 - Monochrome Richter
			33062,// 8126 - Washed Out Richter
			33078,// 8136 - Red Richter
			33079,// 8137 - Black Richter
			33138,// 8172 - Legend of Richter
			33177,// 8199 - Yeti Richter
			33184,// 81A0 - Orange Richter
			33199,// 81AF - Grey Richter
			33273,// 81F9 - Zombie Richter
		};

		public static int[] enemyRichterColors =
		{
			
			33138,// 8172 - Legend of Richter
			33199,// 81AF - Grey Richter
			33273,// 81F9 - Zombie Richter
			33184,// 81A0 - Orange Richter
			33056,// 8220 - Default Richter
			33057,// 8221 - Green Richter
			33058,// 8222 - Sunburst Richter
			33060,// 8224 - Monochrome Richter
			33062,// 8226 - Washed Out Richter
			33078,// 8236 - Red Richter
			33079,// 8237 - Black Richter
			//Enemy Richter Only
			550,  //      - Pale Richter
			588,  //      - 8-Bit Richter
			33030,// 8106 - Sunburst Richter
			33089,// 8141 - Sunburnt Richter
			33090,// 8142 - Frozen Richter
			33138,// 8199 - Yeti Richter
		};

		

		public static int[] axeArmorStone =
		{
			114,	//Stone Sword
			13,		//Medusa Shield
		};

		public static int[] axeArmorCutDark =
		{
			//110,	//Mormegeil
			83,		//Tyrfing
		};

		public static int[] axeArmorOneHandedCut =
		{
			86,		//Gladius
			87,		//Scimitar
			88,		//Cutlass
			89,		//Saber
			90,		//Falchion
			91,		//Broadsword
			92,		//Bekatowa
			93,		//Damascus Sword
			94,		//Hunter Sword
			96,		//Bastard Sword
			99,		//Talwar
			104,	//Sword of Hador
			105,	//Luminus
			106,	//Harper
			108,	//Gram
			118,	//Dark Blade
			125,	//Badelaire
			124,	//Mablung
			126,	//Sword Familiar
		};

		public static int[] axeArmorOneHandedDoubleCut =
		{
			//Short Swords
			19,		//Short sword
			109,	//Jewel sword
			28,		//Shotel
			23,		//Rapier
			22,		//Were bane
			//Swords
			168,	//Alucart Sword
			121,	//Gurthang
			122,	//Mourneblade
			123,	//Alucard Sword
		};

		public static int[] axeArmorOneHandedTripleCut =
		{
			18,		//Basilard
			20,		//Combat Knife
			136,	//Holbein
			163,	//Vorpal
		};

		public static int[] axeArmorTwoHandedCut =
		{
			98,		//Claymore
			101,	//Flamberge
			17,		//Sword of Dawn
			95,		//Estoc
			103,	//Zwei Hander
			107,	//Obsidian Sword
			127,	//Greatsword
			100,	//Katana
		};

		public static int[] axeArmorTwoHandedDoubleCut =
		{
			139,	//Osafune Katana
			165,	//Yasatsuna
			140,	//Masamune
		};

		public static int[] axeArmorTwoHandedCurse =
		{
			26,		//Red Rust
			84,		//Namakura
			141,	//Muramasa
		};

		public static int[] axeArmorTwoHandedHit =
		{
			27,		//Takemitsu
		};

		public static int[] axeArmorTwoHandedDoubleHit =
		{
			21,		//Nunchaku
		};

		public static int[] axeArmorOneHandedHit =
		{
			128,	//Mace
			129,	//Morning Star
			4,		//Shield Rod
		};
		public static int[] axeArmorOneHandedDoubleHit =
		{
			130,    //Holy Rod
			131,	//Star Flail
			132,	//Moon Rod
		};
		public static int[] axeArmorOneHandedTripleHit =
		{
			85,		//Knuckle Duster
			137,	//Blue Knuckles
			97,		//Jewel Knuckles
			102,	//Iron Fist
			120,	//Fist of Tulkas
		};

		public static int[] axeArmorThrowableWeapon =
		{
			133,	//Chakram
			119,	//Heaven Sword
			143,	//Rune Sword
		};

		public static int[] axeArmorShields =
		{
			167,	//Alucart Shield
			5,		//Leather Shield
			6,		//Knight Shield
			7,		//Iron Shield
			8,		//AxeLord Shield
			9,		//Herald Shield
			10,		//Dark Shield
			11,		//Goddess Shield
			12,		//Shaman Shield
			13,		//Medusa Shield
			14,		//Skull Shield
			15,		//Fire Shield
			16,		//Alucard Shield	
		};

		public static int[] axeArmorProjectiles =
		{
			1,		//Monster vial 1
			2,		//Monster vial 2
			3,		//Monster vial 3
			24,		//Karma Coin
			25,		//Magic Missile
			71,		//Neutron Bomb
			72,		//Power of Sire
			73,		//Pentagram
			74,		//Bat Pentagram
			75,		//Shuriken
			76,		//Cross Shuriken
			77,		//Buffalo star
			78,		//Flame star
			79,		//TNT
			80,		//Bwaka Knife
			81,		//Boomerang
			82,		//Javelin
			134,	//Fire boomerang
			135,	//Iron ball
			138,	//Dynamite
		};

		public static int[] axeArmorProjectilesCut =
		{
			25,		//Magic Missile
			75,		//Shuriken
			76,		//Cross Shuriken
			77,		//Buffalo star
			80,		//Bwaka Knife
		};

		public static int[] axeArmorProjectilesDoubleCut =
		{
			82,		//Javelin
		};

		public static int[] axeArmorProjectilesHit =
		{
			71,		//Neutron Bomb
			73,		//Pentagram
			74,		//Bat Pentagram
			135,	//Iron ball
		};

		public static int[] axeArmorProjectilesDoubleHit =
		{
			81,		//Boomerang
		};

		public static int[] axeArmorProjectilesTripleHit =
		{
			1,		//Monster vial 1
			2,		//Monster vial 2
			3,		//Monster vial 3
		};

		public static int[] axeArmorProjectilesQuadrupleHit =
		{
			72,		//Power of Sire
		};

		public static int[] axeArmorProjectilesThunder =
		{
			24,		//Karma Coin
		};

		public static int[] axeArmorProjectilesFire =
		{
			78,		//Flame star
			79,		//TNT
		};

		public static int[] axeArmorProjectilesDoubleFire =
		{
			134,	//Fire boomerang
		};

		public static int[] axeArmorProjectilesTripleFire =
		{
			138,	//Dynamite
		};

		public static int[] axeArmorFoodMinorHealing =
		{ // 20 HP
			32,		//Grapes
			29,		//Orange
			31,		//Banana
			34,		//Pineapple
			30,		//Apple
			33,		//Strawberry
			65,		//Chinese Bun
			58,		//Green Tea
			39,		//Shortcake
			40,		//Tart
			64,		//Red Bean Bun
			38,		//Cheesecake
			43,		//Ice Cream
			57,		//Barley Tea
			41,		//Parfait
			42,		//Pudding
			63,		//Pork Bun
			47,		//Cheese
			44,		//Frankfurter
			56,		//Grape Juice
			45,		//Hamburger
			61,		//Miso Soup
		};

		public static int[] axeArmorFoodModerateHealing =
		{ // 32 HP
			46,		//Pizza
			48,		//Ham and Eggs
			49,		//Omelette
			50,		//Morning Set
			51,		//Lunch A
			52,		//Lunch B
			55,		//Spaghetti
			53,		//Curry Rice
			54,		//Gyros Plate
			66,		//Dim Sum Set
			59,		//Ramen
			60,		//Natou Soup
			37,		//Shittake
			59,		//Natou
		};

		public static int[] axeArmorFoodMajorHealing =
		{ // 80 HP
			35,		//Peanuts
			67,		//Pot Roast
			69,		//Turkey
		};

		public static int[] axeArmorFoodMaxHealing =
		{ // 100 HP
			68,		//Sirloin
			62,		//Sushi
			36,		//Toadstool
		};

		public static int RichterRoomMapMinX = 31;
		public static int RichterRoomMapMaxX = 34;
		public static int RichterRoomMapY = 8;
		public static int EntranceCutsceneMapMinX = 0;
		public static int EntranceCutsceneMapMaxX = 18;
		public static int EntranceCutsceneMapY = 44;
		public static int GalamothRoomMapMinX = 44;
		public static int GalamothRoomMapMaxX = 45;
		public static int GalamothRoomMapMinY = 12;
		public static int GalamothRoomMapMaxY = 13;
		public static int SuccubusMapX = 0;
		public static int SuccubusMapY = 0;

		#region mayhem
		public static string MayhemName = "Mayhem";
		public static string AutoMayhemName = "Auto Mayhem";
		public static float SuperStatsDownFactor = 0.5F;
		public static float SpeedDashFactor = 1.8F;
		public static float SuperUnderwaterFactor = 0.5F;
		public static int UnderwaterQueueIntervalEnd = 3;
		public static int SpeedQueueIntervalStart = 8;
		public static uint SuperRegenExtraGain = 2u;
		public static int BoonItemRetryCount = 15;
		
		public static uint AxeArmorStr = 10u;
		public static uint AxeArmorInt = 10u;
		public static int AxeArmorKickyFeetDamageType = 1;
		public static int AxeArmorVanillaJumpSpeed = 190;
		public static int AxeArmorBaseJumpSpeed = 200;
		public static int AxeArmorMaxJumpSpeed = 255;
		public static int FacePlantAirSpeed = 65000;
		public static int FacePlantGroundSpeed = 90000;

		public static float BuffHPMultiplier = 2F;
		public static uint BuffStrDarkMetamorphosis = 90;
		public static uint BuffStr = 10u;
		public static uint BuffCon = 200u;
		public static uint BuffInt = 20u;
		public static uint BuffLck = 120u;
		public static uint UnarmedInvincibility = 3;
		public static uint UnarmedStr = 10u;
		public static uint UnarmedCon = 100u;
		
		public static uint Rushdown1Con = 600u;
		public static uint Rushdown2Con = 12000u;
		public static uint Rushdown1KFCon = 1200u;
		public static uint Rushdown2KFCon = 15000u;
		public static uint StatOverflowLimit = 50000u;

		public static int highFamiliarXP = 10000;
		public static int medFamiliarXP = 7000;
		public static int lowFamiliarXP = 5904;
		#endregion

		#region Axe Armor Specific

		//Jump
		public static int AxeArmorJumpBaseSpeed = -210000;
		public static int AxeArmorJumpAcceleration = 10000;
		public static int AxeArmorGravityJumpBaseSpeed = -420000;
		public static int AxeArmorGravityJumpAcceleration = -10000;
		public static int AxeArmorGravityJumpAirControl = 1006829555;
		public static int AxeArmorVerticalJumpAirControl = 1006829563;

		//Float
		public static int AxeArmorJumpFloatBaseSpeed = -300000;
		public static int AxeArmorUnderwaterBaseSpeed = 13500;
		public static int AxeArmorFloatBaseSpeed = 9000;
		public static int AxeArmorReducedFallSpeed = 450000;
		public static int AxeArmorFastFallSpeed = 800000;

		//Flight
		public static int AxeArmorMistCeilingFallSpeed = 300000;
		public static int AxeArmorMistCeilingSpeed = -11250;
		public static int AxeArmorMistFlightSpeed = -80000;
		public static int AxeArmorMistFlightUpSpeed = -140000;
		public static int AxeArmorMistFlightFloatSpeed = 1000;
		public static int AxeArmorFlightUpSpeed = -200000;

		//Wolf
		public static int AxeArmorWolfBaseAcceleration = 4000;
		public static int AxeArmorWolfMaxAcceleration = 6000;
		public static int AxeArmorWolfBaseDeacceleration = 2250;
		public static int AxeArmorWolfMaxDeacceleration = 3500;
		public static int AxeArmorWolfMinStopSpeed = 140000;
		public static int AxeArmorWolfMinRunSpeed = 260000;
		public static int AxeArmorWolfMaxRunSpeed = 380000;
		public static int AxeArmorWolfMaxDashSpeed = 500000;
		public static int AxeArmorWolfMaxMayhemSpeed = 600000;
		public static int AxeArmorWolfMaxMayhemSuperSpeed = 700000;

		//2400, 2400,1000,1000
		public static int EnableClipDirection = 9216;
		public static int EnableCeilingClip = 9216;
		public static int EnableLeftClip = 4096;
		public static int EnableRightClip = 4096;

		//1440, 1202,1602,1602
		public static int DisableClipDirection = 5184;
		public static int DisableCeilingClip = 4610;
		public static int DisableRightClip = 5634;
		public static int DisableLeftClip = 5634;

		public static int AxeArmorHand1EffectAddress = 476724;
		public static int AxeArmorHand2EffectAddress = 476912;
		public static int AxeArmorEffectStartAddress = 477100;
		//public static int AxeArmorHeartStartAddress = 477852;
		public static int AxeArmorHeartStartAddress = 477476;

		public const string AxeArmorLibrary = "AxeArmorLibrary";
		public const string AxeArmorHeartName = "AxeArmorHeart";
		public const string AxeArmorHeartLockName = "AxeArmorHeartLock";
		public const string AxeArmorEffectName = "AxeArmorEffect";

		public const string AxeArmorColorName = "Axe Armor Color";
		public const string RemoveEntityName = "Remove Entity";
		public const string OptionMenuName = "Menu";

		public static uint SpirtOrbHeartMPBoost = 15;
		public static uint FaerieScrollHPBoost = 35;
		public static uint HolySymbolHPBoost = 30;
		public static uint JewelOfOpenMPBoost = 25;
		public static uint MermanStatueHeartBoost = 20;

		#endregion

		#region Legacy
		public static int SlowQueueIntervalEnd = 3;
		public static int FastQueueIntervalStart = 8;
		//public static uint SuperThirstExtraDrain = 2u;
		public static int HelpItemRetryCount = 15;

		public const uint FireBallSpeedLeft = 0xFFFF;
		public const uint FireBallSpeedRight = 0x0000;
		public const string FireBallSpeedName = "FireBallSpeed";

		public static readonly SearchableActor SpiritActor = new SearchableActor { Hp = 0, AiId = 39012 };
		public const uint SpiritPalette = 0x5B;
		public const uint SpiritLockOn = 2;
		public const string SpiritLockOnName = "SpiritLockOn";

		public static readonly List<byte> FireballEntityBytes = new List<byte> { 
			0x00, 0xC0, 0xB2, 0x00, 0x00, 
			0x00, 0xB3, 0x00, 0x00, 0x80, 
			0x02, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 
			0x01, 0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x00, 
			0x00, 0x96, 0x00, 0x1A, 0x00,
			0xDC, 0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x10, 
			0x08, 0x00, 0x00, 0x03, 0x00,
			0x02, 0x00, 0x00, 0x00, 0x28, 
			0x00, 0x00, 0x80, 0x00, 0x00, 
			0x04, 0x04, 0x00, 0x14, 0x00,
			0x00, 0x98, 0x07, 0x0B, 0x80, 
			0x04, 0x00, 0x01, 0x00, 0x09, 
			0x00, 0x05, 0x00, 0x08 };
		public static readonly List<byte> SampleEntityBytes = new List<byte> {
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x0A, 0x00, 0x00, 0x00, //Offset 32 = Activator = 0A = 10/Axe
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,//01=momentum (4 stops)
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,//04->14 = Damage Interval
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00 };

		public static readonly List<byte> DarkFireballEntityBytes = new List<byte> { 
			0xE0, 0xBE, 0x7D, 0x00, 
			0x00, 0x00, 0xAF, 0x00, 
			0x00, 0x00, 0x00, 0xFF, 
			0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, //20 
			0x00, 0x00, 0x03, 0x00, 
			0x00, 0x04, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x08, //32 = Activator
			0x00, 0x00, 0x00, 0x00, 
			0x96, 0x00, 0x1B, 0x00, //40
			0x40, 0x78, 0x12, 0x80, 
			0x01, 0x00, 0x00, 0x00, //01=momentum (4 stops)
			0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x10, 0x08, 
			0x00, 0x00, 0x03, 0x00, //60
			0x02, 0x00, 0x00, 0x00, //66 = Damage 
			0x21, 0x00, 0x00, 0x80, //68-69 = Damage Type
			0x00, 0x00, 0x08, 0x08, //74=75 = Hitbox
			0x00, 0x14, 0x00, 0x00, //78 04->14 = Damage Interval
			0xC8, 0x07, 0x0B, 0x80, 
			0x17, 0x00, 0x01, 0x00, 
			0x09, 0x00, 0x09, 0x00, 
			0x08 };

		public static readonly List<byte> AxeEntityBytes = new List<byte> { 
			0x00, 0xC0, 0x8E, 0x00, 
			0x00, 0x70, 0x74, 0x00, 
			0x00, 0x00, 0x02, 0x00, 
			0x00, 0xDC, 0xFB, 0xFF, 
			0x00, 0x00, 0x00, 0x00, 
			0x01, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x0A, 
			0x00, 0x00, 0x00, 0x00, 
			0x94, 0x00, 0x0A, 0x00, 
			0x30, 0x53, 0x12, 0x80, 
			0x01, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x82, 0x0C, 
			0x00, 0x00, 0x07, 0x00,
			0x02, 0x00, 0x00, 0x00, 
			0x1C, 0x00, 0x40, 0x10,
			0x00, 0x00, 0x0C, 0x0C,
			0x00, 0x20, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 
			0x03, 0x00, 0x01, 0x00, 
			0x00, 0x00, 0x00, 0x00, 
			0x04 };

		public static readonly List<byte> AxeLongerEntityBytes = new List<byte> { 
			0x00, 0xC0, 0x8E, 0x00, 0x00, 0x70, 0x74, 0x00, 0x00, 0x00, 0x02, 0x00,
			0x00, 0xDC, 0xFB, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x94, 0x00, 0x0A, 0x00, 0x30, 0x53, 0x12, 0x80, 0x01, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x82, 0x0C, 0x00, 0x00, 0x07, 0x00,
			0x02, 0x00, 0x00, 0x00, 0x1C, 0x00, 0x40, 0x10, 0x00, 0x00, 0x0C, 0x0C,
			0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x01, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x83, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x05, 0x02, 0x00, 
			0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7F, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02};

		public static readonly List<byte> EnemyAxeEntityBytes = new List<byte> { 0x00, 0x00, 0x95, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x80, 0x09, 0x00, 0x00, 0x00, 0x00, 0x88, 0x00, 0x21, 0x00, 0xA0, 0xB8, 0x1C, 0x80, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x8D, 0x00, 0x00, 0x07, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x41, 0x00, 0x00, 0x00, 0x08, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x80, 0x2F, 0x00, 0x00 };
		public static readonly List<byte> EnemyAxeLongerEntityBytes = new List<byte> { 0x00, 0x00, 0x95, 0x00, 0x00, 0x00, 0x90, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x02, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x80, 0x09, 0x00, 0x00, 0x00, 0x00, 0x88, 0x00, 0x21, 0x00, 0xA0, 0xB8, 0x1C, 0x80, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x8D, 0x00, 0x00, 0x07, 0x00, 0x0F, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x41, 0x00, 0x00, 0x00, 0x08, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x80, 0x2F, 0x00, 0x00, 0x00, 0x48, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x43, 0x04, 0x00, 0x00 };
		#endregion

		public static uint ShaftKhaosHp = 25;
		public static uint ShaftAxeArmorHp = 13;
		public static uint ShaftMayhemHp = 20;
		public static uint GalamothMayhemHp = 1500;
		public static uint GalamothMayhemPositionOffset = 100;
		public static uint GalamothKhaosHp = 2000;
		public static uint GalamothKhaosPositionOffset = 100;
		
		public static int SaveIcosahedronFirstCastle = 0xBCAA;
		public static int SaveIcosahedronSecondCastle = 0x1150;
	}
}
