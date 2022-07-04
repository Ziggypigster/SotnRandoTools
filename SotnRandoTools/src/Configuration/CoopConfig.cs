using System.Drawing;

namespace SotnRandoTools.Configuration
{
	public class CoopConfig
	{
		public CoopConfig()
		{
			ReceiveMayhemCommands = true;
			SendMayhemCommands = true;
			ShareLocations = true;
			ShareRelics = true;
			ShareWarps = true;
			SendItems = true;
			SendAssists = true;
			StoreLastServer = true;
			DefaultServer = "";
			DefaultPort = 46318;
		}

		public Point Location { get; set; }
		public bool ReceiveMayhemCommands { get; set; }
		public bool SendMayhemCommands { get; set; }
		public bool ShareLocations { get; set; }
		public bool ShareRelics { get; set; }
		public bool ShareShortcuts { get; set; }
		public bool ShareWarps { get; set; }
		public bool SendAssists;
		public bool SendItems { get; set; }
		public bool StoreLastServer { get; set; }
		public int DefaultPort { get; set; }
		public string DefaultServer { get; set; }
		public bool ConnectionReceiveMayhemCommands { get; set; }
		public bool ConnectionSendMayhemCommands { get; set; }
		public bool ConnectionShareLocations { get; set; }
		public bool ConnectionShareRelics { get; set; }
		public bool ConnectionShareWarps { get; set; }
		public bool ConnectionSendItems { get; set; }
		public bool ConnectionSendAssists { get; set; }

		public void InitiateServerSettings()
		{
			ConnectionReceiveMayhemCommands = ReceiveMayhemCommands;
			ConnectionSendMayhemCommands = SendMayhemCommands;
			ConnectionShareRelics = ShareRelics;
			ConnectionShareWarps = ShareWarps;
			ConnectionSendItems = SendItems;
			ConnectionSendAssists = SendAssists;
			ConnectionShareLocations = ShareLocations;
		}
	}
}
