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
		//Neutral
		private System.Timers.Timer heartsOnlyTimer = new();
		private System.Timers.Timer unarmedTimer = new();
		private System.Timers.Timer turboModeTimer = new();
		private System.Timers.Timer rushDownTimer = new();

		//Evil
		private System.Timers.Timer HPForMPTimer = new();
		private System.Timers.Timer hpForMPDeathTimer = new();
		private System.Timers.Timer underwaterTimer = new();
		private System.Timers.Timer hexTimer = new();
		private System.Timers.Timer slamJamTickTimer = new();
		private System.Timers.Timer toughBossesSpawnTimer = new();
		private System.Timers.Timer getJuggledTimer = new();
		private System.Timers.Timer ambushTimer = new();
		private System.Timers.Timer ambushSpawnTimer = new();

		//Good
		private System.Timers.Timer speedTimer = new();
		private System.Timers.Timer speedOverdriveTimer = new();
		private System.Timers.Timer speedOverdriveOffTimer = new();
		private System.Timers.Timer regenTimer = new();
		private System.Timers.Timer regenTickTimer = new();
		private System.Timers.Timer faceTankTimer = new();
		private System.Timers.Timer timeStopTimer = new();
		private System.Timers.Timer timeStopCheckTimer = new();
		private System.Timers.Timer spellcasterTimer = new();
		private System.Timers.Timer extraRangeTimer = new();
		private System.Timers.Timer summonerTimer = new();
		private System.Timers.Timer summonerSpawnTimer = new();

		#endregion

		#region Legacy Timers
		private System.Timers.Timer subweaponsOnlyTimer = new();
		private System.Timers.Timer honestGamerTimer = new();
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
		Cheat defensePotionCheat;
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

		Cheat batCardXp;
		Cheat ghostCardXp;
		Cheat faerieCardXp;
		Cheat demonCardXp;
		Cheat swordCardXp;
		Cheat spriteCardXp;
		Cheat noseDevilCardXp;

		//New to Mayhem
		Cheat spirtOrb;
		Cheat rushdownBible;
		Cheat turboMode;
		Cheat turboModeJump;
		Cheat accelTime;

		Cheat subWeaponDamage;
		Cheat throwMoreSubWeapons1;
		Cheat throwMoreSubWeapons2;
		Cheat throwMoreSubWeapons3;
		Cheat longerHolyWater1;
		Cheat longerHolyWater2;
		Cheat longerHolyWater3;
		Cheat tallerHolyWater1;
		Cheat tallerHolyWater2;
		Cheat tallerHolyWater3;


		#endregion

		#region Mayhem Variables
		private int vladRelicsObtained = 0;
		private bool hasHolyGlasses = false;

		private uint ambushZone = 0;
		private uint ambushZone2 = 0;
		private uint ambushTriggerRoomX = 0;
		private uint ambushTriggerRoomY = 0;
		private List<Actor> ambushEnemies = new();

		private uint timeStopZone = 0;
		private uint timeStopZone2 = 0;

		private uint summonerZone = 0;
		private uint summonerZone2 = 0;
		private uint summonerTriggerRoomX = 0;
		private uint summonerTriggerRoomY = 0;
		private List<Actor> summonerEnemies = new();

		private int toughBossesCount = 0;
		private uint toughBossesRoomX = 0;
		private uint toughBossesRoomY = 0;

		private string slamJamUser;

		private uint preRushdownCon = 0;
		private uint preRushdownLevel = 0;
		private uint rushdownCon = 12000;
		private uint hpGiven = 0;
		private uint hpTaken = 0;
		private uint mpTaken = 0;
		private uint heartsTaken = 0;
		private uint strTaken = 0;
		private uint conTaken = 0;
		private uint intTaken = 0;
		private uint lckTaken = 0;
		private uint leftHandTaken = 0;
		private uint rightHandTaken = 0;
		private uint subWeaponTaken = 0;
		private uint slamCount = 0;

		private uint minHP = 80;
		private uint minMP = 30;
		private uint minHearts = 50;
		private uint minStr = 7;
		private uint minCon = 7;
		private uint minInt = 7;
		private uint minLck = 7;

		private short painTradeLevel = 1;
		private short maxMayhemLevel = 1;
		private short heartsOnlyLevel = 1;
		private short unarmedLevel = 1;
		private short turboModeLevel = 1;
		private short rushDownLevel = 1;
		private short swapStatsLevel = 1;
		private short swapEquipmentLevel = 1;
		private short swapRelicsLevel = 1;
		private short pandemoniumLevel = 1;

		private bool underwaterActive = false;
		private bool underwaterPaused = false;
		private bool speedActive = false;
		private bool speedOn = false;
		private bool heartsOnlyActive = false;
		private bool unarmedActive = false;
		private bool rushDownActive = false;
		private bool HPForMPActive = false;

		private bool subWeaponsLocked = false;
		private bool weaponsLocked = false;
		private bool heartsLocked = false;
		private bool manaLocked = false;
		private bool invincibilityLocked = false;
		private bool gasCloudTaken = false;
		private bool soulOfWolfTaken = false;
		private bool soulOfBatTaken = false;
		private bool powerOfMistTaken = false;
		private bool grantedTempFlight = false;

		private bool superExtraRange = false;
		private bool superAmbush = false;
		private bool superToughBosses = false;
		private bool superRegen = false;

		#endregion

		#region Shared Variables

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

		private bool timeStopActive = false;
		private uint zaWarudoZone = 0;
		private uint zaWarudoZone2 = 0;

		private uint storedMana = 0;
		private int spentMana = 0;

		private bool statLocked = false;
		private bool speedLocked = false;
		private bool bloodManaActive = false;
		private bool overdriveOn = false;
		private bool subweaponsOnlyActive = false;
		private bool honestGamerActive = false;
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
		private bool superSpeed = false;

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
			ModifyDifficulty();
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

		public void StartMayhem()
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

			notificationService.AddMessage($"Mayhem started");
			Console.WriteLine("Mayhem started");
		}
		public void StopMayhem()
		{
			StopTimers();
			spirtOrb.Disable();
			faerieScroll.Disable();
			if (socketClient.Connected)
			{
				socketClient.Stop();
			}
			notificationService.AddMessage($"Mayhem stopped");
			Console.WriteLine("Mayhem stopped");
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

		public void ModifyDifficulty()
		{
			if (!toolConfig.Khaos.enforceMinStats)
			{
				minHP = 1;
				minMP = 1;
				minHearts = 1;
				minStr = 1;
				minCon = 1;
				minInt = 1;
				minLck = 1;
			}
		}

		private void InitializeTempVariables()
		{
			hpGiven = 0;
			hpTaken /= 10;
			mpTaken /= 5;
			heartsTaken /= 5;
			strTaken /= 5;
			conTaken /= 5;
			intTaken /= 5;
			lckTaken /= 5;
			leftHandTaken = 0;
			rightHandTaken = 0;
		}

		public void disableCurse(short commandIndex)
		{
			// WIP
			// 1 - HeartsLocked
			// 2 - Rushdown
			switch (commandIndex)
			{
				case 1:
					if (!rushDownActive)
					{
						curse.Disable();
						weaponsLocked = false;
					}
					break;
				case 2:
					if (!heartsLocked)
					{
						curse.Disable();
						weaponsLocked = false;
					}
					break;
				default:
					break;
			}
		}

		#region Neutral Effects
		public short NeutralMeterGain(short level)
		{
			short meter = 0;
			switch (level)
			{
				case 2:
					meter = 5;
					break;
				case 3:
					meter = 10;
					break;
				default:
					break;
			}
			return meter;
		}
		public void PainTrade(string user = "Mayhem")
		{

			uint currentHp = alucardApi.CurrentHp;
			uint currentMana = alucardApi.CurrentMp;
			uint currentHearts = alucardApi.CurrentHearts;
			uint currentGold = alucardApi.Gold;

			double a = rng.NextDouble() / 2;
			double b = rng.NextDouble() / 3;
			double c = rng.NextDouble() / 3;
			double sum = a + b + c;
			double painTrade = (9.00 + (3.00 * (4 -painTradeLevel))) / 100.00;

			double percentageHP = (double) (1.00 - ((a / sum) * painTrade));
			double percentageMP = (double) (1.00 - ((b / sum) * painTrade));
			double percentageHearts = (double) (1.00 - ((c / sum) * painTrade));
			double percentageGold = 1;

			if (manaLocked || currentMana < 2)
			{
				percentageHearts -= (1.00-percentageMP);
				percentageMP = 0;
			}
			if (heartsLocked || currentHearts < 2)
			{
				percentageGold -= (1.00-percentageHearts);
				percentageHearts = 0;
			}
			if (currentGold < 15 && percentageGold < 1)
			{
				percentageHP -= (1.00-percentageGold);
				percentageGold = 0;
			}

			//notificationService.AddMessage($"Debug - {painTrade}; a{a} b{b} c{c}; {percentageHP}HP/{percentageMP}MP/{percentageHearts}H/{percentageGold}GP");
			//notificationService.AddMessage($"Debug - {painTrade}; a{a} b{b} c{c}; {percentageHP}HP/{percentageMP}MP/{percentageHearts}H/{percentageGold}GP");

			uint flatRemoval = (uint) (8 - (2*painTradeLevel));
			uint newHP = (uint) Math.Ceiling((currentHp * percentageHP)-flatRemoval) ;
			uint newMP = (uint) Math.Ceiling((currentMana * percentageMP)-flatRemoval);
			uint newHearts = (uint) Math.Ceiling((currentHearts * percentageHearts)-(flatRemoval));
			uint newGold = (uint) Math.Ceiling((currentGold * percentageGold));
			uint HPTaken = currentHp - newHP;
			uint MPTaken = currentMana - newMP;
			uint HeartsTaken = currentHearts - newHearts;
			uint GoldTaken = currentGold - newGold;

			if (newHP < 1)
			{
				newHP = 1;
				HPTaken = 0;
			}
			if (newMP < 0)
			{
				newMP = 0;
				MPTaken = 0;
			}
			if (newHearts < 0)
			{
				newHearts = 0;
				HeartsTaken = 0;
			}
			if (newGold < 0)
			{
				newGold = 0;
				GoldTaken = 0;
			}

			if (!manaLocked)
			{
				alucardApi.CurrentMp = newMP;
			}
			if (!heartsLocked)
			{
				alucardApi.CurrentHearts = newHearts;
			}
			alucardApi.CurrentHp = newHP;
			alucardApi.Gold = newGold;

			string item = toolConfig.Khaos.PainTradeItemRewards[rng.Next(0, toolConfig.Khaos.PainTradeItemRewards.Length)];
			int rolls = 0;
			while (alucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
			{
				item = toolConfig.Khaos.PainTradeItemRewards[rng.Next(0, toolConfig.Khaos.PainTradeItemRewards.Length)];
				rolls++;
			}

			alucardApi.GrantItemByName(item);

			//notificationService.AddMessage($"You gave {HPTaken}HP/{MPTaken}MP/{HeartsTaken}H/{GoldTaken}GP");
			notificationService.AddMessage($"{user} used {KhaosActionNames.PainTrade} for {item}");
			Alert(KhaosActionNames.PainTrade);

			++painTradeLevel;
			if (painTradeLevel > 3)
			{
				painTradeLevel = 1;
			}
		}
		public void MaxMayhem(string user = "Mayhem")
		{
			//alucardApi.Armor = (uint)Equipment.Items.IndexOf("Axe Lord armor");

			//Equipment.Items.IndexOf("Axe Lord armor");
			//alucardApi.GrantItemByName("Axe Lord armor");

			for (int i = 0; i < (maxMayhemLevel * 3); i++)
			{
				alucardApi.GrantItemByName("Dynamite");
			}

			uint currentHP = alucardApi.CurrentHp;
			uint currentMP = alucardApi.CurrentMp;
			uint currentHearts = alucardApi.CurrentHearts;
			uint currentGold = alucardApi.Gold;

			double basePercentage = 10 * maxMayhemLevel;
			double percentageHP = (rng.Next(1, 10) + basePercentage) / 100;
			double percentageMP = (rng.Next(1, 10) + basePercentage) / 100;
			double percentageHearts = (rng.Next(1, 10) + basePercentage) / 100;
			double percentageGold = (rng.Next(1, 10) + basePercentage) / 100;

			if (manaLocked)
			{
				percentageMP = 1;
				percentageHP += (rng.Next(1, 5)) / 100;
				percentageGold += (rng.Next(1, 5)) / 100;
			}
			if (heartsLocked)
			{
				percentageHearts = 1;
				percentageHP += (rng.Next(1, 5)) / 100;
				percentageGold += (rng.Next(1, 5)) / 100;
			}

			int roll = rng.Next(1, 3);
			if (roll == 1)
			{
				percentageHP += 1;
			}
			else
			{
				percentageHP = 1 - percentageHP;
			}


			if (percentageMP != 1)
			{
				roll = rng.Next(1, 3);
				if (roll == 1)
				{
					percentageMP += 1;
				}
				else
				{
					percentageMP = 1 - percentageMP;
				}

			}
			if (percentageHearts != 1)
			{
				roll = rng.Next(1, 3);
				if (roll == 1)
				{
					percentageHearts += 1;
				}
				else
				{
					percentageHearts = 1 - percentageHearts;
				}
			}

			roll = rng.Next(1, 3);
			if (roll == 1)
			{
				percentageGold += 1;
			}
			else
			{
				percentageGold = 1 - percentageGold;
			}

			alucardApi.CurrentHp = (uint) Math.Ceiling(alucardApi.CurrentHp * percentageHP);
			alucardApi.CurrentMp = (uint) Math.Ceiling(alucardApi.CurrentMp * percentageHP);
			alucardApi.CurrentHearts = (uint) Math.Ceiling(alucardApi.CurrentHearts * percentageHearts);
			alucardApi.Gold = (uint) Math.Ceiling(alucardApi.Gold * percentageGold);
			GainKhaosMeter(100);

			notificationService.AddMessage($"{user} used {KhaosActionNames.MaxMayhem}({maxMayhemLevel})");

			++maxMayhemLevel;
			if (maxMayhemLevel > 3)
			{
				maxMayhemLevel = 1;
			}

			Alert(KhaosActionNames.MaxMayhem);
		}
		public void HeartsOnly(string user = "Mayhem")
		{
			heartsLocked = true;
			manaLocked = true;
			weaponsLocked = true;

			int roll = 0;

			switch (heartsOnlyLevel)
			{
				case 1:
					roll = rng.Next(1, 10);
					while (roll == 6)
					{
						roll = rng.Next(1, 10);
					}
					break;
				case 2:
					roll = rng.Next(0, 6);
					if (roll == 0)
					{
						//Turn empty hand (0) into Agunea (9).
						roll = 9;
					}
					throwMoreSubWeapons1.Enable();
					throwMoreSubWeapons2.Enable();
					throwMoreSubWeapons3.Enable();
					break;
				case 3:
					roll = rng.Next(0, 5);
					if (roll == 0)
					{
						//Turn empty hand (0) into Holy Water.
						roll = 3;
					}
					int newSubWeaponDamage = (int) (2 + (sotnApi.AlucardApi.Int * 5) + (sotnApi.AlucardApi.Level * 2));
					if (Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Brilliant mail")
					{
						newSubWeaponDamage = (int) (newSubWeaponDamage * 1.25);
					}
					//if (Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Staurolite" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Staurolite")
					//{
					//newSubWeaponDamage = (int) (newSubWeaponDamage * 1.4);
					//}
					subWeaponDamage.PokeValue(newSubWeaponDamage);
					subWeaponDamage.Enable();
					throwMoreSubWeapons1.Enable();
					throwMoreSubWeapons2.Enable();
					throwMoreSubWeapons3.Enable();
					tallerHolyWater1.Enable();
					tallerHolyWater2.Enable();
					tallerHolyWater3.Enable();
					longerHolyWater1.Enable();
					longerHolyWater2.Enable();
					longerHolyWater3.Enable();
					break;
				default:
					break;
			}

			alucardApi.ActivatePotion(Potion.SmartPotion);
			alucardApi.Subweapon = (Subweapon) roll;
			alucardApi.GrantRelic(Relic.CubeOfZoe);
			if (alucardApi.HasRelic(Relic.GasCloud))
			{
				alucardApi.TakeRelic(Relic.GasCloud);
				gasCloudTaken = true;
			}
			hearts.Enable();
			
			curse.Enable();
			manaCheat.PokeValue(7);
			manaCheat.Enable();
			heartsOnlyActive = true;
			heartsOnlyTimer.Start();

			string name = $"{KhaosActionNames.HeartsOnly}({heartsOnlyLevel})";
			notificationService.AddMessage($"{user} used {name}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Khaotic,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeartsOnly).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.HeartsOnly);

			++heartsOnlyLevel;
			if (heartsOnlyLevel > 3)
			{
				heartsOnlyLevel = 1;
			}
		}
		private void HeartsOnlyOff(object sender, EventArgs e)
		{
			disableCurse(1);
			subWeaponDamage.Disable();
			throwMoreSubWeapons1.Disable();
			throwMoreSubWeapons2.Disable();
			throwMoreSubWeapons3.Disable();
			tallerHolyWater1.Disable();
			tallerHolyWater2.Disable();
			tallerHolyWater3.Disable();
			longerHolyWater1.Disable();
			longerHolyWater2.Disable();
			longerHolyWater3.Disable();

			
			manaCheat.Disable();
			alucardApi.CurrentMp = alucardApi.MaxtMp;
			hearts.Disable();
			if (gasCloudTaken)
			{
				alucardApi.GrantRelic(Relic.GasCloud);
				gasCloudTaken = false;
			}
			heartsLocked = false;
			manaLocked = false;
			heartsOnlyActive = false;
			heartsOnlyTimer.Stop();

		}
		public void Unarmed(string user = "Mayhem")
		{
			// 1 Usage: Attack Cheat, Mana Lock, Heart Lock
			// 2 Usage: Defense Potion(See Guilty Gear), I - Frames(See Guilty Gear)
			// 3 Usage: Strength Potion +Guilty Gear basics used, Reset to 1.

			heartsLocked = true;
			weaponsLocked = true;
			subWeaponsLocked = true;
			unarmedActive = true;
			grantedTempFlight = false;

			alucardApi.GrantRelic(Relic.LeapStone);

			if (alucardApi.Subweapon != 0)
			{
				subWeaponTaken = (uint) alucardApi.Subweapon;
				alucardApi.Subweapon = 0;
			}

			if (alucardApi.HasRelic(Relic.SoulOfBat))
			{
				alucardApi.TakeRelic(Relic.SoulOfBat);
				soulOfBatTaken = true;
				grantedTempFlight = true;
			}
			if (alucardApi.HasRelic(Relic.SoulOfWolf))
			{
				alucardApi.TakeRelic(Relic.SoulOfWolf);
				soulOfWolfTaken = true;
			}
			if (alucardApi.HasRelic(Relic.PowerOfMist))
			{
				alucardApi.TakeRelic(Relic.PowerOfMist);
				powerOfMistTaken = true;
				if (alucardApi.HasRelic(Relic.FormOfMist))
				{
					grantedTempFlight = true;
				}
			}
			if (grantedTempFlight && !alucardApi.HasRelic(Relic.GravityBoots))
			{
				alucardApi.GrantRelic(Relic.GravityBoots);
			}
			else
			{
				grantedTempFlight = false;
			}


			alucardApi.Subweapon = 0;

			alucardApi.ActivatePotion(Potion.StrPotion);
			alucardApi.AttackPotionTimer = Constants.Khaos.UnarmedAttack;

			if (unarmedLevel > 1)
			{
				alucardApi.DefencePotionTimer = Constants.Khaos.UnarmedDefense;
				alucardApi.InvincibilityTimer = Constants.Khaos.UnarmedInvincibility;
				if (unarmedLevel > 2)
				{
					hitboxHeight.Enable();
					hitboxWidth.Enable();
					hitbox2Height.Enable();
					hitbox2Width.Enable();
				}
			}

			unarmedTimer.Start();

			string name = $"{KhaosActionNames.Unarmed}({unarmedLevel})";
			notificationService.AddMessage($"{user} used {name}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Khaotic,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Unarmed).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.Unarmed);

			++unarmedLevel;
			if (unarmedLevel > 3)
			{
				unarmedLevel = 1;
			}
		}

		private void UnarmedOff(Object sender, EventArgs e)
		{
			if (unarmedLevel == 1)
			{
				//attackPotionCheat.Disable();
				hitboxWidth.Disable();
				hitboxHeight.Disable();
				hitbox2Height.Disable();
				hitbox2Width.Disable();
			}
			if (soulOfBatTaken)
			{
				alucardApi.GrantRelic(Relic.SoulOfBat);
				soulOfBatTaken = false;
			}
			if (soulOfWolfTaken)
			{
				alucardApi.GrantRelic(Relic.SoulOfWolf);
				soulOfWolfTaken = false;
			}
			if (powerOfMistTaken)
			{
				alucardApi.GrantRelic(Relic.PowerOfMist);
				powerOfMistTaken = false;
			}
			if (grantedTempFlight)
			{
				alucardApi.TakeRelic(Relic.GravityBoots);
				grantedTempFlight = false;
			}
			if (subWeaponTaken != 0)
			{
				alucardApi.Subweapon = (Subweapon) subWeaponTaken;
				subWeaponTaken = 0;
			}

			heartsLocked = false;
			subWeaponsLocked = false;
			weaponsLocked = false;
			manaLocked = false;
			unarmedActive = false;
			unarmedTimer.Stop();
		}

		public void TurboMode(string user = "Mayhem")
		{
			// Remove leapstone if not already obtained?
			alucardApi.GrantRelic(Relic.LeapStone);
			turboMode.Enable();
			turboModeJump.Enable();
			turboModeTimer.Start();

			if (alucardApi.HasRelic(Relic.PowerOfMist))
			{
				if (alucardApi.HasRelic(Relic.FormOfMist))
				{
					grantedTempFlight = true;
				}
			}

			if (grantedTempFlight && !alucardApi.HasRelic(Relic.GravityBoots))
			{
				alucardApi.GrantRelic(Relic.GravityBoots);
			}
			else
			{
				grantedTempFlight = false;
			}

			if (turboModeLevel == 1)
			{
				accelTime.PokeValue(0);
				accelTime.Enable();
			}
			else
			{
				accelTime.Disable();
				if (turboModeLevel > 2)
				{
					attackPotionCheat.Enable();
				}
			}

			string name = $"{KhaosActionNames.TurboMode}({turboModeLevel})";
			notificationService.AddMessage($"{user} used {name}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Khaotic,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TurboMode).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.TurboMode);
			
			++turboModeLevel;
			if (turboModeLevel > 3)
			{
				turboModeLevel = 1;
			}

		}

		private void TurboModeOff(Object sender, EventArgs e)
		{
			attackPotionCheat.Disable();
			accelTime.PokeValue(5);

			if (grantedTempFlight)
			{
				alucardApi.TakeRelic(Relic.GravityBoots);
				grantedTempFlight = false;
			}

			//accelTime.Disable();
			turboMode.Disable();
			turboModeJump.Disable();
			turboModeTimer.Stop();
		}


		public void RushDown(string user = "Mayhem")
		{ 
			speedLocked = true;
			weaponsLocked = true;
			rushDownActive = true;
			invincibilityLocked = true;
			curse.Enable();
			preRushdownCon = 0;
			preRushdownLevel = 0;
			rushdownBible.Enable();

			if (rushDownLevel == 1)
			{
				alucardApi.CurrentHp = alucardApi.MaxtHp;
				alucardApi.ActivatePotion(Potion.ShieldPotion);
				alucardApi.DefencePotionTimer = Constants.Khaos.RushdownDefense;
				contactDamage.PokeValue(100);
				contactDamage.Enable();

			}
			else
			{
				shineCheat.PokeValue(1);
				shineCheat.Enable();
				if (rushDownLevel == 2)
				{
					preRushdownLevel = alucardApi.Level;
					preRushdownCon = alucardApi.Con;
					alucardApi.Con = rushdownCon;
					statLocked = true;
					contactDamage.PokeValue(25);
					contactDamage.Enable();
				}
				else
				{
					invincibilityCheat.PokeValue(1);
					invincibilityCheat.Enable();
					invincibilityLocked = true;
					contactDamage.PokeValue(35);
					contactDamage.Enable();
				}
			}
			rushDownTimer.Start();

			string name = $"{KhaosActionNames.RushDown}({rushDownLevel})";
			notificationService.AddMessage($"{user} used {name}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Khaotic,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RushDown).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.RushDown);

			++rushDownLevel;
			if (rushDownLevel > 3)
			{
				rushDownLevel = 1;
			}
		}
		private void ToggleRushdownDynamicSpeeds(float factor = 1)
		{
			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);
			uint horizontalAirWhole = (uint) (DefaultSpeeds.WalkingWhole * factor * 2.5);
			uint horizontalAirFract = (uint) (DefaultSpeeds.WalkingFract * factor * 2.5);

			sotnApi.AlucardApi.WalkingWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.WalkingFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.JumpingHorizontalFractSpeed = horizontalFract;
			sotnApi.AlucardApi.JumpingAttackLeftHorizontalWholeSpeed = (uint) (0xFF - horizontalAirWhole);
			sotnApi.AlucardApi.JumpingAttackLeftHorizontalFractSpeed = horizontalAirFract;
			sotnApi.AlucardApi.JumpingAttackRightHorizontalWholeSpeed = horizontalAirWhole;
			sotnApi.AlucardApi.JumpingAttackRightHorizontalFractSpeed = horizontalAirFract;
			sotnApi.AlucardApi.FallingHorizontalWholeSpeed = horizontalWhole;
			sotnApi.AlucardApi.FallingHorizontalFractSpeed = horizontalFract;
		}
		private void RushDownOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (rushDownLevel != 2)
			{
				invincibilityCheat.Disable();
				invincibilityLocked = false;
			}
			if((preRushdownLevel > 0) && preRushdownLevel < alucardApi.Level)
			{
				preRushdownCon += alucardApi.Level - preRushdownLevel;
			}

			if(preRushdownCon > 0)
			{
				alucardApi.Con = preRushdownCon;
			}
			preRushdownLevel = 0;
			preRushdownCon = 0;
			rushdownBible.Disable();
			disableCurse(2);
			shineCheat.Disable();
			contactDamage.Disable();
			sotnApi.AlucardApi.ContactDamage = 0;
			SetSpeed();
			rushDownTimer.Stop();
			statLocked = false;
			invincibilityLocked = false;
			heartsLocked = false;
			speedLocked = false;
			rushDownActive = false;
		}
		public void SwapStats(string user = "Mayhem")
		{
			RandomizeStatsActivate();
			short meter = NeutralMeterGain(swapStatsLevel);
			GainKhaosMeter(meter);
			notificationService.AddMessage($"{user} used {KhaosActionNames.SwapStats}({swapStatsLevel})");
			Alert(KhaosActionNames.SwapStats);

			++swapStatsLevel;
			if (swapStatsLevel > 3)
			{
				swapStatsLevel = 1;
			}


		}
		public void SwapEquipment(string user = "Mayhem")
		{
			RandomizeEquipmentSlots();
			if (swapEquipmentLevel >= 2)
			{
				RandomizeGold();
			}
			else if (swapEquipmentLevel == 3)
			{
				RandomizeInventory();
			}

			notificationService.AddMessage($"{user} used {KhaosActionNames.SwapEquipment}({swapEquipmentLevel})");
			Alert(KhaosActionNames.SwapEquipment);

			++swapEquipmentLevel;
			if (swapEquipmentLevel > 3)
			{
				swapEquipmentLevel = 1;
			}
		}
		public void SwapRelics(string user = "Mayhem")
		{
			RandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			short meter = NeutralMeterGain(swapRelicsLevel);
			GainKhaosMeter(meter);

			notificationService.AddMessage($"{user} used {KhaosActionNames.SwapRelics}({swapRelicsLevel})");
			Alert(KhaosActionNames.KhaosRelics);

			++swapRelicsLevel;
			if (swapRelicsLevel > 3)
			{
				swapRelicsLevel = 1;
			}

		}
		public void Pandemonium(string user = "Mayhem")
		{
			RandomizeGold();
			RandomizeStatsActivate();
			RandomizeEquipmentSlots();
			RandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			RandomizeInventory();
			RandomizeSubweapon();
			sotnApi.GameApi.RespawnBosses();
			sotnApi.GameApi.RespawnItems();
			notificationService.AddMessage($"{user} caused Pandemonium({pandemoniumLevel})");
			Alert(KhaosActionNames.Pandemonium);

			if (pandemoniumLevel >= 2)
			{
				RandomizePotion(user);
			}
			if (pandemoniumLevel == 3)
			{
				GainKhaosMeter(100);
			}

			++pandemoniumLevel;
			if (pandemoniumLevel > 3)
			{
				pandemoniumLevel = 1;
			}

		}
		private void RandomizeGold()
		{
			uint gold = (uint) rng.Next(50, 5000);
			uint roll = (uint) rng.Next(0, 21);
			if (roll > 16 && roll < 20)
			{
				gold = gold * (uint) rng.Next(11, 21);
			}
			else if (roll > 19)
			{
				gold = gold * (uint) rng.Next(21, 41);
			}
			else
			{
				gold = gold * 10;
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
			if (ActualStatPool < CalculatedStatPool)
			{
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
				else
				{
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
			if (heartsLocked)
			{
				Console.WriteLine("Skipping Swap Stats Hearts re-roll due to hearts lock.");
			}
			else
			{
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
				if (pointsPool < minHearts)
				{
					pointsPool = minHearts;
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
			bool hasLibraryCard = alucardApi.HasItemInInventory("Library card");

			alucardApi.ClearInventory();

			int itemCount = rng.Next(toolConfig.Khaos.PandemoniumMinItems, toolConfig.Khaos.PandemoniumMaxItems + 1);

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
			if (hasLibraryCard)
			{
				sotnApi.AlucardApi.GrantItemByName("Library card");
			}
		}
		private void RandomizeSubweapon()
		{
			var subweapons = Enum.GetValues(typeof(Subweapon));
			if (subWeaponsLocked)
			{
				alucardApi.Subweapon = 0;
			}
			else
			{
				alucardApi.Subweapon = (Subweapon) subweapons.GetValue(rng.Next(subweapons.Length));
			}
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
			sotnApi.AlucardApi.GrantItemByName("Library card");
		}
		private void RandomizeEquipmentSlots()
		{
			bool equippedBuggyQuickSwapWeaponRight = Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) (alucardApi.RightHand)]);
			bool equippedBuggyQuickSwapWeaponLeft = Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) (alucardApi.LeftHand)]);
			bool equippedHolyGlasses = Equipment.Items[(int) (alucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (alucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (alucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (alucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";
			bool equippedLibraryCard = Equipment.Items[(int) (alucardApi.RightHand)] == "Library card" || Equipment.Items[(int) (alucardApi.LeftHand)] == "Library card";

			//Console.WriteLine("Hand Values: ");
			//Console.WriteLine(alucardApi.LeftHand);
			//Console.WriteLine(alucardApi.RightHand);

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
			if (subweaponsOnlyActive || heartsOnlyActive)
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
			if (equippedLibraryCard)
			{
				alucardApi.GrantItemByName("Library card");
			}
		}

		public void RandomizePotion(string user = "Mayhem")
		{
			bool highMp = alucardApi.CurrentMp > alucardApi.MaxtMp * 0.6;
			bool highHp = alucardApi.CurrentHp > alucardApi.MaxtHp * 0.6;
			int min = 2;
			int max = 10;
			uint baseGain = 5 + alucardApi.Level;
			uint addGold = 0;

			int result = rng.Next(min, max);

			if (timeStopActive)
			{
				addGold = 50 + (uint) (10 * baseGain * (rng.NextDouble()));
				result = 1;
			}
			else {
				if (manaLocked || highMp)
				{
					++min;
					result = rng.Next(min, max);
				}
				if (result == 3 && highHp)
				{
					min = 4;
					result = rng.Next(min, max);
				}
			}

			switch (result)
			{
				case 1:
					sotnApi.AlucardApi.Gold += addGold;
					notificationService.AddMessage($"{user} gave you extra gold{alucardApi.Gold}");
					break;
				case 2:
					sotnApi.AlucardApi.ActivatePotion(Potion.Mannaprism);
					notificationService.AddMessage($"{user} gave you max MP");
					break;
				case 3:
					sotnApi.AlucardApi.ActivatePotion(Potion.Potion);
					notificationService.AddMessage($"{user} gave you light healing");
					break;
				case 4:
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					notificationService.AddMessage($"{user} gave you intelligence");
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
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistThunder);
					notificationService.AddMessage($"{user} gave you resistance to thunder");
					break;
				case 8:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistDark);
					notificationService.AddMessage($"{user} gave you resistance to dark");
					break;
				case 9:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave you defense");
					break;
				default:
					break;
			}
		}
		public void RandomizeModeratePotion(string user = "Mayhem")
		{
			bool highMp = alucardApi.CurrentMp > alucardApi.MaxtMp * 0.6;
			bool highHp = alucardApi.CurrentHp > alucardApi.MaxtHp * 0.6;
			bool highHearts = alucardApi.CurrentHearts > alucardApi.MaxtHearts * 0.6;
			int min = 2;
			int max = 10;
			uint baseGain = (5 + alucardApi.Level)*2;
			uint addGold = 0;

			int result = rng.Next(min, max);

			if (timeStopActive)
			{
				addGold = 100 + (uint) (10 * baseGain * (rng.NextDouble()));
				result = 1;
			}
			else
			{
				if (manaLocked || highMp)
				{
					min = 3;
					result = rng.Next(min, max);
				}
				if (result == 3 && (highHearts || heartsLocked))
				{
					min = 4;
					result = rng.Next(min, max);
				}
				if (result == 4 && highHp)
				{
					min = 5;
					result = rng.Next(min, max);
				}
			}

			switch (result)
			{
				case 1:
					sotnApi.AlucardApi.Gold += addGold;
					notificationService.AddMessage($"{user} gave you extra gold{alucardApi.Gold}");
					break;
				case 2:
					sotnApi.AlucardApi.ActivatePotion(Potion.Mannaprism);
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					notificationService.AddMessage($"{user} gave you max MP + Int");
					break;
				case 3:
					sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts;
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					notificationService.AddMessage($"{user} gave you max Hearts + Int");
					break;
				case 4:
					sotnApi.AlucardApi.ActivatePotion(Potion.HighPotion);
					notificationService.AddMessage($"{user} gave you moderate healing");
					break;
				case 5:
					sotnApi.AlucardApi.ActivatePotion(Potion.LuckPotion);
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					notificationService.AddMessage($"{user} gave you Int/Lck");
					break;
				case 6:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistFire);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistThunder);
					notificationService.AddMessage($"{user} gave you Fire/Thunder resistance");
					break;
				case 7:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistHoly);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistDark);
					notificationService.AddMessage($"{user} gave you Holy/Dark resistance");
					break;
				case 8:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave you defense");
					break;
				default:
					break;
			}
		}

		public void RandomizeMajorReward(string user = "Mayhem")
		{
			bool highMp = alucardApi.CurrentMp > alucardApi.MaxtMp * 0.6;
			bool highHp = alucardApi.CurrentHp > alucardApi.MaxtHp * 0.6;
			bool highHearts = alucardApi.CurrentHearts > alucardApi.MaxtHearts * 0.6;
			int min = 1;
			int max = 5;

			int result = rng.Next(min, max);

			if (result == 1 && highHp)
			{
				++min;
				result = rng.Next(min, max);
			}

			switch (result)
			{
				case 1:
					sotnApi.AlucardApi.ActivatePotion(Potion.Elixir);
					if (!manaLocked)
					{
						sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
					}
					if (!heartsLocked)
					{
						sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts;
					}
					notificationService.AddMessage($"{user} gave you full restore");
					break;
				case 2:
					sotnApi.AlucardApi.MaxtHp += 5;
					sotnApi.AlucardApi.MaxtMp += 10;
					sotnApi.AlucardApi.MaxtHearts += 10;
					notificationService.AddMessage($"{user} gave you bonus HP/MP/Hearts");
					break;
				case 3:
					sotnApi.AlucardApi.Str += 2;
					sotnApi.AlucardApi.Con += 2;
					if(preRushdownCon > 0)
					{
						preRushdownCon += 2;
					}
					notificationService.AddMessage($"{user} gave you bonus Str/Con");
					break;
				case 4:
					sotnApi.AlucardApi.Int += 2;
					sotnApi.AlucardApi.Lck += 2;
					notificationService.AddMessage($"{user} gave you bonus Int/Lck");
					break;
				case 5:
					sotnApi.AlucardApi.Str += 1;
					sotnApi.AlucardApi.Con += 1;
					if (preRushdownCon > 0)
					{
						preRushdownCon += 1;
					}
					sotnApi.AlucardApi.Int += 1;
					sotnApi.AlucardApi.Lck += 1;
					notificationService.AddMessage($"{user} gave you +1 to all stats");
					break;
				default:
					break;
			}
		}

		#endregion

		#region Negative Effects

		public System.TimeSpan EvilDurationGain(System.TimeSpan baseTimeSpan)
		{
			double timeFactor = 1.0 + (vladRelicsObtained / 10.0);
			int newDurationSeconds = (int) Math.Floor(baseTimeSpan.TotalSeconds * (timeFactor));
			int diffInSeconds = (int) Math.Floor(newDurationSeconds - baseTimeSpan.TotalSeconds);
			int diffInMinutes = (int) Math.Floor(diffInSeconds / 60.0);
			diffInSeconds = diffInSeconds % 60;
			TimeSpan newInterval = new TimeSpan(0, diffInMinutes, diffInSeconds);
			System.TimeSpan newTimeSpan = baseTimeSpan.Add(newInterval);
			Console.WriteLine($"Bad Duration: baseTimeSpan{baseTimeSpan}, newDur {newDurationSeconds}, diffInSec {diffInSeconds}, diffInMinutes{diffInMinutes}, newTimeSpan{newTimeSpan}");
			return newTimeSpan;
		}

		public void MinorTrap(string user = "Mayhem")
		{
			bool entranceCutscene = IsInRoomList(Constants.Khaos.EntranceCutsceneRooms);
			bool succubusRoom = IsInRoomList(Constants.Khaos.SuccubusRoom);
			

			List<int> effectNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
			int min = 1;
			int max = effectNumbers.Count;

			bool alucardIsImmuneToCurse = sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad)
				|| Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Coral circlet";
			bool alucardIsImmuneToStone = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Mirror cuirass"
				|| Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Medusa shield"
				|| Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Medusa shield";
			bool alucardIsImmuneToPoison = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Topaz circlet";

			if (alucardIsImmuneToStone || succubusRoom)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 1);
			}
			if (alucardIsImmuneToCurse)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 2);
			}
			if (alucardIsImmuneToPoison)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 3);
			}
			if (manaLocked)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 4);
			}
			if (heartsLocked)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 5);
			}
			if (succubusRoom || entranceCutscene)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 6);
			}
			//notificationService.AddMessage($"min{min} and max{max}");
			int result = effectNumbers[rng.Next(min, max)];
			//notificationService.AddMessage($"{result}: min{min} and max{max}");

			uint currentHp = sotnApi.AlucardApi.CurrentHp;
			uint currentMp = sotnApi.AlucardApi.CurrentMp;
			uint currentHearts = sotnApi.AlucardApi.CurrentHearts;
			uint currentGold = sotnApi.AlucardApi.Gold;

			if (vladRelicsObtained > 0)
			{
				uint lowHP = (uint) Math.Ceiling(sotnApi.AlucardApi.CurrentHp * ((100.00 - vladRelicsObtained) / 100.00));
				alucardApi.CurrentHp = lowHP;
			}

			double percentageHP = 5 / 100;
			double percentageMP = 5 / 100;
			double percentageHearts = 5 / 100;
			double percentageGold = 3 / 100;

			uint newHP = (uint) Math.Max(1, Math.Round(currentHp - (currentHp * percentageHP) - 5 - sotnApi.AlucardApi.Level));
			uint newMP = (uint) Math.Max(0, Math.Round(currentMp - (currentMp * percentageMP) - 5 - sotnApi.AlucardApi.Level));
			uint newHearts = (uint) Math.Max(0, Math.Round(currentHearts - (currentHearts * percentageHearts) - 5 - sotnApi.AlucardApi.Level));
			uint newGold = (uint) Math.Max(0, Math.Round(currentGold - (currentGold * percentageGold) - (6 * sotnApi.AlucardApi.Level)));

			switch (result)
			{
				case 1:
					SpawnStoneHitbox();
					notificationService.AddMessage($"{user} petrified you");
					break;
				case 2:
					SpawnCurseHitbox();
					notificationService.AddMessage($"{user} cursed you");
					break;
				case 3:
					SpawnPoisonHitbox();
					notificationService.AddMessage($"{user} poisoned you");
					break;
				case 4:
					sotnApi.AlucardApi.CurrentMp = newMP;
					notificationService.AddMessage($"{user} took some MP");
					break;
				case 5:
					sotnApi.AlucardApi.CurrentMp = newHearts;
					notificationService.AddMessage($"{user} took some Hearts");
					break;
				case 6:
					SpawnSlamHitbox();
					notificationService.AddMessage($"{user} slammed you");
					break;
				case 7:
					sotnApi.AlucardApi.Gold = newGold;
					notificationService.AddMessage($"{user} took some Gold");
					break;
				case 8:
					if (newHP < 4)
					{
						sotnApi.AlucardApi.CurrentHp = 1;
						SpawnSlamHitbox();
					}
					else
					{
						sotnApi.AlucardApi.CurrentHp = newHP;
					}
					notificationService.AddMessage($"{user} reduced your HP");
					break;
				default:
					break;
			}

			Alert(KhaosActionNames.MinorTrap);
		}
		public void HPForMP(string user = "Mayhem")
		{
			HPForMPActive = true;
			manaLocked = true;
			storedMana = alucardApi.CurrentMp;

			System.TimeSpan newDuration = EvilDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault().Duration);

			HPForMPTimer.Interval = newDuration.TotalMilliseconds;
			HPForMPTimer.Start();

			notificationService.AddMessage($"{user} used {KhaosActionNames.HPForMP}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.HPForMP,
				Type = Enums.ActionType.Debuff,
				Duration = newDuration
			});
			Alert(KhaosActionNames.HPForMP);
		}
		private void HPForMPOff(Object sender, EventArgs e)
		{
			HPForMPTimer.Stop();
			manaLocked = false;
			HPForMPActive = false;
		}
		public void Underwater(string user = "Mayhem")
		{
			speedLocked = true;
			underwaterActive = true;
			string name = KhaosActionNames.Underwater;

			if (IsInRoomList(Constants.Khaos.EntranceCutsceneRooms))
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Underwater, LocksSpeed = true, Invoker = new MethodInvoker(() => Underwater(user)) });
				return;
			}

			bool meterFull = KhaosMeterFull();
			float enhancedFactor = 1;
			if (meterFull)
			{
				name = "Super " + name; 
				enhancedFactor = Constants.Khaos.SuperUnderwaterFactor;
				SpendKhaosMeter();
			}
			alucardApi.GrantRelic(Relic.HolySymbol);
			SetSpeed(toolConfig.Khaos.UnderwaterFactor * enhancedFactor);
			underwaterPhysics.Enable();

			System.TimeSpan newDuration = EvilDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Underwater).FirstOrDefault().Duration);

			underwaterTimer.Interval = newDuration.TotalMilliseconds;
			underwaterTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Debuff,
				Duration = newDuration
			});

			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Underwater);
		}
		private void UnderwaterOff(Object sender, EventArgs e)
		{
			SetSpeed();
			underwaterActive = false;
			underwaterPhysics.Disable();
			underwaterTimer.Stop();
			speedLocked = false;
		}

		public void Slam(string user = "Mayhem", bool multiSlam = false)
		{
			bool entranceCutscene = IsInRoomList(Constants.Khaos.EntranceCutsceneRooms);
			bool succubusRoom = IsInRoomList(Constants.Khaos.SuccubusRoom);
			if (succubusRoom || entranceCutscene)
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Slam, Invoker = new MethodInvoker(() => Slam(user, multiSlam)) });
			}
			else
			{
				if (multiSlam)
				{
					SpawnSlamHitbox(true);
					notificationService.AddMessage($"{user}: Hex (Slam Jam)");
				}
				else
				{
					SpawnSlamHitbox();
					notificationService.AddMessage($"{user} slammed you");
				}
				Alert(KhaosActionNames.Slam);
			}
		}
		public void Hex(string user = "Mayhem")
		{

			if (IsInRoomList(Constants.Khaos.EntranceCutsceneRooms))
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Hex, Invoker = new MethodInvoker(() => Hex(user)) });
			}

			List<int> effectNumbers = new List<int>() { 1, 2, 3, 4};
			bool meterFull = false;
			
			int max = effectNumbers.Count;
			int min = 1;
			slamJamUser = "";

			if (sotnApi.AlucardApi.MaxtHp < 2)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 1);
			}

			if (heartsLocked || subWeaponsLocked)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 3);
			}

			int roll = effectNumbers[rng.Next(min, max)];
			string name = $"{KhaosActionNames.Hex}";
			string message = $"{user} used ";

			switch (roll){
				case 1:
					hpTaken = sotnApi.AlucardApi.MaxtHp / 2;
					mpTaken = sotnApi.AlucardApi.MaxtMp / 2;
					heartsTaken = sotnApi.AlucardApi.MaxtHearts / 2;
					
					if (sotnApi.AlucardApi.MaxtHp - hpTaken > 0)
					{
						sotnApi.AlucardApi.MaxtHp -= hpTaken;
					}
					if (sotnApi.AlucardApi.CurrentHp > 1)
					{
						sotnApi.AlucardApi.CurrentHp /= 2;
					}
					if (sotnApi.AlucardApi.MaxtMp - mpTaken > 0) {
						sotnApi.AlucardApi.MaxtMp -= mpTaken;
						if (!manaLocked && sotnApi.AlucardApi.CurrentMp > 1)
						{
							sotnApi.AlucardApi.CurrentMp /= 2;
						}
					}
					if (sotnApi.AlucardApi.MaxtHearts - heartsTaken > 0)
					{
						sotnApi.AlucardApi.MaxtHearts -= heartsTaken;
						if (!heartsLocked && sotnApi.AlucardApi.CurrentHearts > 1)
						{
							sotnApi.AlucardApi.CurrentHearts /= 2;
						}
					}
					name += "(HP/MP/H)";
					break;
				case 2:
					strTaken = sotnApi.AlucardApi.Str / 2;
					conTaken = sotnApi.AlucardApi.Con / 2;
					intTaken = sotnApi.AlucardApi.Int / 2;
					lckTaken = sotnApi.AlucardApi.Lck / 2;
					if(sotnApi.AlucardApi.Str > 1)
					{
						sotnApi.AlucardApi.Str -= strTaken;
					}
					if (sotnApi.AlucardApi.Con > 1)
					{
						sotnApi.AlucardApi.Con -= conTaken;
					}
					if (sotnApi.AlucardApi.Int > 1)
					{
						sotnApi.AlucardApi.Int -= intTaken;
					}
					if (sotnApi.AlucardApi.Lck > 1)
					{
						sotnApi.AlucardApi.Lck -= lckTaken;
					}
					name += "(Dmg)";
					break;
				case 3:
					leftHandTaken = sotnApi.AlucardApi.LeftHand;
					rightHandTaken = sotnApi.AlucardApi.RightHand;
					subWeaponTaken = (uint) sotnApi.AlucardApi.Subweapon;
					sotnApi.AlucardApi.RightHand = 0;
					sotnApi.AlucardApi.LeftHand = 0;
					sotnApi.AlucardApi.Subweapon = 0;
					name += "(Wep)";
					break;
				case 4:
					name += "(Slam Jam)";
					slamJamUser = user;
					slamJamTickTimer.Start();
					break;
				default:
					break;

			}
			message += name;
			System.TimeSpan newDuration = EvilDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Hex).FirstOrDefault().Duration);

			hexTimer.Interval = newDuration.TotalMilliseconds;
			hexTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Debuff,
				Duration = newDuration
			});

			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Hex);
		}

		private void SlamJam(Object sender, EventArgs e)
		{
			++slamCount;
			queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Slam, Invoker = new MethodInvoker(() => Slam(slamJamUser, true)) });
			int min = 15000 - (vladRelicsObtained * 500);
			int max = 22500 - (vladRelicsObtained * 750);
			int newInterval = rng.Next(min, max);
			slamJamTickTimer.Interval = newInterval;
		}
		private void SlamJamOff()
		{
			++slamCount;
			queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Slam, Invoker = new MethodInvoker(() => Slam(slamJamUser, true)) });
			Console.WriteLine($"User was slammed: {slamCount} times");
			slamCount = 0;
			slamJamTickTimer.Interval = 300;
			slamJamTickTimer.Stop();
		}

		private void HexOff(Object sender, EventArgs e)
		{
			//SetSpeed();
			if (hpTaken>0||mpTaken>0||heartsTaken>0)
			{
				sotnApi.AlucardApi.MaxtHp += hpTaken;
				sotnApi.AlucardApi.MaxtMp += mpTaken;
				sotnApi.AlucardApi.MaxtHearts += heartsTaken;
				hpTaken = 0;
				mpTaken = 0;
				heartsTaken = 0;
			}
			if (strTaken>0|| conTaken>0 || intTaken>0 || lckTaken>0)
			{
				sotnApi.AlucardApi.Str += strTaken;
				sotnApi.AlucardApi.Con += conTaken;
				sotnApi.AlucardApi.Int += intTaken;
				sotnApi.AlucardApi.Lck += lckTaken;
				strTaken = 0;
				conTaken = 0;
				intTaken = 0;
				lckTaken = 0;
			}
			if (leftHandTaken > 0 || rightHandTaken > 0 || subWeaponTaken > 0)
			{
				
				string item = Equipment.Items[(int)(leftHandTaken)];
				alucardApi.GrantItemByName(item);
				if (!Constants.Khaos.TwoHandedWeapons.Contains(Equipment.Items[(int) rightHandTaken]))
				{
					item = Equipment.Items[(int) (rightHandTaken)];
					alucardApi.GrantItemByName(item);
				}
				if(sotnApi.AlucardApi.Subweapon == 0 && !heartsLocked && !subWeaponsLocked && subWeaponTaken != 0)
				{
					sotnApi.AlucardApi.Subweapon = (Subweapon) subWeaponTaken;
				}
				leftHandTaken = 0;
				rightHandTaken = 0;
				subWeaponTaken = 0;
			}
			if(slamJamUser != "")
			{
				SlamJamOff();
			}
			else
			{
				slamCount = 0;
				slamJamTickTimer.Interval = 300;
				slamJamTickTimer.Stop();
			}
			hexTimer.Stop();
		}

		public void GetJuggled(string user = "Mayhem")
		{
			invincibilityCheat.PokeValue(0);
			invincibilityCheat.Enable();
			defensePotionCheat.PokeValue(1);
			defensePotionCheat.Enable();
			invincibilityLocked = true;
			
			System.TimeSpan newDuration = EvilDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.GetJuggled).FirstOrDefault().Duration);

			getJuggledTimer.Interval = newDuration.TotalMilliseconds;
			getJuggledTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.GetJuggled,
				Type = Enums.ActionType.Debuff,
				Duration = newDuration
			});
			notificationService.AddMessage($"{user} says Get Juggled");
			Alert(KhaosActionNames.GetJuggled);
		}
		private void getJuggledOff(object sender, EventArgs e)
		{
			invincibilityCheat.Disable();
			defensePotionCheat.Disable();
			getJuggledTimer.Stop();
			invincibilityLocked = false;
		}
		public void Ambush(string user = "Mayhem")
		{
			spawnActive = true;
			ambushTriggerRoomX = gameApi.MapXPos;
			ambushTriggerRoomY = gameApi.MapYPos;

			bool meterFull = KhaosMeterFull();
			string name = KhaosActionNames.Ambush;

			if (meterFull)
			{
				name = "Super " + name;
				superAmbush = true;
				SpendKhaosMeter();
			}

			ambushTimer.Start();
			ambushSpawnTimer.Start();
			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Ambush);
		}
		private void AmbushOff(Object sender, EventArgs e)
		{
			superAmbush = false;
			spawnActive = false;
			ambushEnemies.RemoveRange(0, ambushEnemies.Count);
			ambushTimer.Interval = 5 * (60 * 1000);
			ambushTimer.Stop();
			ambushSpawnTimer.Stop();
		}
		private void AmbushSpawn(Object sender, EventArgs e)
		{
			uint mapX = alucardApi.MapX;
			uint mapY = alucardApi.MapY;
			bool keepRichterRoom = ((mapX >= 31 && mapX <= 34) && mapY == 8);
			string name = KhaosActionNames.Ambush;

			if (superAmbush)
			{
				name = "Super " + name;
			}

			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5 || gameApi.CanSave() || keepRichterRoom)
			{
				return;
			}

			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (ambushZone != zone || ambushZone2 != zone2 || ambushEnemies.Count == 0)
			{
				ambushEnemies.RemoveRange(0, ambushEnemies.Count);
				FindAmbushEnemy();
				ambushZone = zone;
				ambushZone2 = zone2;
			}
			else if (ambushEnemies.Count > 0)
			{
				FindAmbushEnemy();
				int enemyIndex = rng.Next(0, ambushEnemies.Count);
				if (ambushTimer.Interval == 5 * (60 * 1000))
				{
					ambushTimer.Stop();
					System.TimeSpan newDuration = EvilDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Ambush).FirstOrDefault().Duration);
					ambushTimer.Interval = newDuration.TotalMilliseconds;

					notificationService.AddTimer(new Services.Models.ActionTimer
					{
						Name = name,
						Type = Enums.ActionType.Debuff,
						Duration = newDuration
					});
					ambushTimer.Start();
				}
				ambushEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				ambushEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				ambushEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				actorApi.SpawnActor(ambushEnemies[enemyIndex]);
			}
		}
		private bool FindAmbushEnemy()
		{
			uint roomX = gameApi.MapXPos;
			uint roomY = gameApi.MapYPos;
			bool boostDmg = false;

			if ((roomX == ambushTriggerRoomX && roomY == ambushTriggerRoomY) || !gameApi.InAlucardMode() || !gameApi.CanMenu())
			{
				return false;
			}

			long enemy = actorApi.FindActorFrom(Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? ambushEnemy = new Actor(actorApi.GetActor(enemy));

				if (ambushEnemy is not null && ambushEnemies.Where(e => e.Sprite == ambushEnemy.Sprite).Count() < 1)
				{
					float statMultiplier = 1.0F + (0.025F * vladRelicsObtained);
					if (superAmbush)
					{
						statMultiplier += .5F;

						bool alucardIsImmuneToCurse = sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad) || Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Coral circlet";
						bool alucardIsImmuneToStone = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Mirror cuirass"
							|| Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Medusa shield"
							|| Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Medusa shield";
						bool alucardIsImmuneToPoison = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Topaz circlet";

						List<int> effectNumbers = new List<int>() {1, 2, 3, 4};
						int min = 1;
						int max = effectNumbers.Count;
						int initMax = effectNumbers.Count;

						if (alucardIsImmuneToStone)
						{
							--max;
							effectNumbers.RemoveAll(item => item == 1);
						}
						if (alucardIsImmuneToCurse)
						{
							--max;
							effectNumbers.RemoveAll(item => item == 2);
						}
						if (alucardIsImmuneToPoison)
						{
							--max;
							effectNumbers.RemoveAll(item => item == 3);
						}
						if(initMax == max) 
						{
							--max;
							effectNumbers.RemoveAll(item => item == 4);
						}

						int damageTypeRoll = effectNumbers[rng.Next(min, max)];

						switch (damageTypeRoll)
						{
							case 1:
								ambushEnemy.DamageTypeA = (uint) Actors.Poison;
								break;
							case 2:
								ambushEnemy.DamageTypeB = (uint) Actors.Curse;
								break;
							case 3:
								ambushEnemy.DamageTypeA = (uint) Actors.Stone;
								ambushEnemy.DamageTypeB = (uint) Actors.Stone;
								break;
							case 4:
								boostDmg = true;
								break;
							default:
								break;
						}
					}
					ambushEnemy.Hp = (ushort) (ambushEnemy.Hp * statMultiplier);
					if (boostDmg)
					{
						statMultiplier += .25F;
					}
					statMultiplier += (0.025F * vladRelicsObtained);
					ambushEnemy.Damage = (ushort) (ambushEnemy.Damage * statMultiplier);

					ambushEnemies.Add(ambushEnemy);
					Console.WriteLine($"Added ambush enemy with hp: {ambushEnemy.Hp} sprite: {ambushEnemy.Sprite} damage: {ambushEnemy.Damage}");
					return true;
				}
			}

			return false;
		}
		public void ToughBosses(string user = "Mayhem")
		{
			toughBossesRoomX = gameApi.MapXPos;
			toughBossesRoomY = gameApi.MapYPos;
			bool meterFull = KhaosMeterFull();
			if (meterFull)
			{
				superToughBosses = true;
				SpendKhaosMeter();
			}
			Console.WriteLine($"toughBossesRoomX{toughBossesRoomX},toughbossesRoomY{toughBossesRoomY},superToughBosses{superToughBosses}");
			
			toughBossesCount++;
			toughBossesSpawnTimer.Start();
			gameApi.RespawnBosses();

			string message = meterFull ? $"{user} used Super {KhaosActionNames.ToughBosses}" : $"{user} used {KhaosActionNames.ToughBosses}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.ToughBosses);
		}
		private void ToughBossesSpawn(Object sender, EventArgs e)
		{
			uint roomX = gameApi.MapXPos;
			uint roomY = gameApi.MapYPos;
			float healthMultiplier = 2.0F + (0.1F * vladRelicsObtained);

			if ((roomX == toughBossesRoomX && roomY == toughBossesRoomY) || !gameApi.InAlucardMode() || !gameApi.CanMenu() || alucardApi.CurrentHp < 5)
			{
				return;
			}

			Actor? bossCopy = null;

			long enemy = sotnApi.ActorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceRomhackBosses : Constants.Khaos.EnduranceBosses);

			if (enemy > 0)
			{
				LiveActor boss = sotnApi.ActorApi.GetLiveActor(enemy);
				bossCopy = new Actor(sotnApi.ActorApi.GetActor(enemy));
				string name = Constants.Khaos.EnduranceRomhackBosses.Where(e => e.Sprite == bossCopy.Sprite).FirstOrDefault().Name;
				Console.WriteLine($"{KhaosActionNames.ToughBosses} boss found name: {name} hp: {bossCopy.Hp}, damage: {bossCopy.Damage}, sprite: {bossCopy.Sprite}, health multiplier: {healthMultiplier}");

				bool right = rng.Next(0, 2) > 0;
				bossCopy.Xpos = right ? (ushort) (bossCopy.Xpos + rng.Next(40, 80)) : (ushort) (bossCopy.Xpos + rng.Next(-80, -40));
				bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
				bossCopy.Hp = (ushort) Math.Round(healthMultiplier * bossCopy.Hp);
				sotnApi.ActorApi.SpawnActor(bossCopy);

				boss.Hp = (ushort) Math.Round(healthMultiplier * boss.Hp);

				if (superToughBosses)
				{
					superToughBosses = false;

					bossCopy.Xpos = rng.Next(0, 2) == 1 ? (ushort) (bossCopy.Xpos + rng.Next(-80, -20)) : (ushort) (bossCopy.Xpos + rng.Next(20, 80));
					bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
					sotnApi.ActorApi.SpawnActor(bossCopy);
					notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} - {name}");
					//notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} - {name} - {boss.Hp}HP");
				}
				else
				{
					notificationService.AddMessage($"{KhaosActionNames.ToughBosses} - {name}");
					//notificationService.AddMessage($"{KhaosActionNames.ToughBosses} - {name} - {boss.Hp}HP");
				}

				toughBossesCount--;
				toughBossesRoomX = roomX;
				toughBossesRoomY = roomY;
				if (toughBossesCount == 0)
				{
					toughBossesSpawnTimer.Stop();
				}
			}
			else
			{
				enemy = actorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceAlternateRomhackBosses : Constants.Khaos.EnduranceAlternateBosses);
				if (enemy > 0)
				{
					LiveActor boss = actorApi.GetLiveActor(enemy);
					string name = Constants.Khaos.EnduranceAlternateBosses.Where(e => e.Sprite == boss.Sprite).FirstOrDefault().Name;
					Console.WriteLine($"Endurance alternate boss found name: {name}, healthMultiplier {healthMultiplier}");

					boss.Palette = (ushort) (boss.Palette + rng.Next(1, 10));

					if (superToughBosses)
					{
						boss.Hp = (ushort) Math.Round((healthMultiplier * 2.3) * boss.Hp);
						superEndurance = false;
						notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} - {name}");
						//notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} - {name} - {boss.Hp}HP");
					}
					else
					{
						boss.Hp = (ushort) Math.Round((healthMultiplier * 1.3) * boss.Hp);
						notificationService.AddMessage($"{KhaosActionNames.ToughBosses} - {name}");
						//notificationService.AddMessage($"{KhaosActionNames.ToughBosses} - {name} - {boss.Hp}HP");
					}

					toughBossesCount--;
					toughBossesRoomX = roomX;
					toughBossesRoomY = roomY;
					if (toughBossesCount == 0)
					{
						toughBossesSpawnTimer.Stop();
					}
				}
				else
				{
					return;
				}
			}
		}

		public void StatsDown(string user = "Mayhem")
		{
			bool meterFull = KhaosMeterFull();
			float enhancedFactor = 1;

			string name = KhaosActionNames.StatsDown;

			if (meterFull)
			{
				name = "Super " + name;
				enhancedFactor = Constants.Khaos.SuperStatsDownFactor;
				SpendKhaosMeter();
			}

			//Ensure hex removed stats or temporary granted stats are also nerfed.
			hpGiven = (uint) (hpGiven * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			hpTaken = (uint) (hpTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			strTaken = (uint) (strTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			conTaken = (uint) (conTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			intTaken = (uint) (intTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			lckTaken = (uint) (lckTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);

			uint newCurrentHp = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newCurrentMp = (uint) (alucardApi.CurrentHp * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			//uint newCurrentHearts = (uint) (alucardApi.CurrentHearts * toolConfig.Khaos.StatsDownFactor * enhancedFactor);

			uint newMaxHearts = (uint) (alucardApi.MaxtHearts * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newMaxHp = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newMaxMp = (uint) (alucardApi.MaxtHp * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newStr = (uint) (alucardApi.Str * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newCon = (uint) (alucardApi.Con * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newInt = (uint) (alucardApi.Int * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newLck = (uint) (alucardApi.Lck * toolConfig.Khaos.StatsDownFactor * enhancedFactor);

			//Zig- Enforce minimum max and current stats
			alucardApi.CurrentHp = newCurrentHp < 1 ? 1 : newMaxHp;
			if (!manaLocked)
			{
				alucardApi.CurrentMp = newCurrentMp < 1 ? 1 : newMaxMp;
			}

			alucardApi.MaxtHp = newMaxHp < minHP ? minHP : newMaxHp;
			alucardApi.MaxtMp = newMaxMp < minMP ? minMP : newMaxMp;
			alucardApi.MaxtHearts = newMaxHearts < minHearts ? minHearts : newMaxHearts;

			if (!heartsLocked == true)
			{
				alucardApi.CurrentHearts = newMaxHearts;
			}

			alucardApi.Str = newStr < minStr ? minStr : newStr;
			alucardApi.Con = newCon < minCon ? minCon : newCon;
			alucardApi.Int = newInt < minInt ? minInt : newInt;
			alucardApi.Lck = newLck < minLck ? minLck : newLck;

			uint newLevel = (uint) (alucardApi.Level * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			//alucardApi.Level = newLevel > 0 ? newLevel : 1;

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

			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.StatsDown);
		}
		public void Confiscate(string user = "Mayhem")
		{
			ConfiscateActivate();
			notificationService.AddMessage($"{user} used {KhaosActionNames.Confiscate}");
			Alert(KhaosActionNames.Confiscate);
		}
		private void ConfiscateActivate()
		{

			bool clearRightHand = false;
			bool clearLeftHand = false;
			bool clearHelm = false;
			bool clearArmor = false;
			bool clearCloak = false;
			bool clearAccessory1 = false;
			bool clearAccessory2 = false;

			float goldPercentage = 0;
			int clearedSlots = 0;

			switch (vladRelicsObtained)
			{
				case 1:
					goldPercentage = 0.4f;
					clearedSlots = 3;
					break;
				case 2:
					goldPercentage = 0.3f;
					clearedSlots = 4;
					break;
				case 3:
					goldPercentage = 0.2f;
					clearedSlots = 5;
					break;
				case 4:
					goldPercentage = 0.1f;
					clearedSlots = 6;
					break;
				case 5:
					goldPercentage = 0;
					clearedSlots = 7;
					break;
				default:
					goldPercentage = 0.5f;
					clearedSlots = 2;
					break;
			}

			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring");
			bool equippedHolyGlasses = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";


			sotnApi.AlucardApi.Gold = goldPercentage == 0 ? 0 : (uint) Math.Round(sotnApi.AlucardApi.Gold * goldPercentage);
			sotnApi.AlucardApi.ClearInventory();

			if (clearedSlots == 7)
			{
				clearRightHand = true;
				clearLeftHand = true;
				clearHelm = true;
				clearArmor = true;
				clearCloak = true;
				clearAccessory1 = true;
				clearAccessory2 = true;
			}
			else
			{
				int[] slots = new int[clearedSlots + 1];
				int slotsIndex = 0;
				for (int i = 0; i <= clearedSlots; i++)
				{
					int result = rng.Next(0, 8);
					while (slots.Contains(result))
					{
						result = rng.Next(0, 8);
					}
					slots[slotsIndex] = result;
					slotsIndex++;
				}

				for (int i = 0; i < slots.Length; i++)
				{
					switch (slots[i])
					{
						case 1:
							clearRightHand = true;
							break;
						case 2:
							clearLeftHand = true;
							break;
						case 3:
							clearHelm = true;
							break;
						case 4:
							clearArmor = true;
							break;
						case 5:
							clearCloak = true;
							break;
						case 6:
							clearAccessory1 = true;
							break;
						case 7:
							clearAccessory1 = true;
							break;
						default:
							break;
					}
				}
			}

			if (clearRightHand)
			{
				sotnApi.AlucardApi.RightHand = 0;
			}
			if (clearLeftHand)
			{
				sotnApi.AlucardApi.LeftHand = 0;
			}
			if (!equippedHolyGlasses && clearHelm)
			{
				sotnApi.AlucardApi.Helm = Equipment.HelmStart;
			}
			if (!equippedSpikeBreaker && clearArmor)
			{
				sotnApi.AlucardApi.Armor = 0;
			}
			if (clearCloak)
			{
				sotnApi.AlucardApi.Cloak = Equipment.CloakStart;
			}
			if (clearAccessory1)
			{
				sotnApi.AlucardApi.Accessory1 = Equipment.AccessoryStart;
			}
			if (clearAccessory2)
			{
				sotnApi.AlucardApi.Accessory2 = Equipment.AccessoryStart;
			}

			sotnApi.GameApi.RespawnItems();

			sotnApi.AlucardApi.HandCursor = 0;
			sotnApi.AlucardApi.HelmCursor = 0;
			sotnApi.AlucardApi.ArmorCursor = 0;
			sotnApi.AlucardApi.CloakCursor = 0;
			sotnApi.AlucardApi.AccessoryCursor = 0;

			if (hasHolyGlasses)
			{
				sotnApi.AlucardApi.GrantItemByName("Holy glasses");
			}
			if (hasSpikeBreaker)
			{
				sotnApi.AlucardApi.GrantItemByName("Spike Breaker");
			}
			if (equippedGoldRing || hasGoldRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Gold Ring");
			}
			if (equippedSilverRing || hasSilverRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Silver Ring");
			}
		}
		#endregion
		#region Positive Effects
		public System.TimeSpan GoodDurationGain(System.TimeSpan baseTimeSpan)
		{
			System.TimeSpan newTimeSpan = baseTimeSpan;
			if (hasHolyGlasses)
			{
				int newDurationSeconds = (int) Math.Floor(baseTimeSpan.TotalSeconds * (1.5));
				int diffInSeconds = (int) Math.Floor(newDurationSeconds - baseTimeSpan.TotalSeconds);
				int diffInMinutes = (int) Math.Floor(diffInSeconds / 60.0);
				diffInSeconds = diffInSeconds % 60;
				TimeSpan newInterval = new TimeSpan(0, diffInMinutes, diffInSeconds);
				newTimeSpan = baseTimeSpan.Add(newInterval);

				Console.WriteLine($"Good Duration: baseTimeSpanTotal{baseTimeSpan.TotalMilliseconds}, baseTimeSpan{baseTimeSpan}, newDur {newDurationSeconds}, diffInSec {diffInSeconds}, diffInMinutes{diffInMinutes}, newTimeSpan{newTimeSpan}, newTimeSpanTotal{newTimeSpan.TotalMilliseconds}");
			}

			return newTimeSpan;
		}

		public void MinorBoon(string user = "Mayhem")
		{
			Relic relic;
			
			int roll;
			uint addHP = 0;
			uint addMP = 0;
			uint addHearts = 0;

			RollRewards(out relic, out addHP, out addMP, out addHearts, out roll);
			GiveRewards(user, relic, addHP, addMP, addHearts, roll);

			void RollRewards(out Relic relic, out uint addHP, out uint addMP, out uint addHearts, out int roll)
			{
				uint baseGain = 5 + alucardApi.Level;
				
				
				addHP = baseGain + alucardApi.Level;
				addMP = baseGain;
				addHearts = baseGain;
				roll = 0;

				roll = rng.Next(1, 4);

				int relicIndex = rng.Next(0, Constants.Khaos.MinorBoonRelics.Length);
				for (int i = 0; i < 11; i++)
				{
					if (!alucardApi.HasRelic(Constants.Khaos.MinorBoonRelics[relicIndex]))
					{
						break;
					}
					else if (i == 10)
					{
						roll = 1;
						break;
					}
					relicIndex = rng.Next(0, Constants.Khaos.MinorBoonRelics.Length);
				}
				relic = Constants.Khaos.MinorBoonRelics[relicIndex];
				Console.WriteLine($"Minor boon rolled:{roll}, rel {relic}");
			}

			void GiveRewards(string user, Relic relic, uint addHP, uint addMP, uint addHearts, int roll)
			{
				uint currentHp = alucardApi.CurrentHp;
				uint currentMp = alucardApi.CurrentMp;
				uint currentHearts = alucardApi.CurrentHearts;

				if (hasHolyGlasses)
				{
					if (manaLocked)
					{
						addHP += addMP;
					}
					else if ((currentMp + addMP) > (currentMp * 1.5))
					{
						alucardApi.CurrentMp = alucardApi.MaxtHp;
					}
					else
					{
						alucardApi.CurrentMp += addMP;
					}
					if (heartsLocked)
					{
						addHP += addHearts;
					}
					else if ((currentHearts + addHearts) > (alucardApi.MaxtHearts * 1.5))
					{
						alucardApi.CurrentHearts += addHearts;
					}
					if ((currentHp + addHP) > (alucardApi.MaxtHp * 1.5))
					{
						alucardApi.CurrentHp = (uint) (currentHp * 1.5);
					}
					else
					{
						alucardApi.CurrentHp += addHP;
					}
				}
				if(roll > 2)
				{
					roll = 2;
				}
				switch (roll)
				{
					case 1:
						Console.WriteLine($"Minor boon rolled: {roll}");
						RandomizePotion(user);
						break;
					case 2:
						alucardApi.GrantRelic(relic);
						Console.WriteLine($"Minor Boon rolled {relic}");
						notificationService.AddMessage($"{user} granted {relic}");
						break;
					default:
						break;
				}
				Alert(KhaosActionNames.MinorBoon);
			}
		}

		public void Speed(string user = "Mayhem")
		{
			speedActive = true;
			speedLocked = true;

			bool meterFull = KhaosMeterFull();
			string name = KhaosActionNames.Speed;

			if (meterFull)
			{
				name = "Super " + name;
				SpendKhaosMeter();
				superSpeed = true;
			}

			SetHasteStaticSpeeds(meterFull);

			System.TimeSpan newDuration = GoodDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Speed).FirstOrDefault().Duration);

			speedTimer.Interval = newDuration.TotalMilliseconds;
			speedTimer.Start();
			
			
			Console.WriteLine($"{user} used {KhaosActionNames.Speed}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Buff,
				Duration = newDuration
			});
			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Haste);
		}
		private void SpeedOff(Object sender, EventArgs e)
		{
			hasteTimer.Stop();
			SetSpeed();
			speedOverdriveOffTimer.Start();
			superSpeed = false;
			speedActive = false;
			speedLocked = false;
		}
		private void SpeedOverdriveOn(object sender, System.Timers.ElapsedEventArgs e)
		{
			visualEffectPaletteCheat.PokeValue(33126);
			visualEffectPaletteCheat.Enable();
			visualEffectTimerCheat.PokeValue(30);
			visualEffectTimerCheat.Enable();
			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * (toolConfig.Khaos.HasteFactor / 1.8));
			overdriveOn = true;
			speedOverdriveTimer.Stop();
		}
		private void SpeedOverdriveOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			visualEffectPaletteCheat.Disable();
			visualEffectTimerCheat.Disable();
			if (speedActive)
			{
				SetHasteStaticSpeeds(superSpeed);
			}
			else
			{
				sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal);
			}
			overdriveOn = false;
			speedOverdriveOffTimer.Stop();
		}
		public void Regen(string user = "Mayhem")
		{
			bool meterFull = KhaosMeterFull();
			string name = KhaosActionNames.Regen;
			if (meterFull)
			{
				name = "Super " + name;
				superRegen = true;
				SpendKhaosMeter();
			}

			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Enable();

			System.TimeSpan newDuration = GoodDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Regen).FirstOrDefault().Duration);

			regenTimer.Interval = newDuration.TotalMilliseconds;
			regenTimer.Start();
			regenTickTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Buff,
				Duration = newDuration 
			});

			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Regen);
		}
		private void RegenGain(Object sender, EventArgs e)
		{
			uint superGain = superRegen ? Constants.Khaos.SuperThirstExtraDrain : 0u;
			if (alucardApi.CurrentHp < (alucardApi.MaxtHp * 2))
			{
				sotnApi.AlucardApi.CurrentHp += (toolConfig.Khaos.RegenGainPerSecond + superGain);
			}
			if (!manaLocked && sotnApi.AlucardApi.CurrentMp < sotnApi.AlucardApi.MaxtMp)
			{
				sotnApi.AlucardApi.CurrentMp += (toolConfig.Khaos.RegenGainPerSecond + superGain);
			}
			if (!heartsLocked && sotnApi.AlucardApi.CurrentHearts < (2*sotnApi.AlucardApi.MaxtHearts))
			{
				sotnApi.AlucardApi.CurrentHearts += (toolConfig.Khaos.RegenGainPerSecond + superGain);
			}

		}
		private void RegenOff(Object sender, EventArgs e)
		{
			darkMetamorphasisCheat.Disable();
			regenTimer.Stop();
			regenTickTimer.Stop();
			superRegen = false;
		}

		public void ModerateBoon(string user = "Mayhem")
		{
			Relic relic;

			int roll;
			string item;

			RollRewards(out relic, out item, out roll);
			GiveRewards(user, relic, item, roll);

			void RollRewards(out Relic relic, out string item, out int roll)
			{
				item = toolConfig.Khaos.ModerateBoonItemRewards[rng.Next(0, toolConfig.Khaos.ModerateBoonItemRewards.Length)];
				bool needsRelic = true;
				relic = Relic.FireOfBat;
				roll = 0;

				while (alucardApi.HasItemInInventory(item) && roll < Constants.Khaos.HelpItemRetryCount)
				{
					item = toolConfig.Khaos.ModerateBoonItemRewards[rng.Next(0, toolConfig.Khaos.ModerateBoonItemRewards.Length)];
					roll++;
				}

				roll = rng.Next(1, 4);

				bool hasSoulOfWolf = alucardApi.HasRelic(Relic.SoulOfWolf) ? true : false;
				bool hasPowerOfWolf = alucardApi.HasRelic(Relic.PowerOfWolf) ? true : false;
				bool hasSkillOfWolf = alucardApi.HasRelic(Relic.SkillOfWolf) ? true : false;
				bool hasFormOfMist = alucardApi.HasRelic(Relic.FormOfMist) ? true : false;
				bool hasGasCloud = alucardApi.HasRelic(Relic.GasCloud) ? true : false;
				bool hasSoulOfBat = alucardApi.HasRelic(Relic.SoulOfBat) ? true : false;
				bool hasEchoOfBat = alucardApi.HasRelic(Relic.EchoOfBat) ? true : false;
				bool hasGravityBoots = alucardApi.HasRelic(Relic.GravityBoots) ? true : false;
				bool hasLeapStone = alucardApi.HasRelic(Relic.LeapStone) ? true : false;

				if (needsRelic && hasSoulOfWolf)
				{
					if (!hasPowerOfWolf)
					{
						relic = Relic.PowerOfWolf;
						needsRelic = false;
					}
					else if (!hasSkillOfWolf)
					{
						relic = Relic.SkillOfWolf;
						needsRelic = false;
					}
				}
				if (needsRelic && hasFormOfMist)
				{
					if (!hasGasCloud)
					{
						relic = Relic.GasCloud;
						needsRelic = false;
					}
				}
				if (needsRelic && hasGravityBoots)
					{
					if (!hasLeapStone)
					{
						relic = Relic.LeapStone;
						needsRelic = false;
					}
				}
				if (needsRelic)
				{
					int relicIndex = rng.Next(0, Constants.Khaos.ModerateBoonRelics.Length);
					for (int i = 0; i < 11; i++)
					{
						if (!alucardApi.HasRelic(Constants.Khaos.ModerateBoonRelics[relicIndex]))
						{
							break;
						}
						else if (i == 10)
						{
							roll = 1;
							if (hasHolyGlasses)
							{
								//Do nothing
							}
							else
							{
								item = "Holy glasses";
							}
							break;
						}
						relicIndex = rng.Next(0, Constants.Khaos.ModerateBoonRelics.Length);
					}
					relic = Constants.Khaos.ModerateBoonRelics[relicIndex];
				}
				Console.WriteLine($"Moderate boon rolled:{roll}, rel {relic}, item {item}");
			}

			void GiveRewards(string user, Relic relic, string item, int roll)
			{
				if (hasHolyGlasses)
				{
					RandomizeModeratePotion(user);
				}

				if(roll > 2)
				{
					roll = 2;
				}
				switch (roll)
				{
					case 1:
						Console.WriteLine($"Moderate boon rolled: {item}");
						alucardApi.GrantItemByName(item);
						notificationService.AddMessage($"{user} gave you a {item}");
						break;
					case 2:
						alucardApi.GrantRelic(relic);
						Console.WriteLine($"Moderate boon rolled {relic}");
						notificationService.AddMessage($"{user} granted {relic}");
						break;
					default:
						break;
				}
				Alert(KhaosActionNames.ModerateBoon);
			}
		}
		public void TimeStop(string user = "Mayhem")
		{
			alucardApi.ActivateStopwatch();
			timeStopZone = gameApi.Zone;
			timeStopZone2 = gameApi.Zone2;
			timeStopActive = true;

			if (!subweaponsOnlyActive && !heartsOnlyActive)
			{
				alucardApi.Subweapon = Subweapon.Stopwatch;
			}
			stopwatchTimer.Enable();

			System.TimeSpan newDuration = GoodDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TimeStop).FirstOrDefault().Duration);
			timeStopTimer.Interval = newDuration.TotalMilliseconds;
			timeStopTimer.Start();
			timeStopCheckTimer.Start();

			notificationService.AddMessage($"{user} used {KhaosActionNames.TimeStop}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.TimeStop,
				Type = Enums.ActionType.Buff,
				Duration = newDuration
			});
			Alert(KhaosActionNames.TimeStop);
		}
		private void TimeStopOff(Object sender, EventArgs e)
		{
			stopwatchTimer.Disable();
			timeStopTimer.Stop();
			timeStopCheckTimer.Stop();
			timeStopActive = false;
		}
		private void TimeStopAreaCheck(Object sender, EventArgs e)
		{
			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (timeStopZone != zone || timeStopZone2 != zone2)
			{
				timeStopZone = zone;
				timeStopZone2 = zone2;
				alucardApi.ActivateStopwatch();
			}
		}
		public void FaceTank(string user = "Mayhem")
		{
			bool meterFull = KhaosMeterFull();
			string name = KhaosActionNames.FaceTank;

			float currentHpPercentage = (float)((1.00*(sotnApi.AlucardApi.CurrentHp)) / sotnApi.AlucardApi.MaxtHp);

			if (currentHpPercentage < 1)
			{
				currentHpPercentage = 1;
			}

			hpGiven = (uint) ((alucardApi.MaxtHp * Constants.Khaos.FaceTankHpMultiplier) - sotnApi.AlucardApi.MaxtHp);
			sotnApi.AlucardApi.MaxtHp += hpGiven;

			if (meterFull) {
				name = "Super " + name;
				SpendKhaosMeter();
				alucardApi.ActivatePotion(Potion.ShieldPotion);
				alucardApi.ActivatePotion(Potion.ResistFire);
				alucardApi.ActivatePotion(Potion.ResistIce);
				alucardApi.ActivatePotion(Potion.ResistThunder);
				alucardApi.ActivatePotion(Potion.ResistHoly);
				alucardApi.ActivatePotion(Potion.ResistDark);
			}

			sotnApi.AlucardApi.CurrentHp = (uint)(sotnApi.AlucardApi.MaxtHp * currentHpPercentage);

			System.TimeSpan newDuration = GoodDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FaceTank).FirstOrDefault().Duration);

			faceTankTimer.Interval = newDuration.TotalMilliseconds;
			faceTankTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Buff,
				Duration = newDuration
			});

			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.FaceTank);
		}

		private void FaceTankOff(Object sender, EventArgs e)
		{
			if(sotnApi.AlucardApi.MaxtHp-hpGiven < minHP)
			{
				sotnApi.AlucardApi.MaxtHp = minHP;
			}
			else
			{
				sotnApi.AlucardApi.MaxtHp -= hpGiven;
			}
			hpGiven = 0;
			faceTankTimer.Stop();
		}
		public void SpellCaster(string user = "Mayhem")
		{
			bool meterFull = KhaosMeterFull();
			string name = KhaosActionNames.SpellCaster;

			if (meterFull)
			{
				name = "Super " + name;
				SpendKhaosMeter();
				if (hasHolyGlasses)
				{
					alucardApi.GrantRelic(Relic.SoulOfBat);
					alucardApi.GrantRelic(Relic.EchoOfBat);
					alucardApi.GrantRelic(Relic.ForceOfEcho);
				}
				alucardApi.GrantRelic(Relic.SoulOfWolf);
				alucardApi.GrantRelic(Relic.FormOfMist);
			}

			alucardApi.ActivatePotion(Potion.SmartPotion);
			Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(99);
			manaCheat.Enable();
			manaLocked = true;

			System.TimeSpan newDuration = GoodDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SpellCaster).FirstOrDefault().Duration);
			
			spellcasterTimer.Interval = newDuration.TotalMilliseconds;
			spellcasterTimer.Start();
			
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Buff,
				Duration = newDuration
			});

			string message = $"{user} used {name}";
			notificationService.AddMessage(message);

			Alert(KhaosActionNames.SpellCaster);
		}

		private void SpellcasterOff(Object sender, EventArgs e)
		{
			//Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.Disable();
			manaLocked = false;
			spellcasterTimer.Stop();
		}
		public void ExtraRange(string user = "Mayhem")
		{
			bool meterFull = KhaosMeterFull();
			string name = KhaosActionNames.ExtraRange;

			if (meterFull)
			{
				name = "Super " + name;
				superExtraRange = true;
				alucardApi.ActivatePotion(Potion.StrPotion);
				alucardApi.AttackPotionTimer = Constants.Khaos.GuiltyGearAttack;
				alucardApi.DarkMetamorphasisTimer = Constants.Khaos.GuiltyGearDarkMetamorphosis;
				alucardApi.DefencePotionTimer = Constants.Khaos.GuiltyGearDefence;
				SpendKhaosMeter();
			}

			hitboxWidth.Enable();
			hitboxHeight.Enable();
			hitbox2Width.Enable();
			hitbox2Height.Enable();

			System.TimeSpan newDuration = GoodDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ExtraRange).FirstOrDefault().Duration);

			extraRangeTimer.Interval = newDuration.TotalMilliseconds;
			extraRangeTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Buff,
				Duration = newDuration
			});

			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.ExtraRange);
			//if (meterFull)
			//{
			//Alert(KhaosActionNames.GuiltyGear);
			//}
			//else
			//{
			//Alert(KhaosActionNames.ExtraRange);
			//}
		}
		private void ExtraRangeOff(Object sender, EventArgs e)
		{
			hitboxWidth.Disable();
			hitboxHeight.Disable();
			hitbox2Width.Disable();
			hitbox2Height.Disable();

			if (superExtraRange)
			{
				superExtraRange = false;
				SetSpeed();
			}

			extraRangeTimer.Stop();
		}
		public void Summoner(string user = "Mayhem")
		{
			spawnActive = true;
			summonerTriggerRoomX = gameApi.MapXPos;
			summonerTriggerRoomY = gameApi.MapYPos;

			summonerTimer.Start();
			summonerSpawnTimer.Start();

			string message = $"{user} made you a {KhaosActionNames.Summoner}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Summoner);
		}
		private void SummonerOff(Object sender, EventArgs e)
		{
			spawnActive = false;
			summonerEnemies.RemoveRange(0, summonerEnemies.Count);
			summonerTimer.Interval = 5 * (60 * 1000);
			summonerTimer.Stop();
			summonerSpawnTimer.Stop();
		}
		private void SummonerSpawn(Object sender, EventArgs e)
		{
			if (!gameApi.InAlucardMode() || !gameApi.CanMenu() || gameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone))
			{
				return;
			}

			uint zone = gameApi.Zone;
			uint zone2 = gameApi.Zone2;

			if (summonerZone != zone || summonerZone2 != zone2 || summonerEnemies.Count == 0)
			{
				summonerEnemies.RemoveRange(0, summonerEnemies.Count);
				FindSummonerEnemy();
				summonerZone = zone;
				summonerZone2 = zone2;
			}
			else if (summonerEnemies.Count > 0)
			{
				FindSummonerEnemy();
				int enemyIndex = rng.Next(0, summonerEnemies.Count);
				if (summonerTimer.Interval == 5 * (60 * 1000))
				{

					summonerTimer.Stop();
					System.TimeSpan newDuration = GoodDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault().Duration);
					summonerTimer.Interval = newDuration.TotalMilliseconds;
					notificationService.AddTimer(new Services.Models.ActionTimer
					{
						Name = KhaosActionNames.Summoner,
						Type = Enums.ActionType.Buff,
						Duration = newDuration
					});
					summonerTimer.Start();

				}
				summonerEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				summonerEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				summonerEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				actorApi.SpawnActor(summonerEnemies[enemyIndex], false);
			}
		}
		private bool FindSummonerEnemy()
		{
			uint roomX = gameApi.MapXPos;
			uint roomY = gameApi.MapYPos;

			if ((roomX == summonerTriggerRoomX && roomY == summonerTriggerRoomY) || !gameApi.InAlucardMode() || !gameApi.CanMenu())
			{
				return false;
			}

			long enemy = actorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.AcceptedRomhackHordeEnemies : Constants.Khaos.AcceptedHordeEnemies);

			if (enemy > 0)
			{
				Actor? summonerEnemy = new Actor(sotnApi.ActorApi.GetActor(enemy));

				if (summonerEnemy is not null && summonerEnemies.Where(e => e.Sprite == summonerEnemy.Sprite).Count() < 1)
				{
					summonerEnemies.Add(summonerEnemy);
					Console.WriteLine($"Added {KhaosActionNames.Summoner} ally with hp: {summonerEnemy.Hp} sprite: {summonerEnemy.Sprite} damage: {summonerEnemy.Damage}");
					return true;
				}
			}

			return false;
		}
		public void MajorBoon(string user = "Mayhem")
		{
			{
				Relic relic;

				int roll;
				string item;

				RollRewards(out relic, out item, out roll);
				GiveRewards(user, relic, item, roll);


				void RollRewards(out Relic relic, out string item, out int roll)
				{
					item = toolConfig.Khaos.MajorBoonItemRewards[rng.Next(0, toolConfig.Khaos.MajorBoonItemRewards.Length)];
					bool needsRelic = true;
					relic = Relic.FireOfBat;
					roll = 0;

					while (alucardApi.HasItemInInventory(item) && roll < Constants.Khaos.HelpItemRetryCount)
					{
						item = toolConfig.Khaos.MajorBoonItemRewards[rng.Next(0, toolConfig.Khaos.MajorBoonItemRewards.Length)];
						roll++;
					}

					roll = rng.Next(1, 4);

					bool hasJewelOfOpen = alucardApi.HasRelic(Relic.JewelOfOpen) ? true : false;
					bool hasMermanStatue = alucardApi.HasRelic(Relic.MermanStatue) ? true : false;
					bool hasSoulOfWolf = alucardApi.HasRelic(Relic.SoulOfWolf) ? true : false;
					bool hasFormOfMist = alucardApi.HasRelic(Relic.FormOfMist) ? true : false;
					bool hasPowerOfMist = alucardApi.HasRelic(Relic.PowerOfMist) ? true : false;
					bool hasGravityBoots = alucardApi.HasRelic(Relic.GravityBoots) ? true : false;
					bool hasLeapStone = alucardApi.HasRelic(Relic.LeapStone) ? true : false;

					if (needsRelic && hasJewelOfOpen)
					{
						if (!hasMermanStatue)
						{
							relic = Relic.MermanStatue;
							needsRelic = false;
						}
					}
					if (needsRelic && hasMermanStatue)
					{
						if (!hasJewelOfOpen)
						{
							relic = Relic.JewelOfOpen;
							needsRelic = false;
						}
					}

					if (needsRelic && hasSoulOfWolf)
					{
						if (!hasGravityBoots)
						{
							relic = Relic.GravityBoots;
							needsRelic = false;
						}
					}
					if (needsRelic && hasGravityBoots)
					{
						if (!hasLeapStone)
						{
							relic = Relic.LeapStone;
							needsRelic = false;
						}
					}
					if (needsRelic && hasPowerOfMist)
					{
						if (!hasFormOfMist)
						{
							relic = Relic.FormOfMist;
							needsRelic = false;
						}
					}
					if (needsRelic && hasFormOfMist)
					{
						if (!hasPowerOfMist)
						{
							relic = Relic.PowerOfMist;
							needsRelic = false;
						}
					}
					if (needsRelic)
					{
						int relicIndex = rng.Next(0, Constants.Khaos.MajorBoonRelics.Length);
						for (int i = 0; i < 11; i++)
						{
							if (!alucardApi.HasRelic(Constants.Khaos.MajorBoonRelics[relicIndex]))
							{
								break;
							}
							else if (i == 10)
							{
								roll = 1;
								if (hasHolyGlasses)
								{
									//Do nothing
								}
								else
								{
									item = "Holy glasses";
								}
								break;
							}
							relicIndex = rng.Next(0, Constants.Khaos.MajorBoonRelics.Length);
						}
						relic = Constants.Khaos.MajorBoonRelics[relicIndex];
					}
					Console.WriteLine($"Major boon rolled:{roll}, rel {relic}, item {item}");
				}

				void GiveRewards(string user, Relic relic, string item, int roll)
				{

					if (hasHolyGlasses)
					{
						RandomizeMajorReward(user);
					}
					if (roll > 2)
					{
						roll = 2;
					}

					switch (roll)
					{
						case 1:
							Console.WriteLine($"Major boon rolled: {item}");
							alucardApi.GrantItemByName(item);
							notificationService.AddMessage($"{user} gave you a {item}");
							break;
						case 2:
							alucardApi.GrantRelic(relic);
							Console.WriteLine($"Major Boon rolled {relic}");
							notificationService.AddMessage($"{user} granted {relic}");
							break;
						default:
							break;
					}
					Alert(KhaosActionNames.ModerateBoon);
				}
			}
		}
		#endregion


		#region Legacy Khaotic Effects
		public void KhaosStatus(string user = "Mayhem")
		{
			//based on 1.16 balance changes.
			bool entranceCutscene = IsInRoomList(Constants.Khaos.EntranceCutsceneRooms);
			bool succubusRoom = IsInRoomList(Constants.Khaos.SuccubusRoom);
			int min = 1;
			int max = 9;

			if (timeStopActive)
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
		public void KhaosEquipment(string user = "Mayhem")
		{
			LegacyRandomizeGold();
			LegacyRandomizeEquipmentSlots();
			notificationService.AddMessage($"{user} used Khaos Equipment");
			Alert(KhaosActionNames.KhaosEquipment);
		}
		public void KhaosStats(string user = "Mayhem")
		{
			LegacyRandomizeStatsActivate();
			notificationService.AddMessage($"{user} used Khaos Stats");
			Alert(KhaosActionNames.KhaosStats);
		}
		public void KhaosRelics(string user = "Mayhem")
		{
			LegacyRandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			notificationService.AddMessage($"{user} used Khaos Relics");
			Alert(KhaosActionNames.KhaosRelics);
		}
		public void PandorasBox(string user = "Mayhem")
		{
			LegacyRandomizeGold();
			LegacyRandomizeStatsActivate();
			LegacyRandomizeEquipmentSlots();
			LegacyRandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			LegacyRandomizeInventory();
			LegacyRandomizeSubweapon();
			sotnApi.GameApi.RespawnBosses();
			sotnApi.GameApi.RespawnItems();
			notificationService.AddMessage($"{user} opened Pandora's Box");
			Alert(KhaosActionNames.PandorasBox);
		}
		public void Gamble(string user = "Mayhem")
		{
			double goldPercent = (rng.NextDouble() / 2);
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
		public void FillMeter(string user = "Mayhem")
		{
			notificationService.KhaosMeter += 100;
			notificationService.AddMessage($"{user} used Fill Meter");
			Alert(KhaosActionNames.KhaosBurst);
		}
		#endregion
		#region Debuffs
		public void Thirst(string user = "Mayhem")
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
		public void Weaken(string user = "Mayhem")
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

			if (heartsLocked == true)
			{
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
		public void Cripple(string user = "Mayhem")
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
		public void BloodMana(string user = "Mayhem")
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
		public void SubweaponsOnly(string user = "Mayhem")
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
		public void HonestGamer(string user = "Mayhem")
		{
			manaCheat.PokeValue(7);
			manaCheat.Enable();
			manaLocked = true;
			honestGamerActive = true;
			honestGamerTimer.Start();
			notificationService.AddMessage($"{user} used Honest Gamer");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.HonestGamer,
				Type = Enums.ActionType.Debuff,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HonestGamer).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.HonestGamer);
		}
		public void Bankrupt(string user = "Mayhem")
		{
			BankruptActivate();
			notificationService.AddMessage($"{user} used Bankrupt");
			Alert(KhaosActionNames.Bankrupt);
		}
		public void RespawnBosses(string user = "Mayhem")
		{
			gameApi.RespawnBosses();
			notificationService.AddMessage($"{user} used Respawn Bosses");
			Alert(KhaosActionNames.RespawnBosses);
		}
		public void Horde(string user = "Mayhem")
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
		public void Endurance(string user = "Mayhem")
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
		public void HnK(string user = "Mayhem")
		{
			invincibilityCheat.PokeValue(0);
			invincibilityCheat.Enable();
			defensePotionCheat.PokeValue(1);
			defensePotionCheat.Enable();
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
		public void LightHelp(string user = "Mayhem")
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

			if (timeStopActive)
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
		public void MediumHelp(string user = "Mayhem")
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
			if ((highHp && (highMp || manaLocked)) || timeStopActive)
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
		public void HeavytHelp(string user = "Mayhem")
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
		public void Vampire(string user = "Mayhem")
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
		public void BattleOrders(string user = "Mayhem")
		{
			alucardApi.CurrentHp = (uint) (alucardApi.MaxtHp * Constants.Khaos.BattleOrdersHpMultiplier);
			alucardApi.CurrentMp = alucardApi.MaxtMp;
			alucardApi.ActivatePotion(Potion.ShieldPotion);
			notificationService.AddMessage($"{user} used Battle Orders");
			Alert(KhaosActionNames.BattleOrders);
		}
		public void Magician(string user = "Mayhem")
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
		public void ZaWarudo(string user = "Mayhem")
		{
			alucardApi.ActivateStopwatch();
			zaWarudoZone = gameApi.Zone;
			zaWarudoZone2 = gameApi.Zone2;
			timeStopActive = true;

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
		public void MeltyBlood(string user = "Mayhem")
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
		public void Lord(string user = "Mayhem")
		{
			lordTriggerRoomX = gameApi.MapXPos;
			lordTriggerRoomY = gameApi.MapYPos;
			spawnActive = true;

			lordTimer.Start();
			lordSpawnTimer.Start();

			string message = $"{user} made you a Lord";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Summoner);
		}

		public void FourBeasts(string user = "Mayhem")
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
		public void Haste(string user = "Mayhem")
		{
			bool meterFull = KhaosMeterFull();

			if (meterFull)
			{
				SpendKhaosMeter();
				superSpeed = true;
			}

			SetHasteStaticSpeeds(meterFull);
			hasteTimer.Start();
			speedActive = true;
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
		#endregion

		public void Update()
		{
			if (gameApi.InAlucardMode())
			{
				if (bloodManaActive || HPForMPActive)
				{
					CheckManaUsage();
				}
				CheckDashInput();

				if (rushDownActive && preRushdownCon != 0 && (alucardApi.Con < rushdownCon))
				{
					alucardApi.Con = rushdownCon;
				}

				if (underwaterActive && !underwaterPaused && sotnApi.GameApi.CanWarp())
				{
					underwaterPaused = true;
					underwaterPhysics.Disable();
				}

				if (underwaterActive && underwaterPaused && !sotnApi.GameApi.CanWarp())
				{
					underwaterPaused = false;
					underwaterPhysics.Enable();
				}

				bool holyGlassesCheck = false;
				if (sotnApi.AlucardApi.HasItemInInventory("Holy glasses") || Equipment.Items[(int) (alucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses")
				{
					holyGlassesCheck = true;
				}
				if (!hasHolyGlasses && holyGlassesCheck)
				{
					hasHolyGlasses = true;
					notificationService.AddMessage("HolyG: Blessings are now stronger");
				}
				else if (hasHolyGlasses && !holyGlassesCheck)
				{
					hasHolyGlasses = false;
				}

				int newVladCount = 0;
				if (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad))
				{
					++newVladCount;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad))
				{
					++newVladCount;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.RibOfVlad))
				{
					++newVladCount;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.RingOfVlad))
				{
					++newVladCount;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.ToothOfVlad))
				{
					++newVladCount;
				}

				if (newVladCount > (int) vladRelicsObtained)
				{
					notificationService.AddMessage("Vlad: Curses are now stronger");
				}

				vladRelicsObtained = newVladCount;

				if (heartsOnlyActive || rushDownActive)
				{
					if (alucardApi.RightHand == 0)
					{
						alucardApi.RightHand = (uint) Equipment.Items.IndexOf("Pizza");
						if (alucardApi.HasItemInInventory("Pizza"))
						{
							alucardApi.TakeOneItemByName("Pizza");
						}
					}
					if (alucardApi.LeftHand == 0)
					{
						alucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Pizza");
						if (alucardApi.HasItemInInventory("Pizza"))
						{
							alucardApi.TakeOneItemByName("Pizza");
						}
					}
				}
				else if (unarmedActive)
				{
					alucardApi.Subweapon = 0;
					if (alucardApi.RightHand != 0)
					{
						if (alucardApi.HasItemInInventory(Equipment.Items[(int) (alucardApi.RightHand)]))
						{
							alucardApi.TakeOneItemByName(Equipment.Items[(int) (alucardApi.RightHand)]);
						}
						alucardApi.GrantItemByName(Equipment.Items[(int) (alucardApi.RightHand)]);
						alucardApi.RightHand = (uint) Equipment.Items.IndexOf("empty hand");
					}
					if (alucardApi.LeftHand != 0)
					{
						if (alucardApi.HasItemInInventory(Equipment.Items[(int) (alucardApi.LeftHand)]))
						{
							alucardApi.TakeOneItemByName(Equipment.Items[(int) (alucardApi.LeftHand)]);
						}
						alucardApi.GrantItemByName(Equipment.Items[(int) (alucardApi.LeftHand)]);
						alucardApi.LeftHand = (uint) Equipment.Items.IndexOf("empty hand");
					}
				}
				else if (subweaponsOnlyActive)
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
				if (rushDownActive)
				{
					if (sotnApi.AlucardApi.HasControl() && gameApi.CanMenu())
					{
						SetHasteStaticSpeeds(true);
						ToggleRushdownDynamicSpeeds(3);
					}
					else
					{
						SetHasteStaticSpeeds(true);
						ToggleRushdownDynamicSpeeds(2);
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

				#region Neutral Commands
				case "paintrade":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.PainTrade).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => PainTrade(user)));
					}
					break;
				case "maxmayhem":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MaxMayhem).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MaxMayhem(user)));
					}
					break;
				case "heartsonly":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeartsOnly).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Hearts Only", LocksHearts = true, LocksWeapons = true, LocksMana = true,  Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => HeartsOnly(user)) });
					}
					break;
				case "unarmed":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Unarmed).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Unarmed", LocksHearts = true, LocksWeapons = true, ChangesSubWeapons = true, Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => Unarmed(user)) });
					}
					break;
				case "turbomode":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TurboMode).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Turbo Name",Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => TurboMode(user)) });
					}
					break;
				case "rushdown":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RushDown).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Rushdown", LocksSpeed = true, LocksWeapons = true, LocksInvincibility = true, Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => RushDown(user)) });
					}
					break;
				case "swapstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SwapStats).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Swap Stats", ChangesStats = true, Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => SwapStats(user)) });
					}
					break;
				case "swapequipment":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SwapEquipment).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Swap Equipment", ChangesWeapons = true, ChangesSubWeapons = true, Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => SwapEquipment(user)) }); ;
					}
					break;
				case "swaprelics":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SwapRelics).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Swap Relics", Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => SwapRelics(user)) });
					}
					break;
				case "pandemonium":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Pandemonium).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Pandemonium", ChangesStats = true, Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => Pandemonium(user)) });
					}
					break;
				#endregion
				#region Negative Commands
				case "minortrap":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinorTrap).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MinorTrap(user)));
					}
					break;
				case "slam":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Slam).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user)));
					}
					break;
				case "hpformp":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "HP For MP", LocksMana = true, Invoker = new MethodInvoker(() => HPForMP(user)) });
					}
					break;
				case "underwater":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Underwater).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Underwater", LocksSpeed = true, Invoker = new MethodInvoker(() => Underwater(user)) });
					}
					break;
				case "hex":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Hex).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Hex", ChangesStats = true, Invoker = new MethodInvoker(() => Hex(user)) });
					}
					break;
				case "getjuggled":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.GetJuggled).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => GetJuggled(user)));
					}
					break;
				case "ambush":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Ambush).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Ambush", Invoker = new MethodInvoker(() => Ambush(user)) });
					}
					break;
				case "toughbosses":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ToughBosses).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ToughBosses(user)));
					}
					break;
				case "statsdown":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.StatsDown).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Stat Down", ChangesStats = true, Invoker = new MethodInvoker(() => StatsDown(user)) });
					}
					break;
				case "confiscate":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Confiscate).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Confiscate", Invoker = new MethodInvoker(() => Confiscate(user)) });
					}
					break;
				#endregion
				#region Positive commands
				case "minorboon":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinorBoon).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MinorBoon(user)));
					}
					break;
				case "regen":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Regen).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Regen", Type = ActionType.Buff, LocksMana = true, Invoker = new MethodInvoker(() => Regen(user)) });
					}
					break;
				case "speed":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Haste).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Speed", Type = ActionType.Buff, LocksSpeed = true, Invoker = new MethodInvoker(() => Speed(user)) });
					}
					break;
				case "moderateboon":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinorBoon).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ModerateBoon(user)));
					}
					break;
				case "timestop":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TimeStop).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => TimeStop(user)));
					}
					break;
				case "facetank":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FaceTank).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "FaceTank", Type = ActionType.Buff, Invoker = new MethodInvoker(() => FaceTank(user)) });
					}
					break;
				case "spellcaster":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SpellCaster).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "SpellCaster", Type = ActionType.Buff, LocksMana = true, Invoker = new MethodInvoker(() => SpellCaster(user)) });
					}
					break;
				case "extrarange":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ExtraRange).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "MeltyBlood", ChangesWeapons=true, Type = ActionType.Buff, Invoker = new MethodInvoker(() => ExtraRange(user)) });
					}
					break;
				case "summoner":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Summoner", LocksSpawning = true, Type = ActionType.Buff, Invoker = new MethodInvoker(() => Summoner(user)) });
					}
					break;
				case "majorboon":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MajorBoon).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MajorBoon(user)));
					}
					break;
				#endregion
				#region Legacy - Chaotic commands
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
						queuedActions.Add(new QueuedAction { Name = "Khaos Equipment", ChangesWeapons=true, Type = ActionType.Khaotic, Invoker = new MethodInvoker(() => KhaosEquipment(user)) });
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
						queuedFastActions.Enqueue(new MethodInvoker(() => FillMeter(user)));
					}
					break;
				#endregion
				#region Legacy - Debuffs
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
				case "honestgamer":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HonestGamer).FirstOrDefault();
					if (commandAction is not null && commandAction.Enabled)
					{
						queuedActions.Add(new QueuedAction { Name = "Honest Gamer", LocksMana = true, Invoker = new MethodInvoker(() => HonestGamer(user)) });
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
				#region Legacy - Buffs
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
				#endregion
				default:
					commandAction = null;
					break;

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


			#region Timers
			hpForMPDeathTimer.Elapsed += KillAlucard;
			hpForMPDeathTimer.Interval = 1 * (1 * 1500);
			HPForMPTimer.Elapsed += HPForMPOff;
			HPForMPTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault().Duration.TotalMilliseconds;

			heartsOnlyTimer.Elapsed += HeartsOnlyOff;
			heartsOnlyTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeartsOnly).FirstOrDefault().Duration.TotalMilliseconds;
			underwaterTimer.Elapsed += UnderwaterOff;
			underwaterTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Underwater).FirstOrDefault().Duration.TotalMilliseconds;
			hexTimer.Elapsed += HexOff;
			hexTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Hex).FirstOrDefault().Duration.TotalMilliseconds;
			slamJamTickTimer.Elapsed += SlamJam;
			slamJamTickTimer.Interval = 300;


			getJuggledTimer.Elapsed += getJuggledOff;
			getJuggledTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.GetJuggled).FirstOrDefault().Duration.TotalMilliseconds;
			ambushTimer.Elapsed += AmbushOff;
			ambushTimer.Interval = 5 * (60 * 1000);
			ambushSpawnTimer.Elapsed += AmbushSpawn;
			ambushSpawnTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Ambush).FirstOrDefault().Interval.TotalMilliseconds;
			toughBossesSpawnTimer.Elapsed += ToughBossesSpawn;
			toughBossesSpawnTimer.Interval = 2 * (1000);

			turboModeTimer.Elapsed += TurboModeOff;
			turboModeTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TurboMode).FirstOrDefault().Duration.TotalMilliseconds;
			unarmedTimer.Elapsed += UnarmedOff;
			unarmedTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Unarmed).FirstOrDefault().Duration.TotalMilliseconds;
			rushDownTimer.Elapsed += RushDownOff;
			rushDownTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RushDown).FirstOrDefault().Duration.TotalMilliseconds;
			
			regenTimer.Elapsed += RegenOff;
			regenTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Regen).FirstOrDefault().Duration.TotalMilliseconds;
			regenTickTimer.Elapsed += RegenGain;
			regenTickTimer.Interval = 1000;
			faceTankTimer.Elapsed += FaceTankOff;
			faceTankTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.FaceTank).FirstOrDefault().Duration.TotalMilliseconds;
			spellcasterTimer.Elapsed += SpellcasterOff;
			spellcasterTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SpellCaster).FirstOrDefault().Duration.TotalMilliseconds;
			extraRangeTimer.Elapsed += ExtraRangeOff;
			extraRangeTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ExtraRange).FirstOrDefault().Duration.TotalMilliseconds;
			timeStopTimer.Elapsed += TimeStopOff;
			timeStopTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TimeStop).FirstOrDefault().Duration.TotalMilliseconds;
			timeStopCheckTimer.Elapsed += TimeStopAreaCheck;
			timeStopCheckTimer.Interval += 2 * 1000;
			
			speedTimer.Elapsed += SpeedOff;
			speedTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Speed).FirstOrDefault().Duration.TotalMilliseconds;
			speedOverdriveTimer.Elapsed += SpeedOverdriveOn;
			speedOverdriveTimer.Interval = (2 * 1000);
			speedOverdriveOffTimer.Elapsed += SpeedOverdriveOff;
			speedOverdriveOffTimer.Interval = (2 * 1000);
			
			summonerTimer.Elapsed += SummonerOff;
			summonerTimer.Interval = 5 * (60 * 1000);
			summonerSpawnTimer.Elapsed += SummonerSpawn;
			summonerSpawnTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault().Interval.TotalMilliseconds;

			#endregion

			#region Legacy Timers

			bloodManaDeathTimer.Elapsed += KillAlucard;
			bloodManaDeathTimer.Interval = 1 * (1 * 1500);

			subweaponsOnlyTimer.Elapsed += SubweaponsOnlyOff;
			subweaponsOnlyTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SubweaponsOnly).FirstOrDefault().Duration.TotalMilliseconds;
			honestGamerTimer.Elapsed += HonestGamerOff;
			honestGamerTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HonestGamer).FirstOrDefault().Duration.TotalMilliseconds;

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
			lordTimer.Elapsed += LordOff;
			lordTimer.Interval = 5 * (60 * 1000);
			lordSpawnTimer.Elapsed += LordSpawn;
			lordSpawnTimer.Interval = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Lord).FirstOrDefault().Interval.TotalMilliseconds;
			#endregion
		}
		private void StopTimers()
		{
			fastActionTimer.Stop();
			actionTimer.Stop();

			#region Mayhem timers

			hpForMPDeathTimer.Stop();
			HPForMPTimer.Interval = 1;

			underwaterTimer.Interval = 1;
			hexTimer.Interval = 1;
			getJuggledTimer.Interval = 1;
			ambushTimer.Interval = 1;
			ambushSpawnTimer.Interval = 1;
			toughBossesSpawnTimer.Interval = 1;

			turboModeTimer.Interval = 1;
			heartsOnlyTimer.Interval = 1;
			unarmedTimer.Interval = 1;
			rushDownTimer.Interval = 1;

			regenTimer.Interval = 1;
			regenTickTimer.Interval = 1;
			spellcasterTimer.Interval = 1;
			extraRangeTimer.Interval = 1;
			timeStopTimer.Interval = 1;
			timeStopCheckTimer.Interval = 1;

			faceTankTimer.Interval = 1;
			speedTimer.Interval = 1;
			speedOverdriveTimer.Interval = 1;
			speedOverdriveOffTimer.Interval = 1;
			summonerTimer.Interval = 1;
			summonerSpawnTimer.Interval = 1;

			regenTickTimer.Stop();
			ambushSpawnTimer.Stop();
			summonerSpawnTimer.Stop();
			toughBossesSpawnTimer.Stop();
			timeStopCheckTimer.Stop();
			speedOverdriveTimer.Stop();
			speedOverdriveOffTimer.Stop();
			#endregion


			#region Legacy Stop Timers
			bloodManaDeathTimer.Interval = 1;
			subweaponsOnlyTimer.Interval = 1;
			honestGamerTimer.Interval = 1;
			crippleTimer.Interval = 1;

			bloodManaTimer.Interval = 1;
			thirstTimer.Interval = 1;
			thirstTickTimer.Interval = 1;
			hordeTimer.Interval = 1;
			hordeSpawnTimer.Interval = 1;
			enduranceSpawnTimer.Interval = 1;
			hnkTimer.Interval = 1;

			vampireTimer.Interval = 1;
			magicianTimer.Interval = 1;
			meltyTimer.Interval = 1;

			fourBeastsTimer.Interval = 1;
			zawarudoTimer.Interval = 1;
			zawarudoCheckTimer.Interval = 1;
			hasteTimer.Interval = 1;
			hasteOverdriveTimer.Interval = 1;
			hasteOverdriveOffTimer.Interval = 1;
			lordTimer.Interval = 1;
			lordSpawnTimer.Interval = 1;

			thirstTickTimer.Stop();
			lordSpawnTimer.Stop();
			hordeSpawnTimer.Stop();
			enduranceSpawnTimer.Stop();
			zawarudoCheckTimer.Stop();
			hasteOverdriveTimer.Stop();
			hasteOverdriveOffTimer.Stop();
			#endregion
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
						if ((queuedActions[i].ChangesStats && statLocked)||
							(queuedActions[i].ChangesSubWeapons && subWeaponsLocked)||
							(queuedActions[i].LocksSpeed && speedLocked)||
							(queuedActions[i].LocksMana && manaLocked)||
							(queuedActions[i].LocksHearts && heartsLocked)||
							(queuedActions[i].LocksWeapons && weaponsLocked)||
							(queuedActions[i].LocksInvincibility && invincibilityLocked)||
							(queuedActions[i].LocksSpawning && spawnActive))
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
						Console.WriteLine($"All actions locked. statlocked: {statLocked}, subWeaponsLocked: {subWeaponsLocked}, speed: {speedLocked}, manaLocked:{manaLocked}, weaponsLocked{weaponsLocked}, invincibility: {invincibilityLocked}, mana: {manaLocked}");
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
			if (gameApi.InAlucardMode() && sotnApi.AlucardApi.HasControl() && sotnApi.AlucardApi.HasHitbox() && gameApi.CanMenu() && alucardApi.CurrentHp > 0 && !gameApi.CanSave() && !keepRichterRoom && !gameApi.InTransition && !gameApi.IsLoading && !alucardApi.IsInvincible() && !IsInRoomList(Constants.Khaos.LoadingRooms) && alucardMapX < 99)
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

		#region Legacy - Khaotic events
		private void LegacyRandomizeGold()
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
		private void LegacyRandomizeStatsActivate()
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
			if (ActualStatPool < CalculatedStatPool)
			{
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
				else
				{
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
			if (heartsLocked)
			{
				Console.WriteLine("Skipping Kstats Hearts re-roll due to hearts lock.");
			}
			else
			{
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
		private void LegacyRandomizeInventory()
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
		private void LegacyRandomizeSubweapon()
		{
			var subweapons = Enum.GetValues(typeof(Subweapon));
			alucardApi.Subweapon = (Subweapon) subweapons.GetValue(rng.Next(subweapons.Length));
		}
		private void LegacyRandomizeRelicsActivate(bool randomizeVladRelics = true)
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
		private void LegacyRandomizeEquipmentSlots()
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

			LegacyRandomizeSubweapon();
			if (subweaponsOnlyActive)
			{
				while (alucardApi.Subweapon == Subweapon.Empty || alucardApi.Subweapon == Subweapon.Stopwatch)
				{
					LegacyRandomizeSubweapon();
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
					sotnApi.AlucardApi.CurrentMp = 0;
					sotnApi.AlucardApi.CurrentHp = 0;
					sotnApi.AlucardApi.RightHand = (uint) Equipment.Items.IndexOf("Pizza");
					sotnApi.AlucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Pizza");
					bloodManaDeathTimer.Start();
				}
			}
		}
		private void KillAlucard(Object sender, EventArgs e)
		{
			Actor hitbox = new Actor();
			uint offsetPosX = sotnApi.AlucardApi.ScreenX - 255;
			uint offsetPosY = sotnApi.AlucardApi.ScreenY - 255;

			hitbox.Xpos = 0;
			hitbox.Ypos = 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.DamageTypeA = (uint) Actors.Slam;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 999;
			sotnApi.ActorApi.SpawnActor(hitbox);
			sotnApi.AlucardApi.InvincibilityTimer = 0;
			manaLocked = false;
			HPForMPOff(sender, e);
			HPForMPTimer.Stop();
			bloodManaDeathTimer.Stop();
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
			if (alucardApi.CurrentHp > toolConfig.Khaos.ThirstDrainPerSecond + 1 + +superDrain)
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
		private void HonestGamerOff(object sender, EventArgs e)
		{
			manaLocked = false;
			manaCheat.Disable();
			alucardApi.CurrentMp = alucardApi.MaxtMp;
			honestGamerTimer.Stop();
			honestGamerActive = false;
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
			else
			{
				enemy = actorApi.FindActorFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceAlternateRomhackBosses : Constants.Khaos.EnduranceAlternateBosses);
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
			hitbox.Damage = 1;
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
			hitbox.Damage = 1;
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
			hitbox.Damage = 1;
			hitbox.DamageTypeA = (uint) Actors.Stone;
			hitbox.DamageTypeB = (uint) Actors.Stone;
			actorApi.SpawnActor(hitbox);
		}
		private void SpawnSlamHitbox(bool fixedFacing = false)
		{
			Actor hitbox = new Actor();
			int roll = rng.Next(0, 2);
			if (fixedFacing)
			{
				hitbox.Xpos = alucardApi.FacingLeft == true ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			}
			else
			{
				hitbox.Xpos = roll == 1 ? (ushort) (alucardApi.ScreenX + 1) : (ushort) 0;
			}
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = (ushort) (alucardApi.Def + 2);
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
			defensePotionCheat.Disable();
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
			stopwatchTimer.Disable();
			zawarudoTimer.Stop();
			zawarudoCheckTimer.Stop();
			timeStopActive = false;
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
			superSpeed = false;
			speedActive = false;
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
			visualEffectPaletteCheat.PokeValue(33126);
			visualEffectPaletteCheat.Enable();
			visualEffectTimerCheat.PokeValue(30);
			visualEffectTimerCheat.Enable();
			alucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * (toolConfig.Khaos.HasteFactor / 1.8));
			overdriveOn = true;
			hasteOverdriveTimer.Stop();
		}
		private void OverdriveOff(object sender, System.Timers.ElapsedEventArgs e)
		{
			visualEffectPaletteCheat.Disable();
			visualEffectTimerCheat.Disable();
			if (speedActive)
			{
				SetHasteStaticSpeeds(superSpeed);
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
			spirtOrb.Enable();
			batCardXp.PokeValue(10000);
			batCardXp.Enable();
			ghostCardXp.PokeValue(10000);
			ghostCardXp.Enable();
			faerieCardXp.Enable();
			demonCardXp.Enable();
			swordCardXp.PokeValue(7000);
			swordCardXp.Enable();
			spriteCardXp.Enable();
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
					InitializeTempVariables();
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
			defensePotionCheat = cheats.GetCheatByName("DefencePotion");
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

			batCardXp = cheats.GetCheatByName("BatCardXp");
			ghostCardXp = cheats.GetCheatByName("GhostCardXp");
			faerieCardXp = cheats.GetCheatByName("FaerieCardXp");
			demonCardXp = cheats.GetCheatByName("DemonCardXp");
			swordCardXp = cheats.GetCheatByName("SwordCardXp");
			spriteCardXp = cheats.GetCheatByName("SpriteCardXp");
			noseDevilCardXp = cheats.GetCheatByName("NoseDevilCardXp");

			//New Cheats

			spirtOrb = cheats.GetCheatByName("SpiritOrb");
			rushdownBible = cheats.GetCheatByName("RushdownBible");
			turboMode = cheats.GetCheatByName("TurboMode");
			turboModeJump = cheats.GetCheatByName("TurboModeJump");
			accelTime = cheats.GetCheatByName("AccelTime");
			subWeaponDamage = cheats.GetCheatByName("SubWeaponDamage");
			throwMoreSubWeapons1 = cheats.GetCheatByName("ThrowMoreSubWeapons1");
			throwMoreSubWeapons2 = cheats.GetCheatByName("ThrowMoreSubWeapons2");
			throwMoreSubWeapons3 = cheats.GetCheatByName("ThrowMoreSubWeapons3");
			longerHolyWater1 = cheats.GetCheatByName("LongerHolyWater1");
			longerHolyWater2 = cheats.GetCheatByName("LongerHolyWater2");
			longerHolyWater3 = cheats.GetCheatByName("LongerHolyWater3");
			tallerHolyWater1 = cheats.GetCheatByName("TallerHolyWater1");
			tallerHolyWater2 = cheats.GetCheatByName("TallerHolyWater2");
			tallerHolyWater3 = cheats.GetCheatByName("TallerHolyWater3");

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
						galamothTorso.Hp = (ushort) Math.Round((2.5 + (vladRelicsObtained / 10)) * Constants.Khaos.GalamothKhaosHp);
						notificationService.AddMessage($"Super Endurance Galamoth");
					}
					else
					{
						galamothTorso.Hp = (ushort) Math.Round((1.5 + (vladRelicsObtained / 15)) * Constants.Khaos.GalamothKhaosHp);
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
			if (inputService.RegisteredMove(InputKeys.Dash, Globals.UpdateCooldownFrames) && !speedOn && speedActive)
			{
				ToggleHasteDynamicSpeeds(superSpeed ? toolConfig.Khaos.HasteFactor * Constants.Khaos.HasteDashFactor : toolConfig.Khaos.HasteFactor);
				speedOn = true;
				hasteOverdriveTimer.Start();
			}
			else if (!inputService.ButtonHeld(InputKeys.Forward) && speedOn)
			{
				ToggleHasteDynamicSpeeds();
				speedOn = false;
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
