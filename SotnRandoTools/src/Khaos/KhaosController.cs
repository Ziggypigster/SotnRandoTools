using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
		//private readonly IGameApi gameApi;
		//private readonly IAlucardApi alucardApi;
		//private readonly IActorApi actorApi;
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
		private Timer autoActionTimer = new();

		#region Timers
		//Debug
		private System.Windows.Forms.Timer rewindTimer = new();
		private System.Windows.Forms.Timer libraryTimer = new();

		//Neutral
		private System.Windows.Forms.Timer heartsOnlyTimer = new();
		private System.Windows.Forms.Timer unarmedTimer = new();
		private System.Windows.Forms.Timer turboModeTimer = new();
		private System.Windows.Forms.Timer rushDownTimer = new();

		//Curses
		private System.Windows.Forms.Timer HPForMPTimer = new();
		private System.Windows.Forms.Timer hpForMPDeathTimer = new();
		private System.Windows.Forms.Timer underwaterTimer = new();
		private System.Windows.Forms.Timer hexTimer = new();
		private System.Windows.Forms.Timer slamJamTimer = new();
		private System.Windows.Forms.Timer slamJamTickTimer = new();
		private System.Windows.Forms.Timer toughBossesSpawnTimer = new();
		private System.Windows.Forms.Timer getJuggledTimer = new();
		private System.Windows.Forms.Timer ambushTimer = new();
		private System.Windows.Forms.Timer ambushSpawnTimer = new();

		//Blessing
		private System.Windows.Forms.Timer speedTimer = new();
		private System.Windows.Forms.Timer speedOverdriveTimer = new();
		private System.Windows.Forms.Timer speedOverdriveOffTimer = new();
		private System.Windows.Forms.Timer regenTimer = new();
		private System.Windows.Forms.Timer regenTickTimer = new();
		private System.Windows.Forms.Timer buffConHPTimer = new();
		private System.Windows.Forms.Timer timeStopTimer = new();
		private System.Windows.Forms.Timer timeStopCheckTimer = new();
		private System.Windows.Forms.Timer buffIntMPTimer = new();
		private System.Windows.Forms.Timer buffLckNeutralTimer = new();
		private System.Windows.Forms.Timer buffStrRangeTimer = new();
		private System.Windows.Forms.Timer summonerTimer = new();
		private System.Windows.Forms.Timer summonerSpawnTimer = new();
		#endregion

		#region Cheats
		Cheat cubeOfZoe;
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
		Cheat rewind;
		Cheat spirtOrb;
		Cheat alucardEffect;
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
		Cheat playerPaletteCheat;
		Cheat continuousWingsmash;
		#endregion

		#region Auto-Mayhem Variables
		private int autoCurrentTypeCount = 0;
		private int autoCurrentMin = 0;
		private int autoCurrentMax = 0;
		private int autoCurrentType = -1;
		private List<int> autoActionTypes = new List<int>();

		private int autoRequiredSkipCount;
		private int autoCurrentConsecutiveCommands;
		private int autoConsistency;
		private int autoBaseWeightMax;
		private int autoBoostedWeightMax;
		private int autoCommandBlessingThreshold;
		private int autoCommandNeutralThreshold;
		private int autoCommandCurseThreshold;
		private int autoMoodBlessingThreshold;
		private int autoMoodNeutralThreshold;
		private int autoMoodCurseThreshold;
		private int autoMoodCommandMin;
		private int autoMoodSwingModifier;
		private int autoMoodRemaining;
		private int autoMoodRollMax;
		private int autoMoodType;
		private int autoMoodLevel;
		private int autoMoodLevelMax;
		private bool autoAllowSmartLogic = false;
		private bool autoMoodMandatory = false;
		private bool allowPity = false;
		
		private int autoTotalCommandsUsed;
		private int autoPerfectMayhemCounter;
		//private int autoConsecutiveBlessings;
		//private int autoConsecutiveNeutrals;
		//private int autoConsecutiveCurses;
		private int autoBlessingListCount;
		private int autoNeutralListCount;
		private int autoCurseListCount;

		#endregion

		#region Mayhem Variables
		private int vladRelicsObtained = 0;
		private bool hasHolyGlasses = false;

		private uint ambushZone = 0;
		private uint ambushTriggerRoomX = 0;
		private uint ambushTriggerRoomY = 0;
		private List<Entity> ambushEnemies = new();

		private uint timeStopZone = 0;

		private uint summonerZone = 0;
		private uint summonerTriggerRoomX = 0;
		private uint summonerTriggerRoomY = 0;
		private List<Entity> summonerEnemies = new();

		private int toughBossesCount = 0;
		private uint toughBossesRoomX = 0;
		private uint toughBossesRoomY = 0;

		private string slamJamUser;
		List<int> activeHexEffects = new();

		private int buffConHPCount = 0;
		private int buffConHpHolyGlassesCount = 0;
		private int superBuffConHPCount = 0;
		private int superToughBossesCount = 0;
		private int superAmbushCount = 0;

		private bool buffStrRangeHolyGlasses = false;
		private bool buffConHPHolyGlasses = false;
		private bool buffIntMPHolyGlasses = false;
		private bool buffLckNeutralHolyGlasses = false;

		//Debug
		private bool allowFirstCastleRewind = true;
		private bool allowSecondCastleRewind = false;
		private bool isAlucardColorFirstCastle = false;
		private bool isAlucardColorSecondCastle = false;
		
		bool WarpEntrance = false;
		bool WarpMines = false;
		bool WarpOuterWall = false;
		bool WarpKeep = false;
		bool WarpOlrox = false;
		bool WarpInvertedEntrance = false;
		bool WarpInvertedMines = false;
		bool WarpInvertedOuterWall = false;
		bool WarpInvertedKeep = false;
		bool WarpInvertedOlrox = false;
		bool OuterWallElevator = false;
		bool EntranceToMarble = false;
		bool AlchemyElevator = false;
		bool ChapelStatue = false;
		bool ColosseumElevator = false;
		bool ColosseumToChapel = false;
		bool MarbleBlueDoor = false;
		bool CavernsSwitchAndBridge = false;
		bool EntranceToCaverns = false;
		bool EntranceWarp = false;
		bool FirstClockRoomDoor = false;
		bool SecondClockRoomDoor = false;
		bool FirstDemonButton = false;
		bool SecondDemonButton = false;
		bool KeepStairs = false;

		//Buff
		private uint hpGiven = 0;
		private uint strGiven = 0;
		private uint conGiven = 0;
		private uint intGiven = 0;
		private uint lckGiven = 0;
		private uint hpGivenPaused = 0;
		private uint strGivenPaused = 0;
		private uint conGivenPaused = 0;
		private uint intGivenPaused = 0;
		private uint lckGivenPaused = 0;

		//Hex
		private uint hpTaken = 0;
		private uint mpTaken = 0;
		private uint heartsTaken = 0;
		private uint strTaken = 0;
		private uint conTaken = 0;
		private uint intTaken = 0;
		private uint lckTaken = 0;
		private uint leftHandTaken = 0;
		private uint rightHandTaken = 0;
		private uint leftHandReturned = 0;
		private uint rightHandReturned = 0;
		private uint subWeaponTaken = 0;
		private uint slamCount = 0;

		private uint minHP = 80u;
		private uint minMP = 30u;
		private uint minHearts = 50u;
		private uint minStr = 7u;
		private uint minCon = 7u;
		private uint minInt = 7u;
		private uint minLck = 7u;

		private int lockedNeutralLevel = 0;
		private int merchantLevel = 1;
		private int maxMayhemLevel = 1;
		private int heartsOnlyLevel = 1;
		private int unarmedLevel = 1;
		private int turboModeLevel = 1;
		private int rushDownLevel = 1;
		private int swapStatsLevel = 1;
		private int swapEquipmentLevel = 1;
		private int swapRelicsLevel = 1;
		private int pandemoniumLevel = 1;
		private int regenLevel = 0;

		private float underwaterBaseFactor = 1F;
		private float underwaterMayhemFactor = 1F;
		private bool underwaterActive = false;
		private bool underwaterPaused = false;

		private bool speedActive = false;
		private bool speedOn = false;
		/*
		private bool hexActive = false;
		private bool hexHPMPHActive = false;
		private bool hexHPMPHPaused = false;
		private bool hexStatsActive = false;
		private bool hexStatsPaused = false;
		private bool hexWeaponsActive = false;
		private bool hexWeaponsPaused = false;
		private bool hexRelicsActive = false;
		private bool hexRelicsPaused = false;*/

		private bool heartsOnlyActive = false;
		private bool unarmedActive = false;
		private bool unarmedPaused = false;
		private bool turboModeActive = false;
		private bool rushDownActive = false;

		private bool HPForMPActive = false;
		private bool HPForMPActivePaused = false;
		private bool hexActive = false;
		private bool hexHPMPHActive = false;
		private bool hexHPMPHPaused = false;
		private bool hexStatsActive = false;
		private bool hexStatsPaused = false;
		private bool hexWeaponsActive = false;
		private bool hexWeaponsPaused = false;
		private bool hexRelicsActive = false;
		private bool hexRelicsPaused = false;
		private bool getJuggledActive = false;
		private bool slamJamActive = false;

		private bool accelTimeActive = false;
		private bool timeStopActive = false;
		private bool buffActive = false;
		private bool givenStatsPaused = false;
		private bool buffStrRangeActive = false;
		private bool buffConHPActive = false;
		private bool buffIntMPActive = false;
		private bool buffLckNeutralActive = false;

		private bool alucardEffectsLocked = false;
		private bool subWeaponsLocked = false;
		private bool weaponQualitiesLocked = false;
		private bool weaponsLocked = false;
		private bool heartsLocked = false;
		private bool mpLocked = false;
		private bool invincibilityLocked = false;

		private bool cubeOfZoeTaken = false;
		private bool forceOfEchoTaken = false;
		private bool echoOfBatTaken = false;
		private bool fireOfBatTaken = false;
		private bool powerOfWolfTaken = false;
		private bool skillOfWolfTaken = false;
		private bool holySymbolTaken = false;
		private bool faerieCardTaken = false;
		private bool spriteCardTaken = false;
		private bool batCardTaken = false;
		private bool demonCardTaken = false;
		private bool noseDevilCardTaken = false;
		private bool ghostCardTaken = false;
		private bool swordCardTaken = false;
		private bool gasCloudTaken = false;
		private bool ringOfVladTaken = false;
		private bool heartOfVladTaken = false;
		private bool ribOfVladTaken = false;
		private bool toothOfVladTaken = false;
		private bool eyeOfVladTaken = false;

		private bool soulOfWolfTaken = false;
		private bool soulOfBatTaken = false;
		private bool powerOfMistTaken = false;
		private bool grantedTempFlight = false;

		private bool superAmbush = false;
		private bool superSlamJam = false;
		private bool superToughBosses = false;
		private bool superRegen = false;
		private bool superSpeed = false;
		private bool superBuffStrRange = false;
		private bool superBuffIntMP = false;
		private bool superBuffConHP = false;
		private bool superBuffLckNeutral = false;

		private int richterColor = 33056;
		private int alucardColor = 33024;
		private int maxPreviousAlucardColors = 6;
		List<int> previousAlucardColors = new List<int>();
		private bool resetColorWhenAlucard = false;
		#endregion

		#region Legacy Variables
		private uint alucardMapX = 0;
		private uint alucardMapY = 0;
		private bool alucardSecondCastle = false;
		private bool inMainMenu = false;

		private uint storedMaxMp = 0;
		private uint storedMp = 0;
		private int spentMp = 0;

		private bool statLocked = false;
		private bool speedLocked = false;
		private bool overdriveOn = false;
		private bool spawnActive = false;

		private int slowInterval;
		private int normalInterval;
		private int fastInterval;

		private bool shaftHpSet = false;
		private bool galamothStatsSet = false;
		#endregion

		public KhaosController(IToolConfig toolConfig, ISotnApi sotnApi, ICheatCollectionAdapter cheats, INotificationService notificationService, IInputService inputService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (sotnApi is null) throw new ArgumentNullException(nameof(sotnApi));
			//if (gameApi is null) throw new ArgumentNullException(nameof(gameApi));
			//if (sotnApi.AlucardApi is null) throw new ArgumentNullException(nameof(sotnApi.AlucardApi));
			//if (actorApi is null) throw new ArgumentNullException(nameof(actorApi));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			if (notificationService == null) throw new ArgumentNullException(nameof(notificationService));
			if (inputService is null) throw new ArgumentNullException(nameof(inputService));
			this.toolConfig = toolConfig;
			this.sotnApi = sotnApi;
			//this.gameApi = gameApi;
			//this.alucardApi = alucardApi;
			//this.actorApi = actorApi;
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

		public bool AutoMayhemOn { get; set; }

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

			InitializeTimerIntervals();
			ModifyCommands();
			ModifyDifficulty();
			StartCheats();
			DateTime startedAt = DateTime.Now;

			if (AutoMayhemOn)
			{
				InitializeAutoMayhemVariables();
				autoActionTimer.Start();
				notificationService.AddMessage($"Auto Mayhem started");
				Console.WriteLine("Auto Mayhem started");
			}
			else
			{
				if (socketClient.Connected)
				{
					//socketClient.Stop();
				}
				else
				{
					socketClient.Start();
				}
				notificationService.AddMessage($"Mayhem started");
				Console.WriteLine("Mayhem started");
			}


		}
		public void StopMayhem()
		{
			StopTimers();
			spirtOrb.Disable();
			faerieScroll.Disable();
			cubeOfZoe.Disable();
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

			if (!toolConfig.Khaos.EnforceMinStats)
			{
				minHP = 1;
				minMP = 30;
				minHearts = 1;
				minStr = 1;
				minCon = 1;
				minInt = 1;
				minLck = 1;
			}
		}

		public void ModifyCommands()
		{
			merchantLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			maxMayhemLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			heartsOnlyLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			unarmedLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			turboModeLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			rushDownLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			swapStatsLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			swapEquipmentLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			swapRelicsLevel = (short) toolConfig.Khaos.NeutralStartLevel;
			pandemoniumLevel = (short) toolConfig.Khaos.NeutralStartLevel;
		}


		private void InitializeTempVariables()
		{
			//Re-initialize stat / item changes.
			hpGivenPaused = hpGiven;
			strGivenPaused = strGiven;
			intGivenPaused = intGiven;
			lckGivenPaused = lckGiven;
			givenStatsPaused = true;

			hpGiven = 0;
			strGiven = 0;
			intGiven = 0;
			lckGiven = 0;

			hpTaken = 0;
			mpTaken = 0;
			heartsTaken = 0;
			strTaken = 0;
			conTaken = 0;
			intTaken = 0;
			lckTaken = 0;
			leftHandTaken = 0;
			rightHandTaken = 0;
			subWeaponTaken = 0;

			//Re-initialize relics
			cubeOfZoeTaken = false;
			forceOfEchoTaken = false;
			echoOfBatTaken = false;
			fireOfBatTaken = false;
			powerOfWolfTaken = false;
			skillOfWolfTaken = false;
			holySymbolTaken = false;
			faerieCardTaken = false;
			spriteCardTaken = false;
			batCardTaken = false;
			demonCardTaken = false;
			noseDevilCardTaken = false;
			ghostCardTaken = false;
			swordCardTaken = false;
			gasCloudTaken = false;
			ringOfVladTaken = false;
			heartOfVladTaken = false;
			ribOfVladTaken = false;
			toothOfVladTaken = false;
			eyeOfVladTaken = false;
			grantedTempFlight = false;

		}

		#region AutoMayhem Functions
		private void InitializeAutoMayhemVariables()
		{
			autoConsistency = (int) (70 - (10 * toolConfig.Khaos.autoCommandConsistency));
			switch (toolConfig.Khaos.autoCommandConsistency)
			{
				case 0:
					autoRequiredSkipCount = (int) (3);
					break;
				case 1:
					autoRequiredSkipCount = (int) (4);
					break;
				case 2:
					autoRequiredSkipCount = (int) (5);
					break;
				case 3:
					autoRequiredSkipCount = (int) (6);
					break;
				case 4:
					autoRequiredSkipCount = (int) (7);
					break;
				case 5:
					autoRequiredSkipCount = (int) (8);
					break;
				default:
					break;
			}

			autoBaseWeightMax = (int) (toolConfig.Khaos.autoBlessingWeight + toolConfig.Khaos.autoNeutralWeight + toolConfig.Khaos.autoCurseWeight);
			autoMoodSwingModifier = 0;
			if (toolConfig.Khaos.autoMoodSwings > 0)
			{
				autoMoodSwingModifier = (int) (1 + toolConfig.Khaos.autoMoodSwings);
				autoBoostedWeightMax = autoBaseWeightMax + autoMoodSwingModifier;
				autoMoodLevelMax = autoMoodSwingModifier;
				autoMoodCommandMin = (int) (10 * autoMoodSwingModifier);
				autoMoodRollMax = (int) (215 - (15 * autoMoodSwingModifier));
			}
			else {
				autoBoostedWeightMax = autoBaseWeightMax;
				autoMoodLevelMax = 0;
				autoMoodRollMax = 0;
			}

			autoMoodNeutralThreshold = 0;
			autoMoodBlessingThreshold = 0;
			autoMoodCurseThreshold = 0;

			int currentCommandThreshold = 0;
			int currentMoodThreshold = 0;

			if (toolConfig.Khaos.autoAllowNeutrals)
			{
				currentCommandThreshold += toolConfig.Khaos.autoNeutralWeight;
				currentMoodThreshold += toolConfig.Khaos.autoNeutralMood;
				autoCommandNeutralThreshold = currentCommandThreshold;
				autoMoodNeutralThreshold = currentMoodThreshold;
			}
			if (toolConfig.Khaos.autoAllowCurses)
			{
				currentCommandThreshold += toolConfig.Khaos.autoCurseWeight;
				currentMoodThreshold += toolConfig.Khaos.autoCurseMood;
				autoCommandCurseThreshold = currentCommandThreshold;
				autoMoodCurseThreshold = currentMoodThreshold;
			}
			if (toolConfig.Khaos.autoAllowBlessings)
			{
				currentCommandThreshold += toolConfig.Khaos.autoBlessingWeight;
				currentMoodThreshold += toolConfig.Khaos.autoBlessingMood;
				autoCommandBlessingThreshold = currentCommandThreshold;
				autoMoodBlessingThreshold = currentMoodThreshold;
			}
			

			autoMoodRemaining = -1;
			autoMoodLevel = -1;
			autoMoodType = -1;
			autoMoodMandatory = false;
			autoTotalCommandsUsed = 0;
			autoBlessingListCount = toolConfig.Khaos.AutoMayhemBlessings.Count;
			autoNeutralListCount = toolConfig.Khaos.AutoMayhemNeutrals.Count;
			autoCurseListCount = toolConfig.Khaos.AutoMayhemCurses.Count;

			toolConfig.Khaos.AutoMayhemBlessings.Clear();
			toolConfig.Khaos.AutoMayhemNeutrals.Clear();
			toolConfig.Khaos.AutoMayhemCurses.Clear();

			Parallel.ForEach(toolConfig.Khaos.Actions, (action) =>
			{
				if (action.AutoMayhemEnabled)
				{
					if (action.StartsOnCooldown)
					{
						action.setCooldown();
					}
					if (action.Type == (int) ActionType.Neutral)
					{
						toolConfig.Khaos.AutoMayhemNeutrals.Add(new SotnRandoTools.Configuration.Models.Action { Command = action.Command, Meter = action.Meter, LastUsedAt = action.LastUsedAt, Cooldown = action.Cooldown, Type = action.Type });
					}
					else if (action.Type == (int) ActionType.Curse)
					{
						toolConfig.Khaos.AutoMayhemCurses.Add(new SotnRandoTools.Configuration.Models.Action { Command = action.Command, Meter = action.Meter, LastUsedAt = action.LastUsedAt, Cooldown = action.Cooldown, Type = action.Type });
					}
					else if (action.Type == (int) ActionType.Blessing)
					{
						toolConfig.Khaos.AutoMayhemBlessings.Add(new SotnRandoTools.Configuration.Models.Action { Command = action.Command, Meter = action.Meter, LastUsedAt = action.LastUsedAt, Cooldown = action.Cooldown, Type = action.Type });
					}

					action.LastUsedAt = null;
				}
			});
		}
		private void AutoMayhemAction()
		{
			if (autoCurrentConsecutiveCommands > autoRequiredSkipCount)
			{
				autoCurrentConsecutiveCommands = 0;
				Console.WriteLine($"Auto Mayhem - Required Skip!");
				return;
			}
			int roll = rng.Next(0, 101);
			Console.WriteLine($"Auto Mayhem - Consistency Roll: {roll}");
			if (roll > autoConsistency)
			{
				int actionType = -1;

				if (autoCurrentTypeCount > 1 && autoCurrentTypeCount < autoCurrentMin)
				{
					Console.WriteLine($"Auto Mayhem - Repeating actionType: {autoCurrentType}");
					actionType = autoCurrentType;
				}
				else
				{
					bool isInvalidActionType = true;
					int cmdRollMax = 0;

					if (autoMoodLevel < 1)
					{
						cmdRollMax = autoBaseWeightMax;
						RollMoodUpdate();
					}
					else
					{
						cmdRollMax = autoBoostedWeightMax;
					}

					while (isInvalidActionType)
					{
						roll = rng.Next(0, cmdRollMax);
						if (roll < autoCommandNeutralThreshold)
						{
							actionType = (int) ActionType.Neutral;
						}
						else if (roll < autoCommandCurseThreshold)
						{
							actionType = (int) ActionType.Curse;
						}
						else if (roll < autoCommandBlessingThreshold)
						{
							actionType = (int) ActionType.Blessing;
						}
						else
						{
							actionType = autoMoodType;
						}
						if ((actionType == autoCurrentType && autoCurrentTypeCount >= autoCurrentMax) || actionType == -1)
						{
							isInvalidActionType = true;
						}
						else
						{
							isInvalidActionType = false;
						}
					}
					
					Console.WriteLine($"Auto Mayhem - Command Roll: roll={roll}, cmdRollMax={cmdRollMax}, actionType={actionType},  currType:{autoCurrentType}, currCount={autoCurrentTypeCount}, currentMax={autoCurrentMax}");
				}
				if (actionType != autoCurrentType)
				{
					autoCurrentTypeCount = 0;
					autoCurrentType = actionType;
					switch (actionType)
					{
						
						case (int) ActionType.Neutral:
							autoCurrentMin = toolConfig.Khaos.autoNeutralMin;
							autoCurrentMax = toolConfig.Khaos.autoNeutralMax;
							break;
						case (int) ActionType.Curse:
							autoCurrentMin = toolConfig.Khaos.autoCurseMin;
							autoCurrentMax = toolConfig.Khaos.autoCurseMax;
							break;
						case (int) ActionType.Blessing:
							autoCurrentMin = toolConfig.Khaos.autoBlessingMin;
							autoCurrentMax = toolConfig.Khaos.autoBlessingMax;
							break;
						default:
							break;
					}
				}
				else
				{
					++autoCurrentTypeCount;
				}



				int index = 0;
				bool isValidAction = false;
				EventAddAction? actionEvent = new();

				switch (actionType)
				{
					case (int) ActionType.Neutral:
						while (!isValidAction)
						{
							index = rng.Next(0, toolConfig.Khaos.AutoMayhemNeutrals.Count);
							//Console.WriteLine($"Auto Mayhem - Neutral Index Attempt: {index}, Max: {toolConfig.Khaos.AutoMayhemNeutrals.Count}");
							if (!toolConfig.Khaos.AutoMayhemNeutrals[index].IsOnCooldown())
							{
								isValidAction = true;
								toolConfig.Khaos.AutoMayhemNeutrals[index].setCooldown();
							}
						}

						actionEvent = new()
						{
							UserName = Constants.Khaos.AutoMayhemName,
							Command = toolConfig.Khaos.AutoMayhemNeutrals[index].Command
						};
						break;
					case (int) ActionType.Curse:
						while (!isValidAction)
						{
							index = rng.Next(0, toolConfig.Khaos.AutoMayhemCurses.Count);
							//Console.WriteLine($"Auto Mayhem - Curse Index Attempt: {index}");
							if (!toolConfig.Khaos.AutoMayhemCurses[index].IsOnCooldown())
							{
								isValidAction = true;
								toolConfig.Khaos.AutoMayhemCurses[index].setCooldown();
							}
						}
						actionEvent = new()
						{
							UserName = Constants.Khaos.AutoMayhemName,
							Command = toolConfig.Khaos.AutoMayhemCurses[index].Command
						};
						break;
					case (int) ActionType.Blessing:
						while (!isValidAction)
						{
							index = rng.Next(0, toolConfig.Khaos.AutoMayhemBlessings.Count);
							//Console.WriteLine($"Auto Mayhem - Blessing Index Attempt: {index}");
							if (!toolConfig.Khaos.AutoMayhemBlessings[index].IsOnCooldown())
							{
								isValidAction = true;
								toolConfig.Khaos.AutoMayhemBlessings[index].setCooldown();
							}
						}
						actionEvent = new()
						{
							UserName = Constants.Khaos.AutoMayhemName,
							Command = toolConfig.Khaos.AutoMayhemBlessings[index].Command
						};
						break;
					default:
						break;
				}



				if (actionEvent.Command != "")
				{
					Console.WriteLine($"Auto Mayhem - Queued Action: {actionEvent.Command}");
					EnqueueAction(actionEvent);
					++autoTotalCommandsUsed;
					++autoPerfectMayhemCounter;
					++autoCurrentConsecutiveCommands;
					if(toolConfig.Khaos.autoAllowPerfectMayhem && autoPerfectMayhemCounter >= toolConfig.Khaos.autoPerfectMayhemTrigger)
					{
						autoPerfectMayhemCounter = 0;
						autoPerfectMayhem();
					}
				}
				Console.WriteLine($"Auto Mayhem - #{autoTotalCommandsUsed} Command Result: {actionEvent.Command}, actionType={actionType}, currentTypeCount={autoCurrentTypeCount}");

			}
			else
			{
				autoCurrentConsecutiveCommands = 0;
				return;
			}
		}
		private void RollMoodUpdate ()
		{
			if (autoMoodRollMax < 1 || autoMoodLevel > 0 || autoMoodMandatory) 
			{
				Console.WriteLine($"Auto Mayhem - Skipping: autoMoodRollMax - {autoMoodRollMax}, autoMoodLevel - {autoMoodLevel}, autoMoodMandatory - {autoMoodMandatory}");
				return;
			}
			else
			{ 
				int roll = rng.Next(0, autoMoodRollMax);
				int actionType = -1;
				if (roll < autoMoodNeutralThreshold)
				{
					actionType = (int) ActionType.Neutral;
				}
				else if (roll < autoMoodCurseThreshold)
				{
					actionType = (int) ActionType.Curse;
				}
				else if (roll < autoMoodBlessingThreshold)
				{
					actionType = (int) ActionType.Blessing;
				}

				if (actionType > -1)
				{
					UpdateAutoMood(actionType,false,false);
				}

				Console.WriteLine($"Auto Mayhem - Mood Results: roll={roll}, autoMoodRollMax={autoMoodRollMax}, actionType={actionType}");
			}
		}
		private void UpdateAutoMood(int actionType = 1, bool IsDecrease = false, bool IsMandatory = false)
		{
			if(autoMoodLevelMax == 0)
			{
				return;
			}
			else if (IsDecrease)
			{
				--autoMoodLevel;
				if (autoMoodLevel == 0)
				{
					autoMoodRemaining = 0;
					autoMoodMandatory = false;
				}
				else
				{
					autoMoodRemaining = autoMoodCommandMin;
				}
				if (autoMoodLevel > 1)
				{
					if (autoMoodMandatory)
					{
						switch (actionType)
						{
							case (int) ActionType.Neutral:
								notificationService.AddMessage("Mood: Mayhem is decreasing");
								break;
							case (int) ActionType.Curse:
								notificationService.AddMessage("Mood: Rage is decreasing");
								break;
							case (int) ActionType.Blessing:
								notificationService.AddMessage("Mood: Pity is decreasing");
								break;
							default:
								break;
						}
					}
					else
					{
						switch (actionType)
						{
							case (int) ActionType.Neutral:
								notificationService.AddMessage($"Neutral mood swing is decreasing");
								break;
							case (int) ActionType.Curse:
								notificationService.AddMessage($"Curse mood swing is decreasing");
								break;
							case (int) ActionType.Blessing:
								notificationService.AddMessage($"Blessing mood swing is decreasing");
								break;
							default:
								break;
						}
					}
				}
				else
				{
					autoMoodType = 0;
					notificationService.AddMessage($"{Constants.Khaos.AutoMayhemName}'s Mood is back to normal!");
				}
			}
			else
			{
				autoMoodRemaining = autoMoodCommandMin;
				if (autoMoodLevel < autoMoodLevelMax)
				{
					++autoMoodLevel;
				}

				if (autoMoodType == actionType)
				{
					
					if (IsMandatory)
					{
						switch (actionType)
						{
							case (int) ActionType.Neutral:
								notificationService.AddMessage($"Mood: Mayhem is rising!");
								break;
							case (int) ActionType.Curse:
								notificationService.AddMessage($"Mood: Rage is rising!");
								break;
							case (int) ActionType.Blessing:
								notificationService.AddMessage($"Mood: Pity is rising!");
								break;
							default:
								break;
						}
					}
					else
					{
						switch (actionType)
						{
							case (int) ActionType.Neutral:
								notificationService.AddMessage($"Mood swing: More Neutrals!");
								break;
							case (int) ActionType.Curse:
								notificationService.AddMessage($"Mood swing: More Curses!");
								break;
							case (int) ActionType.Blessing:
								notificationService.AddMessage($"Mood swing: More Blessings!");
								break;
							default:
								break;
						}
					}
				}
				else
				{
					autoMoodLevel = 1;
					autoMoodType = actionType;

					if (IsMandatory)
					{
						switch (actionType)
						{

							case (int) ActionType.Neutral:
								notificationService.AddMessage($"{Constants.Khaos.AutoMayhemName} wants Perfect Mayhem!");
								break;
							case (int) ActionType.Curse:
								notificationService.AddMessage($"{Constants.Khaos.AutoMayhemName} feels Rage!");
								break;
							case (int) ActionType.Blessing:
								notificationService.AddMessage($"{Constants.Khaos.AutoMayhemName} feels Pity!");
								break;
							default:
								break;
						}
						autoMoodMandatory = true;
					}
					else
					{
						switch (actionType)
						{
							case (int) ActionType.Neutral:
								notificationService.AddMessage($"Mood swing: Neutrals!");
								break;
							case (int) ActionType.Curse:
								notificationService.AddMessage($"Mood swing: Curses!");
								break;
							case (int) ActionType.Blessing:
								notificationService.AddMessage($"Mood swing: Blessings!");
								break;
							default:
								break;
						}
					}
				}
			}
			autoBoostedWeightMax = autoBaseWeightMax + (autoMoodSwingModifier * (1 + autoMoodLevel));
		}

		private void autoPerfectMayhem()
		{
			notificationService.AddMessage($"Perfect MAYHEM!");
			UpdateAutoMood((int) ActionType.Neutral, false, true);
			EnqueueAction(new EventAddAction { Command = "pandemonium", UserName = Constants.Khaos.AutoMayhemName });
			EnqueueAction(new EventAddAction { Command = "maxmayhem", UserName = Constants.Khaos.AutoMayhemName });
		}

		private void autoMayhemRage()
		{
			int roll = rng.Next(0, 5);
			UpdateAutoMood((int) ActionType.Curse, false, true);
			switch (roll)
			{
				case 0:
					notificationService.AddMessage($"Rage: Shakey's Revenge!");
					EnqueueAction(new EventAddAction { Command = "underwater", UserName = Constants.Khaos.AutoMayhemName});
					EnqueueAction(new EventAddAction { Command = "underwater", UserName = Constants.Khaos.AutoMayhemName});
					EnqueueAction(new EventAddAction { Command = "underwater", UserName = Constants.Khaos.AutoMayhemName});
					break;
				case 1:
					notificationService.AddMessage($"Rage: Chaotic Chris!");
					EnqueueAction(new EventAddAction { Command = "maxmayhem", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "slam", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "underwater", UserName = Constants.Khaos.AutoMayhemName });
					break;
				case 2:
					notificationService.AddMessage($"Rage: Tri to Survive!");
					EnqueueAction(new EventAddAction { Command = "getjuggled", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "hex", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "slam", UserName = Constants.Khaos.AutoMayhemName });
					break;
				case 3:
					notificationService.AddMessage($"Rage: Get Kirked!");
					EnqueueAction(new EventAddAction { Command = "getjuggled", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "ambush", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "toughbosses", UserName = Constants.Khaos.AutoMayhemName });
					break;
				case 4:
					notificationService.AddMessage($"Rage: Ziggy Bomb!");
					EnqueueAction(new EventAddAction { Command = "slamjam", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "getjuggled", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "ambush", UserName = Constants.Khaos.AutoMayhemName });
					break;
				default:
					break;
			}
		}

		private void autoMayhemPity()
		{
			int roll = rng.Next(0, 5);
			UpdateAutoMood((int) ActionType.Blessing, false, true);
			switch (roll)
			{
				case 0:
					notificationService.AddMessage($"Pity: Speed is Key!");
					EnqueueAction(new EventAddAction { Command = "speed", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "speed", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "speed", UserName = Constants.Khaos.AutoMayhemName });
					break;
				case 1:
					notificationService.AddMessage($"Pity: Turbo Time!");
					EnqueueAction(new EventAddAction { Command = "turbomode", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "speed", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "buffintmp", UserName = Constants.Khaos.AutoMayhemName });
					break;
				case 2:
					notificationService.AddMessage($"Pity: This enough HP?");
					EnqueueAction(new EventAddAction { Command = "buffconhp", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "buffconhp", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "regen", UserName = Constants.Khaos.AutoMayhemName });
					break;
				case 3:
					notificationService.AddMessage($"Pity: Time's on your side!");
					EnqueueAction(new EventAddAction { Command = "timestop", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "regen", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "buffstrrange", UserName = Constants.Khaos.AutoMayhemName });
					break;
				case 4:
					notificationService.AddMessage($"Pity: Gifts Aplenty!");
					EnqueueAction(new EventAddAction { Command = "moderatequipment", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "equipment", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "items", UserName = Constants.Khaos.AutoMayhemName });
					break;
				case 5:
					notificationService.AddMessage($"Pity: Bulk Up!");
					EnqueueAction(new EventAddAction { Command = "majorstats", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "majorstats", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "buffconhp", UserName = Constants.Khaos.AutoMayhemName });
					break;
				default:
					break;
			}
		}

		#region Debug Commands
		public void LogCurrentRoom(string user = "Mayhem")
		{
			notificationService.AddMessage($"{user} used {KhaosActionNames.LogCurrentRoom}");

			Console.WriteLine($"Debug - Is2ndCastle:{alucardSecondCastle}; Room Co-ordinates: X{sotnApi.GameApi.RoomX}, Y{sotnApi.GameApi.RoomY}; alucardMapX{sotnApi.AlucardApi.MapX}, alucardMapY{sotnApi.AlucardApi.MapY} ");
			Console.WriteLine($"Debug - Equipment: Left ={sotnApi.AlucardApi.LeftHand}, Right = {sotnApi.AlucardApi.RightHand}, Armor = {sotnApi.AlucardApi.Armor}");
		}
		public void Rewind(string user = "Mayhem")
		{
			if (sotnApi.GameApi.IsInMenu() || IsInRoomList(Constants.Khaos.RewindBanRoom) || (!alucardSecondCastle && !allowFirstCastleRewind) || (alucardSecondCastle && !allowSecondCastleRewind))
			{
				queuedFastActions.Enqueue(new MethodInvoker(() => Rewind(user)));
				//queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Rewind, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => Rewind())});
				return;
			}
			notificationService.AddMessage($"{user} used {KhaosActionNames.Rewind}");

			WarpEntrance = sotnApi.AlucardApi.WarpEntrance;
			WarpMines = sotnApi.AlucardApi.WarpMines;
			WarpOuterWall = sotnApi.AlucardApi.WarpOuterWall;
			WarpKeep = sotnApi.AlucardApi.WarpKeep;
			WarpOlrox = sotnApi.AlucardApi.WarpOlrox;
			WarpInvertedEntrance = sotnApi.AlucardApi.WarpInvertedEntrance;
			WarpInvertedMines = sotnApi.AlucardApi.WarpInvertedMines;
			WarpInvertedOuterWall = sotnApi.AlucardApi.WarpInvertedOuterWall;
			WarpInvertedKeep = sotnApi.AlucardApi.WarpInvertedKeep;
			WarpInvertedOlrox = sotnApi.AlucardApi.WarpInvertedOlrox;
			OuterWallElevator = sotnApi.AlucardApi.OuterWallElevator;
			EntranceToMarble = sotnApi.AlucardApi.EntranceToMarble;
			AlchemyElevator = sotnApi.AlucardApi.AlchemyElevator;
			ChapelStatue = sotnApi.AlucardApi.ChapelStatue;
			ColosseumElevator = sotnApi.AlucardApi.ColosseumElevator;
			ColosseumToChapel = sotnApi.AlucardApi.ColosseumToChapel;
			MarbleBlueDoor = sotnApi.AlucardApi.MarbleBlueDoor;
			CavernsSwitchAndBridge = sotnApi.AlucardApi.CavernsSwitchAndBridge;
			EntranceToCaverns = sotnApi.AlucardApi.EntranceToCaverns;
			EntranceWarp = sotnApi.AlucardApi.EntranceWarp;
			FirstClockRoomDoor = sotnApi.AlucardApi.FirstClockRoomDoor;
			SecondClockRoomDoor = sotnApi.AlucardApi.SecondClockRoomDoor;
			FirstDemonButton = sotnApi.AlucardApi.FirstDemonButton;
			SecondDemonButton = sotnApi.AlucardApi.SecondDemonButton;
			KeepStairs = sotnApi.AlucardApi.KeepStairs;






			rewind.Enable();
			rewindTimer.Start();
		}
		public void MinStats(string user = "Mayhem")
		{
			notificationService.AddMessage($"{user} used {KhaosActionNames.MinStats}");
			sotnApi.AlucardApi.MaxtHearts = minHearts;

			HPForMPActivePaused = true;

			sotnApi.AlucardApi.MaxtMp = minMP;
			sotnApi.AlucardApi.CurrentMp = minMP;
			storedMaxMp = sotnApi.AlucardApi.MaxtMp;
			storedMp = sotnApi.AlucardApi.CurrentMp;

			HPForMPActivePaused = false;

			sotnApi.AlucardApi.MaxtHp = minHP + hpGiven;

			sotnApi.AlucardApi.CurrentHp = minHP;
			sotnApi.AlucardApi.CurrentHearts = minHearts;

			sotnApi.AlucardApi.Str = minStr + strGiven;
			sotnApi.AlucardApi.Con = minCon + conGiven;
			sotnApi.AlucardApi.Int = minInt + intGiven;
			sotnApi.AlucardApi.Lck = minLck + lckGiven;

			sotnApi.AlucardApi.Level = 1;
			sotnApi.AlucardApi.Experiecne = 0;
		}
		public void Library(string user = "Mayhem")
		{
			//Library = 41 Hex = 65 Dec
			notificationService.AddMessage($"{user} used {KhaosActionNames.Library}");
			libraryTimer.Start();
			alucardEffectsLocked = true;
			alucardEffect.PokeValue(65);
			alucardEffect.Enable();
		}

		private void RewindOff(object sender, EventArgs e)
		{
			//notificationService.AddMessage($"{KhaosActionNames.Rewind} Off");
			rewind.Disable();

			sotnApi.AlucardApi.WarpEntrance = WarpEntrance;
			sotnApi.AlucardApi.WarpMines = WarpMines;
			sotnApi.AlucardApi.WarpOuterWall = WarpOuterWall;
			sotnApi.AlucardApi.WarpKeep = WarpKeep;
			sotnApi.AlucardApi.WarpOlrox = WarpOlrox;
			sotnApi.AlucardApi.WarpInvertedEntrance = WarpInvertedEntrance;
			sotnApi.AlucardApi.WarpInvertedMines = WarpInvertedMines;
			sotnApi.AlucardApi.WarpInvertedOuterWall = WarpInvertedOuterWall;
			sotnApi.AlucardApi.WarpInvertedKeep = WarpInvertedKeep;
			sotnApi.AlucardApi.WarpInvertedOlrox = WarpInvertedOlrox;
			sotnApi.AlucardApi.OuterWallElevator = OuterWallElevator;
			sotnApi.AlucardApi.EntranceToMarble = EntranceToMarble;
			sotnApi.AlucardApi.AlchemyElevator = AlchemyElevator;
			sotnApi.AlucardApi.ChapelStatue = ChapelStatue;
			sotnApi.AlucardApi.ColosseumElevator = ColosseumElevator;
			sotnApi.AlucardApi.ColosseumToChapel = ColosseumToChapel;
			sotnApi.AlucardApi.MarbleBlueDoor = MarbleBlueDoor;
			sotnApi.AlucardApi.CavernsSwitchAndBridge = CavernsSwitchAndBridge;
			sotnApi.AlucardApi.EntranceToCaverns = EntranceToCaverns;
			sotnApi.AlucardApi.EntranceWarp = EntranceWarp;
			sotnApi.AlucardApi.FirstClockRoomDoor = FirstClockRoomDoor;
			sotnApi.AlucardApi.SecondClockRoomDoor = SecondClockRoomDoor;
			sotnApi.AlucardApi.FirstDemonButton = FirstDemonButton;
			sotnApi.AlucardApi.SecondDemonButton = SecondDemonButton;
			sotnApi.AlucardApi.KeepStairs = KeepStairs;

			rewindTimer.Stop();
		}

		private void LibraryOff(object sender, EventArgs e)
		{
			//notificationService.AddMessage($"{KhaosActionNames.Library} Off");
			alucardEffect.Disable();
			libraryTimer.Stop();
			alucardEffectsLocked = false;	
		}

		#endregion

		public void addHPGiven(uint addHPGiven = 0)
		{
			if (addHPGiven > 0)
			{
				sotnApi.AlucardApi.MaxtHp += addHPGiven;
				if (sotnApi.AlucardApi.CurrentHp + addHPGiven < sotnApi.AlucardApi.MaxtHp * 2)
				{
					sotnApi.AlucardApi.CurrentHp += addHPGiven;
				}
				else
				{
					sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp * 2;
				}
			}
			else
			{
				sotnApi.AlucardApi.MaxtHp += hpGivenPaused;
				sotnApi.AlucardApi.CurrentHp += hpGivenPaused;
				hpGiven = hpGivenPaused;
				hpGivenPaused = 0;
			}
		}
		public void addSTRGiven(uint addSTRGiven = 0)
		{
			if (addSTRGiven > 0) 
			{
				strGiven += addSTRGiven;
				sotnApi.AlucardApi.Str += addSTRGiven;
			}	
			else
			{
				sotnApi.AlucardApi.Str += strGivenPaused;
				strGiven += strGivenPaused;
				strGivenPaused = 0;
			}
		}
		public void addCONGiven(uint addCONGiven = 0)
		{
			if (addCONGiven > 0)
			{
				conGiven += addCONGiven;
				sotnApi.AlucardApi.Con += addCONGiven;
			}
			else
			{
				sotnApi.AlucardApi.Con += conGivenPaused;
				conGiven = conGivenPaused;
				conGivenPaused = 0;
			}
		}
		public void addINTGiven(uint addINTGiven = 0)
		{
			if (addINTGiven > 0)
			{
				intGiven += addINTGiven;
				sotnApi.AlucardApi.Int += addINTGiven;
			}
			else
			{
				sotnApi.AlucardApi.Int += intGivenPaused;
				intGiven = intGivenPaused;
				intGivenPaused = 0;
			}
		}
		public void addLCKGiven(uint addLCKGiven = 0)
		{
			if (addLCKGiven > 0)
			{
				lckGiven += addLCKGiven;
				sotnApi.AlucardApi.Lck += addLCKGiven;
			}
			else
			{
				sotnApi.AlucardApi.Lck += lckGivenPaused;
				lckGiven = lckGivenPaused;
				lckGivenPaused = 0;
			}
		}
		public void takeHPGiven(uint subtractHPGiven = 0)
		{
			if (subtractHPGiven > 0)
			{
				if (givenStatsPaused)
				{
					hpGivenPaused -= subtractHPGiven;
					return;
				}
				else
				{
					if (sotnApi.AlucardApi.CurrentHp - subtractHPGiven > 1)
					{
						sotnApi.AlucardApi.CurrentHp -= subtractHPGiven;
					}
					else
					{
						sotnApi.AlucardApi.CurrentHp = 1;
					}
					if(sotnApi.AlucardApi.MaxtHp > subtractHPGiven)
					{
						sotnApi.AlucardApi.MaxtHp -= subtractHPGiven;
					}
					else
					{
						sotnApi.AlucardApi.MaxtHp = minHP;
					}
				}
			}
			else
			{
				if(sotnApi.AlucardApi.MaxtHp > hpGiven)
				{
					sotnApi.AlucardApi.MaxtHp -= hpGiven;
				}
				else
				{
					sotnApi.AlucardApi.MaxtHp = minHP;
				}
				if (sotnApi.AlucardApi.CurrentHp > hpGiven)
				{
					sotnApi.AlucardApi.CurrentHp -= hpGiven;
				}
				else
				{
					sotnApi.AlucardApi.CurrentHp = 1;
				}
				hpGivenPaused = hpGiven;
				hpGiven = 0;
			}
		}
		public void takeSTRGiven(uint subtractStatGiven = 0)
		{
			uint statCheck = sotnApi.AlucardApi.Str;
			uint minStat = minStr;

			if (subtractStatGiven > 0)
			{
				if (givenStatsPaused)
				{
					strGivenPaused -= subtractStatGiven;
				}
				else 
				{
					if(statCheck > subtractStatGiven && statCheck - subtractStatGiven > minStat)
					{
						sotnApi.AlucardApi.Str = statCheck - subtractStatGiven;
					}
					else
					{
						sotnApi.AlucardApi.Str = minStat;
					}
					strGiven = strGiven > subtractStatGiven ? strGiven - subtractStatGiven : 0;
				}				
			}
			else
			{
				if(statCheck > strGiven && statCheck - strGiven > minStat)
				{
					sotnApi.AlucardApi.Str = statCheck - strGiven;
				}
				else
				{
					sotnApi.AlucardApi.Str = minStat;
				}
				strGivenPaused = strGiven;
				strGiven = 0;
			}
		}
		public void takeCONGiven(uint subtractStatGiven = 0)
		{
			uint statCheck = sotnApi.AlucardApi.Con;
			uint minStat = minCon;

			if (subtractStatGiven > 0)
			{
				if (givenStatsPaused)
				{
					conGivenPaused -= subtractStatGiven;
				}
				else
				{
					if (statCheck > subtractStatGiven && statCheck - subtractStatGiven > minStat)
					{
						sotnApi.AlucardApi.Con = statCheck - subtractStatGiven;
					}
					else
					{
						sotnApi.AlucardApi.Con = minStat;
					}
					conGiven = conGiven > subtractStatGiven ? conGiven - subtractStatGiven : 0;
				}
			}
			else
			{
				if (statCheck > conGiven && statCheck - conGiven > minStat)
				{
					sotnApi.AlucardApi.Con = statCheck - conGiven;
				}
				else
				{
					sotnApi.AlucardApi.Con = minStat;
				}
				conGivenPaused = conGiven;
				conGiven = 0;
			}
		}
		public void takeINTGiven(uint subtractStatGiven = 0)
		{
			uint statCheck = sotnApi.AlucardApi.Int;
			uint minStat = minInt; 

			if (subtractStatGiven > 0)
			{
				if (givenStatsPaused)
				{
					intGivenPaused -= subtractStatGiven;
				}
				else
				{
					if (statCheck > subtractStatGiven && statCheck - subtractStatGiven > minStat)
					{
						sotnApi.AlucardApi.Int = statCheck - subtractStatGiven;
					}
					else
					{
						sotnApi.AlucardApi.Int = minStat;
					}
					intGiven = intGiven > subtractStatGiven ? intGiven - subtractStatGiven : 0;
				}
			}
			else
			{
				if (statCheck > intGiven && statCheck - intGiven > minStat)
				{
					sotnApi.AlucardApi.Int = statCheck - intGiven;
				}
				else
				{
					sotnApi.AlucardApi.Int = minStat;
				}
				intGivenPaused = intGiven;
				intGiven = 0;
			}
		}
		public void takeLCKGiven(uint subtractStatGiven = 0)
		{
			uint statCheck = sotnApi.AlucardApi.Lck;
			uint minStat = minLck;

			if (subtractStatGiven > 0)
			{
				if (givenStatsPaused)
				{
					intGivenPaused -= subtractStatGiven;
				}
				else
				{
					if (statCheck > subtractStatGiven && statCheck - subtractStatGiven > minStat)
					{
						sotnApi.AlucardApi.Lck = statCheck - subtractStatGiven;
					}
					else
					{
						sotnApi.AlucardApi.Lck = minStat;
					}
					lckGiven = lckGiven > subtractStatGiven ? lckGiven - subtractStatGiven : 0;
				}
			}
			else
			{
				if (statCheck > lckGiven && statCheck - lckGiven > minStat)
				{
					sotnApi.AlucardApi.Lck = statCheck - lckGiven;
				}
				else
				{
					sotnApi.AlucardApi.Lck = minLck;
				}
				lckGivenPaused = lckGiven;
				lckGiven = 0;
			}
		}

		#region Neutral Effects
		public short NeutralMeterGain(int level)
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

		public int checkLockedNeutralLevel (int level)
		{
			if(lockedNeutralLevel == 0)
			{
				return level;
			}
			else
			{
				return lockedNeutralLevel;
			}
		}

		public int UpdateNeutralLevel(int level)
		{
			++level;
			if (level > toolConfig.Khaos.NeutralMaxLevel)
			{
				if (toolConfig.Khaos.AllowNeutralLevelReset)
				{
					level = (int)toolConfig.Khaos.NeutralMinLevel;
				}
				else
				{
					level = (int)toolConfig.Khaos.NeutralMaxLevel;
				}
			}
			return level;
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

		public void Merchant(string user = "Mayhem")
		{

			merchantLevel = checkLockedNeutralLevel(merchantLevel);

			uint currentHp = sotnApi.AlucardApi.CurrentHp;
			uint currentMana = sotnApi.AlucardApi.CurrentMp;
			uint currentHearts = sotnApi.AlucardApi.CurrentHearts;
			uint currentGold = sotnApi.AlucardApi.Gold;

			double a = rng.NextDouble() / 2;
			double b = rng.NextDouble() / 3;
			double c = rng.NextDouble() / 3;
			double sum = a + b + c;
			double merchant = 12.00 / 100.00;

			double percentageGold = (double) (1.00 - ((a / sum) * merchant));
			double percentageMP = (double) (1.00 - ((b / sum) * merchant));
			double percentageHearts = (double) (1.00 - ((c / sum) * merchant));
			

			if (mpLocked || currentMana < 2)
			{
				percentageHearts -= (1.00-percentageMP);
				percentageMP = 0;
			}
			if (heartsLocked || currentHearts < 2)
			{
				percentageGold -= (1.00-percentageHearts);
				percentageHearts = 0;
			}
			if (currentGold < 2 && percentageGold < 1)
			{
				percentageGold = 0;
			}

			//notificationService.AddMessage($"Debug - {merchant}; a{a} b{b} c{c}; {percentageMP}MP/{percentageHearts}H/{percentageGold}GP");
			//notificationService.AddMessage($"Debug - {merchant}; a{a} b{b} c{c}; {percentageMP}MP/{percentageHearts}H/{percentageGold}GP");

			uint flatRemoval = (uint)(5 - merchantLevel);

			uint newMP = (uint) Math.Ceiling((currentMana * percentageMP));
			uint newHearts = (uint) Math.Ceiling((currentHearts * percentageHearts));
			uint newGold = (uint) Math.Ceiling((currentGold * percentageGold));

			newMP = newMP <= flatRemoval ? 0 : newMP - flatRemoval;
			newHearts = newHearts <= flatRemoval ? 0 : newHearts - flatRemoval;
			newGold = newGold < 0 ? 0 : newGold;
			
			if (!mpLocked)
			{
				sotnApi.AlucardApi.CurrentMp = newMP;
			}
			if (!heartsLocked)
			{
				sotnApi.AlucardApi.CurrentHearts = newHearts;
			}
			sotnApi.AlucardApi.Gold = newGold;

			string item = "";

			int itemCategory = 0;
			itemCategory = rng.Next(1, 5);

			string merchantType = "";

			switch (itemCategory)
			{
				case 1:
					item = toolConfig.Khaos.merchantArmorRewards[rng.Next(0, toolConfig.Khaos.merchantArmorRewards.Length)];
					merchantType = "Armor";
					break;
				case 2:
					item = toolConfig.Khaos.merchantFoodRewards[rng.Next(0, toolConfig.Khaos.merchantFoodRewards.Length)];
					merchantType = "Food";
					break;
				case 3:
					item = toolConfig.Khaos.merchantItemRewards[rng.Next(0, toolConfig.Khaos.merchantItemRewards.Length)];
					merchantType = "Item";
					break;
				case 4:
					item = toolConfig.Khaos.merchantWeaponRewards[rng.Next(0, toolConfig.Khaos.merchantWeaponRewards.Length)];
					merchantType = "Weapon";
					break;
				default:
					break;
			}

			int numberOfItems = merchantLevel;
			if(buffLckNeutralActive)
			{
				numberOfItems += 1;
			}
			if (superBuffLckNeutral)
			{
				numberOfItems += 1;
			}
				
			for (int i = 0; i < numberOfItems; i++)
			{
				int rolls = 0;
				
				while (sotnApi.AlucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
				{
					switch (itemCategory)
					{
						case 1:
							item = toolConfig.Khaos.merchantArmorRewards[rng.Next(0, toolConfig.Khaos.merchantArmorRewards.Length)];
							break;
						case 2:
							item = toolConfig.Khaos.merchantFoodRewards[rng.Next(0, toolConfig.Khaos.merchantFoodRewards.Length)];
							rolls = 99;
							break;
						case 3:
							item = toolConfig.Khaos.merchantItemRewards[rng.Next(0, toolConfig.Khaos.merchantItemRewards.Length)];
							rolls = 99;
							break;
						case 4:
							item = toolConfig.Khaos.merchantWeaponRewards[rng.Next(0, toolConfig.Khaos.merchantWeaponRewards.Length)];
							break;
						default:
							break;
					}
					
					rolls++;
				}
				string message = $"{user} {KhaosActionNames.Merchant} rolled: merchantType={merchantType}, item={item}";
				Console.WriteLine(message);
				message = $"{user}: 1 {item}";
				notificationService.AddMessage(message);
				sotnApi.AlucardApi.GrantItemByName(item);
			}
				
			//notificationService.AddMessage($"You gave {HPTaken}HP/{MPTaken}MP/{HeartsTaken}H/{GoldTaken}GP");
			//notificationService.AddMessage($"{user} used {merchantType} {KhaosActionNames.Merchant}");
			Alert(KhaosActionNames.Merchant);

			merchantLevel = UpdateNeutralLevel(merchantLevel);
		}
		public void MaxMayhem(string user = "Mayhem")
		{
			//alucardApi.Armor = (uint)Equipment.Items.IndexOf("Axe Lord armor");
			//Equipment.Items.IndexOf("Axe Lord armor");
			sotnApi.AlucardApi.GrantItemByName("Axe Lord armor");

			maxMayhemLevel = checkLockedNeutralLevel(maxMayhemLevel);

			int dynamiteMax = maxMayhemLevel * 3;

			for (int i = 0; i < dynamiteMax; i++)
			{
				sotnApi.AlucardApi.GrantItemByName("Dynamite");
			}

			uint currentHP = sotnApi.AlucardApi.CurrentHp;
			uint currentMP = sotnApi.AlucardApi.CurrentMp;
			uint currentHearts = sotnApi.AlucardApi.CurrentHearts;
			uint currentGold = sotnApi.AlucardApi.Gold;

			double basePercentage = 10 * maxMayhemLevel;
			double percentageHP = (rng.Next(1, 10) + basePercentage) / 100;
			double percentageMP = (rng.Next(1, 10) + basePercentage) / 100;
			double percentageHearts = (rng.Next(1, 10) + basePercentage) / 100;
			double percentageGold = (rng.Next(1, 10) + basePercentage) / 100;

			if (mpLocked)
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

			uint newHp = (uint) Math.Ceiling(sotnApi.AlucardApi.CurrentHp * percentageHP);

			sotnApi.AlucardApi.CurrentHp = newHp < 1? (uint) 1 : newHp;
			sotnApi.AlucardApi.CurrentMp = (uint) Math.Ceiling(sotnApi.AlucardApi.CurrentMp * percentageHP);
			sotnApi.AlucardApi.CurrentHearts = (uint) Math.Ceiling(sotnApi.AlucardApi.CurrentHearts * percentageHearts);


			sotnApi.AlucardApi.Gold = (uint) Math.Ceiling(sotnApi.AlucardApi.Gold * percentageGold);
			GainMayhemMeter(100);
			UpdatePlayerColor();
			UpdateVisualEffect();

			notificationService.AddMessage($"{user} used {KhaosActionNames.MaxMayhem}({maxMayhemLevel})");
			notificationService.AddMessage($"{user}: {dynamiteMax} Dynamite");

			maxMayhemLevel = UpdateNeutralLevel(maxMayhemLevel);

			Alert(KhaosActionNames.MaxMayhem);
		}
		public void UpdatePlayerColor()
		{
			int color;

			if (sotnApi.GameApi.InAlucardMode())
			{
				color = alucardColor;
				bool useFirstCastleColor = false;
				bool useSecondCastleColor = false;
				int roll = rng.Next(1, 4);
				if (roll == 1)
				{
					if (alucardSecondCastle)
					{
						useSecondCastleColor = true;
					}
					else
					{
						useFirstCastleColor = true;
					}
				}

				isAlucardColorFirstCastle = alucardSecondCastle == true ? false : true;
				isAlucardColorSecondCastle = alucardSecondCastle == true ? true: false;

				if (useFirstCastleColor) {
					while (color == alucardColor || previousAlucardColors.Contains(color))
					{
						color = Constants.Khaos.alucardColorsFirstCastle[rng.Next(0, Constants.Khaos.alucardColorsFirstCastle.Length)];
					}
				}
				else if (useSecondCastleColor){
					while (color == alucardColor || previousAlucardColors.Contains(color))
					{
						color = Constants.Khaos.alucardColorsSecondCastle[rng.Next(0, Constants.Khaos.alucardColorsSecondCastle.Length)];
					}
				}
				else
				{
					while (color == alucardColor || previousAlucardColors.Contains(color))
					{
						color = Constants.Khaos.alucardColors[rng.Next(0, Constants.Khaos.alucardColors.Length)];
					}
				}

				alucardColor = color;
				previousAlucardColors.Add(color);

				if (previousAlucardColors.Count > maxPreviousAlucardColors)
				{
					previousAlucardColors.RemoveAt(0);
				}

				Console.WriteLine($"New Alucard Color:{alucardColor}");
			}
			else
			{
				color = Constants.Khaos.richterColors[rng.Next(0, Constants.Khaos.richterColors.Length)];
				richterColor = color;

				Console.WriteLine($"New Richter Color:{richterColor}");
			}
		}
		public void HeartsOnly(string user = "Mayhem")
		{
			heartsLocked = true;
			mpLocked = true;
			weaponsLocked = true;

			heartsOnlyLevel = checkLockedNeutralLevel(heartsOnlyLevel);

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
					//if (Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Staurolite" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Staurolite")
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

			sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
			sotnApi.AlucardApi.Subweapon = (Subweapon) roll;
			sotnApi.AlucardApi.GrantRelic(Relic.CubeOfZoe, true);
			if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
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
				Type = Enums.ActionType.Neutral,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeartsOnly).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.HeartsOnly);

			heartsOnlyLevel = UpdateNeutralLevel(heartsOnlyLevel);
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
			sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
			hearts.Disable();
			if (gasCloudTaken && !unarmedActive)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.GasCloud, true);
				gasCloudTaken = false;
			}
			heartsLocked = false;
			mpLocked = false;
			heartsOnlyActive = false;
			heartsOnlyTimer.Stop();

		}
		public void Unarmed(string user = "Mayhem")
		{
			// 1 Usage: Unarmed STR + 10, Leap Stone, Remove all Booster Subrelics, Heart Lock, Subweapon Lock,
			// 2 Usage: All Previous + Unarmed Strength + 20, Unarmed Con +200
			// 3 Usage: All Previous + Unarmed Strength + 30, Extended Range + I-Frames(See Guilty Gear), Reset to 1.

			heartsLocked = true;
			weaponsLocked = true;
			weaponQualitiesLocked = true;
			subWeaponsLocked = true;
			unarmedActive = true;
			grantedTempFlight = false;

			unarmedLevel = checkLockedNeutralLevel(unarmedLevel);

			sotnApi.AlucardApi.GrantRelic(Relic.LeapStone, true);
			RemoveUnarmedRelics();

			if (sotnApi.AlucardApi.Subweapon != 0)
			{
				subWeaponTaken = (uint) sotnApi.AlucardApi.Subweapon;
				sotnApi.AlucardApi.Subweapon = 0;
			}

			sotnApi.AlucardApi.Subweapon = 0;

			uint tempStrength = (uint)(Constants.Khaos.UnarmedStr * unarmedLevel);
			addSTRGiven(tempStrength);

			if (unarmedLevel > 1)
			{
				uint tempCon = (uint) (Constants.Khaos.UnarmedCon * unarmedLevel);
				//sotnApi.AlucardApi.DefencePotionTimer = Constants.Khaos.UnarmedDefense;

				if (unarmedLevel > 2)
				{
					sotnApi.AlucardApi.InvincibilityTimer = Constants.Khaos.UnarmedInvincibility;
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
				Type = Enums.ActionType.Neutral,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Unarmed).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.Unarmed);

			unarmedLevel = UpdateNeutralLevel(unarmedLevel);
		}
		private void RemoveUnarmedRelics()
		{
			if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SoulOfBat);
				soulOfBatTaken = true;
				grantedTempFlight = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SoulOfWolf);
				soulOfWolfTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.PowerOfMist);
				powerOfMistTaken = true;
				if (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
				{
					grantedTempFlight = true;
				}
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
				gasCloudTaken = true;
			}
			if (grantedTempFlight && !sotnApi.AlucardApi.HasRelic(Relic.GravityBoots))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.GravityBoots, true);
			}
			else
			{
				grantedTempFlight = false;
			}
		}
		private void ReturnUnarmedRelics()
		{
			if (soulOfBatTaken)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfBat, true);
				soulOfBatTaken = false;
			}
			if (soulOfWolfTaken)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfWolf, true);
				soulOfWolfTaken = false;
			}
			if (powerOfMistTaken)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.PowerOfMist, true);
				powerOfMistTaken = false;
			}
			if (gasCloudTaken && (!heartsOnlyActive))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.GasCloud, true);
				gasCloudTaken = false;
			}
			if (grantedTempFlight)
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GravityBoots);
				grantedTempFlight = false;
			}
		}

		private void UnarmedOff(Object sender, EventArgs e)
		{

			if (unarmedLevel == 1)
			{
				hitboxWidth.Disable();
				hitboxHeight.Disable();
				hitbox2Height.Disable();
				hitbox2Width.Disable();
			}

			int prevLevel = unarmedLevel == 0 ? 3 : unarmedLevel - 1;
			uint strRemoved = (uint)(prevLevel * Constants.Khaos.UnarmedStr);
			if(strGiven < strRemoved)
			{
				strRemoved = strGiven;
			}

			takeSTRGiven(strRemoved);
			ReturnUnarmedRelics();

			if (subWeaponTaken != 0)
			{
				sotnApi.AlucardApi.Subweapon = (Subweapon) subWeaponTaken;
				subWeaponTaken = 0;
			}
			heartsLocked = false;
			subWeaponsLocked = false;
			weaponQualitiesLocked = false;
			weaponsLocked = false;
			unarmedActive = false;
			unarmedTimer.Stop();
		}

		public void TurboMode(string user = "Mayhem")
		{
			// Remove leapstone if not already obtained?

			if (turboModeActive)
			{
				queuedActions.Add(new QueuedAction { Name = "Turbo Name", Type = ActionType.Neutral, Invoker = new MethodInvoker(() => TurboMode(user)) });
				return;
			}

			turboModeLevel = checkLockedNeutralLevel(turboModeLevel);

			turboModeActive = true;
			sotnApi.AlucardApi.GrantRelic(Relic.LeapStone, true);
			turboMode.Enable();
			turboModeJump.Enable();
			turboModeTimer.Start();

			if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
			{
				if (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
				{
					grantedTempFlight = true;
				}
			}

			if (grantedTempFlight && !sotnApi.AlucardApi.HasRelic(Relic.GravityBoots))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.GravityBoots);
			}
			else
			{
				grantedTempFlight = false;
			}

			if (turboModeLevel == 1)
			{
				accelTime.PokeValue(0);
				accelTime.Enable();
				accelTimeActive = true;
				
			}
			else
			{
				accelTime.PokeValue(5);
				accelTime.Enable();
				accelTimeActive = false;
				
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
				Type = Enums.ActionType.Neutral,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TurboMode).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.TurboMode);
			
			turboModeLevel = UpdateNeutralLevel(turboModeLevel);
		}

		private void TurboModeOff(Object sender, EventArgs e)
		{
			attackPotionCheat.Disable();
			if (grantedTempFlight)
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GravityBoots);
				grantedTempFlight = false;
			}
			accelTime.PokeValue(5);
			turboMode.Disable();
			turboModeJump.Disable();
			turboModeTimer.Stop();
			accelTimeActive = false;
			turboModeActive = false;
		}


		public void RushDown(string user = "Mayhem")
		{
			alucardEffectsLocked = true;
			speedLocked = true;
			weaponsLocked = true;
			rushDownActive = true;
			invincibilityLocked = true;
			mpLocked = true;

			manaCheat.PokeValue(20);
			manaCheat.Enable();

			curse.Enable();
			// Bible Effect = 002B = 43.
			alucardEffect.PokeValue(43);
			alucardEffect.Enable();
			if (sotnApi.AlucardApi.Subweapon == 0 || sotnApi.AlucardApi.Subweapon == (Subweapon) 6)
			{
				sotnApi.AlucardApi.Subweapon = (Subweapon) 5;
			}

			sotnApi.AlucardApi.ActivatePotion(Potion.ResistStone);

			if (rushDownLevel == 1)
			{
				sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
				//sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
				//sotnApi.AlucardApi.DefencePotionTimer = Constants.Khaos.RushdownDefense;
				addCONGiven(Constants.Khaos.Rushdown1Con);
				
				if (alucardSecondCastle)
				{
					contactDamage.PokeValue(150);
				}
				else
				{
					contactDamage.PokeValue(100);
				}
				contactDamage.Enable();
			}
			else
			{
				shineCheat.PokeValue(1);
				shineCheat.Enable();
				if (rushDownLevel == 2)
				{
					addCONGiven(Constants.Khaos.Rushdown2Con);
					if (alucardSecondCastle)
					{
						contactDamage.PokeValue(22);
					}
					else
					{
						contactDamage.PokeValue(15);
					}
					
					contactDamage.Enable();
				}
				else
				{
					invincibilityCheat.PokeValue(1);
					invincibilityCheat.Enable();
					if (alucardSecondCastle)
					{
						contactDamage.PokeValue(45);
					}
					else
					{
						contactDamage.PokeValue(30);
					}
						
					contactDamage.Enable();
				}
			}
			rushDownTimer.Start();

			string name = $"{KhaosActionNames.RushDown}({rushDownLevel})";
			notificationService.AddMessage($"{user} used {name}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Neutral,
				Duration = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RushDown).FirstOrDefault().Duration
			});
			Alert(KhaosActionNames.RushDown);

			rushDownLevel = UpdateNeutralLevel(rushDownLevel);
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
		private void RushDownOff(object sender, EventArgs e)
		{
			
			if (rushDownLevel != 2)
			{
				invincibilityCheat.Disable();
			}

			if (rushDownLevel == 2)
			{
				takeCONGiven(Constants.Khaos.Rushdown1Con);
			}

			if (rushDownLevel == 3)
			{
				takeCONGiven(Constants.Khaos.Rushdown2Con);
			}

			manaCheat.Disable();
			sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
			mpLocked = false;
			alucardEffect.Disable();
			disableCurse(2);
			shineCheat.Disable();
			contactDamage.Disable();
			sotnApi.AlucardApi.ContactDamage = 0;
			SetSpeed();
			rushDownTimer.Stop();
			invincibilityLocked = false;
			heartsLocked = false;
			speedLocked = false;
			alucardEffectsLocked = false;
			rushDownActive = false;
		}
		public void SwapStats(string user = "Mayhem")
		{
			RandomizeStatsActivate();
			short meter = NeutralMeterGain(swapStatsLevel);
			GainMayhemMeter(meter);
			notificationService.AddMessage($"{user} used {KhaosActionNames.SwapStats}({swapStatsLevel})");
			Alert(KhaosActionNames.SwapStats);

			swapStatsLevel = UpdateNeutralLevel(swapStatsLevel);
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
				//RandomizeInventory();
			}

			notificationService.AddMessage($"{user} used {KhaosActionNames.SwapEquipment}({swapEquipmentLevel})");
			Alert(KhaosActionNames.SwapEquipment);

			swapEquipmentLevel = UpdateNeutralLevel(swapEquipmentLevel);
		}
		public void SwapRelics(string user = "Mayhem")
		{
			RandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			short meter = NeutralMeterGain(swapRelicsLevel);
			GainMayhemMeter(meter);

			notificationService.AddMessage($"{user} used {KhaosActionNames.SwapRelics}({swapRelicsLevel})");
			Alert(KhaosActionNames.SwapRelics);

			swapRelicsLevel = UpdateNeutralLevel(swapRelicsLevel);
		}
		public void Pandemonium(string user = "Mayhem")
		{
			RandomizeGold();
			RandomizeStatsActivate();
			RandomizeEquipmentSlots();
			RandomizeInventory();
			if (!toolConfig.Khaos.RestrictedRelicSwap)
			{
				RandomizeRelicsActivate(!toolConfig.Khaos.KeepVladRelics);
			}
			RandomizeSubweapon();
			sotnApi.GameApi.RespawnBosses();
			sotnApi.GameApi.RespawnItems();
			UpdatePlayerColor();
			UpdateVisualEffect();
			notificationService.AddMessage($"{user} caused Pandemonium({pandemoniumLevel})");
			Alert(KhaosActionNames.Pandemonium);

			if (pandemoniumLevel >= 2)
			{
				RandomizePotion(user);
			}
			if (pandemoniumLevel == 3)
			{
				GainMayhemMeter(100);
			}

			pandemoniumLevel = UpdateNeutralLevel(pandemoniumLevel);
		}
		private void RandomizeGold()
		{
			uint gold = (uint) rng.Next(50, 5000);
			uint roll = (uint) rng.Next(0, 21);
			if (roll > 16 && roll < 20)
			{
				gold *= (uint) rng.Next(10, 21);
			}
			else if (roll > 19)
			{
				gold *= (uint) rng.Next(21, 42);
			}
			else
			{
				gold += (uint) 505;
			}
			sotnApi.AlucardApi.Gold = gold;
		}
		private void RandomizeStatsActivate()
		{
			uint maxHp = sotnApi.AlucardApi.MaxtHp;
			uint currentHp = sotnApi.AlucardApi.CurrentHp;
			uint maxMana = sotnApi.AlucardApi.MaxtMp;
			uint currentMana = sotnApi.AlucardApi.CurrentMp;
			uint maxHearts = sotnApi.AlucardApi.MaxtHearts;
			uint currentHearts = sotnApi.AlucardApi.CurrentHearts;

			uint tempHPRemoved = 0;
			uint tempStrRemoved = 0;
			uint tempConRemoved = 0;
			uint tempIntRemoved = 0;
			uint tempLckRemoved = 0;

			uint str = sotnApi.AlucardApi.Str;
			uint con = sotnApi.AlucardApi.Con;
			uint intel = sotnApi.AlucardApi.Int;
			uint lck = sotnApi.AlucardApi.Lck;

			bool statsPaused = givenStatsPaused;
			
			if (!statsPaused)
			{
				tempHPRemoved = hpGiven;
				tempStrRemoved = strGiven;
				tempConRemoved = conGiven;
				tempIntRemoved = intGiven;
				tempLckRemoved = lckGiven;

				maxHp -= tempHPRemoved;
				str -= tempStrRemoved;
				con -= tempConRemoved;
				intel -= tempIntRemoved;
				lck -= tempLckRemoved;
			}

			//Stat sanity check.

			str = str > Constants.Khaos.StatOverflowLimit ? minStr : str;
			con = con > Constants.Khaos.StatOverflowLimit ? minCon : con;
			intel = intel > Constants.Khaos.StatOverflowLimit ? minInt : intel;
			lck = lck > Constants.Khaos.StatOverflowLimit ? minLck : lck;

			Console.WriteLine($"RandomizeStats: initialStats: HP={maxHp};STR={str};CON={con};INT={intel};LCK={lck};");
			Console.WriteLine($"RandomizeStats: givenStats(Pause = {statsPaused}): HP={hpGiven};STR={strGiven};CON={conGiven};INT={intGiven};LCK={lckGiven};");
			Console.WriteLine($"RandomizeStats: removedStats: HP={tempHPRemoved};STR={tempStrRemoved};CON={tempConRemoved};INT={tempIntRemoved};LCK={tempLckRemoved};");

			//Zig - Increase stat pool check to compensate for higher minimum stats



			uint statPool = str + con + intel + lck > 28 ? str + con + intel + lck - 28 : 28;
			uint offset = (uint) ((rng.NextDouble() / 4) * statPool);

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
			sotnApi.AlucardApi.Str = (uint) (7 + newStr);
			sotnApi.AlucardApi.Con = (uint) (7 + newCon);
			sotnApi.AlucardApi.Int = (uint) (7 + newInt);
			sotnApi.AlucardApi.Lck = (uint) (7 + newLck);

			//Zig - Rounding Fix
			uint CalculatedStatPool = (uint) 28 + statPool;
			uint ActualStatPool = (uint) 28 + newStr + newCon + newInt + newLck;
			if (ActualStatPool < CalculatedStatPool)
			{
				uint roundingOffset = (uint)(CalculatedStatPool > ActualStatPool ? CalculatedStatPool - ActualStatPool : 0);
				int roundPoolRoll = rng.Next(1, 5);
				if (roundPoolRoll == 1)
				{
					sotnApi.AlucardApi.Str += roundingOffset;
				}
				else if (roundPoolRoll == 2)
				{
					sotnApi.AlucardApi.Con += roundingOffset;
				}
				else if (roundPoolRoll == 3)
				{
					sotnApi.AlucardApi.Int += roundingOffset;
				}
				else
				{
					sotnApi.AlucardApi.Lck += roundingOffset;
				}
			}

			//Re-add any temp removed stats;
			sotnApi.AlucardApi.Str += tempStrRemoved;
			sotnApi.AlucardApi.Con += tempConRemoved;
			sotnApi.AlucardApi.Int += tempIntRemoved;
			sotnApi.AlucardApi.Lck += tempLckRemoved;
			
			uint minPoints = (uint) 110;
			uint pointsPool = maxHp + maxMana > minPoints ? maxHp + maxMana - minPoints : maxHp + maxMana;

			Console.WriteLine($"RandomizeStats: HPMPPool({pointsPool})={maxHp}+{maxMana}+/-{minPoints}; StatPool({statPool})={str}+{con}+{intel}+{lck}+/-{offset}, -28; {newStr},{newCon},{newInt},{newLck};");

			if (maxHp + maxMana < minPoints)
			{
				pointsPool = minPoints;
			}
			offset = (uint) ((rng.NextDouble() / 4) * pointsPool);

			int pointsRoll = rng.Next(1, 4);
			if (pointsRoll == 2)
			{
				pointsPool += offset;
			}
			else if (pointsRoll == 3)
			{
				pointsPool -= offset;
			}

			double hpPercent = rng.NextDouble();
			uint pointsHp = (uint)(80 + Math.Round(hpPercent * pointsPool));
			uint pointsMp = (uint)(30 + pointsPool - Math.Round(hpPercent * pointsPool));

			//Add temp removed HP
			pointsHp += tempHPRemoved;
			
			if (currentHp > pointsHp)
			{
				sotnApi.AlucardApi.CurrentHp = pointsHp;
			}
			if (currentMana > pointsMp)
			{
				sotnApi.AlucardApi.CurrentMp = pointsMp;
			}
			if (heartsLocked)
			{
				Console.WriteLine("RandomizeStats: Skipping Hearts re-roll due to hearts lock.");
			}
			else
			{
				pointsPool = maxHearts;
				offset = (uint) ((rng.NextDouble() / 4) * pointsPool);
				pointsRoll = rng.Next(1, 3);
				if (pointsRoll == 1)
				{
					pointsPool +=offset;
				}
				else if (pointsRoll == 2)
				{
					pointsPool -= offset;
				}
				if (pointsPool < minHearts)
				{
					pointsPool = minHearts;
				}
				sotnApi.AlucardApi.CurrentHearts = pointsPool;
				sotnApi.AlucardApi.MaxtHearts = pointsPool;
			}

			sotnApi.AlucardApi.MaxtHp = pointsHp;
			sotnApi.AlucardApi.MaxtMp = pointsMp;

		}
		private void RandomizeInventory()
		{
			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring");
			bool hasLibraryCard = sotnApi.AlucardApi.HasItemInInventory("Library card");

			sotnApi.AlucardApi.ClearInventory();

			int itemCount = rng.Next(toolConfig.Khaos.PandemoniumMinItems, toolConfig.Khaos.PandemoniumMaxItems + 1);
			if (superBuffLckNeutral)
			{
				itemCount++;
			}
			else
			{
				itemCount++;
			}

			for (int i = 0; i < itemCount; i++)
			{
				int result = rng.Next(0, Equipment.Items.Count);
				sotnApi.AlucardApi.GrantItemByName(Equipment.Items[result]);
			}

			if (hasHolyGlasses)
			{
				sotnApi.AlucardApi.GrantItemByName("Holy glasses");
			}
			if (hasSpikeBreaker)
			{
				sotnApi.AlucardApi.GrantItemByName("Spike Breaker");
			}
			if (hasGoldRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Gold Ring");
			}
			if (hasSilverRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Silver Ring");
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
				sotnApi.AlucardApi.Subweapon = 0;
			}
			else
			{
				sotnApi.AlucardApi.Subweapon = (Subweapon) subweapons.GetValue(rng.Next(subweapons.Length));
			}
		}
		private void RandomizeRelicsActivate(bool randomizeVladRelics = true)
		{
			Array? relics = Enum.GetValues(typeof(Relic));
			foreach (object? relic in relics)
			{
				if ((int) relic < 25)
				{
					sotnApi.AlucardApi.GrantRelic((Relic) relic, true);
				}
				int roll = rng.Next(0, 2);
				if (roll > 0)
				{
					if ((int) relic < 25)
					{
						sotnApi.AlucardApi.GrantRelic((Relic) relic, true);
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
					sotnApi.AlucardApi.GrantRelic((Relic) relic, true);
				}
			}

			if (IsInRoomList(Constants.Khaos.SwitchRoom))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.JewelOfOpen, true);
			}

			if (IsInRoomList(Constants.Khaos.MistGateRoom))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.FormOfMist, true);
			}

			sotnApi.AlucardApi.GrantItemByName("Library card");
			//notificationService.AddMessage($"{user}: 1 Library Card)");


			if (buffLckNeutralActive)
			{
				sotnApi.AlucardApi.GrantItemByName("Ring of Arcana");
				//notificationService.AddMessage($"{user}: 1 Ring of Arcana)");
			}

		}
		private void RandomizeEquipmentSlots()
		{
			bool equippedBuggyQuickSwapWeaponRight = Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)]);
			bool equippedBuggyQuickSwapWeaponLeft = Constants.Khaos.BuggyQuickSwapWeapons.Contains(Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)]);
			bool equippedHolyGlasses = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";
			bool equippedLibraryCard = Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Library card" || Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Library card";

			//Console.WriteLine("Hand Values: ");
			//Console.WriteLine(sotnApi.AlucardApi.LeftHand);
			//Console.WriteLine(sotnApi.AlucardApi.RightHand);

			uint newRightHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			uint newLeftHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			uint newHelm = Equipment.HelmStart + (uint) rng.Next(0, Equipment.HelmCount + 1);
			uint newArmor = (uint) rng.Next(0, Equipment.ArmorCount + 1);
			uint newCloak = Equipment.CloakStart + (uint) rng.Next(0, Equipment.CloakCount + 1);
			uint newAccessory1 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);
			uint newAccessory2 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);

			//Reroll the new item if it is progression.
			while (Equipment.Items[(int) (newHelm + Equipment.HandCount + 1)] == "Holy glasses")
			{
				newHelm = Equipment.HelmStart + (uint) rng.Next(0, Equipment.HelmCount + 1);
			}
			while (Equipment.Items[(int) (newArmor + Equipment.HandCount + 1)] == "Spike Breaker") 
			{
				newArmor = (uint) rng.Next(0, Equipment.ArmorCount + 1);
			}
			while (Equipment.Items[(int) (newAccessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (newAccessory1)] == "Gold Ring")
			{
				newAccessory1 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);
			}
			while (Equipment.Items[(int) (newAccessory2 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (newAccessory2)] == "Gold Ring")
			{
				newAccessory2 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);
			}

			Console.WriteLine($"Swap Equipment: {newHelm} = {Equipment.Items[(int) (newHelm + Equipment.HandCount + 1)]},{Equipment.HelmStart}; Compare: {Equipment.Items.IndexOf("Holy glasses")} - {Equipment.HandCount} - 1");
			Console.WriteLine($"Swap Equipment: {newArmor} = {Equipment.Items[(int) (newArmor + Equipment.HandCount + 1)]}; Compare: {Equipment.Items.IndexOf("Spike Breaker")} - {Equipment.HandCount} - 1");
			Console.WriteLine($"Swap Equipment: {newAccessory1} = {Equipment.Items[(int) (newAccessory1 + Equipment.HandCount + 1)]},{Equipment.AccessoryStart}; Compare: {Equipment.Items.IndexOf("Silver Ring")} - {Equipment.HandCount} - 1");
			Console.WriteLine($"Swap Equipment: {newAccessory2} = {Equipment.Items[(int) (newAccessory2 + Equipment.HandCount + 1)]},{Equipment.AccessoryStart}; Compare: {Equipment.Items.IndexOf("Gold Ring")} - {Equipment.HandCount} - 1");



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

			sotnApi.AlucardApi.RightHand = newRightHand;
			sotnApi.AlucardApi.LeftHand = newLeftHand;
			sotnApi.AlucardApi.Armor = newArmor;
			sotnApi.AlucardApi.Helm = newHelm;
			sotnApi.AlucardApi.Cloak = newCloak;
			sotnApi.AlucardApi.Accessory1 = newAccessory1;
			sotnApi.AlucardApi.Accessory2 = newAccessory2;

			RandomizeSubweapon();
			if (heartsOnlyActive)
			{
				while (sotnApi.AlucardApi.Subweapon == Subweapon.Empty || sotnApi.AlucardApi.Subweapon == Subweapon.Stopwatch)
				{
					RandomizeSubweapon();
				}
			}

			if (equippedHolyGlasses)
			{
				sotnApi.AlucardApi.GrantItemByName("Holy glasses");
			}
			if (equippedSpikeBreaker)
			{
				sotnApi.AlucardApi.GrantItemByName("Spike Breaker");
			}
			if (equippedGoldRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Gold Ring");
			}
			if (equippedSilverRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Silver Ring");
			}
			if (equippedLibraryCard)
			{
				sotnApi.AlucardApi.GrantItemByName("Library card");
			}
			if (buffLckNeutralActive)
			{
				sotnApi.AlucardApi.GrantItemByName("Ring of Arcana");
			}
		}

		public void RandomizePotion(string user = "Mayhem")
		{
			bool highMp = sotnApi.AlucardApi.CurrentMp > sotnApi.AlucardApi.MaxtMp * 0.6;
			bool highHp = sotnApi.AlucardApi.CurrentHp > sotnApi.AlucardApi.MaxtHp * 0.6;
			int min = 2;
			int max = 10;
			uint baseGain = 5 + sotnApi.AlucardApi.Level;
			uint addGold = 0;

			if (hasHolyGlasses)
			{
				max = 12;
			}


			int result = rng.Next(min, max);

			if (timeStopActive)
			{
				addGold = 50 + (uint) (10 * baseGain * (rng.NextDouble()));
				result = 1;
			}
			else {
				if (mpLocked || highMp)
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
					notificationService.AddMessage($"{user} gave you extra gold {addGold}");
					break;
				case 2:
					sotnApi.AlucardApi.ActivatePotion(Potion.Mannaprism);
					notificationService.AddMessage($"{user} gave you max MP");
					break;
				case 3:
					sotnApi.AlucardApi.ActivatePotion(Potion.Potion);
					notificationService.AddMessage($"{user} gave Potion");
					break;
				case 4:
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					notificationService.AddMessage($"{user} gave Smart potion");
					break;
				case 5:
					sotnApi.AlucardApi.ActivatePotion(Potion.LuckPotion);
					notificationService.AddMessage($"{user} gave Luck potion");
					break;
				case 6:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistFire);
					notificationService.AddMessage($"{user} gave Resist Fire");
					break;
				case 7:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistThunder);
					notificationService.AddMessage($"{user} gave Resist Thunder");
					break;
				case 8:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistDark);
					notificationService.AddMessage($"{user} gave Resist Dark");
					break;
				case 9:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave Shield Potion");
					break;
				case 10:
				case 11:
				default:
					break;
			}
		}
		public void RandomizeModeratePotion(string user = "Mayhem")
		{
			bool highMp = sotnApi.AlucardApi.CurrentMp > sotnApi.AlucardApi.MaxtMp * 0.6;
			bool highHp = sotnApi.AlucardApi.CurrentHp > sotnApi.AlucardApi.MaxtHp * 0.6;
			bool highHearts = sotnApi.AlucardApi.CurrentHearts > sotnApi.AlucardApi.MaxtHearts * 0.6;
			int min = 2;
			int max = 10;
			uint baseGain = (uint)((5 + sotnApi.AlucardApi.Level)*2);
			uint addGold = 0;

			int result = rng.Next(min, max);

			if (timeStopActive)
			{
				addGold = 100 + (uint) (10 * baseGain * (rng.NextDouble()));
				result = 1;
			}
			else
			{
				if (mpLocked || highMp)
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
					notificationService.AddMessage($"{user} gave extra gold {sotnApi.AlucardApi.Gold}");
					break;
				case 2:
					sotnApi.AlucardApi.ActivatePotion(Potion.Mannaprism);
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					notificationService.AddMessage($"{user} gave max MP + Int");
					break;
				case 3:
					sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts;
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					notificationService.AddMessage($"{user} gave max Hearts + Int");
					break;
				case 4:
					sotnApi.AlucardApi.ActivatePotion(Potion.HighPotion);
					notificationService.AddMessage($"{user} gave High Potion");
					break;
				case 5:
					sotnApi.AlucardApi.ActivatePotion(Potion.LuckPotion);
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					notificationService.AddMessage($"{user} gave Int/Lck potion");
					break;
				case 6:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistFire);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistThunder);
					notificationService.AddMessage($"{user} gave Resist Fire/Thunder");
					break;
				case 7:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistIce);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistHoly);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistDark);
					notificationService.AddMessage($"{user} gave Resist Ice/Holy/Dark");
					break;
				case 8:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					notificationService.AddMessage($"{user} gave Shield Potion");
					break;
				default:
					break;
			}
		}

		public void RandomizeMajorReward(string user = "Mayhem")
		{
			bool highMp = sotnApi.AlucardApi.CurrentMp > sotnApi.AlucardApi.MaxtMp * 0.6;
			bool highHp = sotnApi.AlucardApi.CurrentHp > sotnApi.AlucardApi.MaxtHp * 0.6;
			bool highHearts = sotnApi.AlucardApi.CurrentHearts > sotnApi.AlucardApi.MaxtHearts * 0.6;
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
					if (!mpLocked)
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
					sotnApi.AlucardApi.MaxtHp += 15;
					sotnApi.AlucardApi.MaxtMp += 15;
					sotnApi.AlucardApi.MaxtHearts += 20;
					notificationService.AddMessage($"{user} gave you bonus HP/MP/Hearts");
					break;
				case 3:
					sotnApi.AlucardApi.Str += 2;
					sotnApi.AlucardApi.Con += 2;
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
					sotnApi.AlucardApi.Int += 1;
					sotnApi.AlucardApi.Lck += 1;
					notificationService.AddMessage($"{user} gave you +1 to all stats");
					break;
				default:
					break;
			}
		}

		#endregion
		#region Curse Effects

		public System.TimeSpan CurseDurationGain(System.TimeSpan baseTimeSpan)
		{
			double timeFactor = 1.0 + (.05 * vladRelicsObtained * toolConfig.Khaos.CurseModifier);
			int newDurationSeconds = (int) Math.Floor(baseTimeSpan.TotalSeconds * (timeFactor));
			int diffInSeconds = (int) Math.Floor(newDurationSeconds - baseTimeSpan.TotalSeconds);
			int diffInMinutes = (int) Math.Floor(diffInSeconds / 60.0);
			diffInSeconds = diffInSeconds % 60;
			TimeSpan newInterval = new TimeSpan(0, diffInMinutes, diffInSeconds);
			System.TimeSpan newTimeSpan = baseTimeSpan.Add(newInterval);
			//Console.WriteLine($"Bad Duration: baseTimeSpan{baseTimeSpan}, newDur {newDurationSeconds}, diffInSec {diffInSeconds}, diffInMinutes{diffInMinutes}, newTimeSpan{newTimeSpan}");
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
			if (mpLocked)
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
				sotnApi.AlucardApi.CurrentHp = lowHP;
			}

			double percentageHP = 5 / 100;
			double percentageMP = 5 / 100;
			double percentageHearts = 5 / 100;
			double percentageGold = 3 / 100;

			uint newHP = (uint) Math.Max(1, Math.Round(currentHp - (currentHp * percentageHP) - 5 - vladRelicsObtained - toolConfig.Khaos.CurseModifier - sotnApi.AlucardApi.Level));
			uint newMP = (uint) Math.Max(0, Math.Round(currentMp - (currentMp * percentageMP) - 5 - vladRelicsObtained - toolConfig.Khaos.CurseModifier -sotnApi.AlucardApi.Level));
			uint newHearts = (uint) Math.Max(0, Math.Round(currentHearts - (currentHearts * percentageHearts) - 5 - vladRelicsObtained - toolConfig.Khaos.CurseModifier - sotnApi.AlucardApi.Level));
			uint newGold = (uint) Math.Max(0, Math.Round(currentGold - (currentGold * percentageGold) - ((6 + vladRelicsObtained + toolConfig.Khaos.CurseModifier) * sotnApi.AlucardApi.Level)));

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
					sotnApi.AlucardApi.CurrentHearts = newHearts;
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
					if (newHP <= 2)
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
			if(result == 6)
			{
				Alert(KhaosActionNames.Slam);
			}
			else
			{
				Alert(KhaosActionNames.MinorTrap);
			}
		}
		public void HPForMP(string user = "Mayhem")
		{
			if (HPForMPActive) 
			{
				queuedActions.Add(new QueuedAction { Name = "HP For MP", LocksMana = true, Invoker = new MethodInvoker(() => HPForMP(user)) });
				return;
			}
			mpLocked = true;
			HPForMPActive = true;
			storedMp = sotnApi.AlucardApi.CurrentMp;
			storedMaxMp = sotnApi.AlucardApi.MaxtMp;

			System.TimeSpan newDuration = CurseDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault().Duration);

			HPForMPTimer.Interval = (int) newDuration.TotalMilliseconds;
			HPForMPTimer.Start();

			notificationService.AddMessage($"{user} used {KhaosActionNames.HPForMP}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.HPForMP,
				Type = Enums.ActionType.Curse,
				Duration = newDuration
			});
			Alert(KhaosActionNames.HPForMP);
		}
		private void HPForMPUpdate()
		{

			uint currentMaxMp = sotnApi.AlucardApi.MaxtMp;

			if (HPForMPActivePaused)
			{
				storedMaxMp = currentMaxMp;
				spentMp = 0;
				return;
			}
			else if (currentMaxMp < storedMaxMp)
			{
				storedMaxMp = currentMaxMp;
				return;
			}
			else if (spentMp > 0)
			{
				uint currentHp = sotnApi.AlucardApi.CurrentHp;
				if (currentHp > spentMp)
				{
					sotnApi.AlucardApi.CurrentHp -= (uint) spentMp;
					sotnApi.AlucardApi.CurrentMp += (uint) spentMp;
					//Console.WriteLine($"MP Taken");
				}
				else
				{
					sotnApi.AlucardApi.CurrentMp = 0;
					sotnApi.AlucardApi.CurrentHp = 0;
					sotnApi.AlucardApi.RightHand = (uint) Equipment.Items.IndexOf("Pizza");
					sotnApi.AlucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Pizza");
					hpForMPDeathTimer.Start();
				}
			}
		}

		private void HPForMPOff(Object sender, EventArgs e)
		{
			HPForMPTimer.Stop();
			mpLocked = false;
			HPForMPActive = false;
		}
		public void Underwater(string user = "Mayhem")
		{
			if (IsInRoomList(Constants.Khaos.EntranceCutsceneRooms) || IsInRoomList(Constants.Khaos.ClockRoom))
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Underwater, LocksSpeed = true, Invoker = new MethodInvoker(() => Underwater(user)) });
				return;
			}

			speedLocked = true;
			underwaterActive = true;
			string name = KhaosActionNames.Underwater;

			bool meterFull = MayhemMeterFull();
			float underwaterSuperFactor;
			underwaterMayhemFactor = 1F;

			if (toolConfig.Khaos.NerfUnderwater)
			{
				underwaterBaseFactor = 1F;
				underwaterSuperFactor = toolConfig.Khaos.UnderwaterFactor;
			}
			else
			{
				underwaterBaseFactor = toolConfig.Khaos.UnderwaterFactor;
				underwaterMayhemFactor = 1F;
				underwaterSuperFactor = Constants.Khaos.SuperUnderwaterFactor;
			}
			
			if (meterFull)
			{
				name = "Super " + name;
				underwaterMayhemFactor = Constants.Khaos.SuperUnderwaterFactor;
				SpendMayhemMeter();
			}
			sotnApi.AlucardApi.GrantRelic(Relic.HolySymbol, true);
			SetSpeed((float) underwaterBaseFactor * underwaterMayhemFactor);
			underwaterPhysics.PokeValue(144);
			underwaterPhysics.Enable();

			System.TimeSpan newDuration = CurseDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Underwater).FirstOrDefault().Duration);

			underwaterTimer.Interval = (int) newDuration.TotalMilliseconds;
			underwaterTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Curse,
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

		public void Slam(string user = "Mayhem", bool multiSlam = false, bool canSlamJam = false, bool forceSlamJam = false)
		{
			bool entranceCutscene = IsInRoomList(Constants.Khaos.EntranceCutsceneRooms);
			bool succubusRoom = IsInRoomList(Constants.Khaos.SuccubusRoom);
			if (succubusRoom || entranceCutscene)
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Slam, Invoker = new MethodInvoker(() => Slam(user, multiSlam, canSlamJam, forceSlamJam)) });
				return;
			}
			else
			{
				string message = $"{user}";

				if (multiSlam)
				{
					SpawnSlamHitbox(true);
					if (superSlamJam)
					{
						message += $": Super {KhaosActionNames.SlamJam}!";
					}
					else
					{
						message += $": {KhaosActionNames.SlamJam}!";
					}
					Alert(KhaosActionNames.Slam);
				}
				else
				{
					int roll = rng.Next(0, 8);
					bool meterFull = MayhemMeterFull();
					string name = $"{KhaosActionNames.SlamJam}";
					message += " used ";

					if (slamJamActive == true || canSlamJam == false)
					{
						roll = 8;
					}
					else if (forceSlamJam)
					{
						roll = 0;
					}
					else if (meterFull)
					{
						SpendMayhemMeter();
						roll = 0;
						message += "Super ";
						name = "Super " + name;
						superSlamJam = true;
					}
					if (roll == 0)
					{
						message += $"{KhaosActionNames.SlamJam}!";
						slamJamUser = user;
						slamJamActive = true;
						slamJamTickTimer.Start();

						System.TimeSpan newDuration = CurseDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SlamJam).FirstOrDefault().Duration);

						slamJamTimer.Interval = (int) newDuration.TotalMilliseconds;
						slamJamTimer.Start();

						notificationService.AddTimer(new Services.Models.ActionTimer
						{
							Name = name,
							Type = Enums.ActionType.Curse,
							Duration = newDuration
						});
						Alert(KhaosActionNames.SlamJam);
					}
					else
					{
						SpawnSlamHitbox();
						message += $"{KhaosActionNames.Slam}";
						Alert(KhaosActionNames.Slam);
					}
				}
				notificationService.AddMessage(message);	
			}
		}


		public void Trap(string user = "Mayhem")
		{
			int roll = rollCurseCommandCategory();

			switch (roll)
			{
				case 1:
					MinorTrap(user);
					break;
				case 2:
					ModerateTrap(user);
					break;
				case 3:
					MajorTrap();
					break;
			}
		}

		public int rollCurseCommandCategory()
		{
			int result = 1;
			int baseMinorThreshold = 14;
			int baseModerateThreshold = 5;
			int baseMajorThreshold = 1;

			int baseAdjustment = (int)(vladRelicsObtained * .5 * toolConfig.Khaos.CurseModifier);

			if (baseAdjustment > 0) 
			{
				baseMinorThreshold -= baseAdjustment;
				baseModerateThreshold = baseModerateThreshold - (baseAdjustment - baseAdjustment / 5);
				baseMajorThreshold += baseAdjustment / 5;
			}

			int minorThreshold = baseMinorThreshold;
			int moderateThreshold = minorThreshold + baseModerateThreshold;
			int majorThreshold = moderateThreshold + baseMajorThreshold;

			int roll = rng.Next(1, 21);
			if (roll <= minorThreshold)
			{
				result = 1;
			}
			else if (roll <= moderateThreshold)
			{
				result = 2;
			}
			else if (roll <= majorThreshold)
			{
				result = 3;
			}

			if (result == 3)
			{
				roll = 10;
				result = 2;
			}

			Console.WriteLine($"Rolling Curse Category: Roll={roll},Minor<={minorThreshold},Mod<={moderateThreshold},Major<={majorThreshold};Result={result}");
			return result;
		}


		public void ModerateTrap(string user = "Mayhem")
		{
			List<int> effectNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8};
			int min = 1;
			int max = effectNumbers.Count;

			if (heartsLocked)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 1);
			}
			if (mpLocked)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 2);
			}
			if (rushDownActive)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 5);
			}
			if (AutoMayhemOn)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 8);
			}

			//notificationService.AddMessage($"min{min} and max{max}");
			int result = effectNumbers[rng.Next(min, max)];
	
			//notificationService.AddMessage($"{result}: min{min} and max{max}");

			uint currentMaxHP = sotnApi.AlucardApi.CurrentHp;
			uint currentMaxMP = sotnApi.AlucardApi.CurrentMp;
			uint currentMaxHearts = sotnApi.AlucardApi.CurrentHearts;

			uint adjustedHP = 0;
			uint adjustedMaxHP = sotnApi.AlucardApi.CurrentHp;
			uint adjustedMinHP = minHP;
			uint adjustedMinMP = minMP;
			uint adjustedMinHearts = minHearts;
			uint adjustedMinStr = minStr;
			uint adjustedMinCon = minCon;
			uint adjustedMinInt = minInt;
			uint adjustedMinLck = minLck;

			bool statsPaused = givenStatsPaused;

			if (!statsPaused)
			{
				adjustedHP = hpGiven;
				adjustedMaxHP -= hpGiven;
				adjustedMinHP += hpGiven;
				adjustedMinStr = minStr;
				adjustedMinCon = minCon;
				adjustedMinInt = minInt;
				adjustedMinLck = minLck;
			}

			int baseRemoved = (int)(2 + (.5 * toolConfig.Khaos.CurseModifier));

			uint newStr = (uint)(sotnApi.AlucardApi.Str - baseRemoved);
			uint newCon = (uint)(sotnApi.AlucardApi.Con - baseRemoved);
			uint newInt = (uint)(sotnApi.AlucardApi.Int - baseRemoved);
			uint newLck = (uint)(sotnApi.AlucardApi.Lck - baseRemoved);

			newStr = newStr < adjustedMinStr ? adjustedMinStr : newStr;
			newCon = newCon < adjustedMinCon ? adjustedMinCon : newCon;
			newInt = newInt < adjustedMinInt ? adjustedMinInt : newInt;
			newLck = newLck < adjustedMinLck ? adjustedMinLck : newLck;

			double percentageHP = baseRemoved / 100;
			double percentageMP = baseRemoved / 100;
			double percentageHearts = baseRemoved / 100;

			uint newHP = (uint) Math.Max(minHP, Math.Round(adjustedMaxHP - (adjustedMaxHP * percentageHP) - 10 - baseRemoved)) + adjustedHP;
			uint newMP = (uint) Math.Max(minMP, Math.Round(currentMaxMP - (currentMaxMP * percentageMP) - 5 - baseRemoved));
			uint newHearts = (uint) Math.Max(minHearts, Math.Round(currentMaxHearts - (currentMaxHearts * percentageHearts) - 5 - baseRemoved));

			newHP = adjustedMinHP > newHP ? adjustedMinHP : newHP;
			newMP = adjustedMinMP > newMP ? adjustedMinMP : newMP;
			newHearts = adjustedMinHearts > newHearts ? adjustedMinHearts : newHearts;

			uint newLevel = 1;
			
			if(sotnApi.AlucardApi.Level + 1 > baseRemoved)
			{
				newLevel = (uint) (sotnApi.AlucardApi.Level + 1 - baseRemoved);
			}

			uint newExperience = 0;
			if (newLevel <= StatsValues.ExperienceValues.Length && newLevel > 1)
			{
				newExperience = (uint) StatsValues.ExperienceValues[(int) newLevel - 1];
			}
			else if (newLevel > 1)
			{
				newExperience = (uint) StatsValues.ExperienceValues[StatsValues.ExperienceValues.Length - 1];
			}
			if (newLevel >= 1)
			{
				sotnApi.AlucardApi.Level = newLevel;
				sotnApi.AlucardApi.Experiecne = newExperience;
			}


			switch (result)
			{
				case 1:
					sotnApi.AlucardApi.MaxtHearts = newHearts;
					if (sotnApi.AlucardApi.CurrentHearts > newHearts)
					{
						sotnApi.AlucardApi.CurrentHearts = newHearts;
					}
					notificationService.AddMessage($"{user} stole Hearts");
					break;
				case 2:
					sotnApi.AlucardApi.MaxtMp = newMP;
					if (sotnApi.AlucardApi.CurrentMp > newMP)
					{
						sotnApi.AlucardApi.CurrentMp = newMP;
					}
					notificationService.AddMessage($"{user} stole MaxMP");
					break;
				case 3:
					sotnApi.AlucardApi.MaxtHp = newHP;
					if (sotnApi.AlucardApi.CurrentHp > newHP)
					{
						sotnApi.AlucardApi.CurrentHp = newHP;
					}
					notificationService.AddMessage($"{user} stole MaxHP");
					break;
				case 4:
					sotnApi.AlucardApi.Str = newStr;
					notificationService.AddMessage($"{user} stole STR");
					break;
				case 5:
					sotnApi.AlucardApi.Con = newCon;
					notificationService.AddMessage($"{user} stole CON");
					break;
				case 6:
					sotnApi.AlucardApi.Int = newInt;
					notificationService.AddMessage($"{user} stole INT");
					break;
				case 7:
					sotnApi.AlucardApi.Lck = newLck;
					notificationService.AddMessage($"{user} stole LCK");
					break;
				case 8:
					RemoveItems(false, true);
					notificationService.AddMessage($"{user} took an item + gold!");
					break;
				default:
					break;
			}
				Alert(KhaosActionNames.ModerateTrap);
		}

		public void MajorTrap(string user = "Mayhem")
		{
		}

			public void Hex(string user = "Mayhem", bool allowMulti = false, bool isSuper = false)
		{
			List<int> effectNumbers = new List<int>() {1, 2, 3, 4};
			bool meterFull = MayhemMeterFull();

			int max = effectNumbers.Count;
			int min = 0;
			int roll = 0;

			if (sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp/2)
			{
				effectNumbers.RemoveAll(item => item == 1);
			}

			if (heartsLocked || subWeaponsLocked)
			{
				effectNumbers.RemoveAll(item => item == 3);
			}

			if (vladRelicsObtained < 1 || IsInRoomList(Constants.Khaos.ClockRoom))
			{
				effectNumbers.RemoveAll(item => item == 4);
			}

			if (activeHexEffects.Count > 0)
			{
				effectNumbers = effectNumbers.Except(activeHexEffects).ToList();
			}

			max = effectNumbers.Count;

			if ((hexActive && !allowMulti) || max == 0 || IsInRoomList(Constants.Khaos.EntranceCutsceneRooms))
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Hex, ChangesStats = true, Invoker = new MethodInvoker(() => Hex(user,allowMulti,isSuper)) });
				return;
			}
			else
			{
				roll = effectNumbers[rng.Next(min, max)];
				activeHexEffects.Add(roll);
				//Console.WriteLine($"min/max: {min}/{max}, roll:{roll} active: {activeHexEffects.First()}");
			}

			hexActive = true;
			string name = $"{KhaosActionNames.Hex}";
			string message = $"{user} used ";

			UpdateVisualEffect();

			switch (roll){
				case 1:
					HexTakeHPMPH();
					hexHPMPHActive = true;
					name += "(HP/MP/H)";
					break;
				case 2:
					HexTakeStats();
					hexStatsActive = true;
					name += "(Dmg)";
					break;
				case 3:
					HexTakeWeapons();
					hexWeaponsActive = true;
					name += "(Wep)";
					break;
				case 4:
					HexTakeRelics();
					hexRelicsActive = true;
					name += "(Relics)";
					break;
				default:
					break;

			}

			if (meterFull)
			{
				SpendMayhemMeter();
				Hex(user, true, true);
				isSuper = true;
			}
			if (isSuper)
			{
				name = "Super " + name;
			}

			message += name;
			System.TimeSpan newDuration = CurseDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Hex).FirstOrDefault().Duration);

			hexTimer.Interval = (int) newDuration.TotalMilliseconds;
			hexTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Curse,
				Duration = newDuration
			});

			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Hex);
		}

		private void UpdateVisualEffect(bool isRichterColor = false, int color = 33024)
		{
			if (isRichterColor)
			{
				if (alucardColor != 33056)
				{
					playerPaletteCheat.PokeValue(richterColor);
					playerPaletteCheat.Enable();
				}
				else
				{
					playerPaletteCheat.Disable();
					playerPaletteCheat.PokeValue(33056);
					playerPaletteCheat.Enable();
				}
			}
			else
			{
				if (overdriveOn)
				{
					visualEffectPaletteCheat.PokeValue(33126);
					visualEffectPaletteCheat.Enable();
					visualEffectTimerCheat.PokeValue(30);
					visualEffectTimerCheat.Enable();
				}
				else
				{
					visualEffectPaletteCheat.Disable();
					visualEffectTimerCheat.Disable();

					if (resetColorWhenAlucard) 
					{
						resetColorWhenAlucard = false;
						alucardColor = color;
					}

					if (hexActive)
					{
						playerPaletteCheat.PokeValue(33274);
						playerPaletteCheat.Enable();
					}
					else if (alucardColor != color)
					{
						playerPaletteCheat.PokeValue(alucardColor);
						playerPaletteCheat.Enable();
					}
					else
					{
						playerPaletteCheat.Disable();
						playerPaletteCheat.PokeValue(color);
						playerPaletteCheat.Enable();
					}
				}
			}
		}

		private void HexTakeHPMPH() 
		{
			hpTaken = sotnApi.AlucardApi.MaxtHp / 2;
			mpTaken = sotnApi.AlucardApi.MaxtMp / 2;
			heartsTaken = sotnApi.AlucardApi.MaxtHearts / 2;

			HPForMPActivePaused = true;

			if ((sotnApi.AlucardApi.MaxtHp - hpTaken) > 0)
			{
				sotnApi.AlucardApi.MaxtHp -= hpTaken;
			}
			if (sotnApi.AlucardApi.MaxtHp < sotnApi.AlucardApi.CurrentHp / 2)
			{
				sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
			}
			else if (sotnApi.AlucardApi.CurrentHp > 1)
			{
				sotnApi.AlucardApi.CurrentHp /= 2;
			}
			if (sotnApi.AlucardApi.MaxtMp - mpTaken > 0)
			{
				sotnApi.AlucardApi.MaxtMp -= mpTaken;
				if ((!mpLocked || HPForMPActive) && (sotnApi.AlucardApi.CurrentMp > 1))
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

			storedMaxMp = sotnApi.AlucardApi.MaxtMp;
			storedMp = sotnApi.AlucardApi.CurrentMp;
			spentMp = 0;

			HPForMPActivePaused = false;
		}
		private void HexTakeStats()
		{
			bool statsGivenPaused = givenStatsPaused;
			uint tempStrGiven = 0;
			uint tempConGiven = 0;
			uint tempIntGiven = 0;
			uint tempLckGiven = 0;

			if (!statsGivenPaused) 
			{
				tempStrGiven = strGiven;
				tempConGiven = conGiven;
				tempIntGiven = intGiven;
				tempLckGiven = lckGiven;

				sotnApi.AlucardApi.Str -= tempStrGiven;
				sotnApi.AlucardApi.Con -= tempConGiven;
				sotnApi.AlucardApi.Int -= tempIntGiven;
				sotnApi.AlucardApi.Lck -= tempLckGiven;
			}

			strTaken = (uint) sotnApi.AlucardApi.Str / 2;
			conTaken = (uint) sotnApi.AlucardApi.Con / 2;
			intTaken = (uint) sotnApi.AlucardApi.Int / 2;
			lckTaken = (uint) sotnApi.AlucardApi.Lck / 2;

			if (sotnApi.AlucardApi.Str > 1)
			{
				sotnApi.AlucardApi.Str -= (uint) strTaken;
			}
			if (sotnApi.AlucardApi.Con > 1)
			{
				sotnApi.AlucardApi.Con -= (uint) conTaken;
			}
			if (sotnApi.AlucardApi.Int > 1)
			{
				sotnApi.AlucardApi.Int -= (uint) intTaken;
			}
			if (sotnApi.AlucardApi.Lck > 1)
			{
				sotnApi.AlucardApi.Lck -= (uint) lckTaken;
			}

			sotnApi.AlucardApi.Str += tempStrGiven;
			sotnApi.AlucardApi.Con += tempConGiven;
			sotnApi.AlucardApi.Int += tempIntGiven;
			sotnApi.AlucardApi.Lck += tempLckGiven;
		}
		private void HexTakeWeapons(bool TakeReturnedOnly = false)
		{
			Console.WriteLine($"Hex Weapon Taken Values: {leftHandReturned},{rightHandReturned},{TakeReturnedOnly}");
			if (TakeReturnedOnly) 
			{
				leftHandTaken = leftHandReturned;
				rightHandTaken = rightHandReturned;
				
				if (sotnApi.AlucardApi.RightHand == rightHandTaken || sotnApi.AlucardApi.RightHand == leftHandTaken)
				{
					sotnApi.AlucardApi.RightHand = 0;
				}
				else
				{
					if (sotnApi.AlucardApi.HasItemInInventory(Equipment.Items[(int) rightHandTaken]))
					{
						sotnApi.AlucardApi.TakeOneItemByName(Equipment.Items[(int) rightHandTaken]);
					}
				}
				if (sotnApi.AlucardApi.LeftHand == rightHandTaken || sotnApi.AlucardApi.LeftHand == leftHandTaken)
				{
					sotnApi.AlucardApi.LeftHand = 0;
				}
				else
				{
					if (sotnApi.AlucardApi.HasItemInInventory(Equipment.Items[(int) leftHandTaken]))
					{
						sotnApi.AlucardApi.TakeOneItemByName(Equipment.Items[(int) leftHandTaken]);
					}
				}
			}
			else
			{
				leftHandTaken = sotnApi.AlucardApi.LeftHand;
				rightHandTaken = sotnApi.AlucardApi.RightHand;
				leftHandReturned = 0;
				rightHandReturned = 0;
				sotnApi.AlucardApi.RightHand = 0;
				sotnApi.AlucardApi.LeftHand = 0;
				sotnApi.AlucardApi.Subweapon = 0;
			}
			if (!subWeaponsLocked)
			{
				sotnApi.AlucardApi.CurrentHearts = 0;
				if (!rushDownActive)
				{
					subWeaponTaken = (uint) sotnApi.AlucardApi.Subweapon;
				}
			}
			sotnApi.AlucardApi.Subweapon = 0;
		}

		private void HexTakeRelics()
		{
			if (sotnApi.AlucardApi.HasRelic(Relic.CubeOfZoe))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.CubeOfZoe);
				cubeOfZoeTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.FireOfBat);
				fireOfBatTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.EchoOfBat);
				echoOfBatTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.ForceOfEcho))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.ForceOfEcho);
				forceOfEchoTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist) && 
				(!sotnApi.AlucardApi.HasRelic(Relic.FormOfMist)) ||
				(sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat)) ||
				((sotnApi.AlucardApi.HasRelic(Relic.GravityBoots) && 
					(sotnApi.AlucardApi.HasRelic(Relic.LeapStone) || sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf)))
				))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.PowerOfMist);
				powerOfMistTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.PowerOfWolf);
				powerOfWolfTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SkillOfWolf);
				skillOfWolfTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.HolySymbol))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.HolySymbol);
				skillOfWolfTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.FaerieCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.FaerieCard);
				faerieCardTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SpriteCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SpriteCard);
				spriteCardTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.BatCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.BatCard);
				batCardTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.DemonCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.DemonCard);
				demonCardTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.NoseDevilCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.NoseDevilCard);
				noseDevilCardTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.GhostCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GhostCard);
				ghostCardTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SwordCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SwordCard);
				ghostCardTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
				gasCloudTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.RingOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.RingOfVlad);
				ringOfVladTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.HeartOfVlad);
				heartOfVladTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.RibOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.RibOfVlad);
				ribOfVladTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.ToothOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.ToothOfVlad);
				toothOfVladTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.EyeOfVlad);
				eyeOfVladTaken = true;
			}
		}
		private void HexReturnRelics()
		{
			if (cubeOfZoeTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.CubeOfZoe, true);
				cubeOfZoeTaken = false;
			}
			if (fireOfBatTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.FireOfBat, true);
				fireOfBatTaken = false;
			}
			if (echoOfBatTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.EchoOfBat, true);
				echoOfBatTaken = false;
			}
			if (forceOfEchoTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.ForceOfEcho, true);
				forceOfEchoTaken = false;
			}
			if (powerOfMistTaken == true && (!unarmedActive || hexRelicsPaused))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.PowerOfMist, true);
				powerOfMistTaken = false;
			}
			if (powerOfWolfTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.PowerOfWolf, true);
				powerOfWolfTaken = false;
			}
			if (skillOfWolfTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.SkillOfWolf, true);
				skillOfWolfTaken = false;
			}
			if (holySymbolTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.HolySymbol, true);
				holySymbolTaken = false;
			}
			if (faerieCardTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.FaerieCard, true);
				faerieCardTaken = false;
			}
			if (spriteCardTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.SpriteCard, true);
				spriteCardTaken = false;
			}
			if (batCardTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.BatCard, true);
				batCardTaken = false;
			}
			if (demonCardTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.DemonCard, true);
				demonCardTaken = false;
			}
			if (noseDevilCardTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.NoseDevilCard);
				noseDevilCardTaken = false;
			}
			if (ghostCardTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.GhostCard, true);
				ghostCardTaken = false;
			}
			if (swordCardTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.SwordCard, true);
				swordCardTaken = false;
			}
			if (gasCloudTaken == true && (!heartsOnlyActive && !unarmedActive))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.GasCloud, true);
				gasCloudTaken = false;
			}
			if (ringOfVladTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.RingOfVlad, false);
				ringOfVladTaken = false;
			}
			if (heartOfVladTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.HeartOfVlad, false);
				heartOfVladTaken = false;
			}
			if (ribOfVladTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.RibOfVlad, false);
				ribOfVladTaken = false;
			}
			if (toothOfVladTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.ToothOfVlad, false);
				toothOfVladTaken = true;
			}
			if (eyeOfVladTaken == true)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.EyeOfVlad, false);
				eyeOfVladTaken = false;
			}
		}

		private void SlamJam(Object sender, EventArgs e)
		{
			++slamCount;
			queuedFastActions.Enqueue(new MethodInvoker(() => Slam(slamJamUser, true, false, false)));
			int min = 15000 - (vladRelicsObtained * 500);
			int max = 22500 - (vladRelicsObtained * 750);
			int newInterval = rng.Next(min, max);
			slamJamTickTimer.Interval = newInterval;
		}
		private void SlamJamOff(Object sender, EventArgs e)
		{
			++slamCount;
			Alert(KhaosActionNames.SlamJam);
			queuedFastActions.Enqueue(new MethodInvoker(() => Slam(slamJamUser, true, false, false)));
			Console.WriteLine($"User was slammed: {slamCount} times");
			slamCount = 0;
			slamJamTickTimer.Stop();
			slamJamTickTimer.Interval = 300;
			slamJamTimer.Stop();
			slamJamActive = false;
			superSlamJam = false;
		}

		private void HexOff(Object sender, EventArgs e)
		{
			int roll = activeHexEffects.FirstOrDefault();
			//Console.WriteLine($"roll:{roll} active: {activeHexEffects.First()}");
			if (activeHexEffects.Count > 0)
			{
				
				activeHexEffects.RemoveAt(0);
			}

			switch (roll)
			{
				case 1:
					hexHPMPHActive = false;
					HexReturnHPMPH();
					break;
				case 2:
					hexStatsActive = false;
					HexReturnStats();
					break;
				case 3:
					hexWeaponsActive = false;
					HexReturnWeapons();
					break;
				case 4:
					hexRelicsActive = false;
					HexReturnRelics();
					break;
				default:
					break;
			}

			if (activeHexEffects.Count == 0)
			{
				hexActive = false;
				UpdateVisualEffect();
				hexTimer.Stop();
			}
			else
			{
				hexTimer.Stop();
				hexTimer.Tick += HexOff;
				hexTimer.Interval = fastInterval;
				hexTimer.Start();
			}
		}
		private void HexReturnHPMPH ()
		{
			sotnApi.AlucardApi.MaxtHp += hpTaken;
			if (sotnApi.AlucardApi.MaxtHp > sotnApi.AlucardApi.CurrentHp)
			{
				sotnApi.AlucardApi.CurrentHp += (uint) Math.Round((double) (hpTaken / 2.0));
			}
			sotnApi.AlucardApi.MaxtMp += mpTaken;
			if (!mpLocked)
			{
				sotnApi.AlucardApi.CurrentMp += (uint) Math.Round((double) (mpTaken / 2.0));
			}
			sotnApi.AlucardApi.MaxtHearts += heartsTaken;
			if (!heartsLocked)
			{
				sotnApi.AlucardApi.CurrentHearts += (uint) Math.Round((double) (heartsTaken / 2.0));
			}

			//Safety check on reset
			if (sotnApi.AlucardApi.MaxtHp < minHP)
			{
				sotnApi.AlucardApi.MaxtHp = minHP;
			}
			if (sotnApi.AlucardApi.MaxtMp < minMP)
			{
				sotnApi.AlucardApi.MaxtMp = minMP;
			}
			if (sotnApi.AlucardApi.MaxtHearts < minHearts)
			{
				sotnApi.AlucardApi.MaxtHearts = minHearts;
			}
			hpTaken = 0;
			mpTaken = 0;
			heartsTaken = 0;
		}
		private void HexReturnStats() 
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
		private void HexReturnWeapons()
		{
			string item = Equipment.Items[(int) (leftHandTaken)];
			sotnApi.AlucardApi.GrantItemByName(item);
			if (!Constants.Khaos.TwoHandedWeapons.Contains(Equipment.Items[(int) rightHandTaken]))
			{
				item = Equipment.Items[(int) (rightHandTaken)];
				sotnApi.AlucardApi.GrantItemByName(item);
			}
			if (hexWeaponsPaused || (sotnApi.AlucardApi.Subweapon == 0 && !heartsLocked && !subWeaponsLocked))
			{
				if(subWeaponTaken != 0)
				{
					sotnApi.AlucardApi.Subweapon = (Subweapon) subWeaponTaken;
				}
				if (!heartsLocked && sotnApi.AlucardApi.CurrentHearts < sotnApi.AlucardApi.MaxtHearts)
				{
					sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts;
				}
			}
			leftHandReturned = leftHandTaken;
			rightHandReturned = rightHandTaken;
			leftHandTaken = 0;
			rightHandTaken = 0;
			subWeaponTaken = 0;
			
		}

		public void GetJuggled(string user = "Mayhem")
		{
			/*
			if (getJuggledActive)
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.GetJuggled, LocksInvincibility = true, Invoker = new MethodInvoker(() => GetJuggled(user)) });
				return;
			}*/

			if (getJuggledActive) { 
				queuedActions.Add(new QueuedAction { Name = "GetJuggled", LocksInvincibility = true, Invoker = new MethodInvoker(() => GetJuggled(user)) });
				return;
			}

			getJuggledActive = true;
			invincibilityCheat.PokeValue(0);
			invincibilityCheat.Enable();
			defensePotionCheat.PokeValue(1);
			defensePotionCheat.Enable();
			invincibilityLocked = true;
			
			System.TimeSpan newDuration = CurseDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.GetJuggled).FirstOrDefault().Duration);

			getJuggledTimer.Interval = (int) newDuration.TotalMilliseconds;
			getJuggledTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.GetJuggled,
				Type = Enums.ActionType.Curse,
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
			getJuggledActive = false;
		}
		public void Ambush(string user = "Mayhem")
		{
			spawnActive = true;
			ambushTriggerRoomX = sotnApi.GameApi.RoomX;
			ambushTriggerRoomY = sotnApi.GameApi.RoomY;

			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.Ambush;

			if (meterFull)
			{
				name = "Super " + name;
				superAmbush = true;
				++superAmbushCount;
				SpendMayhemMeter();
			}

			ambushTimer.Start();
			ambushSpawnTimer.Start();
			ambushEnemies.Clear();
			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Ambush);
		}
		private void AmbushOff(Object sender, EventArgs e)
		{
			--superAmbushCount;
			if (superAmbushCount < 1)
			{
				superAmbush = false;
			}
			spawnActive = false;
			ambushEnemies.RemoveRange(0, ambushEnemies.Count);
			ambushTimer.Interval = 5 * (60 * 1000);
			ambushTimer.Stop();
			ambushSpawnTimer.Stop();
		}
		private void AmbushSpawn(Object sender, EventArgs e)
		{
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < (5 + sotnApi.AlucardApi.Level)  || sotnApi.GameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone) || IsInRoomList(Constants.Khaos.ClockRoom))
			{
				return;
			}

			string name = KhaosActionNames.Ambush;
			if (superAmbush)
			{
				name = "Super " + name;
			}

			uint zone2 = sotnApi.GameApi.Zone2;

			if (ambushZone != zone2)
			{
				ambushEnemies.Clear();
				ambushZone = zone2;
			}

			//Console.WriteLine($"ambushZone={ambushZone},zone2={zone2}");
			FindAmbushEnemy();

			if (ambushEnemies.Count > 0)
			{
				int enemyIndex = rng.Next(0, ambushEnemies.Count);
				if (ambushTimer.Interval == 5 * (60 * 1000))
				{
					
					ambushTimer.Stop();
					System.TimeSpan newDuration = CurseDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Ambush).FirstOrDefault().Duration);
					ambushTimer.Interval = (int) newDuration.TotalMilliseconds;
					Console.WriteLine($"Ambush Interval: {ambushTimer.Interval}");

					notificationService.AddTimer(new Services.Models.ActionTimer
					{
						Name = name,
						Type = Enums.ActionType.Curse,
						Duration = newDuration
					});
					ambushTimer.Start();
				}

				ambushEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				int minLeftX = (int) sotnApi.AlucardApi.ScreenX - 30;
				int minRightX = (int) sotnApi.AlucardApi.ScreenX + 30;
				bool isOnLeftSide = sotnApi.AlucardApi.ScreenX <= 127 ? true : false;


				if (ambushEnemies[enemyIndex].Ypos >= (sotnApi.AlucardApi.ScreenY + 40) || ambushEnemies[enemyIndex].Ypos <= (sotnApi.AlucardApi.ScreenY - 40))
				{
					// If Enemy is sufficiently above/below Alucard, spawn anywhere in the X-Axis.
					ambushEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				}
				else
				{
					if ((ambushEnemies[enemyIndex].Ypos) <= sotnApi.AlucardApi.ScreenY)
					{
						if (minLeftX <= 10 || isOnLeftSide)
						{
							ambushEnemies[enemyIndex].Xpos = (ushort) rng.Next(minRightX, 245);
						}
						else if (minRightX >= 245 || !isOnLeftSide)
						{
							ambushEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, minLeftX);
						}
						else
						{
							ambushEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
						}
					}
				}

				Console.WriteLine($"Ambush: Map - X{sotnApi.AlucardApi.MapX},Y{sotnApi.AlucardApi.MapY}, AlucardMap - X{alucardMapX},Y{alucardMapY}; Screen - X{sotnApi.AlucardApi.ScreenX},Y{sotnApi.AlucardApi.ScreenY}; Enemy Position: X{ambushEnemies[enemyIndex].Xpos},Y{ambushEnemies[enemyIndex].Ypos}");

				ambushEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				if (!IsInRoomList(Constants.Khaos.ClockRoom))
				{
					sotnApi.EntityApi.SpawnEntity(ambushEnemies[enemyIndex]);
				}
			}
		}
		private bool FindAmbushEnemy()
		{
			uint roomX = sotnApi.GameApi.RoomX;
			uint roomY = sotnApi.GameApi.RoomY;
			bool boostDmg = false;

			if ((roomX == ambushTriggerRoomX && roomY == ambushTriggerRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || IsInRoomList(Constants.Khaos.ClockRoom))
			{
				//Console.WriteLine($"Failed: roomX={roomX}, ambushTriggerRoomX={ambushTriggerRoomX},roomY={roomY}, ambushTriggerRoomY={ambushTriggerRoomY}");
				return false;
			}
			
			long enemy = sotnApi.EntityApi.FindEntityFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.AcceptedRomhackAmbushEnemies : Constants.Khaos.AcceptedAmbushEnemies);

			if (enemy > 0)
			{
				Entity? ambushEnemy = new Entity(sotnApi.EntityApi.GetEntity(enemy));

				if (ambushEnemy is not null && !ambushEnemies.Where(e => e.AiId == ambushEnemy.AiId).Any())
				{
					//Original
					//float statMultiplier = 1.0F + (0.025F * vladRelicsObtained);
					//float hpMultiplier = statMultiplier;
					//float dmgMultiplier = statMultiplier;

					float statMultiplier = 1.0F + (0.0125F * vladRelicsObtained * toolConfig.Khaos.CurseModifier);
					float hpMultiplier = statMultiplier * (1.0F * (toolConfig.Khaos.AmbushHPModifier *.5F));
					float dmgMultiplier = statMultiplier * (1.0F * (toolConfig.Khaos.AmbushDMGModifier *.5F));

					if (superAmbush)
					{
						//Original:
						//hpMultiplier += .5F;
						//dmgMultiplier += .25F;
						//ambushEnemy.Damage += (ushort) (1 + vladRelicsObtained);

						hpMultiplier += .25F * toolConfig.Khaos.SuperAmbushHPModifier;
						dmgMultiplier += .125F * toolConfig.Khaos.SuperAmbushDMGModifier;
						ambushEnemy.Damage += (ushort) ((1 + vladRelicsObtained) * (.5 * toolConfig.Khaos.CurseModifier));

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
								ambushEnemy.DamageTypeA = (uint) Entities.Poison;
								break;
							case 2:
								ambushEnemy.DamageTypeB = (uint) Entities.Curse;
								break;
							case 3:
								ambushEnemy.DamageTypeA = (uint) Entities.Stone;
								ambushEnemy.DamageTypeB = (uint) Entities.Stone;
								break;
							case 4:
								boostDmg = true;
								break;
							default:
								break;
						}
					}
					//Original
					//ambushEnemy.Hp = (ushort) ((5 + ambushEnemy.Hp + (5*vladRelicsObtained)) * statMultiplier);

					ambushEnemy.Hp = (ushort) ((ambushEnemy.Hp + (2.5 * toolConfig.Khaos.AmbushHPModifier) + (2.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier)) * hpMultiplier);

					if (boostDmg)
					{
						//Original:
						//statMultiplier += .125F;
						dmgMultiplier += (.0625F*toolConfig.Khaos.AmbushDMGModifier);
					}
					//Original
					//ambushEnemy.Damage = (ushort) ((ambushEnemy.Damage + (1 + vladRelicsObtained)) * statMultiplier);

					ambushEnemy.Damage = (ushort)((ambushEnemy.Damage + (.5 * toolConfig.Khaos.AmbushDMGModifier) + (.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier)) * dmgMultiplier);

					ambushEnemies.Add(ambushEnemy);
					Console.WriteLine($"Added ambush enemy with hp: {ambushEnemy.Hp} sprite: {ambushEnemy.AiId} damage: {ambushEnemy.Damage}, total/hp/dmg multiplier:{statMultiplier},{hpMultiplier},{dmgMultiplier}");
					return true;
				}
			}

			return false;
		}
		public void ToughBosses(string user = "Mayhem")
		{
			toughBossesRoomX = sotnApi.GameApi.RoomX;
			toughBossesRoomY = sotnApi.GameApi.RoomY;
			bool meterFull = MayhemMeterFull();
			if (meterFull)
			{
				superToughBosses = true;
				++superToughBossesCount;
				SpendMayhemMeter();
			}
			++toughBossesCount;
			Console.WriteLine($"bosscount{toughBossesCount},toughBossesRoomX{toughBossesRoomX},toughbossesRoomY{toughBossesRoomY},superToughBosses{superToughBosses},romhack{toolConfig.Khaos.RomhackMode}");
			
			toughBossesSpawnTimer.Start();
			sotnApi.GameApi.RespawnBosses();

			string message = meterFull ? $"{user} used Super {KhaosActionNames.ToughBosses}" : $"{user} used {KhaosActionNames.ToughBosses}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.ToughBosses);
		}
		private void ToughBossesSpawn(Object sender, EventArgs e)
		{
			uint roomX = sotnApi.GameApi.RoomX;
			uint roomY = sotnApi.GameApi.RoomY;
			
			ushort newBossHP = 0;
			float healthMultiplier = 1F;
			short healthFlat = 0;
			uint damageFlat = 0;

			//Original
			//float healthMultiplier = 1.7F + (0.1F * vladRelicsObtained);
			//short healthFlat = (short)(50 + (10 * vladRelicsObtained));
			//uint damageFlat = (uint)(2 + (1 * vladRelicsObtained));

			if ((roomX == toughBossesRoomX && roomY == toughBossesRoomY) || !sotnApi.GameApi.InAlucardMode() || sotnApi.AlucardApi.CurrentHp < 5)
			{
				return;
			}

			Entity? bossCopy = null;

			long enemy = sotnApi.EntityApi.FindEntityFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceRomhackBosses : Constants.Khaos.EnduranceBosses);

			if (enemy > 0)
			{
				healthMultiplier += (.35F * toolConfig.Khaos.CloneBossHPModifier);
				healthMultiplier += (.05F * toolConfig.Khaos.CurseModifier);
				healthFlat = (short)(25 * (toolConfig.Khaos.CloneBossHPModifier));
				healthFlat += (short)(5 * toolConfig.Khaos.CurseModifier);
				damageFlat += (uint)(.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier);
				damageFlat += (uint)(toolConfig.Khaos.CloneBossDMGModifier);

				
				LiveEntity boss = sotnApi.EntityApi.GetLiveEntity(enemy);
				bossCopy = new Entity(sotnApi.EntityApi.GetEntity(enemy));
				string name = Constants.Khaos.EnduranceRomhackBosses.Where(e => e.AiId == bossCopy.AiId).FirstOrDefault().Name;

				Console.WriteLine($"Tough Boss clone boss name: {name}, Boss HP{boss.Hp}, Boss Damage{boss.Damage}, healthMultiplier {healthMultiplier}, healthFlat{healthFlat}, damageFlat{damageFlat}");

				bool right = rng.Next(0, 2) > 0;
				bossCopy.Xpos = right ? (ushort) (bossCopy.Xpos + rng.Next(40, 80)) : (ushort) (bossCopy.Xpos + rng.Next(-80, -40));
				bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));


				newBossHP = (ushort)(healthMultiplier * (bossCopy.Hp + healthFlat));

				if (superToughBosses)
				{
					newBossHP = (ushort)((newBossHP) * (toolConfig.Khaos.SuperBossHPModifier * .5F));
					damageFlat = (uint)((damageFlat) * (toolConfig.Khaos.SuperBossDMGModifier * .5F));
				}

				boss.Damage += damageFlat;
				bossCopy.Damage += (ushort) damageFlat;

				if (newBossHP >= Int16.MaxValue)
				{
					//32767 Int16 MaxValue
					boss.Hp = (ushort) (Int16.MaxValue - 2767);
					bossCopy.Hp = (ushort) (Int16.MaxValue - 2767);
				}
				else
				{
					boss.Hp = newBossHP;
					bossCopy.Hp = newBossHP;
				}

				sotnApi.EntityApi.SpawnEntity(bossCopy);
				Console.WriteLine($"{KhaosActionNames.ToughBosses} boss found name: {name} hp: {bossCopy.Hp}, damage: {bossCopy.Damage}, sprite: {bossCopy.AiId}, health multiplier: {healthMultiplier}, health flat {healthFlat}, damage flat {damageFlat}");

				if (superToughBosses)
				{
					--superToughBossesCount;
					if(superToughBossesCount < 1) 
					{
						superToughBosses = false;
					}
					bossCopy.Xpos = rng.Next(0, 2) == 1 ? (ushort) (bossCopy.Xpos + rng.Next(-80, -20)) : (ushort) (bossCopy.Xpos + rng.Next(20, 80));
					bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));
					sotnApi.EntityApi.SpawnEntity(bossCopy);
					notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} - {name}");
					//notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} - {name} - {boss.Hp}HP");
				}
				else
				{
					
					notificationService.AddMessage($"{KhaosActionNames.ToughBosses} - {name}");
					//notificationService.AddMessage($"{KhaosActionNames.ToughBosses} - {name} - {boss.Hp}HP");
				}

				Console.WriteLine($"Tough Boss Clone Boss Calculated: {name}, HP{boss.Hp}, Damage{boss.Damage}");

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
				enemy = sotnApi.EntityApi.FindEntityFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.EnduranceAlternateRomhackBosses : Constants.Khaos.EnduranceAlternateBosses);
				if (enemy > 0)
				{
					LiveEntity boss = sotnApi.EntityApi.GetLiveEntity(enemy);
					string name = Constants.Khaos.EnduranceAlternateBosses.Where(e => e.AiId == boss.AiId).FirstOrDefault().Name;
				
					boss.Palette = (ushort) (boss.Palette + rng.Next(1, 10));

					float totalHealthMultiplier = 1F + (.15F * toolConfig.Khaos.SuperBossHPModifier);

					healthMultiplier = 1F + (.35F * toolConfig.Khaos.SingleBossHPModifier);
					healthMultiplier += (.05F * (toolConfig.Khaos.CurseModifier));
					healthMultiplier *= (1F + (.15F * (toolConfig.Khaos.SingleBossHPModifier)));
					healthFlat = (short) (25 * (toolConfig.Khaos.SingleBossHPModifier));
					healthFlat += (short) (5 * toolConfig.Khaos.CurseModifier);
					healthFlat *= (short) (1.5 * (toolConfig.Khaos.CurseModifier));
					damageFlat += (uint) (.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier);
					damageFlat += (uint) (toolConfig.Khaos.SingleBossDMGModifier);

					Console.WriteLine($"Tough Boss alternate boss found name: {name}, Boss HP{boss.Hp}, Boss Damage{boss.Damage}, totalhealthMultiplier {totalHealthMultiplier}, healthMultiplier {healthMultiplier}, healthFlat{healthFlat}, damageFlat{damageFlat}, toolConfig.Khaos.SuperBossHPModifier{toolConfig.Khaos.SuperBossHPModifier}");

					if (superToughBosses)
					{
						//Original:
						//newBossHP = (ushort) Math.Round(1.3 * (healthMultiplier) * (boss.Hp + (6 * healthFlat)));
						//boss.Damage += (2 * damageFlat);

						newBossHP = (ushort) (totalHealthMultiplier * (healthMultiplier) * (boss.Hp + (healthFlat + (healthFlat * .5F * toolConfig.Khaos.SuperBossHPModifier))));
						boss.Damage += (damageFlat * toolConfig.Khaos.SuperBossDMGModifier);
						--superToughBossesCount;
						if (superToughBossesCount < 1)
						{
							superToughBosses = false;
						}
						notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} - {name}");
						//notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} - {name} - {boss.Hp}HP");
					}
					else
					{
						//Original:
						//newBossHP = (ushort) Math.Round((healthMultiplier) * (boss.Hp + (3*healthFlat)));
						//boss.Damage += damageFlat;

						newBossHP = (ushort) Math.Round((healthMultiplier) * (boss.Hp + (healthFlat)));
						boss.Damage += damageFlat;
						notificationService.AddMessage($"{KhaosActionNames.ToughBosses} - {name}");
						//notificationService.AddMessage($"{KhaosActionNames.ToughBosses} - {name} - {boss.Hp}HP");
					}
					if(newBossHP > Int16.MaxValue)
					{
						//32767 Int16 MaxValue
						boss.Hp = (ushort)(Int16.MaxValue - 767); 
					}
					else
					{
						boss.Hp = newBossHP;
					}
					Console.WriteLine($"Tough Boss Alternate Boss Calculated: {name}, HP{boss.Hp}, Damage{boss.Damage}");

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
			bool meterFull = MayhemMeterFull();
			float enhancedFactor = 1F;

			string name = KhaosActionNames.StatsDown;

			if (meterFull)
			{
				name = "Super " + name;
				enhancedFactor = Constants.Khaos.SuperStatsDownFactor;
				SpendMayhemMeter();
			}
			//Console.WriteLine($"HP Formula: {alucardApi.CurrentHp}*{toolConfig.Khaos.StatsDownFactor}*{enhancedFactor}");

			//Ensure hex removed stats are also nerfed
			hpTaken = (uint) (hpTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			strTaken = (uint) (strTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			conTaken = (uint) (conTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			intTaken = (uint) (intTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			lckTaken = (uint) (lckTaken * toolConfig.Khaos.StatsDownFactor * enhancedFactor);

			uint newCurrentHp = (uint) (sotnApi.AlucardApi.CurrentHp * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newCurrentMp = (uint) (sotnApi.AlucardApi.CurrentHp * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newCurrentHearts = (uint) (sotnApi.AlucardApi.CurrentHearts * toolConfig.Khaos.StatsDownFactor * enhancedFactor);

			uint newMaxHearts = (uint) (sotnApi.AlucardApi.MaxtHearts * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			uint newMaxHp = 0;
			uint newMaxMp = (uint) (sotnApi.AlucardApi.MaxtMp * toolConfig.Khaos.StatsDownFactor * enhancedFactor);

			bool statsPaused = givenStatsPaused;

			uint newStr = 0;
			uint newCon = 0;
			uint newInt = 0;
			uint newLck = 0;

			uint adjustedMinHP = minHP;
			uint adjustedMinStr = minStr;
			uint adjustedMinCon = minCon;
			uint adjustedMinInt = minInt;
			uint adjustedMinLck = minLck;

			if (givenStatsPaused)
			{
				newMaxHp = (uint) ((sotnApi.AlucardApi.MaxtHp) * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
				newStr = (uint) ((sotnApi.AlucardApi.Str) * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
				newCon = (uint) ((sotnApi.AlucardApi.Con) * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
				newInt = (uint) ((sotnApi.AlucardApi.Int) * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
				newLck = (uint) ((sotnApi.AlucardApi.Lck) * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			}
			else
			{
				uint baseHP = sotnApi.AlucardApi.MaxtHp > hpGiven ? sotnApi.AlucardApi.MaxtHp - hpGiven : minHP;
				uint baseStr = sotnApi.AlucardApi.Str > strGiven ? sotnApi.AlucardApi.Str - strGiven : minStr;
				uint baseCon = sotnApi.AlucardApi.Con > conGiven ? sotnApi.AlucardApi.Con - conGiven : minCon;
				uint baseInt = sotnApi.AlucardApi.Int > intGiven ? sotnApi.AlucardApi.Int - intGiven : minInt;
				uint baseLck = sotnApi.AlucardApi.Lck > lckGiven ? sotnApi.AlucardApi.Lck - lckGiven : minLck;

				newMaxHp = (uint) ((baseHP) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + hpGiven;
				newStr = (uint)((baseStr) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + strGiven;
				newCon = (uint)((baseCon) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + conGiven;
				newInt = (uint)((baseInt) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + intGiven;
				newLck = (uint)((baseLck) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + lckGiven;

				adjustedMinHP += hpGiven;
				adjustedMinStr += strGiven;
				adjustedMinCon += conGiven;
				adjustedMinInt += intGiven;
				adjustedMinLck += lckGiven;
			}

			//Zig- Enforce minimum max and current stats
			HPForMPActivePaused = true;

			sotnApi.AlucardApi.MaxtHp = newMaxHp < adjustedMinHP ? adjustedMinHP : newMaxHp;
			sotnApi.AlucardApi.MaxtMp = newMaxMp < minMP ? minMP : newMaxMp;
			sotnApi.AlucardApi.MaxtHearts = newMaxHearts < minHearts ? minHearts : newMaxHearts;

			sotnApi.AlucardApi.CurrentHp = newCurrentHp < 1 ? 1 : newCurrentHp;
			if (!mpLocked || HPForMPActive)
			{
				sotnApi.AlucardApi.CurrentMp = newCurrentMp < 1 ? 1 : newCurrentMp;
			}

			storedMaxMp = sotnApi.AlucardApi.MaxtMp;
			storedMp = sotnApi.AlucardApi.CurrentMp;

			HPForMPActivePaused = false;

			if (!heartsLocked == true)
			{
				sotnApi.AlucardApi.CurrentHearts = newCurrentHearts < 1 ? 1 : newCurrentHearts;
			}

			sotnApi.AlucardApi.Str = newStr < adjustedMinStr ? adjustedMinStr : newStr;
			sotnApi.AlucardApi.Con = newCon < adjustedMinCon ? adjustedMinCon : newCon;
			sotnApi.AlucardApi.Int = newInt < adjustedMinInt ? adjustedMinInt : newInt;
			sotnApi.AlucardApi.Lck = newLck < adjustedMinLck ? adjustedMinLck : newLck;

			//Recalculate XP and set level to 1 if at min stats
			uint newLevel = (uint) (sotnApi.AlucardApi.Level * toolConfig.Khaos.StatsDownFactor * enhancedFactor);
			if(newLevel > 1 && sotnApi.AlucardApi.Str <= adjustedMinStr && sotnApi.AlucardApi.Con <= adjustedMinCon && sotnApi.AlucardApi.Int <= adjustedMinInt && sotnApi.AlucardApi.Lck <= adjustedMinLck)
			{
				newLevel = 1;
			}

			uint newExperience = 0;
			if (newLevel <= StatsValues.ExperienceValues.Length && newLevel > 1)
			{
				newExperience = (uint) StatsValues.ExperienceValues[(int) newLevel - 1];
			}
			else if (newLevel > 1)
			{
				newExperience = (uint) StatsValues.ExperienceValues[StatsValues.ExperienceValues.Length - 1];
			}
			if (newLevel >= 1)
			{
				sotnApi.AlucardApi.Level = newLevel;
				sotnApi.AlucardApi.Experiecne = newExperience;
			}

			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.StatsDown);
		}
		public void Confiscate(string user = "Mayhem")
		{
			RemoveItems(true,false);
			notificationService.AddMessage($"{user} used {KhaosActionNames.Confiscate}");
			Alert(KhaosActionNames.Confiscate);
		}
		private void RemoveItems(bool clearInventory = false, bool staticRemoval = false)
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

			if (staticRemoval) 
			{
				clearedSlots = 1;
				goldPercentage = .5f;
			}
			else{


				int effectPower = toolConfig.Khaos.CurseModifier - 1 > 0 ? toolConfig.Khaos.CurseModifier - 1 : 0;
					effectPower += vladRelicsObtained;
					
				switch (effectPower)
				{
					case 0:
						goldPercentage = 0.7f;
						clearedSlots = 1;
						break;
					case 1:
						goldPercentage = 0.6f;
						clearedSlots = 1;
						break;
					case 2:
						goldPercentage = 0.5f;
						clearedSlots = 2;
						break;
					case 3:
						goldPercentage = 0.4f;
						clearedSlots = 2;
						break;
					case 4:
						goldPercentage = 0.3f;
						clearedSlots = 3;
						break;
					case 5:
						goldPercentage = 0.2f;
						clearedSlots = 3;
						break;
					case 6:
						goldPercentage = 0.1f;
						clearedSlots = 4;
						break;
					case 7:
						goldPercentage = 0.1f;
						clearedSlots = 5;
						break;
					case 8:
						goldPercentage = 0.0f;
						clearedSlots = 6;
						break;
					default:
						goldPercentage = 0f;
						clearedSlots = 7;
						break;
				}
			}

			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring");
			bool equippedHolyGlasses = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			bool equippedSpikeBreaker = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing1 = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedGoldRing2 = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing1 = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring";
			bool equippedSilverRing2 = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";


			sotnApi.AlucardApi.Gold = goldPercentage == 0 ? 0 : (uint) Math.Round(sotnApi.AlucardApi.Gold * goldPercentage);
			if (clearInventory)
			{
				sotnApi.AlucardApi.ClearInventory();
			}
			

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
			if (clearAccessory1 && !equippedGoldRing1 && !equippedSilverRing1)
			{
				sotnApi.AlucardApi.Accessory1 = Equipment.AccessoryStart;
			}
			if (clearAccessory2 && !equippedSilverRing1 && !equippedSilverRing2)
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
			if (hasGoldRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Gold Ring");
			}
			if (hasSilverRing)
			{
				sotnApi.AlucardApi.GrantItemByName("Silver Ring");
			}
		}
		#endregion
		#region Blessing Effects

		public bool checkHasHolyGlasses()
		{
			if (sotnApi.AlucardApi.HasItemInInventory("Holy glasses") || Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses")
			{
				return true;
			}
				return false;	
		}

		public System.TimeSpan BlessingDurationGain(System.TimeSpan baseTimeSpan)
		{
			System.TimeSpan newTimeSpan = baseTimeSpan;
			if (hasHolyGlasses)
			{
				int newDurationSeconds = (int) Math.Floor(baseTimeSpan.TotalSeconds * (1 + (.25 * toolConfig.Khaos.BlessingModifier)));
				int diffInSeconds = (int) Math.Floor(newDurationSeconds - baseTimeSpan.TotalSeconds);
				int diffInMinutes = (int) Math.Floor(diffInSeconds / 60.0);
				diffInSeconds = diffInSeconds % 60;
				TimeSpan newInterval = new TimeSpan(0, diffInMinutes, diffInSeconds);
				newTimeSpan = baseTimeSpan.Add(newInterval);
				//Console.WriteLine($"Good Duration: baseTimeSpanTotal{baseTimeSpan.TotalMilliseconds}, baseTimeSpan{baseTimeSpan}, newDur {newDurationSeconds}, diffInSec {diffInSeconds}, diffInMinutes{diffInMinutes}, newTimeSpan{newTimeSpan}, newTimeSpanTotal{newTimeSpan.TotalMilliseconds}");
			}

			return newTimeSpan;
		}

		public int rollBlessingCommandCategory()
		{
			int result = 1;
			int baseMinorThreshold = 10;
			int baseModerateThreshold = 9;
			int baseMajorThreshold = 1;

			if (hasHolyGlasses)
			{
				baseMinorThreshold -= 6;
				baseModerateThreshold += 4;
				baseMajorThreshold += 2;
			}
			if (buffLckNeutralActive)
			{
				baseMinorThreshold -= 3;
				baseModerateThreshold += 2;
				baseMajorThreshold += 1;
			}
			if (superBuffLckNeutral)
			{
				baseMinorThreshold -= 1;
				baseModerateThreshold -= 2;
				baseMajorThreshold += 3;
			}

			int minorThreshold = baseMinorThreshold;
			int moderateThreshold = minorThreshold + baseModerateThreshold;
			int majorThreshold = moderateThreshold + baseMajorThreshold;

			int roll = rng.Next(1, 21);
			if (roll <= minorThreshold)
			{
				result = 1;
			}
			else if (roll <= moderateThreshold)
			{
				result = 2;
			}
			else if (roll <= majorThreshold)
			{
				result = 3;
			}

			Console.WriteLine($"Rolling Category: Roll={roll},Minor<={minorThreshold},Mod<={moderateThreshold},Major<={majorThreshold};Result={result}");
			return result;
		}

		public void GiveStats(string user = "Mayhem")
		{
			GiveBoon(user, true, 0);
		}
		public void MinorBoon(string user = "Mayhem")
		{
			GiveBoon(user, false, 1);
		}
		public void ModerateBoon(string user = "Mayhem")
		{
			GiveBoon(user, false, 2);
		}
		public void MajorBoon(string user = "Mayhem")
		{
			GiveBoon(user, false, 3);
		}
		public void MinorEquipment(string user = "Mayhem")
		{
			GiveEquipment(user, 1);
		}
		public void ModerateEquipment(string user = "Mayhem")
		{
			GiveEquipment(user, 2);
		}
		public void MajorEquipment(string user = "Mayhem")
		{
			GiveEquipment(user, 3);
		}

		public void MinorItems(string user = "Mayhem")
		{
			GiveItems(user, 1);
		}

		public void ModerateItems(string user = "Mayhem")
		{
			GiveItems(user, 2);
		}
		public void MajorItems(string user = "Mayhem")
		{
			GiveItems(user, 3);
		}

		public void MinorStats(string user = "Mayhem")
		{
			GiveBoon(user, true, 1);
		}

		public void ModerateStats(string user = "Mayhem")
		{
			GiveBoon(user, true, 2);
		}
		public void MajorStats(string user = "Mayhem")
		{
			GiveBoon(user, true, 3);
		}
		public void ProgressionStats(string user = "Mayhem")
		{
			GiveProgression(user, true);
		}


		public void MinorRelic(out Relic relic, out bool isValidRelic)
		{
			/*
			Check for Missing Bat Power Ups:
				If Soul of Bat, no Echo of Bat: Echo of Bat
				If Soul of Bat, no Fire of Bat: Fire of Bat
			*/

			isValidRelic = false;
			relic = Relic.FireOfBat;

			bool hasSoulOfBat = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat) || soulOfBatTaken) ? true : false;
			bool hasEchoOfBat = (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat) || echoOfBatTaken) ? true : false;
			bool hasFireOfBat = (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat) || fireOfBatTaken) ? true : false;

			if (hasSoulOfBat)
			{
				if (!hasEchoOfBat)
				{
					relic = Relic.EchoOfBat;
					isValidRelic = true;
				}
				if (!hasFireOfBat)
				{
					relic = Relic.FireOfBat;
					isValidRelic = true;
				}
			}
			if (!isValidRelic)
			{
				int maxLength = Constants.Khaos.MinorBoonRelics.Length;
				int relicIndex = rng.Next(0, maxLength);
				for (int i = 0; i < 11; i++)
				{
					if (!sotnApi.AlucardApi.HasRelic(Constants.Khaos.MinorBoonRelics[relicIndex]))
					{
						isValidRelic = true;
						break;
					}
					else if (i == maxLength - 1)
					{
						isValidRelic = false;
						break;
					}
					relicIndex = rng.Next(0, maxLength);
				}
				relic = Constants.Khaos.MinorBoonRelics[relicIndex];
			}
			Console.WriteLine($"{KhaosActionNames.MinorRelic} rolled: rel={relic}, isValidRelic={isValidRelic}");
		}

		public void ModerateRelic(out Relic relic, out bool isValidRelic)
		{
			/*
			Check for Missing Wolf Skills:
				If Soul of Wolf, no Power of Wolf: Power of Wolf
				If Soul of Wolf, no Skill of Wolf: Skill of Wolf
			Check for Missing Mist Skills:
				If Form of Mist, no Gas Cloud: Gas Cloud
			*/

			isValidRelic = false;
			relic = Relic.FireOfBat;

			bool hasSoulOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) || soulOfWolfTaken) ? true : false;
			bool hasPowerOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf) || powerOfWolfTaken) ? true : false;
			bool hasSkillOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf) || skillOfWolfTaken) ? true : false;
			bool hasFormOfMist = sotnApi.AlucardApi.HasRelic(Relic.FormOfMist)  ? true : false;
			bool hasGasCloud = (sotnApi.AlucardApi.HasRelic(Relic.GasCloud) || gasCloudTaken) ? true : false;

			if (hasSoulOfWolf)
			{
				if (!hasPowerOfWolf)
				{
					relic = Relic.PowerOfWolf;
					isValidRelic = true;
				}
				if (!hasSkillOfWolf)
				{
					relic = Relic.SkillOfWolf;
					isValidRelic = true;
				}
			}
			if (hasFormOfMist)
			{
				if (!hasGasCloud)
				{
					relic = Relic.GasCloud;
					isValidRelic = true;
				}
			}
			if (!isValidRelic)
			{
				int maxLength = Constants.Khaos.ModerateBoonRelics.Length;
				int relicIndex = rng.Next(0, maxLength);
				for (int i = 0; i < 11; i++)
				{
					if (!sotnApi.AlucardApi.HasRelic(Constants.Khaos.ModerateBoonRelics[relicIndex]))
					{
						isValidRelic = true;
						break;
					}
					else if (i == maxLength - 1)
					{
						isValidRelic = false;
						break;
					}
					relicIndex = rng.Next(0, maxLength);
				}
				relic = Constants.Khaos.ModerateBoonRelics[relicIndex];
			}
			Console.WriteLine($"{KhaosActionNames.ModerateRelic} rolled: rel={relic}, isValidRelic={isValidRelic}");
		}
		public void MajorRelic(out Relic relic, out bool isValidRelic)
		{
			/*
			Give Flight Relic if able
			*/

			isValidRelic = false;
			relic = Relic.FireOfBat;

			GiveFlightRelic(out relic, out isValidRelic); 

			if (!isValidRelic)
			{
				int maxLength = Constants.Khaos.MajorBoonRelics.Length;
				int relicIndex = rng.Next(0, maxLength);
				for (int i = 0; i < 11; i++)
				{
					if (!sotnApi.AlucardApi.HasRelic(Constants.Khaos.MajorBoonRelics[relicIndex]))
					{
						isValidRelic = true;
						break;
					}
					else if (i == maxLength - 1)
					{
						isValidRelic = false;
						break;
					}
					relicIndex = rng.Next(0, maxLength);
				}
				relic = Constants.Khaos.MajorBoonRelics[relicIndex];
			}
			Console.WriteLine($"{KhaosActionNames.MajorRelic} rolled: rel={relic}, isValidRelic={isValidRelic}");
		}
		public void ProgressionRelic(out Relic relic, out bool isValidRelic)
		{
			/*
			Check for missing Castle 1 Checks:
				If Jewel of Open, no Merman Statue: Give Merman
				If Merman Staute, no Jewel of Open: Give Jewel of Open
				If no Leap Stone, Gravity Boots, Form of Mist, Power of Mist, Soul of Wolf, OR Soul of Bat: Soul of Wolf
			Check for missing Flight:
				If Soul of Wolf, no Gravity Boots: Gravity Boots
				If Leapstone, no Gravity: Give Gravity.
				If Gravity, no leapstone: Give Leapstone.
				If Form of Mist, no Power of Mist: Power of Mist
				If Power of Mist, no Form of Mist: Form of Mist
			Check for missing Adventure Progression:
				If no Demon Card OR N Demon Card: Demon Card
			Check for missing Standard progression:
				If no Form of Mist: Form of Mist
				If no Spike Breaker: Spike Breaker
				If no Silver Ring: Silver Ring
				If no Gold Ring: Gold Ring
				If no Holy Glasses: Holy Glasses
			Check for missing Movement Speed Enhancements:
				If no Leap Stone: Leap Stone
				If no Gravity Boots: Gravity Boots
				If no Soul of Bat: Soul of Bat
			Check for missing Vlads (if allowed):
				If missing Heart of Vlad: Heart of Vlad
				If missing Tooth of Vlad: Tooth of Vlad
				If missing Ring of Vlad: Ring of Vlad
				If missing Eye of Vlad: Eye of Vlad
				If missing Rib of Vlad: Rib of Vlad
			*/

			bool hasJewelOfOpen = sotnApi.AlucardApi.HasRelic(Relic.JewelOfOpen) ? true : false;
			bool hasMermanStatue = sotnApi.AlucardApi.HasRelic(Relic.MermanStatue) ? true : false;
			bool hasDemonCard = (sotnApi.AlucardApi.HasRelic(Relic.DemonCard)|| demonCardTaken) ? true : false;
			bool hasNoseDevilCard = (sotnApi.AlucardApi.HasRelic(Relic.NoseDevilCard) || noseDevilCardTaken) ? true : false;
			bool hasSoulOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) || soulOfWolfTaken) ? true : false;
			bool hasFormOfMist = sotnApi.AlucardApi.HasRelic(Relic.FormOfMist) ? true : false;
			bool hasPowerOfMist = (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist) || powerOfMistTaken) ? true : false;
			bool hasSoulOfBat = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat) || soulOfBatTaken) ? true : false;
			bool hasGravityBoots = sotnApi.AlucardApi.HasRelic(Relic.GravityBoots) ? true : false;
			bool hasLeapStone = sotnApi.AlucardApi.HasRelic(Relic.LeapStone) ? true : false;
			bool hasHeartOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) || heartOfVladTaken) ? true : false;
			bool hasToothOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist) || toothOfVladTaken) ? true : false;
			bool hasRingOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat) || ringOfVladTaken) ? true : false;
			bool hasEyeOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.GravityBoots) || eyeOfVladTaken) ? true : false;
			bool hasRibOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.LeapStone) || ribOfVladTaken) ? true : false;

			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses") || Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses" ? true : false;
			bool hasSpikebreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker") || Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker" ? true : false;
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring") || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring" ? true : false;
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring") || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring" ? true : false;


			isValidRelic = false;
			relic = Relic.FireOfBat;

			if(hasJewelOfOpen && !hasMermanStatue)
			{
				relic = Relic.MermanStatue;
				isValidRelic = true;
			}
			else if (!hasJewelOfOpen && hasMermanStatue)
			{
				relic = Relic.JewelOfOpen;
				isValidRelic = true;
			}
			else if (!hasLeapStone && !hasGravityBoots && !hasFormOfMist && !hasPowerOfMist && !hasSoulOfWolf && !hasSoulOfBat)
			{
				relic = Relic.SoulOfWolf;
				isValidRelic = true;
			}

			if (!isValidRelic)
			{
				GiveFlightRelic(out relic, out isValidRelic);
			}
			
			if (!isValidRelic)
			{
				if (!hasJewelOfOpen)
				{
					relic = Relic.JewelOfOpen;
					isValidRelic = true;
				}
				if (!hasDemonCard && !hasNoseDevilCard)
				{
					relic = Relic.DemonCard;
					isValidRelic = true;
				}
				else if (!hasFormOfMist)
				{
					relic = Relic.FormOfMist;
					isValidRelic = true;
				}
				else if (!hasSpikebreaker)
				{
					sotnApi.AlucardApi.GrantItemByName("Spike Breaker");
				}
				else if (!hasGoldRing)
				{
					sotnApi.AlucardApi.GrantItemByName("Gold Ring");
				}
				else if (!hasSilverRing)
				{
					sotnApi.AlucardApi.GrantItemByName("Silver Ring");
				}
				else if (!hasHolyGlasses)
				{
					sotnApi.AlucardApi.GrantItemByName("Holy glasses");
				}
				else if (!hasSoulOfBat)
				{
					relic = Relic.SoulOfBat;
					isValidRelic = true;
				}
				else if (toolConfig.Khaos.ProgressionGivesVlad)
				{
					if (!hasHeartOfVlad)
					{
						relic = Relic.HeartOfVlad;
						isValidRelic = true;
					}
					else if (!hasToothOfVlad)
					{
						relic = Relic.ToothOfVlad;
						isValidRelic = true;
					}
					else if (!hasRingOfVlad)
					{
						relic = Relic.RingOfVlad;
						isValidRelic = true;
					}
					else if (!hasEyeOfVlad)
					{
						relic = Relic.EyeOfVlad;
						isValidRelic = true;
					}
					else if (!hasRibOfVlad)
					{
						relic = Relic.RibOfVlad;
						isValidRelic = true;
					}
				}
			}
			Console.WriteLine($"{KhaosActionNames.ProgressionRelic} rolled: rel={relic}, isValidRelic={isValidRelic}");
		}
		public void GiveFlightRelic(out Relic relic, out bool isValidRelic)
		{
			/*
			Check for Missing Flight:
				If Leapstone, no Gravity: Give Gravity.
				If Gravity, no leapstone: Give Leapstone.
				If Form of Mist, no Power of Mist: Power of Mist
				If Power of Mist, no Form of Mist: Form of Mist
				If Soul of Wolf, no Gravity Boots: Gravity Boots
			*/

			relic = Relic.FireOfBat;
			isValidRelic = false;

			bool hasSoulOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) || soulOfWolfTaken) ? true : false;
			bool hasFormOfMist = sotnApi.AlucardApi.HasRelic(Relic.FormOfMist) ? true : false;
			bool hasPowerOfMist = (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist) || powerOfMistTaken) ? true : false;
			bool hasSoulOfBat = sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat) ? true : false;
			bool hasGravityBoots = sotnApi.AlucardApi.HasRelic(Relic.GravityBoots) ? true : false;
			bool hasLeapStone = (sotnApi.AlucardApi.HasRelic(Relic.LeapStone)) ? true : false;
			
			if (hasSoulOfWolf && !hasGravityBoots)
			{
				relic = Relic.GravityBoots;
				isValidRelic = true;
			}
			else if (hasLeapStone && !hasGravityBoots)
			{
				relic = Relic.GravityBoots;
				isValidRelic = true;
			}
			else if (!hasLeapStone && hasGravityBoots)
			{
				relic = Relic.LeapStone;
				isValidRelic = true;
			}
			else if (hasFormOfMist && !hasPowerOfMist)
			{
				relic = Relic.PowerOfMist;
				isValidRelic = true;
			}
			else if (!hasFormOfMist && hasPowerOfMist)
			{
				relic = Relic.FormOfMist;
				isValidRelic = true;
			}

		}
		public void GivePotions(string user = "Mayhem")
		{
			uint addHP = 0;
			uint addMP = 0;
			uint addHearts = 0;

	
			uint baseGain = (uint) (5 + sotnApi.AlucardApi.Level);
			addHP = baseGain + sotnApi.AlucardApi.Level;
			addMP = baseGain;
			addHearts = baseGain;
	
			uint currentHp = sotnApi.AlucardApi.CurrentHp;
			uint currentMp = sotnApi.AlucardApi.CurrentMp;
			uint currentHearts = sotnApi.AlucardApi.CurrentHearts;

			if (hasHolyGlasses)
			{
				addHP = (uint) (addHP * 1.5);
				addMP = (uint) (addMP * 1.5);
				addHearts = (uint) (addHearts * 1.5);
			}
			if (mpLocked)
			{
				addHP += addMP;
			}
			else if ((currentMp + addMP) > (sotnApi.AlucardApi.MaxtMp * 2))
			{
				sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp * 2;
			}
			else
			{
				sotnApi.AlucardApi.CurrentMp += addMP;
			}
			if (heartsLocked)
			{
				addHP += addHearts;
			}
			else if ((currentHearts + addHearts) > (sotnApi.AlucardApi.MaxtHearts * 2))
			{
				sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts * 2;
			}
			else
			{
				sotnApi.AlucardApi.CurrentHearts += addHearts;
			}

			if ((currentHp + addHP) > (sotnApi.AlucardApi.MaxtHp * 2))
			{
				sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp * 2;
			}
			else
			{
				sotnApi.AlucardApi.CurrentHp += addHP;
			}

			if(hasHolyGlasses)
			{
				RandomizeModeratePotion(user);
			}
			else
			{
				RandomizePotion(user);
			}
				Alert(KhaosActionNames.Potions);
		}
		public void GiveEquipment(string user = "Mayhem", int forcedRoll = 0)
		{
			string item = "";
			string name = "";

			bool meterFull = MayhemMeterFull();
			bool isSuper = false;

			if (meterFull)
			{
				isSuper = true;
				SpendMayhemMeter();
				sotnApi.AlucardApi.GrantItemByName("Talisman");
				sotnApi.AlucardApi.GrantItemByName("Talisman");
				string message = $"{user}: 2 Talisman";
				notificationService.AddMessage(message);
				RollAndRewardEquipment(forcedRoll, isSuper);
			}

			RollAndRewardEquipment(forcedRoll, isSuper);
			Alert(KhaosActionNames.Equipment);

			void RollAndRewardEquipment(int forcedRoll, bool isSuper)
			{
				int itemCategory = 0;
				int itemTypes = 2;
				int itemsPerType = 0;
				int min = 1;
				int max = 3;

				if (forcedRoll != 0)
				{
					itemCategory = forcedRoll;
				}
				else
				{
					itemCategory = rollBlessingCommandCategory();
				}

				switch (itemCategory)
				{
					case 1:
						item = toolConfig.Khaos.minorEquipmentRewards[rng.Next(0, toolConfig.Khaos.minorEquipmentRewards.Length)];
						name = KhaosActionNames.MinorEquipment;
						min += 1;
						max += 1;
						break;
					case 2:
						item = toolConfig.Khaos.moderateEquipmentRewards[rng.Next(0, toolConfig.Khaos.moderateEquipmentRewards.Length)];
						name = KhaosActionNames.ModerateEquipment;
						max += 1;
						break;
					case 3:
						item = toolConfig.Khaos.majorEquipmentRewards[rng.Next(0, toolConfig.Khaos.majorEquipmentRewards.Length)];
						name = KhaosActionNames.MajorEquipment;
						break;
					default:
						break;
				}
				if (isSuper)
				{
					min += 1;
					max += 1;
				}
				if (hasHolyGlasses)
				{
					min += 1;
					max += 1;
				}
				if (buffLckNeutralActive)
				{
					max += 1;
				}

				itemTypes = rng.Next(min, max);

				for (int i = 0; i < itemTypes; i++)
				{
					int rolls = 0;
					itemsPerType = 1;

					while (sotnApi.AlucardApi.HasItemInInventory(item) && rolls < Constants.Khaos.HelpItemRetryCount)
					{
						switch (itemCategory)
						{
							case 1:
								item = toolConfig.Khaos.minorEquipmentRewards[rng.Next(0, toolConfig.Khaos.minorEquipmentRewards.Length)];
								break;
							case 2:
								item = toolConfig.Khaos.moderateEquipmentRewards[rng.Next(0, toolConfig.Khaos.moderateEquipmentRewards.Length)];
								break;
							case 3:
								item = toolConfig.Khaos.majorEquipmentRewards[rng.Next(0, toolConfig.Khaos.majorEquipmentRewards.Length)];
								break;
							default:
								break;
						}
						rolls++;
					}
					string message = $"{user} {name}: {itemsPerType} {item}";
					Console.WriteLine(message);
					message = $"{user}: {itemsPerType} {item}";
					notificationService.AddMessage(message);
					
					for (int x = 0; x < itemsPerType; x++)
					{
						sotnApi.AlucardApi.GrantItemByName(item);
					}
				}
				Alert(name);
			}

		}

		public void GiveItems(string user = "Mayhem", int forcedRoll = 0)
		{
			string item = "";
			string name = "";

			bool meterFull = MayhemMeterFull();
			bool isSuper = false;

			if (meterFull)
			{
				isSuper = true;
				SpendMayhemMeter();
				RollAndRewardItems(user,forcedRoll,isSuper);
				RollAndRewardItems(user, 4, isSuper);
			}

			RollAndRewardItems(user,forcedRoll,isSuper);
			Alert(KhaosActionNames.Items);

			void RollAndRewardItems(string user, int forcedRoll, bool isSuper)
			{

				int itemCategory = 0;
				int itemTypes = 2;
				int itemsPerType = 0;
				int min = 1;
				int max = 2;

				if (forcedRoll != 0)
				{
					itemCategory = forcedRoll;
				}
				else
				{
					itemCategory = rollBlessingCommandCategory();
				}
				switch (itemCategory)
				{
					case 1:
						item = toolConfig.Khaos.minorItemRewards[rng.Next(0, toolConfig.Khaos.minorItemRewards.Length)];
						name = KhaosActionNames.MinorItems;
						itemTypes += 1;
						min += 5;
						max += 11;
						break;
					case 2:
						item = toolConfig.Khaos.moderateItemRewards[rng.Next(0, toolConfig.Khaos.moderateItemRewards.Length)];
						name = KhaosActionNames.ModerateItems;
						min += 4;
						max += 9;
						break;
					case 3:
						item = toolConfig.Khaos.majorItemRewards[rng.Next(0, toolConfig.Khaos.majorItemRewards.Length)];
						name = KhaosActionNames.MajorItems;
						min += 2;
						max += 5;
						break;
					case 4:
						item = toolConfig.Khaos.superItemRewards[rng.Next(0, toolConfig.Khaos.superItemRewards.Length)];
						name = KhaosActionNames.SuperItems;
						min += 1;
						max += 1;
						break;
					default:
						break;
				}
				if (itemCategory < 4)
				{
					if (isSuper)
					{
						min += 2;
						max += 4;
					}
					if (hasHolyGlasses)
					{
						min += 1;
						max += 2;
					}
					if (superBuffLckNeutral)
					{
						min += 1;
						max += 2;
					}
					if (buffLckNeutralActive)
					{
						max += 1;
					}
				}
				
				List<string> itemsSelected = new List<string>();

				for (int i = 0; i < itemTypes; i++)
				{
					itemsPerType = rng.Next(min, max);
					bool isValidItem = false;

					while (isValidItem == false)
					{
						switch (itemCategory)
						{
							case 1:
								item = toolConfig.Khaos.minorItemRewards[rng.Next(0, toolConfig.Khaos.minorItemRewards.Length)];
								break;
							case 2:
								item = toolConfig.Khaos.moderateItemRewards[rng.Next(0, toolConfig.Khaos.moderateItemRewards.Length)];
								break;
							case 3:
								item = toolConfig.Khaos.majorItemRewards[rng.Next(0, toolConfig.Khaos.majorItemRewards.Length)];
								break;
							case 4:
								item = toolConfig.Khaos.superItemRewards[rng.Next(0, toolConfig.Khaos.superItemRewards.Length)];
								break;
							default:
								break;
						}
						if (itemsSelected.Contains(item))
						{
							isValidItem = false;
						}
						else
						{
							isValidItem = true;
							itemsSelected.Add(item);
						}
					}
					string message = $"{user} {name}: {itemsPerType} {item}";
					Console.WriteLine(message);
					message = $"{user}: {itemsPerType} {item}";
					notificationService.AddMessage(message);

					for (int x = 0; x < itemsPerType; x++)
					{
						sotnApi.AlucardApi.GrantItemByName(item);
					}
				}

			}
		}

		public void Speed(string user = "Mayhem")
		{
			speedActive = true;
			speedLocked = true;

			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.Speed;

			if (meterFull)
			{
				name = "Super " + name;
				SpendMayhemMeter();
				superSpeed = true;
			}

			SetHasteStaticSpeeds(meterFull);

			System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Speed).FirstOrDefault().Duration);

			speedTimer.Interval = (int) newDuration.TotalMilliseconds;
			speedTimer.Start();
			
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Blessing,
				Duration = newDuration
			});
			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Speed);
		}
		private void SpeedOff(Object sender, EventArgs e)
		{
			speedTimer.Stop();
			SetSpeed();
			speedOverdriveOffTimer.Start();
			superSpeed = false;
			speedActive = false;
			speedLocked = false;
		}
		private void SpeedOverdriveOn(object sender, EventArgs e)
		{
			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * (toolConfig.Khaos.SpeedFactor / 1.8));
			overdriveOn = true;
			UpdateVisualEffect();
			speedOverdriveTimer.Stop();
		}
		private void SpeedOverdriveOff(object sender, EventArgs e)
		{
			if (speedActive)
			{
				SetHasteStaticSpeeds(superSpeed);
			}
			else
			{
				sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal);
			}
			overdriveOn = false;
			UpdateVisualEffect();
			speedOverdriveOffTimer.Stop();
		}
		public void Regen(string user = "Mayhem")
		{
			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.Regen;
			++regenLevel;
			if (meterFull)
			{
				name = "Super " + name;
				superRegen = true;
				SpendMayhemMeter();
			}

			darkMetamorphasisCheat.PokeValue(1);
			darkMetamorphasisCheat.Enable();

			System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Regen).FirstOrDefault().Duration);

			regenTimer.Interval = (int) newDuration.TotalMilliseconds;
			regenTimer.Start();
			regenTickTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
				Type = Enums.ActionType.Blessing,
				Duration = newDuration 
			});

			string message = $"{user} used {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Regen);
		}
		private void RegenGain(Object sender, EventArgs e)
		{
			uint superGain = superRegen ? Constants.Khaos.SuperRegenExtraGain : 0u;
			uint regenMultiplier = (uint) regenLevel;
			//uint regenMultiplier = (uint) 1u;

			if (sotnApi.AlucardApi.CurrentHp < (sotnApi.AlucardApi.MaxtHp * 2))
			{
				sotnApi.AlucardApi.CurrentHp += (uint)((toolConfig.Khaos.RegenGainPerSecond + superGain) * regenMultiplier);
			}
			if (!mpLocked && sotnApi.AlucardApi.CurrentMp < sotnApi.AlucardApi.MaxtMp)
			{
				sotnApi.AlucardApi.CurrentMp += (uint)((toolConfig.Khaos.RegenGainPerSecond + superGain) * regenMultiplier);
			}
			if (!heartsLocked && sotnApi.AlucardApi.CurrentHearts < (2*sotnApi.AlucardApi.MaxtHearts))
			{
				sotnApi.AlucardApi.CurrentHearts += (uint)((toolConfig.Khaos.RegenGainPerSecond + superGain) * regenMultiplier);
			}

		}
		private void RegenOff(Object sender, EventArgs e)
		{
			--regenLevel;
			if (regenLevel <= 0)
			{
				darkMetamorphasisCheat.Disable();
				regenTimer.Stop();
				regenTickTimer.Stop();
				superRegen = false;
			}
		}
		public void TimeStop(string user = "Mayhem")
		{
			if (timeStopActive)
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.TimeStop, Invoker = new MethodInvoker(() => TimeStop(user)) });
				return;
			}

			sotnApi.AlucardApi.ActivateStopwatch();
			timeStopZone = sotnApi.GameApi.Zone2;
			timeStopActive = true;

			if (!heartsOnlyActive)
			{
				sotnApi.AlucardApi.Subweapon = Subweapon.Stopwatch;
			}
			stopwatchTimer.Enable();

			System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TimeStop).FirstOrDefault().Duration);
			timeStopTimer.Interval = (int) newDuration.TotalMilliseconds;
			timeStopTimer.Start();
			timeStopCheckTimer.Start();

			notificationService.AddMessage($"{user} used {KhaosActionNames.TimeStop}");
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = KhaosActionNames.TimeStop,
				Type = Enums.ActionType.Blessing,
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
			uint zone2 = sotnApi.GameApi.Zone2;

			if (timeStopZone != zone2)
			{
				timeStopZone = zone2;
				sotnApi.AlucardApi.ActivateStopwatch();
			}
		}

		public void GiveBuff(string user = "Mayhem", int forcedRoll = 0)
		{
			List<int> effectNumbers = new List<int>() { 1, 2, 3, 4};
			bool meterFull = MayhemMeterFull();

			int max = effectNumbers.Count;
			int min = 0;
			int roll = 0;
		
			if (buffStrRangeActive || weaponQualitiesLocked)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 1);
			}

			if (buffConHPActive)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 2);
			}

			if (buffIntMPActive || mpLocked) 
			{
				--max;
				effectNumbers.RemoveAll(item => item == 3);
			}

			if (buffLckNeutralActive)
			{
				--max;
				effectNumbers.RemoveAll(item => item == 4);
			}

			max = effectNumbers.Count;

			if (forcedRoll != 0 && !effectNumbers.Contains(forcedRoll))
			{
				max = 0;
			}

			if (max == 0 || IsInRoomList(Constants.Khaos.EntranceCutsceneRooms))
			{
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Buff, Type = ActionType.Blessing, ChangesStats = true, Invoker = new MethodInvoker(() => GiveBuff(user, forcedRoll)) });
				return;
			}
			else if (forcedRoll != 0)
			{
				roll = forcedRoll;
			}
			else
			{
				roll = effectNumbers[rng.Next(min, max)];
			}

			buffActive = true;
			string name = $"{KhaosActionNames.Buff}";
			string message = $"{user} used ";

			//message += name;

			switch (roll)
			{
				case 1:
					//message += "(Str + Range)";
					BuffStrRange(user, message);
					break;
				case 2:
					//message += "(Con + HP)";
					BuffConHP(user, message);
					break;
				case 3:
					//message += "(Int + MP)";
					BuffIntMP(user, message);
					break;
				case 4:
					//message += "(Lck + Neutrals)";
					BuffLckNeutral(user, message);
					break;
				default:
					break;
			}
			//Console.WriteLine($"{message}");
		}


		public void ExtraRange(string user = "Mayhem")
		{
			GiveBuff(user, 1);
		}

		public void FaceTank(string user = "Mayhem")
		{
			GiveBuff(user, 2);
		}

		public void SpellCaster(string user = "Mayhem")
		{
			GiveBuff(user, 3);
		}

		public void Lucky(string user = "Mayhem")
		{
			GiveBuff(user, 4);
		}

		public void CalculateTempStatGiven(out uint newStat, out bool holyGlasses, uint baseAdd, bool isSuper = false)
		{
			if (hasHolyGlasses)
			{
				baseAdd = (uint) (baseAdd * 1.5);
				holyGlasses = true;
			}
			else
			{
				holyGlasses = false;
			}
			if (isSuper)
			{
				baseAdd *= 2;
			}
			newStat = baseAdd;
		}

		public uint CalculateTempStatGiven(uint baseAdd, bool isSuper = false, bool holyGlasses = false)
		{
			//Console.WriteLine($"{baseAdd},{isSuper},{holyGlasses}");
			if (holyGlasses)
			{
				baseAdd = (uint) (baseAdd * 1.5);
			}
			if (isSuper)
			{
				baseAdd *= 2;
			}
			return baseAdd;
		}

		public void BuffConHP(string user = "Mayhem", string message = "")
		{
			if (buffConHPCount >= 2)
			{
				queuedActions.Add(new QueuedAction { Name = "Face Tank", Type = ActionType.Blessing, Invoker = new MethodInvoker(() => FaceTank(user)) });
				return;
			}

			buffConHPActive = true;
			bool meterFull = MayhemMeterFull();

			string name = KhaosActionNames.BuffConHP;
			string timerName = $"Buff({name})";

			float currentHpPercentage = (float)((1.00*(sotnApi.AlucardApi.CurrentHp)) / sotnApi.AlucardApi.MaxtHp);

			if (currentHpPercentage < 1)
			{
				currentHpPercentage = 1;
			}

			sotnApi.AlucardApi.ActivatePotion(Potion.Antivenom);
			uint hpGivenNew = (uint) ((sotnApi.AlucardApi.MaxtHp * Constants.Khaos.BuffHPMultiplier) - sotnApi.AlucardApi.MaxtHp);
			hpGiven += hpGivenNew;
			sotnApi.AlucardApi.MaxtHp += hpGivenNew;

			if (meterFull) {
				name = "Super " + name;
				timerName = "Super " + timerName;
				SpendMayhemMeter();
				superBuffConHP = true;
				++superBuffConHPCount;
				sotnApi.AlucardApi.ActivatePotion(Potion.ResistCurse);
				sotnApi.AlucardApi.ActivatePotion(Potion.ResistStone);
				sotnApi.AlucardApi.ActivatePotion(Potion.ResistFire);
				sotnApi.AlucardApi.ActivatePotion(Potion.ResistIce);
				sotnApi.AlucardApi.ActivatePotion(Potion.ResistThunder);
				sotnApi.AlucardApi.ActivatePotion(Potion.ResistHoly);
				sotnApi.AlucardApi.ActivatePotion(Potion.ResistDark);
			}


			buffConHPHolyGlasses = checkHasHolyGlasses();
			if (buffConHPHolyGlasses)
			{
				++buffConHpHolyGlassesCount;
			}

			uint tempCon = CalculateTempStatGiven(Constants.Khaos.BuffCon, superBuffConHP, buffConHPHolyGlasses);
			addCONGiven(tempCon);

			sotnApi.AlucardApi.CurrentHp = (uint)(sotnApi.AlucardApi.MaxtHp * currentHpPercentage);

			System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffConHP).FirstOrDefault().Duration);

			++buffConHPCount;
			buffConHPTimer.Interval = (int) newDuration.TotalMilliseconds;
			buffConHPTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = timerName,
				Type = Enums.ActionType.Blessing,
				Duration = newDuration
			});

			if(message == "") 
			{
				message = $"{user} used {name}";
			}
			else
			{
				message += $"{name}";
			}

			notificationService.AddMessage(message);
			Alert(KhaosActionNames.BuffConHP);
		}

		private void BuffConHPOff(Object sender, EventArgs e)
		{
			if (buffConHPCount < 1)
			{
				buffConHPCount = 1;
			}

			uint hpTaken = 0;
			bool isStatsPaused = givenStatsPaused;
			
			if(isStatsPaused)
			{
				hpTaken = (uint) (hpGivenPaused / buffConHPCount);
				hpGivenPaused -= hpTaken;
			}
			else
			{
				hpTaken = (uint) (hpGiven / buffConHPCount);
				hpGiven -= hpTaken;

				if (sotnApi.AlucardApi.MaxtHp - hpTaken < minHP)
				{
					sotnApi.AlucardApi.MaxtHp = (uint) minHP;
				}
				else
				{
					sotnApi.AlucardApi.MaxtHp -= (uint) hpTaken;
				}
			}

			--buffConHPCount;

			uint conGiven = 0;

			if(buffConHpHolyGlassesCount >= 1)
			{
				buffConHPHolyGlasses = true;
				--buffConHpHolyGlassesCount;
			}
			else if(buffConHpHolyGlassesCount < 1)
			{
				buffConHPHolyGlasses = false;
			}

			if (superBuffConHPCount > 0) 
			{
				conGiven = CalculateTempStatGiven(Constants.Khaos.BuffCon, true, buffConHPHolyGlasses);
				--superBuffConHPCount;
				if(superBuffConHPCount < 1)
				{
					superBuffConHP = false;
				}
			}
			else
			{
				conGiven = CalculateTempStatGiven(Constants.Khaos.BuffCon, false, buffConHPHolyGlasses);
			}

			takeCONGiven(conGiven);

			if (sotnApi.AlucardApi.CurrentHp > (2 * sotnApi.AlucardApi.MaxtHp))
			{
				sotnApi.AlucardApi.CurrentHp = 2 * sotnApi.AlucardApi.MaxtHp;
			}

			//Console.WriteLine($"hpGiven = {hpGiven}, hpTaken {hpTaken}, faceCount{buffConHPCount}");

			if (buffConHPCount > 0)
			{
				buffConHPTimer.Stop(); 
				buffConHPTimer.Tick += BuffConHPOff;
				buffConHPTimer.Interval = normalInterval;
				buffConHPTimer.Start();
			}
			else
			{

				buffConHPActive = false;
				buffConHPTimer.Stop();
			}

		}
		public void BuffIntMP(string user = "Mayhem", string message = "")
		{
			buffIntMPActive = true;
			mpLocked = true;

			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.BuffIntMP;
			string timerName = $"Buff({name})";

			System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffIntMP).FirstOrDefault().Duration);
			buffIntMPActive = true;

			if (meterFull)
			{
				name = "Super " + name;
				timerName = "Super " + timerName;
				SpendMayhemMeter();
				superBuffIntMP = true;
				newDuration += newDuration;

				string item = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)];
				sotnApi.AlucardApi.GrantItemByName(item);

				uint newArmor = (uint)(Equipment.Items.IndexOf("Mojo mail") - Equipment.HandCount - 1);
				sotnApi.AlucardApi.Armor = newArmor;
			}

			buffIntMPHolyGlasses = checkHasHolyGlasses();
			uint tempInt = CalculateTempStatGiven(Constants.Khaos.BuffInt, superBuffIntMP, buffIntMPHolyGlasses);
			addINTGiven(tempInt);

			sotnApi.AlucardApi.ActivatePotion(Potion.Uncurse);
			//Cheat manaCheat = cheats.GetCheatByName("Mana");
			manaCheat.PokeValue(99);
			manaCheat.Enable();
			

			buffIntMPTimer.Interval = (int) newDuration.TotalMilliseconds;
			buffIntMPTimer.Start();
			
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = timerName,
				Type = Enums.ActionType.Blessing,
				Duration = newDuration
			});

			if(message == "")
			{
				message = $"{user} used {name}";
			}
			else
			{
				message += $"{name}";
			}
			notificationService.AddMessage(message);
			if (superBuffIntMP)
			{
				notificationService.AddMessage($"{user} equipped Mojo mail");
			}

			Alert(KhaosActionNames.BuffIntMP);
		}

		private void BuffIntMPOff(Object sender, EventArgs e)
		{
			uint tempInt = CalculateTempStatGiven(Constants.Khaos.BuffInt, superBuffIntMP, buffIntMPHolyGlasses);
			takeINTGiven(tempInt);
			manaCheat.Disable();
			sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
			buffIntMPTimer.Stop();
			mpLocked = false;
			superBuffIntMP = false;
			buffIntMPActive = false;
		}
		public void BuffStrRange(string user = "Mayhem", string message = "")
		{
			buffStrRangeActive = true;
			weaponQualitiesLocked = true;
			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.BuffStrRange;
			string timerName = $"Buff({name})";

			if (meterFull)
			{
				name = "Super " + name;
				timerName = "Super " + timerName;
				superBuffStrRange = true;
				sotnApi.AlucardApi.DarkMetamorphasisTimer = Constants.Khaos.BuffStrDarkMetamorphosis;
				SpendMayhemMeter();
			}

			
			sotnApi.AlucardApi.ActivatePotion(Potion.Uncurse);

			buffStrRangeHolyGlasses = checkHasHolyGlasses();
			uint tempStr = CalculateTempStatGiven(Constants.Khaos.BuffStr, superBuffStrRange, buffStrRangeHolyGlasses);
			addSTRGiven(tempStr);

			hitboxWidth.Enable();
			hitboxHeight.Enable();
			hitbox2Width.Enable();
			hitbox2Height.Enable();

			System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffStrRange).FirstOrDefault().Duration);

			buffStrRangeTimer.Interval = (int) newDuration.TotalMilliseconds;
			buffStrRangeTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = timerName,
				Type = Enums.ActionType.Blessing,
				Duration = newDuration
			});

			if(message == "")
			{
				message = $"{user} used {name}";
			}
			else
			{
				message += $"{name}";
			}

			notificationService.AddMessage(message);
			Alert(KhaosActionNames.BuffStrRange);
		}
		private void BuffStrRangeOff(Object sender, EventArgs e)
		{
			hitboxWidth.Disable();
			hitboxHeight.Disable();
			hitbox2Width.Disable();
			hitbox2Height.Disable();

			uint tempStr = CalculateTempStatGiven(Constants.Khaos.BuffStr, superBuffStrRange, buffStrRangeHolyGlasses);
			takeSTRGiven(tempStr);
			
			buffStrRangeTimer.Stop();
			superBuffStrRange = false;
			buffStrRangeActive = false;
			weaponQualitiesLocked = false;
		}

		public void BuffLckNeutral(string user = "Mayhem", string message = "")
		{
			buffLckNeutralActive = true;
			lockedNeutralLevel = 3;
			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.BuffLckNeutral;
			string timerName = $"Buff({name})";

			if (meterFull)
			{
				name = "Super " + name;
				timerName = "Super " + timerName;
				sotnApi.AlucardApi.GrantItemByName("Ring of Arcana");
				superBuffLckNeutral = true;
				SpendMayhemMeter();
			}

			buffLckNeutralHolyGlasses = checkHasHolyGlasses();
			uint tempLck = CalculateTempStatGiven(Constants.Khaos.BuffLck, superBuffLckNeutral, buffLckNeutralHolyGlasses);
			addLCKGiven(tempLck);

			System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffLckNeutral).FirstOrDefault().Duration);

			buffLckNeutralTimer.Interval = (int) newDuration.TotalMilliseconds;
			buffLckNeutralTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = timerName,
				Type = Enums.ActionType.Blessing,
				Duration = newDuration
			});

			if (message == "")
			{
				message = $"{user} used {name}";
			}
			else
			{
				message += $"{name}";
			}

			notificationService.AddMessage(message);
			if (meterFull)
			{
				notificationService.AddMessage($"{user}: 1 Ring of Arcana");
			}
			Alert(KhaosActionNames.BuffLckNeutral);
		}
		private void BuffLckNeutralOff(Object sender, EventArgs e)
		{
			uint tempLck = CalculateTempStatGiven(Constants.Khaos.BuffLck, superBuffLckNeutral, buffLckNeutralHolyGlasses);
			takeLCKGiven(tempLck);
			buffLckNeutralTimer.Stop();
			lockedNeutralLevel = 0;
			mpLocked = false;
			superBuffLckNeutral = false;
			buffLckNeutralActive = false;
		}

		public void Summoner(string user = "Mayhem")
		{
			spawnActive = true;
			summonerTriggerRoomX = sotnApi.GameApi.RoomX;
			summonerTriggerRoomY = sotnApi.GameApi.RoomY;

			summonerTimer.Start();
			summonerSpawnTimer.Start();

			summonerEnemies.Clear();
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
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.GameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone) || IsInRoomList(Constants.Khaos.ClockRoom))
			{
				return;
			}

			uint zone2 = sotnApi.GameApi.Zone2;

			if (summonerZone != zone2)
			{
				summonerEnemies.Clear();
				summonerZone = zone2;
			}

			FindSummonerEnemy();

			if (summonerEnemies.Count > 0)
			{
				int enemyIndex = rng.Next(0, summonerEnemies.Count);
				if (summonerTimer.Interval >= 5 * (60 * 1000))
				{

					summonerTimer.Stop();
					System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault().Duration);
					summonerTimer.Interval = (int) newDuration.TotalMilliseconds;
					notificationService.AddTimer(new Services.Models.ActionTimer
					{
						Name = KhaosActionNames.Summoner,
						Type = Enums.ActionType.Blessing,
						Duration = newDuration
					});
					summonerTimer.Start();

				}
				summonerEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, 245);
				summonerEnemies[enemyIndex].Ypos = (ushort) rng.Next(10, 245);
				summonerEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
				if (!IsInRoomList(Constants.Khaos.ClockRoom))
				{
					sotnApi.EntityApi.SpawnEntity(summonerEnemies[enemyIndex], false);
				}
			}
		}
		private bool FindSummonerEnemy()
		{
			uint roomX = sotnApi.GameApi.RoomX;
			uint roomY = sotnApi.GameApi.RoomY;

			if ((roomX == summonerTriggerRoomX && roomY == summonerTriggerRoomY) || !sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || IsInRoomList(Constants.Khaos.ClockRoom))
			{
				return false;
			}

			long enemy = sotnApi.EntityApi.FindEntityFrom(toolConfig.Khaos.RomhackMode ? Constants.Khaos.AcceptedRomhackAmbushEnemies : Constants.Khaos.AcceptedAmbushEnemies);

			if (enemy > 0)
			{
				Entity? summonerEnemy = new Entity(sotnApi.EntityApi.GetEntity(enemy));

				if (summonerEnemy is not null && !summonerEnemies.Where(e => e.AiId == summonerEnemy.AiId).Any())
				{
					summonerEnemies.Add(summonerEnemy);
					Console.WriteLine($"Added {KhaosActionNames.Summoner} ally with hp: {summonerEnemy.Hp} sprite: {summonerEnemy.AiId} damage: {summonerEnemy.Damage}");
					return true;
				}
			}

			return false;
		}

		public void GiveBoon(string user = "Mayhem", bool isStatsOnly = false, uint forcedCategory = 0)
		{
			Relic relic;
			string message = "";
			//string name = "";
			bool isValidRelic = false;
			bool isSuper = false;
			uint statRoll = 0;
			uint categoryRoll = 0;

			bool meterFull = MayhemMeterFull();
			if (meterFull)
			{
				isSuper = true;
				SpendMayhemMeter();
				statRoll += 1;
				RollRewards(out relic, out categoryRoll, out isValidRelic, out message, user, forcedCategory, isSuper, isStatsOnly);
				GiveRewards(user, relic, isValidRelic, message);
				statRoll += categoryRoll;
			}

			RollRewards(out relic, out categoryRoll, out isValidRelic, out message, user, forcedCategory, isSuper, isStatsOnly);
			GiveRewards(user, relic, isValidRelic, message);
			statRoll += categoryRoll;

			if (statRoll > 0)
			{

				sotnApi.AlucardApi.MaxtHp += statRoll * 10;
				sotnApi.AlucardApi.CurrentHp += statRoll * 10;
				sotnApi.AlucardApi.MaxtMp += statRoll * 10;
				sotnApi.AlucardApi.CurrentMp += statRoll * 10;
				sotnApi.AlucardApi.MaxtHearts += statRoll * 10;
				sotnApi.AlucardApi.CurrentHearts += statRoll * 10;
				sotnApi.AlucardApi.Str += statRoll;
				sotnApi.AlucardApi.Con += statRoll;
				sotnApi.AlucardApi.Int += statRoll;
				sotnApi.AlucardApi.Lck += statRoll;
			}

			Alert(KhaosActionNames.Boon);

			void RollRewards(out Relic relic, out uint categoryRoll, out bool isValidRelic, out string message, string user, uint forcedCategory, bool isSuper, bool isStatsOnly)
			{
				isValidRelic = false;
				relic = Relic.FireOfBat;
				if (forcedCategory > 0)
				{
					categoryRoll = forcedCategory;
				}
				else
				{
					categoryRoll = (uint)(rng.Next(1, 4));
				}

				message = $"{user} gave ";
				string messageEnd = "";
				switch (categoryRoll)
				{
					case 1:
						if (!isStatsOnly)
						{
							MinorRelic(out relic, out isValidRelic);
						}
						
						if (isValidRelic)
						{
							messageEnd += $"{relic}";
						}
						else
						{
							messageEnd += $"{KhaosActionNames.MinorStats}";
						}
						break;
					case 2:
						if (!isStatsOnly)
						{
							ModerateRelic(out relic, out isValidRelic);
						}
						if (isValidRelic)
						{
							messageEnd += $"{relic}";
						}
						else
						{
							messageEnd += $"{KhaosActionNames.ModerateStats}";
						}
							
						break;
					case 3:
						if (!isStatsOnly)
						{
							MajorRelic(out relic, out isValidRelic);
						}
						if (isValidRelic)
						{
							messageEnd += $"{relic}";
						}
						else 
						{
							messageEnd += $"{KhaosActionNames.MajorStats}";
						}
						break;
				}
				if (isSuper && (!isValidRelic || isStatsOnly))
				{
					message += "Super ";
				}

				message += messageEnd;

				if (isValidRelic)
				{
					categoryRoll = 0;
				}
			}
				
			void GiveRewards(string user, Relic relic, bool isValidRelic, string message)
			{
				if (isValidRelic)
				{
					sotnApi.AlucardApi.GrantRelic(relic);
				}
				notificationService.AddMessage(message);
				Alert(KhaosActionNames.Boon);
			}
		}
		#endregion

		public void GiveProgression(string user = "Mayhem", bool isStatsOnly = false)
		{
			Relic relic;
			//string name = "";
			string message = "";
			bool isValidRelic = false;
			bool isSuper = false;
			uint statRoll = 0;
			uint categoryRoll = 0;

			bool meterFull = MayhemMeterFull();
			if (meterFull)
			{
				isSuper = true;
				SpendMayhemMeter();
				statRoll += 2;
				sotnApi.AlucardApi.GrantItemByName("Manna prism");
				sotnApi.AlucardApi.GrantItemByName("Manna prism");
				sotnApi.AlucardApi.GrantItemByName("Manna prism");
				sotnApi.AlucardApi.GrantItemByName("Manna prism");
				sotnApi.AlucardApi.GrantItemByName("Manna prism");
				sotnApi.AlucardApi.GrantItemByName("Elixir");
				sotnApi.AlucardApi.GrantItemByName("Elixir");
				RollRewards(out relic, out categoryRoll, out isValidRelic, out message, isSuper, isStatsOnly);
				GiveRewards(user, relic, isValidRelic, message);
				statRoll += categoryRoll;
			}

			RollRewards(out relic, out categoryRoll, out isValidRelic, out message, isSuper, isStatsOnly);
			GiveRewards(user, relic, isValidRelic, message);

			if (isSuper)
			{
				notificationService.AddMessage($"{user}: 5 Mana prism");
				notificationService.AddMessage($"{user}: 2 Elixir");
			}

			statRoll += categoryRoll;

			if (hasHolyGlasses)
			{
				statRoll += 2;
			}

			if (statRoll > 0)
			{

				sotnApi.AlucardApi.MaxtHp += statRoll * 10;
				sotnApi.AlucardApi.CurrentHp += statRoll * 10;
				sotnApi.AlucardApi.MaxtMp += statRoll * 10;
				sotnApi.AlucardApi.CurrentMp += statRoll * 10;
				sotnApi.AlucardApi.MaxtHearts += statRoll * 10;
				sotnApi.AlucardApi.CurrentHearts += statRoll * 10;
				sotnApi.AlucardApi.Str += statRoll;
				sotnApi.AlucardApi.Con += statRoll;
				sotnApi.AlucardApi.Int += statRoll;
				sotnApi.AlucardApi.Lck += statRoll;
			}

			Alert(KhaosActionNames.Boon);

			void RollRewards(out Relic relic, out uint categoryRoll, out bool isValidRelic, out string message, bool isSuper, bool isStatsOnly)
			{
				isValidRelic = false;
				relic = Relic.FireOfBat;

				categoryRoll = 5;

				message = $"{user} gave ";

				if (!isStatsOnly)
				{
					ProgressionRelic(out relic, out isValidRelic);
				}

				if (isValidRelic)
				{
					message += $"{relic}";
				}
				else
				{
					if (isSuper)
					{
						message += "Super ";
					}
					message += $"{KhaosActionNames.ProgressionStats}";
				}
				
				if (isValidRelic)
				{
					categoryRoll = 0;
				}
			}

			void GiveRewards(string user, Relic relic, bool isValidRelic, string message)
			{
				if (isValidRelic)
				{
					sotnApi.AlucardApi.GrantRelic(relic);
				}
				notificationService.AddMessage(message);
				Alert(KhaosActionNames.Boon);
			}
		}
		#endregion

		public void Update()
		{
			if (!sotnApi.GameApi.InAlucardMode())
			{
				allowPity = true;
				accelTime.PokeValue(5);
				if (!givenStatsPaused)
				{
					takeHPGiven();
					takeSTRGiven();
					takeCONGiven();
					takeINTGiven();
					takeLCKGiven();
					givenStatsPaused = true;
				}
			}
			else
			{
				if(AutoMayhemOn && allowPity && sotnApi.AlucardApi.CurrentHp <= 0)
				{
					allowPity = false;
					autoMayhemPity();
				}

				if (resetColorWhenAlucard)
				{
					UpdateVisualEffect();
				}
				
				if (HPForMPActive)
				{
					CheckMPUsage();
				}
				CheckDashInput();

				if (accelTimeActive && sotnApi.GameApi.CanMenu() && !sotnApi.GameApi.CanSave() && !sotnApi.GameApi.IsInMenu())
				{
					accelTime.PokeValue(0);
				}
				else
				{
					accelTime.PokeValue(5);
				}

				if (sotnApi.GameApi.SecondCastle)
				{
					if (isAlucardColorFirstCastle)
					{
						UpdatePlayerColor();
						UpdateVisualEffect();
					}

					if(!allowSecondCastleRewind && ((sotnApi.GameApi.CanSave() && alucardMapY != 54) || IsInRoomList(Constants.Khaos.LoadingRooms)))
					{
						allowSecondCastleRewind = true;
					}
						allowFirstCastleRewind = true;
				}
				else
				{
					if (isAlucardColorSecondCastle)
					{
						UpdatePlayerColor();
						UpdateVisualEffect();
					}
					if (allowFirstCastleRewind && IsInRoomList(Constants.Khaos.RewindBanRoom))
					{
						allowFirstCastleRewind = false;
					}
					else if (!allowFirstCastleRewind && (IsInRoomList(Constants.Khaos.LoadingRooms) || IsInRoomList(Constants.Khaos.RewindUnbanRoom)))
					{
						allowFirstCastleRewind = true;
					}
					allowSecondCastleRewind = false;
				}

				if (sotnApi.GameApi.CanSave())
				{
					if (!givenStatsPaused) 
					{
						takeHPGiven();
						takeSTRGiven();
						takeCONGiven();
						takeINTGiven();
						takeLCKGiven();
						givenStatsPaused = true;
					}
					//Ensure temporary buffs and relics are removed in save rooms
					if (hexActive)
					{
						if (hexHPMPHActive && !hexHPMPHPaused)
						{ 
							hexHPMPHPaused = true;
							HexReturnHPMPH();
						}
						if (hexStatsActive && !hexStatsPaused)
						{
							hexStatsPaused = true;
							HexReturnStats();
						}
						if (hexWeaponsActive && !hexWeaponsPaused)
						{
							hexWeaponsPaused = true;
							HexReturnWeapons();
						}
						if (hexRelicsActive && !hexRelicsPaused)
						{
							hexRelicsPaused = true;
							HexReturnRelics();
						}
					}
				}
				else
				{
					//Ensure temporary buffs and relics are added when leaving save rooms.
					if (givenStatsPaused)
					{
						addHPGiven();
						addSTRGiven();
						addCONGiven();
						addINTGiven();
						addLCKGiven();
						givenStatsPaused = false;
					}
					if (hexActive)
					{
						if (hexHPMPHActive && hexHPMPHPaused)
						{
							hexHPMPHPaused = false;
							HexTakeHPMPH();
						}
						if (hexStatsActive && hexStatsPaused)
						{
							hexStatsPaused = false;
							HexTakeStats();
						}
						if (hexWeaponsActive && hexWeaponsPaused)
						{
							hexWeaponsPaused = false;
							if ((rightHandReturned == 0 && leftHandReturned == 0))
							{
								HexTakeWeapons();
							}
							else
							{
								bool returnWeaponsOnly = true;
								HexTakeWeapons(returnWeaponsOnly);
							}
						}
						if (hexRelicsActive && hexRelicsPaused)
						{
								hexRelicsPaused = false;
								HexTakeRelics();
						}
					}
				}

				if (turboModeActive)
				{
					if (IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.MistGateRoom))
					{
						turboMode.Disable();
						turboModeJump.Disable();
					}
					else
					{
						turboMode.Enable();
						turboModeJump.Enable();
					}
				}
				else
				{
					turboMode.Disable();
					turboModeJump.Disable();
				}

				if (underwaterActive)
				{
					if (!underwaterPaused && (sotnApi.GameApi.CanWarp() || IsInRoomList(Constants.Khaos.ClockRoom) || IsInRoomList(Constants.Khaos.MistGateRoom) || !sotnApi.AlucardApi.HasControl()))
					{
						underwaterPaused = true;
						underwaterPhysics.PokeValue(0);
						if (IsInRoomList(Constants.Khaos.ClockRoom) || IsInRoomList(Constants.Khaos.MistGateRoom) || !sotnApi.AlucardApi.HasControl())
						{
							SetSpeed((float)1);
						}
					}
					else if(underwaterPaused && (!sotnApi.GameApi.CanWarp() && !IsInRoomList(Constants.Khaos.ClockRoom) && !IsInRoomList(Constants.Khaos.MistGateRoom) && sotnApi.AlucardApi.HasControl()))
					{
						underwaterPaused = false;
						underwaterPhysics.PokeValue(144);
						SetSpeed((float) underwaterBaseFactor * underwaterMayhemFactor);
					}
				}
				else if (IsInRoomList(Constants.Khaos.MistGateRoom))
				{
					underwaterPhysics.PokeValue(0);
				}

				bool holyGlassesCheck = false;
				if (sotnApi.AlucardApi.HasItemInInventory("Holy glasses") || Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses")
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
				if (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad) || eyeOfVladTaken)
				{
					++newVladCount;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad) || heartOfVladTaken)
				{
					++newVladCount;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.RibOfVlad) || ribOfVladTaken)
				{
					++newVladCount;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.RingOfVlad) || ringOfVladTaken)
				{
					++newVladCount;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.ToothOfVlad) || toothOfVladTaken)
				{
					++newVladCount;
				}

				if (newVladCount > (int) vladRelicsObtained)
				{
					notificationService.AddMessage("Vlad: Curses are now stronger");
					if (AutoMayhemOn && newVladCount - vladRelicsObtained == 1)
					{
						if (toolConfig.Khaos.autoAllowMayhemRage)
						{
							autoMayhemRage();
						}
					}
				}

				vladRelicsObtained = newVladCount;

				if (rushDownActive || heartsOnlyActive)
				{
					if (sotnApi.AlucardApi.RightHand == 0)
					{
						sotnApi.AlucardApi.RightHand = (uint) Equipment.Items.IndexOf("Pizza");
						if (sotnApi.AlucardApi.HasItemInInventory("Pizza"))
						{
							sotnApi.AlucardApi.TakeOneItemByName("Pizza");
						}
					}
					if (sotnApi.AlucardApi.LeftHand == 0)
					{
						sotnApi.AlucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Pizza");
						if (sotnApi.AlucardApi.HasItemInInventory("Pizza"))
						{
							sotnApi.AlucardApi.TakeOneItemByName("Pizza");
						}
					}

					if (rushDownActive)
					{
						if (sotnApi.AlucardApi.HasControl() && sotnApi.GameApi.CanMenu())
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
					else
					{
						if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
						{
							sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
							gasCloudTaken = true;
						}
						while (sotnApi.AlucardApi.Subweapon == Subweapon.Empty || sotnApi.AlucardApi.Subweapon == Subweapon.Stopwatch)
						{
							RandomizeSubweapon();
						}
					}
				}
				else if (unarmedActive)
				{
					sotnApi.AlucardApi.Subweapon = 0;
					if (sotnApi.AlucardApi.RightHand != 0)
					{
						if (sotnApi.AlucardApi.HasItemInInventory(Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)]))
						{
							sotnApi.AlucardApi.TakeOneItemByName(Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)]);
						}
						sotnApi.AlucardApi.GrantItemByName(Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)]);
						sotnApi.AlucardApi.RightHand = (uint) Equipment.Items.IndexOf("empty hand");
					}
					if (sotnApi.AlucardApi.LeftHand != 0)
					{
						if (sotnApi.AlucardApi.HasItemInInventory(Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)]))
						{
							sotnApi.AlucardApi.TakeOneItemByName(Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)]);
						}
						sotnApi.AlucardApi.GrantItemByName(Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)]);
						sotnApi.AlucardApi.LeftHand = (uint) Equipment.Items.IndexOf("empty hand");
					}
					if (!unarmedPaused && sotnApi.GameApi.CanSave())
					{
						unarmedPaused = true;
						ReturnUnarmedRelics();
					}
					if (unarmedPaused && !sotnApi.GameApi.CanSave())
					{
						unarmedPaused = false;
						RemoveUnarmedRelics();
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
				#region Debug Commands
				case "logcurrentroom":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.LogCurrentRoom).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => LogCurrentRoom(user)));
					}
					break;
				case "rewind":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Rewind).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Rewind(user)));
					}
					break;
				case "minstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinStats).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MinStats(user)));
					}
					break;
				case "library":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Library).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Library", ChangesAlucardEffects = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => Library(user)) });
					}
					break;
				#endregion
				#region Neutral Commands
				case "merchant":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Merchant).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Merchant(user)));
					}
					break;
				case "maxmayhem":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MaxMayhem).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MaxMayhem(user)));
					}
					break;
				case "heartsonly":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeartsOnly).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Hearts Only", LocksHearts = true, LocksWeapons = true, LocksMana = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => HeartsOnly(user)) });
					}
					break;
				case "unarmed":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Unarmed).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Unarmed", LocksHearts = true, LocksWeapons = true, ChangesWeaponQualities = true, ChangesSubWeapons = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => Unarmed(user)) });
					}
					break;
				case "turbomode":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TurboMode).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Turbo Name", Type = ActionType.Neutral, Invoker = new MethodInvoker(() => TurboMode(user)) });
					}
					break;
				case "rushdown":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RushDown).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Rushdown", ChangesAlucardEffects = true, LocksMana = true, LocksSpeed = true, LocksWeapons = true, LocksInvincibility = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => RushDown(user)) });
					}
					break;
				case "swapstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SwapStats).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Swap Stats", ChangesStats = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => SwapStats(user)) });
					}
					break;
				case "swapequipment":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SwapEquipment).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Swap Equipment", ChangesWeapons = true, ChangesSubWeapons = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => SwapEquipment(user)) }); ;
					}
					break;
				case "swaprelics":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SwapRelics).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Swap Relics", Type = ActionType.Neutral, Invoker = new MethodInvoker(() => SwapRelics(user)) });
					}
					break;
				case "pandemonium":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Pandemonium).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Pandemonium", ChangesStats = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => Pandemonium(user)) });
					}
					break;
				#endregion
				#region Negative Commands
				case "minortrap":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinorTrap).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MinorTrap(user)));
					}
					break;
				case "slam":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Slam).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user,false,true,false)));
					}
					break;
				case "slamjam":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SlamJam).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user, false, true,true)));
					}
					break;
				case "hpformp":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "HP For MP", LocksMana = true, Invoker = new MethodInvoker(() => HPForMP(user)) });
					}
					break;
				case "underwater":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Underwater).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Underwater", LocksSpeed = true, Invoker = new MethodInvoker(() => Underwater(user)) });
					}
					break;
				case "trap":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Trap).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Trap(user)));
					}
					break;
				case "moderatetrap":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ModerateTrap).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ModerateTrap(user)));
					}
					break;
				case "hex":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Hex).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Hex", ChangesStats = true, Invoker = new MethodInvoker(() => Hex(user)) });
					}
					break;
				case "getjuggled":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.GetJuggled).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						if (rushDownActive)
						{
							queuedActions.Add(new QueuedAction { Name = "GetJuggled", LocksInvincibility = true, Invoker = new MethodInvoker(() => GetJuggled(user)) });
						}
						else
						{
							queuedFastActions.Enqueue(new MethodInvoker(() => GetJuggled(user)));
						}
					}
					break;
				case "ambush":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Ambush).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Ambush", LocksSpawning = true, Invoker = new MethodInvoker(() => Ambush(user)) });
					}
					break;
				case "majortrap":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MajorTrap).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MajorTrap(user)));
					}
					break;
				case "toughbosses":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ToughBosses).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ToughBosses(user)));
					}
					break;
				case "statsdown":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.StatsDown).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Stat Down", ChangesStats = true, Invoker = new MethodInvoker(() => StatsDown(user)) });
					}
					break;
				case "confiscate":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Confiscate).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Confiscate", Invoker = new MethodInvoker(() => Confiscate(user)) });
					}
					break;
				#endregion
				#region Positive commands
				case "minorboon":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinorBoon).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MinorBoon(user)));
					}
					break;
				case "minorequipment":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinorEquipment).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MinorEquipment(user)));
					}
					break;
				case "minoritems":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinorItems).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MinorItems(user)));
					}
					break;
				case "minorstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MinorStats).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MinorStats(user)));
					}
					break;
				case "potions":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Potions).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => GivePotions(user)));
					}
					break;
				case "regen":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Regen).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Regen", Type = ActionType.Blessing, LocksMana = true, Invoker = new MethodInvoker(() => Regen(user)) });
					}
					break;
				case "speed":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Speed).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Speed", Type = ActionType.Blessing, LocksSpeed = true, Invoker = new MethodInvoker(() => Speed(user)) });
					}
					break;
				case "moderateboon":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ModerateBoon).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ModerateBoon(user)));
					}
					break;
				case "moderateequipment":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ModerateEquipment).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ModerateEquipment(user)));
					}
					break;
				case "moderateitems":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ModerateItems).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ModerateItems(user)));
					}
					break;
				case "moderatestats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ModerateStats).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ModerateStats(user)));
					}
					break;
				case "items":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Items).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => GiveItems(user)));
					}
					break;
				case "equipment":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Equipment).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => GiveEquipment(user)));
					}
					break;
				case "buff":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Buff).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Buff", ChangesStats = true, Type = ActionType.Blessing, Invoker = new MethodInvoker(() => GiveBuff(user)) });
					}
					break;
				case "extrarange":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffStrRange).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "BuffSTRRange", ChangesWeaponQualities = true, Type = ActionType.Blessing, Invoker = new MethodInvoker(() => ExtraRange(user)) });
					}
					break;
				case "facetank":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffConHP).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "BuffConHP", Type = ActionType.Blessing, Invoker = new MethodInvoker(() => FaceTank(user)) });
					}
					break;
				case "spellcaster":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffIntMP).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "SpellCaster", Type = ActionType.Blessing, LocksMana = true, Invoker = new MethodInvoker(() => SpellCaster(user)) });
					}
					break;
				case "lucky":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffLckNeutral).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Lucky", ChangesWeaponQualities = true, Type = ActionType.Blessing, Invoker = new MethodInvoker(() => Lucky(user)) });
					}
					break;
				case "timestop":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TimeStop).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => TimeStop(user)));
					}
					break;
				case "summoner":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "Summoner", LocksSpawning = true, Type = ActionType.Blessing, Invoker = new MethodInvoker(() => Summoner(user)) });
					}
					break;
				case "majorboon":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MajorBoon).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MajorBoon(user)));
					}
					break;
				case "majorequipment":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MajorEquipment).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MajorEquipment(user)));
					}
					break;
				case "majoritems":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MajorItems).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MajorItems(user)));
					}
					break;
				case "majorstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.MajorStats).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => MajorStats(user)));
					}
					break;
				case "boon":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Boon).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => GiveBoon(user)));
					}
					break;
				case "progression":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Progression).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => GiveProgression(user)));
					}
					break;
				case "progressionstats":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.ProgressionStats).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => ProgressionStats(user)));
					}
					break;
				#endregion
				default:
					commandAction = null;
					break;

			}
			if (commandAction is not null)
			{
				GainMayhemMeter(commandAction.Meter);
			}
		}
		private void InitializeTimers()
		{
			fastActionTimer.Tick += ExecuteFastAction;
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Tick += ExecuteAction;
			actionTimer.Interval = 2 * (1 * 1000);
			
			if (AutoMayhemOn)
			{
				autoActionTimer.Tick += ExecuteAutoAction;
				switch (toolConfig.Khaos.autoCommandSpeed)
				{
					case 1:
						autoActionTimer.Interval = 5 * (1 * 1500);
						break;
					case 2:
						autoActionTimer.Interval = 5 * (1 * 1400);
						break;
					case 3:
						autoActionTimer.Interval = 5 * (1 * 1300);
						break;
					case 4:
						autoActionTimer.Interval = 5 * (1 * 1200);
						break;
					case 5:
						autoActionTimer.Interval = 5 * (1 * 1100);
						break;
					default:
						autoActionTimer.Interval = 5 * (1 * 1600);
						break;
				}
			}

			#region Timers

			rewindTimer.Tick += RewindOff;
			rewindTimer.Interval = 10;
			libraryTimer.Tick += LibraryOff;
			libraryTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Library).FirstOrDefault().Duration.TotalMilliseconds;

			hpForMPDeathTimer.Tick += KillAlucard;
			hpForMPDeathTimer.Interval = 1 * (1 * 1500);
			HPForMPTimer.Tick += HPForMPOff;
			HPForMPTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault().Duration.TotalMilliseconds;

			heartsOnlyTimer.Tick += HeartsOnlyOff;
			heartsOnlyTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeartsOnly).FirstOrDefault().Duration.TotalMilliseconds;
			underwaterTimer.Tick += UnderwaterOff;
			underwaterTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Underwater).FirstOrDefault().Duration.TotalMilliseconds;
			hexTimer.Tick += HexOff;
			hexTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Hex).FirstOrDefault().Duration.TotalMilliseconds;
			slamJamTimer.Tick += SlamJamOff;
			slamJamTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SlamJam).FirstOrDefault().Duration.TotalMilliseconds;
			slamJamTickTimer.Tick += SlamJam;
			slamJamTickTimer.Interval = 300;

			getJuggledTimer.Tick += getJuggledOff;
			getJuggledTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.GetJuggled).FirstOrDefault().Duration.TotalMilliseconds;
			ambushTimer.Tick += AmbushOff;
			ambushTimer.Interval = 5 * (60 * 1000);
			ambushSpawnTimer.Tick += AmbushSpawn;
			ambushSpawnTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Ambush).FirstOrDefault().Interval.TotalMilliseconds;
			toughBossesSpawnTimer.Tick += ToughBossesSpawn;
			toughBossesSpawnTimer.Interval = 1 * (1000);

			turboModeTimer.Tick += TurboModeOff;
			turboModeTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TurboMode).FirstOrDefault().Duration.TotalMilliseconds;
			unarmedTimer.Tick += UnarmedOff;
			unarmedTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Unarmed).FirstOrDefault().Duration.TotalMilliseconds;
			rushDownTimer.Tick += RushDownOff;
			rushDownTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RushDown).FirstOrDefault().Duration.TotalMilliseconds;
			
			regenTimer.Tick += RegenOff;
			regenTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Regen).FirstOrDefault().Duration.TotalMilliseconds;
			regenTickTimer.Tick += RegenGain;
			regenTickTimer.Interval = 1000;
			buffStrRangeTimer.Tick += BuffStrRangeOff;
			buffStrRangeTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffStrRange).FirstOrDefault().Duration.TotalMilliseconds;
			buffConHPTimer.Tick += BuffConHPOff;
			buffConHPTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffConHP).FirstOrDefault().Duration.TotalMilliseconds;
			buffIntMPTimer.Tick += BuffIntMPOff;
			buffIntMPTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffIntMP).FirstOrDefault().Duration.TotalMilliseconds;
			buffLckNeutralTimer.Tick += BuffLckNeutralOff;
			buffLckNeutralTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffLckNeutral).FirstOrDefault().Duration.TotalMilliseconds;
			timeStopTimer.Tick += TimeStopOff;
			timeStopTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TimeStop).FirstOrDefault().Duration.TotalMilliseconds;
			timeStopCheckTimer.Tick += TimeStopAreaCheck;
			timeStopCheckTimer.Interval += 2 * 1000;
			
			speedTimer.Tick += SpeedOff;
			speedTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Speed).FirstOrDefault().Duration.TotalMilliseconds;
			speedOverdriveTimer.Tick += SpeedOverdriveOn;
			speedOverdriveTimer.Interval = (2 * 1000);
			speedOverdriveOffTimer.Tick += SpeedOverdriveOff;
			speedOverdriveOffTimer.Interval = (2 * 1000);
			
			summonerTimer.Tick += SummonerOff;
			summonerTimer.Interval = 5 * (60 * 1000);
			summonerSpawnTimer.Tick += SummonerSpawn;
			summonerSpawnTimer.Interval = (int)toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault().Interval.TotalMilliseconds;
			#endregion
		}
		private void InitializeTimerIntervals()
		{
			fastActionTimer.Interval = 2 * (1 * 1000);
			actionTimer.Interval = 2 * (1 * 1000);

			if (AutoMayhemOn)
			{
				autoActionTimer.Tick += ExecuteAutoAction;
				switch (toolConfig.Khaos.autoCommandSpeed)
				{
					case 1:
						autoActionTimer.Interval = 5 * (1 * 1500);
						break;
					case 2:
						autoActionTimer.Interval = 5 * (1 * 1400);
						break;
					case 3:
						autoActionTimer.Interval = 5 * (1 * 1300);
						break;
					case 4:
						autoActionTimer.Interval = 5 * (1 * 1200);
						break;
					case 5:
						autoActionTimer.Interval = 5 * (1 * 1100);
						break;
					default:
						autoActionTimer.Interval = 5 * (1 * 1600);
						break;
				}
			}

			#region Command Timer Intervals

			rewindTimer.Interval = (int) 10;
			libraryTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Library).FirstOrDefault().Duration.TotalMilliseconds;

			hpForMPDeathTimer.Interval = 1 * (1 * 1500);
			HPForMPTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault().Duration.TotalMilliseconds;

			heartsOnlyTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeartsOnly).FirstOrDefault().Duration.TotalMilliseconds;
			underwaterTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Underwater).FirstOrDefault().Duration.TotalMilliseconds;
			hexTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Hex).FirstOrDefault().Duration.TotalMilliseconds;
			slamJamTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SlamJam).FirstOrDefault().Duration.TotalMilliseconds;
			slamJamTickTimer.Interval = 300;

			getJuggledTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.GetJuggled).FirstOrDefault().Duration.TotalMilliseconds;
			ambushTimer.Interval = 5 * (60 * 1000);
			ambushSpawnTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Ambush).FirstOrDefault().Interval.TotalMilliseconds;
			toughBossesSpawnTimer.Interval = 1 * (1000);

			turboModeTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TurboMode).FirstOrDefault().Duration.TotalMilliseconds;
			unarmedTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Unarmed).FirstOrDefault().Duration.TotalMilliseconds;
			rushDownTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RushDown).FirstOrDefault().Duration.TotalMilliseconds;

			regenTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Regen).FirstOrDefault().Duration.TotalMilliseconds;
			regenTickTimer.Interval = 1000;
			buffStrRangeTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffStrRange).FirstOrDefault().Duration.TotalMilliseconds;
			buffConHPTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffConHP).FirstOrDefault().Duration.TotalMilliseconds;
			buffIntMPTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffIntMP).FirstOrDefault().Duration.TotalMilliseconds;
			buffLckNeutralTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffLckNeutral).FirstOrDefault().Duration.TotalMilliseconds;
			timeStopTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TimeStop).FirstOrDefault().Duration.TotalMilliseconds;
			timeStopCheckTimer.Interval += 2 * 1000;

			speedTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Speed).FirstOrDefault().Duration.TotalMilliseconds;
			speedOverdriveTimer.Interval = (2 * 1000);
			speedOverdriveOffTimer.Interval = (2 * 1000);

			summonerTimer.Interval = 5 * (60 * 1000);
			summonerSpawnTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault().Interval.TotalMilliseconds;
			#endregion
		}

		private void StopTimers()
		{
			autoActionTimer.Stop();
			fastActionTimer.Stop();
			actionTimer.Stop();

			#region Mayhem timers
			libraryTimer.Interval = 1;
			rewindTimer.Interval = 1;

			turboModeTimer.Interval = 1;
			heartsOnlyTimer.Interval = 1;
			unarmedTimer.Interval = 1;
			rushDownTimer.Interval = 1;

			hpForMPDeathTimer.Stop();
			HPForMPTimer.Interval = 1;
			underwaterTimer.Interval = 1;
			hexTimer.Interval = 1;
			slamJamTimer.Interval = 1;
			getJuggledTimer.Interval = 1;
			ambushTimer.Interval = 1;
			ambushSpawnTimer.Interval = 1;
			toughBossesSpawnTimer.Interval = 1;

			regenTimer.Interval = 1;
			regenTickTimer.Interval = 1;
			timeStopTimer.Interval = 1;
			timeStopCheckTimer.Interval = 1;
			buffStrRangeTimer.Interval = 1;
			buffConHPTimer.Interval = 1;
			buffIntMPTimer.Interval = 1;
			buffLckNeutralTimer.Interval = 1;
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
		}
		private void ExecuteAction(Object sender, EventArgs e)
		{
			if (queuedActions.Count > 0)
			{
				bool keepRichterRoom = IsInKeepRichterRoom();
				alucardMapX = sotnApi.AlucardApi.MapX;
				alucardMapY = sotnApi.AlucardApi.MapY;

				if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave() && !IsInRoomList(Constants.Khaos.RichterRooms) && !IsInRoomList(Constants.Khaos.LoadingRooms) && !IsInRoomList(Constants.Khaos.ClockRoom) && !IsInRoomList(Constants.Khaos.ShopRoom))
				{
					int index = 0;
					bool actionUnlocked = true;

					for (int i = 0; i < queuedActions.Count; i++)
					{
						index = i;
						actionUnlocked = true;
						if (
							(queuedActions[i].ChangesAlucardEffects && alucardEffectsLocked) ||
							(queuedActions[i].ChangesStats && statLocked)||
							(queuedActions[i].ChangesWeaponQualities && weaponQualitiesLocked) ||
							(queuedActions[i].ChangesWeapons && weaponsLocked)||
							(queuedActions[i].ChangesSubWeapons && subWeaponsLocked)||
							(queuedActions[i].LocksSpeed && speedLocked)||
							(queuedActions[i].LocksMana && mpLocked)||
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
						Console.WriteLine($"All actions locked. alucardEffectsLocked{alucardEffectsLocked}, statlocked: {statLocked}, subWeaponsLocked: {subWeaponsLocked}, speed: {speedLocked}, mpLocked:{mpLocked}, weaponsLocked{weaponsLocked}, invincibility: {invincibilityLocked}, mana: {mpLocked}");
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
			alucardMapX = sotnApi.AlucardApi.MapX;
			alucardMapY = sotnApi.AlucardApi.MapY;
			CheckCastleChanged();
			CheckMainMenu();

			bool keepRichterRoom = IsInKeepRichterRoom();
			bool galamothRoom = IsInGalamothRoom();
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.AlucardApi.HasControl() && sotnApi.AlucardApi.HasHitbox() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave() && !keepRichterRoom && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading && !sotnApi.AlucardApi.IsInvincible() && !IsInRoomList(Constants.Khaos.LoadingRooms) && !IsInRoomList(Constants.Khaos.ClockRoom) && !IsInRoomList(Constants.Khaos.ClockRoom) && !IsInRoomList(Constants.Khaos.ShopRoom) && alucardMapX< 99)
			{
				shaftHpSet = false;
				if (queuedFastActions.Count > 0)
				{
					queuedFastActions.Dequeue()();
				}
			}
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& keepRichterRoom && !shaftHpSet && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading)
			{
				SetShaftHp();
			}
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
				&& galamothRoom && !galamothStatsSet && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading)
			{
				SetGalamothtStats();
			}
			if (!galamothRoom)
			{
				galamothStatsSet = false;
			}
		}

		private void ExecuteAutoAction(Object sender, EventArgs e)
		{
			if(!sotnApi.GameApi.InAlucardMode() || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.ClockRoom) || sotnApi.GameApi.CanSave())
			{
				return;
			}
			else if (queuedActions.Count <= 8)
			{
				if (autoMoodRemaining > 0)
				{
					--autoMoodRemaining;
					if (autoMoodRemaining == 0)
					{
						UpdateAutoMood(autoMoodType, true, false);
					}
				}
				AutoMayhemAction();
			}
		}

		private void KillAlucard(Object sender, EventArgs e)
		{
			Entity hitbox= new Entity();
			uint offsetPosX = sotnApi.AlucardApi.ScreenX - 255;
			uint offsetPosY = sotnApi.AlucardApi.ScreenY - 255;

			hitbox.Xpos = 0;
			hitbox.Ypos = 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.DamageTypeA = (uint) Entities.Slam;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 999;
			sotnApi.EntityApi.SpawnEntity(hitbox);
			sotnApi.AlucardApi.InvincibilityTimer = 0;
			mpLocked = false;
			//HPForMPOff(sender, e);
			//HPForMPTimer.Stop();
			hpForMPDeathTimer.Stop();
		}
		private void SpawnPoisonHitbox()
		{
			Entity hitbox= new();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeA = (uint) Entities.Poison;
			sotnApi.EntityApi.SpawnEntity(hitbox);
		}
		private void SpawnCurseHitbox()
		{
			Entity hitbox= new();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeB = (uint) Entities.Curse;
			sotnApi.EntityApi.SpawnEntity(hitbox);
		}
		private void SpawnStoneHitbox()
		{
			Entity hitbox= new();
			int roll = rng.Next(0, 2);
			hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;
			hitbox.Damage = 1;
			hitbox.DamageTypeA = (uint) Entities.Stone;
			hitbox.DamageTypeB = (uint) Entities.Stone;
			sotnApi.EntityApi.SpawnEntity(hitbox);
		}
		private void SpawnSlamHitbox(bool fixedFacing = false)
		{
			Entity hitbox= new Entity();
			int roll = rng.Next(0, 2);
			if (fixedFacing)
			{
				hitbox.Xpos = sotnApi.AlucardApi.FacingLeft == true ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			}
			else
			{
				hitbox.Xpos = roll == 1 ? (ushort) (sotnApi.AlucardApi.ScreenX + 1) : (ushort) 0;
			}
			hitbox.HitboxHeight = 255;
			hitbox.HitboxWidth = 255;
			hitbox.AutoToggle = 1;


			hitbox.Damage = (ushort) (sotnApi.AlucardApi.Def + 2);
			hitbox.DamageTypeA = (uint) Entities.Slam;
			sotnApi.EntityApi.SpawnEntity(hitbox);
		}
		
		private void SetHasteStaticSpeeds(bool super = false)
		{
			float superFactor = super ? 2F : 1F;
			float superWingsmashFactor = super ? 1.5F : 1F;
			float factor = toolConfig.Khaos.SpeedFactor;

			uint wolfDashTopLeft = DefaultSpeeds.WolfDashTopLeft;

			sotnApi.AlucardApi.WingsmashHorizontalSpeed = (uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5));
			sotnApi.AlucardApi.WolfDashTopRightSpeed = (sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2));
			sotnApi.AlucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) wolfDashTopLeft * ((factor * superFactor) / 2));
			//Console.WriteLine("Set speeds:");
			//Console.WriteLine($"Wingsmash: {(uint) (DefaultSpeeds.WingsmashHorizontal * ((factor * superWingsmashFactor) / 2.5))}");
			//Console.WriteLine($"Wolf dash right: {(sbyte) Math.Floor(DefaultSpeeds.WolfDashTopRight * ((factor * superFactor) / 2))}");
			//Console.WriteLine($"Wolf dash left: {(sbyte) Math.Ceiling((sbyte) DefaultSpeeds.WolfDashTopLeft * ((factor * superFactor) / 2))}");
		}
		private void ToggleHasteDynamicSpeeds(float factor = 1)
		{
			uint horizontalWhole = (uint) (DefaultSpeeds.WalkingWhole * factor);
			uint horizontalFract = (uint) (DefaultSpeeds.WalkingFract * factor);

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
		}

		private bool IsInGalamothRoom()
		{
			uint mapX = sotnApi.AlucardApi.MapX;
			uint mapY = sotnApi.AlucardApi.MapY;
			return (mapX >= Constants.Khaos.GalamothRoomMapMinX && mapX <= Constants.Khaos.GalamothRoomMapMaxX) && (mapY >= Constants.Khaos.GalamothRoomMapMinY && mapY <= Constants.Khaos.GalamothRoomMapMaxY);
		}
		private bool IsInKeepRichterRoom()
		{
			uint mapX = sotnApi.AlucardApi.MapX;
			uint mapY = sotnApi.AlucardApi.MapY;
			return ((mapX >= Constants.Khaos.RichterRoomMapMinX && mapX <= Constants.Khaos.RichterRoomMapMaxX) && mapY == Constants.Khaos.RichterRoomMapY);
		}

		private void StartCheats()
		{
		
			savePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			savePalette.Enable();
			if (toolConfig.Khaos.cubeOfZoeOn)
			{
				cubeOfZoe.Enable();
			}
			if (toolConfig.Khaos.faerieScrollOn)
			{
				faerieScroll.Enable();
			}
			if (toolConfig.Khaos.spiritOrbOn)
			{
				spirtOrb.Enable();
			}
			
			if (sotnApi.GameApi.InAlucardMode())
			{
				playerPaletteCheat.Disable();
			}
			else
			{
				UpdatePlayerColor();
				UpdateVisualEffect(true);
				resetColorWhenAlucard = true;
			}
			
			if (toolConfig.Khaos.BoostFamiliars)
			{
				setFamiliarXP();
			}
			if (toolConfig.Khaos.ContinuousWingsmash)
			{
				continuousWingsmash.Enable();
			}
		}
		private void setFamiliarXP()
		{
			int highXP = 10000;
			int medXP = 7000;
			int lowXP = 5904;

			// 2710 -> 10,000; Bat,Ghost
			// 1B58 -> 7,000; Sword
			// 1711 -> 5,904; Faerie, Spirte, Nose Devil,

			//Original
			//00002710  BatCardXp
			//00002710  GhostCardXp
			//00001710  FaerieCardXp
			//00001710  DemonCardXp
			//00001B58  SwordCardXp
			//00001710  SpriteCardXp
			//00001710  NoseDevilCardXp
			
			batCardXp.PokeValue(highXP);
			batCardXp.Enable();
			ghostCardXp.PokeValue(highXP);
			ghostCardXp.Enable();
			faerieCardXp.PokeValue(lowXP);
			faerieCardXp.Enable();
			demonCardXp.PokeValue(lowXP);
			demonCardXp.Enable();
			swordCardXp.PokeValue(medXP);
			swordCardXp.Enable();
			spriteCardXp.PokeValue(lowXP);
			spriteCardXp.Enable();
			noseDevilCardXp.PokeValue(lowXP);
			noseDevilCardXp.Enable();
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
					//resetColorWhenAlucard = true;
					GainMayhemMeter((short) toolConfig.Khaos.MeterOnReset);
					accelTime.PokeValue(5);
					turboMode.Disable();
					turboModeJump.Disable();
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
			cubeOfZoe = cheats.GetCheatByName("CubeOfZoe");
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
			rewind = cheats.GetCheatByName("Rewind");
			spirtOrb = cheats.GetCheatByName("SpiritOrb");
			alucardEffect = cheats.GetCheatByName("AlucardEffect");
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
			playerPaletteCheat = cheats.GetCheatByName("AlucardPalette");
			continuousWingsmash = cheats.GetCheatByName("ContinuousWingsmash");
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

			uint wolfDashTopLeft = DefaultSpeeds.WolfDashTopLeft;

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
			sotnApi.AlucardApi.WolfDashTopLeftSpeed = (sbyte) Math.Ceiling((sbyte) wolfDashTopLeft * factor);
			sotnApi.AlucardApi.BackdashDecel = slow == true ? DefaultSpeeds.BackdashDecelSlow : DefaultSpeeds.BackdashDecel;
			Console.WriteLine($"Set all speeds with factor {factor}");
		}
		private void SetShaftHp()
		{
			long shaftAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.ShaftOrbActor });
			if (shaftAddress > 0)
			{
				LiveEntity shaft = sotnApi.EntityApi.GetLiveEntity(shaftAddress);
				//Original
				//int bonusHP = (int)((4 + (vladRelicsObtained);
				int bonusHP = (int)((2 * toolConfig.Khaos.ShaftOrbHPModifier) + (.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier));

				if(toughBossesCount > 0)
				{
					toughBossesCount--;
					toughBossesRoomX = sotnApi.GameApi.RoomX;
					toughBossesRoomY = sotnApi.GameApi.RoomY;
					if (toughBossesCount == 0)
					{
						toughBossesSpawnTimer.Stop();
					}
					if (superToughBosses)
					{
						//Orignal:bonusHP *= 3
						bonusHP += (int)((.5 * bonusHP * toolConfig.Khaos.ShaftOrbHPModifier) + (.5 * bonusHP * toolConfig.Khaos.SuperBossHPModifier));
					}
					else 
					{
						//Orignal:bonusHP *= 2
						bonusHP += (int)((.5 * bonusHP * toolConfig.Khaos.ShaftOrbHPModifier));
					}

					shaft.Hp = (int) (Constants.Khaos.ShaftMayhemHp + bonusHP);
				}
				else
				{
					shaft.Hp = (int)(Constants.Khaos.ShaftMayhemHp + bonusHP);
				}

				shaftHpSet = true;
				Console.WriteLine($"Found Shaft Orb actor and set HP to: {shaft.Hp}; bonus HP {bonusHP}");

			}
			else
			{
				return;
			}
		}
		private void SetGalamothtStats()
		{
			long galamothTorsoAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.GalamothTorsoActor });
			if (galamothTorsoAddress > 0)
			{
				LiveEntity galamothTorso = sotnApi.EntityApi.GetLiveEntity(galamothTorsoAddress);

				//Original
				//int bonusHp = 500 + (250 * (vladRelicsObtained));
				int bonusHp = 0;

				if (toolConfig.Khaos.GalamothBossHPModifier > 0)
				{
					double galamothFlatHp = 250 * toolConfig.Khaos.GalamothBossHPModifier;
					bonusHp = (int)((galamothFlatHp + (125 * vladRelicsObtained * toolConfig.Khaos.CurseModifier)) * (toolConfig.Khaos.GalamothBossHPModifier * .5));

					galamothTorso.Hp = (int) (Constants.Khaos.GalamothMayhemHp + bonusHp);

					if (toughBossesCount > 0)
					{
						toughBossesCount--;
						toughBossesRoomX = sotnApi.GameApi.RoomX;
						toughBossesRoomY = sotnApi.GameApi.RoomY;

						if (toughBossesCount == 0)
						{
							toughBossesSpawnTimer.Stop();
						}
						if (superToughBosses)
						{
							galamothTorso.Hp = (int) (Constants.Khaos.GalamothMayhemHp + (1.5 * bonusHp * toolConfig.Khaos.GalamothBossHPModifier) + (1.5 * bonusHp * toolConfig.Khaos.SuperBossHPModifier));
							notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} Galamoth");
						}
						else
						{
							galamothTorso.Hp = (int) (Constants.Khaos.GalamothMayhemHp + (1.5 * toolConfig.Khaos.GalamothBossHPModifier * bonusHp));
							notificationService.AddMessage($"{KhaosActionNames.ToughBosses} Galamoth");
						}
					}
				}

				if (toolConfig.Khaos.GalamothIsRepositioned)
				{
					galamothTorso.Xpos -= Constants.Khaos.GalamothMayhemPositionOffset;
					Console.WriteLine($"Galamoth repositioned");
				}

				Console.WriteLine($"Galamoth Def Nerf: {toolConfig.Khaos.GalamothDefNerf}");

				long galamothHeadAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.GalamothHeadActor });
				LiveEntity galamothHead = sotnApi.EntityApi.GetLiveEntity(galamothHeadAddress);
				galamothHead.Xpos -= Constants.Khaos.GalamothMayhemPositionOffset;

				List<long> galamothParts = sotnApi.EntityApi.GetAllActors(new List<SearchableActor> { Constants.Khaos.GalamothPartsActors });
				foreach (var actor in galamothParts)
				{
					LiveEntity galamothAnchor = sotnApi.EntityApi.GetLiveEntity(actor);
					galamothAnchor.Xpos -= Constants.Khaos.GalamothMayhemPositionOffset;
					if (toolConfig.Khaos.GalamothDefNerf) 
					{ 
						galamothAnchor.Def = 0;
					}
					if(toolConfig.Khaos.GalamothBossDMGModifier > 0)
					{
						galamothAnchor.Damage = (uint) ((10 * toolConfig.Khaos.GalamothBossDMGModifier) + (2.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier));
					}
				}

				galamothStatsSet = true;
				Console.WriteLine($"Found Galamoth actor and set stats:HP Mod {toolConfig.Khaos.GalamothBossHPModifier}, DMG Mod:{toolConfig.Khaos.GalamothBossDMGModifier}");
			}
			else
			{
				return;
			}
		}
		private void CheckMPUsage()
		{
			uint currentMp = sotnApi.AlucardApi.CurrentMp;
			spentMp = 0;
			if (currentMp < storedMp)
			{
				spentMp = (int) storedMp - (int) currentMp;
			}
			storedMp = currentMp;

			if (!HPForMPActivePaused)
			{
				HPForMPUpdate();
			}
		}
		private void CheckDashInput()
		{
			if (inputService.RegisteredMove(InputKeys.Dash, Globals.UpdateCooldownFrames) && !speedOn && speedActive)
			{
				ToggleHasteDynamicSpeeds(superSpeed ? toolConfig.Khaos.SpeedFactor * Constants.Khaos.SpeedDashFactor : toolConfig.Khaos.SpeedFactor);
				speedOn = true;
				speedOverdriveTimer.Start();
			}
			else if (!inputService.ButtonHeld(InputKeys.Forward) && speedOn)
			{
				ToggleHasteDynamicSpeeds();
				speedOn = false;
				speedOverdriveTimer.Stop();
				if (overdriveOn)
				{
					speedOverdriveOffTimer.Start();
				}
			}
		}
		private void CheckWingsmashActive()
		{
			//uint test = SotnApi.Constants.Values.Alucard.States.Bat;
			//bool wingsmashActive = sotnApi.AlucardApi.Action == SotnApi.Constants.Values.Alucard.States.Bat;
			//gainedExperiecne = (int) currentExperiecne - (int) storedExperiecne;
			//storedExperiecne = currentExperiecne;
		}
		private bool MayhemMeterFull()
		{
			return notificationService.KhaosMeter >= 100;
		}
		private void GainMayhemMeter(short meter)
		{
			notificationService.KhaosMeter += meter;
		}
		private void SpendMayhemMeter()
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