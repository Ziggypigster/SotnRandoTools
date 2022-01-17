using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BizHawk.Client.Common;
using Newtonsoft.Json.Linq;
using SotnApi.Constants.Addresses;
using SotnApi.Constants.Values.Alucard;
using SotnApi.Constants.Values.Alucard.Enums;
using SotnApi.Constants.Values.Game;
using SotnApi.Interfaces;
using SotnApi.Models;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Constants;
using SotnRandoTools.Khaos.Enums;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;
using SotnRandoTools.Utils;
using WatsonWebsocket;
using MapLocation = SotnRandoTools.RandoTracker.Models.MapLocation;

namespace SotnRandoTools.Khaos
{
	public class KhaosController
	{
		private readonly IToolConfig toolConfig;
		private readonly ISotnApi sotnApi;
		private readonly ICheatCollectionAdapter cheats;
		private readonly IGameApi gameApi;
		private readonly IAlucardApi alucardApi;
		private readonly IActorApi actorApi;
		private readonly INotificationService notificationService;
		private readonly IInputService inputService;
		private WatsonWsClient socketClient;
		//private WatsonWsServer socketServer;
		private Random rng = new Random();

		private string[]? subscribers =
		{
		};

		private List<QueuedAction> queuedActions = new();
		private Queue<MethodInvoker> queuedFastActions = new();
		private Timer actionTimer = new();
		private Timer fastActionTimer = new();

		#region Timers
		private System.Timers.Timer subweaponsOnlyTimer = new();
		private System.Timers.Timer crippleTimer = new();
		private System.Timers.Timer bloodManaTimer = new();
		private System.Timers.Timer thirstTimer = new();
		private System.Timers.Timer thirstTickTimer = new();
		private System.Timers.Timer hordeTimer = new();
		private System.Timers.Timer hordeSpawnTimer = new();
		private System.Timers.Timer lordTimer = new();
		private System.Timers.Timer lordSpawnTimer = new();
		private System.Timers.Timer enduranceSpawnTimer = new();
		private System.Timers.Timer hnkTimer = new();
		private System.Timers.Timer vampireTimer = new();
		private System.Timers.Timer magicianTimer = new();
		private System.Timers.Timer meltyTimer = new();
		private System.Timers.Timer fourBeastsTimer = new();
		private System.Timers.Timer zawarudoTimer = new();
		private System.Timers.Timer zawarudoCheckTimer = new();
		private System.Timers.Timer hasteTimer = new();
		private System.Timers.Timer hasteOverdriveTimer = new();
		private System.Timers.Timer hasteOverdriveOffTimer = new();
		private System.Timers.Timer bloodManaDeathTimer = new();
		#endregion
		
		#region Cheats
		Cheat faerieScroll;
		Cheat darkMetamorphasisCheat;
		Cheat underwaterPhysics;
		Cheat hearts;
		Cheat curse;
		Cheat manaCheat;
		Cheat attackPotionCheat;
		Cheat defencePotionCheat;
		Cheat stopwatchTimer;
		Cheat hitboxWidth;
		Cheat hitboxHeight;
		Cheat hitbox2Width;
		Cheat hitbox2Height;
		Cheat invincibilityCheat;
		Cheat shineCheat;
		Cheat visualEffectPaletteCheat;
		Cheat visualEffectTimerCheat;
		Cheat savePalette;
		Cheat contactDamage;
		#endregion

		//private int totalMeterGained = 0;
		//private bool pandoraUsed = false;
		private uint alucardMapX = 0;
		private uint alucardMapY = 0;
		private bool alucardSecondCastle = false;
		private bool inMainMenu = false;

		private uint hordeZone = 0;
		private uint hordeZone2 = 0;
		private uint hordeTriggerRoomX = 0;
		private uint hordeTriggerRoomY = 0;
		private List<Actor> hordeEnemies = new();
		
		private uint lordZone = 0;
		private uint lordZone2 = 0;
		private uint lordTriggerRoomX = 0;
		private uint lordTriggerRoomY = 0;
		private List<Actor> lordEnemies = new();

		private int enduranceCount = 0;
		private uint enduranceRoomX = 0;
		private uint enduranceRoomY = 0;

		private bool zaWarudoActive = false;
		private uint zaWarudoZone = 0;
		private uint zaWarudoZone2 = 0;

		private uint storedMana = 0;
		private int spentMana = 0;

		private bool speedLocked = false;
		private bool heartsLocked = false;
		private bool manaLocked = false;
		private bool invincibilityLocked = false;
		private bool bloodManaActive = false;
		private bool hasteActive = false;
		private bool hasteSpeedOn = false;
		private bool overdriveOn = false;
		private bool subweaponsOnlyActive = false;
		private bool gasCloudTaken = false;
		private bool spawnActive = false;

		private int slowInterval;
		private int normalInterval;
		private int fastInterval;

		private bool shaftHpSet = false;
		private bool galamothStatsSet = false;

		private bool superThirst = false;
		private bool superHorde = false;
		private bool superEndurance = false;
		private bool superMelty = false;
		private bool superHaste = false;

		public KhaosController(IToolConfig toolConfig, ISotnApi sotnApi, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi, ICheatCollectionAdapter cheats, INotificationService notificationService, IInputService inputService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			if (alucardApi is null) throw new ArgumentNullException(nameof(alucardApi));
			if (actorApi is null) throw new ArgumentNullException(nameof(actorApi));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			if (notificationService == null) throw new ArgumentNullException(nameof(notificationService));
			if (inputService is null) throw new ArgumentNullException(nameof(inputService));
			this.toolConfig = toolConfig;
			this.sotnApi = sotnApi;
			this.gameApi = gameApi;
			this.alucardApi = alucardApi;
			this.actorApi = actorApi;
			this.cheats = cheats;
			this.notificationService = notificationService;
			this.inputService = inputService;

			InitializeTimers();
			GetCheats();
			notificationService.ActionQueue = queuedActions;
			normalInterval = (int) toolConfig.Khaos.QueueInterval.TotalMilliseconds;
			slowInterval = (int) normalInterval * 2;
			fastInterval = (int) normalInterval / 2;
			Console.WriteLine($"Intervals set. \n normal: {normalInterval / 1000}s, slow:{slowInterval / 1000}s, fast:{fastInterval / 1000}s");

			socketClient = new WatsonWsClient(new Uri(Globals.StreamlabsSocketAddress));
			socketClient.ServerConnected += BotConnected;
			socketClient.ServerDisconnected += BotDisconnected;
			socketClient.MessageReceived += BotMessageReceived;
		}

		public void StartKhaos()
		{
			if (File.Exists(toolConfig.Khaos.NamesFilePath))
			{
				subscribers = FileExtensions.GetLines(toolConfig.Khaos.NamesFilePath);
			}
			actionTimer.Start();
			fastActionTimer.Start();
			if (subscribers is not null && subscribers.Length > 0)
			{
				OverwriteBossNames(subscribers);
			}
			StartCheats();
			socketClient.Start();

			notificationService.AddMessage($"Khaos started");
			Console.WriteLine("Khaos started");
		}
		public void StopKhaos()
		{
			StopTimers();
			//Cheat faerieScroll = cheats.GetCheatByName("FaerieScroll");
			faerieScroll.Disable();
			if (socketClient.Connected)
			{
				socketClient.Stop();
			}
			notificationService.AddMessage($"Khaos stopped");
			Console.WriteLine("Khaos stopped");
		}
		public void OverwriteBossNames(string[] subscribers)
		{
			subscribers = subscribers.OrderBy(x => rng.Next()).ToArray();
			var randomizedBosses = Strings.BossNameAddresses.OrderBy(x => rng.Next());
			int i = 0;
			foreach (var boss in randomizedBosses)
			{
				if (i == subscribers.Length)
				{
					break;
				}
				sotnApi.GameApi.OverwriteString(boss.Value, subscribers[i]);
				Console.WriteLine($"{boss.Key} renamed to {subscribers[i]}");
				i++;
			}
		}

		#region Khaotic Effects
		public void KhaosStatus(string user = "Khaos")
		{
			//based on 1.16 balance changes.
			bool entranceCutscene = IsInRoomList(Constants.Khaos.EntranceCutsceneRooms);
			bool succubusRoom = IsInRoomList(Constants.Khaos.SuccubusRoom);
			int min = 1;
			int max = 9;

			if (zaWarudoActive)
			{
				max = 5;
			}

			int result = rng.Next(min, max);
			bool alucardIsImmuneToCurse = sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad)
				|| Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Coral circlet";
			bool alucardIsImmuneToStone = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Mirror cuirass"
				|| Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Medusa shield"
				|| Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Medusa shield";
			bool alucardIsImmuneToPoison = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Topaz circlet";

			if (succubusRoom && result == 3)
			{
				while (result == 3)
				{
					result = rng.Next(1, max);
				}
			}

			if ((succubusRoom || entranceCutscene) && result == 4)
			{
				while (result == 4)
				{
					result = rng.Next(1, max);
				}
			}

			if (alucardIsImmuneToCurse && result == 2)
			{
				while (result == 2)
				{
					result = rng.Next(1, max);
				}
			}

			if (alucardIsImmuneToStone && result == 3)
			{
				while (result == 3)
				{
					result = rng.Next(1, max);
				}
			}

			if (alucardIsImmuneToPoison && result == 1)
			{
				while (result == 1)
				{
					result = rng.Next(1, max);
				}
			}
			
			switch (result)
			{
				case 1:
					SpawnPoisonHitbox();
					notificationService.AddMessage($"{user} poisoned you");
					break;
				case 2:
					SpawnCurseHitbox();
					notificationService.AddMessage($"{user} cursed you");
					break;
				case 3:
					SpawnStoneHitbox();
					notificationService.AddMessage($"{user} petrified you");
					break;
				case 4:
					SpawnSlamHitbox();
					notificationService.AddMessage($"{user} slammed you");
					break;
				case 5:
					sotnApi.AlucardApi.ActivatePotion(Potion.LuckPotion);
					notificationService.AddMessage($"{user} gave you luck");
					break;
				case 6:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistFire);
					notificationService.AddMessage($"{user} gave you resistance to fire");
					break;
				case 7:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistDark);
					notificationService.AddMessage($"{user} gave you resistance to dark");
					break;
				case 8:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave you defense");
					break;
				default:
					break;
			}

			Alert(KhaosActionNames.KhaosStatus);
		}
		
		public void Slam(string user = "Khaos")
		{
			SpawnSlamHitbox();
			notificationService.AddMessage($"{user} slammed you");
			Alert(KhaosActionNames.Slam);
		}
		
		public void KhaosEquipment(string user = "Khaos")
		{
			RandomizeGold();
			RandomizeEquipmentSlots();
			notificationService.AddMessage($"{user} used Khaos Equipment");
			Alert(KhaosActionNames.KhaosEquipment);
		}
		public void KhaosStats(string user = "Khaos")
		{
			RandomizeStatsActivate();
			notificationService.AddMessage($"{user} used Khaos Stats");
			Alert(KhaosActionNames.KhaosStats);
		}
		public void KhaosRelics(string user = "Khaos")
		{
			RandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			notificationService.AddMessage($"{user} used Khaos Relics");
			Alert(KhaosActionNames.KhaosRelics);
		}
		public void PandorasBox(string user = "Khaos")
		{
			RandomizeGold();
			RandomizeStatsActivate();
			RandomizeEquipmentSlots();
			RandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			RandomizeInventory();
			RandomizeSubweapon();
			sotnApi.GameApi.RespawnBosses();
			sotnApi.GameApi.RespawnItems();
			notificationService.AddMessage($"{user} opened Pandora's Box");
			Alert(KhaosActionNames.PandorasBox);
		}
		public void Gamble(string user = "Khaos")
		{
			double goldPercent = (rng.NextDouble()/ 2);
			uint newGold = (uint) ((double) alucardApi.Gold * goldPercent);
			uint goldSpent = alucardApi.Gold - newGold;
			alucardApi.Gold = newGold;
			string item = Equipment.Items[rng.Next(1, Equipment.Items.Count)];
			while (item.Contains("empty hand") || item.Contains("-"))
			{
				item = Equipment.Items[rng.Next(1, Equipment.Items.Count)];
			}
			alucardApi.GrantItemByName(item);


			notificationService.AddMessage($"{user} gambled {goldSpent} gold for {item}");
			Alert(KhaosActionNames.Gamble);
		}
		public void KhaoticBurst(string user = "Khaos")
		{
			notificationService.KhaosMeter += 100;
			notificationService.AddMessage($"{user} used Khaotic Burst");
			Alert(KhaosActionNames.KhaosBurst);
		}
		#endregion
		#region Debuffs
		public void Thirst(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superThirst = true;
				SpendKhaosMeter();
			}

			//Cheat darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Enable();
			thirstTimer.Start();
			thirstTickTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Thirst,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Thirst).FirstOrDefault().Duration
			});

			string message = meterFull ? $"{user} used Super Thirst" : $"{user} used Thirst";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Thirst);
		}
		public void Weaken(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			float enhancedFactor = 1;
			if (meterFull)
			{
				enhancedFactor = Constants.Khaos.SuperWeakenFactor;
				SpendKhaosMeter();
			}

			alucardApi.CurrentHp = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.CurrentMp = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			
			alucardApi.MaxtHp = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.MaxtMp = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Str = (uint) (alucardApi.Str * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Con = (uint) (alucardApi.Con * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Int = (uint) (alucardApi.Int * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Lck = (uint) (alucardApi.Lck * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			
			//Zig- Enforce minimum stats
			alucardApi.MaxtHp = alucardApi.MaxtHp < 80 ? 80 : alucardApi.MaxtHp;
			alucardApi.MaxtMp = alucardApi.MaxtMp < 30 ? 30 : alucardApi.MaxtMp;
			
			alucardApi.Str = alucardApi.Str < 7 ? 7 : alucardApi.Str;
			alucardApi.Con = alucardApi.Con < 7 ? 7 : alucardApi.Con;
			alucardApi.Int = alucardApi.Int < 7 ? 7 : alucardApi.Int;
			alucardApi.Lck = alucardApi.Lck < 7 ? 7 : alucardApi.Lck;

			if (heartsLocked == true) {
				Console.WriteLine("Skipping Weaken Hearts re-roll due to hearts lock.");
			}
			else
			{
				alucardApi.MaxtHearts = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
				//alucardApi.CurrentHearts = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.WeakenFactor * enhancedFactor);
				alucardApi.MaxtHearts = alucardApi.MaxtHearts < 30 ? 30 : alucardApi.MaxtHearts;
				alucardApi.CurrentHearts = alucardApi.MaxtHearts;
			}
			


			uint newLevel = (uint) (alucardApi.Level * toolConfig.Khaos.WeakenFactor * enhancedFactor);
			alucardApi.Level = newLevel;
			uint newExperience = 0;
			if (newLevel <= StatsValues.ExperienceValues.Length && newLevel > 1)
			{
				newExperience = (uint) StatsValues.ExperienceValues[(int) newLevel - 1];
			}
			else if (newLevel > 1)
			{
				newExperience = (uint) StatsValues.ExperienceValues[StatsValues.ExperienceValues.Length - 1];
			}
			if (newLevel > 1)
			{
				alucardApi.Level = newLevel;
				alucardApi.Experiecne = newExperience;
			}

			string message = meterFull ? $"{user} used Super Weaken" : $"{user} used Weaken";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Weaken);
		}
		public void Cripple(string user = "Khaos")
		{

			if (IsInRoomList(Constants.Khaos.EntranceCutsceneRooms))
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Cripple, LocksSpeed = true, Invoker = new MethodInvoker(() => Cripple(user)) });
			}

			bool meterFull = KhaosMeterFull();
			float enhancedFactor = 1;
			if (meterFull)
			{
				enhancedFactor = Constants.Khaos.SuperCrippleFactor;
				SpendKhaosMeter();
			}

			speedLocked = true;
			SetSpeed(toolConfig.Khaos.CrippleFactor * enhancedFactor);
			Cheat underwaterPhysics = cheats.GetCheatByName("UnderwaterPhysics");
			underwaterPhysics.Enable();
			crippleTimer.Start();
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Cripple,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Cripple).FirstOrDefault().Duration
			});

			string message = meterFull ? $"{user} used Super Cripple" : $"{user} used Cripple";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Cripple);
		}
		public void BloodMana(string user = "Khaos")
		{
			storedMana = alucardApi.CurrentMp;
			bloodManaActive = true;
			bloodManaTimer.Start();
			manaLocked = true;
			notificationService.AddMessage($"{user} used Blood Mana");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.BloodMana,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BloodMana).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.BloodMana);
		}
		public void SubweaponsOnly(string user = "Khaos")
		{
			int roll = rng.Next(1, 10);
			while (roll == 6)
			{
				roll = rng.Next(1, 10);
			}
			alucardApi.Subweapon = (Subweapon) roll;
			alucardApi.ActivatePotion(Potion.SmartPotion);
			alucardApi.GrantRelic(Relic.CubeOfZoe);
			if (alucardApi.HasRelic(Relic.GasCloud))
			{
				alucardApi.TakeRelic(Relic.GasCloud);
				gasCloudTaken = true;
			}
			Cheat hearts = cheats.GetCheatByName("Hearts");
			hearts.Enable();
			heartsLocked = true;
			Cheat curse = cheats.GetCheatByName("CurseTimer");
			curse.Enable();
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(7);
			manaCheat.Enable();
			manaLocked = true;
			subweaponsOnlyActive = true;
			subweaponsOnlyTimer.Start();
			notificationService.AddMessage($"{user} used Subweapons Only");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.SubweaponsOnly,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.SubweaponsOnly);
		}
		public void Bankrupt(string user = "Khaos")
		{
			BankruptActivate();
			notificationService.AddMessage($"{user} used Bankrupt");
			Alert(KhaosActionNames.Bankrupt);
		}
		public void RespawnBosses(string user = "Khaos")
		{
			gameApi.RespawnBosses();
			notificationService.AddMessage($"{user} used Respawn Bosses");
			Alert(KhaosActionNames.RespawnBosses);
		}
		public void Horde(string user = "Khaos")
		{
			hordeTriggerRoomX = gameApi.MapXPos;
			hordeTriggerRoomY = gameApi.MapYPos;
			spawnActive = true;
			bool meterFull = KhaosMeterFull();

			if (meterFull)
			{
				superHorde = true;
				SpendKhaosMeter();
			}

			hordeTimer.Start();
			hordeSpawnTimer.Start();
			string message = meterFull ? $"{user} summoned the Super Horde" : $"{user} summoned the Horde";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.KhaosHorde);
		}
		public void Endurance(string user = "Khaos")
		{
			enduranceRoomX = gameApi.MapXPos;
			enduranceRoomY = gameApi.MapYPos;
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superEndurance = true;
				SpendKhaosMeter();
			}

			enduranceCount++;
			enduranceSpawnTimer.Start();
			string message = meterFull ? $"{user} used Super Endurance" : $"{user} used Endurance";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Endurance);
		}
		public void HnK(string user = "Khaos")
		{
			invincibilityCheat.PokeValue(0);
			invincibilityCheat.Enable();
			defencePotionCheat.PokeValue(1);
			defencePotionCheat.Enable();
			invincibilityLocked = true;
			hnkTimer.Start();
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.HnK,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HnK).FirstOrDefault().Duration
			});
			//statusInfoDisplay.AddTimer(timer);
			notificationService.AddMessage($"{user} used HnK");
			Alert(KhaosActionNames.HnK);
		}
		#endregion
		#region Buffs
		public void LightHelp(string user = "Khaos")
		{
			//Light help updated to restrict based on 80% of Hp
			string item = toolConfig.Khaos.LightHelpItemRewards[rng.Next(0, toolConfig.Khaos.LightHelpItemRewards.Length)];
			int rolls = 0;
			while (alucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
			{
				item = toolConfig.Khaos.LightHelpItemRewards[rng.Next(0, toolConfig.Khaos.LightHelpItemRewards.Length)];
				rolls++;
			}

			bool highHp = alucardApi.CurrentHp > alucardApi.MaxtHp * 0.8;
			
			int roll = rng.Next(1, 4);
			if (highHp && roll == 2)
			{
				roll = 3;
			}

			if (zaWarudoActive)
			{
				roll = 1;
			}

			switch (roll)
			{
				case 1:
					alucardApi.GrantItemByName(item);
					Console.WriteLine($"Light help rolled: {item}");
					notificationService.AddMessage($"{user} gave you a {item}");
					break;
				case 2:
					alucardApi.ActivatePotion(Potion.Potion);
					Console.WriteLine($"Light help rolled activate potion.");
					notificationService.AddMessage($"{user} healed you");
					break;
				case 3:
					alucardApi.ActivatePotion(Potion.ShieldPotion);
					Console.WriteLine($"Light help rolled activate shield potion.");
					notificationService.AddMessage($"{user} used a Shield Potion");
					break;
				default:
					break;
			}
			Alert(KhaosActionNames.LightHelp);
		}
		public void MediumHelp(string user = "Khaos")
		{
			//Re-adding 60% HP/MP Restrictions, but also ManaLocked check.
			string item = toolConfig.Khaos.MediumHelpItemRewards[rng.Next(0, toolConfig.Khaos.MediumHelpItemRewards.Length)];
			int rolls = 0;
			while (alucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
			{
				item = toolConfig.Khaos.MediumHelpItemRewards[rng.Next(0, toolConfig.Khaos.MediumHelpItemRewards.Length)];
				rolls++;
			}

			bool highHp = alucardApi.CurrentHp > alucardApi.MaxtHp * 0.6;
			bool highMp = alucardApi.CurrentMp > alucardApi.MaxtMp * 0.6;

			int roll = rng.Next(1, manaLocked ? 3 : 4);

			if (highHp && roll == 2)
			{
				roll = 3;
			}
			if (highMp && roll == 3)
			{
				roll = 2;
			}
			if (manaLocked && roll == 3)
			{
				roll = 2;
			}
			if ((highHp && (highMp || manaLocked)) || zaWarudoActive)
			{
				roll = 1;
			}

			switch (roll)
			{
				case 1:
					alucardApi.GrantItemByName(item);
					Console.WriteLine($"Medium help rolled: {item}");
					notificationService.AddMessage($"{user} gave you a {item}");
					break;
				case 2:
					alucardApi.ActivatePotion(Potion.Elixir);
					Console.WriteLine($"Medium help rolled activate Elixir.");
					notificationService.AddMessage($"{user} healed you");
					break;
				case 3:
					alucardApi.ActivatePotion(Potion.Mannaprism);
					Console.WriteLine($"Medium help rolled activate Manna prism.");
					notificationService.AddMessage($"{user} used a Mana Prism");
					break;
				default:
					break;
			}
			Alert(KhaosActionNames.MediumHelp);
		}
		public void HeavytHelp(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				SpendKhaosMeter();
			}

			string item;
			int relic;
			int roll;
			RollRewards(out item, out relic, out roll);
			GiveRewards(user, item, relic, roll);
			Alert(KhaosActionNames.HeavyHelp);

			if (meterFull)
			{
				RollRewards(out item, out relic, out roll);
				GiveRewards(user, item, relic, roll);
			}

			void RollRewards(out string item, out int relic, out int roll)
			{
				item = toolConfig.Khaos.HeavyHelpItemRewards[rng.Next(0, toolConfig.Khaos.HeavyHelpItemRewards.Length)];
				int rolls = 0;
				while (alucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
				{
					item = toolConfig.Khaos.HeavyHelpItemRewards[rng.Next(0, toolConfig.Khaos.HeavyHelpItemRewards.Length)];
					rolls++;
				}

				relic = rng.Next(0, Constants.Khaos.ProgressionRelics.Length);

				roll = rng.Next(1, 3);
				for (int i = 0; i < 11; i++)
				{
					if (!alucardApi.HasRelic(Constants.Khaos.ProgressionRelics[relic]))
					{
						break;
					}
					if (i == 10)
					{
						roll = 1;
						break;
					}
					relic = rng.Next(0, Constants.Khaos.ProgressionRelics.Length);
				}
			}

			void GiveRewards(string user, string item, int relic, int roll)
			{
				switch (roll)
				{
					case 1:
						Console.WriteLine($"Heavy help rolled: {item}");
						alucardApi.GrantItemByName(item);
						notificationService.AddMessage($"{user} gave you a {item}");
						break;
					case 2:
						Console.WriteLine($"Heavy help rolled: {Constants.Khaos.ProgressionRelics[relic]}");
						alucardApi.GrantRelic(Constants.Khaos.ProgressionRelics[relic]);
						notificationService.AddMessage($"{user} gave you {Constants.Khaos.ProgressionRelics[relic]}");
						break;
					default:
						break;
				}
			}
		}
		public void Vampire(string user = "Khaos")
		{
			//Cheat darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Enable();
			Cheat attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Enable();
			vampireTimer.Start();
			notificationService.AddMessage($"{user} used Vampire");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Vampire,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Vampire).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.Vampire);
		}
		public void BattleOrders(string user = "Khaos")
		{
			alucardApi.CurrentHp = (uint) (alucardApi.MaxtHp * Constants.Khaos.BattleOrdersHpMultiplier);
			alucardApi.CurrentMp = alucardApi.MaxtMp;
			alucardApi.ActivatePotion(Potion.ShieldPotion);
			notificationService.AddMessage($"{user} used Battle Orders");
			Alert(KhaosActionNames.BattleOrders);
		}
		public void Magician(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				SpendKhaosMeter();
				alucardApi.GrantRelic(Relic.SoulOfBat);
				alucardApi.GrantRelic(Relic.EchoOfBat);
				alucardApi.GrantRelic(Relic.ForceOfEcho);
				alucardApi.GrantRelic(Relic.SoulOfWolf);
				alucardApi.GrantRelic(Relic.PowerOfWolf);
				alucardApi.GrantRelic(Relic.SkillOfWolf);
				alucardApi.GrantRelic(Relic.FormOfMist);
				alucardApi.GrantRelic(Relic.PowerOfMist);
				alucardApi.GrantRelic(Relic.GasCloud);
			}

			alucardApi.GrantItemByName("Wizard hat");
			alucardApi.ActivatePotion(Potion.SmartPotion);
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(99);
			manaCheat.Enable();
			manaLocked = true;
			magicianTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Magician,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Magician).FirstOrDefault().Duration
			});

			string message = meterFull ? $"{user} activated Shapeshifter" : $"{user} activated Magician";
			notificationService.AddMessage(message);

			Alert(KhaosActionNames.Magician);
		}
		public void ZaWarudo(string user = "Khaos")
		{
			alucardApi.ActivateStopwatch();
			zaWarudoZone = gameApi.Zone;
			zaWarudoZone2 = gameApi.Zone2;
			zaWarudoActive = true;

			if (!subweaponsOnlyActive)
			{
				alucardApi.Subweapon = Subweapon.Stopwatch;
			}

			Cheat stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			stopwatchTimer.Enable();
			zawarudoTimer.Start();
			zawarudoCheckTimer.Start();

			notificationService.AddMessage($"{user} used ZA WARUDO");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.ZaWarudo,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ZaWarudo).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.ZaWarudo);
		}
		public void MeltyBlood(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superMelty = true;
				SetHasteStaticSpeeds(true);
				ToggleHasteDynamicSpeeds(2);
				alucardApi.CurrentHp = alucardApi.MaxtHp;
				alucardApi.CurrentMp = alucardApi.MaxtMp;
				alucardApi.ActivatePotion(Potion.StrPotion);
				alucardApi.AttackPotionTimer = Constants.Khaos.GuiltyGearAttack;
				alucardApi.DarkMetamorphasisTimer = Constants.Khaos.GuiltyGearDarkMetamorphosis;
				alucardApi.DefencePotionTimer = Constants.Khaos.GuiltyGearDefence;
				alucardApi.InvincibilityTimer = Constants.Khaos.GuiltyGearInvincibility;
				SpendKhaosMeter();
			}

			Cheat width = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			Cheat height = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			Cheat width2 = cheats.GetCheatByName("AlucardAttackHitbox2Width");
			Cheat height2 = cheats.GetCheatByName("AlucardAttackHitbox2Height");
			width.Enable();
			height.Enable();
			width2.Enable();
			height2.Enable();
			alucardApi.GrantRelic(Relic.LeapStone);
			meltyTimer.Start();
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.MeltyBlood,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MeltyBlood).FirstOrDefault().Duration
			});

			string message = meterFull ? $"{user} activated GUILTY GEAR" : $"{user} activated Melty Blood";
			notificationService.AddMessage(message);
			if (meterFull)
			{
				Alert(KhaosActionNames.GuiltyGear);
			}
			else
			{
				Alert(KhaosActionNames.MeltyBlood);
			}
		}
		public void FourBeasts(string user = "Khaos")
		{
			Cheat invincibilityCheat = cheats.GetCheatByName("Invincibility");
			invincibilityCheat.PokeValue(1);
			invincibilityCheat.Enable();
			invincibilityLocked = true;
			Cheat attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Enable();
			Cheat shineCheat = cheats.GetCheatByName("Shine");
			shineCheat.PokeValue(1);
			shineCheat.Enable();
			fourBeastsTimer.Start();

			notificationService.AddMessage($"{user} used Four Beasts");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.FourBeasts,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FourBeasts).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.FourBeasts);
		}
		public void Haste(string user = "Khaos")
		{
			bool meterFull = KhaosMeterFull();

			if (meterFull)
			{
				SpendKhaosMeter();
				superHaste = true;
			}

			SetHasteStaticSpeeds(meterFull);
			hasteTimer.Start();
			hasteActive = true;
			speedLocked = true;
			Console.WriteLine($"{user} used {KhaosActionNames.Haste}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.Haste,
				Type = Enums.ActionType.Buff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault().Duration
			});
			string message = meterFull ? $"{user} activated Super Haste" : $"{user} activated Haste";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Haste);
		}

		public void Lord(string user = "Khaos")
		{
			lordTriggerRoomX = gameApi.MapXPos;
			lordTriggerRoomY = gameApi.MapYPos;
			spawnActive = true;

			lordTimer.Start();
			lordSpawnTimer.Start();

			//string message = meterFull ? $"{user} made you a Super Lord" : $"{user} made you a Lord";
			string message = $"{user} made you a Lord";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Lord);
		}
		#endregion

		public void Update()
		{
			if (gameApi.InAlucardMode() && bloodManaActive)
			{
				CheckManaUsage();
			}
			if (gameApi.InAlucardMode())
			{
				CheckDashInput();
			}
			if (subweaponsOnlyActive)
			{
				if (alucardApi.RightHand == 0)
				{
					alucardApi.RightHand = (uint) Equipment.Items.IndexOf("Orange");
					if (alucardApi.HasItemInInventory("Orange"))
					{
						alucardApi.TakeOneItemByName("Orange");
					}
				}
				if (alucardApi.LeftHand == 0)
				{
					alucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Orange");
					if (alucardApi.HasItemInInventory("Orange"))
					{
						alucardApi.TakeOneItemByName("Orange");
					}
				}
			}
		}
		public void EnqueueAction(EventAddAction eventData)
		{
			if (eventData.Command is null) throw new ArgumentNullException(nameof(eventData.Command));
			if (eventData.Command == "") throw new ArgumentException($"Parameter {nameof(eventData.Command)} is empty!");
			if (eventData.UserName is null) throw new ArgumentNullException(nameof(eventData.UserName));
			if (eventData.UserName == "") throw new ArgumentException($"Parameter {nameof(eventData.UserName)} is empty!");
			string user = eventData.UserName;
			string action = eventData.Command;

			SotnRandoTools.Configuration.Models.Action? commandAction;
			switch (action)
			{
				#region Khaotic commands
				case "kstatus":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosStatus).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => KhaosStatus(user)));
					}
					break;
				case "kequipment":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosEquipment).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Khaos Equipment", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => KhaosEquipment(user)) });
					}
					break;
				case "kstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosStats).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Khaos Stats", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => KhaosStats(user)) });
					}
					break;
				case "krelics":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosRelics).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Khaos Relics", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => KhaosRelics(user)) });
					}
					break;
				case "pandora":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.PandorasBox).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Pandora's Box", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => PandorasBox(user)) });
					}
					break;
				case "gamble":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Gamble).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Gamble(user)));
					}
					break;
				case "kburst":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosBurst).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => KhaoticBurst(user)));
					}
					break;
				#endregion
				#region Debuffs
				case "slam":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Slam).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user)));
					}
				break;
				case "bankrupt":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Bankrupt).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Bankrupt", Invoker = new MethodInvoker(() => Bankrupt(user)) });
					}
					break;
				case "weaken":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Weaken).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Weaken", Invoker = new MethodInvoker(() => Weaken(user)) });
					}
					break;
				case "respawnbosses":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RespawnBosses).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => RespawnBosses(user)));
					}
					break;
				case "subsonly":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Subweapons Only", LocksMana = true, Invoker = new MethodInvoker(() => SubweaponsOnly(user)) });
					}
					break;
				case "cripple":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Cripple).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Cripple", LocksSpeed = true, Invoker = new MethodInvoker(() => Cripple(user)) });
					}
					break;
				case "bloodmana":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BloodMana).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Blood Mana", LocksMana = true, Invoker = new MethodInvoker(() => BloodMana(user)) });
					}
					break;
				case "thirst":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Thirst).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Thirst", Invoker = new MethodInvoker(() => Thirst(user)) });
					}
					break;
				case "horde":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Horde", Invoker = new MethodInvoker(() => Horde(user)) });
					}
					break;
				case "endurance":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Endurance).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Endurance(user)));
					}
					break;
				case "hnk":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HnK).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => HnK(user)));
					}
					break;
				#endregion
				#region Buffs
				case "vampire":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Vampire).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Vampire(user)));
					}
					break;
				case "lighthelp":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.LightHelp).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => LightHelp(user)));
					}
					break;
				case "mediumhelp":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MediumHelp).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MediumHelp(user)));
					}
					break;
				case "heavyhelp":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeavyHelp).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => HeavytHelp(user)));
					}
					break;
				case "battleorders":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BattleOrders).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Battle Orders", Type = ActionType.Buff, Invoker = new MethodInvoker(() => BattleOrders(user)) });
					}
					break;
				case "magician":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Magician).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Magician", Type = ActionType.Buff, LocksMana = true, Invoker = new MethodInvoker(() => Magician(user)) });
					}
					break;
				case "melty":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MeltyBlood).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "MeltyBlood", Type = ActionType.Buff, Invoker = new MethodInvoker(() => MeltyBlood(user)) });
					}
					break;
				case "fourbeasts":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FourBeasts).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Four Beasts", Type = ActionType.Buff, LocksInvincibility = true, Invoker = new MethodInvoker(() => FourBeasts(user)) });
					}
					break;
				case "zawarudo":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ZaWarudo).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ZaWarudo(user)));
					}
					break;
				case "haste":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Haste", Type = ActionType.Buff, LocksSpeed = true, Invoker = new MethodInvoker(() => Haste(user)) });
					}
					break;
				case "lord":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Lord).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Lord", LocksSpawning = true, Type = ActionType.Buff, Invoker = new MethodInvoker(() => Lord(user)) });
					}
					break;
				default:
					commandAction = null;
					break;
					#endregion
			}
			if (commandAction is not null)
			{
				GainKhaosMeter(commandAction.Meter);
			}
		}
		private void InitializeTimers()
		{
			fastActionTimer.Tick += ExecuteFastAction;
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Tick += ExecuteAction;
			actionTimer.Interval = 2 * (1 * 1000);
			
			bloodManaDeathTimer.Elapsed += KillAlucard;
			bloodManaDeathTimer.Interval = 1 * (1 * 1500);			

			subweaponsOnlyTimer.Elapsed += SubweaponsOnlyOff;
			subweaponsOnlyTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault().Duration.TotalMilliseconds;
			crippleTimer.Elapsed += CrippleOff;
			crippleTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Cripple).FirstOrDefault().Duration.TotalMilliseconds;
			bloodManaTimer.Elapsed += BloodManaOff;
			bloodManaTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BloodMana).FirstOrDefault().Duration.TotalMilliseconds;
			thirstTimer.Elapsed += ThirstOff;
			thirstTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Thirst).FirstOrDefault().Duration.TotalMilliseconds;
			thirstTickTimer.Elapsed += ThirstDrain;
			thirstTickTimer.Interval = 1000;
			hordeTimer.Elapsed += HordeOff;
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeSpawnTimer.Elapsed += HordeSpawn;
			hordeSpawnTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault().Interval.TotalMilliseconds;
			enduranceSpawnTimer.Elapsed += EnduranceSpawn;
			enduranceSpawnTimer.Interval = 2 * (1000);
			hnkTimer.Elapsed += HnKOff;
			hnkTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HnK).FirstOrDefault().Duration.TotalMilliseconds;

			vampireTimer.Elapsed += VampireOff;
			vampireTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Vampire).FirstOrDefault().Duration.TotalMilliseconds;
			magicianTimer.Elapsed += MagicianOff;
			magicianTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Magician).FirstOrDefault().Duration.TotalMilliseconds;
			meltyTimer.Elapsed += MeltyBloodOff;
			meltyTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MeltyBlood).FirstOrDefault().Duration.TotalMilliseconds;
			fourBeastsTimer.Elapsed += FourBeastsOff;
			fourBeastsTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FourBeasts).FirstOrDefault().Duration.TotalMilliseconds;
			zawarudoTimer.Elapsed += ZawarudoOff;
			zawarudoTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ZaWarudo).FirstOrDefault().Duration.TotalMilliseconds;
			zawarudoCheckTimer.Elapsed += ZaWarudoAreaCheck;
			zawarudoCheckTimer.Interval += 2 * 1000;
			hasteTimer.Elapsed += HasteOff;
			hasteTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault().Duration.TotalMilliseconds;
			hasteOverdriveTimer.Elapsed += OverdriveOn;
			hasteOverdriveTimer.Interval = (2 * 1000);
			hasteOverdriveOffTimer.Elapsed += OverdriveOff;
			hasteOverdriveOffTimer.Interval = (2 * 1000);
			hordeTimer.Elapsed += LordOff;
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeSpawnTimer.Elapsed += LordSpawn;
			hordeSpawnTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault().Interval.TotalMilliseconds;
		}
		private void StopTimers()
		{
			fastActionTimer.Stop();
			actionTimer.Stop();

			subweaponsOnlyTimer.Interval = 1;
			crippleTimer.Interval = 1;
			bloodManaTimer.Interval = 1;
			thirstTimer.Interval = 1;
			thirstTickTimer.Stop();
			hordeTimer.Interval = 1;
			hordeSpawnTimer.Stop();
			enduranceSpawnTimer.Stop();

			vampireTimer.Interval = 1;
			magicianTimer.Interval = 1;
			meltyTimer.Interval = 1;
			fourBeastsTimer.Interval = 1;
			zawarudoTimer.Interval = 1;
			zawarudoCheckTimer.Stop();
			hasteTimer.Interval = 1;
			hasteOverdriveTimer.Stop();
			hasteOverdriveOffTimer.Stop();
		}
		private void ExecuteAction(Object sender, EventArgs e)
		{
			if (queuedActions.Count > 0)
			{
				alucardMapX = alucardApi.MapX;
				alucardMapY = alucardApi.MapY;

				if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && !IsInRoomList(Constants.Khaos.RichterRooms) && !IsInRoomList(Constants.Khaos.LoadingRooms))
				{
					int index = 0;
					bool actionUnlocked = true;

					for (int i = 0; i < queuedActions.Count; i++)
					{
						index = i;
						actionUnlocked = true;
						if (queuedActions[i].LocksSpeed && speedLocked)
						{
							actionUnlocked = false;
							continue;
						}
						if (queuedActions[i].LocksMana && manaLocked)
						{
							actionUnlocked = false;
							continue;
						}
						if (queuedActions[i].LocksInvincibility && invincibilityLocked)
						{
							actionUnlocked = false;
							continue;
						}
						if (queuedActions[i].LocksSpawning && spawnActive)
						{
							actionUnlocked = false;
							continue;
						}
						break;
					}

					if (actionUnlocked)
					{
						queuedActions[index].Invoker();
						queuedActions.RemoveAt(index);
						SetDynamicInterval();
					}
					else
					{
						Console.WriteLine($"All actions locked. speed: {speedLocked}, invincibility: {invincibilityLocked}, mana: {manaLocked}");
					}
				}
			}
			else
			{
				actionTimer.Interval = 2000;
			}
		}
		private void SetDynamicInterval()
		{
			if (toolConfig.Khaos.DynamicInterval && queuedActions.Count < Constants.Khaos.SlowQueueIntervalEnd)
			{
				actionTimer.Interval = slowInterval;
				Console.WriteLine($"Interval set to {slowInterval / 1000}s, {actionTimer.Interval}");
			}
			else if (toolConfig.Khaos.DynamicInterval && queuedActions.Count >= Constants.Khaos.FastQueueIntervalStart)
			{
				actionTimer.Interval = fastInterval;
				Console.WriteLine($"Interval set to {fastInterval / 1000}s, {actionTimer.Interval}");
			}
			else
			{
				actionTimer.Interval = normalInterval;
				Console.WriteLine($"Interval set to {normalInterval / 1000}s, {actionTimer.Interval}");
			}
		}
		private void ExecuteFastAction(Object sender, EventArgs e)
		{
			alucardMapX = alucardApi.MapX;
			alucardMapY = alucardApi.MapY;
			CheckCastleChanged();
			CheckMainMenu();
			
			bool keepRichterRoom = IsInKeepRichterRoom();
			bool galamothRoom = IsInGalamothRoom();
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && !keepRichterRoom && !gameApi.InTransition && !gameApi.IsLoading && !alucardApi.IsInvincible() && !IsInRoomList(Constants.Khaos.LoadingRooms) && alucardMapX < 99)
			{
				shaftHpSet = false;
				if (queuedFastActions.Count > 0)
				{
					queuedFastActions.Dequeue()();
				}
			}
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && keepRichterRoom && !shaftHpSet && !gameApi.InTransition && !gameApi.IsLoading)
			{
				SetShaftHp();
			}
			if (gameApi.InAlucardMode() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && galamothRoom && !galamothStatsSet && !gameApi.InTransition && !gameApi.IsLoading)
			{
				SetGalamothtStats();
			}
			if (!galamothRoom)
			{
				galamothStatsSet = false;
			}
		}

		#region Khaotic events
		private void RandomizeGold()
		{
			uint gold = (uint) rng.Next(500, 5000);
			uint roll = (uint) rng.Next(0, 21);
			if (roll > 16 && roll < 20)
			{
				gold = gold * (uint) rng.Next(1, 11);
			}
			else if (roll > 19)
			{
				gold = gold * (uint) rng.Next(10, 81);
			}
			alucardApi.Gold = gold;
		}
		private void RandomizeStatsActivate()
		{
			uint maxHp = alucardApi.MaxtHp;
			uint currentHp = alucardApi.CurrentHp;
			uint maxMana = alucardApi.MaxtMp;
			uint currentMana = alucardApi.CurrentMp;
			uint maxHearts = alucardApi.MaxtHearts;
			uint currentHearts = alucardApi.CurrentHearts;
			uint str = alucardApi.Str;
			uint con = alucardApi.Con;
			uint intel = alucardApi.Int;
			uint lck = alucardApi.Lck;

			//Zig - Increase stat pool check to compensate for higher minimum stats
			uint statPool = str + con + intel + lck > 28 ? str + con + intel + lck - 28 : 28;
			uint offset = (uint) ((rng.NextDouble() / 2) * statPool);

			int statPoolRoll = rng.Next(1, 4);
			if (statPoolRoll == 2)
			{
				statPool = statPool + offset;
			}
			else if (statPoolRoll == 3)
			{
				statPool = statPool - offset;
			}

			double a = rng.NextDouble();
			double b = rng.NextDouble();
			double c = rng.NextDouble(); 
			double d = rng.NextDouble();
			double sum = a + b + c + d;
			double percentageStr = (a / sum);
			double percentageCon = (b / sum);
			double percentageInt = (c / sum);
			double percentageLck = (d / sum);
			uint newStr = (uint) Math.Round(statPool * percentageStr);
			uint newCon = (uint) Math.Round(statPool * percentageCon);
			uint newInt = (uint) Math.Round(statPool * percentageInt);
			uint newLck = (uint) Math.Round(statPool * percentageLck);

			//Zig - keep minimum stats higher to match weaken minimums
			alucardApi.Str = (7 + newStr);
			alucardApi.Con = (7 + newCon);
			alucardApi.Int = (7 + newInt);
			alucardApi.Lck = (7 + newLck);
			
			//Zig - Rounding Fix
			uint CalculatedStatPool = 28 + statPool;
			uint ActualStatPool = 28 + newStr + newCon + newInt + newLck;
			if (ActualStatPool < CalculatedStatPool){
				uint roundingOffset = CalculatedStatPool - ActualStatPool;
				int roundPoolRoll = rng.Next(1, 5);
				if (roundPoolRoll == 1)
				{
					alucardApi.Str = (alucardApi.Str + roundingOffset);
				}
				else if (roundPoolRoll == 2)
				{
					alucardApi.Con = (alucardApi.Con + roundingOffset);
				}
				else if (roundPoolRoll == 3) 
				{
					alucardApi.Int = (alucardApi.Int + roundingOffset);
				}
				else {
					alucardApi.Lck = (alucardApi.Lck + roundingOffset);
				}
			}
			
			uint pointsPool = maxHp + maxMana > 110 ? maxHp + maxMana - 110 : maxHp + maxMana;
			if (maxHp + maxMana < 110)
			{
				pointsPool = 110;
			}
			offset = (uint) ((rng.NextDouble() / 2) * pointsPool);

			int pointsRoll = rng.Next(1, 4);
			if (pointsRoll == 2)
			{
				pointsPool = pointsPool + offset;
			}
			else if (pointsRoll == 3)
			{
				pointsPool = pointsPool - offset;
			}

			double hpPercent = rng.NextDouble();
			uint pointsHp = 80 + (uint) Math.Round(hpPercent * pointsPool);
			uint pointsMp = 30 + pointsPool - (uint) Math.Round(hpPercent * pointsPool);

			if (currentHp > pointsHp)
			{
				alucardApi.CurrentHp = pointsHp;
			}
			if (currentMana > pointsMp)
			{
				alucardApi.CurrentMp = pointsMp;
			}
			if (heartsLocked) {
				Console.WriteLine("Skipping Kstats Hearts re-roll due to hearts lock.");
			}
			else {
				pointsPool = maxHearts;
				offset = (uint) ((rng.NextDouble() / 4) * pointsPool);
				pointsRoll = rng.Next(1, 3);
				if (pointsRoll == 1)
				{
					pointsPool = pointsPool + offset;
				}
				else if (pointsRoll == 2)
				{
					pointsPool = pointsPool - offset;
				}
				if (pointsPool < 30)
				{
					pointsPool = 30;
				}
				alucardApi.CurrentHearts = pointsPool;
				alucardApi.MaxtHearts = pointsPool;
			}	

			alucardApi.MaxtHp = pointsHp;
			alucardApi.MaxtMp = pointsMp;
			
		}
		private void RandomizeInventory()
		{
			bool hasHolyGlasses = alucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = alucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = alucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = alucardApi.HasItemInInventory("Silver Ring");

			alucardApi.ClearInventory();

			int itemCount = rng.Next(toolConfig.Khaos.PandoraMinItems, toolConfig.Khaos.PandoraMaxItems + 1);

			for (int i = 0; i < itemCount; i++)
			{
				int result = rng.Next(0, Equipment.Items.Count);
				alucardApi.GrantItemByName(Equipment.Items[result]);
			}

			if (hasHolyGlasses)
			{
				alucardApi.GrantItemByName("Holy glasses");
			}
			if (hasSpikeBreaker)
			{
				alucardApi.GrantItemByName("Spike Breaker");
			}
			if (hasGoldRing)
			{
				alucardApi.GrantItemByName("Gold Ring");
			}
			if (hasSilverRing)
			{
				alucardApi.GrantItemByName("Silver Ring");
			}
		}
		private void RandomizeSubweapon()
		{
			var subweapons = Enum.GetValues(typeof(Subweapon));
			alucardApi.Subweapon = (Subweapon) subweapons.GetValue(rng.Next(subweapons.Length));
		}
		private void RandomizeRelicsActivate(bool randomizeVladRelics = true)
		{
			Array? relics = Enum.GetValues(typeof(Relic));
			foreach (object? relic in relics)
			{
				if ((int) relic < 25)
				{
					sotnApi.AlucardApi.GrantRelic((Relic) relic);
				}
				int roll = rng.Next(0, 2);
				if (roll > 0)
				{
					if ((int) relic < 25)
					{
						sotnApi.AlucardApi.GrantRelic((Relic) relic);
					}
				}
				else
				{
					if ((int) relic < 25)
					{
						sotnApi.AlucardApi.TakeRelic((Relic) relic);
					}
					else if (randomizeVladRelics)
					{
						sotnApi.AlucardApi.TakeRelic((Relic) relic);
					}
				}
			}

			if (alucardSecondCastle)
			{
				int roll = rng.Next(0, Constants.Khaos.FlightRelics.Count);
				foreach (Relic relic in Constants.Khaos.FlightRelics[roll])
				{
					sotnApi.AlucardApi.GrantRelic((Relic) relic);
				}
			}

			if (IsInRoomList(Constants.Khaos.SwitchRoom))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.JewelOfOpen);
			}

		}
		private void RandomizeEquipmentSlots()
		{
			bool equippedBuggyQuickSwapWeaponRight = Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) (alucardApi.RightHand)]);
			bool equippedBuggyQuickSwapWeaponLeft = Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) (alucardApi.LeftHand)]);
			bool equippedHolyGlasses = Equipment.Items[(int) (alucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (alucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";
			
			uint newRightHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			uint newLeftHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			uint newArmor = (uint) rng.Next(0, Equipment.ArmorCount + 1);
			uint newHelm = Equipment.HelmStart + (uint) rng.Next(0, Equipment.HelmCount + 1);
			uint newCloak = Equipment.CloakStart + (uint) rng.Next(0, Equipment.CloakCount + 1);
			uint newAccessory1 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);
			uint newAccessory2 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);

			if (equippedBuggyQuickSwapWeaponRight || equippedBuggyQuickSwapWeaponLeft)
			{
				while (Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) newRightHand]))
				{
					newRightHand = (uint) rng.Next(0, Equipment.HandCount + 1);
				}
				while (Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) newLeftHand]))
				{
					newLeftHand = (uint) rng.Next(0, Equipment.HandCount + 1);
				}
			}

			alucardApi.RightHand = newRightHand;
			alucardApi.LeftHand = newLeftHand;
			alucardApi.Armor = newArmor;
			alucardApi.Helm = newHelm;
			alucardApi.Cloak = newCloak;
			alucardApi.Accessory1 = newAccessory1;
			alucardApi.Accessory2 = newAccessory2;

			RandomizeSubweapon();
			if (subweaponsOnlyActive)
			{
				while (alucardApi.Subweapon == Subweapon.Empty || alucardApi.Subweapon == Subweapon.Stopwatch)
				{
					RandomizeSubweapon();
				}
			}

			if (equippedHolyGlasses)
			{
				alucardApi.GrantItemByName("Holy glasses");
			}
			if (equippedSpikeBreaker)
			{
				alucardApi.GrantItemByName("Spike Breaker");
			}
			if (equippedGoldRing)
			{
				alucardApi.GrantItemByName("Gold Ring");
			}
			if (equippedSilverRing)
			{
				alucardApi.GrantItemByName("Silver Ring");
			}
		}
		#endregion
		#region Debuff events
		private void BloodManaUpdate()
		{
			if (spentMana > 0)
			{
				uint currentHp = alucardApi.CurrentHp;
				if (currentHp > spentMana)
				{
					alucardApi.CurrentHp -= (uint) spentMana;
					alucardApi.CurrentMp += (uint) spentMana;
				}
				else
				{
					alucardApi.State = States.Standing;
					bloodManaDeathTimer.Start();
					//alucardApi.CurrentHp = 1;
				}
			}
		}
		private void KillAlucard(Object sender, EventArgs e)
		{
			if (alucardApi.State == States.Standing)
			{
				alucardApi.State = States.Death;
				bloodManaDeathTimer.Stop();
			}
		}
		private void BloodManaOff(Object sender, EventArgs e)
		{
			manaLocked = false;
			bloodManaActive = false;
			bloodManaTimer.Stop();
		}
		private void ThirstDrain(Object sender, EventArgs e)
		{
			uint superDrain = superThirst ? Constants.Khaos.SuperThirstExtraDrain : 0u;
			if (alucardApi.CurrentHp > toolConfig.Khaos.ThirstDrainPerSecond + 1 + + superDrain)
			{
				alucardApi.CurrentHp -= (toolConfig.Khaos.ThirstDrainPerSecond + superDrain);
			}
			else
			{
				alucardApi.CurrentHp = 1;
			}
		}
		private void ThirstOff(Object sender, EventArgs e)
		{
			//Cheat darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			darkMetamorphasisCheat.Disable();
			thirstTimer.Stop();
			thirstTickTimer.Stop();
			superThirst = false;
		}
		private void HordeOff(Object sender, EventArgs e)
		{
			superHorde = false;
			spawnActive = false;
			hordeEnemies.RemoveRange(0, hordeEnemies.Count);
			hordeTimer.Interval = 5 * (60 * 1000);
			hordeTimer.Stop();
			hordeSpawnTimer.Stop();
		}
		private void HordeSpawn(Object sender, EventArgs e)
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			bool keepRichterRoom = ((mapX >= 31 && mapX <= 34) && mapY == 8);
			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5 || gameApi.CanSave() || keepRichterRoom)
			{
				return;
			}

			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (hordeZone != zone || hordeZone2 != zone2 || hordeEnemies.Count == 0)
			{
				hordeEnemies.RemoveRange(0, hordeEnemies.Count);
				FindHordeEnemy();
				hordeZone = zone;
				hordeZone2 = zone2;
			}
			else if (hordeEnemies.Count > 0)
			{
				FindHordeEnemy();
				int enemyIndex = rng.Next(0, hordeEnemies.Count);
				if (hordeTimer.Interval == 5 * (60 * 1000))
				{
					hordeTimer.Stop();
					hordeTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault().Duration.TotalMilliseconds;
					notificationService.AddTimer(new Services.Models.ActionTimer
					{
						Name = KhaosActionNames.KhaosHorde,
						Type = Enums.ActionType.Debuff,
						Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.KhaosHorde).FirstOrDefault().Duration
					});
					hordeTimer.Start();
				}
				hordeEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				hordeEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				hordeEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				actorApi.SpawnActor(hordeEnemies[enemyIndex]);
			}
		}
		private bool FindHordeEnemy()
		{
			uint roomX = gameApi.MapXPos;
			uint roomY = gameApi.MapYPos;

			if ((roomX == hordeTriggerRoomX && roomY == hordeTriggerRoomY) || !gameApi.InAlucardMode() || !gameApi.CanMenu())
			{
				return false;
			}

			long enemy = actorApi.FindActorFrom(Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? hordeEnemy = new Actor(actorApi.GetActor(enemy));

				if (hordeEnemy is not null && hordeEnemies.Where(e => e.Sprite == hordeEnemy.Sprite).Count() < 1)
				{
					if (superHorde)
					{
						hordeEnemy.Hp *= 2;
						hordeEnemy.Damage *= 2;

						int damageTypeRoll = rng.Next(0, 4);

						switch (damageTypeRoll)
						{
							case 1:
								hordeEnemy.DamageTypeA = (uint) Actors.Poison;
								break;
							case 2:
								hordeEnemy.DamageTypeB = (uint) Actors.Curse;
								break;
							case 3:
								hordeEnemy.DamageTypeA = (uint) Actors.Stone;
								hordeEnemy.DamageTypeB = (uint) Actors.Stone;
								break;
							default:
								break;
						}
					}
					hordeEnemies.Add(hordeEnemy);
					Console.WriteLine($"Added horde enemy with hp: {hordeEnemy.Hp} sprite: {hordeEnemy.Sprite} damage: {hordeEnemy.Damage}");
					return true;
				}
			}

			return false;
		}
		private void SubweaponsOnlyOff(object sender, EventArgs e)
		{
			Cheat curse = cheats.GetCheatByName("CurseTimer");
			curse.Disable();
			heartsLocked = false;
			manaLocked = false;
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.Disable();
			Cheat hearts = cheats.GetCheatByName("Hearts");
			hearts.Disable();
			if (gasCloudTaken)
			{
				alucardApi.GrantRelic(Relic.GasCloud);
				gasCloudTaken = false;
			}
			subweaponsOnlyTimer.Stop();
			subweaponsOnlyActive = false;
		}
		private void CrippleOff(Object sender, EventArgs e)
		{
			SetSpeed();
			//Cheat underwaterPhysics = cheats.GetCheatByName("UnderwaterPhysics");
			underwaterPhysics.Disable();
			crippleTimer.Stop();
			speedLocked = false;
		}
		private void EnduranceSpawn(Object sender, EventArgs e)
		{
			uint roomX = gameApi.MapXPos;
			uint roomY = gameApi.MapYPos;
			float healthMultiplier = 2.5F;

			if ((roomX == enduranceRoomX && roomY == enduranceRoomY) || !gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5)
			{
				return;
			}

			Actor? bossCopy = null;
			
			long enemy = actorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceRomhackBosses : Constants.Khaos.EnduranceBosses);
		
			if (enemy > 0)
			{
				LiveActor boss = actorApi.GetLiveActor(enemy);
				bossCopy = new Actor(actorApi.GetActor(enemy));
				Console.WriteLine($"Endurance boss found hp: {bossCopy.Hp}, damage: {bossCopy.Damage}, sprite: {bossCopy.Sprite}");

				bossCopy.Xpos = (ushort) (bossCopy.Xpos + rng.Next(-70, 70));
				bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
				bossCopy.Hp = (ushort) (healthMultiplier * bossCopy.Hp);
				actorApi.SpawnActor(bossCopy);

				boss.Hp = (ushort) (healthMultiplier * boss.Hp);

				if (superEndurance)
				{
					superEndurance = false;

					bossCopy.Xpos = rng.Next(0, 2) == 1 ? (ushort) (bossCopy.Xpos + rng.Next(-80, -20)) : (ushort) (bossCopy.Xpos + rng.Next(20, 80));
					bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
					actorApi.SpawnActor(bossCopy);
				}

				enduranceCount--;
				enduranceRoomX = roomX;
				enduranceRoomY = roomY;
				if (enduranceCount == 0)
				{
					enduranceSpawnTimer.Stop();
				}
			}
			else {
				enemy =  actorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceAlternateRomhackBosses : Constants.Khaos.EnduranceAlternateBosses);
				if (enemy > 0)
				{
					LiveActor boss = actorApi.GetLiveActor(enemy);
					string name = Constants.Khaos.EnduranceAlternateBosses.Where(e => e.Sprite == boss.Sprite).FirstOrDefault().Name;
					Console.WriteLine($"Endurance alternate boss found name: {name}");

					boss.Palette = (ushort) (boss.Palette + rng.Next(1, 10));

					if (superEndurance)
					{
						boss.Hp = (ushort) Math.Round((healthMultiplier * 2.0) * boss.Hp);
						superEndurance = false;
						notificationService.AddMessage($"Super Endurance {name}");
					}
					else
					{
						boss.Hp = (ushort) Math.Round((healthMultiplier * 1.3) * boss.Hp);
						notificationService.AddMessage($"Endurance {name}");
					}

					enduranceCount--;
					enduranceRoomX = roomX;
					enduranceRoomY = roomY;
					if (enduranceCount == 0)
					{
						enduranceSpawnTimer.Stop();
					}
				}
				else
				{
					return;
				}
			}
		}
		private void SpawnPoisonHitbox()
		{
			Actor hitbox = new();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 2);
			hitbox.DamageTypeA = (uint) Actors.Poison;
			actorApi.SpawnActor(hitbox);
		}
		private void SpawnCurseHitbox()
		{
			Actor hitbox = new();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 2);
			hitbox.DamageTypeB = (uint) Actors.Curse;
			actorApi.SpawnActor(hitbox);
		}
		private void SpawnStoneHitbox()
		{
			Actor hitbox = new();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 2);
			hitbox.DamageTypeA = (uint) Actors.Stone;
			hitbox.DamageTypeB = (uint) Actors.Stone;
			actorApi.SpawnActor(hitbox);
		}
		private void SpawnSlamHitbox()
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 5);
			hitbox.DamageTypeA = (uint) Actors.Slam;
			actorApi.SpawnActor(hitbox);
		}
		private void BankruptActivate()
		{
			bool hasHolyGlasses = alucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = alucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = alucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = alucardApi.HasItemInInventory("Silver Ring");
			bool equippedHolyGlasses = Equipment.Items[(int) (alucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (alucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";


			alucardApi.Gold = 0;
			alucardApi.ClearInventory();
			alucardApi.RightHand = 0;
			alucardApi.LeftHand = 0;
			alucardApi.Helm = Equipment.HelmStart;
			alucardApi.Armor = 0;
			alucardApi.Cloak = Equipment.CloakStart;
			alucardApi.Accessory1 = Equipment.AccessoryStart;
			alucardApi.Accessory2 = Equipment.AccessoryStart;
			gameApi.RespawnItems();

			if (equippedHolyGlasses || hasHolyGlasses)
			{
				alucardApi.GrantItemByName("Holy glasses");
			}
			if (equippedSpikeBreaker || hasSpikeBreaker)
			{
				alucardApi.GrantItemByName("Spike Breaker");
			}
			if (equippedGoldRing || hasGoldRing)
			{
				alucardApi.GrantItemByName("Gold Ring");
			}
			if (equippedSilverRing || hasSilverRing)
			{
				alucardApi.GrantItemByName("Silver Ring");
			}
		}
		private void HnKOff(object sender, EventArgs e)
		{
			invincibilityCheat.Disable();
			defencePotionCheat.Disable();
			hnkTimer.Stop();
			invincibilityLocked = false;
		}
		#endregion
		#region Buff events
		private void VampireOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			//Cheat darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Disable();
			//Cheat attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			attackPotionCheat.PokeValue(1);
			attackPotionCheat.Disable();
			vampireTimer.Stop();
		}
		private void MagicianOff(Object sender, EventArgs e)
		{
			//Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.Disable();
			manaLocked = false;
			magicianTimer.Stop();
		}
		private void MeltyBloodOff(Object sender, EventArgs e)
		{
			Cheat width = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			Cheat height = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			Cheat width2 = cheats.GetCheatByName("AlucardAttackHitbox2Width");
			Cheat height2 = cheats.GetCheatByName("AlucardAttackHitbox2Height");
			width.Disable();
			height.Disable();
			width2.Disable();
			height2.Disable();

			if (superMelty)
			{
				superMelty = false;
				SetSpeed();
			}

			meltyTimer.Stop();
		}
		private void FourBeastsOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			//Cheat invincibilityCheat = cheats.GetCheatByName("Invincibility");
			invincibilityCheat.Disable();
			invincibilityLocked = false;
			//Cheat attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			attackPotionCheat.Disable();
			//Cheat shineCheat = cheats.GetCheatByName("Shine");
			shineCheat.Disable();
			fourBeastsTimer.Stop();
		}
		private void ZawarudoOff(Object sender, EventArgs e)
		{
			//Cheat stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			stopwatchTimer.Disable();
			zawarudoTimer.Stop();
			zawarudoCheckTimer.Stop();
			zaWarudoActive = false;
		}
		private void ZaWarudoAreaCheck(Object sender, EventArgs e)
		{
			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (zaWarudoZone != zone || zaWarudoZone2 != zone2)
			{
				zaWarudoZone = zone;
				zaWarudoZone2 = zone2;
				alucardApi.ActivateStopwatch();
			}
		}
		private void HasteOff(Object sender, EventArgs e)
		{
			hasteTimer.Stop();
			SetSpeed();
			superHaste = false;
			hasteActive = false;
			speedLocked = false;
			hasteOverdriveOffTimer.Start();
		}
		private void SetHasteStaticSpeeds(bool super = false)
		{
			float superFactor = super ? 2F : 1F;
			float superWingsmashFactor = super ? 1.5F : 1F;
			float factor = toolConfig.Khaos.HasteFactor;
			alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5));
			alucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2));
			alucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * ((factor * superFactor) / 2));
			Console.WriteLine("Set speeds:");
			Console.WriteLine($"Wingsmash: {(uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5))}");
			Console.WriteLine($"Wolf dash right: {(sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2))}");
			Console.WriteLine($"Wolf dash left: {(sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * ((factor * superFactor) / 2))}");
		}
		private void ToggleHasteDynamicSpeeds(float factor = 1)
		{
			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);

			alucardApi.WalkingWholeSpeed = horizontalWhole;
			alucardApi.WalkingFractSpeed = horizontalFract;
			alucardApi.JumpingHorizontalWholeSpeed = horizontalWhole;
			alucardApi.JumpingHorizontalFractSpeed = horizontalFract;
			alucardApi.JumpingAttackLeftHorizontalWholeSpeed = (uint) (0xFF - horizontalWhole);
			alucardApi.JumpingAttackLeftHorizontalFractSpeed = horizontalFract;
			alucardApi.JumpingAttackRightHorizontalWholeSpeed = horizontalWhole;
			alucardApi.JumpingAttackRightHorizontalFractSpeed = horizontalFract;
			alucardApi.FallingHorizontalWholeSpeed = horizontalWhole;
			alucardApi.FallingHorizontalFractSpeed = horizontalFract;
		}
		private void OverdriveOn(object sender, System.Timers.ElapsedEventArgs e)
		{
			//Cheat VisualEffectPaletteCheat = cheats.GetCheatByName("VisualEffectPalette");
			visualEffectPaletteCheat.PokeValue(33126);
			visualEffectPaletteCheat.Enable();
			//Cheat VisualEffectTimerCheat = cheats.GetCheatByName("VisualEffectTimer");
			visualEffectTimerCheat.PokeValue(30);
			visualEffectTimerCheat.Enable();
			alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * (toolConfig.Khaos.HasteFactor / 1.8));
			overdriveOn = true;
			hasteOverdriveTimer.Stop();
		}
		private void OverdriveOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			//Cheat VisualEffectPaletteCheat = cheats.GetCheatByName("VisualEffectPalette");
			visualEffectPaletteCheat.Disable();
			//Cheat VisualEffectTimerCheat = cheats.GetCheatByName("VisualEffectTimer");
			visualEffectTimerCheat.Disable();
			if (hasteActive)
			{
				SetHasteStaticSpeeds(superHaste);
			}
			else
			{
				alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal);
			}
			overdriveOn = false;
			hasteOverdriveOffTimer.Stop();
		}
		private void LordOff(Object sender, EventArgs e)
		{
			spawnActive = false;
			lordEnemies.RemoveRange(0, hordeEnemies.Count);
			lordTimer.Interval = 5 * (60 * 1000);
			lordTimer.Stop();
			lordSpawnTimer.Stop();
		}
		private void LordSpawn(Object sender, EventArgs e)
		{
			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5 || gameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone))
			{
				return;
			}

			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (lordZone != zone || lordZone2 != zone2 || lordEnemies.Count == 0)
			{
				lordEnemies.RemoveRange(0, lordEnemies.Count);
				FindLordEnemy();
				lordZone = zone;
				lordZone2 = zone2;
			}
			else if (lordEnemies.Count > 0)
			{
				FindLordEnemy();
				int enemyIndex = rng.Next(0, lordEnemies.Count);
				if (lordTimer.Interval == 5 * (60 * 1000))
				{

					lordTimer.Stop();
					lordTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Lord).FirstOrDefault().Duration.TotalMilliseconds;
					notificationService.AddTimer(new Services.Models.ActionTimer
					{
						Name = KhaosActionNames.KhaosHorde,
						Type = Enums.ActionType.Debuff,
						Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Lord).FirstOrDefault().Duration
					});
					lordTimer.Start();
					
					//string message = meterFull ? $"You're now a Super Lord" : $"You're now a Lord";
					//notificationService.AddMessage(message);
					//Alert(KhaosActionNames.Lord);	
				}
				lordEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				lordEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				lordEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				actorApi.SpawnActor(lordEnemies[enemyIndex], false);
			}
		}
		private bool FindLordEnemy()
		{
			uint roomX = gameApi.MapXPos;
			uint roomY = gameApi.MapYPos;

			if ((roomX == lordTriggerRoomX && roomY == lordTriggerRoomY) || !gameApi.InAlucardMode() || !gameApi.CanMenu())
			{
				return false;
			}

			long enemy = actorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.AcceptedRomhackHordeEnemies : Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? lordEnemy = new Actor(sotnApi.ActorApi.GetActor(enemy));

				if (lordEnemy is not null && lordEnemies.Where(e => e.Sprite == lordEnemy.Sprite).Count() < 1)
				{
					lordEnemies.Add(lordEnemy);
					Console.WriteLine($"Added Lord enemy with hp: {lordEnemy.Hp} sprite: {lordEnemy.Sprite} damage: {lordEnemy.Damage}");
					return true;
				}
			}

			return false;
		}
		
		#endregion

		private bool IsInGalamothRoom()
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			return (mapX >= Constants.Khaos.GalamothRoomMapMinX && mapX <= Constants.Khaos.GalamothRoomMapMaxX) && (mapY >= Constants.Khaos.GalamothRoomMapMinY && mapY <= Constants.Khaos.GalamothRoomMapMaxY);
		}
		private bool IsInKeepRichterRoom()
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			return ((mapX >= Constants.Khaos.RichterRoomMapMinX && mapX <= Constants.Khaos.RichterRoomMapMaxX) && mapY == Constants.Khaos.RichterRoomMapY);
		}
		private bool IsInSuccubusRoom()
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			return (mapX == Constants.Khaos.SuccubusMapX && mapY == Constants.Khaos.SuccubusMapY);
		}
		private bool IsInEntranceCutscene()
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			return ((mapX >= Constants.Khaos.EntranceCutsceneMapMinX && mapX <= Constants.Khaos.EntranceCutsceneMapMaxX) && mapY == Constants.Khaos.EntranceCutsceneMapY);
		}
		
		
		private void StartCheats()
		{
			faerieScroll.Enable();
			Cheat batCardXp = cheats.GetCheatByName("BatCardXp");
			batCardXp.PokeValue(10000);
			batCardXp.Enable();
			Cheat ghostCardXp = cheats.GetCheatByName("GhostCardXp");
			ghostCardXp.PokeValue(10000);
			ghostCardXp.Enable();
			Cheat faerieCardXp = cheats.GetCheatByName("FaerieCardXp");
			faerieCardXp.Enable();
			Cheat demonCardXp = cheats.GetCheatByName("DemonCardXp");
			demonCardXp.Enable();
			Cheat swordCardXp = cheats.GetCheatByName("SwordCardXp");
			swordCardXp.PokeValue(7000);
			swordCardXp.Enable();
			Cheat spriteCardXp = cheats.GetCheatByName("SpriteCardXp");
			spriteCardXp.Enable();
			Cheat noseDevilCardXp = cheats.GetCheatByName("NoseDevilCardXp");
			noseDevilCardXp.Enable();
			savePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			savePalette.Enable();
		}
		
		private void SetSaveColorPalette()
		{
			if (alucardSecondCastle)
			{
				savePalette.PokeValue(Constants.Khaos.SaveIcosahedronSecondCastle);
			}
			else
			{
				savePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			}
		}

		private void CheckMainMenu()
		{
			if (inMainMenu != (sotnApi.GameApi.Status == SotnApi.Constants.Values.Game.Status.MainMenu))
			{
				if (inMainMenu && (sotnApi.GameApi.Status != SotnApi.Constants.Values.Game.Status.InGame))
				{
					return;
				}
				inMainMenu = sotnApi.GameApi.Status == SotnApi.Constants.Values.Game.Status.MainMenu;
				if (inMainMenu)
				{
					GainKhaosMeter((short) toolConfig.Khaos.MeterOnReset);
				}
			}
		}

		private void CheckCastleChanged()
		{
			if (alucardSecondCastle != sotnApi.GameApi.SecondCastle)
			{
				alucardSecondCastle = sotnApi.GameApi.SecondCastle;
				SetSaveColorPalette();
			}
		}
		public void GetCheats()
		{
			faerieScroll = cheats.GetCheatByName("FaerieScroll");
			darkMetamorphasisCheat = cheats.GetCheatByName("DarkMetamorphasis");
			underwaterPhysics = cheats.GetCheatByName("UnderwaterPhysics");
			hearts = cheats.GetCheatByName("Hearts");
			curse = cheats.GetCheatByName("CurseTimer");
			manaCheat = cheats.GetCheatByName("Mana");
			attackPotionCheat = cheats.GetCheatByName("AttackPotion");
			defencePotionCheat = cheats.GetCheatByName("DefencePotion");
			stopwatchTimer = cheats.GetCheatByName("SubweaponTimer");
			hitboxWidth = cheats.GetCheatByName("AlucardAttackHitboxWidth");
			hitboxHeight = cheats.GetCheatByName("AlucardAttackHitboxHeight");
			hitbox2Width = cheats.GetCheatByName("AlucardAttackHitbox2Width");
			hitbox2Height = cheats.GetCheatByName("AlucardAttackHitbox2Height");
			invincibilityCheat = cheats.GetCheatByName("Invincibility");
			shineCheat = cheats.GetCheatByName("Shine");
			visualEffectPaletteCheat = cheats.GetCheatByName("VisualEffectPalette");
			visualEffectTimerCheat = cheats.GetCheatByName("VisualEffectTimer");
			savePalette = cheats.GetCheatByName("SavePalette");
			contactDamage = cheats.GetCheatByName("ContactDamage");
		}
		
		private bool IsInRoomList(List<MapLocation> rooms)
		{
			return rooms.Where(r => r.X == alucardMapX && r.Y == alucardMapY && Convert.ToBoolean(r.SecondCastle) == alucardSecondCastle).FirstOrDefault() is not null;
		}
		private void SetSpeed(float factor = 1)
		{
			bool slow = factor < 1;
			bool fast = factor > 1;

			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);

			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * factor);
			sotnApi.AlucardApi.WalkingWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.WalkingFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.JumpingHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingAttackLeftHorizontalWholeSpeed = (uint) (0xFF - horizontalWhole);
			sotnApi.AlucardApi.JumpingAttackLeftHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingAttackRightHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.JumpingAttackRightHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.FallingHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.FallingHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * factor);
			sotnApi.AlucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * factor);
			sotnApi.AlucardApi.BackdashDecel = slow == true ? DefaultSpeeds.BackdashDecelSlow : DefaultSpeeds.BackdashDecel;
			Console.WriteLine($"Set all speeds with factor {factor}");
		}
		private void SetShaftHp()
		{
			long shaftAddress = actorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.ShaftActor });
			if (shaftAddress > 0)
			{
				LiveActor shaft = actorApi.GetLiveActor(shaftAddress);

				if (enduranceCount > 0)
				{
					enduranceCount--;
					enduranceRoomX = gameApi.MapXPos;
					enduranceRoomY = gameApi.MapYPos;
					if (enduranceCount == 0)
					{
						enduranceSpawnTimer.Stop();
					}
					if (superEndurance)
					{
						shaft.Hp = (ushort) Math.Round(1.6 * Constants.Khaos.ShaftKhaosHp);
						//notificationService.AddMessage($"Super Endurance Richter");
					}
					else
					{
						shaft.Hp = (ushort) Math.Round(1.3 * Constants.Khaos.ShaftKhaosHp);
						//notificationService.AddMessage($"Endurance Richter");
					}
				}
				else
				{
					shaft.Hp = Constants.Khaos.ShaftKhaosHp;
				}

				shaftHpSet = true;
				Console.WriteLine($"Found Shaft actor and set HP to: {Constants.Khaos.ShaftKhaosHp}");

			}
			else
			{
				return;
			}
		}
		private void SetGalamothtStats()
		{
			long galamothTorsoAddress = actorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.GalamothTorsoActor });
			if (galamothTorsoAddress > 0)
			{
				LiveActor galamothTorso = actorApi.GetLiveActor(galamothTorsoAddress);
				galamothTorso.Hp = Constants.Khaos.GalamothKhaosHp;
				galamothTorso.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;
				Console.WriteLine($"gala def: {galamothTorso.Def}");
				//galamothTorso.Def = 0; Removes XP gained
				
				if (enduranceCount > 0)
				{
					enduranceCount--;
					enduranceRoomX = gameApi.MapXPos;
					enduranceRoomY = gameApi.MapYPos;
					if (enduranceCount == 0)
					{
						enduranceSpawnTimer.Stop();
					}
					if (superEndurance)
					{
						galamothTorso.Hp = (ushort) Math.Round(3.0 * Constants.Khaos.GalamothKhaosHp);
						notificationService.AddMessage($"Super Endurance Galamoth");
					}
					else
					{
						galamothTorso.Hp = (ushort) Math.Round(2.0 * Constants.Khaos.GalamothKhaosHp);
						notificationService.AddMessage($"Endurance Galamoth");
					}
				}
				
				long galamothHeadAddress = actorApi.FindActorFrom(new List<SearchableActor> { Constants.Khaos.GalamothHeadActor });
				LiveActor galamothHead = actorApi.GetLiveActor(galamothHeadAddress);
				galamothHead.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;

				List<long> galamothParts = actorApi.GetAllActors(new List<SearchableActor> { Constants.Khaos.GalamothPartsActors });
				foreach (var actor in galamothParts)
				{
					LiveActor galamothAnchor = actorApi.GetLiveActor(actor);
					galamothAnchor.Xpos -= Constants.Khaos.GalamothKhaosPositionOffset;
					galamothAnchor.Def = 0;
				}

				galamothStatsSet = true;
				Console.WriteLine("Found Galamoth actor and set stats.");
			}
			else
			{
				return;
			}
		}
		private void CheckManaUsage()
		{
			uint currentMana = alucardApi.CurrentMp;
			spentMana = 0;
			if (currentMana < storedMana)
			{
				spentMana = (int) storedMana - (int) currentMana;
			}
			storedMana = currentMana;
			BloodManaUpdate();
		}
		private void CheckDashInput()
		{
			if (inputService.RegisteredMove(InputKeys.Dash, Globals.UpdateCooldownFrames) && !hasteSpeedOn && hasteActive)
			{
				ToggleHasteDynamicSpeeds(superHaste ? toolConfig.Khaos.HasteFactor * Constants.Khaos.HasteDashFactor : toolConfig.Khaos.HasteFactor);
				hasteSpeedOn = true;
				hasteOverdriveTimer.Start();
			}
			else if (!inputService.ButtonHeld(InputKeys.Forward) && hasteSpeedOn)
			{
				ToggleHasteDynamicSpeeds();
				hasteSpeedOn = false;
				hasteOverdriveTimer.Stop();
				if (overdriveOn)
				{
					hasteOverdriveOffTimer.Start();
				}
			}
		}
		private void CheckExperience()
		{
			uint currentExperiecne = alucardApi.Experiecne;
			//gainedExperiecne = (int) currentExperiecne - (int) storedExperiecne;
			//storedExperiecne = currentExperiecne;
		}
		private void CheckWingsmashActive()
		{
			//bool wingsmashActive = alucardApi.Action == SotnApi.Constants.Values.Alucard.States.Bat;
			//gainedExperiecne = (int) currentExperiecne - (int) storedExperiecne;
			//storedExperiecne = currentExperiecne;
		}
		private bool KhaosMeterFull()
		{
			return notificationService.KhaosMeter >= 100;
		}
		private void GainKhaosMeter(short meter)
		{
			notificationService.KhaosMeter += meter;
		}
		private void SpendKhaosMeter()
		{
			notificationService.KhaosMeter -= 100;
		}
		private void Alert(string actionName)
		{
			if (!toolConfig.Khaos.Alerts)
			{
				return;
			}

			var action = toolConfig.Khaos.Actions.Where(a => a.Name == actionName).FirstOrDefault();

			if (action is not null && action.AlertPath is not null && action.AlertPath != String.Empty)
			{
				notificationService.PlayAlert(action.AlertPath);
			}
		}
		private void BotMessageReceived(object sender, MessageReceivedEventArgs e)
		{
			JObject eventJson = JObject.Parse(Encoding.UTF8.GetString(e.Data));
			Console.WriteLine("Message from bot: \n" + eventJson.ToString());

			if (eventJson["event"] is not null && eventJson["data"] is not null && eventJson["event"].ToString() == Globals.ActionSocketEvent)
			{
				JObject actionData = JObject.Parse(eventJson["data"].ToString().Replace("/", ""));
				if (actionData["Command"] is not null && actionData["UserName"] is not null)
				{
					EnqueueAction(new EventAddAction { Command = actionData["Command"].ToString(), UserName = actionData["UserName"].ToString() });
				}
			}
			else if (eventJson["event"] is not null && eventJson["data"] is not null && eventJson["event"].ToString() == Globals.ConnectedSocketEvent)
			{
				notificationService.AddMessage($"Bot connected");
			}
		}
		private void BotDisconnected(object sender, EventArgs e)
		{
			Console.WriteLine("Bot socket disconnected");
		}
		private void BotConnected(object sender, EventArgs e)
		{
			JObject auth = JObject.FromObject(new
			{
				author = Globals.Author,
				website = Globals.Website,
				api_key = toolConfig.Khaos.BotApiKey,
				events = new string[] { Globals.ActionSocketEvent }
			});
			socketClient.SendAsync(auth.ToString(), System.Net.WebSockets.WebSocketMessageType.Text);
			Console.WriteLine("Bot socket connected, sending authentication");
		}
	}
}
