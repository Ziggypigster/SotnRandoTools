using System.Windows.Forms;
using SotnRandoTools.Khaos.Enums;

namespace SotnRandoTools.Khaos.Models
{
	public class QueuedAction
	{
		public QueuedAction()
		{
			Type = ActionType.Debuff;
			ChangesStats = false;
			ChangesWeapons = false;
			ChangesSubWeapons = false;
			LocksWeapons = false;
			LocksSpeed = false;
			LocksMana = false;
			LocksInvincibility = false;
			LocksHearts = false;
		}
		public string Name { get; set; }
		public ActionType Type { get; set; }
		public MethodInvoker Invoker { get; set; }
		public bool ChangesStats { get; set; }
		public bool LocksSpeed { get; set; }
		public bool LocksMana { get; set; }
		public bool LocksHearts { get; set; }
		public bool LocksWeapons { get; set; }
		public bool ChangesSubWeapons { get; set; }
		public bool ChangesWeapons { get; set; }
		public bool LocksInvincibility { get; set; }
		public bool LocksSpawning { get; set; }
	}
}
