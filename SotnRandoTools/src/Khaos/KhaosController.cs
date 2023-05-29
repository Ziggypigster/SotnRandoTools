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
		private System.Windows.Forms.Timer respawnRichterTimer = new();

		//Axe Armor
		private System.Windows.Forms.Timer spiritTimer = new();
		private System.Windows.Forms.Timer fireBallTimer = new();

		private System.Windows.Forms.Timer axeArmorContactDamageTimer = new();
		private System.Windows.Forms.Timer axeArmorEffectTimer = new();
		private System.Windows.Forms.Timer axeArmorHeart1Timer = new();
		private System.Windows.Forms.Timer axeArmorHeart2Timer = new();
		private System.Windows.Forms.Timer axeArmorHeart3Timer = new();
		private System.Windows.Forms.Timer axeArmorHeart4Timer = new();
		private System.Windows.Forms.Timer axeArmorHeart1FrameTimer = new();
		private System.Windows.Forms.Timer axeArmorHeart2FrameTimer = new();
		private System.Windows.Forms.Timer axeArmorHeart3FrameTimer = new();
		private System.Windows.Forms.Timer axeArmorHeart4FrameTimer = new();

		//Neutral
		private System.Windows.Forms.Timer heartsOnlyTimer = new();
		private System.Windows.Forms.Timer unarmedTimer = new();
		private System.Windows.Forms.Timer turboModeTimer = new();
		private System.Windows.Forms.Timer rushDownTimer = new();

		//Curses
		private System.Windows.Forms.Timer HPForMPTimer = new();
		private System.Windows.Forms.Timer HPForMPTickTimer = new();
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
		Cheat openEntranceDoor;
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
		Cheat swordCardLvl;
		Cheat spriteCardXp;
		Cheat noseDevilCardXp;

		//New to Mayhem

		Cheat spirtOrb;
		Cheat richterCutscene1;
		Cheat richterCutscene2;
		Cheat fightRichter1;
		Cheat fightRichter2;
		Cheat fightRichter3;
		//Cheat saveRichter;

		Cheat rewind;
		Cheat alucardEffect;
		Cheat turboMode;
		Cheat turboModeJump;
		Cheat accelTime;

		Cheat alucardHurtboxX;
		Cheat alucardHurtboxY;
		Cheat axeArmorVerticalJumpMoving;
		Cheat axeArmorVerticalJumpStationary;
		Cheat axeArmorHorizontalJump;
		Cheat axeArmorHorizontalSpeed;
		Cheat axeArmorFloat;
		Cheat axeArmorDamage;
		Cheat axeArmorDamageTypeA;
		Cheat axeArmorDamageTypeB;
		Cheat axeArmorDamageInterval;
		Cheat axeArmorWeapon;
		Cheat axeArmorDefense;
		Cheat toggleHurtBox;
		Cheat contactDamageType;
		Cheat jewelSwordRoom;
		Cheat jewelSwordRoomReverse;
		Cheat batLightRoom;
		Cheat startAxeArmor;
		Cheat permaAxeArmor;
		Cheat smoothCrouch;
		Cheat clipDirection;
		Cheat ceilingClip;
		Cheat leftClip;
		Cheat rightClip;

		Cheat characterData;
		Cheat state;
		Cheat action;

		Cheat subWeaponDamage1;
		Cheat subWeaponDamage2;
		Cheat subWeaponDamage3;
		Cheat subWeaponDamage4;
		Cheat subWeaponDamage5;
		Cheat subWeaponDamage6;
		Cheat throwMoreSubWeapons1;
		Cheat throwMoreSubWeapons2;
		Cheat throwMoreSubWeapons3;
		Cheat longerHolyWater1;
		Cheat longerHolyWater2;
		Cheat longerHolyWater3;
		Cheat tallerHolyWater1;
		Cheat tallerHolyWater2;
		Cheat tallerHolyWater3;
		Cheat enemyRichterPaletteCheat;
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

		//private bool autoAllowSmartLogic = false;
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
		private int superSummonerCount = 0;

		private bool buffStrRangeHolyGlasses = false;
		private bool buffConHPHolyGlasses = false;
		private bool buffIntMPHolyGlasses = false;
		private bool buffLckNeutralHolyGlasses = false;

		//Debug
		private bool allowFirstCastleRewind = true;
		private bool allowSecondCastleRewind = false;
		private bool isAlucardColorFirstCastle = false;
		private bool isAlucardColorSecondCastle = false;

		uint WarpsFirstCastle;
		uint WarpsSecondCastle;
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

		bool BatLightRoomSwitch = false;
		bool JewelSwordSwitch = false;

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
		private bool hexConfusedActive = false;
		//private bool hexWeaponsActive = false;
		//private bool hexWeaponsPaused = false;
		private bool hexRelicsActive = false;
		private bool hexRelicsPaused = false;
		private bool getJuggledActive = false;
		private bool slamJamActive = false;

		private bool rewindActive = false;
		private bool rewindDelay = false;
		private bool rewindSettingsFix = false;
		private bool accelTimeActive = false;
		private bool timeStopActive = false;
		//private bool buffActive = false;
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

		private int previousDefense = 0;
		private uint previousHP = 0;
		private uint previousExperience = 0;
		private bool hasSpikeBreaker = false;
		private bool hasBrilliantMail = false;
		private bool hasMojoMail = false;
		private bool hasHealingMail = false;

		private bool hasBatCard = false;
		private bool hasGhostCard = false;
		private bool hasDemonCard = false;
		private bool hasNoseDevilCard = false;
		private bool hasFaerieCard = false;
		private bool hasSpriteCard = false;
		private bool hasSwordCard = false;

		private bool hasSpiritOrb = false;
		private bool hasFaerieScroll = false;
		private bool hasJewelOfOpen = false;
		private bool hasMermanStatue = false;

		private bool hasCubeOfZoe = false;
		private bool hasHolySymbol = false;
		private bool hasSoulOfWolf = false;
		private bool hasPowerOfWolf = false;
		private bool hasSkillOfWolf = false;
		private bool hasFormOfMist = false;
		private bool hasPowerOfMist = false;
		private bool hasGasCloud = false;
		private bool hasSoulOfBat = false;
		private bool hasFireOfBat = false;
		private bool hasEchoOfBat = false;
		private bool hasForceOfEcho = false;
		private bool hasGravityBoots = false;
		private bool hasLeapStone = false;
		private bool hasHeartOfVlad = false;
		private bool hasToothOfVlad = false;
		private bool hasRingOfVlad = false;
		private bool hasEyeOfVlad = false;
		private bool hasRibOfVlad = false;

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

		private bool formOfMistTaken = false;
		private bool soulOfWolfTaken = false;
		private bool soulOfBatTaken = false;
		private bool powerOfMistTaken = false;
		private bool grantedTempFlight = false;

		private bool superAmbush = false;
		private bool superSummoner = false;
		private bool superSlamJam = false;
		private bool superToughBosses = false;
		private bool superRegen = false;
		private bool superSpeed = false;
		private bool superHPForMP = false;
		private bool superTimeStop = false;
		private bool superUnderwater = false;
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

		#region Axe Armor

		private int axeArmorColorCheatCount = 0;
		private int axeArmorEntityCheatCount = 0;
		private int axeArmorMenuCheatCount = 0;
		private int axeArmorHeartDamage1 = 0;
		private int axeArmorHeartDamage2 = 0;
		private int axeArmorHeartDamage3 = 0;
		private int axeArmorHeartDamage4 = 0;
		private int axeArmorHeartUsage1CheatCount = 0;
		private int axeArmorHeartUsage2CheatCount = 0;
		private int axeArmorHeartUsage3CheatCount = 0;
		private int axeArmorHeartUsage4CheatCount = 0;
		private int axeArmorHeartLock1CheatCount = 0;
		private int axeArmorHeartLock2CheatCount = 0;
		private int axeArmorHeartLock3CheatCount = 0;
		private int axeArmorHeartLock4CheatCount = 0;

		private uint axeArmorContactDamage = 0;
		private int axeArmorContactDamageMaxCooldown = 6;
		private int axeArmorContactDamageCooldown = 0;
		private int heartGlobalCooldown = 0;
		private int axeArmorHeart1Weapon = 0;
		private int axeArmorHeart2Weapon = 0;
		private int axeArmorHeart3Weapon = 0;
		private int axeArmorHeart4Weapon = 0;
		private int heartUsage1Cooldown = 0;
		private int heartUsage2Cooldown = 0;
		private int heartUsage3Cooldown = 0;
		private int heartUsage4Cooldown = 0;
		private int heartLock1Cooldown = 0;
		private int heartLock2Cooldown = 0;
		private int heartLock3Cooldown = 0;
		private int heartLock4Cooldown = 0;
		private int heartFacing1 = 0;
		private int heartFacing2 = 0;
		private int heartFacing3 = 0;
		private int heartFacing4 = 0;
		private int heartRising1 = 0;
		private int heartRising2 = 0;
		private int heartRising3 = 0;
		private int heartRising4 = 0;

		//private int maxPotionDuration = 4500;
		private uint axeArmorShieldINT = 0;

		private bool delayStart = true;
		private int axeArmorFrameCount = 0;
		private int axeArmorFrameDelay = 200; // 32
		private int axeArmorSafetyCooldown = 0;
		private int axeArmorMaxSafetyCooldown = 5;
		private int spellCooldown = 0;
		private int facePlantCooldown = 0;
		private int facePlantCooldownBase = 6;
		private int axeArmorDelayedHPRegenDuration = 0;
		private int axeArmorDelayedHeartRegenDuration = 0;
		private int axeArmorDelayedMPRegenDuration = 0;
		private int axeArmorSuperMPRegenCooldown = 0;
		private int axeArmorSuperMPRegenMaxCooldown = 80;
		private int axeArmorHeartsRegenCooldown = 0;
		private int axeArmorHPRegenCooldown = 0;
		private int axeArmorHPRegenMaxCooldown = 40;
		private int axeArmorMPRegenCooldown = 0;
		private int axeArmorMPRegenMaxCooldown = 15;
		private int glideMPCooldown = 0;
		private int glideMPMaxCooldown = 12 * Globals.UpdateCooldownFrames;
		private int glideMPMaxFoMCooldown = 18 * Globals.UpdateCooldownFrames;
		private int flightMPCooldown = 0;
		private int fireballCooldown = 0;
		private int storedResourceDelay = 0;
		private int storedResourceDelayBase = 3;
		private int jumpBoostStocks = 0;
		private int jumpBoostStocksMax = 1;
		private int jumpBoostHover = 0;
		private int jumpBoostHoverBase = 21;
		private int jumpBoostCooldown = 0;
		private int jumpBoostCooldownBase = (3 * Globals.UpdateCooldownFrames) - 4;
		private int jumpBoostCooldownGravityBase = 8 * Globals.UpdateCooldownFrames;
		private int flightDuration = 8 * Globals.UpdateCooldownFrames;
		private int flightMaxDuration = 8 * Globals.UpdateCooldownFrames;
		private int thrustDuration = 0;
		private int thrustMaxDuration = 6;
		private int slamMaxDuration = 20;
		private int slamDuration = 0;
		private bool wolfRunActive = false;
		private bool wolfDashActive = false;
		private bool wolfStrikeBoost = false;
		//private int wolfMPCooldown = 0;

		private bool axeArmorSwordFamiliar = false;
		private bool axeArmorGurthang = false;
		private bool axeArmorTwoHanded = false;
		private bool axeArmorThrust = false;
		private bool axeArmorAllowWeaponConsume = false;
		private bool axeArmorLimitedLeftHand = false;
		private bool axeArmorLimitedRightHand = false;
		private bool axeArmorEmptyLeftHand = false;
		private bool axeArmorEmptyRightHand = false;
		private bool axeArmorShieldLeftHand = false;
		private bool axeArmorShieldRightHand = false;
		private double axeArmorMultiSTR = 1.0;
		private double axeArmorMultiDamage = 1.0;
		private int axeArmorFlatDamage = 0;
		private int axeArmorRightHand = 255;
		private int axeArmorLeftHand = 255;
		private int axeArmorMeleeHits = 1;
		private int equipmentINT = 0;
		private int equipmentSTR = 0;
		private int equipmentLCK = 0;
		private int equipmentATK1 = 0;
		private int equipmentATK2 = 0;

		private int wolfCurrentSpeed = 0;
		private int mistFlightDuration = 0;
		private int mistFlightBaseDuration = 60 * Globals.UpdateCooldownFrames;
		private int mistFlightMaxDuration = 17 * Globals.UpdateCooldownFrames;
		private int mistBoostDuration = 0;
		private int mistBoostMaxDuration = 11 * Globals.UpdateCooldownFrames;
		private int mistBoostBaseDuration = 11 * Globals.UpdateCooldownFrames;
		private int mistBoostWolfBonus = 2 * Globals.UpdateCooldownFrames;
		private int mistBoostPoM = 30 * Globals.UpdateCooldownFrames;
		private int mistBoostFoM = 15 * Globals.UpdateCooldownFrames;
		private int mistTechDuration = 1 * Globals.UpdateCooldownFrames;
		private int mistTechBaseDuration = 2 * Globals.UpdateCooldownFrames;
		private int minAirTime = 2;
		private int minAirTimeBase = 2 * Globals.UpdateCooldownFrames;
		private bool mistBoostResetLocked = false;
		private bool mistCeilingLocked = false;
		private bool mistJumpCancelAllowed = false;

		private uint cloakIndex = 0;
		private uint storedLeftHand = 0;
		private uint storedRightHand = 0;
		private uint storedAxeArmorMaxHearts = 0;
		private uint storedAxeArmorHearts = 0;
		private uint storedAxeArmorMaxMP = 0;
		private uint storedAxeArmorMP = 0;
		private uint axeArmorCrouchHP = 0;
		private bool isHoldUp = false;
		private bool hasAxeArmorStoredResources = false;

		private int useItemCooldown = 0;
		private int useItemCooldownBase = 1 * Globals.UpdateCooldownFrames;
		private int useLeftHandCooldown = 0;
		private int useRightHandCooldown = 0;
		private int minHoldDownTime = 0;
		private int minHoldUpTime = 0;

		private int gurthangBoostTimer = 0;
		private int gurthangBoostMaxTime = 300;
		private int axeArmorInvinCooldown = 7;
		private int axeArmorInvinCooldownBase = 7 * Globals.UpdateCooldownFrames;
		private int gravityJumpCooldown = 5;
		private int gravityJumpCooldownBase = 8 * Globals.UpdateCooldownFrames;
		private int gravityJumpMPCheck = 8 * Globals.UpdateCooldownFrames;
		private int faceplantSpellCooldown = 0;
		private int faceplantAttackCooldown = 0;
		private int faceplantMaxAttackCooldown = 4 * Globals.UpdateCooldownFrames;
		private int faceplantMaxSpellCooldown = 10 * Globals.UpdateCooldownFrames;
		private uint axeArmorHoldUpYPosition;
		private uint axeArmorHoldDownYPosition;
		private bool jumpBoostActive = false;
		private bool jumpBoostAllowed = true;
		private bool gravityJumpAllowed = false;
		private bool isAxeArmorThrustAttack = false;
		private bool isAxeArmorSave = false;
		private bool isAxeArmorBat = false;
		private bool isAxeArmorMist = false;
		private bool isAxeArmorMistFlight = false;
		private bool isAxeArmorHClipAllowed = false;
		private bool isAxeArmorVClipAllowed = false;
		private bool isAxeArmorGravityJump = false;
		private bool isAerialFacePlant = false;

		private List<LiveEntity> fireballs = new();
		private List<LiveEntity> fireballsUp = new();
		private List<LiveEntity> fireballsDown = new();

		private bool fireBallActive = false;
		private ushort batFirePalette = 0;
		private ushort hellfirePalette = 0;
		private bool batFirePaletteSet = false;
		private bool hellfirePaletteSet = false;
		private bool axeArmorActive = false;
		private bool axeArmorStatsActive = false;
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
					socketClient = new WatsonWsClient(new Uri(Globals.StreamlabsSocketAddress));
					socketClient.ServerConnected += BotConnected;
					socketClient.ServerDisconnected += BotDisconnected;
					socketClient.MessageReceived += BotMessageReceived;
					socketClient.Start();
				}
				notificationService.AddMessage($"Mayhem started");
				Console.WriteLine("Mayhem started");
			}


		}
		public void StopMayhem()
		{
			StopTimers();
			alucardHurtboxX.Disable();
			alucardHurtboxY.Disable();
			spirtOrb.Disable();
			faerieScroll.Disable();
			cubeOfZoe.Disable();
			openEntranceDoor.Disable();
			richterCutscene2.Disable();
			axeArmorVerticalJumpMoving.Disable();
			axeArmorVerticalJumpStationary.Disable();
			axeArmorHorizontalSpeed.Disable();
			axeArmorHorizontalJump.Disable();
			axeArmorFloat.Disable();
			axeArmorDamage.Disable();
			axeArmorDamageTypeA.Disable();
			axeArmorDamageTypeB.Disable();
			axeArmorDamageInterval.Disable();
			axeArmorWeapon.Disable();
			axeArmorDefense.Disable();
			startAxeArmor.Disable();
			permaAxeArmor.Disable();
			clipDirection.Disable();
			ceilingClip.Disable();
			leftClip.Disable();
			rightClip.Disable();
			smoothCrouch.Disable();
			characterData.Disable();
			state.Disable();
			action.Disable();

			if (socketClient.Connected)
			{
				socketClient.Stop();
			}
			else
			{
				socketClient.Dispose();
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
				//sotnApi.GameApi.OverwriteString(boss.Value, subscribers[i]);
				sotnApi.GameApi.OverwriteString(boss.Value, subscribers[i], true);
				Console.WriteLine($"{boss.Key} renamed to {subscribers[i]}");
				i++;
			}
		}

		public void ModifyDifficulty()
		{
			adjustMinStats();
		}

		public void adjustMinStats()
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
			else
			{
				minHP = 80u;
				minMP = 30u;
				minHearts = 50u;
				minStr = 7u;
				minCon = 7u;
				minInt = 7u;
				minLck = 7u;
			}
			if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor)
			{
				if (hasSpiritOrb)
				{
					minHearts += Constants.Khaos.SpirtOrbHeartMPBoost;
					minMP += Constants.Khaos.SpirtOrbHeartMPBoost;
				}
				if (hasFaerieScroll)
				{
					minHP += Constants.Khaos.FaerieScrollHPBoost;
				}
				if (hasJewelOfOpen)
				{
					minMP += Constants.Khaos.JewelOfOpenMPBoost;
				}
				if (hasMermanStatue)
				{
					minHearts += Constants.Khaos.MermanStatueHeartBoost;
				}
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
			hellfirePaletteSet = false;
			batFirePaletteSet = false;
			hellfirePalette = 0;
			batFirePalette = 0;
		}

		private void CleanUpAxeArmorCheats(bool flushCheats = true)
		{
			for (int i = 0; i < axeArmorColorCheatCount; i++)
			{
				var colorCheat = cheats.GetCheatByName(Constants.Khaos.AxeArmorColorName);
				cheats.RemoveCheat(colorCheat);
			}
			for (int i = 0; i < axeArmorEntityCheatCount; i++)
			{
				var doorCheat = cheats.GetCheatByName(Constants.Khaos.RemoveEntityName);
				cheats.RemoveCheat(doorCheat);
			}

			for (int i = 0; i < axeArmorMenuCheatCount; i++)
			{
				var menuCheat = cheats.GetCheatByName(Constants.Khaos.OptionMenuName);
				cheats.RemoveCheat(menuCheat);
			}
			axeArmorColorCheatCount = 0;
			axeArmorEntityCheatCount = 0;
			axeArmorMenuCheatCount = 0;
		}
		private void InitializeTempVariables()
		{
			//Re-initialize stat / item changes.
			//allowResetStateWhenAlucard = false;

			//notificationService.KhaosMeter = 0;
			notificationService.EquipMessage = "";
			notificationService.WeaponMessage = "";

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

			//Re-Initialize Axe Armor
			cloakIndex = 0;
			hasSpikeBreaker = false;
			axeArmorRightHand = 255;
			axeArmorLeftHand = 255;

			hasSpiritOrb = false;
			hasFaerieScroll = false;
			hasJewelOfOpen = false;
			hasMermanStatue = false;

			hasCubeOfZoe = false;
			hasHolySymbol = false;
			hasSoulOfWolf = false;
			hasPowerOfWolf = false;
			hasSkillOfWolf = false;
			hasFormOfMist = false;
			hasPowerOfMist = false;
			hasGasCloud = false;
			hasSoulOfBat = false;
			hasFireOfBat = false;
			hasEchoOfBat = false;
			hasForceOfEcho = false;
			hasGravityBoots = false;
			hasLeapStone = false;
			hasHeartOfVlad = false;
			hasToothOfVlad = false;
			hasRingOfVlad = false;
			hasEyeOfVlad = false;
			hasRibOfVlad = false;

			hasBatCard = false;
			hasDemonCard = false;
			hasNoseDevilCard = false;
			hasFaerieCard = false;
			hasSpriteCard = false;
			hasGhostCard = false;
			hasSwordCard = false;

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
					if (toolConfig.Khaos.autoAllowPerfectMayhem && autoPerfectMayhemCounter >= toolConfig.Khaos.autoPerfectMayhemTrigger)
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
		private void RollMoodUpdate()
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
					UpdateAutoMood(actionType, false, false);
				}

				Console.WriteLine($"Auto Mayhem - Mood Results: roll={roll}, autoMoodRollMax={autoMoodRollMax}, actionType={actionType}");
			}
		}
		private void UpdateAutoMood(int actionType = 1, bool IsDecrease = false, bool IsMandatory = false)
		{
			if (autoMoodLevelMax == 0)
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
					EnqueueAction(new EventAddAction { Command = "underwater", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "underwater", UserName = Constants.Khaos.AutoMayhemName });
					EnqueueAction(new EventAddAction { Command = "underwater", UserName = Constants.Khaos.AutoMayhemName });
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
			Console.WriteLine($"Debug - Room{sotnApi.GameApi.Room}, RoomX{sotnApi.GameApi.RoomX}, RoomY{sotnApi.GameApi.RoomY};  " +
				//$"RoomXCord{RoomXCord}, RoomYCord{RoomYCord}; " +
				$"alucardMapX{sotnApi.AlucardApi.MapX}, alucardMapY{sotnApi.AlucardApi.MapY}; " +
				$"alucardX{sotnApi.AlucardApi.ScreenX}, alucardY{sotnApi.AlucardApi.ScreenY}, " +
				$"Is2ndCastle:{alucardSecondCastle}");
			if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor)
			{
				int meleeDamage = 0;
				CheckAxeArmorStats(true);
				CheckAxeArmorAttack(true, 0, out meleeDamage);
			}
			//LogEntityFromRoom(false);
			LogEntityFromRoom(true);
			//RemoveAllRelics();
			//Console.WriteLine($"Debug - Is2ndCastle:{alucardSecondCastle}; Room Co-ordinates: X{sotnApi.GameApi.RoomX}, Y{sotnApi.GameApi.RoomY}; alucardMapX{sotnApi.AlucardApi.MapX}, alucardMapY{sotnApi.AlucardApi.MapY} ");
			//Console.WriteLine($"Debug - Equipment: Left ={sotnApi.AlucardApi.LeftHand}, Right = {sotnApi.AlucardApi.RightHand}, Armor = {sotnApi.AlucardApi.Armor}, Armor Name = {Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)]}");
		}

		private void RemoveAllRelics()
		{
			if (sotnApi.AlucardApi.HasRelic(Relic.CubeOfZoe))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.CubeOfZoe);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SoulOfBat);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.FireOfBat);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.EchoOfBat);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.ForceOfEcho))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.ForceOfEcho);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.FormOfMist);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.PowerOfMist);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.GravityBoots))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GravityBoots);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.LeapStone))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.LeapStone);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SoulOfWolf);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.PowerOfMist);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.PowerOfWolf);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SkillOfWolf);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.HolySymbol))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.HolySymbol);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.FaerieCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.FaerieCard);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SpriteCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SpriteCard);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.BatCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.BatCard);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.DemonCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.DemonCard);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.NoseDevilCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.NoseDevilCard);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.GhostCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GhostCard);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SwordCard))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SwordCard);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.RingOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.RingOfVlad);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.HeartOfVlad);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.RibOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.RibOfVlad);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.ToothOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.ToothOfVlad);
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.EyeOfVlad);
			}
		}


		public void LogEntityFromRoom(bool enemy = true)
		{
			long start = enemy ? Game.EnemyEntitiesStart : Game.FriendlyEntitiesStart;
			int count = enemy ? 155 : 48;//Entities.FriendEntitiesCount

			LiveEntity currentActor;

			for (int i = 0; i < count; i++)
			{

				currentActor = sotnApi.EntityApi.GetLiveEntity(start);

				if (currentActor.Hp == 32767)
				{
					//currentActor.Xpos = 2;
					//currentActor.Ypos = 2;
				}

				Console.WriteLine($"Debug - IsEnemy={enemy}, Address={currentActor.Address},HitboxWidth={currentActor.HitboxWidth},HitboxHeight={currentActor.HitboxHeight},Xpos={currentActor.Xpos},Ypos={currentActor.Ypos}");
				Console.WriteLine($"Debug - DamageTypes{currentActor.DamageTypeA}&&{currentActor.DamageTypeB},Damage={currentActor.Damage},Hp={currentActor.Hp},Def={currentActor.Def},AiId ={currentActor.AiId}");
				Console.WriteLine($"Debug - Pallete={currentActor.Palette},ColorMode={currentActor.ColorMode},LockOn={currentActor.LockOn},IsAutoToggle={currentActor.AutoToggle}");
				start += Entities.Offset;
			}
		}

		public void AxeArmorRoomCheck()
		{
			CheckCastleChanged();

			alucardMapX = sotnApi.AlucardApi.MapX;
			alucardMapY = sotnApi.AlucardApi.MapY;

			AxeArmorEntityCheck();
			AxeArmorClipCheck();

			if (!sotnApi.AlucardApi.CavernsSwitchAndBridge && IsInRoomList(Constants.Khaos.CavernSwitchRoom) && sotnApi.AlucardApi.ScreenX <= 85)
			{
				notificationService.AddMessage($"Switch Toggled!");
				sotnApi.AlucardApi.CavernsSwitchAndBridge = true;
			}
			else if (!BatLightRoomSwitch && IsInRoomList(Constants.Khaos.BatLightRooms) && sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat))
			{
				notificationService.AddMessage($"Switch Toggled!");
				batLightRoom.Enable();
				BatLightRoomSwitch = true;
			}
			else
			{
				if (!IsInRoomList(Constants.Khaos.BatLightRooms))
				{
					BatLightRoomSwitch = false;
					batLightRoom.Disable();
				}
				if (!JewelSwordSwitch && hasSoulOfWolf && hasSoulOfBat && IsInRoomList(Constants.Khaos.JewelSwordRooms))
				{ //Adventure Mode Check
					notificationService.AddMessage($"Hidden Room Toggled!");
					JewelSwordSwitch = true;
					if (alucardSecondCastle)
					{
						jewelSwordRoomReverse.Enable();
					}
					else
					{
						jewelSwordRoom.Enable();
					}
				}
				else if (!IsInRoomList(Constants.Khaos.JewelSwordRooms))
				{
					JewelSwordSwitch = false;
					jewelSwordRoom.Disable();
					jewelSwordRoomReverse.Disable();
				}
			}

		}

		public void AxeArmorRemoveEntities()
		{
			for (int i = 0; i < axeArmorEntityCheatCount; i++)
			{
				var doorCheat = cheats.GetCheatByName(Constants.Khaos.RemoveEntityName);
				cheats.RemoveCheat(doorCheat);
			}
			axeArmorEntityCheatCount = 0;
		}

		public void AxeArmorEntityCheck()
		{
			long start = Game.EnemyEntitiesStart;
			int count = 155; //Normal - 131?

			LiveEntity currentActor;
			if (sotnApi.AlucardApi.ScreenX < 30 || sotnApi.AlucardApi.ScreenX > 225 || sotnApi.AlucardApi.ScreenY < 25 || sotnApi.AlucardApi.ScreenY > 240
				|| !sotnApi.AlucardApi.HasControl()
				|| sotnApi.GameApi.IsInMenu()
				|| sotnApi.GameApi.CanSave()
				|| IsInRoomList(Constants.Khaos.LoadingRooms)
				|| IsInRoomList(Constants.Khaos.ClockRoom)
				)

			{
				AxeArmorRemoveEntities();
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					currentActor = sotnApi.EntityApi.GetLiveEntity(start);

					if (currentActor.AiId == 20196) // OuterWall Door Fix
					{
						cheats.AddCheat(currentActor.Address + 86, 0, Constants.Khaos.RemoveEntityName, WatchSize.Byte);
						++axeArmorEntityCheatCount;

						if (currentActor.Ypos == 129)
						{

							if (currentActor.Xpos > 98 && sotnApi.AlucardApi.State == 41 && sotnApi.AlucardApi.FacingLeft)
							{
								currentActor.LockOn = 2;
								wolfCurrentSpeed = 0;
							}
							else if (currentActor.LockOn == 2)
							{
								currentActor.LockOn = 3;
							}
						}
					}
					else if (currentActor.AiId == 35760) // Reverse OuterWall Door Fix
					{
						cheats.AddCheat(currentActor.Address + 86, 0, Constants.Khaos.RemoveEntityName, WatchSize.Byte);
						++axeArmorEntityCheatCount;

						if (currentActor.Ypos == 129)// && !sotnApi.AlucardApi.FacingLeft && currentActor.Ypos == 129)
						{
							if (currentActor.Xpos < 155 && sotnApi.AlucardApi.State == 41 && !sotnApi.AlucardApi.FacingLeft)
							{
								currentActor.LockOn = 2;
								wolfCurrentSpeed = 0;
							}
							else if (currentActor.LockOn == 2)
							{
								currentActor.LockOn = 3;
							}
						}
					}
					else if (currentActor.Hp == 32767 && currentActor.Def == 3 && currentActor.LockOn == 1 && currentActor.Palette == 0 && currentActor.Damage == 0 && currentActor.ColorMode == 0 // Identify Door-Group
						&& (currentActor.AiId != 29788 && currentActor.AiId != 32104 && currentActor.AiId != 11480 && currentActor.AiId != 63640) // Sealed Doors
						&& (currentActor.AiId != 3820 && currentActor.AiId != 4544 && currentActor.AiId != 51448 && currentActor.AiId != 2720 && currentActor.AiId != 21792 && currentActor.AiId != 25128) // Statutes
						&& (currentActor.AiId != 11264 && currentActor.AiId != 54056 && currentActor.AiId != 63640) // Crashes Game?
						&& (currentActor.AiId != 22800 && currentActor.AiId != 11480 && currentActor.AiId != 6228 && currentActor.AiId != 10804 && currentActor.AiId != 28248) //Non-Door Objects
						&& (currentActor.AiId != 46312 && currentActor.AiId != 45552) //Reverse Castle Candleholders
						&& (currentActor.AiId != 57040 && currentActor.AiId != 21792) //Library Door, Reverse Coliseum
						&& (currentActor.AiId != 19652 && currentActor.AiId != 24104) // Outer Wall Hidden Floor, Reverse Outer Wall
						&& (currentActor.AiId != 11544 && currentActor.AiId != 16712 && currentActor.AiId != 16912) // Reverse Olrox Platform, Olrox Areas 
						&& (currentActor.AiId != 18816 && currentActor.AiId != 32796 && currentActor.AiId != 7144) //Cavern Platform, Merman Statue Gate
						)
					{
						cheats.AddCheat(currentActor.Address + 86, 0, Constants.Khaos.RemoveEntityName, WatchSize.Byte);
						++axeArmorEntityCheatCount;
						//Console.WriteLine($"Debug - Found Regular Door: {currentActor.AiId} @ {currentActor.Address}");
					}
					if (sotnApi.AlucardApi.HasRelic(Relic.JewelOfOpen))
					{
						if (currentActor.AiId == 29788 || currentActor.AiId == 32104)
						{
							//Verified 29788 - Silver
							//Verified 32104 - Chapel
							cheats.AddCheat(currentActor.Address + 40, 0, Constants.Khaos.RemoveEntityName, WatchSize.Byte);
							cheats.AddCheat(currentActor.Address + 86, 0, Constants.Khaos.RemoveEntityName, WatchSize.Byte);
							++axeArmorEntityCheatCount;
							++axeArmorEntityCheatCount;
							//Console.WriteLine($"Debug - Found Sealed Door: {currentActor.AiId} @ {currentActor.Address}");
						}
					}
					if (hasSpikeBreaker) //if (sotnApi.AlucardApi.HasItemInInventory("Spike Breaker"))
					{
						if (currentActor.AiId == 16088 || currentActor.AiId == 17852 || currentActor.AiId == 42012 || currentActor.AiId == 39076) //Silver Ring, Reverse Silver Ring, Clock Tower, Reverse Clock Tower Spike IDs
						{
							// currentActor.AiId == 34480 || currentActor.AiId == 17696 Bat Room, Reverse Olrox
							currentActor.HitboxHeight = 0;
							currentActor.HitboxWidth = 0;
							currentActor.Damage = 0;
						}
						else if (currentActor.AiId == 34092 || (currentActor.AiId == 47912 && !alucardSecondCastle)) // Bat room Room Spike trigger?
						{//490072 = base, change = 490116, offset = 44
							cheats.AddCheat(currentActor.Address + 44, 0, Constants.Khaos.RemoveEntityName, WatchSize.Byte);
							++axeArmorEntityCheatCount;
						}
						else if (currentActor.AiId == 17192 || currentActor.AiId == 22352) // Reverse / Olrox Skeleton Dancer
						{//offset = 40 to 43
							cheats.AddCheat(currentActor.Address + 40, 0, Constants.Khaos.RemoveEntityName, WatchSize.DWord);
							++axeArmorEntityCheatCount;
						}
					}
					start += Entities.Offset;
				}

				start = Game.EnemyEntitiesStart + (Entities.Offset * 131);
				count = 155;

				for (int i = 131; i < count; i++)
				{
					currentActor = sotnApi.EntityApi.GetLiveEntity(start);

					if (sotnApi.AlucardApi.HasRelic(Relic.JewelOfOpen))
					{
						if (currentActor.AiId == 54056 || currentActor.AiId == 63640)
						{
							//Verified 54056 - Marble Gallery
							//Verified 63640 - Reverse Chapel?
							//11480 -  Marble Gallery Switch

							//cheats.AddCheat(currentActor.Address + 44, 6, "Remove Entity", WatchSize.Byte);
							currentActor.LockOn = 6;

							cheats.AddCheat(currentActor.Address + 86, 0, Constants.Khaos.RemoveEntityName, WatchSize.Byte);
							++axeArmorEntityCheatCount;
							//Console.WriteLine($"Debug - Address={currentActor.Address},AiId ={currentActor.AiId}");
						}
					}
					start += Entities.Offset;
				}
			}
		}

		public void AxeArmor(string user = "Mayhem")
		{
			sotnApi.AlucardApi.Armor = (uint) (Equipment.Items.IndexOf("Axe Lord armor") - Equipment.HandCount - 1);
			//sotnApi.AlucardApi.State = 40;
		}

		public void Library(string user = "Mayhem", bool skipNotification = false, int handEffect = 0)
		{
			//Library = 41 Hex = 65 Dec

			if (!sotnApi.AlucardApi.HasControl())
			{
				queuedActions.Add(new QueuedAction { Name = "Library", ChangesAlucardEffects = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => Library(user)) });
				return;
			}

			if (!skipNotification)
			{
				notificationService.AddMessage($"{user} used {KhaosActionNames.Library}");
			}

			libraryTimer.Start();
			alucardEffectsLocked = true;


			//sotnApi.AlucardApi.HorizontalVelocityWhole = 0;
			//sotnApi.AlucardApi.HorizontalVelocityFractional = 0;

			if (!toolConfig.Khaos.BoostAxeArmor)
			{
				alucardEffect.PokeValue(65);
				alucardEffect.Enable();
			}
			else
			{
				int address = 0;
				if (handEffect == 1)
				{
					address = Constants.Khaos.AxeArmorHand1EffectAddress;
				}
				else
				{
					address = Constants.Khaos.AxeArmorHand2EffectAddress;
				}
				string cheatName = Constants.Khaos.AxeArmorLibrary;
				int activator = 65;
				cheats.AddCheat(address + 38, activator, cheatName, WatchSize.Byte); //Effect Activation #
			}
		}

		public void Rewind(string user = "Mayhem", bool safetyCheck = true)
		{
			if (safetyCheck)
			{
				if (rewindSettingsFix || sotnApi.GameApi.IsInMenu() || IsInRoomList(Constants.Khaos.RewindBanRoom) || (!alucardSecondCastle && !allowFirstCastleRewind) || (alucardSecondCastle && !allowSecondCastleRewind))
				{
					queuedFastActions.Enqueue(new MethodInvoker(() => Rewind(user, safetyCheck)));
					//queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Rewind, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => Rewind())});
					return;
				}
			}

			rewindActive = true;
			rewindDelay = true;
			rewindSettingsFix = true;

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

			WarpsFirstCastle = sotnApi.AlucardApi.WarpsFirstCastle;
			WarpsSecondCastle = sotnApi.AlucardApi.WarpsSecondCastle;

			//Keep menu options in place until rewind resolves.


			long start = 248486; //03CAA8 - 2

			//Cloak Color Settings
			LiveEntity currentActor;
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 248490; //03CAAC -2 
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 248494; //03CAB0 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 248498; //O3CAB4 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 248502; //03CAB8 - 2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 248506; //03CABC -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			//Cloak Alucard Model
			start = 669658; //0A37DC -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 669659; //0A37DD -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 669660; //0A37DE -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 669661; //0A37DF -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 669662; //0A37E0 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 669663; //0A37E1 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 669664; //0A37E2 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 669665; //0A37E3 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			//Window Color Settings
			start = 248510; //03CAC0 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 248514; //03CAC4 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 248518; //03CAC8 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			//Reversible Cloak
			start = 248566; //03CAF8 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 453590; //06EBD8 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 453591; //06EBD9 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 453592; //06EBDA -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 453593; //06EBDB -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 453594; //06EBDC -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 453595; //06EBDD -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 453596; //06EBDE -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 453597; //06EBDF -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			axeArmorMenuCheatCount = 26;

			//Sound Settings
			/*
			start = 248570; //03CAFC -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 441078; //06BAF8 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);

			start = 1282214; //1390A8 -2
			currentActor = sotnApi.EntityApi.GetLiveEntity(start);
			cheats.AddCheat(start + 2, (int) (currentActor.Xpos), Constants.Khaos.OptionMenuName, WatchSize.Byte);
			*/

			rewind.Enable();
			rewindTimer.Start();
		}
		public void MinStats(string user = "Mayhem")
		{
			adjustMinStats();

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

		public void ChangeRichterColor(string user = "Mayhem")
		{
			richterColor = toolConfig.Khaos.RichterColor;
			UpdateVisualEffect(true, 123456);
		}

		public void SpawnEntity(string user = "Mayhem")
		{
			int aiID = toolConfig.Khaos.SpawnEntityID;
			notificationService.AddMessage($"Attempting to spawn entity: {aiID}");

			List<SearchableActor> CustomSpawnEntity = new List<SearchableActor>
			{
				new SearchableActor { AiId = aiID },
			};

			long enemy = sotnApi.EntityApi.FindEntityFrom(CustomSpawnEntity);

			if (enemy != 0)
			{
				ambushEnemies.Clear();
				Entity? ambushEnemy = new Entity(sotnApi.EntityApi.GetEntity(enemy));

				if (ambushEnemy is not null && !ambushEnemies.Where(e => e.AiId == ambushEnemy.AiId).Any())
				{
					ambushEnemies.Add(ambushEnemy);
				}

				if (ambushEnemies.Count > 0)
				{
					int enemyIndex = rng.Next(0, ambushEnemies.Count);
					//string name = KhaosActionNames.Ambush;

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

					Console.WriteLine($"Spawn Entity: Map - X{sotnApi.AlucardApi.MapX},Y{sotnApi.AlucardApi.MapY}, AlucardMap - X{alucardMapX},Y{alucardMapY}; Screen - X{sotnApi.AlucardApi.ScreenX},Y{sotnApi.AlucardApi.ScreenY}; Enemy Position: X{ambushEnemies[enemyIndex].Xpos},Y{ambushEnemies[enemyIndex].Ypos}");

					ambushEnemies[enemyIndex].Palette += (ushort) rng.Next(1, 10);
					if (!IsInRoomList(Constants.Khaos.ClockRoom))
					{
						sotnApi.EntityApi.SpawnEntity(ambushEnemies[enemyIndex]);
					}
				}
			}
		}

		public void UpdateAxeArmorHeartCooldowns()
		{
			string cheatName = Constants.Khaos.AxeArmorHeartLockName;

			if(axeArmorFrameCount == Globals.UpdateCooldownFrames)
			{
				//Global Heartlockout
				if (heartGlobalCooldown > 0)
				{
					--heartGlobalCooldown;
				}
				//Heart-Usage Cooldowns
				if (heartUsage1Cooldown > 0)
				{
					--heartUsage1Cooldown;
				}
				if (heartUsage2Cooldown > 0)
				{
					--heartUsage2Cooldown;
				}
				if (heartUsage3Cooldown > 0)
				{
					--heartUsage3Cooldown;
				}
				if (heartUsage4Cooldown > 0)
				{
					--heartUsage4Cooldown;
				}

				//Clean-up locked graphics (size < 12)
				if (heartLock1Cooldown > 0)
				{
					--heartLock1Cooldown;
					if (heartLock1Cooldown == 0)
					{
						for (int i = 0; i < axeArmorHeartLock1CheatCount; i++)
						{
							var heartCheat = cheats.GetCheatByName(cheatName + "1");
							cheats.RemoveCheat(heartCheat);
						}
						axeArmorHeartLock1CheatCount = 0;
					}
				}
				if (heartLock2Cooldown > 0)
				{
					--heartLock2Cooldown;
					if (heartLock2Cooldown == 0)
					{
						for (int i = 0; i < axeArmorHeartLock2CheatCount; i++)
						{
							var heartCheat = cheats.GetCheatByName(cheatName + "2");
							cheats.RemoveCheat(heartCheat);
						}
						axeArmorHeartLock2CheatCount = 0;
					}
				}
				if (heartLock3Cooldown > 0)
				{
					--heartLock3Cooldown;
					if (heartLock3Cooldown == 0)
					{
						for (int i = 0; i < axeArmorHeartLock3CheatCount; i++)
						{
							var heartCheat = cheats.GetCheatByName(cheatName + "3");
							cheats.RemoveCheat(heartCheat);
						}
						axeArmorHeartLock3CheatCount = 0;
					}

				}
				if (heartLock4Cooldown > 0)
				{
					--heartLock4Cooldown;
					if (heartLock4Cooldown == 0)
					{
						for (int i = 0; i < axeArmorHeartLock4CheatCount; i++)
						{
							var heartCheat = cheats.GetCheatByName(cheatName + "4");
							cheats.RemoveCheat(heartCheat);
						}
						axeArmorHeartLock4CheatCount = 0;
					}
				}
			}
		}

		public void AxeArmorHeartActionCheck(bool clockAxe = false, int subWeaponCount = 0)
		{
			bool validHeartUsage = false;
			bool lockVisual = false;
			bool lockXPosition = false;
			bool lockYPosition = false;
			bool updateXPosition = false;
			bool updateYPosition = false;
			bool lockArc = false;
			bool lockXSpeed = false;
			bool lockYSpeed = false;
			bool lockRisingFalling = false;
			bool reduceCost = false;

			int cheatLockCount = 0;
			int cheatUsageCount = 0;
			int freeSlotCount = 0;
			int alucardFacing = sotnApi.AlucardApi.FacingLeft == true ? 255 : 0;
			int xPosition = (int) sotnApi.AlucardApi.ScreenX;
			int yPosition = (int) sotnApi.AlucardApi.ScreenY;
			int baseDamage = 0;
			int damage = 0;
			int damageTypeA = 0;
			int damageTypeB = 0;
			int damageInterval = 0;
			int setXSpeed = 0;
			int setYSpeed = 0;
			int setArcSpeed = 255;
			int setRisingFalling = 0;
			int speedModifier = sotnApi.AlucardApi.FacingLeft == true ? -1 : 1;
			int activator = 0;
			int globalUsageCooldown = 0;
			int heartLockCooldown = 0;
			int heartUsageCooldown = 0;
			int reservedSlot = 0;
			int address = Constants.Khaos.AxeArmorEffectStartAddress;
			string lockName = Constants.Khaos.AxeArmorHeartLockName;
			string cheatName = Constants.Khaos.AxeArmorHeartName;

			int highestINT = 0;
			int lowestINT = 0;

			if (sotnApi.AlucardApi.Int > equipmentINT)
			{
				highestINT = (int) sotnApi.AlucardApi.Int;
				lowestINT = equipmentINT;
			}
			else
			{
				highestINT = equipmentINT;
				lowestINT = (int) sotnApi.AlucardApi.Int;
			}

			lowestINT += (int) axeArmorShieldINT;

			if (heartsOnlyActive)
			{
				int minDamage = lowestINT < 2 ? 2 : lowestINT;  
				if (heartsOnlyLevel >= 2)
				{
					highestINT += (int) (minDamage * .5);
				}
				else if (heartsOnlyLevel >= 3)
				{
					highestINT += (int) minDamage;
				}
			}

			if (Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Heart broach")
			{
				reduceCost = true;
			}
			else if (Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Heart broach")
			{
				reduceCost = true;
			}

			if (heartUsage1Cooldown <= 0)
			{
				++freeSlotCount;
				if (reservedSlot == 0)
				{
					reservedSlot = 1;
				}
			}
			if (heartUsage2Cooldown <= 0)
			{
				++freeSlotCount;
				if (reservedSlot == 0)
				{
					reservedSlot = 2;
				}
			}
			if (heartUsage3Cooldown <= 0)
			{
				++freeSlotCount;
				if (reservedSlot == 0)
				{
					reservedSlot = 3;
				}
			}
			if (heartUsage4Cooldown <= 0)
			{
				++freeSlotCount;
				if (reservedSlot == 0)
				{
					reservedSlot = 4;
				}
			}

			if (reservedSlot != 0)
			{
				cheatName += reservedSlot;
				lockName += reservedSlot;
				address += 188 * reservedSlot;

				if (sotnApi.AlucardApi.Subweapon == Subweapon.Stopwatch && !clockAxe)
				{
					if (freeSlotCount >= 4 && subWeaponCount == 0)
					{
						if ((sotnApi.AlucardApi.CurrentHearts > 19 || (reduceCost && sotnApi.AlucardApi.CurrentHearts > 9)))
						{
							activator = 41;

							if (reduceCost)
							{
								sotnApi.AlucardApi.CurrentHearts -= 10;
							}
							else
							{
								sotnApi.AlucardApi.CurrentHearts -= 20;
							}
							++subWeaponCount;
							validHeartUsage = true;
							heartUsageCooldown = 150;
							globalUsageCooldown = 150;
							//startAxeArmorHeartTickTimer(subWeaponCount, (int) Subweapon.Empty, 0, alucardFacing);
						}
					}
				}
				else
				{
					if (hasBrilliantMail)
					{
						damage += 10;
					}
					if (sotnApi.AlucardApi.HasRelic(Relic.RingOfVlad))
					{
						damage += 10;
					}
					if (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat))
					{
						damage += 10;
					}
					if (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat))
					{
						damage += 5;
					}
					if (sotnApi.AlucardApi.HasRelic(Relic.ForceOfEcho))
					{
						damage += 5;
						//damageTypeA = 8; //Dark Type
					}
					if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
					{
						damage += 5;
					}

					//Reduce damage from other sources by 20%, minimum 1
					if (damage > 4)
					{
						damage -= (int) Math.Round(damage / 5.0);
					}

					if (clockAxe)
					{
						activator = 10;
						validHeartUsage = true;
						damageTypeB = 4; //Water
						damageTypeB = 32; //Ice
						damage += (int) Math.Round(9 + (highestINT * .7) + (lowestINT * .3));
						damage = (int)Math.Round(damage / 2.0);

						damageInterval = 16;
						heartUsageCooldown = 150;
						globalUsageCooldown = 150;
						lockXPosition = true;
						lockYPosition = true;

						switch (subWeaponCount)
						{
							case 1:
								yPosition -= 72;
								break;
							case 2:
								yPosition -= 51;
								break;
							case 3:
								yPosition -= 30;
								break;
						}

						++subWeaponCount;
						startAxeArmorHeartTickTimer(subWeaponCount, (int) Subweapon.Empty, 0, alucardFacing);
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Empty)
					{
						if (sotnApi.AlucardApi.CurrentHearts > 1 || reduceCost && sotnApi.AlucardApi.CurrentHearts > 0)
						{
							if (reduceCost)
							{
								sotnApi.AlucardApi.CurrentHearts -= 1;
							}
							else
							{
								sotnApi.AlucardApi.CurrentHearts -= 2;
							}

							activator = 10;
							validHeartUsage = true;
							damage += (int) Math.Round(9 + (highestINT * .7) + (lowestINT * .3));
							damage = (int) Math.Round(damage / 4.0);
							damageTypeA = 128; //Fire
							damageTypeB = 8; //Dark

							lockXSpeed = true;
							lockYSpeed = true;
							lockArc = true;
							lockRisingFalling = true;

							if (sotnApi.AlucardApi.FacingLeft)
							{
								xPosition -= 25;
							}
							else
							{
								xPosition += 25;
							}

							setXSpeed = 0;
							setYSpeed = 187;
							setArcSpeed = 255;
							setRisingFalling = 255;

							damageInterval = 16;
							heartUsageCooldown = 50;
							globalUsageCooldown = 8;

							startAxeArmorHeartTickTimer(reservedSlot, (int) Subweapon.Empty, 0, alucardFacing);
						}
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Dagger)
					{
						if (sotnApi.AlucardApi.CurrentHearts > 3 || reduceCost && sotnApi.AlucardApi.CurrentHearts > 1)
						{
							if (reduceCost)
							{
								sotnApi.AlucardApi.CurrentHearts -= 2;
							}
							else
							{
								sotnApi.AlucardApi.CurrentHearts -= 4;
							}

							activator = 10;
							validHeartUsage = true;
							damage += (int) Math.Round(9 + (highestINT * .7) + (lowestINT * .3));
							damage = (int) Math.Round(damage * 1.2);
							damageTypeA = 64; // Cut
							damageTypeB = 0;

							lockXSpeed = true;
							lockYSpeed = true;
							lockArc = true;
							lockRisingFalling = true;
							setXSpeed = 16;
							setYSpeed = 48;
							setArcSpeed = 2; // OR 1 for less of angle
							setRisingFalling = 0;

							damageInterval = 20;
							heartUsageCooldown = 12;
							globalUsageCooldown = 5;

							startAxeArmorHeartTickTimer(reservedSlot, (int) Subweapon.Dagger, 0, alucardFacing);
						}
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Axe)
					{
						if (sotnApi.AlucardApi.CurrentHearts > 7 || reduceCost && sotnApi.AlucardApi.CurrentHearts > 3)
						{
							if (reduceCost)
							{
								sotnApi.AlucardApi.CurrentHearts -= 4;
							}
							else
							{
								sotnApi.AlucardApi.CurrentHearts -= 8;
							}

							activator = 10;
							validHeartUsage = true;
							damage += (int) Math.Round((9 + (highestINT * .7) + (lowestINT * .3)));

							damageTypeA = 0; //Hit
							damageTypeB = 0; //Hit

							damageInterval = 10;
							heartUsageCooldown = 20;
							globalUsageCooldown = 7;

							startAxeArmorHeartTickTimer(reservedSlot, (int) Subweapon.Axe, 0, alucardFacing);
						}
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.HolyWater)
					{
						bool validHolyWater = false;
						if (freeSlotCount >= 4 && subWeaponCount == 0)
						{
							if (sotnApi.AlucardApi.CurrentHearts > 15 || reduceCost && sotnApi.AlucardApi.CurrentHearts > 7)
							{
								if (reduceCost)
								{
									sotnApi.AlucardApi.CurrentHearts -= 7;
								}
								else
								{
									sotnApi.AlucardApi.CurrentHearts -= 16;
								}
								validHolyWater = true;
							}
						}
						else if (subWeaponCount >= 1)
						{
							validHolyWater = true;
						}
						if (validHolyWater)
						{
							activator = 10;
							validHeartUsage = true;
							damage += (int) Math.Round(9 + (highestINT * .7) + (lowestINT * .3));
							damage = (int) Math.Round(damage * (.5));
							damageTypeA = 16;  //Holy
							damageTypeB = 4;  //Water

							damageInterval = 16;
							heartUsageCooldown = 90;
							globalUsageCooldown = 30;

							++subWeaponCount;
							startAxeArmorHeartTickTimer(subWeaponCount, (int) Subweapon.HolyWater, 0, alucardFacing);
						}
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Bible)
					{
						bool validBible = false;
						if (freeSlotCount >= 4 && subWeaponCount == 0)
						{
							if (sotnApi.AlucardApi.CurrentHearts > 20 || reduceCost && sotnApi.AlucardApi.CurrentHearts > 10)
							{
								validBible = true;
								if (reduceCost)
								{
									sotnApi.AlucardApi.CurrentHearts -= 10;
								}
								else
								{
									sotnApi.AlucardApi.CurrentHearts -= 20;
								}
							}
						}
						else if (subWeaponCount >= 1)
						{
							validBible = true;
						}
						if (validBible)
						{
							activator = 10;
							validHeartUsage = true;
							damageTypeA = 1;//Curse
							damageTypeB = 16;//Holy
							damage += (int) Math.Round(9 + (highestINT * .7) + (lowestINT * .3));
							damage = (int) Math.Round(damage / 2.0);
							damageInterval = 12;
							heartUsageCooldown = 150;
							globalUsageCooldown = 150;
							updateXPosition = true;
							updateYPosition = true;

							lockVisual = true;
							++subWeaponCount;

							bibleOffsetPosition(subWeaponCount);
							startAxeArmorHeartTickTimer(subWeaponCount, (int) Subweapon.Bible, 0, alucardFacing);
						}
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Cross)
					{
						if (sotnApi.AlucardApi.CurrentHearts > 11 || (reduceCost && sotnApi.AlucardApi.CurrentHearts > 5))
						{
							if (reduceCost)
							{
								sotnApi.AlucardApi.CurrentHearts -= 6;
							}
							else
							{
								sotnApi.AlucardApi.CurrentHearts -= 12;
							}

							activator = 10;
							validHeartUsage = true;
							damage += (int) Math.Round(9 + (highestINT * .7) + (lowestINT * .3));
							damage = (int) Math.Round(damage * 1.1);
							damageTypeA = 8; //Dark
							damageTypeB = 1; //Curse

							if (Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Staroulite")
							{
								damage = (int) Math.Round(damage * 1.2);
							}
							if (Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Staroulite")
							{
								damage = (int) Math.Round(damage * 1.2);
							}

							lockYSpeed = true;
							lockArc = true;
							lockRisingFalling = true;

							setYSpeed = 187;
							setArcSpeed = 255;
							setRisingFalling = 255;

							damageInterval = 16;
							heartUsageCooldown = 41;
							globalUsageCooldown = 11;

							startAxeArmorHeartTickTimer(reservedSlot, (int) Subweapon.Cross, 0, alucardFacing);
						}
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.ReboundStone)
					{
						if (sotnApi.AlucardApi.CurrentHearts > 11 || reduceCost && sotnApi.AlucardApi.CurrentHearts > 5)
						{
							if (reduceCost)
							{
								sotnApi.AlucardApi.CurrentHearts -= 6;
							}
							else
							{
								sotnApi.AlucardApi.CurrentHearts -= 12;
							}

							//activator = 20;
							activator = 10;
							validHeartUsage = true;
							damage += (int) Math.Round((9 + (highestINT * .7) + (lowestINT * .3))*1.4);
							damageTypeA = 2; //Stone
							damageTypeB = 0; //Hit

							damageInterval = 16;
							heartUsageCooldown = 250;
							globalUsageCooldown = 7;

							startAxeArmorHeartTickTimer(reservedSlot, (int) Subweapon.ReboundStone, 0, alucardFacing);
						}
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Jizzhand)
					{
						if (sotnApi.AlucardApi.CurrentHearts > 2 || reduceCost && sotnApi.AlucardApi.CurrentHearts > 1)
						{
							if (reduceCost)
							{
								sotnApi.AlucardApi.CurrentHearts -= 1;
							}
							else
							{
								sotnApi.AlucardApi.CurrentHearts -= 2;
							}

							activator = 10;
							validHeartUsage = true;
							damage += (int) Math.Round((9 + (highestINT * .7) + (lowestINT * .3)) *.9);

							damageTypeA = 192; //Poison
							damageTypeB = 0; //Neutral

							lockXSpeed = true;
							setXSpeed = 0;

							lockYSpeed = true;
							setYSpeed = 250;

							if (sotnApi.AlucardApi.FacingLeft)
							{
								xPosition -= 17;
							}
							else
							{
								xPosition += 17;
							}

							yPosition -= 4;

							damageInterval = 8;
							heartUsageCooldown = 20;
							globalUsageCooldown = 6;

							startAxeArmorHeartTickTimer(reservedSlot, (int) Subweapon.Jizzhand, 0, alucardFacing);
						}
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Agunea)
					{
						if (sotnApi.AlucardApi.CurrentHearts > 4 || reduceCost && sotnApi.AlucardApi.CurrentHearts > 2)
						{
							if (reduceCost)
							{
								sotnApi.AlucardApi.CurrentHearts -= 2;
							}
							else
							{
								sotnApi.AlucardApi.CurrentHearts -= 4;
							}

							//activator = 17;
							activator = 10;
							validHeartUsage = true;
							damage += (int) Math.Round((9 + (highestINT * .7) + (lowestINT * .3))*.4);
							damageTypeA = 64; //Cut
							damageTypeB = 64; //Thunder

							damageInterval = 10;
							heartUsageCooldown = 19;
							globalUsageCooldown = 4;

							lockXSpeed = true;
							lockYSpeed = true;
							lockArc = true;
							lockRisingFalling = true;

							setXSpeed = 0;
							setYSpeed = 187;
							setArcSpeed = 255;
							setRisingFalling = 255;

							startAxeArmorHeartTickTimer(reservedSlot, (int) Subweapon.Agunea, 0, alucardFacing);
						}
					}
				}
				if (validHeartUsage)
				{
					heartGlobalCooldown = globalUsageCooldown;
					if (sotnApi.AlucardApi.Subweapon == Subweapon.Stopwatch)
					{
						heartLockCooldown = heartUsageCooldown - 45;
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Bible)
					{
						heartLockCooldown = heartUsageCooldown - 10;
					}
					else if (sotnApi.AlucardApi.Subweapon == Subweapon.Dagger)
					{
						heartLockCooldown = heartUsageCooldown - 3;
					}
					else
					{
						heartLockCooldown = heartUsageCooldown - 1;
					}

					if (IsInRoomList(Constants.Khaos.GalamothRooms))
					{
						damage = damage / 3;
					}

					baseDamage = damage;

					checkAxeArmorSubWeaponCrit(damage, out damage);

					if (damage < 1)
					{
						damage = 1;
					}
					if(baseDamage < 1)
					{
						baseDamage = 1;
					}

					if (lockXPosition)
					{
						cheats.AddCheat(address + 2, xPosition, lockName, WatchSize.Byte); //Lock X
						++cheatLockCount;
					}
					else if (!updateXPosition)
					{
						cheats.AddCheat(address + 2, xPosition, cheatName, WatchSize.Byte); //X
						++cheatUsageCount;
					}
					if (lockVisual)
					{
						cheats.AddCheat(address + 3, 0, lockName, WatchSize.Byte); //Lock Visual
						++cheatLockCount;
					}
					if (lockYPosition)
					{
						cheats.AddCheat(address + 6, yPosition, lockName, WatchSize.Byte); //Lock Y
						++cheatLockCount;
					}
					else if (!updateYPosition)
					{
						cheats.AddCheat(address + 6, yPosition, cheatName, WatchSize.Byte); //Y
						++cheatUsageCount;
					}

					if (lockXSpeed)
					{
						cheats.AddCheat(address + 10, setXSpeed * speedModifier, lockName, WatchSize.Byte); //Lock Movement Speed
						++cheatLockCount;
						if (setXSpeed == 0)
						{
							cheats.AddCheat(address + 11, 0, lockName, WatchSize.Byte); // Force facing for 0 speed.
							++cheatLockCount;
						}
					}

					if (lockYSpeed)
					{
						cheats.AddCheat(address + 13, setYSpeed, lockName, WatchSize.Byte); //Lock Movement Speed
						++cheatLockCount;
					}

					if (lockArc)
					{
						cheats.AddCheat(address + 14, setArcSpeed, lockName, WatchSize.Byte); //Lock Movement Arc
						++cheatLockCount;
					}

					if (lockRisingFalling)
					{
						cheats.AddCheat(address + 15, setRisingFalling, lockName, WatchSize.Byte); //Lock Movement Arc
						++cheatLockCount;
					}
					//cheats.AddCheat(address + 22, axeColor, cheatName, WatchSize.Byte);
					cheats.AddCheat(address + 38, activator, cheatName, WatchSize.Byte); //Effect Activation #
					cheats.AddCheat(address + 64, damage, lockName, WatchSize.Byte); //Damage
					cheats.AddCheat(address + 66, damageTypeA, lockName, WatchSize.Byte); //DamageTypeA (Minor)
					cheats.AddCheat(address + 67, damageTypeB, lockName, WatchSize.Byte); //DamageTypeB (Major)
					cheats.AddCheat(address + 73, damageInterval, lockName, WatchSize.Byte); //DamageInterval

					++cheatUsageCount;
					cheatLockCount += 4;

					switch (reservedSlot)
					{
						case 1:
							axeArmorHeart1Timer.Start();
							heartUsage1Cooldown = heartUsageCooldown;
							heartLock1Cooldown = heartLockCooldown;
							axeArmorHeartDamage1 = baseDamage;
							axeArmorHeartLock1CheatCount = cheatLockCount;
							axeArmorHeartUsage1CheatCount = cheatUsageCount;
							break;
						case 2:
							axeArmorHeart2Timer.Start();
							heartUsage2Cooldown = heartUsageCooldown;
							heartLock2Cooldown = heartLockCooldown;
							axeArmorHeartDamage2 = baseDamage;
							axeArmorHeartLock2CheatCount = cheatLockCount;
							axeArmorHeartUsage2CheatCount = cheatUsageCount;
							break;
						case 3:
							axeArmorHeart3Timer.Start();
							heartUsage3Cooldown = heartUsageCooldown;
							heartLock3Cooldown = heartLockCooldown;
							axeArmorHeartDamage3 = baseDamage;
							axeArmorHeartLock3CheatCount = cheatLockCount;
							axeArmorHeartUsage3CheatCount = cheatUsageCount;
							break;
						case 4:
							axeArmorHeart4Timer.Start();
							heartUsage4Cooldown = heartUsageCooldown;
							heartLock4Cooldown = heartLockCooldown;
							axeArmorHeartDamage4 = baseDamage;
							axeArmorHeartLock4CheatCount = cheatLockCount;
							axeArmorHeartUsage4CheatCount = cheatUsageCount;
							break;
						default:
							break;
					}
					if (sotnApi.AlucardApi.Subweapon == Subweapon.Stopwatch && !clockAxe)
					{
						AxeArmorHeartActionCheck(true, subWeaponCount);
					}
					if (subWeaponCount >= 1 && subWeaponCount < 5)
					{
						AxeArmorHeartActionCheck(clockAxe, subWeaponCount);
					}
				}
			}
		}

		void checkAxeArmorSubWeaponCrit(in int damage, out int adjustedDamage)
		{
			adjustedDamage = damage;
			if (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad))
			{
				int critCheck = rng.Next(0, 8);
				if (critCheck == 0)
				{
					adjustedDamage = (int) Math.Round(adjustedDamage * 1.25);
					if (adjustedDamage <= 15)
					{
						adjustedDamage += 2;
					}
					else if (damage <= 31)
					{
						adjustedDamage += 1;
					}
				}
			}
		}

		void startAxeArmorHeartTickTimer(in int reservedSlot, in int subWeapon, in int heartRising, in int heartFacing)
		{
			switch (reservedSlot)
			{
				case 1:
					axeArmorHeart1Weapon = subWeapon;
					heartRising1 = heartRising;
					heartFacing1 = heartFacing;
					axeArmorHeart1FrameTimer.Start();
					break;
				case 2:
					axeArmorHeart2Weapon = subWeapon;
					heartRising2 = heartRising;
					heartFacing2 = heartFacing;
					axeArmorHeart2FrameTimer.Start();
					break;
				case 3:
					axeArmorHeart3Weapon = subWeapon;
					heartRising3 = heartRising;
					heartFacing3 = heartFacing;
					axeArmorHeart3FrameTimer.Start();
					break;
				case 4:
					axeArmorHeart4Weapon = subWeapon;
					heartRising4 = heartRising;
					heartFacing4 = heartFacing;
					axeArmorHeart4FrameTimer.Start();
					break;
				default:
					break;
			}
		}

		void holyWaterOffsetPosition(int subWeaponCount, bool removeOnly = false)
		{
			string lockName = Constants.Khaos.AxeArmorHeartLockName + subWeaponCount;
			int address = Constants.Khaos.AxeArmorEffectStartAddress + (188 * subWeaponCount);

			string cheatName = lockName + "XSpeed";
			var heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "XFacing";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YPosition";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YSpeed";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YArc";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YRisingFalling";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			if (removeOnly)
			{
				switch (subWeaponCount)
				{
					case 1:
						heartFacing1 = 0;
						heartRising1 = 0;
						break;
					case 2:
						heartFacing2 = 0;
						heartRising2 = 0;
						break;
					case 3:
						heartFacing3 = 0;
						heartRising3 = 0;
						break;
					case 4:
						heartFacing4 = 0;
						heartRising4 = 0;
						break;
					default:
						break;
				}
			}
			else
			{
				LiveEntity water = sotnApi.EntityApi.GetLiveEntity(address);

				int xPosition = (int) water.Xpos;
				int yPosition = (int) water.Ypos;

				int xSpeed = 2;
				int xFacing = 0;
				int yTimer = 0;
				int yPositionNew = 0;
				int ySpeed = 4;
				int yArc = 2;
				int yRisingFalling = 0;

				//Rising
				//4
				//253
				//255

				//Falling
				//4
				//2
				//0

				bool yPositionChange = false;
				bool yChange = false;
				bool skipY = false;

				switch (subWeaponCount)
				{
					case 1:
						xFacing = heartFacing1;
						yTimer = heartLock1Cooldown;
						yRisingFalling = heartRising1;
						break;
					case 2:
						xFacing = heartFacing2;
						yTimer = heartLock2Cooldown;
						yRisingFalling = heartRising2;
						break;
					case 3:
						xFacing = heartFacing3;
						yTimer = heartLock3Cooldown;
						yRisingFalling = heartRising3;
						break;
					case 4:
						xFacing = heartFacing4;
						yTimer = heartLock4Cooldown;
						yRisingFalling = heartRising4;
						break;
					default:
						break;
				}
				
				xSpeed = (yTimer / 26) - 2;
				if (xSpeed < 1)
				{
					xSpeed = 0;
				}

				int maxHeight = (10 * subWeaponCount);
				if (yPosition < 200 - maxHeight - (yTimer / 2))
				{
					yChange = true;
					yRisingFalling = 0;
					if (yPosition < 20)
					{
						yPositionNew = 20;
						yArc = 0;
						ySpeed = 0;
						yPositionChange = true;
					}
					else if (yPosition < (20 + (5 * subWeaponCount)))
					{
						skipY = false;
					}
					else
					{
						skipY = true;
					}
				}
				else if (yPosition > 194)
				{
					yChange = true;
					yRisingFalling = 255;
					xSpeed += 1;
					if (yPosition > 235)
					{
						yPositionNew = 234;
						yPositionChange = true;
					}
				}
				else if(yRisingFalling == 0)
				{
					skipY = true;
				}

				if (yRisingFalling > 0)
				{   //Rising
					yArc = 255 - subWeaponCount - 1;
					xFacing = 0;
					xSpeed -= 1;
				}
				if (xSpeed < 1)
				{
					if (yTimer % 20 == 0)
					{
						xSpeed = 1;
					}
					else
					{
						xSpeed = 0;
					}
				}

				if (xFacing > 0 && xSpeed > 0)
				{
					xSpeed = 256 - xSpeed;
				}
				else if (xSpeed == 0)
				{
					xFacing = 0;
				}

				if (yChange)
				{
					switch (subWeaponCount)
					{
						case 1:
							heartRising1 = yRisingFalling;
							break;
						case 2:
							heartRising2 = yRisingFalling;
							break;
						case 3:
							heartRising3 = yRisingFalling;
							break;
						case 4:
							heartRising4 = yRisingFalling;
							break;
						default:
							break;
					}
				}

				cheats.AddCheat(address + 10, xSpeed, lockName + "XSpeed", WatchSize.Byte); //Update XSpeed
				cheats.AddCheat(address + 11, xFacing, lockName + "XFacing", WatchSize.Byte); //Update XFacing

				if (yPositionChange)
				{
					cheats.AddCheat(address + 6, yPositionNew, lockName + "YPosition", WatchSize.Byte); //Update Y Speed

				}

				if (!skipY)
				{
					cheats.AddCheat(address + 13, ySpeed, lockName + "YSpeed", WatchSize.Byte); //Update Y Speed
					cheats.AddCheat(address + 14, yArc, lockName + "YArc", WatchSize.Byte); // Update Y Angle
					cheats.AddCheat(address + 15, yRisingFalling, lockName + "YRisingFalling", WatchSize.Byte); // Update Direction
				}
			}
		}

		void checkSubWeaponDamage(int subWeaponCount, bool removeOnly = false)
		{
			string lockName = Constants.Khaos.AxeArmorHeartLockName + subWeaponCount;
			int address = Constants.Khaos.AxeArmorEffectStartAddress + (188 * subWeaponCount);

			string cheatName = lockName + "Damage";
			var heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			if (removeOnly)
			{
				switch (subWeaponCount)
				{
					case 1:
						axeArmorHeartDamage1 = 0;
						break;
					case 2:
						axeArmorHeartDamage2 = 0;
						break;
					case 3:
						axeArmorHeartDamage3 = 0;
						break;
					case 4:
						axeArmorHeartDamage4 = 0;
						break;
					default:
						break;
				}
			}
			else
			{
				int damage = 0;

				switch (subWeaponCount)
				{
					case 1:
						damage = axeArmorHeartDamage1;
						break;
					case 2:
						damage = axeArmorHeartDamage2;
						break;
					case 3:
						damage = axeArmorHeartDamage3;
						break;
					case 4:
						damage = axeArmorHeartDamage4;
						break;
					default:
						break;
				}
				checkAxeArmorSubWeaponCrit(damage, out damage);
				cheats.AddCheat(address + 64, damage, lockName + "Damage", WatchSize.Byte); //Update Damage
			}
		}

			void crossOffsetPosition(int subWeaponCount, bool removeOnly = false)
		{
			string lockName = Constants.Khaos.AxeArmorHeartLockName + subWeaponCount;
			int address = Constants.Khaos.AxeArmorEffectStartAddress + (188 * subWeaponCount);

			string cheatName = lockName + "XSpeed";
			var heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "XFacing";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			if (removeOnly)
			{
				switch (subWeaponCount)
				{
					case 1:
						heartFacing1 = 0;
						break;
					case 2:
						heartFacing2 = 0;
						break;
					case 3:
						heartFacing3 = 0;
						break;
					case 4:
						heartFacing4 = 0;
						break;
					default:
						break;
				}
			}
			else
			{
				LiveEntity cross = sotnApi.EntityApi.GetLiveEntity(address);
				int xPosition = (int) cross.Xpos;
				int yPosition = (int) cross.Ypos;

				int xSpeed = 2;
				int xFacing = 0;
				int xTimer = 0;

				switch (subWeaponCount)
				{
					case 1:
						xFacing = heartFacing1;
						xTimer = heartLock1Cooldown; 
						break;
					case 2:
						xFacing = heartFacing2;
						xTimer = heartLock2Cooldown;
						break;
					case 3:
						xFacing = heartFacing3;
						xTimer = heartLock3Cooldown;
						break;
					case 4:
						xFacing = heartFacing4;
						xTimer = heartLock4Cooldown;
						break;
					default:
						break;
				}

				if (xTimer > 29)
				{
					xSpeed = 1 + ((xTimer - 29) / 4);
				}
				else if(xTimer <= 29 && xTimer > 20)
				{
					xSpeed = 0;
					//xFacing = 255;
				}
				else
				{
					if(xFacing == 0)
					{
						xFacing = 255;
					}
					else
					{
						xFacing = 0;
					}
					xSpeed = 1 + ((20 - xTimer) / 3);
				}
				if(xFacing > 0 && xSpeed > 0)
				{
					xSpeed = 256 - xSpeed;
				}
				else if (xSpeed == 0)
				{
					xFacing = 0;
				}

				cheats.AddCheat(address + 10, xSpeed, lockName + "XSpeed", WatchSize.Byte); //Update XSpeed
				cheats.AddCheat(address + 11, xFacing, lockName + "XFacing", WatchSize.Byte); //Update XFacing

			}
		}

		void bibleOffsetPosition(int subWeaponCount, bool removeOnly = false)
		{
			string lockName = Constants.Khaos.AxeArmorHeartLockName + subWeaponCount;
			int address = Constants.Khaos.AxeArmorEffectStartAddress + (188 * subWeaponCount);

			string cheatName = lockName + "XOffset";
			var heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YOffset";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "Damage";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			if (!removeOnly)
			{
				int xPosition = (int) sotnApi.AlucardApi.ScreenX;
				int yPosition = (int) sotnApi.AlucardApi.ScreenY;

				switch (subWeaponCount)
				{
					case 1:
						xPosition -= 26;
						break;
					case 2:
						yPosition -= 44;
						break;
					case 3:
						xPosition += 26;
						break;
					case 4:
						yPosition += 26;
						break;
					default:
						break;
				}

				if (xPosition > 255 )
				{
					xPosition = 254;
				} 
				if(xPosition < 1)
				{
					xPosition = 1;
				}
				if (yPosition > 230)
				{
					yPosition = 230;
				}
				if (yPosition < 15)
				{
					yPosition = 15;
				}

				cheats.AddCheat(address + 2, xPosition, lockName + "XOffset", WatchSize.Byte); //Update X
				cheats.AddCheat(address + 6, yPosition, lockName + "YOffset", WatchSize.Byte); //Update Y
			}
		}

		void reboundStoneOffsetPosition(int subWeaponCount, bool removeOnly = false)
		{
			string lockName = Constants.Khaos.AxeArmorHeartLockName + subWeaponCount;
			int address = Constants.Khaos.AxeArmorEffectStartAddress + (188 * subWeaponCount);

			string cheatName = lockName + "XPosition";
			var heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "XSpeed";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "XFacing";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YPosition";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YSpeed";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YArc";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = lockName + "YRisingFalling";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			if (removeOnly)
			{
				switch (subWeaponCount)
				{
					case 1:
						heartFacing1 = 0;
						heartRising1 = 0;
						break;
					case 2:
						heartFacing2 = 0;
						heartRising2 = 0;
						break;
					case 3:
						heartFacing3 = 0;
						heartRising3 = 0;
						break;
					case 4:
						heartFacing4 = 0;
						heartRising4 = 0;
						break;
					default:
						break;
				}
			}
			else
			{
				LiveEntity stone = sotnApi.EntityApi.GetLiveEntity(address);

				uint xPosition = stone.Xpos;
				uint yPosition = stone.Ypos;
				int xPositionNew = 0;
				int yPositionNew = 0;

				int xSpeed = 2;
				int xFacing = 0;
				int ySpeed = 4;
				int yArc = 2;
				int yRisingFalling = 0;

				//Rising
				//4
				//253
				//255

				//Falling
				//4
				//2
				//0

				bool xPositionChange = false;
				bool xChange = false;
				bool yPositionChange = false;
				bool yChange = false;

				switch (subWeaponCount)
				{
					case 1:
						xFacing = heartFacing1;
						yRisingFalling = heartRising1; 
						break;
					case 2:
						xFacing = heartFacing2;
						yRisingFalling = heartRising2;
						break;
					case 3:
						xFacing = heartFacing3;
						yRisingFalling = heartRising3;
						break;
					case 4:
						xFacing = heartFacing4;
						yRisingFalling = heartRising4;
						break;
					default:
						break;
				}

				int ReboundOffset = 2;
				if (sotnApi.AlucardApi.State > 40)
				{
					ReboundOffset += 8;
				}
				if (isAxeArmorMist)
				{
					ReboundOffset += 2;
				}
				if(wolfDashActive)
				{
					ReboundOffset += 30;
				}
				else if (wolfRunActive)
				{
					ReboundOffset += 15;
				}


				int xLeftSoftRebound = 20 + ReboundOffset;
				int xLeftHardRebound = xLeftSoftRebound - 10;
				int xRightSoftRebound = 235 - ReboundOffset;
				int xRightHardRebound = xRightSoftRebound + 10;
				int yUpSoftRebound = 36 + ReboundOffset;
				int yUpHardRebound = yUpSoftRebound - 20;
				int yDownSoftRebound = 210 + ReboundOffset;
				int yDownHardRebound = yDownSoftRebound + 20;


				if (xPosition < xLeftSoftRebound)
				{
					xChange = true;
					xFacing = 0;
					if(xPosition < xLeftHardRebound)
					{
						xPositionNew = xLeftHardRebound + 1 + ReboundOffset / 5;
						xPositionChange = true;
					}
				}
				else if (xPosition > xRightSoftRebound)
				{
					xChange = true;
					xFacing = 255;
					if (xPosition > xRightHardRebound)
					{
						xPositionNew = xRightHardRebound - 1 - ReboundOffset / 5;
						xPositionChange = true;
					}
				}
				if (xFacing > 0)
				{ //Facing
					xSpeed = 254;
				}
				if (xChange)
				{
					switch (subWeaponCount)
					{
						case 1:
							heartFacing1 = xFacing;
							break;
						case 2:
							heartFacing2 = xFacing;
							break;
						case 3:
							heartFacing3 = xFacing;
							break;
						case 4:
							heartFacing4 = xFacing;
							break;
						default:
							break;
					}
				}

				if (yPosition < yUpSoftRebound)
				{
					yChange = true;
					yRisingFalling = 0;
					if (yPosition < yUpHardRebound)
					{
						yPositionNew = yUpHardRebound + 1 + ReboundOffset / 5;
						yPositionChange = true;
					}
				}
				else if (yPosition > yDownSoftRebound)
				{
					yChange = true;
					yRisingFalling = 255;
					if (yPosition > yDownHardRebound)
					{
						yPositionNew = yDownHardRebound - 1 - ReboundOffset / 5;
						yPositionChange = true;
					}
				}

				if (xPosition < 5 + (ReboundOffset * 4 / 5)
					|| xPosition > 250 - (ReboundOffset * 4 / 5)
					|| yPosition < 10 + (ReboundOffset * 4 / 5)
					|| yPosition > 245 - (ReboundOffset * 4 / 5)
					)
				{
					int xOffset = rng.Next(-5, 5);
					int yOffset = rng.Next(-5, 5);

					xPositionChange = true;
					xPositionNew = (int) sotnApi.AlucardApi.ScreenX + xOffset;
					yPositionChange = true;
					yPositionNew = (int) sotnApi.AlucardApi.ScreenY + yOffset;
				}

				if (yRisingFalling > 0)
				{   //Rising
					yArc = 253;
				}
				else
				{
					yArc = 2;
				}
				if (yChange)
				{
					switch (subWeaponCount)
					{
						case 1:
							heartRising1 = yRisingFalling;
							break;
						case 2:
							heartRising2 = yRisingFalling;
							break;
						case 3:
							heartRising3 = yRisingFalling;
							break;
						case 4:
							heartRising4 = yRisingFalling;
							break;
						default:
							break;
					}
				}

				if (xPositionChange)
				{
					cheats.AddCheat(address + 2, xPositionNew, lockName + "XPosition", WatchSize.Byte); //Lock X
				}
				if (yPositionChange)
				{
					cheats.AddCheat(address + 6, yPositionNew, lockName + "YPosition", WatchSize.Byte); //Lock Y
				}

				cheats.AddCheat(address + 10, xSpeed, lockName + "XSpeed", WatchSize.Byte); //Update XSpeed
				cheats.AddCheat(address + 11, xFacing, lockName + "XFacing", WatchSize.Byte); //Update XFacing
				cheats.AddCheat(address + 13, ySpeed, lockName + "YSpeed", WatchSize.Byte); //Update Y Speed
				cheats.AddCheat(address + 14, yArc, lockName + "YArc", WatchSize.Byte); // Update Y Angle
				cheats.AddCheat(address + 15, yRisingFalling, lockName + "YRisingFalling", WatchSize.Byte); // Update Direction
			}
		}

		void aguneaOffsetPosition(int subWeaponCount, bool removeOnly = false)
		{
			string lockName = Constants.Khaos.AxeArmorHeartLockName + subWeaponCount;
			int address = Constants.Khaos.AxeArmorEffectStartAddress + (188 * subWeaponCount);

			string cheatName = Constants.Khaos.AxeArmorHeartLockName + subWeaponCount + "XOffset";
			var heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			cheatName = Constants.Khaos.AxeArmorHeartLockName + subWeaponCount + "YOffset";
			heartCheat = cheats.GetCheatByName(cheatName);
			cheats.RemoveCheat(heartCheat);

			if (!removeOnly)
			{

				int yTimer = 0;
				uint xPosition = 0;
				uint yPosition = 0;

				switch (subWeaponCount)
				{
					case 1:
						yTimer = heartLock1Cooldown;
						break;
					case 2:
						yTimer = heartLock2Cooldown;
						break;
					case 3:
						yTimer = heartLock3Cooldown;
						break;
					case 4:
						yTimer = heartLock4Cooldown;
						break;
					default:
						break;
				}

				if (yTimer % 2 == 0 || yTimer == 19)
				{
					AguneaEntityCheck(out xPosition, out yPosition);

					cheats.AddCheat(address + 2, (int) xPosition, lockName + "XOffset", WatchSize.Byte); //Update X
					cheats.AddCheat(address + 6, (int) yPosition, lockName + "YOffset", WatchSize.Byte); //Update Y
				}
			}
		}

		public void AguneaEntityCheck(out uint xPosition, out uint yPosition)
		{
			long start = Game.EnemyEntitiesStart;
			int count = 155; //Normal - 131?

			int xOffset = rng.Next(-15, 15);
			int yOffset = rng.Next(-15, 15);
			xPosition = (uint) (sotnApi.AlucardApi.ScreenX + xOffset);
			yPosition = (uint) (sotnApi.AlucardApi.ScreenY + yOffset);

			LiveEntity currentActor;
			if (!sotnApi.AlucardApi.HasControl()
				|| sotnApi.GameApi.IsInMenu()
				|| sotnApi.GameApi.CanSave()
				|| IsInRoomList(Constants.Khaos.LoadingRooms)
				|| IsInRoomList(Constants.Khaos.ShopRoom)
				)

			{
				//Do nothing for now
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					currentActor = sotnApi.EntityApi.GetLiveEntity(start);

					if (currentActor.Damage != 0 && currentActor.Def != 255 && currentActor.Hp != 32767)
					{
						if(currentActor.Xpos < 255 && currentActor.Ypos < 255)
						{
							xPosition = currentActor.Xpos;
							yPosition = currentActor.Ypos;
							//Console.WriteLine($"xPos{xPosition},yPos{yPosition}");
							return;
						}
					}
					start += Entities.Offset;
				}
			}
		}

		private void AxeArmorContactDamageTick(object sender, EventArgs e)
		{
			if (axeArmorContactDamage > 0)
			{
				if(wolfCurrentSpeed >= Constants.Khaos.AxeArmorWolfMinRunSpeed && axeArmorContactDamageCooldown > 1)
				{
					axeArmorContactDamageCooldown = 2;
				}
				else if (wolfCurrentSpeed >= Constants.Khaos.AxeArmorWolfMinStopSpeed)
				{
					--axeArmorContactDamageCooldown;
				}
				if (axeArmorContactDamageCooldown > 0 && wolfCurrentSpeed <= Constants.Khaos.AxeArmorWolfMinRunSpeed)
				{
					--axeArmorContactDamageCooldown;
					sotnApi.AlucardApi.ContactDamage = 0;
					//notificationService.AddMessage($"ContactDamage = {axeArmorContactDamage}, Cooldown = {axeArmorContactDamageCooldown}");
				}
				else
				{
					sotnApi.AlucardApi.ContactDamage = axeArmorContactDamage;
					axeArmorContactDamageCooldown = axeArmorContactDamageMaxCooldown;
				}
			}
			else
			{
				sotnApi.AlucardApi.ContactDamage = 0;
				axeArmorContactDamageCooldown = axeArmorContactDamageMaxCooldown;
				axeArmorContactDamageTimer.Stop();
			}
		}

		private void AxeArmorFrameTickCheck(in int subWeaponCount, in int subWeapon, in bool removeOnly)
		{
			switch (subWeapon)
			{
				case 3: //Holy Water
					holyWaterOffsetPosition(subWeaponCount, removeOnly);
					checkSubWeaponDamage(subWeaponCount, removeOnly);
					break;
				case 4: //Cross
					crossOffsetPosition(subWeaponCount, removeOnly);
					checkSubWeaponDamage(subWeaponCount, removeOnly);
					break;
				case 5: //Bible
					bibleOffsetPosition(subWeaponCount, removeOnly);
					checkSubWeaponDamage(subWeaponCount, removeOnly);
					break;
				case 7: //ReboundStone
					reboundStoneOffsetPosition(subWeaponCount, removeOnly);
					checkSubWeaponDamage(subWeaponCount, removeOnly);
					break;
				case 9: //Agunea
					aguneaOffsetPosition(subWeaponCount, removeOnly);
					checkSubWeaponDamage(subWeaponCount, removeOnly);
					break;
				default:
					checkSubWeaponDamage(subWeaponCount, removeOnly);
					break;
			}
		}

		private void AxeArmorHeart1FrameTimerTick(object sender, EventArgs e)
		{
			if (heartLock1Cooldown <= 0)
			{
				AxeArmorFrameTickCheck(1, axeArmorHeart1Weapon, true);
				axeArmorHeart1FrameTimer.Stop();
			}
			else
			{
				AxeArmorFrameTickCheck(1, axeArmorHeart1Weapon, false);
			}
		}
		private void AxeArmorHeart2FrameTimerTick(object sender, EventArgs e)
		{
			if (heartLock2Cooldown <= 0)
			{
				AxeArmorFrameTickCheck(2, axeArmorHeart2Weapon, true);
				axeArmorHeart2FrameTimer.Stop();
			}
			else
			{
				AxeArmorFrameTickCheck(2, axeArmorHeart2Weapon, false);
			}
		}
		private void AxeArmorHeart3FrameTimerTick(object sender, EventArgs e)
		{
			if (heartLock3Cooldown <= 0)
			{
				AxeArmorFrameTickCheck(3, axeArmorHeart3Weapon, true);
				axeArmorHeart3FrameTimer.Stop();
			}
			else
			{
				AxeArmorFrameTickCheck(3, axeArmorHeart3Weapon, false);
			}
		}
		private void AxeArmorHeart4FrameTimerTick(object sender, EventArgs e)
		{
			if (heartLock4Cooldown <= 0)
			{
				AxeArmorFrameTickCheck(4, axeArmorHeart4Weapon, true);
				axeArmorHeart4FrameTimer.Stop();
			}
			else
			{
				AxeArmorFrameTickCheck(4, axeArmorHeart4Weapon, false);
			}
		}
		private void AxeArmorHeart1Off(object sender, EventArgs e)
		{
			string cheatName = Constants.Khaos.AxeArmorHeartName + "1";
			for (int i = 0; i < axeArmorHeartUsage1CheatCount; i++)
			{
				var heartCheat = cheats.GetCheatByName(cheatName);
				cheats.RemoveCheat(heartCheat);
			}
			axeArmorHeartUsage1CheatCount = 0;
			axeArmorHeart1Timer.Stop();
		}
		private void AxeArmorHeart2Off(object sender, EventArgs e)
		{
			string cheatName = Constants.Khaos.AxeArmorHeartName + "2";
			for (int i = 0; i < axeArmorHeartUsage2CheatCount; i++)
			{
				var heartCheat = cheats.GetCheatByName(cheatName);
				cheats.RemoveCheat(heartCheat);
			}
			axeArmorHeartUsage2CheatCount = 0;
			axeArmorHeart2Timer.Stop();
		}
		private void AxeArmorHeart3Off(object sender, EventArgs e)
		{
			string cheatName = Constants.Khaos.AxeArmorHeartName + "3";
			for (int i = 0; i < axeArmorHeartUsage3CheatCount; i++)
			{
				var heartCheat = cheats.GetCheatByName(cheatName);
				cheats.RemoveCheat(heartCheat);
			}
			axeArmorHeartUsage3CheatCount = 0;
			axeArmorHeart3Timer.Stop();
		}
		private void AxeArmorHeart4Off(object sender, EventArgs e)
		{
			string cheatName = Constants.Khaos.AxeArmorHeartName + "4";
			for (int i = 0; i < axeArmorHeartUsage4CheatCount; i++)
			{
				var heartCheat = cheats.GetCheatByName(cheatName);
				cheats.RemoveCheat(heartCheat);
			}
			axeArmorHeartUsage4CheatCount = 0;
			axeArmorHeart4Timer.Stop();
		}

		public void AxeArmorEffect(string user = "Mayhem")
		{
			axeArmorEffectTimer.Start();
		}

		private void AxeArmorEffectOff(object sender, EventArgs e)
		{
			axeArmorEffectTimer.Stop();
		}

		private void RewindOff(object sender, EventArgs e)
		{
			//notificationService.AddMessage($"{KhaosActionNames.Rewind} Off");
			rewind.Disable();

			if (rewindDelay)
			{
				rewindDelay = false;
			}
			else if (rewindActive)
			{
				rewindActive = false;
				rewindDelay = true;

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

				sotnApi.AlucardApi.WarpsFirstCastle = WarpsFirstCastle;
				sotnApi.AlucardApi.WarpsSecondCastle = WarpsSecondCastle;
			}
			else if(rewindSettingsFix)
			{
				rewindTimer.Stop();
				for (int i = 0; i < axeArmorMenuCheatCount; i++)
				{
					var colorCheat = cheats.GetCheatByName(Constants.Khaos.OptionMenuName);
					cheats.RemoveCheat(colorCheat);
				}
				rewindSettingsFix = false;
				axeArmorMenuCheatCount = 0;
			}
		}

		private void LibraryOff(object sender, EventArgs e)
		{
			//notificationService.AddMessage($"{KhaosActionNames.Library} Off");
			alucardEffect.Disable();
			var libraryCheat = cheats.GetCheatByName(Constants.Khaos.AxeArmorLibrary);
			cheats.RemoveCheat(libraryCheat);
			libraryTimer.Stop();
			alucardEffectsLocked = false;
		}

		public void RespawnRichter(string user = "Mayhem")
		{
			shaftHpSet = false;
			fightRichter1.Enable();
			fightRichter2.Enable();
			fightRichter3.Enable();
			//saveRichter.Enable();
			richterCutscene1.PokeValue(0);
			richterCutscene1.Enable();
			richterCutscene2.PokeValue(0);
			richterCutscene2.Enable();
			respawnRichterTimer.Start();
			//string message = $"Test: Respawn Richter Activated.";
			//notificationService.AddMessage(message);
		}

		public void RespawnRichterOff(object sender, EventArgs e)
		{
			fightRichter1.Disable();
			fightRichter2.Disable();
			fightRichter3.Disable();
			//saveRichter.Disable();
			richterCutscene1.Disable();
			//string message = $"Test: Respawn Richter Deactivated.";
			//notificationService.AddMessage(message);
			respawnRichterTimer.Stop();
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
					if (sotnApi.AlucardApi.MaxtHp > subtractHPGiven)
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
				if (sotnApi.AlucardApi.MaxtHp > hpGiven)
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
					if (statCheck > subtractStatGiven && statCheck - subtractStatGiven > minStat)
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
				if (statCheck > strGiven && statCheck - strGiven > minStat)
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
					lckGivenPaused -= subtractStatGiven;
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

		public int checkLockedNeutralLevel(int level)
		{
			if (lockedNeutralLevel == 0)
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
					level = (int) toolConfig.Khaos.NeutralMinLevel;
				}
				else
				{
					level = (int) toolConfig.Khaos.NeutralMaxLevel;
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
				percentageHearts -= (1.00 - percentageMP);
				percentageMP = 0;
			}
			if (heartsLocked || currentHearts < 2)
			{
				percentageGold -= (1.00 - percentageHearts);
				percentageHearts = 0;
			}
			if (currentGold < 2 && percentageGold < 1)
			{
				percentageGold = 0;
			}

			//notificationService.AddMessage($"Debug - {merchant}; a{a} b{b} c{c}; {percentageMP}MP/{percentageHearts}H/{percentageGold}GP");
			//notificationService.AddMessage($"Debug - {merchant}; a{a} b{b} c{c}; {percentageMP}MP/{percentageHearts}H/{percentageGold}GP");

			uint flatRemoval = (uint) (5 - merchantLevel);

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
			if (buffLckNeutralActive)
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

			if (sotnApi.AlucardApi.Cloak == Equipment.CloakStart && axeArmorActive)
			{
				sotnApi.AlucardApi.Cloak = (uint)(Equipment.Items.IndexOf("Joseph's cloak") - Equipment.HandCount - 1);
			}

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

			sotnApi.AlucardApi.CurrentHp = newHp < 1 ? (uint) 1 : newHp;
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
				isAlucardColorSecondCastle = alucardSecondCastle == true ? true : false;

				if (useFirstCastleColor) {
					while (color == alucardColor || previousAlucardColors.Contains(color))
					{
						color = Constants.Khaos.alucardColorsFirstCastle[rng.Next(0, Constants.Khaos.alucardColorsFirstCastle.Length)];
					}
				}
				else if (useSecondCastleColor) {
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

					int newSubWeaponDamage = (int) (15 + (sotnApi.AlucardApi.Int * 2) + equipmentINT);

					if (Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Brilliant mail")
					{
						newSubWeaponDamage = (int) (newSubWeaponDamage * 1.25);
					}

					subWeaponDamage1.PokeValue(newSubWeaponDamage);
					subWeaponDamage1.Enable();
					subWeaponDamage2.PokeValue((int)(newSubWeaponDamage - (newSubWeaponDamage * .1)));
					subWeaponDamage2.Enable();
					subWeaponDamage3.PokeValue((int) (newSubWeaponDamage - (newSubWeaponDamage * .2)));
					subWeaponDamage3.Enable();
					subWeaponDamage4.PokeValue((int) (newSubWeaponDamage - (newSubWeaponDamage * .3)));
					subWeaponDamage4.Enable();
					subWeaponDamage5.PokeValue((int) (newSubWeaponDamage - (newSubWeaponDamage * .4)));
					subWeaponDamage5.Enable();
					subWeaponDamage6.PokeValue((int) (newSubWeaponDamage - (newSubWeaponDamage * .5)));
					subWeaponDamage6.Enable();
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
			subWeaponDamage1.Disable();
			subWeaponDamage2.Disable();
			subWeaponDamage3.Disable();
			subWeaponDamage4.Disable();
			subWeaponDamage5.Disable();
			subWeaponDamage6.Disable();
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

			uint tempStrength = (uint) (Constants.Khaos.UnarmedStr * unarmedLevel);
			addSTRGiven(tempStrength);

			if (unarmedLevel > 1)
			{

				uint tempCon = 0;
				if (toolConfig.Khaos.KindAndFair)
				{
					tempCon = (uint) (2 * Constants.Khaos.UnarmedCon * unarmedLevel);
				}
				else
				{
					tempCon = (uint) (Constants.Khaos.UnarmedCon * (unarmedLevel - 1));
				}

				addCONGiven(tempCon);
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
			uint strRemoved = (uint) (prevLevel * Constants.Khaos.UnarmedStr);
			if (strGiven < strRemoved)
			{
				strRemoved = strGiven;
			}


			uint conRemoved = 0;
			if (toolConfig.Khaos.KindAndFair)
			{
				conRemoved = (uint) (2 * Constants.Khaos.UnarmedCon * unarmedLevel);
			}
			else
			{
				conRemoved = (uint) (Constants.Khaos.UnarmedCon * (unarmedLevel - 1));
			}
				
			if (conGiven < conRemoved)
			{
				conRemoved = conGiven;
			}
			takeCONGiven(conRemoved);
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
				if (toolConfig.Khaos.KindAndFair)
				{
					addCONGiven(Constants.Khaos.Rushdown1KFCon);
				}
				else
				{
					addCONGiven(Constants.Khaos.Rushdown1Con);
				}

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
					if (toolConfig.Khaos.KindAndFair)
					{
						addCONGiven(Constants.Khaos.Rushdown2KFCon);
					}
					else
					{
						addCONGiven(Constants.Khaos.Rushdown2Con);
					}

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
				if (toolConfig.Khaos.KindAndFair)
				{
					takeCONGiven(Constants.Khaos.Rushdown1KFCon);
				}
				else
				{
					takeCONGiven(Constants.Khaos.Rushdown1Con);
				}
			}

			if (rushDownLevel == 3)
			{
				if (toolConfig.Khaos.KindAndFair)
				{
					takeCONGiven(Constants.Khaos.Rushdown2KFCon);
				}
				else
				{
					takeCONGiven(Constants.Khaos.Rushdown2Con);
				}
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
				uint roundingOffset = (uint) (CalculatedStatPool > ActualStatPool ? CalculatedStatPool - ActualStatPool : 0);
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
			uint pointsHp = (uint) (80 + Math.Round(hpPercent * pointsPool));
			uint pointsMp = (uint) (30 + pointsPool - Math.Round(hpPercent * pointsPool));

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
					pointsPool += offset;
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
				if (toolConfig.Khaos.RestrictedItemSwap)
				{
					while (result == 184 || result == 204 || result == 242 || result == 243)
					{// Progression Items: Lines - 197,217,255,256
						result = rng.Next(0, Equipment.Items.Count);
					}
				}

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
			if (subWeaponsLocked)
			{
				sotnApi.AlucardApi.Subweapon = 0;
			}
			else
			{
				var subweapons = Enum.GetValues(typeof(Subweapon));
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

			if (IsInRoomList(Constants.Khaos.MistGateRooms))
			{
				sotnApi.AlucardApi.GrantRelic(Relic.FormOfMist, true);
			}

			if (alucardSecondCastle)
			{
				int roll = rng.Next(0, Constants.Khaos.FlightRelics.Count);
				foreach (Relic relic in Constants.Khaos.FlightRelics[roll])
				{
					sotnApi.AlucardApi.GrantRelic((Relic) relic, true);
				}
				if (IsInRoomList(Constants.Khaos.AkmodanZone))
				{
					sotnApi.AlucardApi.GrantRelic(Relic.SoulOfWolf, true);
				}


			}
			else
			{
				if (IsInRoomList(Constants.Khaos.SwitchRoom))
				{
					sotnApi.AlucardApi.GrantRelic(Relic.JewelOfOpen, true);
				}
				else if (IsInRoomList(Constants.Khaos.OlroxZone))
				{ //If in Olrox area, always grant wolf + gravity boots to escape
					sotnApi.AlucardApi.GrantRelic(Relic.SoulOfWolf, true);
					sotnApi.AlucardApi.GrantRelic(Relic.GravityBoots, true);
				}
				else if (IsInRoomList(Constants.Khaos.HolyGlassesZone))
				{
					sotnApi.AlucardApi.GrantRelic(Relic.GravityBoots, true);
				}
				else if (alucardMapY > 44)
				{ //If in catacombs, grant flight to avoid softlocks
					sotnApi.AlucardApi.GrantRelic(Relic.FormOfMist, true);
					sotnApi.AlucardApi.GrantRelic(Relic.PowerOfMist, true);
				}
				if (!sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat)
					&& !(sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist) && !sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
					&& !(sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) && sotnApi.AlucardApi.HasRelic(Relic.GravityBoots))
					)
				{ //If no flight relics in first castle, lean towards granting Leapstone as a catch-all softlock prevention
					sotnApi.AlucardApi.GrantRelic(Relic.LeapStone, true);
				}
			}

			sotnApi.AlucardApi.GrantItemByName("Library card");
			//notificationService.AddMessage($"1 Library Card)");

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

			uint newRightHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			uint newLeftHand = (uint) rng.Next(0, Equipment.HandCount + 1);
			uint newHelm = Equipment.HelmStart + (uint) rng.Next(0, Equipment.HelmCount + 1);
			uint newArmor = (uint) rng.Next(0, Equipment.ArmorCount + 1);
			uint newCloak = Equipment.CloakStart + (uint) rng.Next(0, Equipment.CloakCount + 1);
			uint newAccessory1 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);
			uint newAccessory2 = Equipment.AccessoryStart + (uint) rng.Next(0, Equipment.AccessoryCount + 1);

			//Reroll the new item if it is progression.
			if (toolConfig.Khaos.RestrictedItemSwap)
			{
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
			if (heartsOnlyActive || rushDownActive)
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
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistCurse);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistStone);
					sotnApi.AlucardApi.ActivatePotion(Potion.HighPotion);
					notificationService.AddMessage($"{user} gave High Potion");
					break;
				case 11:
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistFire);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistIce);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistThunder);
					notificationService.AddMessage($"{user} gave Fire/Cold/Thunder");
					break;
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
			uint baseGain = (uint) ((5 + sotnApi.AlucardApi.Level) * 2);
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

		public void RandomizeMajorPotion(string user = "Mayhem")
		{
			bool highMp = sotnApi.AlucardApi.CurrentMp > sotnApi.AlucardApi.MaxtMp * 0.6;
			bool highHp = sotnApi.AlucardApi.CurrentHp > sotnApi.AlucardApi.MaxtHp * 0.6;
			bool highHearts = sotnApi.AlucardApi.CurrentHearts > sotnApi.AlucardApi.MaxtHearts * 0.6;
			int min = 2;
			int max = 4;
			uint baseGain = (uint) (5000 + (1000 + sotnApi.AlucardApi.Level) * 2);
			uint addGold = 0;

			int result = rng.Next(min, max);

			if (timeStopActive)
			{
				addGold = 100 + (uint) (10 * baseGain * (rng.NextDouble()));
				result = 1;
			}

			switch (result)
			{
				case 1:
					sotnApi.AlucardApi.Gold += addGold;
					notificationService.AddMessage($"{user} gave super gold {sotnApi.AlucardApi.Gold}");
					break;
				case 2:
					sotnApi.AlucardApi.ActivatePotion(Potion.StrPotion);
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					sotnApi.AlucardApi.ActivatePotion(Potion.SmartPotion);
					sotnApi.AlucardApi.ActivatePotion(Potion.LuckPotion);
					notificationService.AddMessage($"{user} gave All Stats");
					break;
				case 3:
					sotnApi.AlucardApi.ActivatePotion(Potion.ShieldPotion);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistFire);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistThunder);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistIce);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistHoly);
					sotnApi.AlucardApi.ActivatePotion(Potion.ResistDark);
					notificationService.AddMessage($"{user} gave All Resistances");
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
			uint newMP = (uint) Math.Max(0, Math.Round(currentMp - (currentMp * percentageMP) - 5 - vladRelicsObtained - toolConfig.Khaos.CurseModifier - sotnApi.AlucardApi.Level));
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
			if (result == 6)
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


			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.HPForMP;

			if (meterFull)
			{
				name = "Super " + name;
				superHPForMP = true;
				SpendMayhemMeter();
				notificationService.AddMessage($"{user} used Super {KhaosActionNames.HPForMP}");

				HPForMPTickTimer.Start();
			}
			else
			{
				notificationService.AddMessage($"{user} used {KhaosActionNames.HPForMP}");
			}
	
			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
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

				if (superHPForMP && currentHp > spentMp * 2)
				{
					sotnApi.AlucardApi.CurrentMp += (uint) spentMp;
					spentMp *= 2;
					sotnApi.AlucardApi.CurrentHp -= (uint) spentMp;
				}
				else if (!superHPForMP && currentHp > spentMp)
				{
					sotnApi.AlucardApi.CurrentMp += (uint) spentMp;
					sotnApi.AlucardApi.CurrentHp -= (uint) spentMp;
				}
				else
				{
					sotnApi.AlucardApi.RightHand = (uint) Equipment.Items.IndexOf("Pizza");
					sotnApi.AlucardApi.LeftHand = (uint) Equipment.Items.IndexOf("Pizza");
					sotnApi.AlucardApi.CurrentMp = 0;
					sotnApi.AlucardApi.CurrentHp = 0;
					hpForMPDeathTimer.Start();
				}
			}
		}

		private void HPForMPGain(Object sender, EventArgs e)
		{
			uint conversion = 1;
			uint regenMultiplier = 1;

			if (sotnApi.AlucardApi.CurrentHp > 1 && (sotnApi.AlucardApi.CurrentMp < sotnApi.AlucardApi.MaxtMp))
			{
				sotnApi.AlucardApi.CurrentHp -= (uint) (conversion * regenMultiplier);
				sotnApi.AlucardApi.CurrentMp += (uint) (conversion * regenMultiplier);
			}
		}
		private void HPForMPOff(Object sender, EventArgs e)
		{
			HPForMPTimer.Stop();
			if (superHPForMP)
			{
				HPForMPTickTimer.Stop();
			}
			mpLocked = false;
			HPForMPActive = false;
			superHPForMP = false;
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
				superUnderwater = true;
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
			superUnderwater = false;
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
					MajorTrap(user);
					break;
			}
		}

		public int rollCurseCommandCategory()
		{
			int result = 1;
			bool meterFull = MayhemMeterFull();

			
			int baseMinorThreshold = 14;
			int baseModerateThreshold = 5;
			int baseMajorThreshold = 1;

			int baseAdjustment = (int) (vladRelicsObtained * .5 * toolConfig.Khaos.CurseModifier);

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

			if (meterFull)
			{
				result = 3;
				SpendMayhemMeter();
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

			int result = effectNumbers[rng.Next(min, max)];

			ReduceStats(out uint newStr, out uint newCon, out uint newInt, out uint newLck, out uint newHP, out uint newMP, 
				out uint newHearts, out uint newLevel, out uint newExperience);

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
					RemoveItems(false, true, false);
					notificationService.AddMessage($"{user} stole gold!");
					break;
				default:
					break;
			}
			Alert(KhaosActionNames.ModerateTrap);
		}

		public void MajorTrap(string user = "Mayhem", int forcedEffect = 0)
		{

			int result;
			if(forcedEffect == 0)
			{
				List<int> effectNumbers = new List<int>() {1,2,3};
				int min = 0;
				int max = effectNumbers.Count;

				if (heartsLocked || mpLocked)
				{
					effectNumbers.RemoveAll(item => item == 2);
				}
				if (sotnApi.AlucardApi.LeftHand == 0 && sotnApi.AlucardApi.RightHand == 0)
				{
					effectNumbers.RemoveAll(item => item == 3);
				}

				max = effectNumbers.Count;
				result = effectNumbers[rng.Next(min, max)];
			}
			else 
			{
				result = forcedEffect;
			}

			switch (result)
			{
				case 1:
					ReduceStats(out uint newStr, out uint newCon, out uint newInt, out uint newLck, out uint newHP, out uint newMP,
				out uint newHearts, out uint newLevel, out uint newExperience, 7);

					sotnApi.AlucardApi.MaxtHearts = newHearts;
					if (!heartsLocked && sotnApi.AlucardApi.CurrentHearts > newHearts)
					{
						sotnApi.AlucardApi.CurrentHearts = newHearts;
					}
					sotnApi.AlucardApi.MaxtMp = newMP;
					if (!mpLocked && sotnApi.AlucardApi.CurrentMp > newMP)
					{
						sotnApi.AlucardApi.CurrentMp = newMP;
					}
					sotnApi.AlucardApi.MaxtHp = newHP;
					if (sotnApi.AlucardApi.CurrentHp > newHP)
					{
						sotnApi.AlucardApi.CurrentHp = newHP;
					}
					sotnApi.AlucardApi.Str = newStr;
					if (!rushDownActive)
					{
						sotnApi.AlucardApi.Con = newCon;
					}
					sotnApi.AlucardApi.Int = newInt;
					sotnApi.AlucardApi.Lck = newLck;
	
					notificationService.AddMessage($"{user} slam stole stats!");
					SpawnSlamHitbox();
					//queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user, false, false, false)));
					break;
				case 2:
					uint currentHp = sotnApi.AlucardApi.CurrentHp;
					uint currentMp = sotnApi.AlucardApi.CurrentMp;
					uint currentHearts = sotnApi.AlucardApi.CurrentHearts;

					double percentageHP = 25 / 100;
					double percentageMP = 50 / 100;
					double percentageHearts = 50 / 100;

					newHP = (uint) Math.Max(1, Math.Round(currentHp - (currentHp * percentageHP) - 5 - vladRelicsObtained - toolConfig.Khaos.CurseModifier - sotnApi.AlucardApi.Level));
					newMP = (uint) Math.Max(0, Math.Round(currentMp - (currentMp * percentageMP) - 5 - vladRelicsObtained - toolConfig.Khaos.CurseModifier - sotnApi.AlucardApi.Level));
					newHearts = (uint) Math.Max(0, Math.Round(currentHearts - (currentHearts * percentageHearts) - 5 - vladRelicsObtained - toolConfig.Khaos.CurseModifier - sotnApi.AlucardApi.Level));

					notificationService.AddMessage($"{user} slam stole HP/MP/H!");
					SpawnSlamHitbox();
					//queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user, false, false, false)));
					break;
				case 3:
					sotnApi.AlucardApi.LeftHand = 0;
					sotnApi.AlucardApi.RightHand = 0;
					RemoveItems(false, true, false);
					notificationService.AddMessage($"{user} slam stole items!");
					SpawnSlamHitbox();
					//queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user, false, false, false)));
					break;
				default:
					break;
			}
			Alert(KhaosActionNames.MajorTrap);
		}

		public void ReduceStats(out uint newStr, out uint newCon, out uint newInt, out uint newLck, out uint newHP, 
		out uint newMP, out uint newHearts, out uint newLevel, out uint newExperience, in int statsToReduce = 1)
		{
			adjustMinStats();

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
				adjustedMinStr = minStr + strGiven;
				adjustedMinCon = minCon + conGiven;
				adjustedMinInt = minInt + intGiven;
				adjustedMinLck = minLck + lckGiven;
			}

			int baseRemoved = (int) (2 + (.5 * toolConfig.Khaos.CurseModifier));

			newStr = (uint) (sotnApi.AlucardApi.Str - baseRemoved);
			newCon = (uint) (sotnApi.AlucardApi.Con - baseRemoved);
			newInt = (uint) (sotnApi.AlucardApi.Int - baseRemoved);
			newLck = (uint) (sotnApi.AlucardApi.Lck - baseRemoved);

			newStr = newStr < adjustedMinStr ? adjustedMinStr: newStr;
			newCon = newCon < adjustedMinCon ? adjustedMinCon: newCon;
			newInt = newInt < adjustedMinInt ? adjustedMinInt: newInt;
			newLck = newLck < adjustedMinLck ? adjustedMinLck: newLck;

			double percentageHP = baseRemoved / 100;
			double percentageMP = baseRemoved / 100;
			double percentageHearts = baseRemoved / 100;

			newHP = (uint) Math.Max(minHP, Math.Round(adjustedMaxHP - (adjustedMaxHP * percentageHP) - 10 - baseRemoved)) + adjustedHP;
			newMP = (uint) Math.Max(minMP, Math.Round(currentMaxMP - (currentMaxMP * percentageMP) - 5 - baseRemoved));
			newHearts = (uint) Math.Max(minHearts, Math.Round(currentMaxHearts - (currentMaxHearts * percentageHearts) - 5 - baseRemoved));

			newHP = adjustedMinHP > newHP ? adjustedMinHP : newHP;
			newMP = adjustedMinMP > newMP ? adjustedMinMP : newMP;
			newHearts = adjustedMinHearts > newHearts ? adjustedMinHearts : newHearts;

			newLevel = 1;
			if (sotnApi.AlucardApi.Level + 1 > (baseRemoved * statsToReduce))
			{
				newLevel = (uint) (sotnApi.AlucardApi.Level + 1 - baseRemoved);
			}

			newExperience = 0;
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
		}

		public void Hex(string user = "Mayhem", bool allowMulti = false, bool isSuper = false)
		{
			List<int> effectNumbers = new List<int>() { 1, 2, 3, 4 };
			bool meterFull = MayhemMeterFull();

			int max = effectNumbers.Count;
			int min = 0;
			int roll = 0;

			if (sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp / 2)
			{
				effectNumbers.RemoveAll(item => item == 1);
			}

			if (axeArmorActive)
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
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.Hex, ChangesStats = true, Invoker = new MethodInvoker(() => Hex(user, allowMulti, isSuper)) });
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

			switch (roll) {
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
					sotnApi.GameApi.SetMovementSpeedDirection(true);
					name += "(Confused)";
					hexConfusedActive = true;
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
				if (alucardColor != 33056 || color == 123456)
				{
					playerPaletteCheat.PokeValue(richterColor);
					playerPaletteCheat.Enable();
					int enemyRichterColor = richterColor;
					if (enemyRichterColor < 33000)
					{
						color = Constants.Khaos.richterColors[rng.Next(0, Constants.Khaos.enemyRichterColors.Length)];
						enemyRichterColor = color;
					}
					if (enemyRichterColor > 33000 && enemyRichterColor < 33100)
					{
						//Apply offset for some colors.
						enemyRichterColor += 256;
					}
					enemyRichterPaletteCheat.PokeValue(enemyRichterColor);
					Console.WriteLine($"Richter Color:{richterColor}, Enemy Richter Color:{enemyRichterColor}");
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
					else if (Equipment.Items[(int) (sotnApi.AlucardApi.Cloak + Equipment.HandCount + 1)] == "Joseph's cloak") //Joseph's Cloak
					{
						playerPaletteCheat.PokeValue(color);
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


		private void AxeArmorTakeRelics()
		{
			if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SoulOfBat);
				soulOfBatTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.SoulOfWolf);
				soulOfWolfTaken = true;
			}
			if (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
			{
				sotnApi.AlucardApi.TakeRelic(Relic.FormOfMist);
				formOfMistTaken = true;
			}
		}

		private void AxeArmorTakeResources()
		{
			uint maxHearts = sotnApi.AlucardApi.MaxtHearts;
			uint hearts = sotnApi.AlucardApi.CurrentHearts;
			uint maxMp = sotnApi.AlucardApi.MaxtMp;
			uint currentMp = sotnApi.AlucardApi.CurrentMp;

			hasAxeArmorStoredResources = true;
			storedLeftHand = sotnApi.AlucardApi.LeftHand;
			storedRightHand = sotnApi.AlucardApi.RightHand;
			storedAxeArmorMaxHearts = maxHearts;
			storedAxeArmorHearts = hearts;
			storedAxeArmorMaxMP = maxMp;
			storedAxeArmorMP = currentMp;

			sotnApi.AlucardApi.CurrentHearts = 0;
			sotnApi.AlucardApi.CurrentMp = 0;
			sotnApi.AlucardApi.RightHand = 0;
			sotnApi.AlucardApi.LeftHand = 0;
		}
		private void AxeArmorReturnResources()
		{
			sotnApi.AlucardApi.CurrentHearts = storedAxeArmorHearts;
			if (sotnApi.AlucardApi.MaxtMp < storedAxeArmorMP)
			{
				axeArmorDelayedMPRegenDuration += (int) (storedAxeArmorMP - sotnApi.AlucardApi.MaxtMp);
				sotnApi.AlucardApi.CurrentMp = storedAxeArmorMP;
			}

			sotnApi.AlucardApi.CurrentMp = storedAxeArmorMP;

			sotnApi.AlucardApi.RightHand = storedRightHand;
			sotnApi.AlucardApi.LeftHand = storedLeftHand;

			storedAxeArmorMaxMP = 0;
			storedAxeArmorMP = 0;
			storedAxeArmorHearts = 0;
			storedAxeArmorMaxHearts = 0;
			storedLeftHand = 0;
			storedRightHand = 0;
			hasAxeArmorStoredResources = false;
			if (useItemCooldown < 1)
			{
				useItemCooldown = useItemCooldownBase;
			}
		}

		private void AxeArmorReturnRelics()
		{
			if (soulOfBatTaken)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfBat, true);
				soulOfBatTaken = false;
			}
			if (soulOfWolfTaken)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.SoulOfWolf, true);
				soulOfWolfTaken = true;
			}
			if (formOfMistTaken)
			{
				sotnApi.AlucardApi.GrantRelic(Relic.FormOfMist, true);
				formOfMistTaken = true;
			}
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
					hexConfusedActive = false;
					sotnApi.GameApi.SetMovementSpeedDirection(false);
					//hexWeaponsActive = false;
					//HexReturnWeapons();
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
		private void HexReturnHPMPH()
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
			//if (hexWeaponsPaused || (sotnApi.AlucardApi.Subweapon == 0 && !heartsLocked && !subWeaponsLocked))
			if (sotnApi.AlucardApi.Subweapon == 0 && !heartsLocked && !subWeaponsLocked)
			{
				if (subWeaponTaken != 0)
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
			if (getJuggledActive) {
				queuedActions.Add(new QueuedAction { Name = "GetJuggled", LocksInvincibility = true, Invoker = new MethodInvoker(() => GetJuggled(user)) });
				return;
			}

			getJuggledActive = true;
			invincibilityCheat.PokeValue(0);
			invincibilityCheat.Enable();
			invincibilityLocked = true;

			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.GetJuggled;

			if (meterFull)
			{
				name = "Super " + name;
				SpendMayhemMeter();
			}
			else
			{
				defensePotionCheat.PokeValue(1);
				defensePotionCheat.Enable();
			}

			System.TimeSpan newDuration = CurseDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.GetJuggled).FirstOrDefault().Duration);
			getJuggledTimer.Interval = (int) newDuration.TotalMilliseconds;
			getJuggledTimer.Start();

			notificationService.AddTimer(new Services.Models.ActionTimer
			{
				Name = name,
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
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || sotnApi.AlucardApi.CurrentHp < (5 + sotnApi.AlucardApi.Level) || sotnApi.GameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone) || IsInRoomList(Constants.Khaos.ClockRoom))
			{
				return;
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
				string name = KhaosActionNames.Ambush;
				if (superAmbush)
				{
					name = "Super " + name;
				}

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
					float hpMultiplier = statMultiplier * (1.0F * (toolConfig.Khaos.AmbushHPModifier * .5F));
					float dmgMultiplier = statMultiplier * (1.0F * (toolConfig.Khaos.AmbushDMGModifier * .5F));

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

						List<int> effectNumbers = new List<int>() { 1, 2, 3, 4 };
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
						if (initMax == max)
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

					ushort calcHP = (ushort) ((ambushEnemy.Hp + (2.5 * toolConfig.Khaos.AmbushHPModifier) + (2.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier)) * hpMultiplier);

					if (calcHP > 0)
					{
						ambushEnemy.Hp = calcHP;
					}

					if (boostDmg)
					{
						//Original:
						//statMultiplier += .125F;
						dmgMultiplier += (.0625F * toolConfig.Khaos.AmbushDMGModifier);
					}
					//Original
					//ambushEnemy.Damage = (ushort) ((ambushEnemy.Damage + (1 + vladRelicsObtained)) * statMultiplier);

					ushort calcDamage = (ushort) ((ambushEnemy.Damage + (.5 * toolConfig.Khaos.AmbushDMGModifier) + (.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier)) * dmgMultiplier);

					if (calcDamage > 0)
					{
						ambushEnemy.Damage = calcDamage;
					}

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
			if (toolConfig.Khaos.RespawnRichter && !alucardSecondCastle && (!(alucardMapX > 30 && alucardMapX < 38) || !(alucardMapY > 5 && alucardMapY < 13)))
			{
				RespawnRichter();
			}
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
				healthFlat = (short) (25 * (toolConfig.Khaos.CloneBossHPModifier));
				healthFlat += (short) (5 * toolConfig.Khaos.CurseModifier);
				damageFlat += (uint) (.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier);
				damageFlat += (uint) (toolConfig.Khaos.CloneBossDMGModifier);


				LiveEntity boss = sotnApi.EntityApi.GetLiveEntity(enemy);
				bossCopy = new Entity(sotnApi.EntityApi.GetEntity(enemy));
				string name = Constants.Khaos.EnduranceRomhackBosses.Where(e => e.AiId == bossCopy.AiId).FirstOrDefault().Name;

				Console.WriteLine($"Tough Boss clone boss name: {name}, Boss HP{boss.Hp}, Boss Damage{boss.Damage}, healthMultiplier {healthMultiplier}, healthFlat{healthFlat}, damageFlat{damageFlat}");

				bool right = rng.Next(0, 2) > 0;
				bossCopy.Xpos = right ? (ushort) (bossCopy.Xpos + rng.Next(40, 80)) : (ushort) (bossCopy.Xpos + rng.Next(-80, -40));
				bossCopy.Palette = (ushort) (bossCopy.Palette + rng.Next(1, 10));


				newBossHP = (ushort) (healthMultiplier * (bossCopy.Hp + healthFlat));

				if (superToughBosses)
				{
					newBossHP = (ushort) ((newBossHP) * (toolConfig.Khaos.SuperBossHPModifier * .5F));
					damageFlat = (uint) ((damageFlat) * (toolConfig.Khaos.SuperBossDMGModifier * .5F));
				}

				boss.Damage += damageFlat;
				bossCopy.Damage += (ushort) damageFlat;

				if (newBossHP >= Int16.MaxValue)
				{
					//32767 Int16 MaxValue
					boss.Hp = (ushort) (Int16.MaxValue - 2767);
					bossCopy.Hp = (ushort) (Int16.MaxValue - 2767);
				}
				else if (newBossHP > 0)
				{

					boss.Hp = newBossHP;
					bossCopy.Hp = newBossHP;
				}
				else
				{
					newBossHP = (ushort) (boss.Hp);
					bossCopy.Hp = newBossHP;
				}

				sotnApi.EntityApi.SpawnEntity(bossCopy);
				Console.WriteLine($"{KhaosActionNames.ToughBosses} boss found name: {name} hp: {bossCopy.Hp}, damage: {bossCopy.Damage}, sprite: {bossCopy.AiId}, health multiplier: {healthMultiplier}, health flat {healthFlat}, damage flat {damageFlat}");

				if (superToughBosses)
				{
					--superToughBossesCount;
					if (superToughBossesCount < 1)
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
					if (newBossHP > Int16.MaxValue)
					{
						//32767 Int16 MaxValue
						boss.Hp = (ushort) (Int16.MaxValue - 767);
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
				newStr = (uint) ((baseStr) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + strGiven;
				newCon = (uint) ((baseCon) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + conGiven;
				newInt = (uint) ((baseInt) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + intGiven;
				newLck = (uint) ((baseLck) * toolConfig.Khaos.StatsDownFactor * enhancedFactor) + lckGiven;

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
			if (newLevel > 1 && sotnApi.AlucardApi.Str <= adjustedMinStr && sotnApi.AlucardApi.Con <= adjustedMinCon && sotnApi.AlucardApi.Int <= adjustedMinInt && sotnApi.AlucardApi.Lck <= adjustedMinLck)
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
			bool meterFull = MayhemMeterFull();

			if (meterFull)
			{
				SpendMayhemMeter();
				RemoveItems(true, false, true);
				notificationService.AddMessage($"{user} used Super {KhaosActionNames.Confiscate}");
			}
			else
			{
				RemoveItems(false, false, false);
				notificationService.AddMessage($"{user} used {KhaosActionNames.Confiscate}");
			}
			Alert(KhaosActionNames.Confiscate);
		}
		private void RemoveItems(bool clearInventory = false, bool staticRemoval = false, bool isSuper = false)
		{
			bool clearSubWeapon = false;
			bool clearRightHand = false;
			bool clearLeftHand = false;
			bool clearHelm = false;
			bool clearArmor = false;
			bool clearCloak = false;
			bool clearAccessory1 = false;
			bool clearAccessory2 = false;

			bool clearHolySymbol = false;
			bool clearLeapStone = false;
			bool clearSoulOfWolf = false;
			bool clearSkillOfWolf = false;
			bool clearPowerOfWolf = false;
			bool clearFireOfBat = false;
			bool clearForceOfEcho = false;
			bool clearGasCloud = false;
			bool clearBatCard = false;
			bool clearFaerieCard = false;
			bool clearGhostCard = false;
			bool clearSwordCard = false;

			float goldPercentage = 0;
			int clearedSlots = 0;
			int clearedRelics = 0;

			if (staticRemoval)
			{
				clearedSlots = 1;
				goldPercentage = .5f;
			}
			else {
				clearSubWeapon = true;
				int effectPower = toolConfig.Khaos.CurseModifier - 1 > 0 ? toolConfig.Khaos.CurseModifier - 1 : 0;
				effectPower += vladRelicsObtained;

				if (isSuper)
				{
					if(vladRelicsObtained > 0)
					{
						effectPower += (vladRelicsObtained);
					}
					else
					{
						effectPower += 1;
					}
				}

				switch (effectPower)
				{
					case 0:
						goldPercentage = 0.70f;
						clearedSlots = 2;
						clearedRelics = 1;
						break;
					case 1:
						goldPercentage = 0.65f;
						clearedSlots = 3;
						clearedRelics = 1;
						break;
					case 2:
						goldPercentage = 0.60f;
						clearedSlots = 3;
						clearedRelics = 2;
						break;
					case 3:
						goldPercentage = 0.55f;
						clearedSlots = 4;
						clearedRelics = 2;
						break;
					case 4:
						goldPercentage = 0.50f;
						clearedSlots = 4;
						clearedRelics = 3;
						break;
					case 5:
						goldPercentage = 0.45f;
						clearedSlots = 5;
						clearedRelics = 3;
						break;
					case 6:
						goldPercentage = 0.40f;
						clearedSlots = 5;
						clearedRelics = 4;
						break;
					case 7:
						goldPercentage = 0.35f;
						clearedSlots = 6;
						clearedRelics = 4;
						break;
					case 8:
						goldPercentage = 0.30f;
						clearedSlots = 6;
						clearedRelics = 5;
						break;
					case 9:
						goldPercentage = 0.25f;
						clearedSlots = 7;
						clearedRelics = 5;
						break;
					case 10:
						goldPercentage = 0.20f;
						clearedSlots = 7;
						clearedRelics = 6;
						break;
					case 11:
						goldPercentage = 0.15f;
						clearedSlots = 7;
						clearedRelics = 6;
						break;
					case 12:
						goldPercentage = 0.10f;
						clearedSlots = 7;
						clearedRelics = 7;
						break;
					case 13:
						goldPercentage = 0.05f;
						clearedSlots = 7;
						clearedRelics = 7;
						break;
					default:
						goldPercentage = 0f;
						clearedSlots = 7;
						clearedRelics = 8;
						break;
				}
			}

			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses");
			bool hasSpikeBreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker");
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring");
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring");
			bool equippedHolyGlasses = Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses";
			//bool equippedAxeArmor = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Axe Lord armor";
			bool equippedSpikeBreaker = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker";
			bool equippedGoldRing1 = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedGoldRing2 = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring";
			bool equippedSilverRing1 = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring";
			bool equippedSilverRing2 = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring";

			
			if (clearedSlots > 0)
			{
				List<int> effectNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7};
				if (sotnApi.AlucardApi.RightHand == 0)
				{
					effectNumbers.RemoveAll(item => item == 1);
				}
				if (sotnApi.AlucardApi.LeftHand == 0)
				{
					effectNumbers.RemoveAll(item => item == 2);
				}
				if (equippedHolyGlasses || sotnApi.AlucardApi.Helm == Equipment.HelmStart)
				{
					effectNumbers.RemoveAll(item => item == 3);
				}
				if (equippedSpikeBreaker || sotnApi.AlucardApi.Armor == 0 || (axeArmorActive && toolConfig.Khaos.PermaAxeArmor))
				{
					effectNumbers.RemoveAll(item => item == 4);
				}
				if (sotnApi.AlucardApi.Cloak == Equipment.CloakStart)
				{
					effectNumbers.RemoveAll(item => item == 5);
				}
				if (equippedGoldRing1 || equippedSilverRing1 || sotnApi.AlucardApi.Accessory1 == Equipment.AccessoryStart)
				{
					effectNumbers.RemoveAll(item => item == 6);
					
				}
				if (equippedGoldRing2 || equippedSilverRing2 || sotnApi.AlucardApi.Accessory2 == Equipment.AccessoryStart)
				{
					effectNumbers.RemoveAll(item => item == 7);
				}

				int max = effectNumbers.Count;
				int min = 0;

				if (max == 0)
				{
					goldPercentage = 0;
				}
				else
				{
					if (clearedSlots > max - 1)
					{
						clearedSlots = max - 1;
					}

					int[] slots = new int[clearedSlots + 1];
					int slotsIndex = 0;
					for (int i = 0; i <= clearedSlots; i++)
					{
						int result = effectNumbers[rng.Next(min, max)];
						while (slots.Contains(result))
						{
							result = effectNumbers[rng.Next(min, max)];
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
			}
			if (clearedRelics > 0)
			{
				List<int> effectNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
				if (!hasHolySymbol)
				{
					effectNumbers.RemoveAll(item => item == 1);
				}
				if (!hasLeapStone || (!hasSoulOfBat && !hasFormOfMist) || (hasFormOfMist && !hasPowerOfMist))
				{
					effectNumbers.RemoveAll(item => item == 2);
				}
				if (!hasSoulOfWolf || (!hasSoulOfBat && !hasFormOfMist))
				{
					effectNumbers.RemoveAll(item => item == 3);
				}
				if (!hasSkillOfWolf)
				{
					effectNumbers.RemoveAll(item => item == 4);
				}
				if (!hasPowerOfWolf)
				{
					effectNumbers.RemoveAll(item => item == 5);
				}
				if (!hasFireOfBat)
				{
					effectNumbers.RemoveAll(item => item == 6);
				}
				if (!hasForceOfEcho)
				{
					effectNumbers.RemoveAll(item => item == 7);
				}
				if (!hasGasCloud)
				{
					effectNumbers.RemoveAll(item => item == 8);
				}
				if (!sotnApi.AlucardApi.HasRelic(Relic.BatCard))
				{//bat card
					effectNumbers.RemoveAll(item => item == 9);
				}
				if (!sotnApi.AlucardApi.HasRelic(Relic.FaerieCard))
				{//faerie card
					effectNumbers.RemoveAll(item => item == 10);
				}
				if (!sotnApi.AlucardApi.HasRelic(Relic.GhostCard))
				{//ghost card
					effectNumbers.RemoveAll(item => item == 11);
				}
				if (!sotnApi.AlucardApi.HasRelic(Relic.SwordCard))
				{//sword card
					effectNumbers.RemoveAll(item => item == 12);
				}

				int max = effectNumbers.Count;
				int min = 0;

				if (max == 0)
				{
					goldPercentage = 0;
				}
				else
				{
					if (clearedSlots > max - 1)
					{
						clearedSlots = max - 1;
					}

					int[] slots = new int[clearedSlots + 1];
					int slotsIndex = 0;
					for (int i = 0; i <= clearedSlots; i++)
					{
						int result = effectNumbers[rng.Next(min, max)];
						while (slots.Contains(result))
						{
							result = effectNumbers[rng.Next(min, max)];
						}
						slots[slotsIndex] = result;
						slotsIndex++;
					}

					for (int i = 0; i < slots.Length; i++)
					{
						switch (slots[i])
						{
							case 1:
								clearHolySymbol = true;
								sotnApi.AlucardApi.TakeRelic(Relic.HolySymbol);
								break;
							case 2:
								clearLeapStone = true;
								sotnApi.AlucardApi.TakeRelic(Relic.LeapStone);
								break;
							case 3:
								clearSoulOfWolf = true;
								sotnApi.AlucardApi.TakeRelic(Relic.SoulOfWolf);
								break;
							case 4:
								clearSkillOfWolf = true;
								sotnApi.AlucardApi.TakeRelic(Relic.SkillOfWolf);
								break;
							case 5:
								clearPowerOfWolf = true;
								sotnApi.AlucardApi.TakeRelic(Relic.PowerOfWolf);
								break;
							case 6:
								clearFireOfBat = true;
								sotnApi.AlucardApi.TakeRelic(Relic.FireOfBat);
								break;
							case 7:
								clearForceOfEcho = true;
								sotnApi.AlucardApi.TakeRelic(Relic.ForceOfEcho);
								break;
							case 8:
								clearGasCloud = true;
								sotnApi.AlucardApi.TakeRelic(Relic.GasCloud);
								break;
							case 9:
								clearBatCard = true;
								sotnApi.AlucardApi.TakeRelic(Relic.BatCard);
								break;
							case 10:
								clearFaerieCard = true;
								sotnApi.AlucardApi.TakeRelic(Relic.FaerieCard);
								break;
							case 11:
								clearGhostCard = true;
								sotnApi.AlucardApi.TakeRelic(Relic.GhostCard);
								break;
							case 12:
								clearSwordCard = true;
								sotnApi.AlucardApi.TakeRelic(Relic.SwordCard);
								break;
							default:
								break;
						}
					}

					Console.WriteLine($"Confiscate: " +
						$"clearHolySymbol={clearHolySymbol}," +
						$"clearLeapStone={clearLeapStone}," +
						$"clearSoulOfWolf={clearSoulOfWolf}," +
						$"clearSkillOfWolf={clearSkillOfWolf}," +
						$"clearPowerOfWolf={clearPowerOfWolf}," +
						$"clearFireOfBat={clearFireOfBat}," +
						$"clearForceOfEcho={clearForceOfEcho}," +
						$"clearGasCloud={clearGasCloud}," +
						$"clearBatCard={clearBatCard}," +
						$"clearFaerieCard={clearFaerieCard}," +
						$"clearGhostCard={clearGhostCard}," +
						$"clearSwordCard={clearSwordCard}," +
						$"");
				}
			}


			sotnApi.AlucardApi.Gold = goldPercentage == 0 ? 0 : (uint) Math.Round(sotnApi.AlucardApi.Gold * goldPercentage);

			if (clearInventory)
			{
				sotnApi.AlucardApi.ClearInventory();
			}
			if (clearSubWeapon)
			{
				sotnApi.AlucardApi.Subweapon = Subweapon.Empty;
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

			if (isSuper)
			{
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
				If Soul of Bat, no Force of Echo: Force of Echo
			*/

			isValidRelic = false;
			relic = Relic.FireOfBat;

			if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor && toolConfig.Khaos.AxeArmorTips)
			{
				//Do nothing
			}
			else
			{
				hasSoulOfBat = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat) || soulOfBatTaken) ? true : false;
				hasEchoOfBat = (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat) || echoOfBatTaken) ? true : false;
				hasForceOfEcho = (sotnApi.AlucardApi.HasRelic(Relic.ForceOfEcho) || forceOfEchoTaken) ? true : false;
				hasFireOfBat = (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat) || fireOfBatTaken) ? true : false;
			}

			if (hasSoulOfBat)
			{
				if (!hasEchoOfBat)
				{
					relic = Relic.EchoOfBat;
					isValidRelic = true;
				}
				else if (!hasFireOfBat)
				{
					relic = Relic.FireOfBat;
					isValidRelic = true;
				}
				else if (!hasForceOfEcho)
				{
					relic = Relic.ForceOfEcho;
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

			if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor && toolConfig.Khaos.AxeArmorTips)
			{
				//Do nothing
			}
			else
			{
				hasSoulOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) || soulOfWolfTaken) ? true : false;
				hasPowerOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf) || powerOfWolfTaken) ? true : false;
				hasSkillOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf) || skillOfWolfTaken) ? true : false;
				hasFormOfMist = sotnApi.AlucardApi.HasRelic(Relic.FormOfMist) ? true : false;
				hasGasCloud = (sotnApi.AlucardApi.HasRelic(Relic.GasCloud) || gasCloudTaken) ? true : false;
			}

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
			bool hasDemonCard = (sotnApi.AlucardApi.HasRelic(Relic.DemonCard) || demonCardTaken) ? true : false;
			bool hasNoseDevilCard = (sotnApi.AlucardApi.HasRelic(Relic.NoseDevilCard) || noseDevilCardTaken) ? true : false;

			if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor && toolConfig.Khaos.AxeArmorTips)
			{
				//Do nothing for now.
			}
			else
			{
				hasSoulOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) || soulOfWolfTaken) ? true : false;
				hasFormOfMist = sotnApi.AlucardApi.HasRelic(Relic.FormOfMist) ? true : false;
				hasPowerOfMist = (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist) || powerOfMistTaken) ? true : false;
				hasSoulOfBat = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat) || soulOfBatTaken) ? true : false;
				hasGravityBoots = sotnApi.AlucardApi.HasRelic(Relic.GravityBoots) ? true : false;
				hasLeapStone = sotnApi.AlucardApi.HasRelic(Relic.LeapStone) ? true : false;
				hasHeartOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad) || heartOfVladTaken) ? true : false;
				hasToothOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.ToothOfVlad) || toothOfVladTaken) ? true : false;
				hasRibOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.RibOfVlad) || ribOfVladTaken) ? true : false;
				hasEyeOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad) || eyeOfVladTaken) ? true : false;
				hasRingOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.RingOfVlad) || ringOfVladTaken) ? true : false;
			}


			bool hasHolyGlasses = sotnApi.AlucardApi.HasItemInInventory("Holy glasses") || Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses" ? true : false;
			bool hasSpikebreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker") || Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Spike Breaker" ? true : false;
			bool hasGoldRing = sotnApi.AlucardApi.HasItemInInventory("Gold Ring") || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Gold Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Gold Ring" ? true : false;
			bool hasSilverRing = sotnApi.AlucardApi.HasItemInInventory("Silver Ring") || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Silver Ring" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Silver Ring" ? true : false;


			isValidRelic = false;
			relic = Relic.FireOfBat;

			if (hasJewelOfOpen && !hasMermanStatue)
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

			bool isSuper = false;
			bool meterFull = MayhemMeterFull();

			int baseGainMulitiplier = 1;

			if (meterFull && !timeStopActive)
			{
				SpendMayhemMeter();
				baseGainMulitiplier = 2;
				isSuper = true;
			}

			uint baseGain = (uint) ((5 + sotnApi.AlucardApi.Level) * baseGainMulitiplier);
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

			if (isSuper)
			{
				RandomizeMajorPotion(user);
			}
			else if (hasHolyGlasses)
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
				RollAndRewardItems(user, forcedRoll, isSuper);
				RollAndRewardItems(user, 4, isSuper);
			}

			RollAndRewardItems(user, forcedRoll, isSuper);
			
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
						Alert(KhaosActionNames.MinorItems);
						break;
					case 2:
						item = toolConfig.Khaos.moderateItemRewards[rng.Next(0, toolConfig.Khaos.moderateItemRewards.Length)];
						name = KhaosActionNames.ModerateItems;
						min += 4;
						max += 9;
						Alert(KhaosActionNames.ModerateItems);
						break;
					case 3:
						item = toolConfig.Khaos.majorItemRewards[rng.Next(0, toolConfig.Khaos.majorItemRewards.Length)];
						name = KhaosActionNames.MajorItems;
						min += 2;
						max += 5;
						Alert(KhaosActionNames.MajorItems);
						break;
					case 4:
						item = toolConfig.Khaos.superItemRewards[rng.Next(0, toolConfig.Khaos.superItemRewards.Length)];
						name = KhaosActionNames.SuperItems;
						min += 1;
						max += 1;
						Alert(KhaosActionNames.MajorItems);
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
				sotnApi.AlucardApi.CurrentHp += (uint) ((toolConfig.Khaos.RegenGainPerSecond + superGain) * regenMultiplier);
			}
			if (!mpLocked && sotnApi.AlucardApi.CurrentMp < sotnApi.AlucardApi.MaxtMp)
			{
				sotnApi.AlucardApi.CurrentMp += (uint) ((toolConfig.Khaos.RegenGainPerSecond + superGain) * regenMultiplier);
			}
			if (!heartsLocked && sotnApi.AlucardApi.CurrentHearts < (2 * sotnApi.AlucardApi.MaxtHearts))
			{
				sotnApi.AlucardApi.CurrentHearts += (uint) ((toolConfig.Khaos.RegenGainPerSecond + superGain) * regenMultiplier);
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
				queuedActions.Add(new QueuedAction { Name = KhaosActionNames.TimeStop, Type = ActionType.Blessing, Invoker = new MethodInvoker(() => TimeStop(user)) });
				return;
			}

			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.TimeStop;

			if (!invincibilityLocked && meterFull)
			{
				name = "Super " + name;
				superTimeStop = true;
				invincibilityLocked = true;
				invincibilityCheat.PokeValue(1);
				invincibilityCheat.Enable();
				SpendMayhemMeter();
			}
			

			sotnApi.AlucardApi.ActivateStopwatch();
			timeStopZone = sotnApi.GameApi.Zone2;
			timeStopActive = true;

			if (!heartsOnlyActive && !rushDownActive)
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
				Name = name,
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
			if(superTimeStop)
			{
				superTimeStop = false;
				invincibilityCheat.Disable();
				invincibilityLocked = false;
			}
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
			List<int> effectNumbers = new List<int>() { 1, 2, 3, 4 };
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

			//buffActive = true;
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

			float currentHpPercentage = (float) ((1.00 * (sotnApi.AlucardApi.CurrentHp)) / sotnApi.AlucardApi.MaxtHp);

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
			if (toolConfig.Khaos.KindAndFair)
			{
				tempCon *= 3;
			}

			addCONGiven(tempCon);

			sotnApi.AlucardApi.CurrentHp = (uint) (sotnApi.AlucardApi.MaxtHp * currentHpPercentage);

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

			if (message == "")
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

			if (isStatsPaused)
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

			if (buffConHpHolyGlassesCount >= 1)
			{
				buffConHPHolyGlasses = true;
				--buffConHpHolyGlassesCount;
			}
			else if (buffConHpHolyGlassesCount < 1)
			{
				buffConHPHolyGlasses = false;
			}

			if (superBuffConHPCount > 0)
			{
				conGiven = CalculateTempStatGiven(Constants.Khaos.BuffCon, true, buffConHPHolyGlasses);
				--superBuffConHPCount;
				if (superBuffConHPCount < 1)
				{
					superBuffConHP = false;
				}
			}
			else
			{
				conGiven = CalculateTempStatGiven(Constants.Khaos.BuffCon, false, buffConHPHolyGlasses);
			}
			if (toolConfig.Khaos.KindAndFair)
			{
				conGiven *= 3;
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
			if (mpLocked)
			{
				queuedActions.Add(new QueuedAction { Name = "SpellCaster", Type = ActionType.Blessing, LocksMana = true, Invoker = new MethodInvoker(() => SpellCaster(user)) });
				return;
			}

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

				uint newArmor = (uint) (Equipment.Items.IndexOf("Mojo mail") - Equipment.HandCount - 1);
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

			if (message == "")
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

			if (message == "")
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
			superBuffLckNeutral = false;
			buffLckNeutralActive = false;
		}

		public void Summoner(string user = "Mayhem")
		{
			spawnActive = true;
			summonerTriggerRoomX = sotnApi.GameApi.RoomX;
			summonerTriggerRoomY = sotnApi.GameApi.RoomY;

			bool meterFull = MayhemMeterFull();
			string name = KhaosActionNames.Summoner;

			if (meterFull)
			{
				name = "Super " + name;
				superSummoner = true;
				++superSummonerCount;
				SpendMayhemMeter();
			}

			summonerTimer.Start();
			summonerSpawnTimer.Start();
			summonerEnemies.Clear();

			string message = $"{user} made you a {name}";
			notificationService.AddMessage(message);
			Alert(KhaosActionNames.Summoner);
		}
		private void SummonerOff(Object sender, EventArgs e)
		{
			spawnActive = false;
			--superSummonerCount;
			if (superSummonerCount < 1)
			{
				superSummoner = false;
			}

			summonerEnemies.RemoveRange(0, summonerEnemies.Count);
			summonerTimer.Interval = 5 * (60 * 1000);
			summonerTimer.Stop();
			summonerSpawnTimer.Stop();
		}

	private void SummonerSpawn(Object sender, EventArgs e)
		{
			if (!sotnApi.GameApi.InAlucardMode() || !sotnApi.GameApi.CanMenu() || IsInRoomList(Constants.Khaos.SummonerBanZone) || sotnApi.GameApi.CanSave() || IsInRoomList(Constants.Khaos.RichterRooms) || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.LesserDemonZone) || IsInRoomList(Constants.Khaos.ClockRoom))
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
				string name = KhaosActionNames.Summoner;
				if (superSummoner)
				{
					name = "Super " + name;
				}

				if (summonerTimer.Interval >= 5 * (60 * 1000))
				{

					summonerTimer.Stop();
					System.TimeSpan newDuration = BlessingDurationGain(toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault().Duration);
					summonerTimer.Interval = (int) newDuration.TotalMilliseconds;
					notificationService.AddTimer(new Services.Models.ActionTimer
					{
						Name = name,
						Type = Enums.ActionType.Blessing,
						Duration = newDuration
					});
					summonerTimer.Start();

				}

				int min;
				int max;

				bool facing = sotnApi.AlucardApi.FacingLeft;
				
				if(sotnApi.AlucardApi.State == 0 && sotnApi.AlucardApi.Action == 3)
				{
					facing = !facing;
				}

				if (facing)
				{
					
					if (sotnApi.AlucardApi.ScreenX > 40)
					{
						max = (int) (sotnApi.AlucardApi.ScreenX - 25);
					}
					else
					{
						max = 15;
					}
					summonerEnemies[enemyIndex].Xpos = (ushort) rng.Next(10, max);
				}
				else
				{
					
					if (sotnApi.AlucardApi.ScreenX < 220)
					{
						min = (int)(sotnApi.AlucardApi.ScreenX + 25);
					}
					else
					{
						min = 240;
					}
					summonerEnemies[enemyIndex].Xpos = (ushort) rng.Next(min, 245);

				}
				
				if(sotnApi.AlucardApi.ScreenY < 220)
				{
					max = (int)(sotnApi.AlucardApi.ScreenY + 25);
				}
				else
				{
					max = 245;
				}
				if (sotnApi.AlucardApi.ScreenY > 95)
				{
					min = (int) (sotnApi.AlucardApi.ScreenY - 85);
				}
				else 
				{
					min = 10;
				}

				summonerEnemies[enemyIndex].Ypos = (ushort) rng.Next(min, max);
				summonerEnemies[enemyIndex].Palette -= (ushort) rng.Next(1, 10);

				if (superSummoner)
				{
					summonerEnemies[enemyIndex].Hp += (ushort) (summonerEnemies[enemyIndex].Hp);
					summonerEnemies[enemyIndex].Damage += (ushort) ((1 + summonerEnemies[enemyIndex].Damage) * .5);
				}

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

					//Summoner Ban List
					if (summonerEnemy.AiId == 48728 ||
						summonerEnemy.AiId == 14076 ||
						summonerEnemy.AiId == 11852 ||
						summonerEnemy.AiId == 19232 ||
						summonerEnemy.AiId == 19008 ||
						summonerEnemy.AiId == 36064 ||
						summonerEnemy.AiId == 33188 
					)
					{	
						Console.WriteLine($"Skipped {KhaosActionNames.Summoner} ally with hp: {summonerEnemy.Hp} sprite: {summonerEnemy.AiId} damage: {summonerEnemy.Damage}");
					}
					else
					{
						summonerEnemies.Add(summonerEnemy);
						Console.WriteLine($"Added {KhaosActionNames.Summoner} ally with hp: {summonerEnemy.Hp} sprite: {summonerEnemy.AiId} damage: {summonerEnemy.Damage}");
						return true;
					}
				}
			}

			return false;
		}

		public void GiveBoon(string user = "Mayhem", bool isStatsOnly = false, uint forcedCategory = 0)
		{
			Relic relic;
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
					categoryRoll = (uint) (rng.Next(1, 4));
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
							Alert(KhaosActionNames.MinorBoon);
						}
						else
						{
							messageEnd += $"{KhaosActionNames.MinorStats}";
							Alert(KhaosActionNames.MinorStats);
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
							Alert(KhaosActionNames.ModerateBoon);
						}
						else
						{
							messageEnd += $"{KhaosActionNames.ModerateStats}";
							Alert(KhaosActionNames.ModerateStats);
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
							Alert(KhaosActionNames.MajorBoon);
						}
						else
						{
							messageEnd += $"{KhaosActionNames.MajorStats}";
							Alert(KhaosActionNames.MajorStats);
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
			}
		}
		#endregion

		public void GiveProgression(string user = "Mayhem", bool isStatsOnly = false)
		{
			Relic relic;
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

			if(categoryRoll > 0)
			{
				Alert(KhaosActionNames.ProgressionStats);
			}
			else
			{
				Alert(KhaosActionNames.ProgressionRelic);
			}

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
				//Alert(KhaosActionNames.Boon);
			}
		}
		#endregion

		public void AxeArmorInputs()
		{
			if (sotnApi.GameApi.InAlucardMode())
			{
				UpdateAxeArmor();
			}
		}

		public void Update()
		{
			if (!sotnApi.GameApi.InAlucardMode())
			{
				allowPity = true;
				accelTime.PokeValue(5);

				axeArmorWeapon.Disable();

				if (!givenStatsPaused)
				{
					takeHPGiven();
					takeSTRGiven();
					takeCONGiven();
					takeINTGiven();
					takeLCKGiven();
					givenStatsPaused = true;
				}

				if (sotnApi.GameApi.InPrologue())
				{
					if (!resetColorWhenAlucard)
					{
						UpdatePlayerColor();
						UpdateVisualEffect(true);
						resetColorWhenAlucard = true;
					}
				}

				if (toolConfig.Khaos.PermaAxeArmor)
				{
					startAxeArmor.Enable();
				}
				else
				{
					startAxeArmor.Disable();
				}
			}
			else
			{
				if (AutoMayhemOn && allowPity && sotnApi.AlucardApi.CurrentHp <= 0)
				{
					allowPity = false;
					autoMayhemPity();
				}

				if (resetColorWhenAlucard)
				{
					UpdateVisualEffect();
				}

				if (IsInRoomList(Constants.Khaos.RichterRooms))
				{
					enemyRichterPaletteCheat.Enable();
				}
				else
				{
					enemyRichterPaletteCheat.Disable();
				}

				if (HPForMPActive)
				{
					CheckMPUsage();
				}

				CheckDashInput();
				UpdateAxeArmor();
				CheckEquippedCloak();

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
					richterCutscene2.PokeValue(1);
					richterCutscene2.Enable();
					//richterCutscene1.PokeValue(1);
					//richterCutscene1.Enable();
					fightRichter1.Disable();
					fightRichter2.Disable();
					fightRichter3.Disable();
					//saveRichter.Disable();
					richterCutscene1.Disable();

					if (isAlucardColorFirstCastle)
					{
						UpdatePlayerColor();
						UpdateVisualEffect();
					}

					if (!allowSecondCastleRewind && ((sotnApi.GameApi.CanSave() && alucardMapY != 54) || IsInRoomList(Constants.Khaos.LoadingRooms)))
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
						/*
						if (hexWeaponsActive && !hexWeaponsPaused)
						{
							hexWeaponsPaused = true;
							HexReturnWeapons();
						}*/
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
						/*
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
						}*/
						if (hexRelicsActive && hexRelicsPaused)
						{
							hexRelicsPaused = false;
							HexTakeRelics();
						}
					}
				}

				if (turboModeActive)
				{
					if ((!sotnApi.AlucardApi.HasControl() && IsInRoomList(Constants.Khaos.RichterRooms)) || IsInRoomList(Constants.Khaos.MistGateRooms))
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
					if (!underwaterPaused && (sotnApi.GameApi.CanWarp() || IsInRoomList(Constants.Khaos.ClockRoom) || IsInRoomList(Constants.Khaos.MistGateRooms) || !sotnApi.AlucardApi.HasControl()))
					{
						underwaterPaused = true;
						underwaterPhysics.PokeValue(0);
						if (IsInRoomList(Constants.Khaos.ClockRoom) || IsInRoomList(Constants.Khaos.MistGateRooms) || !sotnApi.AlucardApi.HasControl())
						{
							SetSpeed((float) 1);
						}
					}
					else if (underwaterPaused && (!sotnApi.GameApi.CanWarp() && !IsInRoomList(Constants.Khaos.ClockRoom) && !IsInRoomList(Constants.Khaos.MistGateRooms) && sotnApi.AlucardApi.HasControl()))
					{
						underwaterPaused = false;
						underwaterPhysics.PokeValue(144);
						SetSpeed((float) underwaterBaseFactor * underwaterMayhemFactor);
					}
				}
				else if (IsInRoomList(Constants.Khaos.MistGateRooms))
				{
					underwaterPhysics.PokeValue(0);
				}

				bool holyGlassesCheck = false;
				if (sotnApi.AlucardApi.HasItemInInventory("Holy glasses") || Equipment.Items[(int) (sotnApi.AlucardApi.Helm + Equipment.HandCount + 1)] == "Holy glasses")
				{
					holyGlassesCheck = true;
				}

				bool CubeOfZoe = (sotnApi.AlucardApi.HasRelic(Relic.CubeOfZoe) || cubeOfZoeTaken) == true ? true : false;
				bool HolySymbol = (sotnApi.AlucardApi.HasRelic(Relic.HolySymbol) || holySymbolTaken) == true ? true : false;
				bool GravityBoots = sotnApi.AlucardApi.HasRelic(Relic.GravityBoots);
				bool LeapStone = sotnApi.AlucardApi.HasRelic(Relic.LeapStone);
				bool SoulOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) || soulOfWolfTaken) == true ? true : false;
				bool PowerOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf) || powerOfWolfTaken) == true ? true : false;
				bool SkillOfWolf = (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf) || skillOfWolfTaken) == true ? true : false;
				bool FormOfMist = (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist) || formOfMistTaken) == true ? true : false;
				bool PowerOfMist = (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist) || powerOfMistTaken) == true ? true : false;
				bool GasCloud = (sotnApi.AlucardApi.HasRelic(Relic.GasCloud) || gasCloudTaken) == true ? true : false;
				bool SoulOfBat = (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat) || soulOfBatTaken) == true ? true : false;
				bool FireOfBat = (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat) || fireOfBatTaken) == true ? true : false;
				bool EchoOfBat = (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat) || echoOfBatTaken) == true ? true : false;
				bool ForceOfEcho = (sotnApi.AlucardApi.HasRelic(Relic.ForceOfEcho) || forceOfEchoTaken) == true ? true : false;

				bool HeartOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad) || heartOfVladTaken) == true ? true : false;
				bool ToothOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.ToothOfVlad) || toothOfVladTaken) == true ? true : false;
				bool RingOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.RingOfVlad) || ringOfVladTaken) == true ? true : false;
				bool EyeOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad) || eyeOfVladTaken) == true ? true : false;
				bool RibOfVlad = (sotnApi.AlucardApi.HasRelic(Relic.RibOfVlad) || ribOfVladTaken) == true ? true : false;

				if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor && toolConfig.Khaos.AxeArmorTips)
				{
					if (hasCubeOfZoe != CubeOfZoe && CubeOfZoe != false)
					{
						hasCubeOfZoe = true;
						notificationService.AddMessage("Regen Hearts, stackable");
					}
					else if (hasCubeOfZoe != CubeOfZoe && CubeOfZoe != false)
					{
						hasCubeOfZoe = true;
						notificationService.AddMessage("Regen Hearts, stackable");
					}
					else if (hasHolySymbol != HolySymbol && HolySymbol != false)
					{
						hasHolySymbol = true;
						notificationService.AddMessage("Fireballs -4 MP Cost");
					}
					else if (hasLeapStone != LeapStone && LeapStone != false)
					{
						hasLeapStone = true;
						notificationService.AddMessage("Triple jump");
					}
					else if (hasGravityBoots != GravityBoots && GravityBoots != false)
					{
						hasGravityBoots = true;
						notificationService.AddMessage("Wolf+Up+Jump, chainable");
					}
					else if (hasSoulOfWolf != SoulOfWolf && SoulOfWolf != false)
					{
						hasSoulOfWolf = true;
						notificationService.AddMessage("Wolf+Attack upgraded");
					}
					else if (hasPowerOfWolf != PowerOfWolf && PowerOfWolf != false)
					{
						hasPowerOfWolf = true;
						notificationService.AddMessage("Wolf+Attack upgraded++");
					}
					else if (hasSkillOfWolf != SkillOfWolf && SkillOfWolf != false)
					{
						hasSkillOfWolf = true;
						notificationService.AddMessage("Wolf dmg/air control+");
					}
					else if (hasFormOfMist != FormOfMist && FormOfMist != false)
					{
						hasFormOfMist = true;
						notificationService.AddMessage("Mist+Up for Olrox/Mist Gates");
					}
					else if (hasPowerOfMist != PowerOfMist && PowerOfMist != false)
					{
						hasPowerOfMist = true;
						notificationService.AddMessage("Mist+Bat = Mist Flight");
					}
					else if (hasGasCloud != GasCloud && GasCloud != false)
					{
						hasGasCloud = true;
						notificationService.AddMessage("Mist deals Curse on contact");
					}
					else if (hasSoulOfBat != SoulOfBat && SoulOfBat != false)
					{
						hasSoulOfBat = true;
						notificationService.AddMessage("Bat & Mist+Bat Flight");
					}
					else if (hasFireOfBat != FireOfBat && FireOfBat != false)
					{
						hasFireOfBat = true;
						notificationService.AddMessage("Spell/SubWpn dmg++");
					}
					else if (hasEchoOfBat != EchoOfBat && EchoOfBat != false)
					{
						hasEchoOfBat = true;
						notificationService.AddMessage("Lights On, Spell/SubWpn dmg+");
					}
					else if (hasForceOfEcho != ForceOfEcho && ForceOfEcho != false)
					{
						hasForceOfEcho = true;
						notificationService.AddMessage("Upgrade Spell/SubWpn dmg+");
					}
					else if (hasHeartOfVlad != HeartOfVlad && HeartOfVlad != false)
					{
						hasHeartOfVlad = true;
						notificationService.AddMessage("Regen Hearts, stackable");
					}
					else if (hasToothOfVlad != ToothOfVlad && ToothOfVlad != false)
					{
						hasToothOfVlad = true;
						notificationService.AddMessage("Melee Base STR Scaling+");
					}
					else if (hasRibOfVlad != RibOfVlad && RibOfVlad != false)
					{
						hasRibOfVlad = true;
						notificationService.AddMessage("+5 Defense");
					}
					else if (hasRingOfVlad != RingOfVlad && RingOfVlad != false)
					{
						hasRingOfVlad = true;
						notificationService.AddMessage("Spell/SubWpn Dmg++");
					}
					else if (hasEyeOfVlad != EyeOfVlad && EyeOfVlad != false)
					{
						hasEyeOfVlad = true;
						notificationService.AddMessage("1/8 crits for SubWpns");
					}
				}
				else
				{
					hasCubeOfZoe = CubeOfZoe;
					hasHolySymbol = HolySymbol;
					hasGravityBoots = GravityBoots;
					hasLeapStone = LeapStone;
					hasSoulOfWolf = SoulOfWolf;
					hasPowerOfWolf = PowerOfWolf;
					hasSkillOfWolf = SkillOfWolf;
					hasFormOfMist = FormOfMist;
					hasPowerOfMist = PowerOfMist;
					hasGasCloud = GasCloud;
					hasSoulOfBat = SoulOfBat;
					hasFireOfBat = FireOfBat;
					hasEchoOfBat = EchoOfBat;
					hasForceOfEcho = ForceOfEcho;

					hasHeartOfVlad = HeartOfVlad;
					hasToothOfVlad = ToothOfVlad;
					hasRibOfVlad = RibOfVlad;
					hasRingOfVlad = RingOfVlad;
					hasEyeOfVlad = EyeOfVlad;
				}
				//Vanilla Mayhem Messages
				if (!hasHolyGlasses && holyGlassesCheck)
				{
					hasHolyGlasses = true;
					if (!axeArmorActive || !toolConfig.Khaos.BoostAxeArmor || !toolConfig.Khaos.AxeArmorTips)
					{
						notificationService.AddMessage("HolyG: Blessings are now stronger");
					}

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
					if (!axeArmorActive || !toolConfig.Khaos.BoostAxeArmor || !toolConfig.Khaos.AxeArmorTips)
					{
						notificationService.AddMessage("Vlad: Curses are now stronger");
					}

					if (AutoMayhemOn && newVladCount - vladRelicsObtained == 1)
					{
						if (toolConfig.Khaos.autoAllowMayhemRage)
						{
							autoMayhemRage();
						}
					}
				}

				vladRelicsObtained = newVladCount;

				if (hexConfusedActive)
				{
					if (sotnApi.AlucardApi.HasControl())
					{
						sotnApi.GameApi.SetMovementSpeedDirection(true);
					}
					else
					{
						sotnApi.GameApi.SetMovementSpeedDirection(false);
					}
				}

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
					}
					while (sotnApi.AlucardApi.Subweapon == Subweapon.Empty || sotnApi.AlucardApi.Subweapon == Subweapon.Stopwatch)
					{
						RandomizeSubweapon();
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
				case "axearmor":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.LogCurrentRoom).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedActions.Add(new QueuedAction { Name = "AxeArmor", ChangesAlucardEffects = true, Type = ActionType.Neutral, Invoker = new MethodInvoker(() => AxeArmor(user)) });
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
				/*
				case "spawnentity":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SpawnEntity).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => SpawnEntity(user)));
					}
					break;
				*/
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
						queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user, false, true, false)));
					}
					break;
				case "slamjam":
					commandAction = toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.SlamJam).FirstOrDefault();
					if (commandAction is not null && ((!AutoMayhemOn && commandAction.Enabled) || (AutoMayhemOn && commandAction.AutoMayhemEnabled)))
					{
						queuedFastActions.Enqueue(new MethodInvoker(() => Slam(user, false, true, true)));
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
			respawnRichterTimer.Tick += RespawnRichterOff;
			respawnRichterTimer.Interval = 1 * (175);

			fireBallTimer.Tick += FireBallOff;
			fireBallTimer.Interval = 2 * 1000;
			spiritTimer.Tick += SpiritOff;
			spiritTimer.Interval = 10 * 1000;

			axeArmorContactDamageTimer.Tick += AxeArmorContactDamageTick;
			axeArmorContactDamageTimer.Interval = 6;

			axeArmorEffectTimer.Tick += AxeArmorEffectOff;
			axeArmorEffectTimer.Interval = 3;
			axeArmorHeart1Timer.Tick += AxeArmorHeart1Off;
			axeArmorHeart1Timer.Interval = 3;
			axeArmorHeart2Timer.Tick += AxeArmorHeart2Off;
			axeArmorHeart2Timer.Interval = 3;
			axeArmorHeart3Timer.Tick += AxeArmorHeart3Off;
			axeArmorHeart3Timer.Interval = 3;
			axeArmorHeart4Timer.Tick += AxeArmorHeart4Off;
			axeArmorHeart4Timer.Interval = 3;

			axeArmorHeart1FrameTimer.Tick += AxeArmorHeart1FrameTimerTick;
			axeArmorHeart1FrameTimer.Interval = 125;
			axeArmorHeart2FrameTimer.Tick += AxeArmorHeart2FrameTimerTick;
			axeArmorHeart2FrameTimer.Interval = 125;
			axeArmorHeart3FrameTimer.Tick += AxeArmorHeart3FrameTimerTick;
			axeArmorHeart3FrameTimer.Interval = 125;
			axeArmorHeart4FrameTimer.Tick += AxeArmorHeart4FrameTimerTick;
			axeArmorHeart4FrameTimer.Interval = 125;

			hpForMPDeathTimer.Tick += KillAlucard;
			hpForMPDeathTimer.Interval = 1 * (1 * 1500);
			HPForMPTimer.Tick += HPForMPOff;
			HPForMPTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault().Duration.TotalMilliseconds;
			HPForMPTickTimer.Tick += HPForMPGain;
			HPForMPTickTimer.Interval = 1000;

			heartsOnlyTimer.Tick += HeartsOnlyOff;
			heartsOnlyTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HeartsOnly).FirstOrDefault().Duration.TotalMilliseconds;
			underwaterTimer.Tick += UnderwaterOff;
			underwaterTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Underwater).FirstOrDefault().Duration.TotalMilliseconds;
			hexTimer.Tick += HexOff;
			hexTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Hex).FirstOrDefault().Duration.TotalMilliseconds;
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
			turboModeTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TurboMode).FirstOrDefault().Duration.TotalMilliseconds;
			unarmedTimer.Tick += UnarmedOff;
			unarmedTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Unarmed).FirstOrDefault().Duration.TotalMilliseconds;
			rushDownTimer.Tick += RushDownOff;
			rushDownTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.RushDown).FirstOrDefault().Duration.TotalMilliseconds;

			regenTimer.Tick += RegenOff;
			regenTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Regen).FirstOrDefault().Duration.TotalMilliseconds;
			regenTickTimer.Tick += RegenGain;
			regenTickTimer.Interval = 1000;
			buffStrRangeTimer.Tick += BuffStrRangeOff;
			buffStrRangeTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffStrRange).FirstOrDefault().Duration.TotalMilliseconds;
			buffConHPTimer.Tick += BuffConHPOff;
			buffConHPTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffConHP).FirstOrDefault().Duration.TotalMilliseconds;
			buffIntMPTimer.Tick += BuffIntMPOff;
			buffIntMPTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffIntMP).FirstOrDefault().Duration.TotalMilliseconds;
			buffLckNeutralTimer.Tick += BuffLckNeutralOff;
			buffLckNeutralTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.BuffLckNeutral).FirstOrDefault().Duration.TotalMilliseconds;
			timeStopTimer.Tick += TimeStopOff;
			timeStopTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.TimeStop).FirstOrDefault().Duration.TotalMilliseconds;
			timeStopCheckTimer.Tick += TimeStopAreaCheck;
			timeStopCheckTimer.Interval += 2 * 1000;

			speedTimer.Tick += SpeedOff;
			speedTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Speed).FirstOrDefault().Duration.TotalMilliseconds;
			speedOverdriveTimer.Tick += SpeedOverdriveOn;
			speedOverdriveTimer.Interval = (2 * 1000);
			speedOverdriveOffTimer.Tick += SpeedOverdriveOff;
			speedOverdriveOffTimer.Interval = (2 * 1000);

			summonerTimer.Tick += SummonerOff;
			summonerTimer.Interval = 5 * (60 * 1000);
			summonerSpawnTimer.Tick += SummonerSpawn;
			summonerSpawnTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.Summoner).FirstOrDefault().Interval.TotalMilliseconds;
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

			axeArmorContactDamageTimer.Interval = 3;

			axeArmorEffectTimer.Interval = 3;
			axeArmorHeart1Timer.Interval = 3;
			axeArmorHeart2Timer.Interval = 3;
			axeArmorHeart3Timer.Interval = 3;
			axeArmorHeart4Timer.Interval = 3;

			axeArmorHeart1FrameTimer.Interval = 125;
			axeArmorHeart2FrameTimer.Interval = 125;
			axeArmorHeart3FrameTimer.Interval = 125;
			axeArmorHeart4FrameTimer.Interval = 125;

			fireBallTimer.Interval = 2 * 1000;
			spiritTimer.Interval = 10 * 1000;

			hpForMPDeathTimer.Interval = 1 * (1 * 1500);
			HPForMPTimer.Interval = (int) toolConfig.Khaos.Actions.Where(a => a.Name == KhaosActionNames.HPForMP).FirstOrDefault().Duration.TotalMilliseconds;
			HPForMPTickTimer.Interval = 1000;

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

			HPForMPTickTimer.Interval = 1;
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

				if (sotnApi.GameApi.InAlucardMode() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave()
					&& (!IsInRoomList(Constants.Khaos.RichterRooms) || sotnApi.AlucardApi.HasControl() && IsInRoomList(Constants.Khaos.RichterRooms))
					&& !IsInRoomList(Constants.Khaos.LoadingRooms) && !IsInRoomList(Constants.Khaos.ClockRoom) && !IsInRoomList(Constants.Khaos.ShopRoom))
				{
					int index = 0;
					bool actionUnlocked = true;

					for (int i = 0; i < queuedActions.Count; i++)
					{
						index = i;
						actionUnlocked = true;
						if (
							(queuedActions[i].ChangesAlucardEffects && alucardEffectsLocked) ||
							(queuedActions[i].ChangesStats && statLocked) ||
							(queuedActions[i].ChangesWeaponQualities && weaponQualitiesLocked) ||
							(queuedActions[i].ChangesWeapons && weaponsLocked) ||
							(queuedActions[i].ChangesSubWeapons && subWeaponsLocked) ||
							(queuedActions[i].LocksSpeed && speedLocked) ||
							(queuedActions[i].LocksMana && mpLocked) ||
							(queuedActions[i].LocksHearts && heartsLocked) ||
							(queuedActions[i].LocksWeapons && weaponsLocked) ||
							(queuedActions[i].LocksInvincibility && invincibilityLocked) ||
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
			if (sotnApi.GameApi.InAlucardMode() && sotnApi.AlucardApi.HasControl() && sotnApi.AlucardApi.HasHitbox() && sotnApi.GameApi.CanMenu() && sotnApi.AlucardApi.CurrentHp > 0 && !sotnApi.GameApi.CanSave() && !sotnApi.GameApi.InTransition && !sotnApi.GameApi.IsLoading && !sotnApi.AlucardApi.IsInvincible() && !IsInRoomList(Constants.Khaos.LoadingRooms) && !IsInRoomList(Constants.Khaos.ClockRoom) && !IsInRoomList(Constants.Khaos.ClockRoom) && !IsInRoomList(Constants.Khaos.ShopRoom) && alucardMapX < 99)
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
				SetGalamothStats();
			}
			if (!galamothRoom)
			{
				galamothStatsSet = false;
			}
		}

		private void ExecuteAutoAction(Object sender, EventArgs e)
		{
			if (!sotnApi.GameApi.InAlucardMode() || IsInRoomList(Constants.Khaos.ShopRoom) || IsInRoomList(Constants.Khaos.ClockRoom) || sotnApi.GameApi.CanSave())
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
			if (sotnApi.GameApi.InAlucardMode() && !sotnApi.AlucardApi.IsInvincible() && sotnApi.AlucardApi.HasHitbox() && sotnApi.AlucardApi.HasControl() && !sotnApi.GameApi.IsInMenu())
			{
				Entity hitbox = new Entity();
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
				hpForMPDeathTimer.Stop();
			}
		}
		private void SpawnPoisonHitbox()
		{
			Entity hitbox = new();
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
			Entity hitbox = new();
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
			Entity hitbox = new();
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
			Entity hitbox = new Entity();
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
			if (axeArmorActive)
			{
				slamDuration = slamMaxDuration;
				axeArmorFloat.PokeValue(-250000);
				axeArmorFloat.Enable();
				//Console.WriteLine($"Slam initiated: {slamDuration}");
			}
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
			notificationService.InitializeService();
			savePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle);
			savePalette.Enable();
			if (toolConfig.Khaos.spiritOrbOn)
			{
				spirtOrb.Enable();
			}
			if (toolConfig.Khaos.faerieScrollOn)
			{
				faerieScroll.Enable();
			}
			if (toolConfig.Khaos.cubeOfZoeOn)
			{
				cubeOfZoe.Enable();
			}
			if (toolConfig.Khaos.OpenEntranceDoor)
			{
				openEntranceDoor.Enable();
			}

			if (sotnApi.GameApi.InAlucardMode())
			{
				playerPaletteCheat.Disable();
				enemyRichterPaletteCheat.Disable();
			}
			//else
			//{
			//UpdatePlayerColor();
			//UpdateVisualEffect(true);
			//resetColorWhenAlucard = true;
			//}

			if (toolConfig.Khaos.BoostFamiliars)
			{
				setFamiliarXP();
			}
			if (toolConfig.Khaos.ContinuousWingsmash)
			{
				continuousWingsmash.Enable();
			}

			richterCutscene2.PokeValue(0);
			richterCutscene1.PokeValue(0);
		}
		private void setFamiliarXP()
		{
			int highXP = Constants.Khaos.highFamiliarXP;
			int medXP = Constants.Khaos.medFamiliarXP;
			int lowXP = Constants.Khaos.lowFamiliarXP;

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
			int offset = rng.Next(0, 15);
			if (alucardSecondCastle)
			{
				savePalette.PokeValue(Constants.Khaos.SaveIcosahedronSecondCastle + offset);
			}
			else
			{
				savePalette.PokeValue(Constants.Khaos.SaveIcosahedronFirstCastle + offset);
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
					//Mayhem Resets
					GainMayhemMeter((short) toolConfig.Khaos.MeterOnReset);
					accelTime.PokeValue(5);
					turboMode.Disable();
					turboModeJump.Disable();

					//Axe Armor Cheats

					state.Disable();
					action.Disable();
					startAxeArmor.Disable();
					permaAxeArmor.Disable();
					characterData.Disable();
					enemyRichterPaletteCheat.Disable();

					alucardHurtboxX.Disable();
					alucardHurtboxY.Disable();

					axeArmorStatsActive = false;
					//axeArmor2ndCastleStatsActive = false;
					axeArmorFloat.Disable();
					axeArmorWeapon.Disable();
					axeArmorDamage.Disable();
					axeArmorDamageTypeA.Disable();
					axeArmorDamageTypeB.Disable();
					axeArmorDamageInterval.Disable();
					
					axeArmorDefense.Disable();
					axeArmorHorizontalJump.Disable();
					axeArmorHorizontalSpeed.Disable();
					axeArmorVerticalJumpMoving.Disable();
					axeArmorVerticalJumpStationary.Disable();

					clipDirection.Disable();
					ceilingClip.Disable();
					leftClip.Disable();
					rightClip.Disable();
					batLightRoom.Disable();
					jewelSwordRoom.Disable();
					jewelSwordRoomReverse.Disable();
					swordCardLvl.Disable();
					smoothCrouch.Disable();

					//Reset Axe Armor Variables
					hasBatCard = false;
					hasFaerieCard = false;
					hasGhostCard = false;
					hasDemonCard = false;
					hasNoseDevilCard = false;
					hasSwordCard = false;
					hasSpriteCard = false;
					
					hasFaerieScroll = false;
					hasSpiritOrb = false;
					hasMermanStatue = false;
					hasJewelOfOpen = false;
					
					hasSpikeBreaker = false;
					hasBrilliantMail = false;
					hasHealingMail = false;
					hasMojoMail = false;

					heartGlobalCooldown = 0;
					heartUsage1Cooldown = 1;
					heartUsage2Cooldown = 1;
					heartUsage3Cooldown = 1;
					heartUsage4Cooldown = 1;
					heartLock1Cooldown = 1;
					heartLock2Cooldown = 1;
					heartLock3Cooldown = 1;
					heartLock4Cooldown = 1;
					UpdateAxeArmorHeartCooldowns();
					AxeArmorRemoveEntities();

					//Other
					hpForMPDeathTimer.Stop();
					CleanUpAxeArmorCheats();
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
			swordCardLvl = cheats.GetCheatByName("SwordCardLvl");
			spriteCardXp = cheats.GetCheatByName("SpriteCardXp");
			noseDevilCardXp = cheats.GetCheatByName("NoseDevilCardXp");

			//New Cheats
			spirtOrb = cheats.GetCheatByName("SpiritOrb");
			openEntranceDoor = cheats.GetCheatByName("OpenEntranceDoor");
			richterCutscene1 = cheats.GetCheatByName("RichterCutscene1");
			richterCutscene2 = cheats.GetCheatByName("RichterCutscene2");
			fightRichter1 = cheats.GetCheatByName("FightRichter1");
			fightRichter2 = cheats.GetCheatByName("FightRichter2");
			fightRichter3 = cheats.GetCheatByName("FightRichter3");
			//saveRichter = cheats.GetCheatByName("SaveRichter");


			alucardHurtboxX = cheats.GetCheatByName("AlucardHurtBoxX");
			alucardHurtboxY = cheats.GetCheatByName("AlucardHurtBoxY");

			startAxeArmor = cheats.GetCheatByName("ArmorIndexStart");
			permaAxeArmor = cheats.GetCheatByName("ArmorIndex");
			axeArmorVerticalJumpMoving = cheats.GetCheatByName("AxeArmorVertJumpMove");
			axeArmorVerticalJumpStationary = cheats.GetCheatByName("AxeArmorVertJumpStand");
			axeArmorHorizontalJump = cheats.GetCheatByName("AxeArmorHoriJump");
			axeArmorFloat = cheats.GetCheatByName("AxeArmorFloat");
			axeArmorDefense = cheats.GetCheatByName("AxeArmorDefense");
			axeArmorDamage = cheats.GetCheatByName("AxeArmorDamage");
			axeArmorWeapon = cheats.GetCheatByName("AxeArmorWeapon");
			axeArmorDamageTypeA = cheats.GetCheatByName("AxeArmorDamageTypeA");
			axeArmorDamageTypeB = cheats.GetCheatByName("AxeArmorDamageTypeB");
			axeArmorDamageInterval = cheats.GetCheatByName("AxeArmorDamageInterval");
			axeArmorWeapon = cheats.GetCheatByName("AxeArmorWeapon");
			axeArmorHorizontalSpeed = cheats.GetCheatByName("AxeArmorHoriSpeed");
			toggleHurtBox = cheats.GetCheatByName("ToggleHurtBox");
			contactDamageType = cheats.GetCheatByName("ContactDamageType");
			jewelSwordRoom = cheats.GetCheatByName("JewelSwordRoom");
			jewelSwordRoomReverse = cheats.GetCheatByName("JewelSwordReverseRoom");
			batLightRoom = cheats.GetCheatByName("BatLightRoom");
			smoothCrouch = cheats.GetCheatByName("SmoothCrouch");
			clipDirection = cheats.GetCheatByName("ClipDirection");
			ceilingClip = cheats.GetCheatByName("CeilingClip");
			leftClip = cheats.GetCheatByName("LeftClip");
			rightClip = cheats.GetCheatByName("RightClip");

			characterData = cheats.GetCheatByName("CharacterData");
			state = cheats.GetCheatByName("State");
			action = cheats.GetCheatByName("Action");

			rewind = cheats.GetCheatByName("Rewind");
			alucardEffect = cheats.GetCheatByName("AlucardEffect");
			turboMode = cheats.GetCheatByName("TurboMode");
			turboModeJump = cheats.GetCheatByName("TurboModeJump");
			accelTime = cheats.GetCheatByName("AccelTime");
			subWeaponDamage1 = cheats.GetCheatByName("SubWeaponDamage1");
			subWeaponDamage2 = cheats.GetCheatByName("SubWeaponDamage2");
			subWeaponDamage3 = cheats.GetCheatByName("SubWeaponDamage3");
			subWeaponDamage4 = cheats.GetCheatByName("SubWeaponDamage4");
			subWeaponDamage5 = cheats.GetCheatByName("SubWeaponDamage5");
			subWeaponDamage6 = cheats.GetCheatByName("SubWeaponDamage6");
			throwMoreSubWeapons1 = cheats.GetCheatByName("ThrowMoreSubWeapons1");
			throwMoreSubWeapons2 = cheats.GetCheatByName("ThrowMoreSubWeapons2");
			throwMoreSubWeapons3 = cheats.GetCheatByName("ThrowMoreSubWeapons3");
			longerHolyWater1 = cheats.GetCheatByName("LongerHolyWater1");
			longerHolyWater2 = cheats.GetCheatByName("LongerHolyWater2");
			longerHolyWater3 = cheats.GetCheatByName("LongerHolyWater3");
			tallerHolyWater1 = cheats.GetCheatByName("TallerHolyWater1");
			tallerHolyWater2 = cheats.GetCheatByName("TallerHolyWater2");
			tallerHolyWater3 = cheats.GetCheatByName("TallerHolyWater3");
			enemyRichterPaletteCheat = cheats.GetCheatByName("EnemyRichterPalette");
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
				uint shaftBaseHP = 0;
				int bonusHP = 0;

				if (toolConfig.Khaos.PermaAxeArmor)
				{
					shaftBaseHP = Constants.Khaos.ShaftAxeArmorHp;
					bonusHP = (int) ((2 * toolConfig.Khaos.ShaftOrbHPModifier) + (.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier));
				}
				else
				{
					shaftBaseHP = Constants.Khaos.ShaftMayhemHp;
					bonusHP = (int) ((2 * toolConfig.Khaos.ShaftOrbHPModifier) + (.5 * vladRelicsObtained * toolConfig.Khaos.CurseModifier));

				}

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
						//Orignal:bonusHP *= 3
						bonusHP += (int) ((.5 * bonusHP * toolConfig.Khaos.ShaftOrbHPModifier) + (.5 * bonusHP * toolConfig.Khaos.SuperBossHPModifier));
					}
					else
					{
						//Orignal:bonusHP *= 2
						bonusHP += (int) ((.5 * bonusHP * toolConfig.Khaos.ShaftOrbHPModifier));
					}

				}

				shaft.Hp = (int) (shaftBaseHP + bonusHP);
				shaftHpSet = true;
				Console.WriteLine($"Found Shaft Orb actor and set HP to: {shaft.Hp}; bonus HP {bonusHP}");

			}
			else
			{
				return;
			}
		}
		private void SetGalamothStats()
		{
			long galamothTorsoAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.GalamothTorsoActor });
			uint axeArmorDef = 0;

			if (galamothTorsoAddress > 0)
			{
				LiveEntity galamothTorso = sotnApi.EntityApi.GetLiveEntity(galamothTorsoAddress);

				//Original
				//int bonusHp = 500 + (250 * (vladRelicsObtained));
				int bonusHp = 0;

				if (toolConfig.Khaos.GalamothBossHPModifier > 0)
				{
					double galamothFlatHp = 250;
					double galamothScalingHp = 125;
					double axeArmorFlatHp = 0;
					

					if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor)
					{
						galamothFlatHp = 1833;
						galamothScalingHp = 333;
						if (hasRibOfVlad)
						{
							axeArmorDef += 4;
						}
						if (hasToothOfVlad)
						{
							axeArmorDef += 3;
						}
						if (hasRingOfVlad)
						{
							axeArmorDef += 3;
						}
						if (hasEyeOfVlad)
						{
							axeArmorDef += 2;
						}
						if (hasHolySymbol)
						{
							axeArmorDef += 2;
						}
						if (hasHeartOfVlad)
						{
							axeArmorDef += 1;
						}
						if (hasCubeOfZoe)
						{
							axeArmorFlatHp += 300;
							axeArmorDef += 1;
						}
						if (hasSoulOfBat)
						{
							axeArmorFlatHp += 200;
						}
						if (hasEchoOfBat)
						{
							axeArmorFlatHp += 200;
						}
						if (hasFireOfBat)
						{
							axeArmorFlatHp += 200;
						}
						if (hasForceOfEcho)
						{
							axeArmorFlatHp += 200;
						}
						if (hasFormOfMist)
						{
							axeArmorFlatHp += 150;
						}
						if (hasPowerOfMist)
						{
							axeArmorFlatHp += 150;
						}
						if (hasGasCloud)
						{
							axeArmorFlatHp += 150;
						}
						if (hasSoulOfWolf)
						{
							axeArmorFlatHp += 150;
						}
						if (hasSkillOfWolf)
						{
							axeArmorFlatHp += 150;
						}
						if (hasPowerOfWolf)
						{
							axeArmorFlatHp += 150;
						}
					}

					galamothFlatHp *= toolConfig.Khaos.GalamothBossHPModifier;

					bonusHp = (int) ((galamothFlatHp + (galamothScalingHp * vladRelicsObtained * toolConfig.Khaos.CurseModifier)) * (toolConfig.Khaos.GalamothBossHPModifier * .5));

					galamothTorso.Hp = (int) (Constants.Khaos.GalamothMayhemHp + bonusHp + axeArmorFlatHp);

					if (toughBossesCount > 0)
					{
						toughBossesCount--;
						toughBossesRoomX = sotnApi.GameApi.RoomX;
						toughBossesRoomY = sotnApi.GameApi.RoomY;

						if (toughBossesCount == 0)
						{
							toughBossesSpawnTimer.Stop();
						}

						if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor)
						{
							if (superToughBosses)
							{
								galamothTorso.Hp = (int) (Constants.Khaos.GalamothMayhemHp + (1.25 * bonusHp * toolConfig.Khaos.GalamothBossHPModifier) + (1.25 * bonusHp * toolConfig.Khaos.SuperBossHPModifier));
								notificationService.AddMessage($"Super {KhaosActionNames.ToughBosses} Galamoth");
							}
							else
							{
								galamothTorso.Hp = (int) (Constants.Khaos.GalamothMayhemHp + (1.25 * toolConfig.Khaos.GalamothBossHPModifier * bonusHp));
								notificationService.AddMessage($"{KhaosActionNames.ToughBosses} Galamoth");
							}
						}
						else
						{
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
						if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor)
						{
							galamothAnchor.Def = 14 + axeArmorDef;
						}
						else
						{
							galamothAnchor.Def = 0;
						}
					}
					if (toolConfig.Khaos.GalamothBossDMGModifier > 0)
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

		private void UpdateAxeArmor()
		{
			++axeArmorFrameCount;

			if (delayStart)
			{
				if (axeArmorFrameDelay > axeArmorFrameCount)
				{
					return;
				}
				else
				{
					axeArmorFrameCount = 1;
					delayStart = false;
				}
			}
			else if (axeArmorFrameCount > Globals.UpdateCooldownFrames)
			{
				axeArmorFrameCount = 1;
			}
				
			CheckAxeArmorStats();

			axeArmorActive = Equipment.Items[(int) (sotnApi.AlucardApi.Armor + Equipment.HandCount + 1)] == "Axe Lord armor";

			CheckAxeArmorFireballs();

			bool spiritOrb = sotnApi.AlucardApi.HasRelic(Relic.SpiritOrb);
			bool faerieScroll = sotnApi.AlucardApi.HasRelic(Relic.FaerieScroll);
			bool jewelOfOpen = sotnApi.AlucardApi.HasRelic(Relic.JewelOfOpen);
			bool mermanStatue = sotnApi.AlucardApi.HasRelic(Relic.MermanStatue);

			bool batCard = sotnApi.AlucardApi.HasRelic(Relic.BatCard);
			bool demonCard = sotnApi.AlucardApi.HasRelic(Relic.DemonCard);
			bool noseDevilCard = sotnApi.AlucardApi.HasRelic(Relic.NoseDevilCard);
			bool faerieCard = sotnApi.AlucardApi.HasRelic(Relic.FaerieCard);
			bool spriteCard = sotnApi.AlucardApi.HasRelic(Relic.SpriteCard);
			bool ghostCard = sotnApi.AlucardApi.HasRelic(Relic.GhostCard);
			bool swordCard = sotnApi.AlucardApi.HasRelic(Relic.SwordCard);

			bool spikeBreaker = sotnApi.AlucardApi.HasItemInInventory("Spike Breaker");
			bool brilliantMail = sotnApi.AlucardApi.HasItemInInventory("Brilliant mail");
			bool healingMail = sotnApi.AlucardApi.HasItemInInventory("Healing mail");
			bool mojoMail = sotnApi.AlucardApi.HasItemInInventory("Mojo mail");

			bool hasHammer = sotnApi.AlucardApi.HasItemInInventory("Hammer");
			bool consumeHammer = false;
			bool hasBloodStoneFirst = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Bloodstone";
			bool hasBloodStoneSecond = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Bloodstone";
			uint bloodStoneCount = 0;

			bool heldUp = inputService.ButtonHeld(PlaystationInputKeys.Up);
			bool heldDown = inputService.ButtonHeld(PlaystationInputKeys.Down);
			bool heldLeft = inputService.ButtonHeld(PlaystationInputKeys.Left);
			bool heldRight = inputService.ButtonHeld(PlaystationInputKeys.Right);
			bool heldCross = inputService.ButtonHeld(PlaystationInputKeys.Cross);
			bool heldR1 = inputService.ButtonHeld(PlaystationInputKeys.R1);
			bool heldR2 = inputService.ButtonHeld(PlaystationInputKeys.R2);

			bool pressUpCDFrames = inputService.ButtonPressed(PlaystationInputKeys.Up, Globals.UpdateCooldownFrames);
			bool pressDownCDFrames = inputService.ButtonPressed(PlaystationInputKeys.Down, Globals.UpdateCooldownFrames);
			bool pressLeftCDFrames = inputService.ButtonPressed(PlaystationInputKeys.Left, Globals.UpdateCooldownFrames);
			bool pressRightCDFrames = inputService.ButtonPressed(PlaystationInputKeys.Right, Globals.UpdateCooldownFrames);
			bool pressSquareCDFrames = inputService.ButtonPressed(PlaystationInputKeys.Square, Globals.UpdateCooldownFrames);
			bool pressTriangleCDFrames = inputService.ButtonPressed(PlaystationInputKeys.Triangle, Globals.UpdateCooldownFrames);
			bool pressCircleCDFrames = inputService.ButtonPressed(PlaystationInputKeys.Circle, Globals.UpdateCooldownFrames);
			bool pressCrossCDFrames = inputService.ButtonPressed(PlaystationInputKeys.Cross, Globals.UpdateCooldownFrames);
			bool pressL1_CDFrames = inputService.ButtonPressed(PlaystationInputKeys.L1, Globals.UpdateCooldownFrames);
			bool pressR1_CDFrames = inputService.ButtonPressed(PlaystationInputKeys.R1, Globals.UpdateCooldownFrames);
			bool pressR2_CDFrames = inputService.ButtonPressed(PlaystationInputKeys.R2, Globals.UpdateCooldownFrames);

			bool pressLeft10 = inputService.ButtonPressed(PlaystationInputKeys.Left, 10);
			bool pressRight10 = inputService.ButtonPressed(PlaystationInputKeys.Right, 10);
			bool pressR2_10 = inputService.ButtonPressed(PlaystationInputKeys.R2, 10);

			bool pressUp5 = inputService.ButtonPressed(PlaystationInputKeys.Up, 5);
			bool pressDown5 = inputService.ButtonPressed(PlaystationInputKeys.Down, 5);
			bool pressLeft5 = inputService.ButtonPressed(PlaystationInputKeys.Left, 5);
			bool pressRight5 = inputService.ButtonPressed(PlaystationInputKeys.Right, 5);
			bool pressL1_5 = inputService.ButtonPressed(PlaystationInputKeys.L1, 5);
			bool pressL1_16 = inputService.ButtonPressed(PlaystationInputKeys.L1, 16);

			bool pressLeft32 = inputService.ButtonPressed(PlaystationInputKeys.Left, 32);
			bool pressRight32 = inputService.ButtonPressed(PlaystationInputKeys.Right, 32);
			bool pressSquare32 = inputService.ButtonPressed(PlaystationInputKeys.Square, 32);
			bool pressTriangle32 = inputService.ButtonPressed(PlaystationInputKeys.Triangle, 32);
			bool pressCircle32 = inputService.ButtonPressed(PlaystationInputKeys.Circle, 32);
			bool pressCross32 = inputService.ButtonPressed(PlaystationInputKeys.Cross, 32);
			bool pressL1_32 = inputService.ButtonPressed(PlaystationInputKeys.L1, 32);
			bool pressR1_32 = inputService.ButtonPressed(PlaystationInputKeys.R1, 32);
			bool pressR2_32 = inputService.ButtonPressed(PlaystationInputKeys.R2, 32);

			bool pressCross60 = inputService.ButtonPressed(PlaystationInputKeys.Cross, 60);
			bool pressL1_60 = inputService.ButtonPressed(PlaystationInputKeys.L1, 60);
			bool pressR1_60 = inputService.ButtonPressed(PlaystationInputKeys.R1, 60);

			if (hasBloodStoneFirst)
			{
				++bloodStoneCount;
			}
			if (hasBloodStoneSecond)
			{
				++bloodStoneCount;
			}

			if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor)
			{
				if (axeArmorSwordFamiliar)
				{
					//Cap sword familiar weapon to level 49 to prevent item duping.
					int swordLvl = (int)(sotnApi.AlucardApi.Level);
					if (swordLvl > 50)
					{
						swordLvl = 49;
					}
					swordCardLvl.PokeValue(swordLvl);
					swordCardLvl.Enable();
				}
				else
				{
					swordCardLvl.Disable();
				}

				bool canGrantBonuses = false;
				if (!IsInRoomList(Constants.Khaos.LoadingRooms) && !sotnApi.GameApi.CanSave())
				{
					canGrantBonuses = true;
				}

				if (hasHammer && !subWeaponsLocked && sotnApi.AlucardApi.Subweapon == 0)
				{
					int roll = rng.Next(1, 10);
					sotnApi.AlucardApi.Subweapon = (Subweapon) roll;
					sotnApi.AlucardApi.TakeOneItemByName("Hammer");
					consumeHammer = true;
				}


				if (toolConfig.Khaos.AxeArmorTips)
				{
					if (canGrantBonuses)
					{
						//Stat Boosts
						if (spiritOrb == true && hasSpiritOrb == false)
						{
							notificationService.AddMessage("+15 Max MP/Hearts");
						}
						if (faerieScroll == true && hasFaerieScroll == false)
						{
							notificationService.AddMessage("+30 Max HP");
						}
						if (jewelOfOpen == true && hasJewelOfOpen == false)
						{
							notificationService.AddMessage("+25 Max MP");
						}
						if (mermanStatue == true && hasMermanStatue == false)
						{
							notificationService.AddMessage("+20 Max Hearts");
						}

						//Familiars
						if (batCard == true && hasBatCard == false)
						{
							notificationService.AddMessage("+20 Monster Vial 2");
						}
						if (demonCard == true && hasDemonCard == false)
						{
							notificationService.AddMessage("+1 Moonstone");
						}
						if (noseDevilCard == true && hasNoseDevilCard == false)
						{
							notificationService.AddMessage("+1 Opal");
						}
						if (faerieCard == true && hasFaerieCard == false)
						{
							notificationService.AddMessage("+1 Elixir");
						}
						if (spriteCard == true && hasSpriteCard == false)
						{
							notificationService.AddMessage("+1 Life apple");
						}
						if (ghostCard == true && hasGhostCard == false)
						{
							notificationService.AddMessage("+1 Healing Mail");
						}
						if (swordCard == true && hasSwordCard == false)
						{
							notificationService.AddMessage("+1 Sword Familiar");
						}
					}
					
					//Armors
					if (spikeBreaker == true && hasSpikeBreaker == false)
					{
						notificationService.AddMessage("Spike Immunity");
					}
					if (brilliantMail == true && hasBrilliantMail == false)
					{
						notificationService.AddMessage("SubWpn Dmg + 10");
					}
					if (mojoMail == true && hasMojoMail == false)
					{
						notificationService.AddMessage("Faceplant/Spell + Flat Damage");
					}
					if (healingMail == true && hasHealingMail == false)
					{
						notificationService.AddMessage("Slow HP when Moving");
					}

					//Items
					if(consumeHammer)
					{
						notificationService.AddMessage("Hammer to Subweapon");
					}
				}

				//Stat Boosts
				if (spiritOrb == true && hasSpiritOrb == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.MaxtMp += Constants.Khaos.SpirtOrbHeartMPBoost;
						sotnApi.AlucardApi.MaxtHearts += Constants.Khaos.SpirtOrbHeartMPBoost;
						if (sotnApi.AlucardApi.CurrentMp < sotnApi.AlucardApi.MaxtMp)
						{
							sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
						}
						if (sotnApi.AlucardApi.CurrentHearts < sotnApi.AlucardApi.MaxtHearts)
						{
							sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts;
						}
					}
					hasSpiritOrb = true;
				}
				if (faerieScroll == true && hasFaerieScroll == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.MaxtHp += Constants.Khaos.FaerieScrollHPBoost;
						if (sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp)
						{
							sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
						}
					}
					hasFaerieScroll = true;
				}
				if (jewelOfOpen == true && hasJewelOfOpen == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.MaxtHp += Constants.Khaos.JewelOfOpenMPBoost;
						if (sotnApi.AlucardApi.CurrentMp < sotnApi.AlucardApi.MaxtMp)
						{
							sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
						}
					}
					hasJewelOfOpen = true;
				}
				if (mermanStatue == true && hasMermanStatue == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.MaxtHearts += Constants.Khaos.MermanStatueHeartBoost;
						if (sotnApi.AlucardApi.CurrentHearts < sotnApi.AlucardApi.MaxtHearts)
						{
							sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts;
						}
					}
					hasMermanStatue = true;
				}

				// Familiar Flag
				if (batCard == true && hasBatCard == false)
				{
					if (canGrantBonuses)
					{
						for(int i = 0; i < 20; i++)
						{
							sotnApi.AlucardApi.GrantItemByName("monster vial 2");
						}
					}
					hasBatCard = true;
				}
				if (demonCard == true && hasDemonCard == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.GrantItemByName("Moonstone");
					}
					hasDemonCard = true;
				}
				if (noseDevilCard == true && hasNoseDevilCard == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.GrantItemByName("Opal");
					}
					hasNoseDevilCard = true;
				}
				if (faerieCard == true && hasFaerieCard == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.GrantItemByName("Elixir");
					}
					hasFaerieCard = true;
				}
				if (spriteCard == true && hasSpriteCard == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.GrantItemByName("Life apple");
					}
					hasSpriteCard = true;
				}
				if (ghostCard == true && hasGhostCard == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.GrantItemByName("Healing mail");
					}
					hasGhostCard = true;
				}
				if (swordCard == true && hasSwordCard == false)
				{
					if (canGrantBonuses)
					{
						sotnApi.AlucardApi.GrantItemByName("Sword Familiar");
					}
					hasSwordCard = true;
				}

				//Restore Items if removed
				if (swordCard == false && hasSwordCard == true)
				{
					bool equippedSword = Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Sword Familiar" || Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Sword Familiar" || sotnApi.AlucardApi.HasItemInInventory("Sword Familiar");
					if (!equippedSword)
					{
						sotnApi.AlucardApi.GrantItemByName("Sword Familiar");
					}
					swordCard = true;
				}
				if (spikeBreaker == false && hasSpikeBreaker == true)
				{
					sotnApi.AlucardApi.GrantItemByName("Spike Breaker");
					spikeBreaker = true;
				}
				if (brilliantMail == false && hasBrilliantMail == true)
				{
					sotnApi.AlucardApi.GrantItemByName("Brilliant mail");
					brilliantMail = true;
				}
				if (healingMail == false && (hasHealingMail && hasGhostCard))
				{
					sotnApi.AlucardApi.GrantItemByName("Healing mail");
					healingMail = true;
				}
				if (mojoMail == false && hasMojoMail == true)
				{
					sotnApi.AlucardApi.GrantItemByName("Mojo mail");
					mojoMail = true;
				}

				if (previousHP > sotnApi.AlucardApi.CurrentHp)
				{
					bool equippedTalismanFirst = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Talisman";
					bool equippedTalismanSecond = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Talisman";
					int maxHeld = 0;

					if (equippedTalismanFirst)
					{
						maxHeld += 1;
					}
					if (equippedTalismanSecond)
					{
						maxHeld += 1;
					}

					for (int i = 0; i < maxHeld; i++)
					{
						int max = (int) (10 - ((sotnApi.AlucardApi.Lck + equipmentLCK) / 10));
						if (max < 3)
						{
							max = 3;
						}
						int roll = rng.Next(0, max);
						if (roll == 0)
						{
							AxeArmorStatRestore(previousHP -sotnApi.AlucardApi.CurrentHp, true, false, false, false);
							break;
						}
					}
				}

				if (previousExperience < sotnApi.AlucardApi.Experiecne)
				{
					bool equippedMourne = Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Mourneblade" || Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Mourneblade";
					bool equippedJewel = Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)] == "Jewel sword" || Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)] == "Jewel sword";

					if (equippedMourne)
					{
						//Mourneblade Heal Effect
						AxeArmorStatRestore(sotnApi.AlucardApi.Level, true, false, false, false);
					}
					if (equippedJewel)
					{
						int max = (int)(16 - ((sotnApi.AlucardApi.Lck + equipmentLCK) / 20));
						if (max < 3)
						{
							max = 3;
						}
						int roll = rng.Next(0, max);
						if (roll == 0)
						{
							roll = rng.Next(0, 200);
							if(roll <= 99)
							{
								notificationService.AddMessage($"+1 Zircon");
								sotnApi.AlucardApi.GrantItemByName("Zircon");
							}
							else if (roll <= 150)
							{
								notificationService.AddMessage($"+1 Aquamarine");
								sotnApi.AlucardApi.GrantItemByName("Aquamarine");
							}
							else if (roll <= 190)
							{
								notificationService.AddMessage($"+1 Turquoise");
								sotnApi.AlucardApi.GrantItemByName("Turquoise");
							}
							else if (roll <= 194)
							{
								notificationService.AddMessage($"+1 Onyx");
								sotnApi.AlucardApi.GrantItemByName("Onyx");
							}
							else if (roll <= 196)
							{
								notificationService.AddMessage($"+1 Garnet");
								sotnApi.AlucardApi.GrantItemByName("Garnet");
							}
							else if (roll <= 198)
							{
								notificationService.AddMessage($"+1 Opal");
								sotnApi.AlucardApi.GrantItemByName("Opal");
							}
							else
							{
								notificationService.AddMessage($"+1 Diamond");
								sotnApi.AlucardApi.GrantItemByName("Diamond");
							}
							
							
						}
					}
				}
			}
			else
			{
				notificationService.WeaponMessage = "";
				notificationService.EquipMessage = "";
			}

			previousExperience = sotnApi.AlucardApi.Experiecne;
			previousHP = sotnApi.AlucardApi.CurrentHp;

			if (toolConfig.Khaos.PermaAxeArmor)
			{
				permaAxeArmor.Enable();
			}
			else
			{
				permaAxeArmor.Disable();
			}

			hasBrilliantMail = brilliantMail;
			hasSpikeBreaker = spikeBreaker;
			hasHealingMail = healingMail;
			hasMojoMail = mojoMail;

			if (wolfCurrentSpeed == 0)
			{
				axeArmorHorizontalSpeed.Disable();
			}

			if (axeArmorActive && toolConfig.Khaos.BoostAxeArmor)
			{
				if (sotnApi.GameApi.IsInMenu())
				{
					int menuDamage = 0;
					int menuDamageTotal;
					CheckAxeArmorAttack(false, menuDamage, out menuDamageTotal);
				}

				uint currentMP = sotnApi.AlucardApi.CurrentMp;
				uint currentHearts = sotnApi.AlucardApi.CurrentHearts;

				AxeArmorRoomCheck();
				if (!axeArmorStatsActive)
				{
					notificationService.AddMessage($"Axe Armor Mode!");
					axeArmorDamage.Enable();
					axeArmorDamageTypeA.Enable();
					axeArmorDamageTypeB.Enable();
					axeArmorDamageInterval.Enable();
					axeArmorWeapon.PokeValue(unchecked((int) 2148678240)); //80123A60
					axeArmorWeapon.Enable();
					axeArmorVerticalJumpMoving.Enable();
					axeArmorVerticalJumpStationary.Enable();
					axeArmorHorizontalJump.Enable();
					axeArmorStatsActive = true;
				}

				if (Globals.UpdateCooldownFrames == axeArmorFrameCount)
				{
					//UpdateAxeArmorDefense
					int newDefense = 0;

					if (sotnApi.AlucardApi.HasItemInInventory("God's Garb"))
					{
						newDefense = 25;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Alucard mail"))
					{
						newDefense = 20;
					}
					else if (alucardSecondCastle && sotnApi.AlucardApi.HasItemInInventory("Walk armor"))
					{
						newDefense = 18;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Diamond plate")
						|| sotnApi.AlucardApi.HasItemInInventory("Mojo mail"))
					{
						newDefense = 15;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Dracula tunic"))
					{
						newDefense = 14;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Fury plate"))
					{
						newDefense = 13;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Spike Breaker"))
					{
						newDefense = 12;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Holy mail"))
					{
						newDefense = 11;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Platinum mail"))
					{
						newDefense = 10;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Walk armor"))
					{
						newDefense = 9;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Gold plate")
						|| sotnApi.AlucardApi.HasItemInInventory("Lightning mail")
						|| sotnApi.AlucardApi.HasItemInInventory("Ice mail")
						|| sotnApi.AlucardApi.HasItemInInventory("Dark armor"))
					{
						newDefense = 8;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Silver plate"))
					{
						newDefense = 6;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Steel cuirass"))
					{
						newDefense = 5;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Fire mail")
						|| sotnApi.AlucardApi.HasItemInInventory("Healing mail")
						|| sotnApi.AlucardApi.HasItemInInventory("Iron cuirass"))
					{
						newDefense = 4;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Mirror cuirass")
						|| sotnApi.AlucardApi.HasItemInInventory("Bronze cuirass"))
					{
						newDefense = 3;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Hide cuirass")
						|| sotnApi.AlucardApi.HasItemInInventory("Alucart mail"))
					{
						newDefense = 2;
					}
					else if (sotnApi.AlucardApi.HasItemInInventory("Cloth tunic"))
					{
						newDefense = 1;
					}

					if (sotnApi.AlucardApi.HasRelic(Relic.RibOfVlad))
					{
						newDefense += 5;
					}

					if (previousDefense != newDefense)
					{
						if (toolConfig.Khaos.AxeArmorTips)
						{
							notificationService.AddMessage($"Armor Def: {newDefense}");
						}
					}

					previousDefense = newDefense;

					axeArmorDefense.PokeValue(newDefense);
					axeArmorDefense.Enable();

					if (sotnApi.GameApi.IsInMenu() || IsInRoomList(Constants.Khaos.ShopRoom))
					{
						axeArmorSafetyCooldown = axeArmorMaxSafetyCooldown;
					}
					else if (axeArmorSafetyCooldown > 0)
					{
						--axeArmorSafetyCooldown;
					}
					if (facePlantCooldown > 0)
					{
						--facePlantCooldown;
					}
					if (spellCooldown > 0)
					{
						--spellCooldown;
					}
					if (gurthangBoostTimer > 0)
					{
						--gurthangBoostTimer;
					}
					if (thrustDuration > 0)
					{
						--thrustDuration;
					}
					else
					{
						isAxeArmorThrustAttack = false;
					}
					if (sotnApi.AlucardApi.HasControl()
					&& ((sotnApi.AlucardApi.State == 40 || sotnApi.AlucardApi.State == 41 || sotnApi.AlucardApi.State == 42) && sotnApi.AlucardApi.Action == 0)
					&& !isAxeArmorGravityJump
					&& !inputService.ButtonPressed(PlaystationInputKeys.L1, Globals.UpdateCooldownFrames)
					&& !inputService.ButtonPressed(PlaystationInputKeys.R1, Globals.UpdateCooldownFrames)
					&& !inputService.ButtonPressed(PlaystationInputKeys.Triangle, Globals.UpdateCooldownFrames)
					)
					{
						if (!sotnApi.GameApi.IsInMenu())
						{
							if (axeArmorSuperMPRegenCooldown == 0)
							{
								if (axeArmorMPRegenCooldown > 0)
								{
									--axeArmorMPRegenCooldown;
								}
								if (axeArmorMPRegenCooldown > 0)
								{
									--axeArmorMPRegenCooldown;
								}
								if (axeArmorMPRegenCooldown > 0)
								{
									--axeArmorMPRegenCooldown;
								}
								if (axeArmorMPRegenCooldown > 0)
								{
									--axeArmorMPRegenCooldown;
								}
							}
							else
							{
								--axeArmorSuperMPRegenCooldown;
								if (axeArmorSuperMPRegenCooldown <= 20)
								{
									if (axeArmorMPRegenCooldown > 0)
									{
										--axeArmorMPRegenCooldown;
									}
								}
								if (axeArmorSuperMPRegenCooldown <= 40)
								{
									if (axeArmorMPRegenCooldown > 0)
									{
										--axeArmorMPRegenCooldown;
									}
								}
								if (axeArmorSuperMPRegenCooldown <= 60)
								{
									if (sotnApi.AlucardApi.State == 41 || sotnApi.AlucardApi.State == 42)
									{
										axeArmorSuperMPRegenCooldown = 60;
									}
									if (axeArmorMPRegenCooldown > 0)
									{
										--axeArmorMPRegenCooldown;
									}
								}
							}
						}
					}
					else
					{
						axeArmorSuperMPRegenCooldown = axeArmorSuperMPRegenMaxCooldown; //80
					}

					if (hasHealingMail && sotnApi.AlucardApi.HasControl() && !sotnApi.GameApi.IsInMenu() && (sotnApi.AlucardApi.State == 41 || sotnApi.AlucardApi.State == 42 || sotnApi.AlucardApi.State == 43))
					{
						if (axeArmorHPRegenCooldown == 0 && sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp)
						{
							sotnApi.AlucardApi.CurrentHp += 1;
							axeArmorHPRegenCooldown = axeArmorHPRegenMaxCooldown;
						}
						else if (axeArmorHPRegenCooldown > 0)
						{
							--axeArmorHPRegenCooldown;
						}
					}

					if (!sotnApi.GameApi.IsInMenu())
					{
						if (!hasAxeArmorStoredResources && axeArmorDelayedMPRegenDuration > 0)
						{
							if (currentMP < sotnApi.AlucardApi.MaxtMp * 2)
							{
								currentMP += 1;
								sotnApi.AlucardApi.CurrentMp += 1;
							}
							--axeArmorDelayedMPRegenDuration;
							++axeArmorMPRegenCooldown;
						}
						if (!hasAxeArmorStoredResources && axeArmorDelayedHeartRegenDuration > 0)
						{
							if (sotnApi.AlucardApi.CurrentHearts < sotnApi.AlucardApi.MaxtHearts)
							{
								sotnApi.AlucardApi.CurrentHearts += 1;
							}
							--axeArmorDelayedHeartRegenDuration;
						}
						if (axeArmorDelayedHPRegenDuration > 0)
						{
							if (sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp)
							{
								sotnApi.AlucardApi.CurrentHp += 1;
								--axeArmorDelayedHPRegenDuration;
							}
						}

						if (sotnApi.AlucardApi.State != 43)
						{
							axeArmorContactDamage = 0;
							axeArmorContactDamageCooldown = 0;

							if (axeArmorMPRegenCooldown == 0)
							{
								if (hasAxeArmorStoredResources)
								{
									if (storedAxeArmorMP < storedAxeArmorMaxMP)
									{
										storedAxeArmorMP += 1;
									}
								}
								else
								{
									if (currentMP < sotnApi.AlucardApi.MaxtMp)
									{
										currentMP += 1;
										sotnApi.AlucardApi.CurrentMp += 1;
									}
								}

								axeArmorMPRegenCooldown = axeArmorMPRegenMaxCooldown;

								if (Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Mystic Pendant")
								{
									axeArmorMPRegenCooldown -= 3;
								}
								if (Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Mystic Pendant")
								{
									axeArmorMPRegenCooldown -= 3;
								}
							}
							else
							{
								--axeArmorMPRegenCooldown;
							}
							if (sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad) || sotnApi.AlucardApi.HasRelic(Relic.CubeOfZoe))
							{
								if (axeArmorHeartsRegenCooldown == 0)
								{
									if (hasAxeArmorStoredResources)
									{
										if (storedAxeArmorHearts < storedAxeArmorMaxHearts)
										{
											storedAxeArmorHearts += 1;
										}
									}
									else
									{
										if (currentHearts < sotnApi.AlucardApi.MaxtHearts)
										{
											currentHearts += 1;
											sotnApi.AlucardApi.CurrentHearts += 1;
										}
									}


									axeArmorHeartsRegenCooldown = 90;
									if (sotnApi.AlucardApi.HasRelic(Relic.HeartOfVlad) && sotnApi.AlucardApi.HasRelic(Relic.CubeOfZoe))
									{
										axeArmorHeartsRegenCooldown = 60;
									}
								}
								else
								{
									--axeArmorHeartsRegenCooldown;
								}
							}
						}
					}
				}

				if (sotnApi.GameApi.CanSave() && sotnApi.AlucardApi.ScreenX > 90 && sotnApi.AlucardApi.ScreenX < 150)
				{
					if (hasAxeArmorStoredResources)
					{
						storedAxeArmorMP = sotnApi.AlucardApi.MaxtMp;
						sotnApi.AlucardApi.CurrentHearts = storedAxeArmorHearts;
						sotnApi.AlucardApi.LeftHand = storedLeftHand;
						sotnApi.AlucardApi.RightHand = storedRightHand;
					}

					if (sotnApi.AlucardApi.State == 0 && sotnApi.AlucardApi.Action == 1)
					{
						sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
					}
				}
				else if (hasAxeArmorStoredResources)
				{
					if (sotnApi.AlucardApi.CurrentMp > 0)
					{
						sotnApi.AlucardApi.CurrentMp = 1;
					}
					if (sotnApi.AlucardApi.CurrentHearts > 0)
					{
						sotnApi.AlucardApi.CurrentHearts = 0;
					}
				}
				
				if (sotnApi.GameApi.CanSave() && !isAxeArmorSave
					&& sotnApi.AlucardApi.MapX < 133 && sotnApi.AlucardApi.MapX > 129
					)
				{
					isAxeArmorSave = true;
					state.PokeValue(0);
					state.Enable();
					action.PokeValue(1);
					action.Enable();
				}
				else if (isAxeArmorSave)
				{
					state.Disable();
					action.Disable();
					isAxeArmorSave = false;
				}

				if (sotnApi.AlucardApi.HasControl() && !sotnApi.GameApi.IsInMenu()
					&& sotnApi.AlucardApi.State != 50
					&& sotnApi.AlucardApi.State != 43
					&& sotnApi.AlucardApi.State != 10
					&& sotnApi.AlucardApi.State != 12
					&& sotnApi.AlucardApi.CurrentHp > 0)
				{
					int meleeDamage = 0; //AxeArmor Damage
					int meleeDamageTotal = 0; //AxeArmor Calc Damage
					sotnApi.AlucardApi.ContactDamage = 0;
					axeArmorHorizontalSpeed.Disable();

					CheckAxeArmorHeldItems(pressSquareCDFrames, pressCircleCDFrames, heldUp);

					if (sotnApi.AlucardApi.HasRelic(Relic.LeapStone))
					{
						jumpBoostStocksMax = 3;
					}
					else
					{
						jumpBoostStocksMax = 2;
					}

					bool releasedJump = false;

					if (!inputService.ButtonHeld(PlaystationInputKeys.Cross))
					{
						releasedJump = true;
						jumpBoostAllowed = true;
						jumpBoostActive = false;
						jumpBoostCooldown = 0;

						if (sotnApi.AlucardApi.HasRelic(Relic.LeapStone)
							|| sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf)
							|| sotnApi.AlucardApi.HasRelic(Relic.FormOfMist)
							|| isAxeArmorThrustAttack)
						{
							gravityJumpCooldown = gravityJumpCooldownBase; // 8 * 8
						}

						minAirTime = minAirTimeBase; // 2 * 8
						if (!invincibilityLocked)
						{
							toggleHurtBox.PokeValue(0);
							toggleHurtBox.Disable();
						}
					}

					if (sotnApi.AlucardApi.State == 40 || sotnApi.AlucardApi.State == 41)
					{
						flightMaxDuration = mistFlightBaseDuration;
						mistBoostMaxDuration = mistBoostBaseDuration;
						mistTechDuration = mistTechBaseDuration;
						mistJumpCancelAllowed = false;

						if (hasPowerOfMist)
						{
							mistBoostMaxDuration += mistBoostPoM; //30
							mistTechDuration += mistTechBaseDuration;
						}
						else if (hasFormOfMist)
						{
							mistBoostMaxDuration += mistBoostFoM; //15
						}
						if (hasSoulOfWolf)
						{
							mistBoostMaxDuration += mistBoostWolfBonus; //3
						}
						if (hasSkillOfWolf)
						{
							mistBoostMaxDuration += mistBoostWolfBonus; //3
						}
						if (hasPowerOfWolf)
						{
							mistBoostMaxDuration += mistBoostWolfBonus; //3
						}


						if (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
						{
							mistFlightDuration = mistFlightMaxDuration;
							isAxeArmorVClipAllowed = true;
							isAxeArmorHClipAllowed = true;
						}
						else if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) || sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
						{
							mistFlightDuration = mistFlightMaxDuration;
							isAxeArmorVClipAllowed = true;
							isAxeArmorHClipAllowed = false;
						}
						else
						{
							mistFlightDuration = 0;
							isAxeArmorVClipAllowed = false;
							isAxeArmorHClipAllowed = false;
						}

						flightDuration = flightMaxDuration;
						jumpBoostStocks = jumpBoostStocksMax;
						gravityJumpCooldown = gravityJumpCooldownBase;
						gravityJumpAllowed = true;
						isAxeArmorGravityJump = false;
						isAxeArmorBat = false;
						isAxeArmorMist = false;
						isAxeArmorMistFlight = false;
						mistBoostResetLocked = false;

						axeArmorHorizontalJump.PokeValue(Constants.Khaos.AxeArmorBaseJumpSpeed);
						if (!invincibilityLocked)
						{
							toggleHurtBox.PokeValue(0);
							toggleHurtBox.Disable();
							contactDamage.PokeValue(0);
							contactDamage.Disable();
						}
					}
					if (sotnApi.AlucardApi.State == 42)
					{
						if (minAirTime > -1)
						{
							minAirTime -= 1;
						}
						if(axeArmorThrust && sotnApi.AlucardApi.Action == 1)
						{
							isAxeArmorThrustAttack = true;
							wolfRunActive = true;
							thrustDuration = thrustMaxDuration;
							if(wolfCurrentSpeed < Constants.Khaos.AxeArmorWolfMinRunSpeed)
							{
								if (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf))
								{
									wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMaxRunSpeed;
								}
								else
								{
									wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMinRunSpeed;
								}
							}
						}
					}
					else if (wolfStrikeBoost)
					{
						minAirTime = 0;
					}
					else
					{
						minAirTime = minAirTimeBase;
					}

					if((pressUpCDFrames|| heldUp)
						&& !IsInRoomList(Constants.Khaos.ShopRoom)
						&& !IsInRoomList(Constants.Khaos.AxeArmorPreventFormShiftRooms)
						&& !pressLeft32
						&& !pressRight32
						&& !pressSquare32
						&& !pressTriangle32
						&& !pressCircle32
						&& !pressCross32
						&& !pressL1_32
						&& !pressR1_32
						&& !pressR2_32
						&& sotnApi.AlucardApi.HorizontalVelocityWhole == 0
						&& sotnApi.AlucardApi.CurrentHp > 0
						&& sotnApi.AlucardApi.State != 42
						&& (sotnApi.AlucardApi.Action == 0 || sotnApi.AlucardApi.Action == 2)) 
					{
						if (minHoldUpTime == 0)
						{

							if (!sotnApi.GameApi.CanSave())
							{
								smoothCrouch.Enable();
							}
							if (!hasAxeArmorStoredResources)
							{
								AxeArmorTakeResources();
								storedResourceDelay = storedResourceDelayBase;
								axeArmorInvinCooldown = axeArmorInvinCooldownBase;
							}
							if (axeArmorCrouchHP > sotnApi.AlucardApi.CurrentHp)
							{
								axeArmorInvinCooldown = axeArmorInvinCooldownBase * 2;
							}
							axeArmorCrouchHP = sotnApi.AlucardApi.CurrentHp;
							isHoldUp = true;
							if(sotnApi.AlucardApi.CurrentHp <= 0)
							{
								AxeArmorDisableStateWhenDead();
							}
							else
							{
								if (!alucardSecondCastle && sotnApi.AlucardApi.ScreenY == 136 && sotnApi.AlucardApi.MapX == 31 && sotnApi.AlucardApi.MapY == 8)
								{
									state.PokeValue(18);
									action.PokeValue(2);
								}
								else if (alucardSecondCastle && sotnApi.AlucardApi.ScreenY == 136 && sotnApi.AlucardApi.MapX == 32 && sotnApi.AlucardApi.MapY == 56)
								{
									state.PokeValue(18);
									action.PokeValue(4);
								}
								else if (sotnApi.GameApi.CanSave())
								{
									state.PokeValue(0);
									action.PokeValue(0);
								}
								else
								{
									state.PokeValue(1);
									action.PokeValue(0);
								}
								characterData.Enable();
								state.Enable();
								action.Enable();
							}					
						}
						else
						{
							if (minHoldUpTime > 0)
							{
								minHoldUpTime -= 1;
							}
							axeArmorHoldUpYPosition = sotnApi.AlucardApi.ScreenY;
						}
					}
					else if ((pressDownCDFrames || heldDown)
						&& !IsInRoomList(Constants.Khaos.ShopRoom)
						&& !IsInRoomList(Constants.Khaos.AxeArmorPreventFormShiftRooms)
						&& !pressLeft32
						&& !pressRight32
						&& !pressSquare32
						&& !pressTriangle32
						&& !pressCircle32
						&& !pressCross32
						&& !pressL1_32
						&& !pressR1_32
						&& !pressR2_32
						&& sotnApi.AlucardApi.HorizontalVelocityWhole == 0
						&& sotnApi.AlucardApi.CurrentHp > 0
						&& sotnApi.AlucardApi.State != 42
						&& (sotnApi.AlucardApi.Action == 0 || sotnApi.AlucardApi.Action == 1))
					{
						if (minHoldDownTime == 0)
						{
							if (!sotnApi.GameApi.CanSave())
							{
								smoothCrouch.Enable();
							}
							if (!hasAxeArmorStoredResources)
							{
								AxeArmorTakeResources();
								storedResourceDelay = storedResourceDelayBase;
								axeArmorInvinCooldown = axeArmorInvinCooldownBase;
							}
							if (sotnApi.AlucardApi.CurrentHp <= 0)
							{
								AxeArmorDisableStateWhenDead();
							}
							else
							{
								if (axeArmorCrouchHP > sotnApi.AlucardApi.CurrentHp)
								{
									axeArmorInvinCooldown = axeArmorInvinCooldownBase * 2;
								}
								axeArmorCrouchHP = sotnApi.AlucardApi.CurrentHp;
								characterData.Enable();

								state.PokeValue(2);
								state.Enable();
								action.PokeValue(1);
								action.Enable();
								alucardHurtboxY.Enable();
							}
						}
						else
						{
							if (minHoldDownTime > 0)
							{
								minHoldDownTime -= 1;
							}
							axeArmorHoldDownYPosition = sotnApi.AlucardApi.ScreenY;
						}
					}
					else
					{
						axeArmorCrouchHP = 0;
						characterData.Disable();
						state.PokeValue(40);
						state.Disable();
						action.PokeValue(0);
						action.Disable();
						alucardHurtboxY.Disable();
						CheckSmoothCrouch();
						if (isHoldUp 
							&& (pressL1_CDFrames || pressUpCDFrames)
						)
						{
							facePlantCooldown = 1;
							isHoldUp = false;
						}
						axeArmorHoldUpYPosition = 0;
						axeArmorHoldDownYPosition = 0;
						if(storedResourceDelay > 0)
						{
							--storedResourceDelay;
						}
						else if (hasAxeArmorStoredResources)
						{
							AxeArmorReturnResources();
						}
					}

					if (pressSquareCDFrames || pressTriangleCDFrames || pressCircleCDFrames)
					{
						axeArmorInvinCooldown = 0;
					}

					if (axeArmorInvinCooldown > 0 &&
						((isAxeArmorGravityJump && gravityJumpCooldown == 0)
						|| pressCrossCDFrames
						|| sotnApi.AlucardApi.State < 40))
					{
						--axeArmorInvinCooldown;
						if (!invincibilityLocked)
						{
							toggleHurtBox.PokeValue(1);
							toggleHurtBox.Enable();
						}
					}
					else
					{
						toggleHurtBox.PokeValue(0);
						toggleHurtBox.Disable();
					}

					if (sotnApi.AlucardApi.HasRelic(Relic.GravityBoots) && pressR2_CDFrames)
					{
						if (!isAxeArmorGravityJump
							&& gravityJumpAllowed
							&& (pressCrossCDFrames || heldCross)
							&& pressUpCDFrames
							&& currentMP > 3
						)
						{
							axeArmorVerticalJumpMoving.PokeValue(Constants.Khaos.AxeArmorGravityJumpAirControl);
							axeArmorVerticalJumpStationary.PokeValue(Constants.Khaos.AxeArmorGravityJumpAirControl);
							jumpBoostActive = false;
							if (underwaterActive)
							{
								jumpBoostCooldown = jumpBoostCooldownGravityBase * 2; // 8 * 8 * 2
							}
							else
							{
								jumpBoostCooldown = jumpBoostCooldownGravityBase; // 8 * 8
							}
							jumpBoostAllowed = false;
							isAxeArmorGravityJump = true;
							isAxeArmorBat = false;
							if (!invincibilityLocked)
							{
								toggleHurtBox.PokeValue(1);
								toggleHurtBox.Enable();
							}

							if (sotnApi.AlucardApi.HasRelic(Relic.LeapStone) 
								|| sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) 
								|| sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
							{
								gravityJumpAllowed = true;
							}
							else
							{
								gravityJumpAllowed = false;
							}

							axeArmorInvinCooldown = axeArmorInvinCooldownBase;
							axeArmorHorizontalJump.PokeValue(Constants.Khaos.AxeArmorMaxJumpSpeed);
							axeArmorHorizontalJump.Enable();
						}
						else if (!isAxeArmorGravityJump)
						{
							axeArmorVerticalJumpMoving.PokeValue(Constants.Khaos.AxeArmorVerticalJumpAirControl);
							axeArmorVerticalJumpStationary.PokeValue(Constants.Khaos.AxeArmorVerticalJumpAirControl);
							if (underwaterActive)
							{
								axeArmorHorizontalJump.PokeValue(Constants.Khaos.AxeArmorVanillaJumpSpeed);
							}
							else
							{
								axeArmorHorizontalJump.PokeValue(Constants.Khaos.AxeArmorBaseJumpSpeed);
							}
							axeArmorHorizontalJump.Enable();
						}
					}
					else
					{
						isAxeArmorBat = false;
						axeArmorVerticalJumpMoving.PokeValue(Constants.Khaos.AxeArmorVerticalJumpAirControl);
						axeArmorVerticalJumpStationary.PokeValue(Constants.Khaos.AxeArmorVerticalJumpAirControl);
						isAxeArmorGravityJump = false;
						if (underwaterActive)
						{
							axeArmorHorizontalJump.PokeValue(Constants.Khaos.AxeArmorVanillaJumpSpeed);
						}
						else
						{
							axeArmorHorizontalJump.PokeValue(Constants.Khaos.AxeArmorBaseJumpSpeed);
						}
						
						axeArmorHorizontalJump.Enable();
					}

					if (facePlantCooldown == 0 && (currentMP > 4)
						&& !sotnApi.GameApi.IsInMenu()
						&& pressL1_CDFrames
						&& sotnApi.AlucardApi.Action == 0)
					{   //Invul / Slow Fall / Faceplant
						jumpBoostHover = 0;
						isAxeArmorMist = true;
						isAxeArmorBat = false;
						if (bloodStoneCount > 0)
						{
							AxeArmorStatRestore(bloodStoneCount, true, false, false, false);
						}

						glideMPCooldown = glideMPMaxCooldown;
						sotnApi.AlucardApi.CurrentMp -= 4;
						currentMP -= 4;

						if (sotnApi.AlucardApi.State == 41)
						{
							isAerialFacePlant = false;
						}
						else
						{
							isAerialFacePlant = true;
						}
						facePlantCooldown = facePlantCooldownBase;
						faceplantSpellCooldown = faceplantMaxSpellCooldown;
						faceplantAttackCooldown = faceplantMaxAttackCooldown;
						if (!mistBoostResetLocked)
						{
							mistBoostDuration = mistBoostMaxDuration;
						}	
						sotnApi.AlucardApi.State = 43;
						axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorFloatBaseSpeed);
						axeArmorFloat.Enable();
					}
					else if (isAxeArmorGravityJump
						&& currentMP > 3
						&& gravityJumpCooldown > 0
						&& pressCrossCDFrames
						&& heldCross)
					{
						isAxeArmorBat = false;
						if (!sotnApi.GameApi.InTransition)
						{
							if (gravityJumpCooldown == gravityJumpMPCheck)
							{
								if (!releasedJump)
								{
									sotnApi.AlucardApi.CurrentMp -= 4;
									currentMP -= 4;
								}
							}
							if (gravityJumpCooldown > 0)
							{
								--gravityJumpCooldown;
								jumpBoostHover = 0;
								axeArmorInvinCooldown = axeArmorInvinCooldownBase;
								if (!invincibilityLocked)
								{
									toggleHurtBox.PokeValue(1);
									toggleHurtBox.Enable();
								}
							}

							int jumpHeight = 0;

							if (sotnApi.AlucardApi.ScreenY < 50)
							{
								if (sotnApi.AlucardApi.ScreenY < 25)
								{
									jumpHeight = Constants.Khaos.AxeArmorJumpBaseSpeed - (Constants.Khaos.AxeArmorGravityJumpAcceleration);
								}
								else
								{
									jumpHeight = Constants.Khaos.AxeArmorGravityJumpBaseSpeed + (((40 - gravityJumpCooldown) / 8) * Constants.Khaos.AxeArmorGravityJumpAcceleration);
									//jumpHeight = Constants.Khaos.AxeArmorGravityJumpBaseSpeed + ((5 - gravityJumpCooldown) * Constants.Khaos.AxeArmorGravityJumpAcceleration);
								}
							}
							else
							{
								jumpHeight = Constants.Khaos.AxeArmorGravityJumpBaseSpeed - (Constants.Khaos.AxeArmorGravityJumpAcceleration * (gravityJumpCooldown / 8));
							}

							axeArmorFloat.PokeValue(jumpHeight);
							axeArmorFloat.Enable();
						}
					}
					else if (flightDuration > 0 && currentMP > 2 && pressR1_CDFrames)
					{   //Flight / Glide
						jumpBoostHover = 0;
						if (!sotnApi.GameApi.InTransition)
						{
							if (!sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
							{
								--flightDuration;
								if(axeArmorFrameCount == Globals.UpdateCooldownFrames)
								{
									--flightMPCooldown;
									--flightMPCooldown;
									--flightMPCooldown;
								}
							}

							if (sotnApi.AlucardApi.State == 41 || sotnApi.AlucardApi.State == 40)
							{
								if (flightMPCooldown < 10)
								{
									flightMPCooldown = 10;
								}
							}
							else if (flightMPCooldown <= 0)
							{
								currentMP -= 3;
								sotnApi.AlucardApi.CurrentMp -= 3;
								flightMPCooldown = 10;
							}
							else if (axeArmorFrameCount == Globals.UpdateCooldownFrames)
							{
								--flightMPCooldown;
							}
						}

						if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
						{
							isAxeArmorBat = true;
							if (pressUpCDFrames)
							{
								axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorFlightUpSpeed);
								axeArmorFloat.Enable();
							}
							else if (pressDownCDFrames)
							{
								axeArmorFloat.Disable();
							}
							else
							{
								axeArmorFloat.PokeValue(0);
								axeArmorFloat.Enable();
							}
						}
						else
						{
							isAxeArmorBat = false;
							axeArmorFloat.PokeValue(0);
							axeArmorFloat.Enable();
						}
					}
					else if (jumpBoostAllowed && minAirTime == 0 && jumpBoostCooldown == 0 && jumpBoostStocks > 0
						&& (pressCrossCDFrames|| heldCross)
						&& (sotnApi.AlucardApi.State == 42 || wolfStrikeBoost)
						)
					{
						isAxeArmorBat = false;
						if (underwaterActive)
						{
							jumpBoostCooldown = jumpBoostCooldownGravityBase; // 8 * 8
						}
						else
						{
							jumpBoostCooldown = jumpBoostCooldownBase; // 3 * 8
						}
						jumpBoostCooldown += (jumpBoostStocks * (Globals.UpdateCooldownFrames - 4) * jumpBoostStocksMax);

						jumpBoostHover = jumpBoostHoverBase;
						jumpBoostActive = true;
						--jumpBoostStocks;
						jumpBoostAllowed = false;
						axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorJumpFloatBaseSpeed);
						axeArmorFloat.Enable();
						axeArmorHorizontalJump.PokeValue(Constants.Khaos.AxeArmorMaxJumpSpeed);
						axeArmorHorizontalJump.Enable();

					}
					else if (jumpBoostActive && (heldCross || pressCrossCDFrames))
					{
						isAxeArmorBat = false;

						if (underwaterActive)
						{
							jumpBoostHover = (jumpBoostHoverBase * 2) - (7 - (jumpBoostStocks * 6)) - (jumpBoostCooldown / 2);
						}
						else
						{
							jumpBoostHover = jumpBoostHoverBase - (7 - (jumpBoostStocks * 6)) - (jumpBoostCooldown / 2);
						}
						if (jumpBoostCooldown == 0)
						{
							jumpBoostActive = false;
						}
						else
						{
							if (!sotnApi.GameApi.InTransition)
							{
								--jumpBoostCooldown;
							}
							
						}
						int jumpHeight = Constants.Khaos.AxeArmorJumpBaseSpeed + (Constants.Khaos.AxeArmorJumpAcceleration * (jumpBoostCooldown / 8));

						axeArmorFloat.PokeValue(jumpHeight);
						axeArmorFloat.Enable();
					}
					else if (jumpBoostHover > 0)
					{
						int jumpHeight = -18200 + 9000 - (jumpBoostHover * 300);
						--jumpBoostHover;

						axeArmorFloat.PokeValue(jumpHeight);
						axeArmorFloat.Enable();
					}
					else if (sotnApi.AlucardApi.State == 42 && underwaterActive)
					{
						axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorUnderwaterBaseSpeed);
						axeArmorFloat.Enable();
					}
					else
					{
						isAxeArmorBat = false;
						axeArmorFloat.Disable();
					}

					if (sotnApi.AlucardApi.State == 41)
					{
						if (pressR2_10 && (heldLeft || heldRight)
							&& !wolfDashActive
							&& !wolfRunActive
							)
						{
							if (!wolfDashActive && sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf))
							{
								wolfCurrentSpeed = 0;
								wolfRunActive = false;
								wolfDashActive = true;
							}
							else if (!wolfRunActive && !wolfDashActive)
							{
								wolfRunActive = true;
							}

							if (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf))
							{
								wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMaxRunSpeed;
							}
							else
							{
								wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMinRunSpeed;
							}
						}
					}

					if (isAlucardColorSecondCastle
						&& sotnApi.AlucardApi.MapX == 31
						&& sotnApi.AlucardApi.MapY == 36
						&& sotnApi.AlucardApi.ScreenY <= 116)
					{
						wolfDashActive = false;
						wolfRunActive = false;
						wolfCurrentSpeed = 0;
					}
					else if ((wolfDashActive || wolfRunActive))
					{
						int acceleration = sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) == true ? Constants.Khaos.AxeArmorWolfMaxAcceleration : Constants.Khaos.AxeArmorWolfBaseAcceleration;
						int deacceleration = sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) == true ? Constants.Khaos.AxeArmorWolfMaxDeacceleration : Constants.Khaos.AxeArmorWolfBaseDeacceleration;
						int maxRunSpeed = Constants.Khaos.AxeArmorWolfMaxRunSpeed;
						int minRunSpeed = Constants.Khaos.AxeArmorWolfMinRunSpeed;
						int maxDashSpeed = Constants.Khaos.AxeArmorWolfMaxDashSpeed;

						if (underwaterActive)
						{
							if (toolConfig.Khaos.NerfUnderwater)
							{
								if (superUnderwater)
								{
									maxRunSpeed = (int) (maxRunSpeed * toolConfig.Khaos.UnderwaterFactor);
									maxDashSpeed = (int) (maxRunSpeed * toolConfig.Khaos.UnderwaterFactor);
								}	
							}
							else
							{
								if (superUnderwater)
								{
									maxRunSpeed = (int) (maxRunSpeed * toolConfig.Khaos.UnderwaterFactor * Constants.Khaos.SuperUnderwaterFactor);
									maxDashSpeed = (int) (maxRunSpeed * toolConfig.Khaos.UnderwaterFactor * Constants.Khaos.SuperUnderwaterFactor);
								}
								else
								{
									maxRunSpeed = (int) (maxRunSpeed * toolConfig.Khaos.UnderwaterFactor);
									maxDashSpeed = (int) (maxRunSpeed * toolConfig.Khaos.UnderwaterFactor);
								}
							}
							if (maxRunSpeed < minRunSpeed)
							{
								maxRunSpeed = minRunSpeed + 40000;
							}
							if (maxDashSpeed < maxRunSpeed)
							{
								maxDashSpeed = minRunSpeed + 40000;
							}

							float underwaterAxeArmorBaseFactor;
							float underwaterAxeArmorSuperFactor;
							float underwaterAxeArmorMayhemFactor = 1F;

							if (toolConfig.Khaos.NerfUnderwater)
							{
								underwaterAxeArmorBaseFactor = 1F;
								underwaterAxeArmorSuperFactor = toolConfig.Khaos.UnderwaterFactor;
							}
							else
							{
								underwaterAxeArmorBaseFactor = toolConfig.Khaos.UnderwaterFactor;
								underwaterAxeArmorSuperFactor = Constants.Khaos.SuperUnderwaterFactor;
							}

							if (superUnderwater)
							{
								underwaterAxeArmorMayhemFactor = Constants.Khaos.SuperUnderwaterFactor;
							}

							float underwaterAxeArmorModifier = underwaterAxeArmorBaseFactor * underwaterAxeArmorSuperFactor * underwaterAxeArmorMayhemFactor;

							acceleration = (int)((acceleration * underwaterAxeArmorModifier) / Globals.UpdateCooldownFrames);
							deacceleration = (int)((acceleration * underwaterAxeArmorModifier) / Globals.UpdateCooldownFrames);
						}
						if (speedActive)
						{
							
							if (superSpeed)
							{
								acceleration *= 3;
								maxRunSpeed = Constants.Khaos.AxeArmorWolfMaxMayhemSuperSpeed;
							}
							else
							{
								acceleration *= 2;
								maxRunSpeed = Constants.Khaos.AxeArmorWolfMaxMayhemSpeed;
							}
						}

						int facingModifier = 1;
						int speedModifier = 1;
						int wolfSpeedAdjustment = 0;
						bool hasSkillOfWolf = sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf);
						bool enableAxeWolfDamage = true;
						bool gradualStop = false;
						bool instantStop = false;

						if (sotnApi.AlucardApi.FacingLeft)
						{
							facingModifier = -1;
						}
						if (isAxeArmorThrustAttack)
						{
							speedModifier = -3;
						}
						if (!heldR2 && (sotnApi.AlucardApi.State == 40 || (sotnApi.AlucardApi.State == 41 && !isAxeArmorThrustAttack)))
						{
							instantStop = true;
							wolfCurrentSpeed = 0;
						}
						else if (sotnApi.AlucardApi.State == 42 && heldR1)
						{
							if ((!isAxeArmorThrustAttack && hasSkillOfWolf) || (isAxeArmorThrustAttack && !hasSkillOfWolf))
							{
								wolfSpeedAdjustment -= (Constants.Khaos.AxeArmorWolfBaseDeacceleration * 2 * speedModifier);
							}
							else
							{
								wolfSpeedAdjustment -= (Constants.Khaos.AxeArmorWolfBaseDeacceleration * 3 * speedModifier);
							}
						}
						else if (sotnApi.AlucardApi.State == 42)
						{
							if (speedActive)
							{
								wolfSpeedAdjustment -= Constants.Khaos.AxeArmorWolfBaseDeacceleration * 3 * speedModifier;
							}
							else if (!jumpBoostActive && !isAxeArmorGravityJump)
							{
								if ((!isAxeArmorThrustAttack && hasSkillOfWolf) || (isAxeArmorThrustAttack && !hasSkillOfWolf))
								{
									wolfSpeedAdjustment -= Constants.Khaos.AxeArmorWolfBaseDeacceleration * 5 * speedModifier;
								}
								else
								{
									wolfSpeedAdjustment -= Constants.Khaos.AxeArmorWolfBaseDeacceleration * 7 * speedModifier;
								}
							}
							else
							{
								if ((!isAxeArmorThrustAttack && hasSkillOfWolf) || (isAxeArmorThrustAttack && !hasSkillOfWolf))
								{
									wolfSpeedAdjustment -= Constants.Khaos.AxeArmorWolfBaseDeacceleration * 6 * speedModifier;
								}
								else
								{
									wolfSpeedAdjustment -= Constants.Khaos.AxeArmorWolfBaseDeacceleration * 9 * speedModifier;
								}
							}
						}
						else if (sotnApi.AlucardApi.State == 43)
						{
							if (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf))
							{
								wolfSpeedAdjustment -= Constants.Khaos.AxeArmorWolfBaseDeacceleration * 12;
							}
							else
							{
								wolfSpeedAdjustment -= Constants.Khaos.AxeArmorWolfBaseDeacceleration * 18;
							}
						}
						else if (pressR2_CDFrames && (pressLeftCDFrames || pressRightCDFrames)
							&& sotnApi.AlucardApi.State == 41)
						{
							if (wolfDashActive)
							{
								acceleration *= 2;
							}
							wolfSpeedAdjustment += acceleration;
						}
						else if ((sotnApi.AlucardApi.State == 40 && sotnApi.AlucardApi.Action == 1))
						{
							if (inputService.ButtonHeld(PlaystationInputKeys.Down))
							{
								wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMinRunSpeed;
							}
							else
							{
								if (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf) && wolfCurrentSpeed < Constants.Khaos.AxeArmorWolfMaxRunSpeed - (acceleration * 4))
								{
									wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMaxRunSpeed;
								}
								else if (wolfCurrentSpeed < Constants.Khaos.AxeArmorWolfMinRunSpeed - (acceleration * 4))
								{
									wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMinRunSpeed;
								}

								if (wolfDashActive)
								{
									wolfSpeedAdjustment += acceleration * 3;
								}
								wolfSpeedAdjustment += acceleration * 3;
							}

							wolfStrikeBoost = true;
						}
						else if (wolfStrikeBoost || pressL1_16)
						{
							wolfSpeedAdjustment += acceleration / 2;
							wolfStrikeBoost = false;
						}
						else if (pressL1_60 || pressR1_60 || pressCross60)
						{ 
							//Handle gradual speed reduction
							if (pressL1_32 || pressR1_32 || pressCross32)
							{
								wolfSpeedAdjustment -= 15 * deacceleration;
								if (wolfDashActive)
								{
									wolfSpeedAdjustment -= 5 * deacceleration;
								}
							}
							else
							{
								deacceleration *= 40;
								wolfSpeedAdjustment -= deacceleration;
							}
							if (wolfCurrentSpeed < Constants.Khaos.AxeArmorWolfMinStopSpeed)
							{
								if (wolfCurrentSpeed < 1000)
								{
									instantStop = true;
								}
								else
								{
									gradualStop = true;
								}
							}
						}
						else
						{
							wolfCurrentSpeed = 0;
							instantStop = true;
						}

						//Frame adjustment
						wolfCurrentSpeed += (int) (wolfSpeedAdjustment / Globals.UpdateCooldownFrames);

						if (wolfDashActive && wolfCurrentSpeed > maxDashSpeed)
						{
							wolfCurrentSpeed = maxDashSpeed;
						}
						else if (wolfRunActive && wolfCurrentSpeed > maxRunSpeed)
						{
							wolfCurrentSpeed = maxRunSpeed;
						}
						else if (wolfCurrentSpeed < minRunSpeed)
						{
							enableAxeWolfDamage = false;
						}

						if (enableAxeWolfDamage)
						{
							if (!inputService.ButtonPressed(PlaystationInputKeys.R1, Globals.UpdateCooldownFrames))
							{
								if (sotnApi.AlucardApi.State != 42)
								{
									if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf))
									{
										meleeDamage += 20;
									}
									if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf))
									{
										meleeDamage += 5;
									}
								}
								if (sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf))
								{
									meleeDamage += 5;
								}
							}

							if (isAxeArmorMist)
							{
								//Do nothing?
								//(sotnApi.AlucardApi.State == 42 && !hasSkillOfWolf)
							}
							else
							{
								axeArmorHorizontalSpeed.Enable();
								axeArmorHorizontalSpeed.PokeValue(wolfCurrentSpeed * facingModifier);
							}
						}
						else
						{
							wolfStrikeBoost = false;
							if (gradualStop || instantStop)
							{
								if (gradualStop)
								{
									axeArmorHorizontalSpeed.PokeValue(1000 * facingModifier);
								}
								else if (instantStop)
								{
									axeArmorHorizontalSpeed.PokeValue(0);
								}
								axeArmorHorizontalSpeed.Enable();
								wolfCurrentSpeed = 0;
								wolfDashActive = false;
								wolfRunActive = false;
							}
							else if (wolfCurrentSpeed > Constants.Khaos.AxeArmorWolfMinStopSpeed)
							{
								axeArmorHorizontalSpeed.Enable();
								axeArmorHorizontalSpeed.PokeValue(wolfCurrentSpeed * facingModifier);
							}
							else
							{
								axeArmorHorizontalSpeed.PokeValue(0);
								axeArmorHorizontalSpeed.Disable();
								wolfCurrentSpeed = 0;
								wolfDashActive = false;
								wolfRunActive = false;
							}
						}
					}

					/*
					if (currentMP > 4 && !spiritActive && inputService.ButtonPressed(PlaystationInputKeys.L2, 10))
					{
						//sotnApi.AlucardApi.CurrentMp -= 5;
						var spiritAddress = sotnApi.EntityApi.FindEntityFrom(new List<SearchableActor> { Constants.Khaos.SpiritActor }, false);
						if (spiritAddress > 0)
						{
							LiveEntity liveSpirit = sotnApi.EntityApi.GetLiveEntity(spiritAddress);
							if (liveSpirit.LockOn == Entities.LockedOn)
							{
								liveSpirit.Palette = Constants.Khaos.SpiritPalette;
								liveSpirit.InvincibilityFrames = 4;
								spiritActive = true;
								cheats.AddCheat(spiritAddress + Entities.LockOnOffset, Entities.LockedOn, Constants.Khaos.SpiritLockOnName, WatchSize.Byte);
								spiritTimer.Start();
							}
						}
					}
					*/

					UpdateAxeArmorHeartCooldowns();

					if (heartGlobalCooldown < 1 && pressCircleCDFrames)
					{
						AxeArmorHeartActionCheck();
					}

					if (((currentMP > 23) || (hasHolySymbol && currentMP > 19))
						&& !fireBallActive
						&& !heartsOnlyActive
						&& pressTriangleCDFrames
						&& !heldR1
						&& (sotnApi.AlucardApi.State == 40 || sotnApi.AlucardApi.State == 41)
						&& facePlantCooldown == 0
						&& axeArmorSafetyCooldown == 0
						&& spellCooldown == 0
						)
					{

						if (axeArmorGurthang)
						{
							gurthangBoostTimer = gurthangBoostMaxTime;
						}

						if (bloodStoneCount > 0)
						{
							AxeArmorStatRestore(bloodStoneCount * 6, true, false, false, false);
						}

						if (hasHolySymbol)
						{
							sotnApi.AlucardApi.CurrentMp -= 20;
						}
						else
						{
							sotnApi.AlucardApi.CurrentMp -= 24;
						}

						int fireBallCount = 2;

						bool alucardFacing = sotnApi.AlucardApi.FacingLeft;
						long address = 0;
						int offsetX = alucardFacing ? -20 : 20;

						short fireBallDamage = 0;
						short calculatedFireBallDamage = 0;
						if (!heartsOnlyActive)
						{
							uint baseFireballDamage = 12;
							double baseINTMultiplier = .30;
							double equipINTMultiplier = .42;

							if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
							{
								baseINTMultiplier += .05;
								equipINTMultiplier += .07;
							}

							if (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat))
							{
								baseINTMultiplier += .05;
								equipINTMultiplier += .07;
							}
							if (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat))
							{
								baseINTMultiplier += .10;
								equipINTMultiplier += .14;
							}
							if (sotnApi.AlucardApi.HasRelic(Relic.ForceOfEcho))
							{
								baseINTMultiplier += .05;
								equipINTMultiplier += .07;
							}
							if (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad))
							{
								baseFireballDamage += 12;
							}
							if (hasMojoMail)
							{
								baseFireballDamage += 12;
							}
							calculatedFireBallDamage = (short) ((baseFireballDamage) + ((sotnApi.AlucardApi.Int) * baseINTMultiplier) + ((equipmentINT + +axeArmorShieldINT) * equipINTMultiplier));
							if (IsInRoomList(Constants.Khaos.GalamothRooms))
							{
								calculatedFireBallDamage = (short) (calculatedFireBallDamage / 3);
							}
						}

						while (fireBallCount > 0)
						{
							Entity fireball = new Entity(Constants.Khaos.DarkFireballEntityBytes);
							fireball.Xpos = (ushort) (sotnApi.AlucardApi.ScreenX + offsetX);

							if (!hellfirePaletteSet)
							{
								hellfirePalette = fireball.Palette;
								hellfirePaletteSet = true;
							}

							if (fireBallCount == 2)
							{
								fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY - 10);
							}
							else
							{
								fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY + 10);
							}

							fireball.SpeedHorizontal = alucardFacing ? (ushort) 0xFFFF : (ushort) 0;

							fireBallDamage = (short) (calculatedFireBallDamage);

							if (fireBallDamage < 1)
							{
								fireBallDamage = 1;
							}

							fireball.Damage = (ushort) fireBallDamage;

							if (pressUpCDFrames)
							{
								//Fire
								//fireball.DamageTypeA = 128;
								fireball.DamageTypeA = 64;
								fireball.DamageTypeB = 128;
								fireball.Palette = (ushort) (hellfirePalette + 12);
								// 1 = Curse
								// 2 = Petrify
								// 4 = Water
								// 8 = Dark
								// 16 = Holy
								// 32 = Ice
								// 64 = Cut if Damage Type A only
								// 100 = Lightning
								// 128 = Fire
								// 192 = Poison
								//Ice color  = (ushort)(hellfirePalette + 1);
								//Lightning color = (ushort) (hellfirePalette + 6);
								//Darkness color = (ushort) (hellfirePalette + 9);
								//Fire Color = (ushort) (hellfirePalette + 12);
								//Alternate Fire = fireball.Palette = (ushort)(hellfirePalette + 17);	
							}
							else if (pressDownCDFrames)
							{
								//Lightning
								fireball.DamageTypeA = (uint) 64;
								fireball.DamageTypeB = (uint) 64;
								fireball.Palette = (ushort) (hellfirePalette + 6);
							}
							else
							{
								//Ice
								//fireball.DamageTypeA = (uint) 64;
								fireball.DamageTypeA = (uint) 64;
								fireball.DamageTypeB = (uint) 32;
								fireball.Palette = (ushort) (hellfirePalette + 1);

							}

							address = sotnApi.EntityApi.SpawnEntity(fireball, false);
							LiveEntity liveFireball = sotnApi.EntityApi.GetLiveEntity(address);

							--fireBallCount;

						}
						spellCooldown = 4; // Original = 3
						fireBallActive = true;
						cheats.AddCheat(address + SotnApi.Constants.Values.Game.Entities.SpeedWholeOffset, alucardFacing ? (ushort) Constants.Khaos.FireBallSpeedLeft : (ushort) Constants.Khaos.FireBallSpeedRight, Constants.Khaos.FireBallSpeedName, WatchSize.Word);
						fireBallTimer.Start();
					}

					else if (((currentMP > 17) || (hasHolySymbol && currentMP > 13))
						&& pressTriangleCDFrames
						&& !heartsOnlyActive 
						&& (inputService.ButtonHeld(PlaystationInputKeys.R1) || sotnApi.AlucardApi.State == 42)
						&& fireballCooldown == 0
						&& facePlantCooldown == 0
						&& axeArmorSafetyCooldown == 0
						&& spellCooldown == 0)
					{

						if (axeArmorGurthang)
						{
							gurthangBoostTimer = gurthangBoostMaxTime;
						}

						if (hasHolySymbol)
						{
							sotnApi.AlucardApi.CurrentMp -= 14;
						}
						else
						{
							sotnApi.AlucardApi.CurrentMp -= 18;
						}

						int fireBallCount = 2;
						int fireBallStart = 10;
						uint secondaryDamageType = 16; //Holy

						if (bloodStoneCount > 0)
						{
							AxeArmorStatRestore(bloodStoneCount * 4, true, false, false, false);
						}

						if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
						{
							fireBallCount++;
						}
						if (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat))
						{
							fireBallCount++;
						}
						if (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat))
						{
							fireBallCount++;
						}
						if (sotnApi.AlucardApi.HasRelic(Relic.ForceOfEcho))
						{
							fireBallCount++;
							secondaryDamageType = 128; //Fire
						}

						if (fireBallCount > 2)
						{
							fireBallStart += 10;
						}

						while (fireBallCount > 0)
						{
							Entity fireball = new Entity(Constants.Khaos.FireballEntityBytes);
							bool alucardFacing = sotnApi.AlucardApi.FacingLeft;
							int offsetX = alucardFacing ? -20 : 20;

							if (!batFirePaletteSet)
							{
								batFirePalette = (ushort) (fireball.Palette + 5);
								//+5 Mostly white
								//+6 Alt Fire
								//+17 Petrify 
								//+21 Blue Fireball
								//+22 Alt Blue Fireball
								//+23 Holy Fire Ball
								//+25 Alt Dark Green
								//+26 Thunder Ball
								//+27 Thunder+Dark
								//+28 Alt All Red
								batFirePaletteSet = true;
							}

							if (secondaryDamageType == 0)
							{
								fireball.Palette = (ushort) (batFirePalette);
							}
							else
							{
								fireball.Palette = (ushort) (batFirePalette + 19);
							}

							fireball.DamageTypeA = secondaryDamageType; //Holy or Fire
							fireball.DamageTypeB = 16; //Holy

							fireball.Xpos = (ushort) (sotnApi.AlucardApi.ScreenX + offsetX);
							fireball.Ypos = (ushort) (sotnApi.AlucardApi.ScreenY + (fireBallStart - (fireBallCount * 10)));

							long address = sotnApi.EntityApi.SpawnEntity(fireball, false);
							LiveEntity liveFireball = sotnApi.EntityApi.GetLiveEntity(address);

							if (pressDownCDFrames)
							{
								fireballsDown.Add(liveFireball);
							}
							else if (pressUpCDFrames)
							{
								fireballsUp.Add(liveFireball);
							}
							else
							{
								fireballs.Add(liveFireball);
							}
							--fireBallCount;
						}

						//++batFirePalette;
						fireballCooldown = 9; //Original = 9
						spellCooldown = 4; //Original = 3
					}
					//Set Damage Calculation Last, account for underflow
					CheckAxeArmorAttack(false, meleeDamage, out meleeDamageTotal);
					
					if (meleeDamageTotal < -1 || heartsOnlyActive || rushDownActive)
					{
						meleeDamageTotal = -1;
					}
					axeArmorDamage.PokeValue(meleeDamageTotal);
				}
				else
				{
					CheckSmoothCrouch();

					if (!sotnApi.GameApi.IsInMenu() && sotnApi.AlucardApi.State == 43 && isAxeArmorMist && currentMP > 0)
					{
						bool applyContactDamage = false;
						uint contactDamage = 1;

						axeArmorContactDamageTimer.Start();
						if (!heldCross)
						{
							mistJumpCancelAllowed = true;
						}
						if (faceplantAttackCooldown > 0)
						{
							--faceplantAttackCooldown;
						}
						else
						{
							//Faceplant: Speed up Conversion / Mist Sustain
							if (pressSquareCDFrames
							&& !sotnApi.GameApi.InTransition
							&& sotnApi.AlucardApi.CurrentMp > 0
							&& ((heldR2 && ((wolfDashActive && (wolfCurrentSpeed < Constants.Khaos.AxeArmorWolfMaxDashSpeed)) || ((wolfCurrentSpeed < Constants.Khaos.AxeArmorWolfMaxRunSpeed) || (!wolfRunActive && !wolfDashActive))))
							|| (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist) && (mistBoostDuration < mistBoostMaxDuration))))
							{
								float faceplantRelicModifier = 1F;

								if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
								{
									--glideMPCooldown;
									faceplantRelicModifier = 2F;
									if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
								{
									if (mistBoostDuration < mistBoostMaxDuration * 5)
									{
										mistBoostDuration = mistBoostMaxDuration * 5;
									}
								}
							}
							else
							{
								--glideMPCooldown;
								--glideMPCooldown;
								if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
								{
									if (mistBoostDuration < mistBoostMaxDuration * 3)
									{
										mistBoostDuration = mistBoostMaxDuration * 3;
									}
								}
							}
							if (heldR2)
							{
								if (!wolfRunActive)
								{
									wolfRunActive = true;
								}
								if (!wolfDashActive && sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf))
								{
									wolfDashActive = true;
								}

								int faceplantAcceleration = Constants.Khaos.AxeArmorMistAcceleration;

								if (hasFormOfMist)
								{
									faceplantAcceleration += Constants.Khaos.AxeArmorMistAccelerationRelic;
								}
								if (hasPowerOfMist)
								{
									faceplantAcceleration += Constants.Khaos.AxeArmorMistAccelerationRelic;
								}

								float faceplantModifier = 1F;

								if (superUnderwater)
								{
									faceplantModifier = Constants.Khaos.SuperUnderwaterFactor;
								}

								else if (underwaterActive)
								{
									faceplantModifier = toolConfig.Khaos.UnderwaterFactor;
								}

								wolfCurrentSpeed += (int) (faceplantRelicModifier * faceplantAcceleration * faceplantModifier);


								if (superSpeed)
								{
									if (wolfCurrentSpeed > Constants.Khaos.AxeArmorWolfMaxMayhemSuperSpeed)
									{
										wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMaxMayhemSuperSpeed;
									}
								}
								else if (speedActive)
								{
									if (wolfCurrentSpeed > Constants.Khaos.AxeArmorWolfMaxMayhemSpeed)
									{
										wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMaxMayhemSpeed;
									}
								}
								else if (wolfDashActive)
								{
									if (wolfCurrentSpeed > Constants.Khaos.AxeArmorWolfMaxDashSpeed)
									{
										wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMaxDashSpeed;
									}
								}
								else if (wolfCurrentSpeed > Constants.Khaos.AxeArmorWolfMaxRunSpeed)
								{
									wolfCurrentSpeed = Constants.Khaos.AxeArmorWolfMaxRunSpeed;
								}

								axeArmorSafetyCooldown = axeArmorMaxSafetyCooldown;
							}
								}
							}
						if (faceplantSpellCooldown > 0)
						{
							--faceplantSpellCooldown;
						}
						else if (mistJumpCancelAllowed 
							&& sotnApi.AlucardApi.HasRelic(Relic.GravityBoots)
							&& (heldCross || pressCrossCDFrames)
							&& pressUpCDFrames)
						{
							// Faceplant: Gravity Boots Jump Cancel
							sotnApi.AlucardApi.State = 41;
							facePlantCooldown = facePlantCooldownBase + 2;
							mistBoostResetLocked = true;
							mistJumpCancelAllowed = false;
						}
						else if (mistJumpCancelAllowed
							&& sotnApi.AlucardApi.HasRelic(Relic.LeapStone)
							&& (heldCross || pressCrossCDFrames)
							&& jumpBoostStocks > 0)
						{
							// Faceplant: Leapstone Jump Cancel
							sotnApi.AlucardApi.State = 41;
							facePlantCooldown = facePlantCooldownBase + 2;
							mistBoostResetLocked = true;
							mistJumpCancelAllowed = false;
							//--jumpBoostStocks;
						}
						else
						{
						// Faceplant: Hearts to MP Conversion
							if (inputService.ButtonPressed(PlaystationInputKeys.Triangle, Globals.UpdateCooldownFrames)
								&& axeArmorDelayedMPRegenDuration < sotnApi.AlucardApi.MaxtHearts
								&& sotnApi.AlucardApi.CurrentHearts > 0)
								{
									uint remainingHearts = 0;
									int mpDifference = (int) (sotnApi.AlucardApi.MaxtMp - sotnApi.AlucardApi.CurrentMp - axeArmorDelayedMPRegenDuration);
									int heartDifference = sotnApi.AlucardApi.CurrentHearts > mpDifference ? (int) mpDifference : (int) sotnApi.AlucardApi.CurrentHearts;
									//MaxMP * 2

									int adjustedMultiplier = 1;
									if (!sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
									{
										adjustedMultiplier = 2;
									}

									int adjustedDifference = heartDifference * adjustedMultiplier;

									if (sotnApi.AlucardApi.CurrentHearts >= adjustedDifference)
									{
										remainingHearts = (uint) (sotnApi.AlucardApi.CurrentHearts - (adjustedDifference));
									}
									else
									{
										heartDifference = (int) sotnApi.AlucardApi.CurrentHearts / adjustedMultiplier;
									}
									axeArmorSafetyCooldown = axeArmorMaxSafetyCooldown;

									axeArmorDelayedMPRegenDuration += (int) heartDifference;
									sotnApi.AlucardApi.CurrentHearts = remainingHearts;
								}

								//Faceplant: Subweapons to HP/Hearts Conversion
							if (inputService.ButtonPressed(PlaystationInputKeys.Circle, Globals.UpdateCooldownFrames)
								&& ((axeArmorDelayedHeartRegenDuration < sotnApi.AlucardApi.MaxtHearts / 2 || axeArmorDelayedHeartRegenDuration == 0)
								|| (axeArmorDelayedHPRegenDuration < sotnApi.AlucardApi.MaxtHearts / 2 || axeArmorDelayedHPRegenDuration == 0))
								&& sotnApi.AlucardApi.Subweapon != Subweapon.Empty)
								{
									sotnApi.AlucardApi.Subweapon = Subweapon.Empty;

									if (!heartsOnlyActive)
									{
										int hpDivision = 2;

										if (sotnApi.AlucardApi.HasRelic(Relic.GasCloud))
										{
											axeArmorDelayedHeartRegenDuration += (int) sotnApi.AlucardApi.MaxtHearts;
											hpDivision = 1;
										}
										else
										{
											axeArmorDelayedHeartRegenDuration += (int) sotnApi.AlucardApi.MaxtHearts;
										}
										int hpDelayed = (int) (sotnApi.AlucardApi.MaxtHearts / hpDivision);
										axeArmorDelayedHPRegenDuration += (hpDelayed);
									}
									axeArmorSafetyCooldown = axeArmorMaxSafetyCooldown;
								}
							}
							if ((heldR1) ///|| pressR1_CDFrames)
								&& (hasSoulOfBat || (hasPowerOfMist && hasFormOfMist)))
							{
								//Console.WriteLine($"hasSoulOfBat:{hasSoulOfBat},hasPowerOfMist:{hasPowerOfMist},hasFormOfMist:{hasFormOfMist}");
								isAxeArmorMistFlight = true;
								if (!sotnApi.GameApi.InTransition)
								{
									--glideMPCooldown;
									--glideMPCooldown;
								}
								if (pressUp5)
								{
									if (mistCeilingLocked)
									{
										axeArmorFloat.PokeValue(0);
									}
									else
									{
										axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorMistFlightUpSpeed);
									}
									axeArmorFloat.Enable();
									applyContactDamage = true;
									contactDamage = 4;
								}
								else if (pressDown5)
								{
									if (mistCeilingLocked)
									{
										axeArmorFloat.PokeValue(-Constants.Khaos.AxeArmorMistFlightSpeed);
									}
									else
									{
										axeArmorFloat.PokeValue(-Constants.Khaos.AxeArmorMistFlightUpSpeed);

									}

									axeArmorFloat.Enable();
									applyContactDamage = true;
									contactDamage = 4;
								}
								else
								{
									applyContactDamage = true;
									contactDamage = 2;
									axeArmorFloat.PokeValue(-Constants.Khaos.AxeArmorMistFlightFloatSpeed);
									axeArmorFloat.Enable();
								}
							}
							else
							{
								isAxeArmorMistFlight = false;
								if (pressDown5)
								{
									if (mistCeilingLocked)
									{
										axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorMistCeilingFallSpeed);
									}
									else if (IsInRoomList(Constants.Khaos.ReduceFastFallRooms))
									{
										axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorReducedFallSpeed);
									}
									else
									{
										axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorFastFallSpeed);
									}

									axeArmorFloat.Enable();
									applyContactDamage = true;
									contactDamage = 4;
									if (!sotnApi.GameApi.InTransition)
									{
										++glideMPCooldown;
										++glideMPCooldown;
									}
								}
								else if (pressUp5)
								{
									applyContactDamage = true;

									if (mistCeilingLocked)
									{
										axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorMistCeilingSpeed);
										axeArmorFloat.Enable();
										if (!sotnApi.GameApi.InTransition)
										{
											mistFlightDuration -= 2;
										}
									}
									else if (mistFlightDuration > 1)
									{
										contactDamage = 6;
										axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorMistFlightSpeed);
										axeArmorFloat.Enable();
										if (!sotnApi.GameApi.InTransition)
										{
											mistFlightDuration -= 2;
										}
									}
									else
									{
										contactDamage = 4;
										if (mistBoostDuration > 1)
										{
											axeArmorFloat.PokeValue(0);
											axeArmorFloat.Enable();
											mistBoostDuration -= 2;
											applyContactDamage = true;
											contactDamage = 10;
										}
										else
										{
											axeArmorFloat.Disable();
										}
									}
								}
								else if (pressL1_5)
								{
									if (mistBoostDuration > 0)
									{
										axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorFloatBaseSpeed);
										axeArmorFloat.Enable();
										if (!sotnApi.GameApi.InTransition)
										{
											--glideMPCooldown;
											--glideMPCooldown;
											--mistBoostDuration;
										}
									}
									else if (underwaterActive)
									{
										axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorUnderwaterBaseSpeed);
										axeArmorFloat.Enable();
									}
									else
									{
										axeArmorFloat.Disable();
									}
									applyContactDamage = true;
									contactDamage = 2;
								}
								else if (underwaterActive)
								{
									axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorUnderwaterBaseSpeed);
									axeArmorFloat.Enable();
								}
								else
								{
									applyContactDamage = true;
									contactDamage = 2;
									axeArmorFloat.Disable();
								}
							}

							if (applyContactDamage && !rushDownActive && sotnApi.AlucardApi.HasRelic(Relic.GasCloud) 
								&& !IsInRoomList(Constants.Khaos.RichterRooms))
							{
								if (heartsOnlyActive)
								{
									contactDamage = 1;
								}
								else
								{
									bool reduceScaling = false;
									if (contactDamage < 4)
									{
										reduceScaling = true;
									}

									if (wolfCurrentSpeed > Constants.Khaos.AxeArmorWolfMinRunSpeed)
									{
										if (wolfRunActive)
										{
											contactDamage += 1;
										}
										else if (wolfDashActive)
										{
											contactDamage += 1;
										}
										if (wolfCurrentSpeed >= Constants.Khaos.AxeArmorWolfMinRunSpeed)
										{
											contactDamage += 1;
											if (wolfCurrentSpeed >= Constants.Khaos.AxeArmorWolfMaxRunSpeed)
											{
												contactDamage += 2;
												if (wolfCurrentSpeed >= Constants.Khaos.AxeArmorWolfMaxDashSpeed)
												{
													contactDamage += 2;
													if (wolfCurrentSpeed >= Constants.Khaos.AxeArmorWolfMaxMayhemSpeed)
													{
														contactDamage += 4;
														if (wolfCurrentSpeed >= Constants.Khaos.AxeArmorWolfMaxMayhemSuperSpeed)
														{
															contactDamage += 4;
														}
													}
												}
											}
										}
									}

									contactDamage = (uint) Math.Round(1.5 * contactDamage);
									contactDamage += 1;

									if (reduceScaling)
									{
										contactDamage += (uint) Math.Round(((axeArmorShieldINT + sotnApi.AlucardApi.Int) / 10.0) + (equipmentINT / 20.0));
									}
									else
									{
										contactDamage += (uint) Math.Round(((axeArmorShieldINT + sotnApi.AlucardApi.Int) / 5.0) + (equipmentINT / 10.0));
									}


									if (hasMojoMail)
									{
										contactDamage += 5;
									}
									if (sotnApi.AlucardApi.HasRelic(Relic.RingOfVlad))
									{
										contactDamage += 2;
									}


									if (IsInGalamothRoom())
									{
										contactDamage = (uint) Math.Round(contactDamage * 1.0 / 3);
									}
								}

								if (contactDamage < 1)
								{
									contactDamage = 1;
								}

								if (axeArmorContactDamage == 0)
								{
									axeArmorContactDamage = contactDamage;
									axeArmorContactDamageCooldown = 0;
									axeArmorContactDamageTimer.Start();
								}
								else
								{
									axeArmorContactDamage = contactDamage;
								}

								contactDamageType.PokeValue(Constants.Khaos.AxeArmorKickyFeetDamageType);
								contactDamageType.Enable();
							}
							else
							{
								axeArmorContactDamage = 0;
								contactDamageType.PokeValue(0);
								contactDamageType.Disable();
							}

							//Left + Right Speed Check
							int facePlantSpeed = 0;
							int facingModifier = 1;
							bool checkFacePlantSpeed = true;

							if (pressLeft5)
							{
								checkFacePlantSpeed = true;
								if (!sotnApi.AlucardApi.FacingLeft)
								{
									if (underwaterActive)
									{
										wolfCurrentSpeed -= 4000;
									}
									else if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
									{
										wolfCurrentSpeed -= 12000;
									}
									{
										wolfCurrentSpeed -= 36000;
									}
									sotnApi.AlucardApi.FacingLeft = true;
								}
							}
							else if (pressRight5)
							{
								checkFacePlantSpeed = true;
								if (sotnApi.AlucardApi.FacingLeft)
								{
									if (underwaterActive)
									{
										wolfCurrentSpeed -= 4000;
									}
									else if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
									{
										wolfCurrentSpeed -= 12000;
									}
									{
										wolfCurrentSpeed -= 36000;
									}
									sotnApi.AlucardApi.FacingLeft = false;
								}
							}

							if (wolfCurrentSpeed < 0 || !heldR2)
							{
								wolfCurrentSpeed = 0;
							}

							if (checkFacePlantSpeed)
							{
								int baseSpeed = 0;
								int wolfSpeed = 0;
								if (isAerialFacePlant)
								{
									baseSpeed = Constants.Khaos.FacePlantAirSpeed;
								}
								else
								{
									baseSpeed = Constants.Khaos.FacePlantGroundSpeed;
								}
								if (wolfCurrentSpeed > Constants.Khaos.AxeArmorWolfMinStopSpeed)
								{
									wolfSpeed = wolfCurrentSpeed;
									if (!isAxeArmorMistFlight)
									{

										if (sotnApi.AlucardApi.HasRelic(Relic.PowerOfMist))
										{
											if (isAerialFacePlant)
											{
												wolfSpeed = (int) Math.Round(wolfSpeed * 1.3);
											}
										}
										else if (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
										{
											if (isAerialFacePlant)
											{
												wolfSpeed = (int) Math.Round(wolfSpeed * .90);
											}
											wolfSpeed = (int) Math.Round(wolfSpeed * .75);
										}
										else
										{
											if (isAerialFacePlant)
											{
												wolfSpeed = (int) Math.Round(wolfSpeed * .5750);
											}
											else
											{
												wolfSpeed = (int) Math.Round(wolfSpeed * .5);
											}
										}
									}
								}

								if (baseSpeed > wolfSpeed)
								{
									facePlantSpeed = baseSpeed;
								}
								else
								{
									facePlantSpeed = wolfSpeed;
								}
							}

							if (facePlantSpeed > 0)
							{
								if (sotnApi.AlucardApi.FacingLeft)
								{
									facingModifier = -1;
								}
								axeArmorHorizontalSpeed.Enable();
								axeArmorHorizontalSpeed.PokeValue(facePlantSpeed * facingModifier);
							}
							else
							{
								axeArmorHorizontalSpeed.Disable();
							}

							if (!sotnApi.GameApi.InTransition)
							{
								if (glideMPCooldown <= 0)
								{
									sotnApi.AlucardApi.CurrentMp -= 1;
									currentMP -= 1;
									if (sotnApi.AlucardApi.HasRelic(Relic.FormOfMist))
									{
										glideMPCooldown = glideMPMaxFoMCooldown;
									}
									else
									{
										glideMPCooldown = glideMPMaxCooldown;
									}
								}
								else
								{
									--glideMPCooldown;
									--glideMPCooldown;
								}
							}
						}
					else
					{
						if (sotnApi.AlucardApi.HasControl() && !sotnApi.GameApi.IsInMenu())
						{
							UpdateAxeArmorHeartCooldowns();
						}
						if (slamDuration > 0)
						{
							if (slamDuration == slamMaxDuration)
							{
								if (wolfCurrentSpeed > 0)
								{
									int roll = rng.Next(0, 2);
									if (roll == 0)
									{
										int facingModifier = 1;
										if (sotnApi.AlucardApi.FacingLeft)
										{
											facingModifier = -1;
										}
										axeArmorHorizontalSpeed.PokeValue(wolfCurrentSpeed * facingModifier * -1);
										axeArmorHorizontalSpeed.Enable();
									}
								}
							}
							else
							{
								axeArmorHorizontalSpeed.Disable();
							}
							int flightspeed = -200000 - (2500 * slamDuration);
							axeArmorFloat.PokeValue(flightspeed);
							axeArmorFloat.Enable();
							--slamDuration;
						}
						else
						{
							if (sotnApi.AlucardApi.HasControl()
							&& !sotnApi.GameApi.IsInMenu()
							&& sotnApi.AlucardApi.State == 43
							&& mistTechDuration > 0
							&& pressR1_CDFrames)
							{
								mistTechDuration -= 1;
								axeArmorFloat.PokeValue(Constants.Khaos.AxeArmorMistFlightUpSpeed);
								axeArmorFloat.Enable();
							}
							else
							{
								axeArmorFloat.Disable();
							}
							if (wolfCurrentSpeed > 0 && inputService.ButtonHeld(PlaystationInputKeys.R2))
							{
								int acceleration = sotnApi.AlucardApi.HasRelic(Relic.SoulOfWolf) == true ? Constants.Khaos.AxeArmorWolfMaxAcceleration : Constants.Khaos.AxeArmorWolfBaseAcceleration;
								int maxSpeed = sotnApi.AlucardApi.HasRelic(Relic.PowerOfWolf) == true ? Constants.Khaos.AxeArmorWolfMaxMayhemSuperSpeed : Constants.Khaos.AxeArmorWolfMaxDashSpeed; ;
								int facingModifier = 1;

								if (sotnApi.AlucardApi.FacingLeft)
								{
									facingModifier = -1;
								}
								if (!(sotnApi.AlucardApi.HasRelic(Relic.SkillOfWolf)))
								{
									acceleration /= 2;
								}

								wolfCurrentSpeed += acceleration;
						
								if (wolfCurrentSpeed > maxSpeed)
								{
									wolfCurrentSpeed = maxSpeed;
								}
								axeArmorHorizontalSpeed.PokeValue(wolfCurrentSpeed * facingModifier);
								axeArmorHorizontalSpeed.Disable();
								//axeArmorHorizontalSpeed.Enable();
							}
							else
							{
								axeArmorHorizontalSpeed.Disable();
							}
						}
						action.Disable();
						state.Disable();
					}
				}
			}
			else
			{
				action.Disable();
				state.Disable();
				if (axeArmorStatsActive)
				{
					axeArmorVerticalJumpMoving.Disable();
					axeArmorVerticalJumpStationary.Disable();
					axeArmorHorizontalJump.Disable();
					axeArmorDamage.Disable();
					axeArmorDamageTypeA.Disable();
					axeArmorDamageTypeB.Disable();
					axeArmorDamageInterval.Disable();
					axeArmorWeapon.PokeValue(0);
					axeArmorWeapon.Disable();
					axeArmorStatsActive = false;
				}
				axeArmorFloat.Disable();
				axeArmorHorizontalSpeed.Disable();
			}
		}

		private void CheckSmoothCrouch()
		{
			if (!IsInRoomList(Constants.Khaos.ClockRoom)
				&& !IsInRoomList(Constants.Khaos.ReverseElevator)
				&& !IsInRoomList(Constants.Khaos.Elevator)
				&& (sotnApi.GameApi.CanSave()
				|| !hasAxeArmorStoredResources)
			)
			{
				smoothCrouch.Disable();
			}
			else
			{
				smoothCrouch.Enable();
			}
		}

		private void AxeArmorDisableStateWhenDead()
		{
			minHoldDownTime = 30;
			minHoldUpTime = 30;
			characterData.Disable();
			state.Disable();
			action.Disable();
			hpForMPDeathTimer.Start();
		}
		private void CheckEquippedCloak()
		{
			uint newCloakIndex = sotnApi.AlucardApi.Cloak;

			if (newCloakIndex != cloakIndex)
			{
				cloakIndex = newCloakIndex;
				UpdateVisualEffect();
				if (axeArmorActive)
				{
					UpdateAxeArmorCloak();
				}
			}
		}

		private void UpdateAxeArmorCloak()
		{
			int colorCode = 1;
			string heldCloak = Equipment.Items[(int) (sotnApi.AlucardApi.Cloak + Equipment.HandCount + 1)];

			Console.WriteLine($"Cloak Index :{sotnApi.AlucardApi.Cloak}, heldCloak{heldCloak}");
			//notificationService.AddMessage($"Cloak Index :{sotnApi.AlucardApi.Cloak}, heldCloak{heldCloak}");
			if (heldCloak == "----cloak")
			{
				colorCode = 1;
			}
			else if (heldCloak == "Cloth cape")
			{
				colorCode = 140;
			}
			else if (heldCloak == "Reverse cloak")
			{
				colorCode = 132;
			}
			else if (heldCloak == "Elven cloak")
			{
				colorCode = 190;
			}
			else if (heldCloak == "Crystal cloak")
			{
				colorCode = 178;
			}
			else if (heldCloak == "Royal cloak")
			{
				colorCode = 135;
			}
			else if (heldCloak == "Blood cloak")
			{
				colorCode = 138;
			}
			else if (heldCloak == "Joseph's cloak")
			{
				SetAxeArmorColor(out colorCode);
			}
			else if (heldCloak == "Twilight cloak")
			{
				colorCode = 100;
			}

			for (int i = 0; i < axeArmorColorCheatCount; i++)
			{
				var colorCheat = cheats.GetCheatByName(Constants.Khaos.AxeArmorColorName);
				cheats.RemoveCheat(colorCheat);
			}
			axeArmorColorCheatCount = 0;

			cheats.AddCheat(1548640, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548662, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548684, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548712, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548734, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte); // Axe Stance 1
			cheats.AddCheat(1548756, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548784, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte); // Axe Stance 2
			cheats.AddCheat(1548806, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548832, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548854, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548880, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548904, 1, Constants.Khaos.AxeArmorColorName, WatchSize.Byte); // Axe Armor Swing
			cheats.AddCheat(1548928, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548956, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1548984, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1549006, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1549032, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			cheats.AddCheat(1549060, colorCode, Constants.Khaos.AxeArmorColorName, WatchSize.Byte);
			
			axeArmorColorCheatCount = 18;

			/*
				132 - Reverse Orange
				135 - Grey Knight. Golden Axe
				138 - Fire (Blood Cloak)
				140 - Monochrome
				178 - Darker Red
				190 - Grey with Bright Green
				100 - Neon Knight
			*/
			/* Joseph's Cloak
				97 - Normal Knight 
				103 - Barney
				115 - Yellow and Blue
				116 - Green
				117 - Aqua
				118 - Alt Blue
				119 - Light Purple
				120 - Black
				137 - Pale Yellow w/ Blue
				144 - Sunburst
				154 - Dark Red
			*/
		}

		private void SetAxeArmorColor(out int colorCode)
		{
			colorCode = 1;
			int colorSet = rng.Next(1, 12);

			switch (colorSet)
			{
				case 1:
					colorCode = 97;
					break;
				case 2:
					colorCode = 103;
					break;
				case 3:
					colorCode = 115;
					break;
				case 4:
					colorCode = 116;
					break;
				case 5:
					colorCode = 117;
					break;
				case 6:
					colorCode = 118;
					break;
				case 7:
					colorCode = 119;
					break;
				case 8:
					colorCode = 120;
					break;
				case 9:
					colorCode = 137;
					break;
				case 10:
					colorCode = 144;
					break;
				case 11:
					colorCode = 154;
					break;
			}

		}

		private void CheckAxeArmorHeldItems(bool pressSquareCDFrames, bool pressCircleCDFrames, bool heldUp)
		{
			// find me
			bool isValidItem = false;
			bool isConsumeableWeapon = false;
			bool equippedDuplicator = false;
			bool fastCooldown = true;
			int subWeaponCooldown = 0;
			int handEffect = 0;

			for (int i = 0; i < 3; i++)
			{
				var handCheat = cheats.GetCheatByName("Hand Effect 1");
				cheats.RemoveCheat(handCheat);
			}

			for (int i = 0; i < 3; i++)
			{
				var handCheat = cheats.GetCheatByName("Hand Effect 2");
				cheats.RemoveCheat(handCheat);
			}
			
			if (useItemCooldown == 0 && !sotnApi.GameApi.CanSave())
			{
				equippedDuplicator = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Duplicator" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Duplicator";

				if (useRightHandCooldown == 0 && pressSquareCDFrames)
				{
					uint rightHand = sotnApi.AlucardApi.RightHand;
					handEffect = 1;
					checkHeldItemList(in rightHand, in handEffect, out isValidItem, out fastCooldown);

					if (axeArmorLimitedRightHand && axeArmorAllowWeaponConsume)
					{
						isValidItem = true;
						isConsumeableWeapon = true;
						axeArmorAllowWeaponConsume = false;
					}

					if (isValidItem == true)
					{
						string itemName = "";

						if (hasAxeArmorStoredResources)
						{
							itemName = Equipment.Items[(int) (storedRightHand)];
						}
						else
						{
							itemName = Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)];
						}

						if (!equippedDuplicator)
						{
							if (sotnApi.AlucardApi.HasItemInInventory(itemName))
							{
								sotnApi.AlucardApi.TakeOneItemByName(itemName);
							}
							else
							{
								storedRightHand = 0;
								sotnApi.AlucardApi.RightHand = (uint) Equipment.Items.IndexOf("empty hand");
							}
						}
					}
				}
				else if (useLeftHandCooldown == 0 && pressCircleCDFrames && !heldUp)
				{
					uint leftHand = sotnApi.AlucardApi.LeftHand;
					handEffect = 2;
					checkHeldItemList(in leftHand, in handEffect, out isValidItem, out fastCooldown);
					if (isValidItem == true)
					{
						subWeaponCooldown = 40;
						string itemName = "";

						if (hasAxeArmorStoredResources)
						{
							itemName = Equipment.Items[(int) (storedLeftHand)];
						}
						else
						{
							itemName = Equipment.Items[(int) (sotnApi.AlucardApi.LeftHand)];
						}

						if (!equippedDuplicator)
						{
							if (sotnApi.AlucardApi.HasItemInInventory(itemName))
							{
								sotnApi.AlucardApi.TakeOneItemByName(itemName);
							}
							else
							{
								storedLeftHand = 0;
								sotnApi.AlucardApi.LeftHand = (uint) Equipment.Items.IndexOf("empty hand");
							}
						}
					}
				}
			}
			else if (useItemCooldown > 0)
			{
				useItemCooldown -= 1;
			}
			if (isValidItem)
			{
				int cooldown = 40 * Globals.UpdateCooldownFrames;
				useItemCooldown = useItemCooldownBase;

				if (isConsumeableWeapon)
				{
					cooldown = 2 * Globals.UpdateCooldownFrames;
				}
				else if (fastCooldown)
				{
					cooldown = 4 * Globals.UpdateCooldownFrames;
				}
				else if (equippedDuplicator)
				{
					cooldown = 42 * Globals.UpdateCooldownFrames;
				}

				if (handEffect == 1)
				{
					useRightHandCooldown = cooldown;
				}
				else
				{
					useLeftHandCooldown = cooldown;
				}

				heartGlobalCooldown += subWeaponCooldown; // add to subweapon global cooldown
			}
			if (useRightHandCooldown > 0)
			{
				--useRightHandCooldown;
				if (useRightHandCooldown == 0)
				{
					for (int i = 0; i < 3; i++)
					{
						var handCheat = cheats.GetCheatByName("Hand Effect 1");
						cheats.RemoveCheat(handCheat);
					}
				}
			}
			if (useLeftHandCooldown > 0)
			{
				--useLeftHandCooldown;
				if (useLeftHandCooldown == 0)
				{
					for (int i = 0; i < 3; i++)
					{
						var handCheat = cheats.GetCheatByName("Hand Effect 2");
						cheats.RemoveCheat(handCheat);
					}
				}
			}
		}

		void checkHeldItemList(in uint hand, in int handEffect, out bool isValidItem, out bool fastCooldown)
		{
			isValidItem = false;
			fastCooldown = false;
			int foodIndex = (int) hand;
			int address = Constants.Khaos.AxeArmorHand1EffectAddress - 188 + (188 * handEffect);
			int itemIndex = 0;
			string cheatName = "Hand Effect " + handEffect;

			bool skipHandEffectSet = false;

			if (hand == (uint) Equipment.Items.IndexOf("Frankfurter"))
			{
				notificationService.AddMessage($"Gross Corndog Toss");
				skipHandEffectSet = true;
				isValidItem = true;
			}
			else if (Array.Exists(Constants.Khaos.axeArmorFoodMinorHealing, x => x == foodIndex))
			{
				uint healAmount = 20;
				if (sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp)
				{
					if (sotnApi.AlucardApi.CurrentHp + healAmount < sotnApi.AlucardApi.MaxtHp)
					{
						sotnApi.AlucardApi.CurrentHp += healAmount;
					}
					else
					{
						sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
					}
					skipHandEffectSet = true;
					isValidItem = true;
				}
			}
			else if (Array.Exists(Constants.Khaos.axeArmorFoodModerateHealing, x => x == foodIndex))
			{
				uint healAmount = 32;
				if (sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp)
			
				{
					if (sotnApi.AlucardApi.CurrentHp + healAmount < sotnApi.AlucardApi.MaxtHp)
					{
						sotnApi.AlucardApi.CurrentHp += healAmount;
					}
					else
					{
						sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
					}
					skipHandEffectSet = true;
					isValidItem = true;
				}
			}
			else if (Array.Exists(Constants.Khaos.axeArmorFoodMajorHealing, x => x == foodIndex))
			{
				uint healAmount = 80;
				if (sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp)
				{
					if (sotnApi.AlucardApi.CurrentHp + healAmount < sotnApi.AlucardApi.MaxtHp)
					{
						sotnApi.AlucardApi.CurrentHp += healAmount;
					}
					else
					{
						sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
					}
					skipHandEffectSet = true;
					isValidItem = true;
				}
			}
			else if (Array.Exists(Constants.Khaos.axeArmorFoodMaxHealing, x => x == foodIndex))
			{
				uint healAmount = 100;
				if (sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp)
				{
					if (sotnApi.AlucardApi.CurrentHp + healAmount < sotnApi.AlucardApi.MaxtHp)
					{
						sotnApi.AlucardApi.CurrentHp += healAmount;
					}
					else
					{
						sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
					}
					skipHandEffectSet = true;
					isValidItem = true;
				}
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Meal ticket"))
			{
				string newItemName = "Pizza";
				int roll = rng.Next(0, 10);
				int newItemIndex = 0;

				if (roll < 4)
				{
					newItemIndex = Constants.Khaos.axeArmorFoodMinorHealing[rng.Next(0, Constants.Khaos.axeArmorFoodMinorHealing.Length)];
					newItemName = Equipment.Items[newItemIndex];
				}
				else if (roll < 7)
				{
					newItemIndex = Constants.Khaos.axeArmorFoodModerateHealing[rng.Next(0, Constants.Khaos.axeArmorFoodModerateHealing.Length)];
					newItemName = Equipment.Items[newItemIndex];
				}
				else if (roll < 9)
				{
					newItemIndex = Constants.Khaos.axeArmorFoodMajorHealing[rng.Next(0, Constants.Khaos.axeArmorFoodMajorHealing.Length)];
					newItemName = Equipment.Items[newItemIndex];
				}
				else
				{
					newItemIndex = Constants.Khaos.axeArmorFoodMaxHealing[rng.Next(0, Constants.Khaos.axeArmorFoodMaxHealing.Length)];
					newItemName = Equipment.Items[newItemIndex];
				}
				sotnApi.AlucardApi.GrantItemByName(newItemName);
				notificationService.AddMessage($"+1 {newItemName}");
				skipHandEffectSet = true;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Library card"))
			{
				Library("You", true, handEffect);
				skipHandEffectSet = true;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Antivenom"))
			{
				itemIndex = (int) Potion.Antivenom;
				uint recoveryAmount = 20;
				AxeArmorStatRestore(recoveryAmount, true, true, true, true);
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Uncurse"))
			{
				itemIndex = (int) Potion.Uncurse;
				uint recoveryAmount = 35;
				AxeArmorStatRestore(recoveryAmount, true, true, true, true);
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Elixir"))
			{
				itemIndex = (int) Potion.Elixir;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("High potion"))
			{
				itemIndex = (int) Potion.HighPotion;
				sotnApi.AlucardApi.ActivatePotion(Potion.HighPotion);
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Potion"))
			{
				itemIndex = (int) Potion.Potion;
				sotnApi.AlucardApi.ActivatePotion(Potion.Potion);
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Manna prism"))
			{
				itemIndex = (int) Potion.Mannaprism;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Str. potion"))
			{
				itemIndex = (int) Potion.StrPotion;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Smart potion"))
			{
				itemIndex = (int) Potion.SmartPotion;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Attack potion"))
			{
				itemIndex = (int) Potion.AttackPotion;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Shield potion"))
			{
				itemIndex = (int) Potion.ShieldPotion;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Luck potion"))
			{
				itemIndex = (int) Potion.LuckPotion;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Heart Refresh"))
			{
				itemIndex = (int) Potion.Antivenom;
				sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Resist fire"))
			{
				itemIndex = (int) Potion.ResistFire;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Resist thunder"))
			{
				itemIndex = (int) Potion.ResistThunder;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Resist ice"))
			{
				itemIndex = (int) Potion.ResistIce;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Resist holy"))
			{
				itemIndex = (int) Potion.ResistHoly;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Resist dark"))
			{
				itemIndex = (int) Potion.ResistDark;
				isValidItem = true;
			}
			else if (hand == (uint) Equipment.Items.IndexOf("Resist stone"))
			{
				itemIndex = (int) Potion.ResistStone;
				uint recoveryAmount = 50;
				AxeArmorStatRestore(recoveryAmount, true, true, true, true);
				isValidItem = true;
			}
			if (isValidItem && !skipHandEffectSet)
			{
				int Potion = 39; //27 in Hex
				cheats.AddCheat(address + 38, Potion, cheatName, WatchSize.Byte); //+38
				cheats.AddCheat(address + 49, itemIndex, cheatName, WatchSize.Byte); //+49, Activator Index
			}
		}

		private void AxeArmorStatRestore(uint recoveryAmount, bool restoreHP, bool restoreMP, bool restoreHearts, bool checkForAnkh)
		{
			if (checkForAnkh)
			{
				bool equippedAnkh = Equipment.Items[(int) (sotnApi.AlucardApi.Accessory1 + Equipment.HandCount + 1)] == "Ankh of Life" || Equipment.Items[(int) (sotnApi.AlucardApi.Accessory2 + Equipment.HandCount + 1)] == "Ankh of Life";

				if (equippedAnkh)
				{
					recoveryAmount = (uint) (1.5 * recoveryAmount);
				}
			}

			if (restoreHP && sotnApi.AlucardApi.CurrentHp < sotnApi.AlucardApi.MaxtHp)
			{
				if (sotnApi.AlucardApi.CurrentHp + recoveryAmount < sotnApi.AlucardApi.MaxtHp)
				{
					sotnApi.AlucardApi.CurrentHp += recoveryAmount;
				}
				else
				{
					sotnApi.AlucardApi.CurrentHp = sotnApi.AlucardApi.MaxtHp;
				}
			}
			if (restoreMP && !mpLocked && sotnApi.AlucardApi.CurrentMp < sotnApi.AlucardApi.MaxtMp)
			{
				if (sotnApi.AlucardApi.CurrentMp + recoveryAmount < sotnApi.AlucardApi.MaxtMp)
				{
					sotnApi.AlucardApi.CurrentMp += recoveryAmount;
				}
				else
				{
					sotnApi.AlucardApi.CurrentMp = sotnApi.AlucardApi.MaxtMp;
				}
			}
			if (restoreHearts && !heartsLocked && sotnApi.AlucardApi.CurrentHearts < sotnApi.AlucardApi.MaxtHearts)
			{
				if (sotnApi.AlucardApi.CurrentHearts + recoveryAmount < sotnApi.AlucardApi.MaxtHearts)
				{
					sotnApi.AlucardApi.CurrentHearts += recoveryAmount;
				}
				else
				{
					sotnApi.AlucardApi.CurrentHearts = sotnApi.AlucardApi.MaxtHearts;
				}
			}
		}
		private void AxeArmorClipCheck()
		{
			bool enableClipDirection = false;
			bool enableCeilingClip = false;
			bool enableLeftClip = false;
			bool enableRightClip = false;
			bool olroxOverride = false;
			bool deathSkip = false;

			mistCeilingLocked = false;

			if (isAxeArmorBat || isAxeArmorMist)
			{ // Reverse Castle, Ceiling Check
				if (alucardMapX == 25 && alucardMapY == 53 && alucardSecondCastle)
				{
					if (sotnApi.AlucardApi.ScreenX >= 198 && sotnApi.AlucardApi.ScreenX <= 217)
					{
						enableCeilingClip = true;
					}
				}
			}
			if (!isAxeArmorMistFlight)
			{
				if (IsInRoomList(Constants.Khaos.AxeArmorDeathSkip))
				{
					if ((sotnApi.AlucardApi.ScreenY >= 75 && sotnApi.AlucardApi.ScreenY <= 136) && ((sotnApi.AlucardApi.ScreenX > 180 && alucardMapX != 21) || sotnApi.AlucardApi.ScreenX < 56))
					{
						enableLeftClip = true;
						enableRightClip = true;
						enableClipDirection = true;
						enableCeilingClip = true;
						deathSkip = true;
					}
				}
				else if (IsInRoomList(Constants.Khaos.AxeArmorOlroxRooms))
				{
					if (sotnApi.AlucardApi.ScreenY <= 133)
					{
						enableCeilingClip = isAxeArmorMist;
						enableClipDirection = true;
						enableLeftClip = isAxeArmorMist;
						enableRightClip = isAxeArmorMist;
						olroxOverride = isAxeArmorVClipAllowed;
					}
					else if(sotnApi.AlucardApi.ScreenY >= 133)
					{
						if (sotnApi.AlucardApi.ScreenY >= 230)
						{
							enableCeilingClip = true;
							MistGateAntiSoftLock();
						}
						enableClipDirection = true;
					}
				}
				else if (IsInRoomList(Constants.Khaos.AxeArmorReverseOlroxRooms))
				{
					if (sotnApi.AlucardApi.ScreenY <= 115)
					{
						enableCeilingClip = isAxeArmorMist;
						enableLeftClip = isAxeArmorMist;
						enableRightClip = isAxeArmorMist;
						olroxOverride = isAxeArmorVClipAllowed;
					}
				}
				else if (IsInRoomList(Constants.Khaos.MistGateRooms))
				{
					if (isAxeArmorMist && sotnApi.AlucardApi.CurrentMp < 6) //General Softlock Fix
					{
						sotnApi.AlucardApi.CurrentMp = 6;
					}

					
					if (alucardSecondCastle)
					{
						if ((alucardMapX == 16 || alucardMapX == 17)) //Reverse Library
						{
							if (sotnApi.AlucardApi.ScreenY <= 136 && sotnApi.AlucardApi.ScreenY > 95)
							{
								enableCeilingClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableRightClip = isAxeArmorMist;
								if (alucardMapX == 16 && sotnApi.AlucardApi.ScreenY > 125)
								{
									enableClipDirection = true;
									MistGateAntiSoftLock();
								}
							}
						}
						else if ((alucardMapX == 50)) // Reverse Silver Ring
						{
							if (sotnApi.AlucardApi.ScreenY <= 96)
							{
								enableRightClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableCeilingClip = isAxeArmorMist;
								mistCeilingLocked = true;
							}
							if (sotnApi.AlucardApi.ScreenY <= 131)
							{
								enableRightClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableCeilingClip = isAxeArmorMist;
								mistCeilingLocked = true;
							}
							if (sotnApi.AlucardApi.ScreenY >= 131)
							{
								enableClipDirection = true;
								MistGateAntiSoftLock();
							}
							/*
							if (sotnApi.AlucardApi.ScreenY <= 95)
							{
								enableRightClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								mistCeilingLocked = isAxeArmorMist;
							}
							else
							{
								enableCeilingClip = true;
								if (sotnApi.AlucardApi.ScreenY <= 131)
								{
									enableRightClip = true;
									enableLeftClip = true;
								}
								if (sotnApi.AlucardApi.ScreenY >= 131)
								{
									MistGateAntiSoftLock();
								}
							}*/
						}
						else if (alucardMapX == 3)
						{
							if (sotnApi.AlucardApi.ScreenX >= 199
							&& sotnApi.AlucardApi.ScreenX <= 233) //Reverse Outer Wall
							{
								enableClipDirection = true;
								enableCeilingClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableRightClip = isAxeArmorMist;
							}
							if (sotnApi.AlucardApi.ScreenX >= 199)
							{
								mistCeilingLocked = true;
								MistGateAntiSoftLock();
							}
						}
						else if ((alucardMapX == 41 || alucardMapX == 42)) //Reverse Coliseum
						{
							enableCeilingClip = isAxeArmorMist;
							if (sotnApi.AlucardApi.ScreenY <= 116)
							{
								enableCeilingClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableRightClip = isAxeArmorMist;
							}
							else
							{
								if(sotnApi.AlucardApi.ScreenY >= 120)
								{
									enableClipDirection = true;
								}
								if (sotnApi.AlucardApi.ScreenY >= 153)
								{
									enableClipDirection = true;
									MistGateAntiSoftLock();
								}
							}
						}
					}
					else
					{
						if ((alucardMapX == 46 || alucardMapX == 47)) // Library
						{
							if (sotnApi.AlucardApi.ScreenY <= 136)
							{
								if(sotnApi.AlucardApi.ScreenY >= 133)
								{
									enableCeilingClip = true;
									enableClipDirection = true;
								}
								enableCeilingClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableRightClip = isAxeArmorMist;
							}
							else
							{
								enableCeilingClip = isAxeArmorMist;
								enableClipDirection = true;
								MistGateAntiSoftLock();
							}
						}
						else if ((alucardMapX == 13 || alucardMapX == 14)) // Silver Ring
						{
							if (sotnApi.AlucardApi.ScreenY <= 96)
							{
								enableRightClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableCeilingClip = isAxeArmorMist;
								mistCeilingLocked = true;
							}
							if (sotnApi.AlucardApi.ScreenY <= 131)
							{
								enableRightClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableCeilingClip = isAxeArmorMist;
								mistCeilingLocked = true;
							}
							if (sotnApi.AlucardApi.ScreenY >= 131)
							{
								enableClipDirection = true;
								MistGateAntiSoftLock();
							}
						}
						else if (alucardMapX == 60) //Outer Wall
						{
							if (sotnApi.AlucardApi.ScreenX >= 24
							&& sotnApi.AlucardApi.ScreenX <= 58) 
							{
								enableClipDirection = true;
								enableCeilingClip = isAxeArmorMist;
								enableLeftClip = isAxeArmorMist;
								enableRightClip = isAxeArmorMist;
								//mistCeilingLocked = true;
							}
							if (sotnApi.AlucardApi.ScreenX <= 59)
							{
								mistCeilingLocked = true;
								MistGateAntiSoftLock();
							}
						}
						else if (alucardMapX == 21 || alucardMapX == 22)  // Coliseum
						{
							enableCeilingClip = isAxeArmorMist;
							if (sotnApi.AlucardApi.ScreenY <= 134)
							{
								enableLeftClip = isAxeArmorMist;
								enableRightClip = isAxeArmorMist;
							}
							else if (sotnApi.AlucardApi.ScreenY > 138)
							{
								enableClipDirection = true;
								if (sotnApi.AlucardApi.ScreenY > 180)
								{
									MistGateAntiSoftLock();
								}
							}
						}
					}
				}
			}

			void MistGateAntiSoftLock()
			{
				mistFlightDuration = mistFlightMaxDuration;
				if (sotnApi.AlucardApi.CurrentMp < 6)
				{
					sotnApi.AlucardApi.CurrentMp = 6;
				}
			}
			//Offscreen = 237 Screen Y

			if (enableClipDirection)
			{
				clipDirection.PokeValue(Constants.Khaos.EnableClipDirection);
				clipDirection.Enable();
			}
			else
			{
				clipDirection.PokeValue(Constants.Khaos.DisableClipDirection);
			}

			if (enableCeilingClip && (isAxeArmorVClipAllowed || deathSkip))
			{
				ceilingClip.PokeValue(Constants.Khaos.EnableCeilingClip);
				ceilingClip.Enable();
			}
			else
			{
				ceilingClip.PokeValue(Constants.Khaos.DisableCeilingClip);
			}
			if (enableLeftClip && (isAxeArmorHClipAllowed || olroxOverride || deathSkip))
			{
				leftClip.PokeValue(Constants.Khaos.EnableLeftClip);
				leftClip.Enable();
			}
			else
			{
				leftClip.PokeValue(Constants.Khaos.DisableLeftClip);
			}
			if (enableRightClip && (isAxeArmorHClipAllowed || olroxOverride || deathSkip))
			{
				rightClip.PokeValue(Constants.Khaos.EnableRightClip);
				rightClip.Enable();
			}
			else
			{
				rightClip.PokeValue(Constants.Khaos.DisableRightClip);
			}
		}

		private void CheckAxeArmorStats(bool log = false)
		{
			//log = true;

			//Work around - Grab Alucard current stats
			LiveEntity equipSTRActor;
			LiveEntity equipINTActor;
			LiveEntity equipLCKActor;
			LiveEntity equipATK1Actor;
			LiveEntity equipATK2Actor;

			long STRStart = 621510;  //097BC8 -2
			long INTStart = 621518;  //097BD0 -2
			long LCKStart = 621522;  //097BD4 -2
			long ATK1Start = 621594; //097C1C -2
			long ATK2Start = 621598; //097C20 -2

			equipSTRActor = sotnApi.EntityApi.GetLiveEntity(STRStart);
			equipINTActor = sotnApi.EntityApi.GetLiveEntity(INTStart);
			equipLCKActor = sotnApi.EntityApi.GetLiveEntity(LCKStart);
			equipATK1Actor = sotnApi.EntityApi.GetLiveEntity(ATK1Start);
			equipATK2Actor = sotnApi.EntityApi.GetLiveEntity(ATK2Start);

			equipmentSTR = (equipSTRActor.Xpos) > 64000 ? (int) (equipSTRActor.Xpos) - 65536 : (int) (equipSTRActor.Xpos);
			equipmentINT = (equipINTActor.Xpos) > 64000 ? (int)(equipINTActor.Xpos) - 65536 : (int) (equipINTActor.Xpos);
			equipmentLCK = (equipSTRActor.Xpos) > 64000 ? (int) (equipLCKActor.Xpos) - 65536 : (int) (equipLCKActor.Xpos);
			equipmentATK1 = (equipATK1Actor.Xpos) > 64000 ? (int) (equipATK1Actor.Xpos) - 65536 : (int) (equipATK1Actor.Xpos);
			equipmentATK2 = (equipATK2Actor.Xpos) > 64000 ? (int) (equipATK2Actor.Xpos) - 65536 : (int) (equipATK2Actor.Xpos);

			if (log)
			{
				Console.WriteLine($"INT:{equipmentINT},STR:{equipmentSTR},ATK1:{equipmentATK1},ATK2:{equipmentATK2}");
			}
		}
		private void CheckAxeArmorAttack(in bool logDamage, in int flatDamage, out int damage)
		{
			damage = 0;

			if (heartsOnlyActive)
			{
				//Set damage to 0 if hearts only.
				notificationService.WeaponMessage = "";
				//notificationService.AddMenuMessage($"WPN: Disabled!");
				return;
			}

			//Initialize Axe Armor Damage
			int multiplierBaseDamage = (int) (equipmentATK1 - equipmentSTR - sotnApi.AlucardApi.Str - 2);
			int offHandBonus = (int) (equipmentATK2 - equipmentSTR - sotnApi.AlucardApi.Str - 2);
			int vanillaINTScaling;
			int vladBonus = 0;
			int handsWithDamage = 0;
			int leftHand = axeArmorLeftHand;
			int rightHand = axeArmorRightHand;
			bool requireLogs = false;

			CheckAxeArmorWeapon();

			if (leftHand != axeArmorLeftHand || rightHand != axeArmorRightHand)
			{
				requireLogs = true;
			}


			int shieldBonusDamage = (int) Math.Round((axeArmorShieldINT * .15)) + 1;
			int shieldBaseDamage = (int) Math.Round((sotnApi.AlucardApi.Str * .5) + (equipmentSTR * .5));
			shieldBaseDamage += shieldBonusDamage;
			int shieldRodBonus = shieldBonusDamage;


			if (equipmentATK1 > 0 && equipmentATK2 > 0 && !axeArmorLimitedLeftHand)
			{
				handsWithDamage = 2;
				damage += equipmentATK1;

				damage = (int) Math.Round(damage * 3.0 / 4.0);

				if (multiplierBaseDamage > 0)
				{
					//Weapon1Atk
					damage += (int) Math.Round((multiplierBaseDamage * axeArmorMultiDamage) - (multiplierBaseDamage));
				}

				if (offHandBonus > 0)
				{
					//Weapon2Atk
					damage += (int) Math.Round((offHandBonus) * .35) + 1;
				}

				//STRPenalty
				damage += (int) Math.Round((sotnApi.AlucardApi.Str * axeArmorMultiSTR) - (sotnApi.AlucardApi.Str));
				damage += 3 + axeArmorFlatDamage;

				if (axeArmorTwoHanded)
				{ //2H Damage Boost: +9 Damage, 1 Base STR, .75 Equipment Bonus
					damage += 7;
					damage += (int) Math.Round((sotnApi.AlucardApi.Str) / 4.0);
				}
				else if (axeArmorEmptyRightHand && axeArmorEmptyRightHand)
				{ //Unarmed: +2 Damage, .75 Base STR, .75 Equipment Bonus
					///damage += 1;
				}
				else
				{ //Dual-wield 1H: +4 Damage, .75 Base STR, .75 Equipment Bonus
					damage += 2;
				}

				if (axeArmorShieldRightHand)
				{
					damage += shieldBaseDamage;
				}
				if (axeArmorShieldLeftHand)
				{
					damage += shieldBonusDamage;
				}
				if (axeArmorShieldINT >= 65)
				{
					damage += shieldRodBonus;
				}
			}
			else if (equipmentATK1 > 0)
			{
				//All without Shield: 0.75 ATK1 + .67 Base STR + .50 Equipment STR 
				//All with Shield: 0.75 ATK1 + .75 Base STR + .75 Equipment STR
				
				handsWithDamage = 1;
				damage += (int) Math.Round(equipmentATK1 * (3.0 / 4.0));

				if (multiplierBaseDamage > 0)
				{
					damage += (int) Math.Round((multiplierBaseDamage * axeArmorMultiDamage) - (multiplierBaseDamage));
				}
				damage += (int) Math.Round((sotnApi.AlucardApi.Str * axeArmorMultiSTR) - (sotnApi.AlucardApi.Str));
				damage += 2 + axeArmorFlatDamage;

				if (axeArmorShieldRightHand)
				{
					damage += shieldBonusDamage;
				}
				if (!axeArmorEmptyRightHand)
				{
					damage += 2;
				}
				if (axeArmorShieldLeftHand)
				{
					damage += shieldBonusDamage;
				}
				else
				{
					damage -= (int) Math.Round((sotnApi.AlucardApi.Str * .08));
					damage -= (int) Math.Round(equipmentSTR / 4.0);
				}
				if (axeArmorShieldINT >= 65)
				{ // Shield Rod Alt Damage
					damage += shieldRodBonus;
				}
			}
			else if (axeArmorShieldRightHand)
			{ // ...................
				damage = shieldBaseDamage;
				if (multiplierBaseDamage > 0)
				{
					damage += (int) Math.Round((multiplierBaseDamage * axeArmorMultiDamage) - (multiplierBaseDamage));
				}
				damage += (int) Math.Round((sotnApi.AlucardApi.Str * axeArmorMultiSTR) - (sotnApi.AlucardApi.Str));
				damage += 2 + axeArmorFlatDamage;

				if (equipmentATK2 > 0 && !axeArmorLimitedLeftHand)
				{
					damage += (int) Math.Round((equipmentSTR * .25)) + 1;
					damage += (int) Math.Round((sotnApi.AlucardApi.Str * .08)) + 1;
				}
				if (offHandBonus > 0 && !axeArmorLimitedLeftHand)
				{ //Add .35 offhand damage + 1, round up
					damage += (int) Math.Round((offHandBonus) * .35) +1;
					handsWithDamage = 1;
				}
				if (axeArmorShieldLeftHand)
				{ // Apply Shield Bonus Damage
					damage += shieldBonusDamage;
				}
				if (axeArmorShieldINT >= 65)
				{ // Shield Rod Alt Damage
					damage += shieldRodBonus;
				}
			}
			else
			{
				damage += axeArmorFlatDamage;
				damage -= 50;
			}

			if (hasToothOfVlad)
			{
				vladBonus = 0;
				double vladScaling = 0.0;

				if (axeArmorTwoHanded)
				{
					vladScaling = 4;
				}
				else
				{
					if (axeArmorShieldRightHand || axeArmorEmptyRightHand)
					{
						vladScaling += 1;
					}
					else if (equipmentATK1 > 0 && !axeArmorLimitedRightHand)
					{
						vladScaling += 1.5;
					}
					if (axeArmorShieldLeftHand || axeArmorEmptyLeftHand)
					{
						vladScaling += 1;
					}
					else if (equipmentATK2> 0 && !axeArmorLimitedLeftHand)
					{
						vladScaling += 1.5;
					}
					
				}
				vladBonus += (int) Math.Round((sotnApi.AlucardApi.Str * .070) * vladScaling);
				damage += vladBonus;
			}

			int weaponDamage = damage;
			//Remove Vanilla INT from Damage Formula
			damage += flatDamage;

			if(axeArmorGurthang && gurthangBoostTimer > 0) 
			{
				damage += 25;
			}


			removeVanillaINTScaling(damage, out vanillaINTScaling, out damage);

			if (sotnApi.GameApi.IsInMenu())
			{
				notificationService.WeaponMessage = $"WPN: {weaponDamage} DMG, x{axeArmorMeleeHits} Hits";
				notificationService.EquipMessage = $"SPL/SUBWPN INT: +{axeArmorShieldINT}";
				//notificationService.WeaponMessage = $"Wpn: {weaponDamage}, Adj: {damage}, SHLD: {axeArmorShieldINT}";
				//notificationService.EquipMessage = $"NewINT: {newINTScaling}, Vanilla:{vanillaINTScaling}";
			}
			else
			{
				notificationService.WeaponMessage = "";
				notificationService.EquipMessage = "";
			}

			if (logDamage || requireLogs)
			{
				Console.WriteLine($"Sword Familiar={axeArmorSwordFamiliar};Gurthang={axeArmorGurthang}; Thrust={axeArmorThrust}; 2H={axeArmorTwoHanded}; RH={sotnApi.AlucardApi.RightHand}; LH={sotnApi.AlucardApi.LeftHand}; weaponDamage={weaponDamage} adjustedDamage={damage}; flatDamage{flatDamage}; vanillaINTScaling={vanillaINTScaling}; ATK1={equipmentATK1}; ATK2={equipmentATK2}; EquipINT={equipmentINT}; EquipSTR={equipmentSTR}; BaseSTR={sotnApi.AlucardApi.Str}; vladBonus={vladBonus}; AxeFlatDamage={axeArmorFlatDamage}; AxeMultiDMG={axeArmorMultiDamage}; AxeMultiSTR={axeArmorMultiSTR}; offHandUnscaled={offHandBonus}; " +
					$"multiplierBaseDamage={multiplierBaseDamage}; handsWithDamage={handsWithDamage}; emptyLeftHand={axeArmorEmptyLeftHand}; emptyRight={axeArmorEmptyRightHand}; 2H={axeArmorTwoHanded}; R-Shield={axeArmorShieldRightHand}; L-Shield={axeArmorShieldLeftHand}; axeArmorShieldINT={axeArmorShieldINT}; s-base={shieldBaseDamage}; s-bonus={shieldBonusDamage}; s-rod={shieldRodBonus}");
			}
		}

		private void removeVanillaINTScaling(int damage, out int vanillaINTScaling, out int adjustedDamage)
		{
			vanillaINTScaling = (int) Math.Round((equipmentINT * (8.0 / 10.0)) + (sotnApi.AlucardApi.Int / 5.0));

			if (vanillaINTScaling < 0 && damage + vanillaINTScaling > 0)
			{
				damage += vanillaINTScaling;
			}
			if (vanillaINTScaling < 0 && damage + vanillaINTScaling < 0)
			{
				if (damage < 0)
				{
					damage = -vanillaINTScaling;
				}
				else
				{
					damage = -(vanillaINTScaling + damage) - 1;
				}
			}
			else if (vanillaINTScaling > 0 && damage - vanillaINTScaling < 0)
			{
				damage = vanillaINTScaling - 1;
			}
			else
			{
				damage -= vanillaINTScaling;
			}
			adjustedDamage = damage;

			if(adjustedDamage < 1)
			{
				adjustedDamage = 1;
			}
		}

		private void CheckAxeArmorWeapon()
		{
			int leftHandIndex = (int) sotnApi.AlucardApi.LeftHand;
			int rightHandIndex = (int) sotnApi.AlucardApi.RightHand;

			int damageFlatModifier = 0;
			double damageMultiModifier = 1.0;
			double strMultiModifier = 1.0;

			//bool isShieldRodBonus = false;
			bool isTwoHanded = false;
			bool isThrust = false;
			bool leftHandLimitedWpn = false;
			bool rightHandLimitedWpn = false;
			bool leftHandShield = false;
			bool rightHandShield = false;

			
			

			if (axeArmorLeftHand != leftHandIndex || axeArmorRightHand != rightHandIndex) 
			{
				axeArmorShieldINT = 0;
				uint shieldBonusINT = 0;

				bool isSwordFamiliar = false;

				if (rightHandIndex == Equipment.Items.IndexOf("Sword Familiar")
				|| leftHandIndex == Equipment.Items.IndexOf("Sword Familiar"))
				{
					isSwordFamiliar = true;
				}

				axeArmorSwordFamiliar = isSwordFamiliar;

				if (rightHandIndex == Equipment.Items.IndexOf("Shield rod")
				|| rightHandIndex == Equipment.Items.IndexOf("Mablung Sword"))
				{
					if (Array.Exists(Constants.Khaos.axeArmorShields, x => x == leftHandIndex))
					{ //Shield Rod Bonus if Shield is Held
						damageFlatModifier += 10;
						damageMultiModifier += .5;
						SetAxeArmorShieldBonusInt(leftHandIndex, out shieldBonusINT);
						axeArmorShieldINT += shieldBonusINT;
						axeArmorShieldINT += 60;
					}
				}
				else if (Array.Exists(Constants.Khaos.axeArmorShields, x => x == rightHandIndex))
				{
					if (rightHandIndex == Equipment.Items.IndexOf("Dark shield"))
					{ //Dark, Dark, DoubleHit
						rightHandShield = true;
						SetAxeArmorWeaponQualities(2, 8, 8, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					}
					else if (rightHandIndex == Equipment.Items.IndexOf("Medusa shield"))
					{ //Stone, Stone, DoubleHit
						rightHandShield = true;
						SetAxeArmorWeaponQualities(2, 2, 2, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					}
					else if (rightHandIndex == Equipment.Items.IndexOf("AxeLord shield"))
					{ //Stone, Stone, DoubleHit
						rightHandShield = true;
						SetAxeArmorWeaponQualities(2, 192, 4, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					}
					else if (rightHandIndex == Equipment.Items.IndexOf("Fire shield"))
					{ //Fire, SingleHit
						rightHandShield = true;
						SetAxeArmorWeaponQualities(1, 0, 128, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					}
					else if (rightHandIndex == Equipment.Items.IndexOf("Skull shield"))
					{   //Poison, Water - Single Hit
						rightHandShield = true;
						SetAxeArmorWeaponQualities(3, 192, 4, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					}
					else
					{   //Poison, Water - Single Hit
						rightHandShield = true;
						SetAxeArmorWeaponQualities(1, 192, 4, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					}

					SetAxeArmorShieldBonusInt(rightHandIndex, out shieldBonusINT);
					axeArmorShieldINT += shieldBonusINT;


					if (leftHandIndex == Equipment.Items.IndexOf("Shield rod")
						|| leftHandIndex == Equipment.Items.IndexOf("Mablung Sword"))
					{
						axeArmorShieldINT += 60;
					}
					else if (Array.Exists(Constants.Khaos.axeArmorShields, x => x == leftHandIndex))
					{
						SetAxeArmorShieldBonusInt(leftHandIndex, out shieldBonusINT);
						axeArmorShieldINT += shieldBonusINT;
					}

				}
				else if (Array.Exists(Constants.Khaos.axeArmorShields, x => x == leftHandIndex))
				{
					SetAxeArmorShieldBonusInt(leftHandIndex, out shieldBonusINT);
					axeArmorShieldINT += shieldBonusINT;
				}
			}

			if (axeArmorLeftHand == leftHandIndex)
			{
				//Do nothing this patch for projectiles?
			}
			else {
				bool emptyLeftHand = leftHandIndex == (uint) Equipment.Items.IndexOf("empty hand") ? true : false;

				if (Array.Exists(Constants.Khaos.axeArmorProjectiles, x => x == leftHandIndex))
				{ //All Projectile Placeholder
					leftHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorShields, x => x == leftHandIndex))
				{ // Basic Shield Bonus
					axeArmorRightHand = 255;
					leftHandShield = true;
				}

				axeArmorLeftHand = leftHandIndex;
				axeArmorEmptyLeftHand = emptyLeftHand;
				axeArmorShieldLeftHand = leftHandShield;
				axeArmorLimitedLeftHand = leftHandLimitedWpn;
			}

			if (axeArmorRightHand == rightHandIndex)
			{
				if (inputService.ButtonReleased(PlaystationInputKeys.Square, Globals.UpdateCooldownFrames))
				{
					axeArmorAllowWeaponConsume = true;
				}
			}
			else
			{
				bool emptyRightHand = rightHandIndex == Equipment.Items.IndexOf("empty hand") ? true : false;

				if (Array.Exists(Constants.Khaos.axeArmorShields, x => x == rightHandIndex))
				{
					//Do Nothing
				}
				else if (emptyRightHand)
				{ // Unarmed
					SetAxeArmorWeaponQualities(1, 0, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					damageFlatModifier += 2;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorStone, x => x == rightHandIndex))
				{ //Stone, Stone: Double Hit, Damage - 6
					SetAxeArmorWeaponQualities(2, 2, 2, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (Array.Exists(Constants.Khaos.axeArmorThrowableWeapon, x => x == rightHandIndex))
				{ //Cut, Extra Range, Double Hit, Damage - 6
					SetAxeArmorWeaponQualities(2, 64, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (Array.Exists(Constants.Khaos.axeArmorOneHandedCut, x => x == rightHandIndex))
				{ //--Cut
					SetAxeArmorWeaponQualities(1, 64, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (Array.Exists(Constants.Khaos.axeArmorOneHandedDoubleCut, x => x == rightHandIndex))
				{ //Cut, Double Hit, Damage - 6
					SetAxeArmorWeaponQualities(2, 64, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (Array.Exists(Constants.Khaos.axeArmorOneHandedTripleCut, x => x == rightHandIndex))
				{ //Cut, Cut, Triple Hit, Attack - 12;
					SetAxeArmorWeaponQualities(3, 64, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (Array.Exists(Constants.Khaos.axeArmorCutDark, x => x == rightHandIndex))
				{ //Cut, Dark
					SetAxeArmorWeaponQualities(1, 64, 8, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (Array.Exists(Constants.Khaos.axeArmorOneHandedHit, x => x == rightHandIndex))
				{ //Hit, Hit, Single Hit
					SetAxeArmorWeaponQualities(1, 0, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					damageFlatModifier += 4;
					damageMultiModifier += 1.25;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorOneHandedDoubleHit, x => x == rightHandIndex))
				{ //Hit, Hit, Double Hit
					SetAxeArmorWeaponQualities(2, 0, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					damageFlatModifier += 4;
					damageMultiModifier += 1.25;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorOneHandedTripleHit, x => x == rightHandIndex))
				{ //Hit, Hit, Triple Hit
					SetAxeArmorWeaponQualities(3, 0, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (Array.Exists(Constants.Khaos.axeArmorTwoHandedCurse, x => x == rightHandIndex))
				{ //Curse, Curse
					SetAxeArmorWeaponQualities(1, 1, 1, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					isTwoHanded = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorTwoHandedHit, x => x == rightHandIndex))
				{ //Hit, Hit
					SetAxeArmorWeaponQualities(1, 0, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					isTwoHanded = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorTwoHandedDoubleHit, x => x == rightHandIndex))
				{ //Hit, Hit, Damage - 6
					SetAxeArmorWeaponQualities(2, 0, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					isTwoHanded = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorTwoHandedCut, x => x == rightHandIndex))
				{ //Cut, Cut
					SetAxeArmorWeaponQualities(1, 64, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					isTwoHanded = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorTwoHandedDoubleCut, x => x == rightHandIndex))
				{ //Cut, Cut, Double Hit
					SetAxeArmorWeaponQualities(2, 64, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					isTwoHanded = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesCut, x => x == rightHandIndex))
				{ //Cut: Projectile Placeholder, Single Hit
					SetAxeArmorWeaponQualities(1, 64, 0, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesDoubleCut, x => x == rightHandIndex))
				{ //Cut: Projectile Placeholder, Double Hit
					SetAxeArmorWeaponQualities(2, 64, 0, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesHit, x => x == rightHandIndex))
				{ //Hit: Single Hit
					SetAxeArmorWeaponQualities(1, 0, 0, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesDoubleHit, x => x == rightHandIndex))
				{ //Hit: Double Hit
					SetAxeArmorWeaponQualities(2, 0, 0, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesTripleHit, x => x == rightHandIndex))
				{ //Hit: Triple Hit
					SetAxeArmorWeaponQualities(3, 0, 0, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesQuadrupleHit, x => x == rightHandIndex))
				{ //Hit: Triple Hit
					SetAxeArmorWeaponQualities(4, 0, 0, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesThunder, x => x == rightHandIndex))
				{ //Thunder: Single Hit
					SetAxeArmorWeaponQualities(1, 0, 64, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesFire, x => x == rightHandIndex))
				{ //Fire: Single Hit
					SetAxeArmorWeaponQualities(1, 0, 128, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesDoubleFire, x => x == rightHandIndex))
				{ //Fire: Double Hit
					SetAxeArmorWeaponQualities(2, 0, 128, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (Array.Exists(Constants.Khaos.axeArmorProjectilesTripleFire, x => x == rightHandIndex))
				{ //Fire: Triple Hit
					SetAxeArmorWeaponQualities(3, 0, 128, true, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
					rightHandLimitedWpn = true;
				}
				else if (rightHandIndex == Equipment.Items.IndexOf("Icebrand"))
				{ //Cut, Ice, Doublehit, Damage - 6
					SetAxeArmorWeaponQualities(2, 64, 32, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (rightHandIndex == Equipment.Items.IndexOf("Firebrand"))
				{ //Cut, Fire, Doublehit, Damage -6
					SetAxeArmorWeaponQualities(2, 64, 128, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (rightHandIndex == Equipment.Items.IndexOf("Thunderbrand"))
				{ //Cut, Thunder, Doublehit, Damage - 6
					SetAxeArmorWeaponQualities(2, 64, 64, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (rightHandIndex == Equipment.Items.IndexOf("Mormegil"))
				{ //Cut, Dark, Double Hit, Damage - 6
					SetAxeArmorWeaponQualities(2, 64, 8, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (rightHandIndex == Equipment.Items.IndexOf("Holy sword"))
				{ //Cut, Holy, Double Hit, Damage - 6
					SetAxeArmorWeaponQualities(2, 64, 16, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (rightHandIndex == Equipment.Items.IndexOf("Terminus Est"))
				{ //Poison, Double Hit, Damage - 6
					SetAxeArmorWeaponQualities(2, 192, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (rightHandIndex == Equipment.Items.IndexOf("Marsil"))
				{ //Cut, Fire: Quadruple Hit, Attack - 18;
					SetAxeArmorWeaponQualities(4, 64, 128, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else if (rightHandIndex == Equipment.Items.IndexOf("Crissaegrim"))
				{ //Cut, Cut, Quadruple Hit, Attack - 18;
					SetAxeArmorWeaponQualities(4, 64, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}
				else
				{
					SetAxeArmorWeaponQualities(0, 64, 0, false, out damageFlatModifier, out damageMultiModifier, out strMultiModifier);
				}

				if (Constants.Khaos.ThrustWeapons.Contains(Equipment.Items[(int) (sotnApi.AlucardApi.RightHand)]))
				{
					isThrust = true;
				}

				bool isGurthang = false;

				if (rightHandIndex == Equipment.Items.IndexOf("Gurthang"))
				{
					isGurthang = true;
				}

				axeArmorGurthang = isGurthang;
				axeArmorAllowWeaponConsume = false;
				axeArmorThrust = isThrust;
				axeArmorTwoHanded = isTwoHanded;
				axeArmorFlatDamage = damageFlatModifier;
				axeArmorMultiDamage = damageMultiModifier;
				axeArmorMultiSTR = strMultiModifier;
				axeArmorEmptyRightHand = emptyRightHand;
				axeArmorLimitedRightHand = rightHandLimitedWpn;
				axeArmorShieldRightHand = rightHandShield;
				axeArmorRightHand = rightHandIndex;
			}
		}

		private void SetAxeArmorShieldBonusInt(in int handIndex, out uint shieldBonusInt)
		{
			shieldBonusInt = 0;
			if (handIndex == Equipment.Items.IndexOf("Alucart shield"))
			{
				shieldBonusInt = 6;
			}
			if (handIndex == Equipment.Items.IndexOf("Leather shield"))
			{
				shieldBonusInt = 8;
			}
			else if (handIndex == Equipment.Items.IndexOf("Knight shield") || handIndex == Equipment.Items.IndexOf("Iron shield"))
			{
				shieldBonusInt = 12;
			}
			else if (handIndex == Equipment.Items.IndexOf("Skull shield") || handIndex == Equipment.Items.IndexOf("Medusa shield") || handIndex == Equipment.Items.IndexOf("Dark shield"))
			{
				shieldBonusInt = 16;
			}
			else if (handIndex == Equipment.Items.IndexOf("Fire shield") || handIndex == Equipment.Items.IndexOf("Herald shield"))
			{
				shieldBonusInt = 20;
			}
			else if (handIndex == Equipment.Items.IndexOf("Shaman shield") || handIndex == Equipment.Items.IndexOf("Goddess shield"))
			{
				shieldBonusInt = 24;
			}
			else if (handIndex == Equipment.Items.IndexOf("AxeLord shield"))
			{
				shieldBonusInt = 28;
			}
			else if (handIndex == Equipment.Items.IndexOf("Alucard shield"))
			{
				shieldBonusInt = 32;
			}
		}
		

		private void SetAxeArmorWeaponQualities (in int numberOfHits, in int damageTypeA, in int damageTypeB, in bool limitedUse, out int damageFlatModifier, out double damageMultiModifier, out double strMultiModifier)
		{
			damageFlatModifier = 0;
			damageMultiModifier = 1.0;
			strMultiModifier = 1.0;

			axeArmorDamageTypeA.PokeValue(damageTypeA);
			axeArmorDamageTypeB.PokeValue(damageTypeB);
			axeArmorMeleeHits = numberOfHits;

			//Console.log(numberOfHits, damageTypeA, damageTypeB, limitedUse, damageFlatModifier, damageMultiModifier, strMultiModifier);

			switch (numberOfHits)
			{
				case 0:
					damageFlatModifier = -50;
					damageFlatModifier = -50;
					damageMultiModifier = 0.0;
					axeArmorDamageInterval.PokeValue(18);
					break;
				case 1:
					damageFlatModifier -= 4;
					damageMultiModifier = 1.0;
					strMultiModifier = 1.0;
					axeArmorDamageInterval.PokeValue(18);
					break;
				case 2:
					damageFlatModifier -= 5;
					damageMultiModifier = .85;
					strMultiModifier =.85;
					axeArmorDamageInterval.PokeValue(10);
					break;
				case 3:
					damageFlatModifier -= 6;
					damageMultiModifier = .78;
					strMultiModifier = .78;
					axeArmorDamageInterval.PokeValue(8);
					break;
				case 4:
					damageFlatModifier -= 7;
					damageMultiModifier = .70;
					strMultiModifier = .70;
					axeArmorDamageInterval.PokeValue(5);
				break;
				default:
					damageFlatModifier = -50;
					damageMultiModifier = 0.0;
					axeArmorDamageInterval.PokeValue(16);
					break;
			};
		if (limitedUse)
		{
			damageFlatModifier = 0;
			damageMultiModifier = 1.0;			
			strMultiModifier = 1.0;
		}
	}

	private void CheckAxeArmorFireballs()
		{
			fireballs.RemoveAll(f => f.Damage == 0 || f.Damage == 80);
			fireballsUp.RemoveAll(f => f.Damage == 0 || f.Damage == 80);
			fireballsDown.RemoveAll(f => f.Damage == 0 || f.Damage == 80);

			uint baseFireballDamage = 8;
			uint fireBallDamage;

			//Mini Fireball
			if (!heartsOnlyActive)
			{
				double baseINTModifier = .24;
				double equipINTModifier = .36;

				if (sotnApi.AlucardApi.HasRelic(Relic.SoulOfBat))
				{
					baseINTModifier += .04;
					equipINTModifier += .06;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.EchoOfBat))
				{
					baseINTModifier += .04;
					equipINTModifier += .06;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.ForceOfEcho))
				{
					baseINTModifier += .04;
					equipINTModifier += .06;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.FireOfBat))
				{
					baseINTModifier += .08;
					equipINTModifier += .12;
				}
				if (sotnApi.AlucardApi.HasRelic(Relic.EyeOfVlad))
				{
					baseFireballDamage += 8;
				}
				if (hasMojoMail)
				{
					baseFireballDamage += 8;
				}

				baseFireballDamage += (uint) (8 + ((sotnApi.AlucardApi.Int) * baseINTModifier) + ((equipmentINT + axeArmorShieldINT) * equipINTModifier));

				if (IsInRoomList(Constants.Khaos.GalamothRooms))
				{
					baseFireballDamage = baseFireballDamage / 3;
				}

				if (baseFireballDamage == 80)
				{
					baseFireballDamage = 81;
				}
				else if (baseFireballDamage < 1)
				{
					baseFireballDamage = 1;
				}
			}

			fireBallDamage = baseFireballDamage;

			foreach (var fball in fireballs)
			{
				fball.Damage = fireBallDamage;
				if (fball.SpeedHorizontal > 1)
				{
					fball.SpeedHorizontal = 4;
				}
				else
				{
					fball.SpeedHorizontal = -5;
				}
			}
			foreach (var fball in fireballsUp)
			{
				fball.Damage = fireBallDamage;
				if (fball.SpeedHorizontal > 1)
				{
					fball.SpeedHorizontal = 4;
				}
				else
				{
					fball.SpeedHorizontal = -5;
				}
				fball.SpeedVertical = -1;
			}
			foreach (var fball in fireballsDown)
			{
				fball.Damage = fireBallDamage;
				if (fball.SpeedHorizontal > 1)
				{
					fball.SpeedHorizontal = 4;
				}
				else
				{
					fball.SpeedHorizontal = -5;
				}
				fball.SpeedVertical = 2;
			}

			if(axeArmorFrameCount == Globals.UpdateCooldownFrames)
			{
				if (fireballCooldown > 0)
				{
					fireballCooldown--;
				}
			}
		}

		private void FireBallOff(Object sender, EventArgs e)
		{
			var fireBallCheat = cheats.GetCheatByName(Constants.Khaos.FireBallSpeedName);
			fireBallCheat.Disable();
			cheats.RemoveCheat(fireBallCheat);
			fireBallActive = false;
			fireBallTimer.Stop();
		}

		private void SpiritOff(Object sender, EventArgs e)
		{
			var lockOnCheat = cheats.GetCheatByName(Constants.Khaos.SpiritLockOnName);
			lockOnCheat.Disable();
			cheats.RemoveCheat(lockOnCheat);
			//spiritActive = false;
			spiritTimer.Stop();
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
			if (!toolConfig.Khaos.DisableMayhemMeter)
			{
				notificationService.KhaosMeter += meter;
			}
		}
		private void SpendMayhemMeter()
		{
			if (!toolConfig.Khaos.DisableMayhemMeter)
			{
				notificationService.KhaosMeter -= 100;
			}
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