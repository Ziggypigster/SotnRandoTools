using System;
using System.Windows.Forms;
using SotnRandoTools.Configuration.Interfaces;

namespace SotnRandoTools
{
	public partial class CoopSettingsPanel : UserControl
	{
		private readonly IToolConfig? toolConfig;

		public CoopSettingsPanel(IToolConfig toolConfig)
		{
			if (toolConfig is null) throw new ArgumentNullException(nameof(toolConfig));
			this.toolConfig = toolConfig;

			InitializeComponent();
		}

		private void MultiplayerSettingsPanel_Load(object sender, EventArgs e)
		{
			receiveMayhemCommandsCheckbox.Checked = toolConfig.Coop.ReceiveMayhemCommands;
			sendMayhemCommandsCheckbox.Checked = toolConfig.Coop.SendMayhemCommands;
			
			shareLocationsCheckbox.Checked = toolConfig.Coop.ShareLocations;
			shareRelicsCheckbox.Checked = toolConfig.Coop.ShareRelics;
			shareWarpsCheckbox.Checked = toolConfig.Coop.ShareWarps;
			sendAssistsCheckbox.Checked = toolConfig.Coop.SendAssists;
			sendItemsCheckbox.Checked = toolConfig.Coop.SendItems;
			
			saveServerCheckbox.Checked = toolConfig.Coop.StoreLastServer;
			portTextBox.Text = toolConfig.Coop.DefaultPort.ToString();
			serverTextBox.Text = toolConfig.Coop.DefaultServer;
		}

		private void portTextBox_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.DefaultPort = Int32.Parse(portTextBox.Text);
		}

		private void saveServerCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.StoreLastServer = saveServerCheckbox.Checked;
		}

		private void serverTextBox_TextChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.DefaultServer = serverTextBox.Text;
		}

		private void receiveMayhemCommandsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.ReceiveMayhemCommands = shareRelicsCheckbox.Checked;
		}

		private void sendMayhemCommandsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.SendMayhemCommands = shareRelicsCheckbox.Checked;
		}


		private void shareShortcutsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			//toolConfig.Coop.ShareShortCuts = shareShortcutsCheckbox.Checked;
		}
		private void shareLocationsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.ShareLocations = shareLocationsCheckbox.Checked;
		}
		private void shareRelicsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.ShareRelics = shareRelicsCheckbox.Checked;
		}
		private void shareWarpsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.ShareWarps = shareWarpsCheckbox.Checked;
		}
		private void sendAssistsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.SendAssists = sendAssistsCheckbox.Checked;
		}
		private void sendItemsCheckbox_CheckedChanged(object sender, EventArgs e)
		{
			toolConfig.Coop.SendItems = sendItemsCheckbox.Checked;
		}

		private void saveButton_Click(object sender, EventArgs e)
		{
			toolConfig.SaveConfig();
		}
	}
}
