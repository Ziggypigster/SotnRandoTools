using System;
using System.Drawing;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;
using SotnRandoTools.Services;

namespace SotnRandoTools
{
	public partial class KhaosSettingsPanel : UserControl
	{
		private readonly IToolConfig? toolConfig;
		private readonly INotificationService notificationService;
		private BindingSource actionsAlertsSource = new();
		private BindingSource actionsOtherSource = new();
		private BindingSource actionsAutoSource = new();
		private bool pauseDifficultyChange = false;

		public KhaosSettingsPanel(IToolConfig toolConfig, INotificationService notificationService)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			if (notificationService is null) throw new ArgumentNullException(nameof(notificationService));
			this.toolConfig = toolConfig;
			this.notificationService = notificationService;

			InitializeComponent();
		}

		private void KhaosSettingsPanel_Load(object sender, EventArgs e)
		{
			alertsCheckbox.Checked = toolConfig.Khaos.Alerts;
			namesPath.Text = toolConfig.Khaos.NamesFilePath;
			botApiKey.Text = toolConfig.Khaos.BotApiKey;
			volumeTrackBar.Value = toolConfig.Khaos.Volume;

			//General
			queueTextBox.Text = toolConfig.Khaos.QueueInterval.ToString();
			meterOnResetTextBox.Text = toolConfig.Khaos.MeterOnReset.ToString();
			enforceMinStatsCheckbox.Checked = toolConfig.Khaos.EnforceMinStats;

			spiritOrbOnCheckbox.Checked = toolConfig.Khaos.spiritOrbOn;
			faerieScrollOnCheckbox.Checked = toolConfig.Khaos.faerieScrollOn;
			cubeOfZoeOnCheckbox.Checked = toolConfig.Khaos.cubeOfZoeOn;

			boostFamiliarsCheckBox.Checked = toolConfig.Khaos.BoostFamiliars;
			continuousWingSmashCheckBox.Checked = toolConfig.Khaos.ContinuousWingsmash;
			dynamicIntervalCheckBox.Checked = toolConfig.Khaos.DynamicInterval;
			romhackModeCheckBox.Checked = toolConfig.Khaos.RomhackMode;

			//Auto-Mayhem
			autoMayhemDifficultyComboBox.SelectedIndex = toolConfig.Khaos.autoMayhemDifficulty;
			autoCommandSpeedComboBox.SelectedIndex = toolConfig.Khaos.autoCommandSpeed;
			autoCommandConsistencyComboBox.SelectedIndex = toolConfig.Khaos.autoCommandConsistency;
			autoMoodSwingsComboBox.SelectedIndex = toolConfig.Khaos.autoMoodSwings;

			autoPerfectMayhemTriggerTextBox.Text = toolConfig.Khaos.autoPerfectMayhemTrigger.ToString();
			allowPerfectMayhemCheckbox.Checked = toolConfig.Khaos.autoAllowPerfectMayhem;
			allowMayhemPityCheckBox.Checked = toolConfig.Khaos.autoAllowMayhemPity;
			allowMayhemRageCheckBox.Checked = toolConfig.Khaos.autoAllowMayhemRage;

			//allowSmartLogicCheckBox.Checked = toolConfig.Khaos.autoEnableSmartLogic;
			allowBlessingsCheckBox.Checked = toolConfig.Khaos.autoAllowBlessings;
			allowNeutralsCheckBox.Checked = toolConfig.Khaos.autoAllowNeutrals;
			allowCursesCheckBox.Checked = toolConfig.Khaos.autoAllowCurses;

			blessingsWeightTextBox.Text = toolConfig.Khaos.autoBlessingWeight.ToString();
			blessingsMoodTextBox.Text = toolConfig.Khaos.autoBlessingMood.ToString();
			blessingsMinTextBox.Text = toolConfig.Khaos.autoBlessingMin.ToString();
			blessingsMaxTextBox.Text = toolConfig.Khaos.autoBlessingMax.ToString();

			neutralsWeightTextBox.Text = toolConfig.Khaos.autoNeutralWeight.ToString();
			neutralsMoodTextBox.Text = toolConfig.Khaos.autoNeutralMood.ToString();
			neutralsMinTextBox.Text = toolConfig.Khaos.autoNeutralMin.ToString();
			neutralsMaxTextBox.Text = toolConfig.Khaos.autoNeutralMax.ToString();

			cursesWeightTextBox.Text = toolConfig.Khaos.autoCurseWeight.ToString();
			cursesMoodTextBox.Text = toolConfig.Khaos.autoCurseMood.ToString();
			cursesMinTextBox.Text = toolConfig.Khaos.autoCurseMin.ToString();
			cursesMaxTextBox.Text = toolConfig.Khaos.autoCurseMax.ToString();


			//Command
			blessingComboBox.SelectedIndex = toolConfig.Khaos.BlessingModifier;
			curseComboBox.SelectedIndex = toolConfig.Khaos.CurseModifier;
			neutralMinLevelTextBox.Text = toolConfig.Khaos.NeutralMinLevel.ToString();
			neutralStartLevelTextBox.Text = toolConfig.Khaos.NeutralStartLevel.ToString();
			neutralMaxLevelTextBox.Text = toolConfig.Khaos.NeutralMaxLevel.ToString();
			allowNeutralLevelResetCheckbox.Checked = toolConfig.Khaos.AllowNeutralLevelReset;

			underwaterTextBox.Text = (toolConfig.Khaos.UnderwaterFactor * 100) + "%";
			speedTextBox.Text = (toolConfig.Khaos.SpeedFactor * 100) + "%";
			statsDownTextBox.Text = (toolConfig.Khaos.StatsDownFactor * 100) + "%";
			regenTextBox.Text = toolConfig.Khaos.RegenGainPerSecond.ToString();
			pandemoniumMinTextBox.Text = toolConfig.Khaos.PandemoniumMinItems.ToString();
			pandemoniumMaxTextBox.Text = toolConfig.Khaos.PandemoniumMaxItems.ToString();
			keepVladRelicsCheckbox.Checked = toolConfig.Khaos.KeepVladRelics;
			restrictedRelicSwapCheckBox.Checked = toolConfig.Khaos.RestrictedRelicSwap;

			//Enemy
			cloneBossDMGComboBox.SelectedIndex = (int) toolConfig.Khaos.CloneBossDMGModifier;
			cloneBossHPComboBox.SelectedIndex = (int) toolConfig.Khaos.CloneBossHPModifier;
			singleBossDMGComboBox.SelectedIndex = (int) toolConfig.Khaos.SingleBossDMGModifier;
			singleBossHPComboBox.SelectedIndex = (int) toolConfig.Khaos.SingleBossHPModifier;
			galamothHPComboBox.SelectedIndex = (int) toolConfig.Khaos.GalamothBossHPModifier;
			galamothDMGComboBox.SelectedIndex = (int) toolConfig.Khaos.GalamothBossDMGModifier;
			galamothDefNerfCheckBox.Checked = toolConfig.Khaos.GalamothDefNerf;
			galamothIsRepositionedCheckBox.Checked = toolConfig.Khaos.GalamothIsRepositioned;
			shaftHPComboBox.SelectedIndex = (int) toolConfig.Khaos.ShaftOrbHPModifier;
			superBossHPComboBox.SelectedIndex = (int) toolConfig.Khaos.SuperBossHPModifier;
			superBossDMGComboBox.SelectedIndex = (int) toolConfig.Khaos.SuperBossDMGModifier;
			ambushHPComboBox.SelectedIndex = (int) toolConfig.Khaos.AmbushHPModifier;
			ambushDMGComboBox.SelectedIndex = (int) toolConfig.Khaos.AmbushDMGModifier;
			superAmbushHPComboBox.SelectedIndex = (int) toolConfig.Khaos.SuperAmbushHPModifier;
			superAmbushDMGComboBox.SelectedIndex = (int) toolConfig.Khaos.SuperAmbushDMGModifier;

			foreach (var action in toolConfig.Khaos.Actions)
			{
				actionsAlertsSource.Add(action);
				actionsOtherSource.Add(action);
				actionsAutoSource.Add(action);
			}
			alertsGridView.AutoGenerateColumns = false;
			alertsGridView.DataSource = actionsAlertsSource;
			alertsGridView.CellClick += AlertsGridView_BrowseClick;
			actionsGridView.AutoGenerateColumns = false;
			actionsGridView.DataSource = actionsOtherSource;
			autoMayhemGridView.AutoGenerateColumns = false;
			autoMayhemGridView.DataSource = actionsAutoSource;
		}

		private void AlertsGridView_BrowseClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex < 0 || e.ColumnIndex !=
			alertsGridView.Columns["Browse"].Index) return;
			alertFileDialog.Tag = e.RowIndex;
			alertFileDialog.ShowDialog();
		}
		private void alertFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			alertsGridView.Rows[(int) alertFileDialog.Tag].Cells[1].Value = alertFileDialog.FileName;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			toolConfig.SaveConfig();
		}

		private void volumeTrackBar_Scroll(object sender, EventArgs e)
		{
			toolConfig.Khaos.Volume = volumeTrackBar.Value;
			notificationService.Volume = (double) volumeTrackBar.Value / 10F;
		}

		private void alertsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.Alerts = alertsCheckbox.Checked;
		}

		private void namesBrowseButton_Click(object sender, EventArgs e)
		{
			namesFileDialog.ShowDialog();
		}

		private void namesFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
		{
			namesPath.Text = namesFileDialog.FileName;
			toolConfig.Khaos.NamesFilePath = namesFileDialog.FileName;
		}

		private void queueTextBox_Validated(object sender, EventArgs e)
		{
			TimeSpan queueInterval;
			bool result = TimeSpan.TryParse(queueTextBox.Text, out queueInterval);
			if (result)
			{
				toolConfig.Khaos.QueueInterval = queueInterval;
			}
			queueTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void queueTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			TimeSpan queueInterval;
			TimeSpan minSpan = new TimeSpan(0, 0, 10);
			TimeSpan maxSpan = new TimeSpan(0, 10, 0);
			bool result = TimeSpan.TryParse(queueTextBox.Text, out queueInterval);
			if (!result)
			{
				this.queueTextBox.Text = "";
				this.queueTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(queueTextBox, "Invalid value! Format: (hh:mm:ss)");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
			if (queueInterval < minSpan || queueInterval > maxSpan)
			{
				this.queueTextBox.Text = "";
				this.queueTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(queueTextBox, "Value must be greater than 10 seconds and lower than 10 minutes!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void dynamicIntervalCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.DynamicInterval = dynamicIntervalCheckBox.Checked;
		}

		private void botApiKey_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.BotApiKey = botApiKey.Text;
		}

		#region General
		private void romhackModeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.RomhackMode = romhackModeCheckBox.Checked;
		}

		private void resetToDefaultButton_Click(object sender, EventArgs e)
		{
			toolConfig.Khaos.DefaultSettings();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void continuousWingSmashCheckBox_CheckChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.ContinuousWingsmash = continuousWingSmashCheckBox.Checked;
		}

		private void meterOnResetTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int meterOnReset;
			bool result = Int32.TryParse(meterOnResetTextBox.Text, out meterOnReset);
			if (!result || meterOnReset < 0 || meterOnReset > 101)
			{
				this.meterOnResetTextBox.Text = "";
				this.meterOnResetTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(meterOnResetTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void meterOnResetTextBox_Validated(object sender, EventArgs e)
		{
			int meterOnReset;
			bool result = Int32.TryParse(meterOnResetTextBox.Text, out meterOnReset);
			if (result)
			{
				toolConfig.Khaos.MeterOnReset = meterOnReset;
			}
			meterOnResetTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}
		#endregion
		#region Command
		private void curseComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.CurseModifier = curseComboBox.SelectedIndex;
		}

		private void neutralMinLevelTextbox_Validated(object sender, EventArgs e)
		{
			int neutralMinLevel;
			bool result = Int32.TryParse(neutralMinLevelTextBox.Text, out neutralMinLevel);
			if (result)
			{
				toolConfig.Khaos.NeutralMinLevel = neutralMinLevel;
			}
			neutralMinLevelTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void neutralMinLevelTextbox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int neutralMinLevel;
			bool result = Int32.TryParse(neutralMinLevelTextBox.Text, out neutralMinLevel);
			if (!result || neutralMinLevel < 1 || neutralMinLevel > 3)
			{
				this.neutralMinLevelTextBox.Text = "";
				this.neutralMinLevelTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(neutralMinLevelTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void neutralStartLevelTextbox_Validated(object sender, EventArgs e)
		{
			int neutralStartLevel;
			bool result = Int32.TryParse(neutralStartLevelTextBox.Text, out neutralStartLevel);
			if (result)
			{
				toolConfig.Khaos.NeutralStartLevel = neutralStartLevel;
			}
			neutralStartLevelTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void neutralStartLevelTextbox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int neutralStartLevel;
			bool result = Int32.TryParse(neutralMaxLevelTextBox.Text, out neutralStartLevel);
			if (!result || neutralStartLevel < 1 || neutralStartLevel > 3)
			{
				this.neutralStartLevelTextBox.Text = "";
				this.neutralStartLevelTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(neutralStartLevelTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}


		private void neutralMaxLevelTextbox_Validated(object sender, EventArgs e)
		{
			int neutralMaxLevel;
			bool result = Int32.TryParse(neutralMaxLevelTextBox.Text, out neutralMaxLevel);
			if (result)
			{
				toolConfig.Khaos.NeutralMaxLevel = neutralMaxLevel;
			}
			neutralMaxLevelTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void neutralMaxLevelTextbox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int neutralMaxLevel;
			bool result = Int32.TryParse(neutralMaxLevelTextBox.Text, out neutralMaxLevel);
			if (!result || neutralMaxLevel < 1 || neutralMaxLevel > 3)
			{
				this.neutralMaxLevelTextBox.Text = "";
				this.neutralMaxLevelTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(neutralMaxLevelTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void progressionGivesVladCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.ProgressionGivesVlad = progressionGivesVladCheckbox.Checked;
		}


		private void keepVladRelicsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.KeepVladRelics = keepVladRelicsCheckbox.Checked;
		}

		private void enforceMinStatsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.EnforceMinStats = enforceMinStatsCheckbox.Checked;
		}


		private void underwaterTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = underwaterTextBox.Text.Replace("%", "");
			int underwaterPercentage;
			bool result = Int32.TryParse(boxText, out underwaterPercentage);
			if (result)
			{
				toolConfig.Khaos.UnderwaterFactor = (underwaterPercentage / 100F);
			}
			underwaterTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void underwaterTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//if (this.ActiveControl.Equals(sender))
			//return;
			string boxText = underwaterTextBox.Text.Replace("%", "");
			int underwaterPercentage;
			bool result = Int32.TryParse(boxText, out underwaterPercentage);
			if (!result || underwaterPercentage < 0 || underwaterPercentage > 90)
			{
				this.underwaterTextBox.Text = "";
				this.underwaterTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(underwaterTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void speedTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = speedTextBox.Text.Replace("%", "");
			int speedPercentage;
			bool result = Int32.TryParse(boxText, out speedPercentage);
			if (result)
			{
				toolConfig.Khaos.SpeedFactor = (speedPercentage / 100F);
			}
			speedTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void speedTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = speedTextBox.Text.Replace("%", "");
			int speedPercentage;
			bool result = Int32.TryParse(boxText, out speedPercentage);
			if (!result || speedPercentage < 100 || speedPercentage > 1000)
			{
				this.speedTextBox.Text = "";
				this.speedTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(speedTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void statsDownTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = statsDownTextBox.Text.Replace("%", "");
			int statsDownPercentage;
			bool result = Int32.TryParse(boxText, out statsDownPercentage);
			if (result)
			{
				toolConfig.Khaos.StatsDownFactor = (statsDownPercentage / 100F);
			}
			statsDownTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void statsDownTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = statsDownTextBox.Text.Replace("%", "");
			int statsDownPercentage;
			bool result = Int32.TryParse(boxText, out statsDownPercentage);
			if (!result || statsDownPercentage < 10 || statsDownPercentage > 90)
			{
				this.statsDownTextBox.Text = "";
				this.statsDownTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(statsDownTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void regenTextBox_Validated(object sender, EventArgs e)
		{
			int regenGain;
			bool result = Int32.TryParse(regenTextBox.Text, out regenGain);
			if (result)
			{
				toolConfig.Khaos.RegenGainPerSecond = (uint) regenGain;
			}
			regenTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void regenTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int regenGain;
			bool result = Int32.TryParse(regenTextBox.Text, out regenGain);
			if (!result || regenGain < 1 || regenGain > 100)
			{
				this.regenTextBox.Text = "";
				this.regenTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(regenTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void pandemoniumMinTextBox_Validated(object sender, EventArgs e)
		{
			int pandemoniumMinItems;
			bool result = Int32.TryParse(pandemoniumMinTextBox.Text, out pandemoniumMinItems);
			if (result)
			{
				toolConfig.Khaos.PandemoniumMinItems = pandemoniumMinItems;
			}
			pandemoniumMinTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void pandemoniumMinTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int pandemoniumMinItems;
			bool result = Int32.TryParse(pandemoniumMinTextBox.Text, out pandemoniumMinItems);
			if (!result || pandemoniumMinItems < 0 || pandemoniumMinItems > 100)
			{
				this.pandemoniumMinTextBox.Text = "";
				this.pandemoniumMinTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(pandemoniumMinTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void pandemoniumMaxTextBox_Validated(object sender, EventArgs e)
		{
			int pandemoniumMaxItems;
			bool result = Int32.TryParse(pandemoniumMaxTextBox.Text, out pandemoniumMaxItems);
			if (result)
			{
				toolConfig.Khaos.PandemoniumMaxItems = pandemoniumMaxItems;
			}
			pandemoniumMaxTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void pandemoniumMaxTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int pandoraMaxItems;
			bool result = Int32.TryParse(pandemoniumMaxTextBox.Text, out pandoraMaxItems);
			if (!result || pandoraMaxItems < 1 || pandoraMaxItems > 100)
			{
				this.pandemoniumMaxTextBox.Text = "";
				this.pandemoniumMaxTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(pandemoniumMaxTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}
		#endregion

		#region Enemy Tab
		private void restrictedRelicSwapCheckBox_CheckedChange(object sender, EventArgs e)
		{
			toolConfig.Khaos.RestrictedRelicSwap = restrictedRelicSwapCheckBox.Checked;
		}
		private void cloneBossHPComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.CloneBossHPModifier = cloneBossHPComboBox.SelectedIndex;
		}

		private void cloneBossDMGComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.CloneBossDMGModifier = (uint) cloneBossDMGComboBox.SelectedIndex;
		}

		private void singleBossHPComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.SingleBossHPModifier = singleBossHPComboBox.SelectedIndex;
		}

		private void singleBossDMGComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.SingleBossDMGModifier = (uint) singleBossDMGComboBox.SelectedIndex;
		}

		private void galamothHPComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.GalamothBossHPModifier = galamothHPComboBox.SelectedIndex;
		}

		private void galamothDMGComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.GalamothBossDMGModifier = (uint) galamothDMGComboBox.SelectedIndex;
		}

		private void galamothDefNerfCheckBox_CheckChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.GalamothIsRepositioned = galamothDefNerfCheckBox.Checked;
		}

		private void galamothIsRepositionedCheckBox_CheckChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.GalamothIsRepositioned = galamothIsRepositionedCheckBox.Checked;
		}
		private void shaftOrbHPComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.ShaftOrbHPModifier = shaftHPComboBox.SelectedIndex;
		}
		private void superBossHPComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.SuperBossHPModifier = superBossHPComboBox.SelectedIndex;
		}
		private void superBossDMGComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.SuperBossDMGModifier = (uint) superBossDMGComboBox.SelectedIndex;
		}
		private void ambushHPComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.AmbushHPModifier = ambushHPComboBox.SelectedIndex;
		}
		private void ambushDMGComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.AmbushDMGModifier = (uint) ambushDMGComboBox.SelectedIndex;
		}
		private void superAmbushHPComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.AmbushHPModifier = ambushHPComboBox.SelectedIndex;
		}
		private void superAmbushDMGComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.AmbushDMGModifier = (uint) ambushDMGComboBox.SelectedIndex;
		}
		#endregion

		private void blessingComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.BlessingModifier = blessingComboBox.SelectedIndex;
		}

		private void boostFamiliarsCheckBox_CheckChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.BoostFamiliars = boostFamiliarsCheckBox.Checked;
		}

		private void spiritOrbOnCheckBox_CheckChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.spiritOrbOn = spiritOrbOnCheckbox.Checked;
		}

		private void faerieScrollOnCheckBox_CheckChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.faerieScrollOn = faerieScrollOnCheckbox.Checked;
		}

		private void cubeOfZoeCheckBox_CheckChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.faerieScrollOn = faerieScrollOnCheckbox.Checked;
		}

		private void difficultyComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoMayhemDifficulty = autoMayhemDifficultyComboBox.SelectedIndex;
			setDifficulty();
			pauseDifficultyChange = true;
			KhaosSettingsPanel_Load(sender, e);
			pauseDifficultyChange = false;
		}
		private void setCustomDifficulty()
		{
			if (!pauseDifficultyChange)
			{
				toolConfig.Khaos.autoMayhemDifficulty = 0;
			}
		}

		private void setDifficulty()
		{	
			switch (toolConfig.Khaos.autoMayhemDifficulty)
			{
				case 1:
					toolConfig.Khaos.autoCommandSpeed = 0;
					toolConfig.Khaos.autoCommandConsistency = 0;
					toolConfig.Khaos.autoMoodSwings = 0;

					toolConfig.Khaos.autoPerfectMayhemTrigger = 1100;
					toolConfig.Khaos.autoAllowPerfectMayhem = true;
					toolConfig.Khaos.autoAllowMayhemPity = true;
					toolConfig.Khaos.autoAllowMayhemRage = false;

					toolConfig.Khaos.autoEnableSmartLogic = true;
					toolConfig.Khaos.autoAllowBlessings = true;
					toolConfig.Khaos.autoAllowNeutrals = true;
					toolConfig.Khaos.autoAllowCurses = true;

					toolConfig.Khaos.autoBlessingWeight = 7;
					toolConfig.Khaos.autoBlessingMood = 4;
					toolConfig.Khaos.autoBlessingMin = 2;
					toolConfig.Khaos.autoBlessingMax = 7;

					toolConfig.Khaos.autoNeutralWeight = 6;
					toolConfig.Khaos.autoNeutralMood = 2;
					toolConfig.Khaos.autoNeutralMin = 1;
					toolConfig.Khaos.autoNeutralMax = 6;

					toolConfig.Khaos.autoCurseWeight = 5;
					toolConfig.Khaos.autoCurseMood = 1;
					toolConfig.Khaos.autoCurseMin = 1;
					toolConfig.Khaos.autoCurseMax = 5;
					break;
				case 2:
					toolConfig.Khaos.autoCommandSpeed = 1;
					toolConfig.Khaos.autoCommandConsistency = 1;
					toolConfig.Khaos.autoMoodSwings = 1;

					toolConfig.Khaos.autoPerfectMayhemTrigger = 1000;
					toolConfig.Khaos.autoAllowPerfectMayhem = true;
					toolConfig.Khaos.autoAllowMayhemPity = true;
					toolConfig.Khaos.autoAllowMayhemRage = true;

					toolConfig.Khaos.autoEnableSmartLogic = true;
					toolConfig.Khaos.autoAllowBlessings = true;
					toolConfig.Khaos.autoAllowNeutrals = true;
					toolConfig.Khaos.autoAllowCurses = true;

					toolConfig.Khaos.autoBlessingWeight = 6;
					toolConfig.Khaos.autoBlessingMood = 3;
					toolConfig.Khaos.autoBlessingMin = 1;
					toolConfig.Khaos.autoBlessingMax = 6;

					toolConfig.Khaos.autoNeutralWeight = 6;
					toolConfig.Khaos.autoNeutralMood = 2;
					toolConfig.Khaos.autoNeutralMin = 1;
					toolConfig.Khaos.autoNeutralMax = 6;

					toolConfig.Khaos.autoCurseWeight = 6;
					toolConfig.Khaos.autoCurseMood = 2;
					toolConfig.Khaos.autoCurseMin = 1;
					toolConfig.Khaos.autoCurseMax = 6;
					break;
				case 3:
					toolConfig.Khaos.autoCommandSpeed = 2;
					toolConfig.Khaos.autoCommandConsistency = 2;
					toolConfig.Khaos.autoMoodSwings = 2;

					toolConfig.Khaos.autoPerfectMayhemTrigger = 900;
					toolConfig.Khaos.autoAllowPerfectMayhem = true;
					toolConfig.Khaos.autoAllowMayhemPity = true;
					toolConfig.Khaos.autoAllowMayhemRage = true;

					toolConfig.Khaos.autoEnableSmartLogic = true;
					toolConfig.Khaos.autoAllowBlessings = true;
					toolConfig.Khaos.autoAllowNeutrals = true;
					toolConfig.Khaos.autoAllowCurses = true;

					toolConfig.Khaos.autoBlessingWeight = 5;
					toolConfig.Khaos.autoBlessingMood = 2;
					toolConfig.Khaos.autoBlessingMin = 1;
					toolConfig.Khaos.autoBlessingMax = 5;

					toolConfig.Khaos.autoNeutralWeight = 6;
					toolConfig.Khaos.autoNeutralMood = 2;
					toolConfig.Khaos.autoNeutralMin = 1;
					toolConfig.Khaos.autoNeutralMax = 6;

					toolConfig.Khaos.autoCurseWeight = 7;
					toolConfig.Khaos.autoCurseMood = 3;
					toolConfig.Khaos.autoCurseMin = 2;
					toolConfig.Khaos.autoCurseMax = 7;
					break;
				case 4:
					toolConfig.Khaos.autoCommandSpeed = 3;
					toolConfig.Khaos.autoCommandConsistency = 3;
					toolConfig.Khaos.autoMoodSwings = 3;

					toolConfig.Khaos.autoPerfectMayhemTrigger = 800;
					toolConfig.Khaos.autoAllowPerfectMayhem = true;
					toolConfig.Khaos.autoAllowMayhemPity = true;
					toolConfig.Khaos.autoAllowMayhemRage = true;

					toolConfig.Khaos.autoEnableSmartLogic = true;
					toolConfig.Khaos.autoAllowBlessings = true;
					toolConfig.Khaos.autoAllowNeutrals = true;
					toolConfig.Khaos.autoAllowCurses = true;

					toolConfig.Khaos.autoBlessingWeight = 4;
					toolConfig.Khaos.autoBlessingMood = 1;
					toolConfig.Khaos.autoBlessingMin = 1;
					toolConfig.Khaos.autoBlessingMax = 4;

					toolConfig.Khaos.autoNeutralWeight = 6;
					toolConfig.Khaos.autoNeutralMood = 2;
					toolConfig.Khaos.autoNeutralMin = 1;
					toolConfig.Khaos.autoNeutralMax = 6;

					toolConfig.Khaos.autoCurseWeight = 8;
					toolConfig.Khaos.autoCurseMood = 4;
					toolConfig.Khaos.autoCurseMin = 3;
					toolConfig.Khaos.autoCurseMax = 8;
					break;
				case 5:
					toolConfig.Khaos.autoCommandSpeed = 4;
					toolConfig.Khaos.autoCommandConsistency = 4;
					toolConfig.Khaos.autoMoodSwings = 4;

					toolConfig.Khaos.autoPerfectMayhemTrigger = 700;
					toolConfig.Khaos.autoAllowPerfectMayhem = true;
					toolConfig.Khaos.autoAllowMayhemPity = false;
					toolConfig.Khaos.autoAllowMayhemRage = true;

					toolConfig.Khaos.autoEnableSmartLogic = true;
					toolConfig.Khaos.autoAllowBlessings = true;
					toolConfig.Khaos.autoAllowNeutrals = true;
					toolConfig.Khaos.autoAllowCurses = true;

					toolConfig.Khaos.autoBlessingWeight = 3;
					toolConfig.Khaos.autoBlessingMood = 0;
					toolConfig.Khaos.autoBlessingMin = 1;
					toolConfig.Khaos.autoBlessingMax = 3;

					toolConfig.Khaos.autoNeutralWeight = 5;
					toolConfig.Khaos.autoNeutralMood = 2;
					toolConfig.Khaos.autoNeutralMin = 1;
					toolConfig.Khaos.autoNeutralMax = 5;

					toolConfig.Khaos.autoCurseWeight = 9;
					toolConfig.Khaos.autoCurseMood = 5;
					toolConfig.Khaos.autoCurseMin = 4;
					toolConfig.Khaos.autoCurseMax = 9;
					break;
			}
		}
		private void moodSwingsComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoMoodSwings = autoMoodSwingsComboBox.SelectedIndex;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void autoPerfectMayhemTriggerTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = autoPerfectMayhemTriggerTextBox.Text.Replace("%", "");
			int autoRelicThreshold;
			bool result = Int32.TryParse(boxText, out autoRelicThreshold);
			if (result)
			{
				toolConfig.Khaos.autoPerfectMayhemTrigger = (int) (autoRelicThreshold);
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			autoPerfectMayhemTriggerTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void autoPerfectMayhemTriggerTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = autoPerfectMayhemTriggerTextBox.Text.Replace("%", "");
			int autoRelicThreshold;
			bool result = Int32.TryParse(boxText, out autoRelicThreshold);
			if (!result || autoRelicThreshold < 0 || autoRelicThreshold > 10000)
			{
				this.autoPerfectMayhemTriggerTextBox.Text = "";
				this.autoPerfectMayhemTriggerTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(autoPerfectMayhemTriggerTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void autoRelicThresholdTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = autoPerfectMayhemTriggerTextBox.Text.Replace("%", "");
			int autoRelicThreshold;
			bool result = Int32.TryParse(boxText, out autoRelicThreshold);
			if (!result || autoRelicThreshold < 0 || autoRelicThreshold > 10000)
			{
				this.autoPerfectMayhemTriggerTextBox.Text = "";
				this.autoPerfectMayhemTriggerTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(autoPerfectMayhemTriggerTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void allowSmartLogicCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			//toolConfig.Khaos.autoAllowSmartLogic = allowSmartLogicCheckBox.Checked;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}


		private void allowNeutralsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoAllowNeutrals = allowNeutralsCheckBox.Checked;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void allowCursesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoAllowCurses = allowCursesCheckBox.Checked;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void allowBlessingsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoAllowBlessings = allowBlessingsCheckBox.Checked;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}
		private void allowPerfectMayhemCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoAllowPerfectMayhem = allowPerfectMayhemCheckbox.Checked;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void allowMayhemPityCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoAllowMayhemPity = allowMayhemPityCheckBox.Checked;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void allowMayhemRageCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoAllowMayhemRage = allowMayhemRageCheckBox.Checked;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void cmdConsistencyComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoCommandConsistency = autoCommandConsistencyComboBox.SelectedIndex;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void cmdSpeedComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.autoCommandSpeed = autoCommandSpeedComboBox.SelectedIndex;
			setCustomDifficulty();
			KhaosSettingsPanel_Load(sender, e);
		}

		private void blessingsWeightTextBox_Validated(object sender, EventArgs e)
		{
			int weight;
			bool result = Int32.TryParse(blessingsWeightTextBox.Text, out weight);
			if (result)
			{
				toolConfig.Khaos.autoBlessingWeight = weight;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			blessingsWeightTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void blessingsWeightTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int weight;
			bool result = Int32.TryParse(blessingsWeightTextBox.Text, out weight);
			if (!result || weight < 0 || weight > 21)
			{
				this.blessingsWeightTextBox.Text = "";
				this.blessingsWeightTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(blessingsWeightTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void neutralsWeightTextBox_Validated(object sender, EventArgs e)
		{
			int weight;
			bool result = Int32.TryParse(neutralsWeightTextBox.Text, out weight);
			if (result)
			{
				toolConfig.Khaos.autoNeutralWeight = weight;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			neutralsWeightTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void neutralsWeightTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int weight;
			bool result = Int32.TryParse(neutralsWeightTextBox.Text, out weight);
			if (!result || weight < 0 || weight > 21)
			{
				this.neutralsWeightTextBox.Text = "";
				this.neutralsWeightTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(neutralsWeightTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void cursesWeightTextBox_Validated(object sender, EventArgs e)
		{
			int weight;
			bool result = Int32.TryParse(cursesWeightTextBox.Text, out weight);
			if (result)
			{
				toolConfig.Khaos.autoCurseWeight = weight;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			cursesWeightTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void cursesWeightTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int weight;
			bool result = Int32.TryParse(cursesWeightTextBox.Text, out weight);
			if (!result || weight < 0 || weight > 21)
			{
				this.cursesWeightTextBox.Text = "";
				this.cursesWeightTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(neutralsWeightTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void blessingsMoodTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = blessingsMoodTextBox.Text.Replace("%", "");
			int mood;
			bool result = Int32.TryParse(boxText, out mood);
			if (result)
			{
				toolConfig.Khaos.autoBlessingMood = (int) (mood);
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			blessingsMoodTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void blessingsMoodTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = blessingsMoodTextBox.Text.Replace("%", "");
			int mood;
			bool result = Int32.TryParse(boxText, out mood);
			if (!result || mood < 0 || mood > 21)
			{
				this.blessingsMoodTextBox.Text = "";
				this.blessingsMoodTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(cursesMoodTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void neutralsMoodTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = neutralsMoodTextBox.Text.Replace("%", "");
			int mood;
			bool result = Int32.TryParse(boxText, out mood);
			if (result)
			{
				toolConfig.Khaos.autoNeutralMood = (int) (mood);
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			neutralsMoodTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void neutralsMoodTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = neutralsMoodTextBox.Text.Replace("%", "");
			int mood;
			bool result = Int32.TryParse(boxText, out mood);
			if (!result || mood < 0 || mood > 21)
			{
				this.neutralsMoodTextBox.Text = "";
				this.neutralsMoodTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(cursesMoodTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void cursesMoodTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = cursesMoodTextBox.Text.Replace("%", "");
			int mood;
			bool result = Int32.TryParse(boxText, out mood);
			if (result)
			{
				toolConfig.Khaos.autoCurseMood = (int) (mood);
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			cursesMoodTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void cursesMoodTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = cursesMoodTextBox.Text.Replace("%", "");
			int mood;
			bool result = Int32.TryParse(boxText, out mood);
			if (!result || mood < 0 || mood > 21)
			{
				this.cursesMoodTextBox.Text = "";
				this.cursesMoodTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(cursesMoodTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void blessingsMinTextBox_Validated(object sender, EventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(blessingsMinTextBox.Text, out amount);
			if (result)
			{
				toolConfig.Khaos.autoBlessingMax = amount;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			blessingsMinTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void blessingsMinTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(blessingsMinTextBox.Text, out amount);
			if (!result || amount < 0 || amount > 10)
			{
				this.blessingsMinTextBox.Text = "";
				this.blessingsMinTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(blessingsMinTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void neutralsMinTextBox_Validated(object sender, EventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(neutralsMinTextBox.Text, out amount);
			if (result)
			{
				toolConfig.Khaos.autoNeutralMax = amount;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			neutralsMinTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void neutralsMinTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(neutralsMinTextBox.Text, out amount);
			if (!result || amount < 0 || amount > 10)
			{
				this.neutralsMinTextBox.Text = "";
				this.neutralsMinTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(neutralsMinTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void cursesMinTextBox_Validated(object sender, EventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(cursesMinTextBox.Text, out amount);
			if (result)
			{
				toolConfig.Khaos.autoCurseMax = amount;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			cursesMinTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void cursesMinTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(cursesMinTextBox.Text, out amount);
			if (!result || amount < 0 || amount > 10)
			{
				this.cursesMinTextBox.Text = "";
				this.cursesMinTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(cursesMinTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void blessingsMaxTextBox_Validated(object sender, EventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(blessingsMaxTextBox.Text, out amount);
			if (result)
			{
				toolConfig.Khaos.autoBlessingMax = amount;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			blessingsMaxTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void blessingsMaxTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(blessingsMaxTextBox.Text, out amount);
			if (!result || amount < 0 || amount > 10)
			{
				this.blessingsMaxTextBox.Text = "";
				this.blessingsMaxTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(blessingsMaxTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void neutralsMaxTextBox_Validated(object sender, EventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(neutralsMaxTextBox.Text, out amount);
			if (result)
			{
				toolConfig.Khaos.autoNeutralMax = amount;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			neutralsMaxTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void neutralsMaxTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(neutralsMaxTextBox.Text, out amount);
			if (!result || amount < 0 || amount > 10)
			{
				this.neutralsMaxTextBox.Text = "";
				this.neutralsMaxTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(neutralsMaxTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void cursesMaxTextBox_Validated(object sender, EventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(cursesMaxTextBox.Text, out amount);
			if (result)
			{
				toolConfig.Khaos.autoCurseMax = amount;
				setCustomDifficulty();
				KhaosSettingsPanel_Load(sender, e);
			}
			cursesMaxTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void cursesMaxTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int amount;
			bool result = Int32.TryParse(cursesMaxTextBox.Text, out amount);
			if (!result || amount < 0 || amount > 10)
			{
				this.cursesMaxTextBox.Text = "";
				this.cursesMaxTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(cursesMaxTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void autoMayhemGridView_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
		{

		}

		private void allowNeutralLevelResetCheckbox_CheckedChanged(object sender, EventArgs e)
		{

		}
	}
}
