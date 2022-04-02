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
			continuousWingSmashCheckBox.Checked = toolConfig.Khaos.ContinuousWingsmash;
			boostFamiliarsCheckBox.Checked = toolConfig.Khaos.BoostFamiliars;
			romhackModeCheckBox.Checked = toolConfig.Khaos.RomhackMode;
			allowNeutralLevelResetCheckbox.Checked = toolConfig.Khaos.AllowNeutralLevelReset;

			//Command
			blessingComboBox.SelectedIndex = toolConfig.Khaos.BlessingModifier;
			curseComboBox.SelectedIndex = toolConfig.Khaos.CurseModifier;
			neutralMinLevelTextBox.Text = toolConfig.Khaos.NeutralMinLevel.ToString();
			neutralStartLevelTextBox.Text = toolConfig.Khaos.NeutralStartLevel.ToString();
			neutralMaxLevelTextBox.Text = toolConfig.Khaos.NeutralMaxLevel.ToString();

			underwaterTextBox.Text = (toolConfig.Khaos.UnderwaterFactor * 100) + "%";
			speedTextBox.Text = (toolConfig.Khaos.SpeedFactor * 100) + "%";
			statsDownTextBox.Text = (toolConfig.Khaos.StatsDownFactor * 100) + "%";
			regenTextBox.Text = toolConfig.Khaos.RegenGainPerSecond.ToString();
			pandemoniumMinTextBox.Text = toolConfig.Khaos.PandemoniumMinItems.ToString();
			pandemoniumMaxTextBox.Text = toolConfig.Khaos.PandemoniumMaxItems.ToString();
			keepVladRelicsCheckbox.Checked = toolConfig.Khaos.KeepVladRelics;
			restrictedRelicSwapCheckBox.Checked = toolConfig.Khaos.RestrictedRelicSwap;

			crippleTextBox.Text = (toolConfig.Khaos.CrippleFactor * 100) + "%";
			hasteTextBox.Text = (toolConfig.Khaos.HasteFactor * 100) + "%";
			weakenTextBox.Text = (toolConfig.Khaos.WeakenFactor * 100) + "%";
			thirstTextBox.Text = toolConfig.Khaos.ThirstDrainPerSecond.ToString();
			pandoraMinTextBox.Text = toolConfig.Khaos.PandoraMinItems.ToString();
			pandoraMaxTextBox.Text = toolConfig.Khaos.PandoraMaxItems.ToString();

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
			}
			alertsGridView.AutoGenerateColumns = false;
			alertsGridView.DataSource = actionsAlertsSource;
			alertsGridView.CellClick += AlertsGridView_BrowseClick;
			actionsGridView.AutoGenerateColumns = false;
			actionsGridView.DataSource = actionsOtherSource;
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


		#region Legacy

		private void crippleTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = crippleTextBox.Text.Replace("%", "");
			int cripplePercentage;
			bool result = Int32.TryParse(boxText, out cripplePercentage);
			if (result)
			{
				toolConfig.Khaos.UnderwaterFactor = (cripplePercentage / 100F);
			}
			crippleTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void crippleTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = crippleTextBox.Text.Replace("%", "");
			int cripplePercentage;
			bool result = Int32.TryParse(boxText, out cripplePercentage);
			if (!result || cripplePercentage < 0 || cripplePercentage > 90)
			{
				this.crippleTextBox.Text = "";
				this.crippleTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(crippleTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void hasteTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = hasteTextBox.Text.Replace("%", "");
			int hastePercentage;
			bool result = Int32.TryParse(boxText, out hastePercentage);
			if (result)
			{
				toolConfig.Khaos.HasteFactor = (hastePercentage / 100F);
			}
			hasteTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void hasteTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = hasteTextBox.Text.Replace("%", "");
			int hastePercentage;
			bool result = Int32.TryParse(boxText, out hastePercentage);
			if (!result || hastePercentage < 100 || hastePercentage > 1000)
			{
				this.hasteTextBox.Text = "";
				this.hasteTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(speedTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void weakenTextBox_Validated(object sender, EventArgs e)
		{
			string boxText = statsDownTextBox.Text.Replace("%", "");
			int weakenPercentage;
			bool result = Int32.TryParse(boxText, out weakenPercentage);
			if (result)
			{
				toolConfig.Khaos.WeakenFactor = (weakenPercentage / 100F);
			}
			statsDownTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void weakenTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string boxText = statsDownTextBox.Text.Replace("%", "");
			int weakenPercentage;
			bool result = Int32.TryParse(boxText, out weakenPercentage);
			if (!result || weakenPercentage < 10 || weakenPercentage > 90)
			{
				this.statsDownTextBox.Text = "";
				this.statsDownTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(statsDownTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void thirstTextBox_Validated(object sender, EventArgs e)
		{
			int thirstDrain;
			bool result = Int32.TryParse(regenTextBox.Text, out thirstDrain);
			if (result)
			{
				toolConfig.Khaos.RegenGainPerSecond = (uint) thirstDrain;
			}
			thirstTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void thirstTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int thirstDrain;
			bool result = Int32.TryParse(thirstTextBox.Text, out thirstDrain);
			if (!result || thirstDrain < 1 || thirstDrain > 100)
			{
				this.thirstTextBox.Text = "";
				this.thirstTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(regenTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void pandoraMinTextBox_Validated(object sender, EventArgs e)
		{
			int pandoraMinItems;
			bool result = Int32.TryParse(pandoraMinTextBox.Text, out pandoraMinItems);
			if (result)
			{
				toolConfig.Khaos.PandemoniumMinItems = pandoraMinItems;
			}
			pandoraMinTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void pandoraMinTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			int pandoraMinItems;
			bool result = Int32.TryParse(pandoraMinTextBox.Text, out pandoraMinItems);
			if (!result || pandoraMinItems < 0 || pandoraMinItems > 100)
			{
				this.pandoraMinTextBox.Text = "";
				this.pandoraMinTextBox.BackColor = Color.Red;
				this.valueToolTip.SetToolTip(pandoraMinTextBox, "Invalid value!");
				this.valueToolTip.ToolTipIcon = ToolTipIcon.Warning;
				this.valueToolTip.Active = true;
				e.Cancel = true;
			}
		}

		private void pandoraMaxTextBox_Validated(object sender, EventArgs e)
		{
			int pandoraMaxItems;
			bool result = Int32.TryParse(pandemoniumMaxTextBox.Text, out pandoraMaxItems);
			if (result)
			{
				toolConfig.Khaos.PandoraMaxItems = pandoraMaxItems;
			}
			pandemoniumMaxTextBox.BackColor = Color.White;
			this.valueToolTip.Active = false;
		}

		private void pandoraMaxTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
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

		#region General
		private void romhackModeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.RomhackMode = romhackModeCheckBox.Checked;
		}

		private void resetToDefaultButton_Click(object sender, EventArgs e)
		{
			toolConfig.Khaos.Default();
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
		private void blessingComboBox_SelectedValueChanged(object sender, EventArgs e)
		{
			toolConfig.Khaos.BlessingModifier = blessingComboBox.SelectedIndex;
		}

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

		private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
		{

		}
	}
}
