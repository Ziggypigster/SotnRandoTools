using System;
using System.Windows.Forms;
using BizHawk.Client.Common;
using BizHawk.Emulation.Common;
using SotnApi.Interfaces;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Khaos;
using SotnRandoTools.Khaos.Models;
using SotnRandoTools.Services;
using SotnRandoTools.Services.Adapters;

namespace SotnRandoTools
{
	public partial class KhaosForm : Form
	{
		private ICheatCollectionAdapter adaptedCheats;
		private readonly IToolConfig toolConfig;
		private KhaosController? khaosControler;
		//private System.Windows.Forms.Timer countdownTimer;

		private string heartOfVladLocation = String.Empty;
		private string toothOfVladLocation = String.Empty;
		private string ribOfVladLocation = String.Empty;
		private string ringOfVladLocation = String.Empty;
		private string eyeOfVladLocation = String.Empty;

		private string batLocation = String.Empty;
		private string mistLocation = String.Empty;
		private string jewelLocation = String.Empty;
		private string gravityBootsLocation = String.Empty;
		private string leapstoneLocation = String.Empty;
		private string mermanLocation = String.Empty;

		private bool started = false;
		private bool connected = false;


		public KhaosForm(IToolConfig toolConfig, CheatCollection cheats, ISotnApi sotnApi, IGameApi gameApi, IAlucardApi alucardApi, INotificationService notificationService, IInputService inputService, IMemoryDomains memoryDomains)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			this.toolConfig = toolConfig;
			if (memoryDomains == null) throw new ArgumentNullException(nameof(memoryDomains));

			adaptedCheats = new CheatCollectionAdapter(cheats, memoryDomains);
			khaosControler = new KhaosController(toolConfig, sotnApi, adaptedCheats, notificationService, inputService);

			InitializeComponent();
			SuspendLayout();
			ResumeLayout();
		}
		public ICheatCollectionAdapter AdaptedCheats
		{
			get => adaptedCheats;

			set
			{
				adaptedCheats = value;
				khaosControler.GetCheats();
			}
		}

		public void UpdateKhaosValues()
		{
			if (khaosControler is not null)
			{
				khaosControler.Update();
			}
		}
		public void UpdateAxeArmor()
		{
			if (khaosControler is not null)
			{
				khaosControler.AxeArmorInputs();
			}
		}

		private void Khaos_Load(object sender, EventArgs e)
		{
			this.Location = toolConfig.Khaos.Location;
			queueRadio.Checked = toolConfig.Khaos.ControlPannelQueueActions;
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				queueRadio.Checked = true;
			}
			else
			{
				instantRadio.Checked = true;
			}
		}
		private void KhaosForm_Move(object sender, EventArgs e)
		{
			if (this.Location.X > 0)
			{
				toolConfig.Khaos.Location = this.Location;
			}
		}
		private void queueRadio_CheckedChanged(object sender, EventArgs e)
		{
			if (queueRadio.Checked)
			{
				toolConfig.Khaos.ControlPannelQueueActions = true;
			}
			else
			{
				toolConfig.Khaos.ControlPannelQueueActions = false;
			}
		}
		private void startButton_Click(object sender, EventArgs e)
		{
			if (started)
			{
				started = false;
				khaosControler.StopMayhem();
				startButton.Enabled = false;
				startButton.Text = "Start";
				startButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				startButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
				connectButton.Enabled = true;
				autoMayhemButton.Enabled = true;
				connectButton.Text = "Connect Bot";
				connectButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				connectButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
				autoMayhemButton.Text = "Start Auto Mayhem";
				autoMayhemButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				autoMayhemButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
			}
			else
			{
				started = true;
				khaosControler.StartMayhem();
				startButton.Text = "Stop";
				startButton.BackColor = System.Drawing.Color.FromArgb(114, 32, 25);
				startButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(169, 19, 7);
				autoMayhemButton.Enabled = false;
				connectButton.Enabled = false;
			}
		}

		#region Debug Only
		private void libraryButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "library", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Library();
			}
		}
		private void rewindButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "rewind", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Rewind();
			}
		}

		private void forcedRewindButton_Click(object sender, EventArgs e)
		{
			khaosControler.Rewind("Mayhem", false);
		}

		private void minStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "minstats", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MinStats();
			}
		}

		private void axeArmorButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "axearmor", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.AxeArmor();
			}
		}

		private void logCurrentRoomButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.LogCurrentRoom();
				//khaosControler.EnqueueAction(new EventAddAction { Command = "logcurrentroom", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.LogCurrentRoom();
			}
		}

		#endregion

		#region Neutral Effects
		private void merchantButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "merchant", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Merchant();
			}
		}
		private void maxMayhemButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "maxmayhem", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MaxMayhem();
			}
		}
		private void heartsOnlyButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "heartsonly", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.HeartsOnly();
			}
		}
		private void unarmedButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "unarmed", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Unarmed();
			}
		}
		private void turboModeButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "turbomode", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.TurboMode();
			}
		}
		private void rushDownButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "rushdown", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.RushDown();
			}
		}
		private void swapStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "swapstats", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.SwapStats();
			}
		}
		private void swapEquipmentButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "swapequipment", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.SwapEquipment();
			}
		}
		private void swapRelicsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "swaprelics", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.SwapRelics();
			}
		}
		private void pandemoniumButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "pandemonium", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Pandemonium();
			}
		}

		#endregion

		#region Evil Effects
		private void minorTrapButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "minortrap", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MinorTrap();
			}
		}
		private void hpForMPCurseButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "hpformp", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.HPForMP();
			}
		}
		private void underwaterButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "underwater", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Underwater();
			}
		}
		private void getJuggledButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "getjuggled", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.GetJuggled();
			}
		}
		private void ambushButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "ambush", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Ambush();
			}
		}
		private void toughBossesButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "toughbosses", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ToughBosses();
			}
		}
		private void statDownButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "statsdown", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.StatsDown();
			}
		}
		private void confiscateButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "confiscate", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Confiscate();
			}
		}

		private void slamButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "slam", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Slam();
			}
		}

		private void trapButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "trap", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Trap();
			}
		}

		private void majorTrapButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "majortrap", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MajorTrap();
			}
		}

		private void hexButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "hex", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Hex();
			}
		}
		#endregion

		#region Good Effects

		private void minorBoonButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "minorboon", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MinorBoon();
			}
		}
		private void minorEquipmentButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "minorequipment", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MinorEquipment();
			}
		}
		private void minorItemsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "minoritems", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MinorItems();
			}
		}
		private void minorStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "minorstats", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MinorStats();
			}
		}
		private void moderateBoonButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "moderateboon", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ModerateBoon();
			}
		}
		private void moderateEquipmentButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "moderateequipment", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ModerateEquipment();
			}
		}
		private void moderateItemsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "moderateitems", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ModerateItems();
			}
		}
		private void moderateStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "moderatestats", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ModerateStats();
			}
		}
		private void majorBoonButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "majorboon", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MajorBoon();
			}
		}
		private void majorEquipmentButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "majorequipment", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MajorEquipment();
			}
		}
		private void majorItemsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "majoritems", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MajorItems();
			}
		}
		private void majorStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "majorstats", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.MajorStats();
			}
		}
		private void progressionStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "progressionstats", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ProgressionStats();
			}
		}
		private void potionsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "potions", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.GivePotions();
			}
		}


		private void speedButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "speed", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Speed();
			}
		}
		private void regenButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "regen", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Regen();
			}
		}

		private void itemsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "items", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.GiveItems();
			}
		}

		private void equipmentButton_Click(object sender, EventArgs e)
		{
			
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "equipment", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.GiveEquipment();
			}
		}


		private void buffButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "buff", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.GiveBuff();
			}
		}

		private void timeStopButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "timestop", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.TimeStop();
			}
		}
		private void extraRangeButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "extrarange", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ExtraRange();
			}
		}
		private void faceTankButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "facetank", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.FaceTank();
			}
		}
		private void spellCasterButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "spellcaster", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.SpellCaster();
			}
		}
		private void luckyButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "lucky", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Lucky();
			}
		}

		private void summonerButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "summoner", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.Summoner();
			}
		}
		private void boonButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "boon", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.GiveBoon();
			}
		}

		private void progressionButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "progression", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.GiveProgression();
			}
		}


		#endregion

		private void KhaosForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (started)
			{
				started = false;
				khaosControler.StopMayhem();
				startButton.Text = "Start";
			}
			khaosControler = null;
		}

		private void modePanel_Enter(object sender, EventArgs e)
		{

		}

		private void autoMayhemButton_Click(object sender, EventArgs e)
		{
			if(khaosControler.AutoMayhemOn == true)
			{
				khaosControler.AutoMayhemOn = false;
				autoMayhemButton.Text = "Start Auto Mayhem";
				autoMayhemButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				autoMayhemButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
				if (!connected)
				{
					startButton.Enabled = false;
				}
			}
			else
			{
				khaosControler.AutoMayhemOn = true;
				startButton.Enabled = true;
				autoMayhemButton.Text = "Stop Auto Mayhem";
				connectButton.Text = "Connect Bot";
				connected = false;
				connectButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				connectButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
				autoMayhemButton.BackColor = System.Drawing.Color.FromArgb(93, 56, 147);
				autoMayhemButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(145, 70, 255);
			}
		}

		private void connectButton_Click(object sender, EventArgs e)
		{
			if (connected)
			{
				connectButton.Text = "Connect Bot";
				connected = false;
				connectButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				connectButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
				if (!khaosControler.AutoMayhemOn)
				{
					startButton.Enabled = false;
				}
			}
			else
			{
				startButton.Enabled = true;
				connectButton.Text = "Disconnect Bot";
				connected = true;
				connectButton.BackColor = System.Drawing.Color.FromArgb(93, 56, 147);
				connectButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(145, 70, 255);
				khaosControler.AutoMayhemOn = false;
				autoMayhemButton.Text = "Start Auto Mayhem";
				autoMayhemButton.BackColor = System.Drawing.Color.FromArgb(17, 0, 17);
				autoMayhemButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(48, 20, 48);
			}
		}

		private void moderateTrapButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "moderatetrap", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ModerateTrap();
			}
		}

		private void spawnEntityButton_Click(object sender, EventArgs e)
		{
			khaosControler.SpawnEntity();
		}

		private void richterColorButton_Click(object sender, EventArgs e)
		{
			khaosControler.ChangeRichterColor();
		}

		private void spawnEntityIDTextBox_TextChanged(object sender, EventArgs e)
		{
			string boxText = spawnEntityIDTextBox.Text.Replace("%", "");
			int spawnID;
			bool result = Int32.TryParse(boxText, out spawnID);
			if (result)
			{
				toolConfig.Khaos.SpawnEntityID = (int) (spawnID);
			}
		}

		private void richterColorTextBox_TextChanged(object sender, EventArgs e)
		{
			string boxText = richterColorTextBox.Text.Replace("%", "");
			int spawnID;
			bool result = Int32.TryParse(boxText, out spawnID);
			if (result)
			{
				toolConfig.Khaos.RichterColor = (int) (spawnID);
			}
		}
	}
}
