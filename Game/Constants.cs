using System;
using System.Collections.Generic;
using Game.Types;

namespace Game
{
	// pack: 2
	public static class Constants
	{
		public const int MAX_TRAY = 3;
		public const int MAX_STORAGE = 10;
		public const int MOLECULE_TYPE_COUNT = MoleculeType.E - MoleculeType.A + 1;
		public const int TOTAL_TURNS = 200 * 2;
		public const int PROJECT_HEALTH = 50;

		public static readonly Dictionary<Tuple<ModuleType, ModuleType>, int> distances = new Dictionary<Tuple<ModuleType, ModuleType>, int>
		{
			{ Tuple.Create(ModuleType.START_POS, ModuleType.SAMPLES), 2 },
			{ Tuple.Create(ModuleType.START_POS, ModuleType.DIAGNOSIS), 2 },
			{ Tuple.Create(ModuleType.START_POS, ModuleType.MOLECULES), 2 },
			{ Tuple.Create(ModuleType.START_POS, ModuleType.LABORATORY), 2 },
			{ Tuple.Create(ModuleType.SAMPLES, ModuleType.SAMPLES), 0 },
			{ Tuple.Create(ModuleType.SAMPLES, ModuleType.DIAGNOSIS), 3 },
			{ Tuple.Create(ModuleType.SAMPLES, ModuleType.MOLECULES), 3 },
			{ Tuple.Create(ModuleType.SAMPLES, ModuleType.LABORATORY), 3 },
			{ Tuple.Create(ModuleType.DIAGNOSIS, ModuleType.DIAGNOSIS), 0 },
			{ Tuple.Create(ModuleType.DIAGNOSIS, ModuleType.SAMPLES), 3 },
			{ Tuple.Create(ModuleType.DIAGNOSIS, ModuleType.MOLECULES), 3 },
			{ Tuple.Create(ModuleType.DIAGNOSIS, ModuleType.LABORATORY), 4 },
			{ Tuple.Create(ModuleType.MOLECULES, ModuleType.MOLECULES), 0 },
			{ Tuple.Create(ModuleType.MOLECULES, ModuleType.SAMPLES), 3 },
			{ Tuple.Create(ModuleType.MOLECULES, ModuleType.DIAGNOSIS), 3 },
			{ Tuple.Create(ModuleType.MOLECULES, ModuleType.LABORATORY), 3 },
			{ Tuple.Create(ModuleType.LABORATORY, ModuleType.LABORATORY), 0 },
			{ Tuple.Create(ModuleType.LABORATORY, ModuleType.SAMPLES), 3 },
			{ Tuple.Create(ModuleType.LABORATORY, ModuleType.DIAGNOSIS), 4 },
			{ Tuple.Create(ModuleType.LABORATORY, ModuleType.MOLECULES), 3 },
		};
	}
}