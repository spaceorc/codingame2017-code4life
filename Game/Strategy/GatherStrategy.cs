using System.Collections.Generic;
using System.Linq;
using Game.State;
using Game.Types;

namespace Game.Strategy
{
	public class GatherStrategy : RobotStrategyBase
	{
		private readonly GameState gameState;

		public GatherStrategy(GameState gameState)
		{
			this.gameState = gameState;
		}

		public override IRobotStrategy Process(TurnState turnState)
		{
			var additionalExpertise = new MoleculeSet();
			var usedMolecules = new MoleculeSet();

			var samples = turnState.carriedSamples.ToList();

			var producedSamples = new List<Sample>();
			while (true)
			{
				var canProduceSample = samples.FirstOrDefault(x => turnState.robot.CanProduce(x, additionalExpertise, usedMolecules));
				if (canProduceSample == null)
					break;
				usedMolecules = usedMolecules.Add(turnState.robot.GetCost(canProduceSample, additionalExpertise));
				additionalExpertise = additionalExpertise.Gain(canProduceSample.gain);
				samples = samples.Except(new[]{ canProduceSample }).ToList();
				producedSamples.Add(canProduceSample);
			}

			var canGatherSample = samples.OrderByDescending(x => x.health).FirstOrDefault(x => turnState.robot.CanGather(turnState, x, additionalExpertise, usedMolecules));
			if (canGatherSample != null)
			{
				if (turnState.robot.GoTo(ModuleType.MOLECULES) == GoToResult.Arrived)
					turnState.robot.Connect(GetMoleculeToGather(turnState, canGatherSample, additionalExpertise, usedMolecules));
				return null;
			}

			if (turnState.robot.storage.TotalCount < Constants.MAX_STORAGE)
			{
				canGatherSample = samples.OrderByDescending(x => x.health).FirstOrDefault(x => turnState.robot.CanGather(turnState, x, additionalExpertise, usedMolecules, recycle: true));
				if (canGatherSample != null)
				{
					var ungatheredType = GetMoleculeToGather(turnState, canGatherSample, additionalExpertise, usedMolecules);
					if (ungatheredType != MoleculeType.Unknown)
					{
						if (turnState.robot.GoTo(ModuleType.MOLECULES) == GoToResult.Arrived)
							turnState.robot.Connect(ungatheredType);
						return null;
					}
				}
			}

			if (producedSamples.Any())
				return new ProduceStrategy(gameState);

			return new DropStrategy(gameState);
		}

		private static MoleculeType GetMoleculeToGather(TurnState turnState, Sample sample, MoleculeSet additionalExpertise = null, MoleculeSet usedMolecules = null)
		{
			var moleculesToGather = turnState.robot.GetMoleculesToGather(sample, additionalExpertise, usedMolecules);
			var minAvailable = int.MaxValue;
			var result = MoleculeType.Unknown;
			for (var i = 0; i < moleculesToGather.counts.Length; i++)
			{
				if (moleculesToGather.counts[i] > 0)
				{
					if (turnState.available.counts[i] < minAvailable && turnState.available.counts[i] > 0)
					{
						result = (MoleculeType)i;
						minAvailable = turnState.available.counts[i];
					}
				}
			}
			return result;
		}
	}
}