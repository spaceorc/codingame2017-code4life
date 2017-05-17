using System;
using System.Collections.Generic;
using Game.State;

namespace Game.Types
{
	public class Robot
	{
		public readonly int eta;
		public readonly MoleculeSet expertise;
		public readonly int score;
		public readonly MoleculeSet storage;
		public readonly ModuleType target;
		public readonly List<Sample> samples = new List<Sample>();

		public Robot(string target, int eta, int score, int storageA, int storageB, int storageC, int storageD, int storageE, int expertiseA, int expertiseB, int expertiseC, int expertiseD, int expertiseE)
		{
			this.target = (ModuleType)Enum.Parse(typeof (ModuleType), target);
			this.eta = eta;
			this.score = score;
			storage = new MoleculeSet(storageA, storageB, storageC, storageD, storageE);
			expertise = new MoleculeSet(expertiseA, expertiseB, expertiseC, expertiseD, expertiseE);
		}

		public override string ToString()
		{
			return $"{target}[eta:{eta}, score:{score}]: exp={expertise}, storage={storage}";
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

		public MoleculeSet GetMoleculesToGather(Sample sample, MoleculeSet additionalExpertise = null, MoleculeSet usedMolecules = null)
		{
			var cost = GetCost(sample, additionalExpertise);
			return cost.Subtract(storage.Subtract(usedMolecules));
		}

		public bool CanGather(TurnState turnState, Sample sample, MoleculeSet additionalExpertise = null, MoleculeSet usedMolecules = null, bool recycle = false)
		{
			var moleculesToGather = GetMoleculesToGather(sample, additionalExpertise, usedMolecules);
			var available = turnState.available.Add(recycle ? usedMolecules : null);
			if (!available.IsSupersetOf(moleculesToGather))
				return false;
			var requiredStorage = storage.Add(moleculesToGather).Subtract(recycle ? usedMolecules : null);
			return requiredStorage.totalCount <= Constants.MAX_STORAGE;
		}

		public bool CanProduce(Sample sample, MoleculeSet additionalExpertise = null, MoleculeSet usedMolecules = null)
		{
			var moleculesCanUse = storage.Add(expertise).Add(additionalExpertise).Subtract(usedMolecules);
			return moleculesCanUse.IsSupersetOf(sample.cost);
		}

		public MoleculeSet GetCost(Sample sample, MoleculeSet additionalExpertise = null)
		{
			return sample.cost.Subtract(expertise).Subtract(additionalExpertise);
		}
	}
}