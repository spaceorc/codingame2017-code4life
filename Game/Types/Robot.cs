using System;
using System.Linq;
using Game.State;

namespace Game.Types
{
	public class Robot
	{
		public readonly int eta;
		public readonly int[] expertise;
		public readonly int score;
		public readonly int[] storage;
		public readonly ModuleType target;

		public Robot(string target, int eta, int score, int storageA, int storageB, int storageC, int storageD, int storageE, int expertiseA, int expertiseB, int expertiseC, int expertiseD, int expertiseE)
		{
			this.target = (ModuleType)Enum.Parse(typeof (ModuleType), target);
			this.eta = eta;
			this.score = score;
			storage = new[] {storageA, storageB, storageC, storageD, storageE};
			expertise = new[] {expertiseA, expertiseB, expertiseC, expertiseD, expertiseE};
		}

		public override string ToString()
		{
			return $"{target}[eta:{eta}, score:{score}]: exp={expertise.ToMoleculesString()}, storage={storage.ToMoleculesString()}";
		}

		public GoToResult GoTo(ModuleType module)
		{
			if (target != module || eta != 0)
			{
				Console.WriteLine($"GOTO {module}");
				return GoToResult.OnTheWay;
			}
			return GoToResult.Arrived;
		}

		public void Connect(int arg)
		{
			Console.WriteLine($"CONNECT {arg}");
		}

		public void Connect(MoleculeType moleculeType)
		{
			Console.WriteLine($"CONNECT {moleculeType}");
		}

		public bool CanGather(TurnState turnState, Sample sample, int[] additionalExpertise = null, int[] usedMolecules = null, int[] recycledMolecules = null)
		{
			var extra = 0;
			for (var i = 0; i < sample.cost.Length; i++)
			{
				if (storage[i] + expertise[i] + turnState.available[i] - (usedMolecules?[i] ?? 0) + (additionalExpertise?[i] ?? 0) < sample.cost[i])
					return false;
				var extraOfType = sample.cost[i] - (storage[i] - (usedMolecules?[i] ?? 0) - (recycledMolecules?[i] ?? 0) + expertise[i] + (additionalExpertise?[i] ?? 0));
				if (extraOfType > 0)
					extra += extraOfType;
			}
			if (storage.Sum() - (recycledMolecules?.Sum() ?? 0) + extra > Constants.MAX_STORAGE)
				return false;
			return true;
		}

		public bool CanProduce(Sample sample, int[] additionalExpertise = null, int[] usedMolecules = null)
		{
			for (var i = 0; i < sample.cost.Length; i++)
			{
				if (storage[i] - (usedMolecules?[i] ?? 0) + expertise[i] + (additionalExpertise?[i] ?? 0) < sample.cost[i])
					return false;
			}
			return true;
		}
	}
}