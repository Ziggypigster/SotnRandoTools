﻿using System;
using System.Linq;
using BizHawk.Client.Common;
using BizHawk.Emulation.Common;

namespace SotnRandoTools.Services.Adapters
{
	public class CheatCollectionAdapter : ICheatCollectionAdapter
	{
		private readonly CheatCollection cheats;
		private readonly IMemoryDomains domains;
		public CheatCollectionAdapter(CheatCollection cheats, IMemoryDomains domains)
		{
			if (cheats is null) throw new ArgumentNullException(nameof(cheats));
			if (domains is null) throw new ArgumentNullException(nameof(domains));
			this.cheats = cheats;
			this.domains = domains;
		}

		public Cheat this[int index]
		{
			get
			{
				return cheats[index];
			}
		}

		public bool Load(string path, bool append)
		{
			return cheats.Load(domains, path, append);
		}

		public void DisableAll()
		{
			cheats.DisableAll();
		}

		public Cheat GetCheatByName(string name)
		{
			return cheats.Where(x => x.Name == name).FirstOrDefault();
		}

		public void AddCheat(long address, int value, string name, WatchSize size)
		{
			if (String.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name));

			var watch = Watch.GenerateWatch(domains.MainMemory, address, size, WatchDisplayType.Hex, true, name);
			var cheat = new BizHawk.Client.Common.Cheat(watch, value);
			cheats.Add(cheat);
		}

		public void RemoveCheat(Cheat cheat)
		{
			cheats.Remove(cheat);
		}

		public bool Load(IMemoryDomains domains, string path, bool append)
		{
			throw new NotImplementedException();
		}
	}
}
