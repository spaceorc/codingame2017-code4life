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
			var additionalExpertise = new int[Constants.MOLECULE_TYPE_COUNT];
			var usedMolecules = new int[Constants.MOLECULE_TYPE_COUNT];

			var samples = turnState.carriedSamples.ToList();

			var producedSamples = new List<Sample>();
			while (true)
			{
				var canProduceSample = samples.FirstOrDefault(x => turnState.robot.CanProduce(x, additionalExpertise, usedMolecules));
				if (canProduceSample == null)
					break;
				for (int i = 0; i < canProduceSample.cost.Length; i++)
				{
					var used = canProduceSample.cost[i] - turnState.robot.expertise[i] - additionalExpertise[i];
					if (used > 0)
						usedMolecules[i] += used;
				}
				additionalExpertise[(int)canProduceSample.gain]++;
				samples = samples.Except(new[]{ canProduceSample }).ToList();
				producedSamples.Add(canProduceSample);
			}

			var canGatherSample = samples.OrderByDescending(x => x.health).FirstOrDefault(x => turnState.robot.CanGather(turnState, x, additionalExpertise, usedMolecules));
			if (canGatherSample != null)
			{
				if (turnState.robot.GoTo(ModuleType.MOLECULES) == GoToResult.Arrived)
					turnState.robot.Connect(GetUngatheredType(turnState, canGatherSample, additionalExpertise, usedMolecules));
				return null;
			}

			if (turnState.robot.storage.Sum() < Constants.MAX_STORAGE)
			{
				canGatherSample = samples.OrderByDescending(x => x.health).FirstOrDefault(x => turnState.robot.CanGather(turnState, x, additionalExpertise, recycledMolecules: usedMolecules));
				if (canGatherSample != null)
				{
					var ungatheredType = GetUngatheredType(turnState, canGatherSample, additionalExpertise, usedMolecules);
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

		private static MoleculeType GetUngatheredType(TurnState turnState, Sample sample, int[] additionalExpertise = null, int[] usedMolecules = null)
		{
			var robot = turnState.robots[0];
			var minAvailable = int.MaxValue;
			var result = MoleculeType.Unknown;
			for (var i = 0; i < sample.cost.Length; i++)
			{
				if (robot.storage[i] - (usedMolecules?[i] ?? 0) + robot.expertise[i] + (additionalExpertise?[i] ?? 0) < sample.cost[i])
				{
					if (turnState.available[i] < minAvailable && turnState.available[i] > 0)
					{
						result = (MoleculeType)i;
						minAvailable = turnState.available[i];
					}
				}
			}
			return result;
		}
	}
}