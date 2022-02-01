using System;
using System.Windows.Forms;
using BizHawk.Client.Common;
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
		private bool started = false;

		public KhaosForm(IToolConfig toolConfig, CheatCollection cheats, ISotnApi sotnApi, IGameApi gameApi, IAlucardApi alucardApi, IActorApi actorApi, INotificationService notificationService, IInputService inputService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (cheats == null) throw new ArgumentNullException(nameof(cheats));
			this.toolConfig = toolConfig;

			adaptedCheats = new CheatCollectionAdapter(cheats);
			khaosControler = new KhaosController(toolConfig, sotnApi, gameApi, alucardApi, actorApi, adaptedCheats, notificationService, inputService);

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
				startButton.Text = "Start";
			}
			else
			{
				started = true;
				khaosControler.StartMayhem();
				startButton.Text = "Stop";
			}
		}

		#region Neutral Effects
		private void painTradeButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "paintrade", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.PainTrade();
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
		private void extraRangeButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "extrareach", UserName = "Mayhem" });
			}
			else
			{
				khaosControler.ExtraRange();
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

		#endregion

		#region Khaotic effects
		private void randomStatusButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "kstatus", UserName = "Legacy" });
			}
			else
			{
				khaosControler.KhaosStatus();
			}
		}
		private void randomEquipmentButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "kequipment", UserName = "Legacy" });
			}
			else
			{

				khaosControler.KhaosEquipment();
			}
		}
		private void randomizeStatsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "kstats", UserName = "Legacy" });
			}
			else
			{

				khaosControler.KhaosStats();
			}
		}
		private void randomizeRelicsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "krelics", UserName = "Legacy" });
			}
			else
			{

				khaosControler.KhaosRelics();
			}
		}
		private void pandorasBoxButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "pandora", UserName = "Legacy" });
			}
			else
			{

				khaosControler.PandorasBox();
			}
		}
		private void bankruptButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "bankrupt", UserName = "Legacy" });
			}
			else
			{
				khaosControler.Bankrupt();
			}
		}
		private void gambleButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "gamble", UserName = "Legacy" });
			}
			else
			{

				khaosControler.Gamble();
			}
		}
		private void kburstButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "kburst", UserName = "Legacy" });
			}
			else
			{

				khaosControler.FillMeter();
			}
		}
		#endregion
		#region Debuffs

		private void weakenButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "weaken", UserName = "Legacy" });
			}
			else
			{
				khaosControler.Weaken();
			}
		}
		private void respawnBossesButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "respawnbosses", UserName = "Legacy" });
			}
			else
			{
				khaosControler.RespawnBosses();
			}
		}
		private void subsonlyButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "subsonly", UserName = "Legacy" });
			}
			else
			{

				khaosControler.SubweaponsOnly();
			}
		}
		private void honestGamerButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "honestgamer", UserName = "Legacy" });
			}
			else
			{

				khaosControler.HonestGamer();
			}
		}
		private void bloodManaButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "bloodmana", UserName = "Legacy" });
			}
			else
			{
				khaosControler.BloodMana();
			}
		}
		private void thurstButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "thirst", UserName = "Legacy" });
			}
			else
			{
				khaosControler.Thirst();
			}
		}
		private void hordeButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "horde", UserName = "Legacy" });
			}
			else
			{
				khaosControler.Horde();
			}
		}
		private void enduranceButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "endurance", UserName = "Legacy" });
			}
			else
			{
				khaosControler.Endurance();
			}
		}
		private void crippleButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "cripple", UserName = "Legacy" });
			}
			else
			{

				khaosControler.Cripple();
			}
		}
		private void hnkButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "hnk", UserName = "Legacy" });
			}
			else
			{

				khaosControler.HnK();
			}
		}
		#endregion
		#region Buffs
		private void lightHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "lighthelp", UserName = "Legacy" });
			}
			else
			{
				khaosControler.LightHelp();
			}
		}
		private void mediumHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "mediumhelp", UserName = "Legacy" });
			}
			else
			{

				khaosControler.MediumHelp();
			}
		}
		private void heavyHelpButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "heavyhelp", UserName = "Legacy" });
			}
			else
			{

				khaosControler.HeavytHelp();
			}
		}
		private void battleOrdersButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "battleorders", UserName = "Legacy" });
			}
			else
			{

				khaosControler.BattleOrders();
			}
		}
		private void magicianButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "magician", UserName = "Legacy" });
			}
			else
			{

				khaosControler.Magician();
			}
		}
		private void meltyButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "melty", UserName = "Legacy" });
			}
			else
			{

				khaosControler.MeltyBlood();
			}
		}
		private void vampireButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "vampire", UserName = "Legacy" });
			}
			else
			{

				khaosControler.Vampire();
			}
		}
		private void fourBeastsButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "fourbeasts", UserName = "Legacy" });
			}
			else
			{
				khaosControler.FourBeasts();
			}
		}
		private void zawarudoButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "zawarudo", UserName = "Legacy" });
			}
			else
			{
				khaosControler.ZaWarudo();
			}
		}
		private void hasteButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "haste", UserName = "Legacy" });
			}
			else
			{
				khaosControler.Haste();
			}
		}
		private void lordButton_Click(object sender, EventArgs e)
		{
			if (toolConfig.Khaos.ControlPannelQueueActions)
			{
				khaosControler.EnqueueAction(new EventAddAction { Command = "lord", UserName = "Legacy" });
			}
			else
			{
				khaosControler.Lord();
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

		private void actionsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void radioButton2_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void queueRadio_CheckedChanged_1(object sender, EventArgs e)
		{

		}
	}
}
