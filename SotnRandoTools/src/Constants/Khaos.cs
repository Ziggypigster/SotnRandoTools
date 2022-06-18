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
		public static List<MapLocation> ClockRoom = new List<MapLocation>
		{
			new MapLocation{X = 32, Y = 26, SecondCastle = 0},
			new MapLocation{X = 31, Y = 37, SecondCastle = 1}
		};
		public static List<MapLocation> RichterRooms = new List<MapLocation>
		{
			new MapLocation{X = 31, Y = 8, SecondCastle = 0},
			new MapLocation{X = 32, Y = 8, SecondCastle = 0},
			new MapLocation{X = 33, Y = 8, SecondCastle = 0},
			new MapLocation{X = 34, Y = 8, SecondCastle = 0},
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
		public static List<MapLocation> MistGateRoom = new List<MapLocation>
		{
			new MapLocation{X = 46, Y = 21, SecondCastle = 0}, //Library
			new MapLocation{X = 47, Y = 21, SecondCastle = 0}, //Library
			new MapLocation{X = 13, Y = 15, SecondCastle = 0}, //Silver Ring
			new MapLocation{X = 14, Y = 15, SecondCastle = 0}, //Silver Ring
			new MapLocation{X = 60, Y = 27, SecondCastle = 0}, //Outer Wall
			new MapLocation{X = 3, Y = 36, SecondCastle = 1}, //Reverse Library
			new MapLocation{X = 3, Y = 36, SecondCastle = 1}, //Reverse Silver Ring
			new MapLocation{X = 3, Y = 36, SecondCastle = 1}, //Reverse Outer Wall
		};
		public static List<MapLocation> RewindBanRoom = new List<MapLocation>
		{
			new MapLocation{X = 40, Y = 39, SecondCastle = 0}, //Scylla Room 
			//new MapLocation{X = 31, Y = 30, SecondCastle = 1}, //???
		};
		public static List<MapLocation> RewindUnbanRoom = new List<MapLocation>
		{
			new MapLocation{X = 39, Y = 39, SecondCastle = 0}, //Outside Scylla Room
			new MapLocation{X = 50, Y = 20, SecondCastle = 0}, //Room Outside Librarian
		};
		public static List<MapLocation> GalamothRooms = new List<MapLocation>
		{
			new MapLocation{X = 44, Y = 12, SecondCastle = 1},
			new MapLocation{X = 45, Y = 12, SecondCastle = 1},
			new MapLocation{X = 44, Y = 13, SecondCastle = 1},
			new MapLocation{X = 45, Y = 13, SecondCastle = 1},
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
			new SearchableActor { Name="Skeleton Floating Catacombs", Hp = 9, Damage = 2, AiId = 8684  }
		};
		public static List<SearchableActor> AcceptedRomhackAmbushEnemies = new List<SearchableActor>
		{
			new SearchableActor {AiId = 25776 },
			new SearchableActor {AiId = 25188 },
			new SearchableActor {AiId = 23212  },
			new SearchableActor {AiId = 42580  },
			new SearchableActor {AiId = 4612 },
			new SearchableActor {AiId = 14308  },
			new SearchableActor {AiId = 31064  },
			new SearchableActor {AiId = 24516 },
			new SearchableActor {AiId = 26412 },
			new SearchableActor {AiId = 17852  },
			new SearchableActor {AiId = 46300  },
			new SearchableActor {AiId = 48588 },
			new SearchableActor {AiId = 30320  },
			new SearchableActor {AiId = 26360 },
			new SearchableActor {AiId = 48588  },
			new SearchableActor {AiId = 51080  },
			new SearchableActor {AiId = 52040  },
			new SearchableActor {AiId = 54896 },
			new SearchableActor {AiId = 14964  },
			new SearchableActor {AiId = 60200  },
			new SearchableActor {AiId = 22572 },
			new SearchableActor {AiId = 49236 },
			new SearchableActor {AiId = 772  },
			new SearchableActor {AiId = 56172 },
			new SearchableActor {AiId = 64000 },
			new SearchableActor {AiId = 18916 },
			new SearchableActor {AiId = 1432  },
			new SearchableActor {AiId = 59616 },
			new SearchableActor {AiId = 916  },
			new SearchableActor {AiId = 43308 },
			new SearchableActor {AiId = 50472 },
			new SearchableActor {AiId = 34488 },
			new SearchableActor {AiId = 38568 },
			new SearchableActor {AiId = 16344  },
			new SearchableActor {AiId = 14276 },
			new SearchableActor {AiId = 12196  },
			new SearchableActor {AiId = 15756 },
			new SearchableActor {AiId = 18060 },
			new SearchableActor {AiId = 21864 },
			new SearchableActor {AiId = 11068 },
			new SearchableActor {AiId = 18404 },
			new SearchableActor {AiId = 20436 },
			new SearchableActor {AiId = 15440 },
			new SearchableActor {AiId = 49068 },
			new SearchableActor {AiId = 36428 },
			new SearchableActor {AiId = 31116 },
			new SearchableActor {AiId = 33464 },
			new SearchableActor {AiId = 33204 },
			new SearchableActor {AiId = 38856 },
			new SearchableActor {AiId = 8932 },
			new SearchableActor {AiId = 64232 },
			new SearchableActor {AiId = 22344 },
			new SearchableActor {AiId = 17300 },
			new SearchableActor {AiId = 10100 },
			new SearchableActor {AiId = 48728 },
			new SearchableActor {AiId = 45404 },
			new SearchableActor {AiId = 54652 },
			new SearchableActor {AiId = 18024 },
			new SearchableActor {AiId = 24640 },
			new SearchableActor {AiId = 14584 },
			new SearchableActor {AiId = 45800 },
			new SearchableActor {AiId = 43916 },
			new SearchableActor {AiId = 29328 },
			new SearchableActor {AiId = 14076 },
			new SearchableActor {AiId = 2536 },
			new SearchableActor {AiId = 9876 },
			new SearchableActor {AiId = 27132 },
			new SearchableActor {AiId = 64648 },
			new SearchableActor {AiId = 33952 },
			new SearchableActor {AiId = 39840 },
			new SearchableActor {AiId = 38772 },
			new SearchableActor {AiId = 27600 },
			new SearchableActor {AiId = 23676 },
			new SearchableActor {AiId = 43228 },
			new SearchableActor {AiId = 33052 },
			new SearchableActor {AiId = 15412 },
			new SearchableActor {AiId = 7536  },
			new SearchableActor {AiId = 8684  }
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

		public static int[] alucardColors =
		{
			//33024, 8100 - Default coloring for reference
			//33126, Used in speed overdrive

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
			309,//0135 - Richter Alternate
			330,//014A - 80s Punk
			33057,// 8121 - Green Richter
			33058,// 8122 - Sunburst Richter
			33060,// 8124 - Monochrome Richter
			33062,// 8126 - Washed Out Richter
			33078,// 8136 - Red Richter
			33079,// 8137 - Black Richter
			33138,// 8172 - Legend of Richter
			33199,// 81AF - Grey Richter
			33272,// 81F9 - Zombie Richter
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
		public static float BuffHPMultiplier = 2F;


		public static uint BuffStrDarkMetamorphosis = 90;
		public static uint BuffStr = 10u;
		public static uint BuffCon = 100u;
		public static uint BuffInt = 20u;
		public static uint BuffLck = 120u;
		public static uint UnarmedInvincibility = 3;
		public static uint UnarmedStr = 10u;
		public static uint UnarmedCon = 200u;
		//public static uint UnarmedAttack = 60;
		//public static uint UnarmedDefense = 60;
		
		public static uint Rushdown1Con = 600u;
		//public static uint RushdownDefense = 60u;
		public static uint Rushdown2Con = 12000u;
		public static uint StatOverflowLimit = 50000u;
		#endregion

		#region Legacy
		public static int SlowQueueIntervalEnd = 3;
		public static int FastQueueIntervalStart = 8;
		//public static uint SuperThirstExtraDrain = 2u;
		public static int HelpItemRetryCount = 15;
		#endregion



		public static uint ShaftKhaosHp = 25;
		public static uint ShaftMayhemHp = 20;
		public static uint GalamothMayhemHp = 1500;
		public static uint GalamothMayhemPositionOffset = 100;
		public static uint GalamothKhaosHp = 2000;
		public static uint GalamothKhaosPositionOffset = 100;
		
		public static int SaveIcosahedronFirstCastle = 0xBCAA;
		public static int SaveIcosahedronSecondCastle = 0x1150;
	}
}
